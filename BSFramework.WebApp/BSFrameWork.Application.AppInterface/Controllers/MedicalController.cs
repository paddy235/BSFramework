using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class MedicalController : ApiController
    {
        private MedicalBLL bll = new MedicalBLL();
        /// <summary>
        /// 新增创建图片获取id
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetMedicalGuId()
        {
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            try
            {
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                string newpath = string.Empty;//数据库路径
                string id = string.Empty;//Guid
                //新建二维码图片
                GetGuId(userId, "职业健康", out newpath, out id);
                return new
                {
                    info = "成功",
                    code = 0,
                    data = new
                    {
                        Guid = id,
                        path = newpath.Replace("~", url)
                    }
                };
            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1, data = new { } };

            }

        }


        private void GetGuId(string userId, string type, out string newpath, out string id)
        {
            try
            {
                FileInfoBLL fb = new FileInfoBLL();
                var flist = fb.GetFilebyDescription(userId, "职业健康二维码");
                if (flist != null)
                {
                    newpath = flist.FilePath;
                    id = flist.FolderId;
                }
                else
                {
                    //新增创建新的证书id
                    id = Guid.NewGuid().ToString();
                    //二维码画图
                    var encoder = new QRCodeEncoder();
                    var image = encoder.Encode(id + "|" + type, Encoding.UTF8);
                    var filedir = BSFramework.Util.Config.GetValue("FilePath");
                    if (!System.IO.Directory.Exists(filedir))
                    {
                        System.IO.Directory.CreateDirectory(filedir);
                    }

                    if (!Directory.Exists(Path.Combine(filedir, "DocumentFile", "Medical")))
                    {
                        Directory.CreateDirectory(Path.Combine(filedir, "DocumentFile", "Medical"));
                    }
                    //保存路径
                    var newurl = Path.Combine(filedir, "DocumentFile", "Medical", id + ".jpg");
                    //保存图片
                    image.Save(newurl);
                    //创建数据实体
                    var fileentity = new FileInfoEntity
                    {
                        FileId = Guid.NewGuid().ToString(),
                        FolderId = id,
                        RecId = userId,
                        FileName = System.IO.Path.GetFileName(newurl),
                        FilePath = "~/Resource/DocumentFile/Medical/" + id + ".jpg",
                        FileType = "jpg",
                        FileExtensions = ".jpg",
                        Description = "职业健康二维码",
                        FileSize = "0",
                        DeleteMark = 0,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now
                    };
                    //保存数据
                    fb.SaveForm(fileentity);
                    newpath = fileentity.FilePath;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }



        /// <summary>
        /// 获取信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object getMedical()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            // UserEntity user = new UserBLL().GetEntity(userId);
            try
            {
                var entity = bll.getMedicalInfo(userId).OrderByDescending(x=>x.MedicalTime);
                return new { info = "查询成功", code = result, count = entity.Count(), data = entity };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new List<MedicalEntity>() };

            }

        }


        /// <summary>
        /// 获取信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object getMedicalDetail()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            string id = dy.data;
            // UserEntity user = new UserBLL().GetEntity(userId);
            try
            {
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                var entity = bll.getMedicalDetail(userId, id);
                if (entity == null)
                {
                    return new { info = "查询成功", code = result, data = entity };
                }
                foreach (var item in entity.Files)
                {
                    item.FilePath = item.FilePath.Replace("~/", url);
                }
                var Getpath = entity.Files.FirstOrDefault(x => x.Description == "职业健康二维码");
                if (Getpath == null)
                {
                    entity.path = "";
                }
                else
                {
                    entity.path = Getpath.FilePath;
                    entity.Files.Remove(Getpath);
                }
                var data = new
                {
                    MedicalId = entity.MedicalId,
                    MedicalType = entity.MedicalType,
                    MedicalTime = entity.MedicalTime.ToString("yyyy-MM-dd"),
                    MedicalTypeId = entity.MedicalTypeId,
                    Organization = entity.Organization,
                    HealthResult = entity.HealthResult,
                    HealthResultId = entity.HealthResultId,
                    ResultDetail = entity.ResultDetail,
                    remark=entity.remark,
                    Files=entity.Files,
                    path=entity.path

                };
                return new { info = "查询成功", code = result, data = data };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new MedicalEntity() };

            }

        }

        /// <summary>
        /// 新增体检信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object addMedical()
        {
            var result = 0;
            var message = string.Empty;

            try
            {
                string res = HttpContext.Current.Request.Form["json"];
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var entity = JsonConvert.DeserializeObject<MedicalEntity>(json["data"].ToString());
                UserEntity user = new UserBLL().GetEntity(userId);
                if (entity.Files == null) entity.Files = new List<FileInfoEntity>();
                List<FileInfoEntity> deletefiles = entity.Files.ToList();
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                var old = fileBll.GetFilesByRecIdNew(entity.MedicalId).Where(x => x.Description != "职业健康二维码").ToList();
                var undeletefiles = old.Except(deletefiles, new FileCompare());
                var deletefilesinfo = old.Except(undeletefiles, new FileCompare()).ToList();
                entity.Files = undeletefiles.ToList();
                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
                }
                if (!Directory.Exists(Path.Combine(filedir, "AppFile", "Medical")))
                {
                    Directory.CreateDirectory(Path.Combine(filedir, "AppFile", "Medical"));
                }
                entity.createtime = DateTime.Now;
                entity.createuser = user.RealName;
                entity.createuserid = user.UserId;
                //entity.Id = Guid.NewGuid().ToString();
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[i];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    var fileentity = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.MedicalId,
                        RecId = entity.MedicalId,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/Medical/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = file.ContentType.StartsWith("audio") ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now
                    };
                    file.SaveAs(Path.Combine(filedir, "AppFile", "Medical", fileId + ext));
                    //保存附件信息
                    entity.Files.Add(fileentity);
                }

                bll.addMedical(entity);
                for (int i = 0; i < deletefilesinfo.Count; i++)
                {
                    string url = Config.GetValue("FilePath") + deletefilesinfo[i].FilePath.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(deletefilesinfo[i].FilePath) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                }
                message = "操作成功";
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        /// <summary>
        /// 同步图片
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object MedicalFile()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;//业务记录Id
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                var data = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description != "职业健康二维码").ToList();
                foreach (var item in data)
                {
                    item.FilePath = item.FilePath.Replace("~", url);
                }
                return new { code = 0, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {
                return new { code = 0, info = ex.Message, data = new FileInfoEntity() };

            }

        }
        /// <summary>
        /// 上传图片删除图片
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object UploadMedical()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data.id;//业务记录Id
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                string getDel = dy.data.DelKeys;
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
                        FilePath = "~/Resource/AppFile/Medical/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0
                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Medical"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Medical");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Medical\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                return new { code = 0, info = "操作成功", };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 修改用户证书
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UpdateMedical()
        {
            var result = 0;
            var message = string.Empty;

            try
            {
                string res = HttpContext.Current.Request.Form["json"];
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var entity = JsonConvert.DeserializeObject<MedicalEntity>(json["data"].ToString());
                UserEntity user = new UserBLL().GetEntity(userId);
                if (entity.Files == null) entity.Files = new List<FileInfoEntity>();
                List<FileInfoEntity> deletefiles = entity.Files.ToList();
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                var old = fileBll.GetFilesByRecIdNew(entity.MedicalId).Where(x => x.Description != "职业健康二维码").ToList();
                var undeletefiles = old.Except(deletefiles, new FileCompare());
                var deletefilesinfo = old.Except(undeletefiles, new FileCompare()).ToList();
                entity.Files = undeletefiles.ToList();
                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
                }

                if (!Directory.Exists(Path.Combine(filedir, "AppFile", "Medical")))
                {
                    Directory.CreateDirectory(Path.Combine(filedir, "AppFile", "Medical"));
                }
                var one = bll.getMedicalDetail(userId, entity.MedicalId);
                entity.createtime = one.createtime;
                entity.createuser = one.createuser;
                entity.createuserid = one.createuserid;
                entity.modifytime = DateTime.Now;
                entity.modifyuser = user.RealName;
                entity.modifyuserid = user.UserId;
                //entity.Id = Guid.NewGuid().ToString();
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[i];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    var fileentity = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.MedicalId,
                        RecId = entity.MedicalId,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/Medical/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = file.ContentType.StartsWith("audio") ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now
                    };
                    file.SaveAs(Path.Combine(filedir, "AppFile", "Medical", fileId + ext));
                    //保存附件信息
                    entity.Files.Add(fileentity);
                }
                entity.remark = string.IsNullOrEmpty(entity.remark) ? "" : entity.remark;
                bll.modifyMedical(entity);
                for (int i = 0; i < deletefilesinfo.Count; i++)
                {
                    string url = Config.GetValue("FilePath") + deletefilesinfo[i].FilePath.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(deletefilesinfo[i].FilePath) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                }
                message = "操作成功";
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        /// <summary>
        /// 删除信息
        /// </summary>
        /// <returns></returns>
        public object DelMedical()
        {

            var result = 0;
            var message = string.Empty;
            try
            {
                var json = HttpContext.Current.Request.Form.Get("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
                string id = dy.data;//业务记录Id
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                var old = fileBll.GetFilesByRecIdNew(id);

                bll.delMedical(id);
                List<FileInfoEntity> fileList = old.ToList();
                for (int i = 0; i < fileList.Count; i++)
                {
                    string url = Config.GetValue("FilePath") + fileList[i].FilePath.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(fileList[i].FilePath) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                }
                message = "操作成功";
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

    }
}
