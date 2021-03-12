using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.AttendManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EmergencyManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.PushInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.AttendManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.EmergencyManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.PushInfoManage;

using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class EmergencyController : ApiController
    {
        EmergencyBLL ebll = new EmergencyBLL();
        PushInfoBLL pushBll = new PushInfoBLL();
        DepartmentBLL deptBll = new DepartmentBLL();
        UserBLL ubll = new UserBLL();
        //
        // GET: /Emergency/

        [HttpPost]
        public object GetList([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                string from = dy.data.from;
                string to = dy.data.to;
                string name = dy.data.name;
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                IList list = ebll.GetPageList(from, to, name, user.DepartmentId, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total).ToList();
                return new { code = 0, info = "获取数据成功", count = total, data = new { attends = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        //获取应急处置卡的类别
        [HttpPost]
        public object GetTypeList([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            string keyvalue = dy.data.keyvalue;
            var data = default(List<EmergencyCardTypeEntity>);
            UserEntity user = new UserBLL().GetEntity(userId);

            try
            {
                if (!string.IsNullOrEmpty(keyvalue))
                {
                    data = ebll.GetAllCardType(user.DepartmentCode).Where(x => x.TypeName.Contains(keyvalue)).ToList();

                }
                else
                {
                    data = ebll.GetAllCardType(user.DepartmentCode).ToList();

                }

                for (int i = 0; i < data.Count; i++)
                {
                    if (string.IsNullOrEmpty(data[i].ParentCardId))
                    {
                        data.Remove(data[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
            return new { code = result, info = message, data = data, count = data.Count };

        }

        //获取应急处置卡的明细
        [HttpPost]
        public object GetTypeDetailList([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            bool ispage = false;
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            //ios 无是否分页
            if (res.Contains("allowPaging"))
            {
                ispage = dy.allowPaging;
            }
            string userId = dy.userId;
            string keyvalueTwo = dy.data.keyvalueTwo;
            string CardId = dy.data.cardId;
            string typeid = dy.data.typeid;
            long pageIndex = dy.data.pageIndex;//当前索引页
            long pageSize = dy.data.pageSize;//每页记录数
            UserEntity user = new UserBLL().GetEntity(userId);
            var data = default(List<EmergencyEntity>);
            var returnData = new GetTypeDetailReturn();

            var ispush = false;
            try
            {
                var DeptConfig = Config.GetValue("EmergencyCard");
                var url = Config.GetValue("AppUrl");
                var urlPDF = Config.GetValue("pdfview");
                var deptName = deptBll.GetEntity(user.DepartmentId).FullName;
                if (DeptConfig.Contains(","))
                {
                    var deptSplit = DeptConfig.Split(',');
                    for (int i = 0; i < deptSplit.Count(); i++)
                    {

                        if (deptName == deptSplit[i])
                        {
                            ispush = true;
                            break;
                        }
                    }
                }
                else
                {
                    if (DeptConfig == deptName)
                    {
                        ispush = true;
                    }
                }

                data = ebll.GetList("", typeid, keyvalueTwo, CardId, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize), ispage).ToList();

                foreach (var item in data)
                {
                    if (!string.IsNullOrEmpty(item.FilePath))
                    {
                        item.FilePath = url + item.FilePath.Substring(2, item.FilePath.Length - 2);
                        item.urlFilePath = urlPDF + item.FileId;
                    }

                }
                returnData.ispush = ispush;
                returnData.entity = data;
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
            return new { code = result, info = message, data = returnData, count = returnData.entity.Count };

        }
        //添加阅读数据量
        [HttpPost]
        public object addReadNum([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            string CardId = dy.data.cardId;
            try
            {
                EmergencyEntity model = ebll.GetEntity(CardId);
                model.seenum = model.seenum > 0 ? model.seenum + 1 : 1;
                ebll.SaveEmergencyEntity(model);
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

            return new { info = message };

        }
        //推送阅读
        [HttpPost]
        public object PushRead([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            string pushid = dy.data.pushid;
            try
            {
                PushPersonEntity one = new PushPersonEntity();
                one.personid = userId;
                one.pushid = pushid;
                pushBll.PushRead(one);
            }
            catch (Exception ex)
            {
                return new { info = ex.Message };
            }

            return new { info = message };
        }


        //获取推送记录
        [HttpPost]
        public object GetPushList([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            string pushid = dy.data.pushid;
            var data = default(List<PushInfoEntity>);
            try
            {
                data = pushBll.GetPushInfoList(userId, pushid).ToList();
                foreach (var item in data)
                {
                    var allPerson = pushBll.GetPushPerson(item.pushid);
                    var NoreadPerson = allPerson.Where(x => x.isread == false).ToList();
                    item.noreadcount = NoreadPerson.Count();
                    item.count = allPerson.Count();
                    var ck = allPerson.FirstOrDefault(x => x.isread == false && x.personid == userId);
                    item.isread = ck == null ? true : false;
                    //item.NoreadPerson = new string[NoreadPerson.Count];
                    //for (int i = 0; i < item.NoreadPerson.Count(); i++)
                    //{
                    //    item.NoreadPerson[i] = NoreadPerson[i].person;
                    //}
                }
                data = data.OrderByDescending(x => x.createdate).ToList();
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
            return new { code = result, info = message, data = data, count = data.Count };

        }

        //获取推送记录
        [HttpPost]
        public object NoReadPerson([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            string pushid = dy.data.pushid;
            var data = default(List<PushInfoEntity>);
            try
            {
                data = pushBll.GetPushInfoList(userId, pushid).ToList();
                foreach (var item in data)
                {
                    var allPerson = pushBll.GetPushPerson(item.pushid);
                    var NoreadPerson = allPerson.Where(x => x.isread == false).ToList();
                    item.noreadcount = NoreadPerson.Count();
                    item.count = allPerson.Count();
                    item.NoreadPerson = new string[NoreadPerson.Count];
                    for (int i = 0; i < item.NoreadPerson.Count(); i++)
                    {
                        item.NoreadPerson[i] = NoreadPerson[i].person;
                    }
                }

            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
            return new { code = result, info = message, data = data[0].NoreadPerson, count = data.Count };

        }
        //推送消息
        public object goPushSave([FromBody]JObject json)
        {
            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            string detailid = dy.data.detailid;
            UserEntity user = new UserBLL().GetEntity(userId);
            try
            {
                var GetList = new UserBLL().GetList().ToList();
                string[] userList = new string[GetList.Count()];
                var deptName = deptBll.GetEntity(user.DepartmentId).FullName;
                PushInfoEntity model = new PushInfoEntity();
                model.pushid = Guid.NewGuid().ToString();
                model.pushuser = user.RealName;
                model.pushuserid = user.UserId;
                model.createdate = DateTime.Now;
                model.person = "";
                model.personid = "";
                model.content = "您收到【" + deptName + "】" + user.RealName + "发来的" + detailid.Split(',').Count() + "张应急处置卡，请及时进行学习。";
                model.detailid = detailid;
                List<PushPersonEntity> List = new List<PushPersonEntity>();
                for (int i = 0; i < GetList.Count(); i++)
                {
                    //model.person = string.IsNullOrEmpty(model.person) ? GetList[i].RealName : model.person + "," + GetList[i].RealName;
                    //model.personid = string.IsNullOrEmpty(model.personid) ? GetList[i].UserId : model.personid + "," + GetList[i].UserId;
                    userList[i] = GetList[i].Account;
                    var one = new PushPersonEntity();
                    one.id = Guid.NewGuid().ToString();
                    one.person = GetList[i].RealName;
                    one.personid = GetList[i].UserId;
                    one.pushid = model.pushid;
                    one.isread = false;
                    List.Add(one);
                }
                pushBll.SavePushInfo(model, List);
                //var ck = MessageClient.SendRequest(userList, model.pushid, "应急处置卡|" + model.pushid, "班组帮应急处置卡通知", model.content);

                //if (!ck)
                //{
                //    return new { code = 1, info = "推送消息失败!" };
                //}
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
            return new { code = result, info = message };


        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostFiles()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "No Files" };
            }

            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Emergency";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                //var model = Newtonsoft.Json.JsonConvert.DeserializeObject<DataBucket<string>>(json);
                var Evaluation = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<Evaluation>>(json);
                var userbll = new UserBLL();
                var user = userbll.GetEntity(Evaluation.UserId);

                var files = new List<FileInfoEntity>();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    var fileid = Guid.NewGuid().ToString();

                    var fileentity = new FileInfoEntity() { RecId = Evaluation.Data.keyvalue, CreateDate = DateTime.Now, CreateUserId = Evaluation.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);
                    string[] type = new string[] { "jpeg", "jpg", "mp3", "mpeg", "mp4" };
                    string one = string.Empty;
                    switch (file.Headers.ContentType.MediaType)
                    {
                        case "image/jpeg":
                            fileentity.Description = "照片";
                            one = file.Headers.ContentType.MediaType.ToString().Split('/')[1];
                            break;
                        case "image/jpg":
                            fileentity.Description = "照片";
                            one = file.Headers.ContentType.MediaType.ToString().Split('/')[1];
                            break;
                        case "image/mp3":
                            fileentity.Description = "音频";
                            one = file.Headers.ContentType.MediaType.ToString().Split('/')[1];
                            break;
                        case "audio/mpeg":
                            fileentity.Description = "音频";
                            one = file.Headers.ContentType.MediaType.ToString().Split('/')[1];
                            break;
                        case "video/mp4":
                            fileentity.Description = "视频";
                            one = file.Headers.ContentType.MediaType.ToString().Split('/')[1];
                            break;
                        default:
                            break;
                    }
                    var ck = false;
                    for (int i = 0; i < type.Length; i++)
                    {
                        if (filename.Contains(type[i]))
                        {
                            ck = true;
                            break;
                        }
                    }
                    if (!ck)
                    {
                        fileentity.FilePath = fileentity.FilePath + "." + one;
                        filename = filename + "." + one;
                    }

                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    files.Add(fileentity);

                }
                //评价内容
                var fileContextid = Guid.NewGuid().ToString();
                var fileContext = new FileInfoEntity() { RecId = Evaluation.Data.keyvalue, CreateDate = DateTime.Now, CreateUserId = Evaluation.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileContextid, FileName = "方案评估", ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = "" };
                fileContext.FilePath = Evaluation.Data.Plan;
                fileContext.Description = "评价";
                files.Add(fileContext);
                var fileEffectid = Guid.NewGuid().ToString();
                var fileEffect = new FileInfoEntity() { RecId = Evaluation.Data.keyvalue, CreateDate = DateTime.Now, CreateUserId = Evaluation.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileEffectid, FileName = "效果评估", ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = "" };
                fileEffect.FilePath = Evaluation.Data.Effect;
                fileEffect.Description = "评价"; ;
                files.Add(fileEffect);
                var filebll = new FileInfoBLL();
                //var deletlist = filebll.GetFilesByRecIdNew(Evaluation.Data.keyvalue);
                //foreach (var item in deletlist)
                //{

                //        filebll.DeleteFile(item.RecId, item.FileName, item.FilePath);

                //}
                filebll.SaveFiles(files);

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

    }
}



