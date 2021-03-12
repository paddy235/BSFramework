using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFramework.Util.Offices;
using BSFramework.Application.Code;
using System.IO;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Busines.PublicInfoManage;

namespace BSFramework.Application.Web.Controllers
{
    /// <summary>
    /// 描 述：公共控制器
    /// </summary>
    public class UtilityController : Controller
    {
        #region 验证对象值不能重复
        #endregion

        #region 导出Excel
        /// <summary>
        /// 请选择要导出的字段页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ExcelExportForm()
        {
            return View();
        }
        /// <summary>
        /// 执行导出Excel
        /// </summary>
        /// <param name="JsonColumn">表头</param>
        /// <param name="JsonData">数据</param>
        /// <param name="exportField">导出字段</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public void ExecuteExportExcel(string columnJson, string rowJson, string exportField, string filename)
        {
            //设置导出格式
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.Title = Server.UrlDecode(filename);
            excelconfig.TitleFont = "微软雅黑";
            excelconfig.TitlePoint = 15;
            excelconfig.FileName = Server.UrlDecode(filename) + ".xls";
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnEntity = new List<ColumnEntity>();
            //表头
            List<GridColumnModel> columnData = columnJson.ToList<GridColumnModel>();
            //行数据
            DataTable rowData = rowJson.ToTable();
            //写入Excel表头
            string[] fieldInfo = exportField.Split(',');
            foreach (string item in fieldInfo)
            {
                var list = columnData.FindAll(t => t.name == item);
                foreach (GridColumnModel gridcolumnmodel in list)
                {
                    if (gridcolumnmodel.hidden.ToLower() == "false" && gridcolumnmodel.label != null)
                    {
                        string align = gridcolumnmodel.align;
                        excelconfig.ColumnEntity.Add(new ColumnEntity()
                        {
                            Column = gridcolumnmodel.name,
                            ExcelColumn = gridcolumnmodel.label,
                            //Width = gridcolumnmodel.width,
                            Alignment = gridcolumnmodel.align,
                        });
                    }
                }
            }
            ExcelHelper.ExcelDownload(rowData, excelconfig);
        }
        #endregion

        #region
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="Filedata">文件对象</param>
        /// <returns></returns>
        [HttpPost]
        public string UploadFile()
        {
            try
            {
                //没有文件上传，直接返回
                if (Request.Files.Count == 0)
                {
                    return "No Files";
                }
                HttpPostedFileBase Filedata = Request.Files[0];
                //获取文件完整文件名(包含绝对路径)
                //文件存放路径格式：/Resource/ResourceFile/{userId}{data}/{guid}.{后缀名}
                string userId = OperatorProvider.Provider.Current().UserId;
                string fileGuid = Guid.NewGuid().ToString();
                long filesize = Filedata.ContentLength;
                string FileEextension = Path.GetExtension(Filedata.FileName);
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                string virtualPath = string.Format("/Resource/temp/{0}/{1}/{2}{3}", userId, uploadDate, fileGuid, FileEextension);
                string fullFileName = this.Server.MapPath("~" + virtualPath);
                //创建文件夹
                string path = Path.GetDirectoryName(fullFileName);

                string callback = Request.Params["CKEditorFuncNum"];
                Directory.CreateDirectory(path);
                if (!System.IO.File.Exists(fullFileName))
                {
                    //保存文件
                    Filedata.SaveAs(fullFileName);
                }
                return "<script type=\"text/javascript\">window.parent.CKEDITOR.tools.callFunction(" + callback + ",'" + Request.ApplicationPath + virtualPath + "');</script>";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// FileDrop组件以流的方式实现文件上传
        /// </summary>
        /// <param name="filePath">文件存储路径</param>
        /// <param name="recId">关联记录Id</param>
        /// <param name="isDate">是否按日期目录存储文件</param>
        /// <returns></returns>
        [HttpPost]
        public string PostFile([System.Web.Http.FromUri]string filePath, [System.Web.Http.FromUri]string recId, [System.Web.Http.FromUri]int isDate = 0)
        {
            string newFilePath = "";
            if (Request.Files.Count > 0)
            {
                foreach (string key in Request.Files.Keys)
                {
                    HttpPostedFileBase file = Request.Files[key];
                    //原始文件名
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string fileGuid = Guid.NewGuid().ToString();
                    long filesize = file.ContentLength;
                    string FileEextension = Path.GetExtension(fileName);
                    string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                    string dir = isDate == 0 ? string.Format("~/Resource/{0}", filePath) : string.Format("~/Resource/{0}/{1}", filePath, uploadDate);
                    string newFileName = fileGuid + FileEextension;
                    newFilePath = dir + "/" + newFileName;
                    if (!Directory.Exists(Server.MapPath(dir)))
                    {
                        Directory.CreateDirectory(Server.MapPath(dir));
                    }

                    FileInfoEntity fileInfoEntity = new FileInfoEntity();
                    if (!System.IO.File.Exists(Server.MapPath(newFilePath)))
                    {
                        //保存文件
                        file.SaveAs(Server.MapPath(newFilePath));
                        //文件信息写入数据库
                        fileInfoEntity.Create();
                        fileInfoEntity.FileId = fileGuid;
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            fileInfoEntity.FolderId = filePath;
                        }
                        else
                        {
                            fileInfoEntity.FolderId = "0";
                        }
                        fileInfoEntity.RecId = recId;
                        fileInfoEntity.FileName = fileName;
                        fileInfoEntity.FilePath = dir + "/" + newFileName;
                        fileInfoEntity.FileSize = (Math.Round(decimal.Parse(filesize.ToString()) / decimal.Parse("1024"), 2)).ToString();//文件大小（kb）
                        fileInfoEntity.FileExtensions = FileEextension;
                        switch (FileEextension.ToLower())
                        {
                            case ".mp3":
                                fileInfoEntity.Description = "音频";
                                break;
                            case ".jpg":
                            case ".png":
                            case ".jepg":
                            case ".gif":
                            case ".bmp":
                                fileInfoEntity.Description = "照片";
                                break;
                        }
                        fileInfoEntity.FileType = FileEextension.TrimStart('.');
                        FileInfoBLL fileInfoBLL = new FileInfoBLL();
                        fileInfoBLL.SaveForm("", fileInfoEntity);
                    }
                }

            }
            return newFilePath;
        }

        /// <summary>
        /// FileDrop组件以流的方式实现文件上传(后台安全活动专用)
        /// </summary>
        /// <param name="filePath">文件存储路径</param>
        /// <param name="recId">关联记录Id</param>
        /// <param name="isDate">是否按日期目录存储文件</param>
        /// <returns></returns>
        [HttpPost]
        public string PostFileActity([System.Web.Http.FromUri]string filePath, [System.Web.Http.FromUri]string recId, [System.Web.Http.FromUri]string ptype, [System.Web.Http.FromUri]int isDate = 0)
        {
            switch (ptype)
            {
                case"0":
                    ptype = "班组活动";
                    break;
                case "1":
                    ptype = "音频";
                    break;
                case "2":
                    ptype = "照片";
                    break;
                case "3":
                    ptype = "视频";
                    break;
                default:
                    break;
            }
            
            string newFilePath = "";
            if (Request.Files.Count > 0)
            {
                foreach (string key in Request.Files.Keys)
                {
                    HttpPostedFileBase file = Request.Files[key];
                    //原始文件名
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    string fileGuid = Guid.NewGuid().ToString();
                    long filesize = file.ContentLength;
                    string FileEextension = Path.GetExtension(fileName);
                    string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                    string dir = isDate == 0 ? string.Format("~/Resource/{0}", filePath) : string.Format("~/Resource/{0}/{1}", filePath, uploadDate);
                    string newFileName = fileGuid + FileEextension;
                    newFilePath = dir + "/" + newFileName;
                    if (!Directory.Exists(Server.MapPath(dir)))
                    {
                        Directory.CreateDirectory(Server.MapPath(dir));
                    }

                    FileInfoEntity fileInfoEntity = new FileInfoEntity();
                    if (!System.IO.File.Exists(Server.MapPath(newFilePath)))
                    {
                        //保存文件
                        file.SaveAs(Server.MapPath(newFilePath));
                        //文件信息写入数据库
                        fileInfoEntity.Create();
                        fileInfoEntity.FileId = fileGuid;
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            fileInfoEntity.FolderId = filePath;
                        }
                        else
                        {
                            fileInfoEntity.FolderId = "0";
                        }
                        fileInfoEntity.RecId = recId;
                        fileInfoEntity.Description = ptype;
                        fileInfoEntity.FileName = fileName;
                        fileInfoEntity.FilePath = dir + "/" + newFileName;
                        fileInfoEntity.FileSize = (Math.Round(decimal.Parse(filesize.ToString()) / decimal.Parse("1024"), 2)).ToString();//文件大小（kb）
                        fileInfoEntity.FileExtensions = FileEextension;
                        fileInfoEntity.FileType = FileEextension.TrimStart('.');
                        FileInfoBLL fileInfoBLL = new FileInfoBLL();
                        fileInfoBLL.SaveForm("", fileInfoEntity);
                    }
                }

            }
            return newFilePath;
        }

        #endregion
    }
}
