using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.InnovationManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class WorkInnovationController : ApiController
    {
        private string url = BSFramework.Util.Config.GetValue("AppUrl");
        private string dept = BSFramework.Util.Config.GetValue("workinnovation");

        private WorkInnovationBLL ebll = new WorkInnovationBLL();
        private DepartmentBLL deptBll = new DepartmentBLL();

        /// <summary>
        ///数据操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Operation()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<WorkInnovationModel>>(json);
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                UserEntity user = new UserBLL().GetEntity(dy.userId);
                string getDel = string.IsNullOrEmpty(dy.data.DelKeys) ? "" : dy.data.DelKeys;
                #region 删除数据
                if (!string.IsNullOrEmpty(dy.data.deldata))
                {
                    var fileList = fileBll.GetFilesByRecIdNew(dy.data.deldata).ToList();
                    getDel = string.Join(",", fileList.Select(x => x.FileId));
                }
                #endregion
                #region 修改删除图片
                var DelKeys = getDel.Split(',');
                string keys = string.Empty;
                for (int i = 0; i < DelKeys.Length; i++)
                {
                    if (string.IsNullOrEmpty(DelKeys[i]))
                    {
                        continue;
                    }
                    FileInfoEntity fileList = fileBll.GetEntity(DelKeys[i]);
                    if (fileList == null)
                    {
                        continue;
                    }
                    string url = Config.GetValue("FilePath") + fileList.FilePath.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(fileList.FilePath) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                    fileBll.Delete(DelKeys[i]);
                }
                #endregion
                string id = string.Empty;
                bool Operation = false;
                if (dy.data.main != null)
                {
                    if (!string.IsNullOrEmpty(dy.data.main.innovationid))
                    {
                        Operation = false;
                        id = dy.data.main.innovationid;
                    }
                    else
                    {
                        Operation = true;
                        id = Guid.NewGuid().ToString();
                        dy.data.main.innovationid = id;
                        dy.data.main.deptid = user.DepartmentId;
                        var dept = deptBll.GetEntity(user.DepartmentId);
                        dy.data.main.deptname = dept.FullName;
                    }
                }
                if (dy.data.audit != null)
                {
                    dy.data.audit = dy.data.audit.OrderBy(x => x.isspecial).ToList();
                    foreach (var item in dy.data.audit)
                    {
                        if (string.IsNullOrEmpty(item.auditid))
                        {
                            item.auditid = Guid.NewGuid().ToString();
                        }
                        if (string.IsNullOrEmpty(item.innovationid))
                        {

                            if (dy.data.main != null)
                            {
                                item.innovationid = dy.data.main.innovationid;
                            }
                        }
                    }
                }

                #region 存储图片
                var Description = string.Empty;

                FileInfoEntity fi = null;
                foreach (string key in HttpContext.Current.Request.Files.AllKeys)
                {

                    HttpPostedFile file = HttpContext.Current.Request.Files[key];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名

                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = id,
                        RecId = id,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/WorkInnovation/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = file.FileName.Contains("xz_") ? "xz" : file.FileName.Contains("fj_") ? "fj" : "ty",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\WorkInnovation"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\WorkInnovation");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\WorkInnovation\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                #endregion
                if (!string.IsNullOrEmpty(dy.data.deldata))
                {
                    ebll.Operation(new WorkInnovationEntity() { innovationid = dy.data.deldata }, null, true);
                }
                else
                {
                    ebll.Operation(dy.data.main, dy.data.audit, Operation);


                }
                return new { info = "操作成功", code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }

        ///<summary>
        ///审核人获取数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetWorkInnovationExtensions(BaseDataModel dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel>(json);
                var data = ebll.getWorkInnovationByidExtensions(dy.userId);
                foreach (var item in data)
                {
                    if (item.proposedPhoto == null)
                    {
                        item.proposedPhoto = new List<FileInfoEntity>();
                    }
                    if (item.statusquoPhoto == null)
                    {
                        item.statusquoPhoto = new List<FileInfoEntity>();
                    }
                    if (item.proposedFile == null)
                    {
                        item.proposedFile = new List<FileInfoEntity>();
                    }
                    foreach (var items in item.proposedPhoto)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                    foreach (var items in item.statusquoPhoto)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                    foreach (var items in item.proposedFile)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                }
                return new { info = "操作成功", code = result, data = data };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }
        /// <summary>
        /// 获取数据
        /// <returns></returns>
        [HttpPost]
        public object GetWorkInnovation(BaseDataModel dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel<string>>(json);
                var dic = new Dictionary<string, string>();
                var data = ebll.getList("", null);
                foreach (var item in data)
                {
                    if (item.proposedPhoto == null)
                    {
                        item.proposedPhoto = new List<FileInfoEntity>();
                    }
                    if (item.statusquoPhoto == null)
                    {
                        item.statusquoPhoto = new List<FileInfoEntity>();
                    }
                    if (item.proposedFile == null)
                    {
                        item.proposedFile = new List<FileInfoEntity>();
                    }
                    foreach (var items in item.proposedPhoto)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                    foreach (var items in item.statusquoPhoto)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                    foreach (var items in item.proposedFile)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                }
                return new { info = "操作成功", code = result, data = data };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }


        /// <summary>
        /// 获取二级审核人员
        /// <returns></returns>
        [HttpPost]
        public object GetAuditPerson(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel<string>>(json);
                var data = ebll.getAuditUser(dy.userId, dept, "app");
                var resultdata = data.Select(x => new { value = x.UserId, text = x.RealName });
                return new { info = "操作成功", code = result, data = resultdata };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }
        /// <summary>
        /// 获取上级部门id
        /// <returns></returns>
        [HttpPost]
        public object getParentDept(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel<string>>(json);
                var dept = deptBll.GetEntity(dy.data);
                return new { info = "操作成功", code = result, data = dept.ParentId };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }
        /// <summary>
        /// 获取数据
        /// <returns></returns>
        [HttpPost]
        public object GetWorkInnovationbyuser(BaseDataModel dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel<string>>(json);
                var data = ebll.getWorkInnovationByuser(dy.userId);
                foreach (var item in data)
                {
                    if (item.proposedPhoto == null)
                    {
                        item.proposedPhoto = new List<FileInfoEntity>();
                    }
                    if (item.statusquoPhoto == null)
                    {
                        item.statusquoPhoto = new List<FileInfoEntity>();
                    }
                    if (item.proposedFile == null)
                    {
                        item.proposedFile = new List<FileInfoEntity>();
                    }
                    foreach (var items in item.proposedPhoto)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                    foreach (var items in item.statusquoPhoto)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                    foreach (var items in item.proposedFile)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                }
                return new { info = "操作成功", code = result, data = data };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }
    }
}