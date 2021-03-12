using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.OndutyManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.OndutyManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class OndutyController : BaseApiController
    {
        private OndutyBLL bll = new OndutyBLL();
        private OndutyRecordBLL bllrecord = new OndutyRecordBLL();
        private string url = BSFramework.Util.Config.GetValue("AppUrl");
        private FileInfoBLL fileBll = new FileInfoBLL();
        #region face onduty
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetListJson(BaseDataModel<getList> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                Pagination pagination = new Pagination();
                //pagination.p_kid = "id";
                //pagination.p_fields = "CreateUserName,CreateDate,ondutyuser,ondutyuserid,ondutyshift,ondutycontext,ondutytime,ondutydeptid,ondutydeptcode,ondutydept";
                //pagination.p_tablename = "wg_onduty";
                //pagination.conditionJson = "1=1";
                //pagination.sidx = "CreateDate";
                //pagination.sord = "desc";
                //var watch = CommonHelper.TimerStart();
                var user = new UserBLL().GetEntity(dy.userId);
               // string queryJson = "{}";
                if (dy.allowPaging)
                {
                    pagination.page = dy.pageIndex;
                    pagination.rows = dy.pageSize;
                }
                else
                {
                    pagination.page = 1;
                    pagination.rows = 2000;
                }
                //if (dy.data.type == "0")
                //{
                //    pagination.conditionJson += string.Format(" and ondutyuserid='{0}'", dy.userId);
                //}
                //if (!string.IsNullOrEmpty(dy.data.start))
                //{
                //    pagination.conditionJson += string.Format(" and '{0}'<=ondutytime", dy.data.start);

                //}
                //if (!string.IsNullOrEmpty(dy.data.end))
                //{
                //    pagination.conditionJson += string.Format(" and ondutytime<='{0}'", dy.data.end);

                //}
                var data = bll.GetPagesList(pagination, JsonConvert.SerializeObject(dy.data),user.UserId);
                //var dataJson = data.ToJson();
                //var dataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OndutyEntity>>(dataJson);

                foreach (var item in data)
                {
                    var fileList = fileBll.GetFilesByRecIdNew(item.id).ToList();
                    if (fileList.Count == 0)
                    {
                        item.Files = new List<FileInfoEntity>();
                    }
                    else
                    {
                        foreach (var items in fileList)
                        {
                            items.FilePath = items.FilePath.Replace("~/", url);
                        }
                        item.Files = fileList;
                    }


                }
                return new { code = result, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }

        }

        public class getList
        {
            public string type { get; set; }
            public string start { get; set; }
            public string end { get; set; }
        }

        private class OndutyModel
        {
            public OndutyEntity entity { get; set; }
            public string DelKeys { get; set; }
        }

        /// 上传图片删除图片
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object UploadOnduty()
        {
            try
            {
                var GetJson = HttpContext.Current.Request["json"];
                var dy = JsonConvert.DeserializeObject<BaseDataModel<OndutyModel>>(GetJson);
                string userId = dy.userId;
                var Entity = dy.data.entity;
                var ck = false;
                if (string.IsNullOrEmpty(Entity.id))
                {
                    ck = true;
                    Entity.id = Guid.NewGuid().ToString();
                }
                string id = Entity.id;
                UserEntity user = new UserBLL().GetEntity(Entity.ondutyuserid);
                var dept = new DepartmentBLL().GetEntity(Entity.ondutydeptid);
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
                        FilePath = "~/Resource/AppFile/Onduty/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Onduty"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Onduty");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Onduty\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);


                }
                if (ck)
                {
                    Entity.ondutydeptid = dept.DepartmentId;
                    Entity.ondutydeptcode = dept.EnCode;
                    Entity.ondutydept = dept.FullName;
                    bll.SaveForm("", Entity);
                }
                else
                {
                    bll.SaveForm(Entity.id, Entity);
                }
                return new { code = 0, info = "操作成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// app签到
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<OndutyEntity> GetCkImg(ParamBucket<string> args)
        {
            var user = OperatorProvider.Provider.Current();

            try
            {
                var validUser = default(UserEntity);
                using (var faceutil = new FaceUtil())
                {
                    validUser = faceutil.Valid(args.Data);
                }
                if (validUser == null)
                {
                    var keyvalue = "79e16661-6a5c-47b8-a50e-93c28a9ba691";
                    var type = "duty";
                    var path = "";
                    var id = "";
                    GetGuId(keyvalue, type, out path, out id);
                    #endregion
                    var reEntity = new OndutyEntity();
                    reEntity.path = path.Replace("~/", url);

                    return new ModelBucket<OndutyEntity>() { Success = false, Message = "签到失败", Data = reEntity };
                }

                var nowTime = DateTime.Now;
                OndutyEntity entity = new OndutyEntity();
                entity.id = Guid.NewGuid().ToString();
                entity.CreateUserId = user.UserId;
                entity.CreateUserName = user.UserName;
                entity.CreateDate = nowTime;
                entity.ondutycontext = "人脸识别签到";
                var deptID = validUser.DepartmentId;
                var dept = new DepartmentBLL().GetEntity(deptID);
                entity.ondutydept = dept.FullName;
                entity.ondutydeptcode = dept.EnCode;
                entity.ondutydeptid = dept.DepartmentId;
                entity.ondutyuser = validUser.RealName;
                entity.ondutyuserid = validUser.UserId;
                entity.ondutytime = nowTime;
                entity.ondutyshift = nowTime.Hour <= 12 ? "白班" : "夜班";
                bll.SaveForm("", entity);
                return new ModelBucket<OndutyEntity> { Success = true, Data = entity };

            }
            catch (Exception ex)
            {
                return new ModelBucket<OndutyEntity>() { Success = false, Message = ex.Message };
            }
        }

        private void GetGuId(string keyvalue, string type, out string newpath, out string id)
        {
            try
            {
                var user = OperatorProvider.Provider.Current();
                FileInfoBLL fb = new FileInfoBLL();
                var flist = fb.GetFilebyDescription(keyvalue, type);
                if (flist != null)
                {
                    newpath = flist.FilePath;
                    id = flist.FolderId;
                }
                else
                {
                    //新增创建新的证书id
                    id = keyvalue;
                    //二维码画图
                    var encoder = new QRCodeEncoder();
                    var image = encoder.Encode(id + "|" + type, Encoding.UTF8);
                    var filedir = BSFramework.Util.Config.GetValue("FilePath");
                    if (!System.IO.Directory.Exists(filedir))
                    {
                        System.IO.Directory.CreateDirectory(filedir);
                    }

                    if (!Directory.Exists(Path.Combine(filedir, "DocumentFile", "Onduty")))
                    {
                        Directory.CreateDirectory(Path.Combine(filedir, "DocumentFile", "Onduty"));
                    }
                    //保存路径
                    var newurl = Path.Combine(filedir, "DocumentFile", "Onduty", id + ".jpg");
                    //保存图片
                    image.Save(newurl);
                    //创建数据实体
                    var fileentity = new FileInfoEntity
                    {
                        FileId = Guid.NewGuid().ToString(),
                        FolderId = id,
                        RecId = keyvalue,
                        FileName = System.IO.Path.GetFileName(newurl),
                        FilePath = "~/Resource/DocumentFile/Onduty/" + id + ".jpg",
                        FileType = "jpg",
                        FileExtensions = ".jpg",
                        Description = type,
                        FileSize = "0",
                        DeleteMark = 0,
                        CreateUserId = user.UserId,
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

        #region 
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetRecordListJson(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                Pagination pagination = new Pagination();
                //pagination.p_kid = "id";
                //pagination.p_fields = "CreateUserName,CreateDate,ondutyuser,ondutyuserid,ondutyshift,ondutycontext,ondutytime,ondutydeptid,ondutydeptcode,ondutydept";
                //pagination.p_tablename = "wg_ondutyrecord";
                //pagination.conditionJson = "1=1";
                //pagination.sidx = "CreateDate";
                //pagination.sord = "desc";
                //var watch = CommonHelper.TimerStart();
                var user = new UserBLL().GetEntity(dy.userId);
                //string queryJson = "{}";
                if (dy.allowPaging)
                {
                    pagination.page = dy.pageIndex;
                    pagination.rows = dy.pageSize;
                }
                else
                {
                    pagination.page = 1;
                    pagination.rows = 2000;
                }
                //if (dy.data == "0")
                //{
                //    pagination.conditionJson += string.Format(" and ondutyuserid='{0}'", dy.userId);
                //}
                string queryJson ="{}";
                if (dy.data == "0")
                {
                    queryJson = "{'type':'0'}";
                }
                var data = bll.GetPagesList(pagination, queryJson,dy.userId);
                //var dataJson = data.ToJson();
                //var dataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OndutyRecordEntity>>(dataJson);

                foreach (var item in data)
                {
                    var fileList = fileBll.GetFilesByRecIdNew(item.id).ToList();
                    if (fileList.Count == 0)
                    {
                        item.Files = new List<FileInfoEntity>();
                    }
                    else
                    {
                        foreach (var items in fileList)
                        {
                            items.FilePath = items.FilePath.Replace("~/", url);
                        }
                        item.Files = fileList;
                    }


                }
                return new { code = result, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }

        }

        private class OndutyRecordModel
        {
            public OndutyRecordEntity entity { get; set; }
            public string DelKeys { get; set; }
        }

        /// 上传图片删除图片
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object UploadOndutyRecord()
        {
            try
            {
                var GetJson = HttpContext.Current.Request["json"];
                var dy = JsonConvert.DeserializeObject<BaseDataModel<OndutyRecordModel>>(GetJson);
                string userId = dy.userId;
                var Entity = dy.data.entity;
                var ck = false;
                if (string.IsNullOrEmpty(Entity.id))
                {
                    ck = true;
                    Entity.id = Guid.NewGuid().ToString();
                }
                string id = Entity.id;
                UserEntity user = new UserBLL().GetEntity(Entity.ondutyuserid);
                var dept = new DepartmentBLL().GetEntity(Entity.ondutydeptid);
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
                        FilePath = "~/Resource/AppFile/OndutyRecord/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\OndutyRecord"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\OndutyRecord");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\OndutyRecord\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);


                }
                if (ck)
                {
                    Entity.ondutydeptid = dept.DepartmentId;
                    Entity.ondutydeptcode = dept.EnCode;
                    Entity.ondutydept = dept.FullName;
                    bllrecord.SaveForm("", Entity);
                }
                else
                {
                    bllrecord.SaveForm(Entity.id, Entity);
                }
                return new { code = 0, info = "操作成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        #endregion
    }
}
