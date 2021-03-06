using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using Aspose.Cells;

namespace BSFramework.Util.Offices
{
    /// <summary>
    /// Office2Pdf 将Office文档转化为pdf
    /// </summary>
    public class Office2PDFHelper
    {
        public Office2PDFHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        /// <summary>
        /// Word转换成pdf
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <returns>true=转换成功</returns>
        public static bool DOCConvertToPDF(string sourcePath, string targetPath)
        {
            #region
            //bool result = false;
            //Word.WdExportFormat exportFormat = Word.WdExportFormat.wdExportFormatPDF;
            //object paramMissing = Type.Missing;
            //Word.ApplicationClass wordApplication = new Word.ApplicationClass();
            //Word.Document wordDocument = null;
            //try
            //{
            //    object paramSourceDocPath = sourcePath;
            //    string paramExportFilePath = targetPath;
            //    Word.WdExportFormat paramExportFormat = exportFormat;
            //    bool paramOpenAfterExport = false;
            //    Word.WdExportOptimizeFor paramExportOptimizeFor = Word.WdExportOptimizeFor.wdExportOptimizeForPrint;
            //    Word.WdExportRange paramExportRange = Word.WdExportRange.wdExportAllDocument;
            //    int paramStartPage = 0;
            //    int paramEndPage = 0;
            //    Word.WdExportItem paramExportItem = Word.WdExportItem.wdExportDocumentContent;
            //    bool paramIncludeDocProps = true;
            //    bool paramKeepIRM = true;
            //    Word.WdExportCreateBookmarks paramCreateBookmarks = Word.WdExportCreateBookmarks.wdExportCreateWordBookmarks;
            //    bool paramDocStructureTags = true;
            //    bool paramBitmapMissingFonts = true;
            //    bool paramUseISO19005_1 = false;
            //    wordDocument = wordApplication.Documents.Open(
            //        ref paramSourceDocPath, ref paramMissing, ref paramMissing,
            //        ref paramMissing, ref paramMissing, ref paramMissing,
            //        ref paramMissing, ref paramMissing, ref paramMissing,
            //        ref paramMissing, ref paramMissing, ref paramMissing,
            //        ref paramMissing, ref paramMissing, ref paramMissing,
            //        ref paramMissing);
            //    if (wordDocument != null)
            //        wordDocument.ExportAsFixedFormat(paramExportFilePath,
            //            paramExportFormat, paramOpenAfterExport,
            //            paramExportOptimizeFor, paramExportRange, paramStartPage,
            //            paramEndPage, paramExportItem, paramIncludeDocProps,
            //            paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
            //            paramBitmapMissingFonts, paramUseISO19005_1,
            //            ref paramMissing);
            //    result = true;
            //}
            //catch
            //{
            //    result = false;
            //}
            //finally
            //{
            //    if (wordDocument != null)
            //    {
            //        wordDocument.Close(ref paramMissing, ref paramMissing, ref paramMissing);
            //        wordDocument = null;
            //    }
            //    if (wordApplication != null)
            //    {
            //        wordApplication.Quit(ref paramMissing, ref paramMissing, ref paramMissing);
            //        wordApplication = null;
            //    }
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //}
            //return result;
            #endregion
            try
            {
                Aspose.Words.Document doc = new Aspose.Words.Document(sourcePath);
                doc.Save(targetPath, Aspose.Words.SaveFormat.Pdf);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
         
        }

        /// <summary>
        /// 把Excel文件转换成PDF格式文件  
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <returns>true=转换成功</returns>
        #region 旧
        //public static bool XLSConvertToPDF(string sourcePath, string targetPath)
        //{
        //    bool result = false;
        //    Excel.XlFixedFormatType targetType = Excel.XlFixedFormatType.xlTypePDF;
        //    object missing = Type.Missing;
        //    Excel.ApplicationClass application = null;
        //    Excel.Workbook workBook = null;
        //    try
        //    {
        //        application = new Excel.ApplicationClass();
        //        object target = targetPath;
        //        object type = targetType;
        //        workBook = application.Workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
        //            missing, missing, missing, missing, missing, missing, missing, missing, missing);
        //        workBook.ExportAsFixedFormat(targetType, target, Excel.XlFixedFormatQuality.xlQualityStandard, true, false, missing, missing, missing, missing);
        //        result = true;
        //    }
        //    catch(Exception ex)
        //    {
        //        result = false;
        //    }
        //    finally
        //    {
        //        if (workBook != null)
        //        {
        //            workBook.Close(true, missing, missing);
        //            workBook = null;
        //        }
        //        if (application != null)
        //        {
        //            application.Quit();
        //            application = null;
        //        }
        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();
        //    }
        //    return result;
        //}
        #endregion
        public static bool XLSConvertToPDF(string sourcePath, string targetPath) {
            try
            {
                var book = new Workbook(sourcePath);
                book.Save(targetPath, SaveFormat.Pdf);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        ///<summary>        
        /// 把PowerPoint文件转换成PDF格式文件       
        ///</summary>        
        ///<param name="sourcePath">源文件路径</param>     
        ///<param name="targetPath">目标文件路径</param> 
        ///<returns>true=转换成功</returns> 
        public static bool PPTConvertToPDF(string sourcePath, string targetPath)
        {
            bool result;
            PowerPoint.PpSaveAsFileType targetFileType = PowerPoint.PpSaveAsFileType.ppSaveAsPDF;
            object missing = Type.Missing;
            PowerPoint.ApplicationClass application = null;
            PowerPoint.Presentation persentation = null;
            try
            {
                application = new PowerPoint.ApplicationClass();
                persentation = application.Presentations.Open(sourcePath, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse); persentation.SaveAs(targetPath, targetFileType, Microsoft.Office.Core.MsoTriState.msoTrue);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (persentation != null)
                {
                    persentation.Close();
                    persentation = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return result;
        }
    }
}

