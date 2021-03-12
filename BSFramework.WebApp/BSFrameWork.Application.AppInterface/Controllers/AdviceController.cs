using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.InnovationManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class AdviceController : ApiController
    {
        private string url = BSFramework.Util.Config.GetValue("AppUrl");
        private AdviceBLL ebll = new AdviceBLL();
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
                var dy = JsonConvert.DeserializeObject<BaseDataModel<AdviceModel>>(json);
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
                //修改
                if (dy.data.updatedata != null)
                {
                    id = dy.data.updatedata.adviceid;
                }
                if (dy.data.adddata != null)
                {
                    id = Guid.NewGuid().ToString();
                    dy.data.adddata.adviceid = id;
                    dy.data.adddata.deptid = user.DepartmentId;
                    var dept = deptBll.GetEntity(user.DepartmentId);
                    dy.data.adddata.deptname = dept.FullName;

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
                        FilePath = "~/Resource/AppFile/Advice/" + fileId + ext,
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
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Advice"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Advice");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Advice\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                #endregion
                ebll.Operation(dy.data.adddata, dy.data.updatedata, dy.data.deldata, dy.data.audit, dy.data.auditupdate);
                return new { info = "操作成功", code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }

        /// <summary>
        ///统计
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAdviceList(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel>(json);
                var dic = new Dictionary<string, string>();
                dic.Add("deptid", dy.data);
                var data = ebll.getAdviceList(dic, null);
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
                var ad = new Dictionary<string, List<AdviceEntity>>();
                int count = data.Count;
                int i = 0;
                var year = string.Empty;
                if (data.Count > 0)
                {
                    year = data[0].reporttime.ToString("yyyy");
                }
                var next = string.Empty;
                var group = new List<AdviceEntity>();
                foreach (var item in data)
                {
                    i++;
                    next = item.reporttime.ToString("yyyy");
                    if (next != year)
                    {
                        var addEntity = new List<AdviceEntity>();
                        addEntity.AddRange(group);
                        ad.Add(year, addEntity);
                        group.Clear();
                        //重置
                        group.Add(item);
                        year = item.reporttime.ToString("yyyy");
                    }
                    else
                    {

                        group.Add(item);
                    }
                    if (i == count && ad.Count == 0)
                    {
                        var addEntity = new List<AdviceEntity>();
                        addEntity.AddRange(group);
                        ad.Add(year, addEntity);
                        group.Clear();
                    }

                }
                var Resultdata = ad.Select(x => new { year = x.Key, yeardata = x.Value });
                return new { info = "操作成功", code = result, data = Resultdata };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }

        /// <summary>
        ///审核人获取数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAdviceExtensions(BaseDataModel dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel>(json);
                var data = ebll.getAdviceByidExtensions(dy.userId);
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
        /// <summary>
        /// 获取数据
        /// <returns></returns>
        [HttpPost]
        public object GetAdvice(BaseDataModel dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel<string>>(json);
                var dic = new Dictionary<string, string>();
                var data = ebll.getAdviceListIndex(dic, null);
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



        /// <summary>
        /// 获取数据
        /// <returns></returns>
        [HttpPost]
        public object GetAdvicebyuser(BaseDataModel dy)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            try
            {
                //var dy = JsonConvert.DeserializeObject<BaseDataModel<string>>(json);
                var data = ebll.getAdvicebyuser(dy.userId);
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
