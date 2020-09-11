using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Security;
using DataSystemsConversionTool.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace DataSystemsConversionTool.Helper
{
    public static class UploadDataExtensions
    {
        /// <summary>
        /// 執行轉換.bat
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static String ConvertFileExeBatch(this DataSystemsConversionModel model)
        {
            //BatchFileName = "InputInvoiceRequestConvert.bat"
            String args = $"{model.BatchFileName} {model.SourceInvoiceDataDtsx} {model.InvoiceRequestTemp} {model.SourceInvoiceDetailDataDtsx} {model.DestinationFileName}";
            var proc = $"{model.BatchFileName}".RunBatch(args);
            proc.WaitForExit();
            return args;
        }       

        public static Process RunBatch(this String batchFileName, String args)
        {
            //Logger.Info($"{batchFileName} {args}");
            ProcessStartInfo info = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, batchFileName), args)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            return Process.Start(info);
        }

        /// <summary>
        /// 匯入副檔名為.xlsx的Excel檔(Office 2007)
        /// </summary>
        public static DataSet ImportXLSX(string fuExcel)
        {

            XSSFWorkbook workbook = null;
            XSSFSheet sheet = null;
            DataSet ds = new DataSet();

            try
            {
                using (FileStream files = new FileStream(fuExcel, FileMode.Open, FileAccess.Read))
                {
                    #region 讀Excel檔，逐行寫入DataTable
                    workbook = new XSSFWorkbook(files); //只能讀取 System.IO.Stream 
                                                        //FileContent 屬性會取得指向要上載之檔案的 Stream 物件。這個屬性可以用於存取檔案的內容 (做為位元組)。 
                                                        //   例如，您可以使用 FileContent 屬性傳回的 Stream 物件，將檔案的內容做為位元組進行讀取並將其以位元組陣列儲存。 
                                                        //FileContent 屬性，型別：System.IO.Stream 

                    sheet = (XSSFSheet)workbook.GetSheetAt(0);   //0表示：第一個 worksheet工作表
                    DataTable dt = new DataTable();

                    XSSFRow headerRow = (XSSFRow)sheet.GetRow(0);   //Excel 表頭列


                    for (int colIdx = 0; colIdx <= headerRow.LastCellNum; colIdx++) //表頭列，共有幾個 "欄位"?（取得最後一欄的數字） 
                    {
                        if (headerRow.GetCell(colIdx) != null)
                            dt.Columns.Add(new DataColumn(headerRow.GetCell(colIdx).StringCellValue));
                        //欄位名有折行時，只取第一行的名稱做法是headerRow.GetCell(colIdx).StringCellValue.Replace("\n", ",").Split(',')[0]
                    }

                    //For迴圈的「啟始值」為1，表示不包含 Excel表頭列
                    for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)   //每一列做迴圈
                    {
                        XSSFRow exlRow = (XSSFRow)sheet.GetRow(rowIdx); //不包含 Excel表頭列的 "其他資料列"
                        DataRow newDataRow = dt.NewRow();

                        for (int colIdx = exlRow.FirstCellNum; colIdx <= exlRow.LastCellNum; colIdx++)   //每一個欄位做迴圈
                        {
                            //每一個欄位，都加入同一列 DataRow
                            if (exlRow.GetCell(colIdx) != null)
                            {
                                switch (exlRow.GetCell(colIdx).CellType)
                                {
                                    case CellType.String:
                                        newDataRow[colIdx] = exlRow.GetCell(colIdx).ToString();
                                        break;
                                    case CellType.Numeric:
                                        newDataRow[colIdx] = exlRow.GetCell(colIdx);
                                        break;
                                    case CellType.Boolean:
                                        exlRow.GetCell(colIdx).SetCellType(CellType.Boolean);
                                        newDataRow[colIdx] = exlRow.GetCell(colIdx).BooleanCellValue;
                                        break;
                                    case CellType.Error:
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        dt.Rows.Add(newDataRow);
                    }

                    ds.Tables.Add(dt);

                    #endregion 讀Excel檔，逐行寫入DataTable

                }
            }
            catch (Exception err)
            {

                throw err;
            }
            finally
            {
                //釋放 NPOI的資源
                workbook = null;
                sheet = null;
            }

            return ds;
        }

        /// <summary>
        /// 匯入副檔名為.xls的Excel檔
        /// </summary>
        public static DataSet ImportXLS(string path, string fuExcel = "")
        {
            HSSFWorkbook workbook = null;
            HSSFSheet sheet = null;
            DataSet ds = new DataSet();

            try
            {
                using (FileStream files = new FileStream(fuExcel, FileMode.Open, FileAccess.Read))
                {
                    #region 讀Excel檔，逐行寫入DataTable
                    workbook = new HSSFWorkbook(files); //只能讀取 System.IO.Stream 
                                                        //FileContent 屬性會取得指向要上載之檔案的 Stream 物件。這個屬性可以用於存取檔案的內容 (做為位元組)。 
                                                        //   例如，您可以使用 FileContent 屬性傳回的 Stream 物件，將檔案的內容做為位元組進行讀取並將其以位元組陣列儲存。 
                                                        //FileContent 屬性，型別：System.IO.Stream 

                    sheet = (HSSFSheet)workbook.GetSheetAt(0);   //0表示：第一個 worksheet工作表
                    DataTable dt = new DataTable();

                    HSSFRow headerRow = (HSSFRow)sheet.GetRow(0);   //Excel 表頭列

                    for (int colIdx = 0; colIdx <= headerRow.LastCellNum; colIdx++) //表頭列，共有幾個 "欄位"?（取得最後一欄的數字） 
                    {
                        if (headerRow.GetCell(colIdx) != null)
                            dt.Columns.Add(new DataColumn(headerRow.GetCell(colIdx).StringCellValue));
                        //欄位名有折行時，只取第一行的名稱做法是headerRow.GetCell(colIdx).StringCellValue.Replace("\n", ",").Split(',')[0]
                    }

                    //For迴圈的「啟始值」為1，表示不包含 Excel表頭列
                    for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)   //每一列做迴圈
                    {
                        HSSFRow exlRow = (HSSFRow)sheet.GetRow(rowIdx); //不包含 Excel表頭列的 "其他資料列"
                        DataRow newDataRow = dt.NewRow();

                        for (int colIdx = exlRow.FirstCellNum; colIdx <= exlRow.LastCellNum; colIdx++)   //每一個欄位做迴圈
                        {
                            //每一個欄位，都加入同一列 DataRow
                            if (exlRow.GetCell(colIdx) != null)
                            {
                                switch (exlRow.GetCell(colIdx).CellType)
                                {
                                    case CellType.String:
                                        newDataRow[colIdx] = exlRow.GetCell(colIdx).ToString();
                                        break;
                                    case CellType.Numeric:
                                        newDataRow[colIdx] = exlRow.GetCell(colIdx);
                                        break;
                                    case CellType.Boolean:
                                        exlRow.GetCell(colIdx).SetCellType(CellType.Boolean);
                                        newDataRow[colIdx] = exlRow.GetCell(colIdx).BooleanCellValue;
                                        break;
                                    case CellType.Error:
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        dt.Rows.Add(newDataRow);
                    }

                    #endregion 讀Excel檔，逐行寫入DataTable
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                //釋放 NPOI的資源
                workbook = null;
                sheet = null;
            }

            return ds;
        }

        public static string ExcelDataConversion(this string path)
        {try
            {
                XSSFSheet sheet = null;
                using (FileStream fsIn = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                {
                    //開啟Excel
                    XSSFWorkbook workbook = new XSSFWorkbook(fsIn);
                    fsIn.Close();

                    //取得Main工作表                        
                    sheet = (XSSFSheet)workbook.GetSheetAt(0);

                    for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)   //每一列做迴圈
                    {
                        XSSFRow exlRow = (XSSFRow)sheet.GetRow(rowIdx);

                        for (int colIdx = exlRow.FirstCellNum; colIdx <= exlRow.LastCellNum; colIdx++)   //每一個欄位做迴圈
                        {
                            XSSFCell exlCell = (XSSFCell)exlRow.GetCell(colIdx);

                            switch (colIdx)
                            {
                                case 3:
                                    if (exlCell == null)
                                    {
                                        exlRow.CreateCell(3);
                                        exlRow.GetCell(3).SetCellValue("70062419");
                                    }

                                    break;

                                case 16:
                                    if (exlCell == null)
                                    {
                                        exlRow.CreateCell(16);
                                        exlRow.GetCell(16).SetCellValue(7);
                                    }

                                    break;

                                case 27:
                                    if (!(exlCell == null))
                                    {
                                        if (exlRow.GetCell(26) == null)
                                        {
                                            exlRow.CreateCell(26);
                                            exlRow.GetCell(26).SetCellValue(1);
                                        }
                                    }

                                    break;

                                default:
                                    break;

                            }

                        }

                    }

                    //取得Detail工作表                        
                    sheet = (XSSFSheet)workbook.GetSheetAt(1);

                    for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)   //每一列做迴圈
                    {
                        XSSFRow exlRow = (XSSFRow)sheet.GetRow(rowIdx);

                        for (int colIdx = exlRow.FirstCellNum; colIdx <= exlRow.LastCellNum; colIdx++)   //每一個欄位做迴圈
                        {
                            XSSFCell exlCell = (XSSFCell)exlRow.GetCell(colIdx);

                            switch (colIdx)
                            {
                                case 1:
                                    if (!(exlCell == null))
                                    {
                                        var col1 = exlRow.GetCell(1).StringCellValue.Split('@');
                                        exlRow.CreateCell(1);
                                        exlRow.GetCell(1).SetCellValue(col1[0]);
                                        if (col1.Length>1)
                                        {
                                            if(exlRow.GetCell(7) ==null)
                                            {
                                                exlRow.CreateCell(7);
                                            }
                                            exlRow.GetCell(7).SetCellValue(col1[1]);
                                        }                                            
                                    }

                                    break;

                                case 2:
                                    if (exlCell == null)
                                    {
                                        exlRow.CreateCell(2);
                                        exlRow.GetCell(2).SetCellValue(1);
                                    }

                                    break;

                                case 4:
                                    var col = (XSSFCell)(exlRow.GetCell(5));
                                    if (col != null)
                                    {
                                        if (exlCell == null)
                                        {
                                            exlRow.CreateCell(4);
                                            exlRow.GetCell(4).SetCellValue(col.StringCellValue);
                                        }
                                    }

                                    break;

                                default:

                                    break;

                            }

                        }

                    }

                    //要求公式重算結果
                    sheet.ForceFormulaRecalculation = true;

                    //InvoiceRequestTemp.xls                
                    using (FileStream fsOut = new FileStream(path, FileMode.Create))
                    {
                        workbook.Write(fsOut);
                        fsOut.Close();
                    }
                }
                return "資料轉換成功";
            }
            catch(SecurityException ex)
            {
                return $"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}";
            }
        }

        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="sheet">處理的sheet</param>
        /// <param name="startRow">從第幾行開始（0開始）</param>
        /// <param name="delCount">共删除N行</param>
        public static void DelRow(XSSFSheet sheet, int startRow, int delCount)
        {
            //sheet.ShiftRows(startRow + 1, sheet.LastRowNum, -1, false, false);//删除一行（為負數只能為-1）
            for (int i = 0; i < delCount; i++)
            {
                sheet.ShiftRows(startRow + 1, sheet.LastRowNum, -1);
            }
        }

        /// <summary>
        /// 刪除excel行
        /// </summary>
        /// <param name="path"></param>
        public static string DelExcelRow(this string path, int startRow, int rowCount)
        {
            try
            {
                //DataTable dt = new DataTable();
                XSSFWorkbook workbook;
                using (FileStream fsIn = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                {
                    workbook = new XSSFWorkbook(fsIn);
                    XSSFSheet sheet = (XSSFSheet)workbook.GetSheetAt(0);
                    // 假設我們删除ID為0-1(第1-2行,因為帶一行標題頭)的幾行數據
                    DelRow(sheet, startRow, rowCount);

                }
                //把編輯過后的工作薄重新保存為excel文件
                FileStream fsOut = System.IO.File.Create(path);
                workbook.Write(fsOut);
                fsOut.Close();
                return "表頭刪除成功";
            }
            catch(Exception ex)
            {
                return $"Error message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}";
            }
        }
    }
}