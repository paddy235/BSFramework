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
    public class QcActivityController : ApiController
    { 
        private string url = BSFramework.Util.Config.GetValue("AppUrl");

        private QcActivityBLL ebll = new QcActivityBLL();
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
                var dy = JsonConvert.DeserializeObject<BaseDataModel<QcActivityModel>>(json);
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
                    string url = Config.GetValue("FilePath") + fileList.FilePath.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(fileList.FilePath) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                    fileBll.Delete(DelKeys[i]);
                }
                #endregion
                string id = string.Empty;
                bool EntityType = false;
                if (dy.data.entity != null)
                {
                    if (string.IsNullOrEmpty(dy.data.entity.qcid))
                    {
                        id = Guid.NewGuid().ToString(); EntityType = true;
                    }
                    else
                    {
                        id = dy.data.entity.qcid;
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
                        FilePath = "~/Resource/AppFile/QcActivity/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = file.FileName.Contains("tp_") ? "照片" : "文件",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\QcActivity"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\QcActivity");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\QcActivity\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                #endregion
                #region 数据操作
                if (string.IsNullOrEmpty(dy.data.deldata))
                {
                    if (EntityType)
                    {
                        dy.data.entity.qcid = id;
                        ebll.addEntity(dy.data.entity);
                    }
                    else
                    {
                        ebll.EditEntity(dy.data.entity);
                    }
                }
                else
                {
                    ebll.delEntity(dy.data.deldata);
                }
                #endregion
                return new { info = "操作成功", code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }

        /// <summary>
        ///数据操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object getUserDept(BaseDataModel<string> entity)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var data = ebll.getUserDept(entity.data);

                return new { info = "操作成功", code = result, data = data };
            }
            catch (Exception ex)
            {
                return new { info = "操作失败：" + ex.Message, code = 1 };
            }
        }
        /// <summary>
        ///数据操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object getQcListGroup(BaseDataModel<string> entity)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                //UserEntity user = new UserBLL().GetEntity(entity.userId);
                Dictionary<string, string> str = new Dictionary<string, string>();
                str.Add("deptid", entity.data);
                var data = ebll.getQcList(str, null);
                foreach (var item in data)
                {
                    if (item.Photos == null)
                    {
                        item.Photos = new List<FileInfoEntity>();
                    }
                    if (item.Files == null)
                    {
                        item.Files = new List<FileInfoEntity>();
                    }
                    foreach (var items in item.Photos)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                    foreach (var items in item.Files)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                }
                var Qc = new Dictionary<string, List<QcActivityEntity>>();
                var year = string.Empty;
                if (data.Count > 0)
                {
                    year = data[0].subjecttime.ToString("yyyy");
                }
                var next = string.Empty;
                var group = new List<QcActivityEntity>();
                int count = data.Count;
                int i = 0;
                foreach (var item in data)
                {
                    i++;
                    next = item.subjecttime.ToString("yyyy");
                    if (next != year)
                    {
                        var  addEntity= new List<QcActivityEntity>();
                        addEntity.AddRange(group);
                        Qc.Add(year, addEntity);
                        group.Clear();
                        //重置
                        group.Add(item);
                        year = item.subjecttime.ToString("yyyy");
                    }
                    else
                    {
                        group.Add(item);
                    }
                    if (i==count&&Qc.Count==0)
                    {
                        var addEntity = new List<QcActivityEntity>();
                        addEntity.AddRange(group);
                        Qc.Add(year, addEntity);
                        group.Clear();
                    }
                }
                var Resultdata = Qc.Select(x => new { year = x.Key, yeardata = x.Value });
                return new { info = "操作成功", code = result, data = Resultdata };
            }
            catch (Exception ex)
            {
                return new { info = "操作失败：" + ex.Message, code = 1 };
            }
        }

        /// <summary>
        ///数据操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object getQcList(BaseDataModel<string> entity)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                //UserEntity user = new UserBLL().GetEntity(entity.userId);
                Dictionary<string, string> str = new Dictionary<string, string>();
                str.Add("deptid", entity.data);
                var data = ebll.getQcList(str, null);
                foreach (var item in data)
                {
                    if (item.Photos == null)
                    {
                        item.Photos = new List<FileInfoEntity>();
                    }
                    if (item.Files == null)
                    {
                        item.Files = new List<FileInfoEntity>();
                    }
                    foreach (var items in item.Photos)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                    foreach (var items in item.Files)
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
