using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.CertificateManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.CertificateManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class CertificateController : ApiController
    {

        private CertificateBLL cbll = new CertificateBLL();
        private UserCertificateBLL ucbll = new UserCertificateBLL();

        #region  证书类型
        /// <summary>
        /// 获取目录第一级
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object getCertificateFirst()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            try
            {
                var entity = cbll.getCertificateFirst().OrderBy(x=>x.Sort);
                return new { info = "查询成功", code = result, data = entity };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new CertificateEntity() };

            }

        }

        /// <summary>
        /// 获取下级目录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object getCertificateById()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            string id = dy.data.id;
            string num = dy.data.num;
            try
            {
                var entity = cbll.getCertificateById(id).OrderBy(x => x.Sort).ToList();
                if (!string.IsNullOrEmpty(num))
                {
                    var getNum = Convert.ToInt32(num);
                    entity = entity.Take(getNum).ToList();
                }

                return new { info = "查询成功", code = result, data = entity };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new List<CertificateEntity>() };

            }

        }

        /// <summary>
        /// 模糊查询列框
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object selectLike()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            string name = dy.data.name;
            string id = dy.data.id;
            try
            {
                var entity = cbll.selectLikeList(name, id).OrderBy(x => x.Sort).ToList();
                return new { info = "查询成功", code = result, data = entity };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new List<CertificateEntity>() };

            }

        }

        [HttpPost]
        public object selectLikeNew()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            DataItemBLL ditem = new DataItemBLL();
            DataItemDetailBLL detail = new DataItemDetailBLL();
            string name = dy.data.name;
            try
            {
                List<DataItemDetailEntity> flist = new List<DataItemDetailEntity>();
                var folks = ditem.GetEntityByCode("PersonWrokType");
                if (folks != null)
                {
                    flist = detail.GetList(folks.ItemId).Where(x=>x.ItemName.Contains(name)).ToList();
                }
                var nlist = flist.Select(x => new
                {
                    Text = x.ItemName,
                    Value = x.ItemName
                }).ToList();
                return new { info = "查询成功", code = result, data = nlist };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new List<CertificateEntity>() };

            }

        }
       

        /// <summary>
        /// 新增类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object addCertificate()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            var jobject = JObject.Parse(json);
            string userId = dy.userId;
            try
            {
                UserBLL ubll = new UserBLL();
                UserEntity user = ubll.GetEntity(userId);
                var jobjson = jobject.Value<JToken>("data").ToJson();
                var postEntity = JsonConvert.DeserializeObject<CertificateEntity>(jobjson.ToString());
                postEntity.CreateTime = DateTime.Now;
                postEntity.CreateUserId = user.UserId;
                postEntity.CreateUser = user.RealName;
                var getOne = cbll.selectLike(postEntity.CertificateName, postEntity.ParentId);
                if (getOne != null)
                {
                    message = "操作失败,已经存在！";
                    return new { info = message, code = result };
                }
                var ck = cbll.addCertificate(postEntity);
                result = ck ? 0 : 1;
                message = ck ? "操作成功" : "操作失败";
                return new { info = message, code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1, data = new CertificateEntity() };

            }

        }
        #endregion
        #region   用户证书
        /// <summary>
        /// 新增创建图片获取id
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetCertificateGuId()
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
                GetGuId(userId, "人员证书", out newpath, out id);
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
                var flist = fb.GetFilebyDescription(userId, "成员证书二维码");
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

                    if (!Directory.Exists(Path.Combine(filedir, "DocumentFile", "Certificate")))
                    {
                        Directory.CreateDirectory(Path.Combine(filedir, "DocumentFile", "Certificate"));
                    }
                    //保存路径
                    var newurl = Path.Combine(filedir, "DocumentFile", "Certificate", id + ".jpg");
                    //保存图片
                    image.Save(newurl);
                    //创建数据实体
                    var fileentity = new FileInfoEntity
                    {
                        FileId = Guid.NewGuid().ToString(),
                        FolderId = id,
                        RecId = userId,
                        FileName = System.IO.Path.GetFileName(newurl),
                        FilePath = "~/Resource/DocumentFile/Certificate/" + id + ".jpg",
                        FileType = "jpg",
                        FileExtensions = ".jpg",
                        Description = "成员证书二维码",
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
        /// 获取用户证件信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object getCertificate()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            UserEntity user = new UserBLL().GetEntity(userId);
            try
            {
                var entity = ucbll.getUserCertificateByuserId(userId);
                if (entity == null)
                {
                    return new { info = "查询成功", code = result, data = new List<UserCertificateEntity>() };
                }
                entity = entity.OrderByDescending(x => x.createtime).ToList();
                var gettime = DateTime.Now;
                var ckTime = new DateTime(gettime.Year, gettime.Month, gettime.Day);
                var ck = new DateTime(1980, 1, 1);
                foreach (var item in entity)
                {
                    item.state = string.Empty;

                    if (item.effectiveendtime > ck)
                    {

                        if (item.effectiveendtime < ckTime)
                        {
                            item.state = "已过期";
                        }
                        if (item.effectiveendtime.AddMonths(-3) <= ckTime && ckTime <= item.effectiveendtime)
                        {
                            item.state = "快过期";
                        }
                    }
                    if (item.effectivetime > ck)
                    {

                        if (item.effectivetime < ckTime)
                        {
                            item.state = "已过期";
                        }
                        if (item.effectivetime.AddMonths(-3) <= ckTime && ckTime <= item.effectivetime)
                        {
                            item.state = "快过期";
                        }
                    }
                    if (item.neweffectivetime > ck)
                    {

                        if (item.neweffectivetime < ckTime)
                        {
                            item.state = "已过期";
                        }

                        if (item.neweffectivetime.AddMonths(-3) <= ckTime && ckTime <= item.neweffectivetime)
                        {
                            item.state = "快过期";
                        }
                    }
                    if (item.CertificateName.Contains("特种"))
                    {
                        if (item.rechecktime > ck)
                        {
                            if (item.rechecktime.AddMonths(-3) <= ckTime && ckTime <= item.rechecktime)
                            {
                                if (string.IsNullOrEmpty(item.state))
                                {
                                    item.state += "应复审";
                                }
                                else
                                {
                                    item.state += ",应复审";
                                }
                            }
                        }
                    }

                }
                var group = entity.GroupBy(x => x.CertificateName.Split(',')[0]);
                var getgroup = new List<UserCertificateEntity>();
                foreach (var item in group)
                {
                    var get = entity.Where(x => x.CertificateName.Contains(item.Key));
                    getgroup.AddRange(get);
                }
                List<object> resultLis = new List<object>();
                foreach (var item in entity)
                {
                    var resultData = new
                    {
                        Id = item.Id,
                        CertificateName = item.CertificateName,
                        firsttime = item.firsttime > ck ? item.firsttime.ToString("yyyy-MM-dd") : "无",
                        effectivestarttime = item.effectivestarttime > ck ? item.effectivestarttime.ToString("yyyy-MM-dd") : "无",
                        approvaltime = item.approvaltime > ck ? item.approvaltime.ToString("yyyy-MM-dd") : "无",
                        neweffectivetime = item.neweffectivetime > ck ? item.neweffectivetime.ToString("yyyy-MM-dd") : "无",
                        effectivetime = item.effectivetime > ck ? item.effectivetime.ToString("yyyy-MM-dd") : "无",
                        getthetime = item.getthetime > ck ? item.getthetime.ToString("yyyy-MM-dd") : "无",
                        effectiveendtime = item.effectiveendtime > ck ? item.effectiveendtime.ToString("yyyy-MM-dd") : "无",
                        rechecktime = item.rechecktime > ck ? item.rechecktime.ToString("yyyy-MM-dd") : "无",
                        numbercode = item.numbercode,
                        userid = item.userid,
                        issue = item.issue,
                        createtime = item.createtime,
                        createuser = item.createuser,
                        createuserid = item.createuserid,
                        modifytime = item.modifytime,
                        modifyuser = item.modifyuser,
                        modifyuserid = item.modifyuserid,
                        Files = item.Files,
                        state = item.state,
                        path = item.path
                    };
                    resultLis.Add(resultData);
                }
                return new { info = "查询成功", code = result, data = resultLis };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new List<UserCertificateEntity>() };

            }

        }
        /// <summary>
        /// 获取证件详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object getUserCertificateById()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            string Id = dy.data;
            // UserEntity user = new UserBLL().GetEntity(userId);
            try
            {
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                var entity = ucbll.getUserCertificateById(userId, Id);
                if (entity == null)
                {
                    return new { info = "查询成功", code = result, data = entity };
                }
                foreach (var item in entity.Files)
                {
                    item.FilePath = item.FilePath.Replace("~/", url);
                }
                var Getpath = entity.Files.FirstOrDefault(x => x.Description == "成员证书二维码");
                if (Getpath == null)
                {
                    entity.path = "";
                }
                else
                {
                    entity.path = Getpath.FilePath;
                    entity.Files.Remove(Getpath);
                }
                var ck = new DateTime(1980, 1, 1);
                var resultData = new
                {
                    Id = entity.Id,
                    CertificateName = entity.CertificateName,
                    firsttime = entity.firsttime > ck ? entity.firsttime.ToString("yyyy-MM-dd") : "",
                    effectivestarttime = entity.effectivestarttime > ck ? entity.effectivestarttime.ToString("yyyy-MM-dd") : "",
                    approvaltime = entity.approvaltime > ck ? entity.approvaltime.ToString("yyyy-MM-dd") : "",
                    neweffectivetime = entity.neweffectivetime > ck ? entity.neweffectivetime.ToString("yyyy-MM-dd") : "",
                    effectivetime = entity.effectivetime > ck ? entity.effectivetime.ToString("yyyy-MM-dd") : "",
                    getthetime = entity.getthetime > ck ? entity.getthetime.ToString("yyyy-MM-dd") : "",
                    effectiveendtime = entity.effectiveendtime > ck ? entity.effectiveendtime.ToString("yyyy-MM-dd") : "",
                    rechecktime = entity.rechecktime > ck ? entity.rechecktime.ToString("yyyy-MM-dd") : "",
                    numbercode = entity.numbercode,
                    userid = entity.userid,
                    issue = entity.issue,
                    createtime = entity.createtime,
                    createuser = entity.createuser,
                    createuserid = entity.createuserid,
                    modifytime = entity.modifytime,
                    modifyuser = entity.modifyuser,
                    modifyuserid = entity.modifyuserid,
                    Files = entity.Files,
                    state = entity.state,
                    path = entity.path
                };
                return new { info = "查询成功", code = result, data = resultData };

            }
            catch (Exception ex)
            {

                return new { info = "查询失败：" + ex.Message, code = 1, data = new UserCertificateEntity() };

            }

        }


        /// <summary>
        /// 新增用户证书
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object addUserCertificate()
        {
            var result = 0;
            var message = string.Empty;

            try
            {
                string res = HttpContext.Current.Request.Form["json"];
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var entity = JsonConvert.DeserializeObject<UserCertificateEntity>(json["data"].ToString());
                UserEntity user = new UserBLL().GetEntity(userId);
                entity.createuserid = userId;
                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
                }
                if (!Directory.Exists(Path.Combine(filedir, "AppFile", "Certificate")))
                {
                    Directory.CreateDirectory(Path.Combine(filedir, "AppFile", "Certificate"));
                }
                if (entity.Files == null) entity.Files = new List<FileInfoEntity>();
                List<FileInfoEntity> deletefiles = entity.Files.ToList();
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                List<FileInfoEntity> deletefilesInfo = new List<FileInfoEntity>();

                var old = fileBll.GetFilesByRecIdNew(entity.Id).Where(x => x.Description != "成员证书二维码").ToList();
                var undeletefiles = old.Except(deletefiles, new FileCompare());
                deletefilesInfo = old.Except(undeletefiles, new FileCompare()).ToList();
                entity.Files = undeletefiles.ToList();
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
                        FolderId = entity.Id,
                        RecId = entity.Id,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/Certificate/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = file.ContentType.StartsWith("audio") ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now
                    };
                    file.SaveAs(Path.Combine(filedir, "AppFile", "Certificate", fileId + ext));
                    //保存附件信息
                    entity.Files.Add(fileentity);
                }

                ucbll.addUserCertificate(entity);
                for (int i = 0; i < deletefilesInfo.Count; i++)
                {

                    string url = Config.GetValue("FilePath") + deletefilesInfo[i].FilePath.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(deletefilesInfo[i].FilePath) && System.IO.File.Exists(url))
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
        public object UserCertificateFile()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;//业务记录Id
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                var data = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description != "成员证书二维码").ToList();
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
        public object UploadUserCertificate()
        {
            try
            {
                var GetJson = HttpContext.Current.Request["json"];
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(GetJson);
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
                        FilePath = "~/Resource/AppFile/Certificate/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0
                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Certificate"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Certificate");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Certificate\\" + fileId + ext);
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
        public object UpdateUserCertificate()
        {
            var result = 0;
            var message = string.Empty;

            try
            {
                string res = HttpContext.Current.Request.Form["json"];
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var entity = JsonConvert.DeserializeObject<UserCertificateEntity>(json["data"].ToString());
                UserEntity user = new UserBLL().GetEntity(userId);
                if (entity.Files == null) entity.Files = new List<FileInfoEntity>();
                List<FileInfoEntity> deletefiles = entity.Files.ToList();
                List<FileInfoEntity> deletefilesInfo = new List<FileInfoEntity>();

                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                var old = fileBll.GetFilesByRecIdNew(entity.Id).Where(x => x.Description != "成员证书二维码").ToList();
                var undeletefiles = old.Except(deletefiles, new FileCompare());
                deletefilesInfo = old.Except(undeletefiles, new FileCompare()).ToList();
                entity.Files = undeletefiles.ToList();
                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
                }

                if (!Directory.Exists(Path.Combine(filedir, "AppFile", "Certificate")))
                {
                    Directory.CreateDirectory(Path.Combine(filedir, "AppFile", "Certificate"));
                }
                var one = ucbll.getUserCertificateById(userId, entity.Id);
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
                        FolderId = entity.Id,
                        RecId = entity.Id,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/Certificate/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = file.ContentType.StartsWith("audio") ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now
                    };
                    file.SaveAs(Path.Combine(filedir, "AppFile", "Certificate", fileId + ext));
                    //保存附件信息
                    entity.Files.Add(fileentity);
                }
                ucbll.modifyUserCertificate(entity);
                for (int i = 0; i < deletefilesInfo.Count; i++)
                {

                    string url = Config.GetValue("FilePath") + deletefilesInfo[i].FilePath.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(deletefilesInfo[i].FilePath) && System.IO.File.Exists(url))
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
        /// 删除证件信息
        /// </summary>
        /// <returns></returns>
        public object DelUserCertificate()
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
                var old = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description != "成员证书二维码").ToList();

                ucbll.DelUserCertificate(id);
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
        #endregion
    }
}
