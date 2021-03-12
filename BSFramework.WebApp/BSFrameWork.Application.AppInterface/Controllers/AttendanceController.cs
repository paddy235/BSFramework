using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.OndutyManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.OndutyManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Service.BusinessExceptions;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class AttendanceController : BaseApiController
    {
        [HttpPost]
        public object GetDeptAttendance()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var year = jobject.SelectToken("data.year").Value<int>();
            var month = jobject.SelectToken("data.month").Value<int>();
            var userid = jobject.Value<string>("userId");
            var data = default(List<UserAttendanceEntity>);
            var user = new UserBLL().GetEntity(userid);

            try
            {
                var bll = new WorkmeetingBLL();
                var isMenu = new bool[2];
                data = bll.GetAttendanceData2(user.DepartmentId, new DateTime(year, month, 1), new DateTime(year, month, 1).AddMonths(1).AddMinutes(-1), isMenu);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = data == null ? 0 : data.Count };
        }
        private bool[] getMenu(string userid)
        {
            var dept = new DepartmentBLL().GetList();
            var root = dept.FirstOrDefault(x => x.ParentId == "0");
            var get = dept.Where(x => x.ParentId == root.DepartmentId).OrderBy(x => x.EnCode).First();
            var dyresultS = MywebClient("MenuConfig/GetMenuList",
               "{'userid':'" + userid + "','data':{'id':'" + get.DepartmentId + "','platform':1,'themetype':0}}");
            bool[] IsMenu = new bool[2];
            if (dyresultS.Contains("人脸签到"))
            {
                IsMenu[0] = true;
            }
            else
            {
                IsMenu[0] = false;
            }
            if (dyresultS.Contains("考勤签到"))
            {
                IsMenu[1] = true;
            }
            else
            {
                IsMenu[1] = false;
            }

            return IsMenu;
        }
        private static string MywebClient(string url, string val)
        {
            var webclient = new WebClient();
            var ApiIp = Config.GetValue("ErchtmsApiUrl");
            NameValueCollection postVal = new NameValueCollection();
            postVal.Add("json", val);
            var getData = webclient.UploadValues(ApiIp + url, postVal);
            var result = System.Text.Encoding.UTF8.GetString(getData);
            NLog.LogManager.GetCurrentClassLogger().Info("windows终端-获取考情授权\r\n-->请求地址：{0}\r\n-->请求数据：{1}\r\n-->返回数据：{2}", url, val, result);
            return result;
        }

        [HttpPost]
        public object GetUserAttendance()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var year = jobject.SelectToken("data.year").Value<int>();
            var month = jobject.SelectToken("data.month").Value<int>();
            var userid = jobject.SelectToken("data.UserId").Value<string>();
            var data = default(UserAttendanceEntity);


            try
            {
                var bll = new WorkmeetingBLL();
                data = bll.GetMonthAttendance2(userid, new DateTime(year, month, 1), new DateTime(year, month, 1).AddMonths(1).AddMinutes(-1));
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data };
        }


        [HttpPost]
        public object GetUserAttendance2()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var from = jobject.SelectToken("data.from").Value<DateTime>();
            var to = jobject.SelectToken("data.to").Value<DateTime>();
            var userid = jobject.SelectToken("data.UserId").Value<string>();
            var data = new List<DayAttendanceEntity>();
            var list = new List<DateTime>();
            try
            {
                var bll = new WorkmeetingBLL();
                to = to.AddDays(1).AddMinutes(-1);
                data = bll.GetDayAttendance2(userid, from, to);
                //while (date <= to)
                //{
                //    //var d = bll.GetDayAttendance2(userid, date);
                //    //data.Add(new { Date = date.Date, Data = d });
                //    list.Add(date);
                //    date = date.AddDays(1);
                //}
                //Parallel.ForEach(list, x => data.Add(new { Date = x.Date, Data = bll.GetDayAttendance2(userid, x) }));
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }
            var datalist = data.Select(x => new { Date = x.Date, Data = x }).ToList();

            return new { code = result, info = message, data = datalist, count = datalist.Count };
        }

        /// <summary>
        /// 值班表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetDutyData()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var from = jobject.SelectToken("data.from").Value<DateTime>();
            var to = jobject.SelectToken("data.to").Value<DateTime>();
            var userid = jobject.Value<string>("userId");
            var data = default(List<object>);

            try
            {
                data = new List<object>();
                var user = new UserBLL().GetEntity(userid);
                var bll = new WorkmeetingBLL();
                var date = from;
                while (date <= to)
                {
                    var d = bll.GetDutyPerson(user.DepartmentId, date);
                    data.Add(new { Date = date.Date, Day = date.ToString("ddd"), Data = d == null ? string.Empty : string.Join(",", d.Select(x => x.UserName).Distinct()) });
                    date = date.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = data.Count };
        }

        /// <summary>
        /// 值班表人员详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetDutyDataDetail()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var from = jobject.SelectToken("data.from").Value<DateTime>();
            var to = jobject.SelectToken("data.to").Value<DateTime>();
            var userid = jobject.Value<string>("userId");
            var data = default(List<object>);

            try
            {
                data = new List<object>();
                var user = new UserBLL().GetEntity(userid);
                var bll = new WorkmeetingBLL();
                var date = from;
                while (date <= to)
                {
                    var d = bll.GetDutyPerson(user.DepartmentId, date);
                    var list = new List<UnSignRecordEntity>();
                    var ford = d.Where(x => x.ReasonRemark != null).OrderBy(x => x.StartTime);
                    foreach (var item in ford)
                    {
                        var ck = list.FirstOrDefault(x => x.UserId == item.UserId);
                        if (ck == null)
                        {
                            list.Add(item);
                        }
                        else
                        {
                            if (ck.ReasonRemark != item.ReasonRemark)
                            {
                                list.Add(item);
                            }
                        }

                    }

                    data.Add(new { Date = date.Date, PersonDetail = list });
                    date = date.AddDays(1);
                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = data.Count };
        }
        /// <summary>
        /// 天值班详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetDutyPerson()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var userid = jobject.Value<string>("userId");
            var date = jobject.SelectToken("data.Date").Value<DateTime>();
            var data = default(List<UnSignRecordEntity>);

            try
            {
                var user = new UserBLL().GetEntity(userid);
                var bll = new WorkmeetingBLL();
                data = bll.GetDutyPerson(user.DepartmentId, date);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            var dd = data.GroupBy(x => new { x.UserId, x.UserName }).Select(x => new { x.Key.UserId, x.Key.UserName, ReasonRemark = string.Join(",", x.Select(y => y.ReasonRemark).Distinct().OrderBy(y => y)) });

            return new { code = result, info = message, data = dd, count = dd.Count() };
        }


        /// <summary>
        /// 天值班详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetDayAttendance()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var userid = jobject.SelectToken("data.UserId").Value<string>();
            var date = jobject.SelectToken("data.Date").Value<DateTime>();
            var data = default(List<UnSignRecordEntity>);

            try
            {
                var user = new UserBLL().GetEntity(userid);
                var bll = new WorkmeetingBLL();
                data = bll.GetDayAttendance3(userid, date);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = data.Count };
        }

        /// <summary>
        /// 修改值班表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object PostDutyPerson()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ListParam<WorkmeetingEntity>>(json);

            try
            {
                var bll = new WorkmeetingBLL();
                foreach (var item in model.Data)
                {
                    item.DutyPerson.ForEach(x => x.UnSignRecordId = Guid.NewGuid().ToString());
                }
                bll.PostDutyPerson(model.Data);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        /// <summary>
        /// 删除值班人员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object PostDelDutyPerson()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var userid = jobject.SelectToken("data.UserId").Value<string>();
            var date = jobject.SelectToken("data.Date").Value<DateTime>();

            try
            {
                var bll = new WorkmeetingBLL();
                bll.PostDelDutyPerson(date, userid);

            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        /// <summary>
        /// 修改考勤
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object PostAttendance()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<WorkmeetingEntity>>(json);

            try
            {
                var bll = new WorkmeetingBLL();
                model.Data.DutyPerson.ForEach(x => x.UnSignRecordId = Guid.NewGuid().ToString());
                bll.PostAttendance(model.Data);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }
        #region 人脸考勤
        private FaceAttendanceBLL facebll = new FaceAttendanceBLL();
        private string url = BSFramework.Util.Config.GetValue("AppUrl");
        private FileInfoBLL fileBll = new FileInfoBLL();

        public class SelecAttendance
        {
            public string type { get; set; }
            public string start { get; set; }
            public string end { get; set; }
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetListJson(BaseDataModel<SelecAttendance> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                Pagination pagination = new Pagination();
                //pagination.p_kid = "id";
                //pagination.p_fields = "CreateUserName,CreateDate,ondutyuser,ondutyuserid,ondutyshift,ondutycontext,ondutytime,ondutydeptid,ondutydeptcode,ondutydept";
                //pagination.p_tablename = "wg_faceattendance";
                //pagination.conditionJson = "1=1";
                //pagination.sidx = "CreateDate";
                //pagination.sord = "asc";
                ////var watch = CommonHelper.TimerStart();
                var user = new UserBLL().GetEntity(dy.userId);
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
                var data = facebll.GetPagesList(pagination,JsonConvert.SerializeObject(dy.data), user.UserId);
                //var dataJson = data.ToJson();
                //var dataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FaceAttendanceEntity>>(dataJson);

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

        private class FaceAttendanceModel
        {
            public FaceAttendanceEntity entity { get; set; }
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
                var dy = JsonConvert.DeserializeObject<BaseDataModel<FaceAttendanceModel>>(GetJson);
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
                if (ck)
                {
                    WorkOrderBLL order = new WorkOrderBLL();
                    Entity.ondutydeptid = dept.DepartmentId;
                    Entity.ondutydeptcode = dept.EnCode;
                    Entity.ondutydept = dept.FullName;
                    var nowTime = DateTime.Now;
                    var start = nowTime.Date.AddDays(-1);
                    var end = nowTime.Date.AddDays(1).AddSeconds(-1);
                    var workState = order.workAttendance(start, end, Entity.ondutydeptid, Entity.ondutyuserid);
                    if (workState == "无效")
                    {
                        return new { code = 0, info = "操作成功,无效签到" };
                    }
                    Entity.ondutycontext = "扫码考勤签到";
                    Entity.ondutyshift = workState;
                    facebll.SaveForm("", Entity);
                }
                else
                {
                    facebll.SaveForm(Entity.id, Entity);
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
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\FaceAttendance"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\FaceAttendance");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\FaceAttendance\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);


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
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<FaceAttendanceEntity> InAttendance(ParamBucket<string[]> args)
        {
            var user = OperatorProvider.Provider.Current();

            try
            {
                var users = default(List<UserEntity>);
                using (var faceutil = new FaceUtil())
                {
                    users = faceutil.Valid(args.Data);
                }
                if (users == null) return new ListBucket<FaceAttendanceEntity>() { Success = false };

                List<FaceAttendanceEntity> list = new List<FaceAttendanceEntity>();
                foreach (var item in users)
                {
                    WorkOrderBLL order = new WorkOrderBLL();
                    var nowTime = DateTime.Now;
                    FaceAttendanceEntity entity = new FaceAttendanceEntity();
                    entity.id = Guid.NewGuid().ToString();
                    entity.CreateUserId = user.UserId;
                    entity.CreateUserName = user.UserName;
                    entity.CreateDate = nowTime;
                    entity.ondutycontext = "人脸考勤签到";
                    var deptID = item.DepartmentId;
                    var dept = new DepartmentBLL().GetEntity(deptID);
                    entity.ondutydept = dept.FullName;
                    entity.ondutydeptcode = dept.EnCode;
                    entity.ondutydeptid = dept.DepartmentId;
                    entity.ondutyuser = item.RealName;
                    entity.ondutyuserid = item.UserId;
                    entity.ondutytime = nowTime;
                    var start = nowTime.Date.AddDays(-1);
                    var end = nowTime.Date.AddDays(1).AddSeconds(-1);
                    var workState = order.workAttendance(start, end, entity.ondutydeptid, entity.ondutyuserid);
                    if (workState == "无效")
                    {
                        continue;
                    }
                    entity.ondutyshift = workState;
                    facebll.SaveForm("", entity);
                    list.Add(entity);
                }

                return new ListBucket<FaceAttendanceEntity>() { Success = true, Data = list };
            }
            catch (Exception ex)
            {
                return new ListBucket<FaceAttendanceEntity>() { Success = false, Message = ex.Message };
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

                    if (!Directory.Exists(Path.Combine(filedir, "DocumentFile", "FaceAttendance")))
                    {
                        Directory.CreateDirectory(Path.Combine(filedir, "DocumentFile", "FaceAttendance"));
                    }
                    //保存路径
                    var newurl = Path.Combine(filedir, "DocumentFile", "FaceAttendance", id + ".jpg");
                    //保存图片
                    image.Save(newurl);
                    //创建数据实体
                    var fileentity = new FileInfoEntity
                    {
                        FileId = Guid.NewGuid().ToString(),
                        FolderId = id,
                        RecId = keyvalue,
                        FileName = System.IO.Path.GetFileName(newurl),
                        FilePath = "~/Resource/DocumentFile/FaceAttendance/" + id + ".jpg",
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
        #endregion



    }
}
