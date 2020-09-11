using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Windows.Forms;
using DataSystemsConversionTool.Helper;
using DataSystemsConversionTool.Models;
using DataSystemsConversionTool.Properties;
using Microsoft.SqlServer.Dts.Runtime;
using Model.Helper;
using Model.Locale;
using Model.Models.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace DataSystemsConversionTool
{
    public partial class OpenFileDialogForm : Form
    {


        private Button selectButton;
        private OpenFileDialog openFileDialog1;
        private TextBox textBox1;

        public OpenFileDialogForm()
        {
            Init();
        }

        private void Init()
        {           
            this.Text = "鼎新資料轉入大平台小工具";
            openFileDialog1 = new OpenFileDialog();

            selectButton = new Button
            {
                Size = new Size(100, 20),
                Location = new Point(15, 15),
                Text = "Select file"
            };
            selectButton.Click += new EventHandler(SelectButton_Click);
            textBox1 = new TextBox
            {
                Size = new Size(300, 300),
                Location = new Point(15, 40),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical               
            };
            ClientSize = new Size(330, 360);
            Controls.Add(selectButton);
            Controls.Add(textBox1);
        }

        private void SetText(string msg)
        {
            if (textBox1.Text != "")
            {
                textBox1.Text = textBox1.Text + "\r\n" + msg;
            }
            else
                textBox1.Text += msg;
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {            
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //var sr = new StreamReader(openFileDialog1.FileName);

                    //Step1.取得Source Path
                    var projectPath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

                    var model = new DataSystemsConversionModel
                    {
                        SourceInvoiceDataDtsx = Path.Combine(projectPath, Settings.Default.SourceInvoiceDataDtsx),
                        SourceInvoiceDetailDataDtsx = Path.Combine(projectPath, Settings.Default.SourceInvoiceDetailDataDtsx),
                        ERPFileName = Path.Combine(projectPath, $"{ Settings.Default.ERPFileName}.xlsx"),
                        InvoiceRequestTemp = Path.Combine(System.Windows.Forms.Application.StartupPath, $"{Settings.Default.InvoiceRequestTemp}.xlsx"),
                        InvoiceRequestSample = Path.Combine(System.Windows.Forms.Application.StartupPath, $"{Settings.Default.InvoiceRequestSample}.xlsx"),
                        DestinationFileName = Path.Combine(System.Windows.Forms.Application.StartupPath, $"{Settings.Default.DestinationFileName}.xlsx")
                    };

                    //Step2.先刪再copyERP
                    if (File.Exists(model.ERPFileName))
                    {
                        File.Delete(model.ERPFileName);
                    }
                    File.Copy(openFileDialog1.FileName, model.ERPFileName);

                    //Step3.先刪再copy，將InvoiceRequestSample copy一份變成 InvoiceRequestTemp
                    if (File.Exists(model.InvoiceRequestTemp))
                    {
                        File.Delete(model.InvoiceRequestTemp);
                    }
                    File.Copy(model.InvoiceRequestSample, model.InvoiceRequestTemp);

                    //Step4.刪除ERP.excel前兩列
                    var result = model.ERPFileName.DelExcelRow(0, 2);
                    this.SetText(result);

                    //Step5.執行發票主頁明細頁轉換;                    
                    result = this.ConvertFile(model.SourceInvoiceDataDtsx, 1);
                    result = result + "\r\n" + this.ConvertFile(model.SourceInvoiceDetailDataDtsx, 2);
                    this.SetText(result);
                    if (result.Contains("Failure"))
                    {
                        return;
                    }

                    //Step6.若成功的話，補齊大平台資料;  
                    result = model.InvoiceRequestTemp.ExcelDataConversion();
                    this.SetText(result);
                    try
                    {
                        //Step7.搬移檔案至大平台;   
                        string processType = Settings.Default.ProcessType;// int processType = (int)Naming.InvoiceProcessType.A0401_Xlsx_Allocation_ByIssuer;
                        var agentID = Settings.Default.AgentID;
                        var uploadFile = $@"{Settings.Default.UploadFileUrl}?";
                        var sd = Settings.Default.Sender;
                        WebClient client = new WebClient();
                        string myFile = $@"{model.InvoiceRequestTemp}";
                        client.Credentials = CredentialCache.DefaultCredentials;
                        byte[] responseArray = client.UploadFile($"{uploadFile}KeyID=&AgentID={agentID}&ProcessType={processType}&Sender={sd}", "POST", myFile);

                        var returnValue = JObject.Parse(System.Text.Encoding.ASCII.GetString(responseArray));
                        if (((JValue)returnValue["result"]).Value.ToString() == "False")
                        {
                            SetText($"傳送至大平台失敗! 錯誤代碼：{((JValue)((JContainer)returnValue["errorCode"]).First).Value}。");
                        }
                        else
                        {
                            SetText("已傳送至大平台!");
                        }

                        client.Dispose();                       
                        
                    }
                    catch (Exception ex)
                    {
                        SetText($"Error message: {ex.Message}\n\n" +
                   $"Details:\n\n{ex.StackTrace}");
                    }

                    //Step8.保留一份檔案在Daliy資料夾，重新命名為當天日期
                    var target = DateTime.Today.DailyStorePath(DateTime.Now.Ticks + "_" + Path.GetFileName(model.DestinationFileName), out string path);
                    File.Move(model.InvoiceRequestTemp, target);
                    target = DateTime.Today.DailyStorePath(DateTime.Now.Ticks + "_" + Path.GetFileName(model.ERPFileName), out string path1);
                    File.Move(model.ERPFileName, target);

                }
                catch (SecurityException ex)
                {
                    SetText($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                    //MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    //$"Details:\n\n{ex.StackTrace}");
                }
            }
        }



        private string ConvertFile(string path, int type)
        {
            Microsoft.SqlServer.Dts.Runtime.Application app = new Microsoft.SqlServer.Dts.Runtime.Application();
            Package package = null;

            string msg = string.Empty;

            try
            {
                //指定絕對路徑給 LoadPackage
                package = app.LoadPackage(path, null);

                Microsoft.SqlServer.Dts.Runtime.DTSExecResult results = package.Execute();

                if (type == 1)
                {
                    msg = "執行 Invoice SSIS 狀態：" + results.ToString();
                    //MessageBox.Show("執行 Invoice SSIS 狀態：" + results.ToString());
                }
                else
                {
                    msg = "執行 Detail SSIS 狀態：" + results.ToString();
                    //MessageBox.Show("執行 Detail SSIS 狀態：" + results.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                package.Dispose();
                package = null;
            }

            return msg;
        }

        private void OpenFileDialogForm_Load(object sender, EventArgs e)
        {

        }
    }
}
