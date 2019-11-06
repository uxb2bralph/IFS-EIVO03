using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Collections;
using System.Drawing;
using System.IO;
using Utility;
using Config;

namespace Uxnet.Web.Module.Common
{
    public class BaseWebPageUtility
    {
        //這裡用來設定Web Server的動態設定值,這些值無法固定地設在web.config中,
        //通常都是系統一啟動時,自動去檢查執行環境,並取得各種所需的屬性值.

        private static string _TempPath;

        #region 物件初始化

        static BaseWebPageUtility()
        {
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpServerUtility server = context.Server;

            //設定網站暫存資料夾
            if (String.IsNullOrEmpty(BasicConfiguration.Values["tempPath"]))
            {
                _TempPath = request.MapPath("~/temp");
            }
            else if (BasicConfiguration.Values["tempPath"].StartsWith("~/"))
            {
                _TempPath = request.MapPath(BasicConfiguration.Values["tempPath"]);
            }
            else
            {
                _TempPath = BasicConfiguration.Values["tempPath"];
            }

            if (!Directory.Exists(_TempPath))
            {
                Directory.CreateDirectory(_TempPath);
            }

        }
        #endregion


        #region AssignValue
        public static void AssignValue(DataRow row, Control control, DataColumn column)
        {
            string str;
            switch (control.GetType().Name)
            {
                case "Label":
                    str = ((Label)control).Text.Trim();
                    if (!String.IsNullOrEmpty(str))
                        row[control.ID] = Convert.ChangeType(str, column.DataType);
                    break;
                case "HtmlInputText":
                case "HtmlInputHidden":
                    str = ((HtmlInputControl)control).Value.Trim();
                    if (!String.IsNullOrEmpty(str))
                        row[control.ID] = Convert.ChangeType(str, column.DataType);
                    break;
                case "HtmlSelect":
                    str = ((HtmlSelect)control).Value;
                    if (!String.IsNullOrEmpty(str))
                        row[control.ID] = Convert.ChangeType(str, column.DataType);
                    break;
                case "HtmlInputRadioButton":
                    row[control.ID] = Convert.ChangeType(((HtmlInputRadioButton)control).Checked, column.DataType);
                    break;
                case "RadioButton":
                    row[control.ID] = Convert.ChangeType(((RadioButton)control).Checked, column.DataType);
                    break;
                case "HtmlTextArea":
                    row[control.ID] = Convert.ChangeType(((HtmlTextArea)control).Value, column.DataType);
                    break;
                case "TextBox":
                    if (!String.IsNullOrEmpty(((TextBox)control).Text))
                        row[control.ID] = Convert.ChangeType(((TextBox)control).Text, column.DataType);
                    break;
                case "DropDownList":
                    row[control.ID] = Convert.ChangeType(((DropDownList)control).SelectedValue, column.DataType);
                    break;
                case "Literal":
                    str = ((Literal)control).Text.Trim();
                    if (!String.IsNullOrEmpty(str))
                        row[control.ID] = Convert.ChangeType(str, column.DataType);
                    break;
                case "CalendarInput_ascx":
                    if (((CalendarInput)control).HasValue)
                    {
                        row[control.ID] = ((CalendarInput)control).DateTimeValue;
                    }
                    break;
                case "SelectHelper_ascx":
                    str = ((SelectHelper)control).Value;
                    if (!String.IsNullOrEmpty(str))
                        row[control.ID] = Convert.ChangeType(str, column.DataType);
                    break;

            }

        }
        #endregion

        #region AssignValue
        public static void AssignValue(DataRow row, Control control)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                Control ctrl = control.FindControl(column.ColumnName);
                if (ctrl != null)
                {
                    AssignValue(row, ctrl, column);
                }
            }
        }
        #endregion

        #region AssignValue
        public static void AssignValue(IDictionary paramValue, Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                string str;
                switch (ctrl.GetType().Name)
                {
                    case "Label":
                        str = ((Label)ctrl).Text.Trim();
                        if (!String.IsNullOrEmpty(str))
                            paramValue[ctrl.ID] = str;
                        break;
                    case "HtmlInputText":
                    case "HtmlInputHidden":
                        str = ((HtmlInputControl)ctrl).Value.Trim();
                        if (!String.IsNullOrEmpty(str))
                        {
                            paramValue[ctrl.ID] = str;
                        }
                        break;
                    case "HtmlSelect":
                        paramValue[ctrl.ID] = ((HtmlSelect)ctrl).Value;
                        break;
                    case "HtmlInputRadioButton":
                        paramValue[ctrl.ID] = ((HtmlInputRadioButton)ctrl).Checked;
                        break;
                    case "RadioButton":
                        paramValue[ctrl.ID] = ((RadioButton)ctrl).Checked;
                        break;
                    case "CalendarInput_ascx":
                        if (((CalendarInput)ctrl).HasValue)
                        {
                            paramValue[ctrl.ID] = ((CalendarInput)control).DateTimeValue;
                        }
                        break;
                    //					case "CalendarText_ascx":
                    //						if(!String.IsNullOrEmpty(((level_1.CalendarText)control).Text))
                    //						{
                    //							row[control.ID] = ((level_1.CalendarText)control).DateTimeValue;
                    //						}
                    //						break;
                    case "HtmlTextArea":
                        str = ((HtmlTextArea)ctrl).Value.Trim();
                        if (!String.IsNullOrEmpty(str))
                            paramValue[ctrl.ID] = str;
                        break;
                    case "TextBox":
                        str = ((TextBox)ctrl).Text.Trim();
                        if (!String.IsNullOrEmpty(str))
                            paramValue[ctrl.ID] = str;
                        break;
                    case "DropDownList":
                        paramValue[ctrl.ID] = ((DropDownList)ctrl).SelectedValue;
                        break;
                    case "Literal":
                        str = ((Literal)ctrl).Text.Trim();
                        if (!String.IsNullOrEmpty(str))
                            paramValue[ctrl.ID] = str;
                        break;
                    case "SelectHelper_ascx":
                        str = ((SelectHelper)ctrl).Value;
                        if (!String.IsNullOrEmpty(str))
                            paramValue[ctrl.ID] = str;
                        break;

                }

            }

        }
        #endregion


        #region WebTempPath

        public static string WebTempPath
        {
            get
            {
                return _TempPath;
            }
        }

        #endregion

        public static void StartUp()
        {
            //object obj = _webUtility;
        }

        public static void PutTempObject(object obj)
        {
            System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
            session[session.SessionID] = obj;
        }

        public static void RemoveTempObject()
        {
            System.Web.SessionState.HttpSessionState session = HttpContext.Current.Session;
            session.Remove(session.SessionID);
        }



        public static string CreateObjectClientDeclaration(Control control)
        {
            return String.Format("var {0} = document.all('{1}');", control.ID, control.ClientID);
        }

        public static void CreateObjectClientDeclarationWithScript(Control control)
        {
            control.Page.ClientScript.RegisterStartupScript(control.GetType(), control.ID, String.Format("var {0} = document.all('{1}');", control.ID, control.ClientID), true);
        }

        public static string CreateObjectClientDeclaration(string id, string clientID)
        {
            return String.Format("var {0} = document.all('{1}');", id, clientID);
        }

        public static void CreateObjectClientDeclarationFunctionWithScript(Control control)
        {
            control.Page.ClientScript.RegisterClientScriptBlock(control.GetType(), control.ID, String.Format("function {0}() {{ return document.all('{1}');}}", control.ID, control.ClientID), true);
        }

        public static void GetInputValue(Control owner, DataRow rowToSave)
        {

            foreach (DataColumn column in rowToSave.Table.Columns)
            {
                Control control = owner.FindControl(column.ColumnName);
                if (control != null)
                {
                    AssignValue(rowToSave, control, column);
                }
            }

        }


        //		public static object InitializeControlAll(object target,DataRow row)
        //		{
        //			string str;
        //			switch(target.GetType().Name)
        //			{
        //				case "Label":
        //					Label lab = (Label)target;					
        //					return lab.ID;
        //				case "HtmlInputText":
        //					HtmlInputText objTxt = (HtmlInputText)target;					
        //					return objTxt.ID;
        //					//				case "HtmlSelect":
        //					//					Label lab = (Label)target;					
        //					//					return lab.ID;
        //					//					break;
        //					//				case "HtmlInputRadioButton":                    
        //					//					Label lab = (Label)target;					
        //					//					return lab.ID;
        //					//					break;
        //					//				case "RadioButton":                    
        //					//					Label lab = (Label)target;					
        //					//					return lab.ID;
        //					//					break;
        //					//				case "CalendarInput_ascx":
        //					//					if(!String.IsNullOrEmpty(((level_1.CalendarInput)control).Text))
        //					//					{
        //					//						row[control.ID] = ((level_1.CalendarInput)control).DateTimeValue;
        //					//					}
        //					//					break;
        //				case "HtmlTextArea":
        //					HtmlTextArea objTxtA = (HtmlTextArea)target;					
        //					return objTxtA.ID;
        //				case "TextBox":
        //					TextBox objText = (TextBox)target;					
        //					return objText.ID;
        //					//				case "DropDownList":
        //					//					TextBox objTxt = (TextBox)target;					
        //					//					return objTxt.ID;
        //					//					break;
        //					//				case "HtmlInputCheckBox":
        //					//					TextBox objTxt = (TextBox)target;					
        //					//					return objTxt.ID;
        //					//					break;
        //					//				case "RadioButton":
        //					//					RadioButton RadioBox = (RadioButton)target;
        //					//					return Convert.ToInt32(row[RadioBox.ID])>0;
        //					//					break;
        //				default:
        //					return null;
        //			}
        //			
        //		}
        //

        public static void DisplayDifference(Control control, Control scope, DataRow current, DataRow target, Color dispColor)
        {
            DataColumnCollection cols0 = current.Table.Columns;
            DataColumnCollection cols1 = target.Table.Columns;

            string columnName;

            foreach (DataColumn col in cols0)
            {
                columnName = col.ColumnName;
                if (cols1.Contains(col.ColumnName) && !current[columnName].Equals(target[columnName]))
                {
                    Control webCtrl = control.FindControl(columnName);
                    if (null != webCtrl && webCtrl is WebControl && scope.Controls.Contains(webCtrl))
                    {
                        ((WebControl)webCtrl).ForeColor = dispColor;
                    }
                }
            }
        }

        public static object InitializeControl(object target, DataRow row)
        {
            if (target is HtmlInputCheckBox)
            {
                HtmlInputCheckBox chkbox = (HtmlInputCheckBox)target;
                chkbox.Checked = Convert.ToInt32(row[chkbox.ID]) > 0;
                return chkbox.ID;
            }
            else if (target is RadioButton) //HtmlInputRadioButton
            {
                RadioButton RadioBox = (RadioButton)target;
                return Convert.ToInt32(row[RadioBox.ID]) > 0;
            }
            return null;
        }

        #region 由DataRow檢查初始化CheckBox
        public static object InitChecked(object target, DataRow row)
        {
            if (target is HtmlInputCheckBox)
            {
                HtmlInputCheckBox chkbox = (HtmlInputCheckBox)target;
                chkbox.Checked = Convert.ToInt32(row[chkbox.ID]) > 0;
                return chkbox.ID;
            }
            return null;
        }

        #endregion

        #region 由DataRow檢查初始化HtmlSelect
        public static object InitSelect(object target, DataRow row)
        {
            if (target is HtmlSelect)
            {
                HtmlSelect select = (HtmlSelect)target;
                if (select.SelectedIndex < 0 && select.Items.Count > 0)
                    select.SelectedIndex = 0;

                for (int i = 0; i < select.Items.Count; i++)
                {
                    if (row[select.ID].Equals(select.Items[i].Text))
                    {
                        select.SelectedIndex = i;
                        break;
                    }
                }
                return select.ID;
            }
            return null;
        }
        #endregion

    }

    public static partial class ExtensionMathods
    {
        public static String GetInputValue(this HttpRequest request,Control control)
        {
            return request[control.UniqueID];
        }
    }

    public class TemplateContainer : Control, INamingContainer
    {

    }

}
