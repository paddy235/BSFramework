using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Busines.EmergencyManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Service.BusinessExceptions;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Bst.Fx.Uploading;
using Bst.ServiceContract.MessageQueue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
using TaskScheduler;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class TrainingController : ApiController
    {

        /// <summary>
        /// 获取我的危险预知训练
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetMyTrainings([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(IList<DangerEntity>);

            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                int pageIndex = json.Value<int>("pageIndex");//当前索引页
                int pageSize = json.Value<int>("pageSize");//每页记录数
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);

                var bll = new DangerBLL();
                data = bll.GetMyTrainings(userId, pageSize, pageIndex, out total);

                PostBLL pbll = new PostBLL();
                PeopleBLL peobll = new PeopleBLL();
                //var dutys = getNewData(user.UserId, "");
                foreach (var item in data)
                {
                    item.Persons = string.Join(",", item.JobUsers.Select(x => x.UserName));
                    var people = new PeopleEntity();
                    var dutydanger = new DutyDangerEntity();
                    foreach (var jobuser in item.JobUsers)
                    {
                        people = peobll.GetEntity(jobuser.UserId);
                        if (people != null)
                        {
                            if (!string.IsNullOrEmpty(people.RoleDutyId))
                            {
                                dutydanger = peobll.GetDutyDangerEntityByRole(people.RoleDutyId);
                                if (dutydanger != null) jobuser.DangerDutyContent = dutydanger.DutyContent;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = total };
        }
        public List<RoleEntity> getNewData(string userid, string deptid)
        {
            var dict = new
            {
                data = deptid,
                userid = userid,
                tokenid = ""
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Post", "GetPostByDeptId"), "json=" + JsonConvert.SerializeObject(dict));
            var ret = JsonConvert.DeserializeObject<RetDataModel>(res);
            return JsonConvert.DeserializeObject<List<RoleEntity>>(ret.data.ToString());
        }
        /// <summary>
        /// 获取危险预知训练
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<DangerEntity> GetTrainings([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(List<DangerEntity>);
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                int pageIndex = json.Value<int>("pageIndex");//当前索引页
                int pageSize = json.Value<int>("pageSize");//每页记录数
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);

                var bll = new DangerBLL();

                if (user.RoleName.Contains("班组长") || (user.RoleName.Contains("班组级用户") && user.RoleName.Contains("负责人")))
                    data = bll.GetTrainings(user.DepartmentId, pageSize, pageIndex, out total).ToList();
                else
                    data = bll.GetMyTrainings(userId, pageSize, pageIndex, out total).ToList();

                PostBLL pbll = new PostBLL();
                PeopleBLL peobll = new PeopleBLL();


                foreach (var item in data)
                {
                    //if (!string.IsNullOrEmpty(item.Photo)) item.Photo = string.Format("{0}://{1}{2}", this.Request.RequestUri.Scheme, this.Request.RequestUri.Host, item.Photo);
                    foreach (var item1 in item.Files)
                    {
                        item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                    }
                    var people = new PeopleEntity();
                    var dutydanger = new DutyDangerEntity();
                    foreach (var jobuser in item.JobUsers)
                    {
                        people = peobll.GetEntity(jobuser.UserId);
                        if (people != null)
                        {
                            if (!string.IsNullOrEmpty(people.RoleDutyId))
                            {
                                dutydanger = peobll.GetDutyDangerEntityByRole(people.RoleDutyId);
                                if (dutydanger != null) jobuser.DangerDutyContent = dutydanger.DutyContent;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            if (data == null) data = new List<DangerEntity>();

            return new ListBucket<DangerEntity>() { Data = data, Success = true, Total = total };
        }

        [HttpPost]
        public object GetTrainingDetail([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var data = default(DangerEntity);
            try
            {
                string res = jobject.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userId;
                string id = dy.data;
                UserEntity user = new UserBLL().GetEntity(userId);
                var url = ConfigurationManager.AppSettings["AppUrl"].ToString();

                var charary = id.ToCharArray();
                if (charary.Length == 37)
                {
                    var newary = new char[36];
                    for (int i = 0; i < newary.Length; i++)
                    {
                        newary[i] = charary[i + 1];
                    }
                    id = new string(newary);
                }

                var bll = new DangerBLL();
                data = bll.GetTrainingDetail(id);
                if (data != null && data.Files != null)
                {
                    foreach (var item in data.Files)
                    {
                        item.FilePath = item.FilePath.Replace("~/", url);
                        if (item.Description == "视频")
                        {
                            if (!string.IsNullOrEmpty(item.OtherUrl))
                            {
                                if (new Uploader().Query(item.OtherUrl))
                                    item.FilePath = item.OtherUrl;
                            }
                        }
                    }
                }
                PeopleBLL peobll = new PeopleBLL();
                foreach (var jobuser in data.JobUsers)
                {
                    var people = peobll.GetEntity(jobuser.UserId);
                    if (people != null && !string.IsNullOrEmpty(people.RoleDutyId))
                    {
                        var dutydanger = peobll.GetDutyDangerEntityByRole(people.RoleDutyId);
                        if (dutydanger != null) jobuser.DangerDutyContent = dutydanger.DutyContent;
                    }
                }

                data.Evaluateions = new WorkmeetingBLL().GetEntityEvaluate(data.Id);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = 1 };
        }

        [HttpPost]
        public object PostTraining([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var data = json["data"];
                DangerEntity entity = JsonConvert.DeserializeObject<DangerEntity>(data.ToString());
                UserEntity user = new UserBLL().GetEntity(userId);

                var bll = new DangerBLL();
                bll.PostTraining(entity);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object BeginTraining([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var data = json["data"];
                DangerEntity entity = JsonConvert.DeserializeObject<DangerEntity>(data.ToString());
                UserEntity user = new UserBLL().GetEntity(userId);

                entity.CreateDate = DateTime.Now;

                var bll = new DangerBLL();
                bll.BeginTraining(entity);
            }
            catch (TrainingExcuteException ex)
            {
                result = 2;
                message = ex.Message;
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object FinishTraining([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var data = json["data"];
                DangerEntity entity = JsonConvert.DeserializeObject<DangerEntity>(data.ToString());
                UserEntity user = new UserBLL().GetEntity(userId);

                entity.CreateDate = DateTime.Now;

                var bll = new DangerBLL();
                bll.FinishTraining(entity);

                var messagebll = new MessageBLL();
                messagebll.FinishTodo(Config.GetValue("CustomerModel"), entity.Id);

            }
            catch (TrainingExcuteException ex)
            {
                result = 2;
                message = ex.Message;
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object UploadTraining()
        {
            var result = 0;
            var message = string.Empty;
            var data = default(DangerEntity);
            try
            {
                string res = HttpContext.Current.Request.Form["json"];
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var datajson = json["data"];
                DangerEntity entity = JsonConvert.DeserializeObject<DangerEntity>(datajson.ToString());
                for (int i = 0; i < entity.Files.Count; i++)
                {
                    if (entity.Files[i].State != 1)
                    {
                        entity.Files.Remove(entity.Files[i]);
                        i--;
                    }
                }

                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
                }

                if (!Directory.Exists(Path.Combine(filedir, "AppFile", "Training")))
                {
                    Directory.CreateDirectory(Path.Combine(filedir, "AppFile", "Training"));
                }

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
                        FilePath = "~/Resource/AppFile/Training/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = file.ContentType.StartsWith("audio") ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now
                    };
                    file.SaveAs(Path.Combine(filedir, "AppFile", "Training", fileId + ext));
                    //保存附件信息
                    entity.Files.Add(fileentity);
                }

                UserEntity user = new UserBLL().GetEntity(userId);

                entity.CreateDate = DateTime.Now;

                var bll = new DangerBLL();
                bll.UploadTraining(entity);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object GetTrainingUsers()
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);
            //var data = default(IList<JobUserEntity>);
            dynamic data = new object();
            try
            {
                UserEntity user = new UserBLL().GetEntity(model.UserId);

                var bll = new DangerBLL();
                data = bll.GetTrainingUsers(model.Data, model.UserId, model.PageSize, model.PageIndex, out total);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = total };
        }

        [HttpPost]
        public object GetDangerous([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(IList<DangerTemplateEntity>);
            string res = jobject.Value<string>("json");
            var json = JObject.Parse(res.ToString());
            int pageIndex = json.Value<int>("pageIndex");//当前索引页
            int pageSize = json.Value<int>("pageSize");//每页记录数
            string userId = json.Value<string>("userId");
            string query = json.Value<string>("data");

            UserEntity user = new UserBLL().GetEntity(userId);

            var bll = new DangerBLL();
            data = bll.GetDangerous(query, pageSize, pageIndex, out total);

            return new { code = result, info = message, data = data, count = total };
        }

        [HttpPost]
        public object GetMeasures([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(IList<DangerTemplateEntity>);
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                int pageIndex = json.Value<int>("pageIndex");//当前索引页
                int pageSize = json.Value<int>("pageSize");//每页记录数
                string userId = json.Value<string>("userId");
                string query = json.Value<string>("data");

                UserEntity user = new UserBLL().GetEntity(userId);

                var bll = new DangerBLL();
                data = bll.GetMeasures(query, pageSize, pageIndex, out total);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = total };
        }

        [HttpPost]
        public object PostTrainingItem([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var data = string.Empty;
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                MeasuresEntity entity = JsonConvert.DeserializeObject<MeasuresEntity>(json["data"].ToString());
                UserEntity user = new UserBLL().GetEntity(userId);
                entity.CreateUserId = userId;

                var bll = new DangerBLL();
                entity.CreateDate = DateTime.Now;
                data = bll.AddItem(entity).Id;
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data };
        }

        [HttpPost]
        public object FinishTraining2()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);
            var data = default(DangerEntity);

            try
            {
                var bll = new DangerBLL();
                data = bll.FinishTraining2(model.Data);

                var messagebll = new MessageBLL();
                messagebll.FinishTodo(Config.GetValue("CustomerModel"), model.Data.Id);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data };
        }

        [HttpPost]
        public object FindTrainings()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);
            var data = default(List<DangerEntity>);
            UserEntity user = new UserBLL().GetEntity(model.UserId);
            var total = 0;
            try
            {
                var trainingtype = Config.GetValue("TrainingType");
                if (trainingtype == "人身风险预控")
                {
                    HumanDangerBLL bll1 = new HumanDangerBLL();
                    var data1 = bll1.Find(model.Data, user.DepartmentId, model.PageSize, model.PageIndex, out total);
                    data = data1.Select(x => new DangerEntity() { JobId = x.HumanDangerId.ToString(), JobName = x.Task, JobAddress = x.TaskArea }).ToList();
                }
                else
                {
                    var bll = new DangerBLL();
                    data = bll.FindTrainings(model.Data, int.MaxValue);
                    data = data.Where(x => user.DepartmentCode.StartsWith(x.DeptCode) || x.DeptCode.StartsWith(user.DepartmentCode)).ToList();
                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = total };
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
            var fold = "Training";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);

                var userbll = new UserBLL();
                var user = userbll.GetEntity(model.UserId);

                var files = new List<FileInfoEntity>();
                var filebll = new FileInfoBLL();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    if (filebll.Exist(filename))
                        continue;

                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { RecId = model.Data, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);

                    switch (file.Headers.ContentType.MediaType)
                    {
                        case "image/jpeg":
                            fileentity.Description = "照片";
                            break;
                        case "image/jpg":
                            fileentity.Description = "照片";
                            break;
                        case "image/png":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mpeg":
                            fileentity.Description = "音频";
                            break;
                        case "audio/mp3":
                            fileentity.Description = "音频";
                            break;
                        case "video/mp4":
                            fileentity.Description = "视频";
                            break;
                        default:
                            break;
                    }
                    files.Add(fileentity);

                }
                filebll.SaveFiles(files);

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        public object AddTraining()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);
            var data = default(DangerEntity);

            try
            {
                model.Data.CreateDate = DateTime.Now;
                model.Data.CreateUserId = model.UserId;
                model.Data.OperUserId = model.UserId;
                model.Data.OperDate = DateTime.Now;
                model.Data.Status = 0;
                model.Data.Sno = DateTime.Now.ToString("yyyyMMddHHmmss");
                var bll = new DangerBLL();
                data = bll.AddTraining(model.Data);
                PeopleBLL peobll = new PeopleBLL();
                if (data.JobUsers != null)
                {
                    foreach (var jobuser in data.JobUsers)
                    {
                        var people = peobll.GetEntity(jobuser.UserId);
                        if (!string.IsNullOrEmpty(people.RoleDutyId))
                        {
                            var dutydanger = peobll.GetDutyDangerEntityByRole(people.RoleDutyId);
                            if (dutydanger != null) jobuser.DangerDutyContent = dutydanger.DutyContent;
                        }
                    }
                }
                var messagebll = new MessageBLL();
                if (data != null)
                    messagebll.SendMessage(Config.GetValue("CustomerModel"), data.Id);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data };
        }

        [HttpPost]
        public object TrainingScore()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);

            try
            {
                model.Data.OperUserId = model.UserId;
                model.Data.OperDate = DateTime.Now;
                var bll = new DangerBLL();
                bll.TrainingScore(model.Data);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object GetTrainingData()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var from = jobject.SelectToken("data.from").Value<DateTime>();
            var to = jobject.SelectToken("data.to").Value<DateTime>();
            var userid = jobject.Value<string>("userId");
            var pageSize = jobject.Value<int>("pageSize");
            var pageIndex = jobject.Value<int>("pageIndex");
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            var data = default(List<DangerEntity>);
            var total = 0;

            try
            {
                to = to.AddDays(1).AddSeconds(-1);
                var bll = new DangerBLL();
                data = bll.GetTrainingData(userid, from, to, pageSize, pageIndex, out total);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            if (data != null)
            {
                foreach (var item in data)
                {
                    foreach (var item1 in item.Files)
                    {
                        item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                    }
                }
            }

            return new { code = result, info = message, data = data, count = total };
        }

        [HttpPost]
        public async Task<HttpResponseMessage> UploadVideo()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "No Files" };
            }

            var result = 0;
            var message = string.Empty;

            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var guid = provider.FormData.Get("uuid");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<VideoModel>>(json);

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    var path = Path.Combine(root, model.Data.serialNum);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    if (File.Exists(Path.Combine(path, filename)))
                        File.Delete(Path.Combine(path, filename));

                    File.Move(file.LocalFileName, Path.Combine(path, filename));
                }

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        public object MergeVideo()
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<VideoModel>>(json);
            var userbll = new UserBLL();
            var user = userbll.GetEntity(model.UserId);
            var root = HttpContext.Current.Server.MapPath("~/files");
            var fileid = model.Data.serialNum;
            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Training";

            var fileentity = new FileInfoEntity() { RecId = model.Data.Id, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = model.Data.FileName, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(model.Data.FileName) };
            fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(model.Data.FileName);
            fileentity.Description = "视频";

            if (!Directory.Exists(Path.Combine(filepath, fold)))
                Directory.CreateDirectory(Path.Combine(filepath, fold));


            NLog.LogManager.GetCurrentClassLogger().Info($"{filepath},{fold},{fileid},{model.Data.FileName}");
            if (!File.Exists(Path.Combine(filepath, fold, fileid + Path.GetExtension(model.Data.FileName))))
            {
                try
                {
                    using (var writer = File.Create(Path.Combine(filepath, fold, fileid + Path.GetExtension(model.Data.FileName))))
                    {
                        var buffer = new byte[2048];
                        for (int i = 0; i < model.Data.Count; i++)
                        {
                            using (var reader = File.OpenRead(Path.Combine(root, model.Data.serialNum, string.Format("{0}_{1}_{2}.tmp", Path.GetFileNameWithoutExtension(model.Data.FileName), model.Data.serialNum, i))))
                            {
                                var size = 1;
                                while (size > 0)
                                {
                                    size = reader.Read(buffer, 0, buffer.Length);
                                    if (size == 0) break;

                                    writer.Write(buffer, 0, size);
                                }
                                reader.Close();
                            }
                        }
                        writer.Flush();
                        writer.Close();
                    }

                    Directory.Delete(Path.Combine(root, model.Data.serialNum), true);

                    var filebll = new FileInfoBLL();
                    filebll.SaveFiles(new List<FileInfoEntity>() { fileentity });

                    var logger = NLog.LogManager.GetCurrentClassLogger();
                    logger.Info("上传队列 {0}", fileentity.FileId);

                    using (var factory = new ChannelFactory<IQueueService>("upload"))
                    {
                        var channel = factory.CreateChannel();
                        channel.Upload(fileentity.FileId);
                    }
                }
                catch (Exception ex)
                {
                    result = 1;
                    message = ex.Message;
                }
            }


            return new { code = result, info = message, count = total };
        }

        private void PushVideo(FileInfoEntity entity)
        {
            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();


            var url = ConfigurationManager.AppSettings["videosystem"].ToString();
            var request = HttpWebRequest.Create(url);
            request.Method = "GET";
        }

        [HttpPost]
        public object GetFileList()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);
            var data = default(List<FileInfoEntity>);

            try
            {
                var filebll = new FileInfoBLL();
                data = filebll.GetFileList(model.Data);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            if (data != null)
            {
                foreach (var item in data)
                {
                    item.FilePath = item.FilePath.Replace("~/", url);
                    if (!string.IsNullOrEmpty(item.OtherUrl))
                    {
                        if (new Uploader().Query(item.OtherUrl))
                            item.FilePath = item.OtherUrl;
                    }
                }
            }

            return new { code = result, info = message, data = data };
        }

        /// <summary>
        /// 获取危险预知训练（班组帮）
        /// 与终端有业务差别
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetTraining()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);

            var bll = new DangerBLL();
            var data = bll.GetTraining(model.Data, model.UserId);

            if (data == null) return new { code = result, info = message, data = data };

            var jobuser = data.JobUsers.Find(x => x.UserId == model.UserId);
            switch (data.Status)
            {
                case 0:
                    if (jobuser.JobType == "ischecker")
                    {
                        data.StatusDescription = "危险辨识并确认";
                    }
                    else
                    {
                        switch (jobuser.DangerStatus)
                        {
                            case 2:
                                data.StatusDescription = "负责人确认";
                                break;
                            default:
                                data.StatusDescription = "危险辨识";
                                break;
                        }
                    }
                    break;
                case 1:
                    if (jobuser.JobType == "ischecker")
                    {
                        data.StatusDescription = "措施落实并确认";
                    }
                    else
                    {
                        switch (jobuser.DangerStatus)
                        {
                            case 4:
                                data.StatusDescription = "负责人确定落实";
                                break;
                            default:
                                data.StatusDescription = "措施落实并确认";
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }

            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            if (data.Files != null)
            {
                foreach (var item in data.Files)
                {
                    item.FilePath = url + item.FilePath.Replace("~/", string.Empty);
                }
            }

            return new { code = result, info = message, data = data };
        }

        /// <summary>
        /// 获取危险预知训练列表（班组帮）
        /// 与终端有业务差别
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetUserTrainings()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);

            var bll = new DangerBLL();
            var data = bll.GetUserTrainings(model.UserId, model.Data);

            return new { code = result, info = message, data, count = data.Count };
        }

        /// <summary>
        /// 作业人保存训练项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UpdateTraingingItem()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);

            var bll = new DangerBLL();
            var data = bll.UpdateTraingingItem(model.Data, model.UserId);

            return new { code = result, info = message, data };
        }

        /// <summary>
        /// 作业人提交训练项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SubmitTraingingItem()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);

            var bll = new DangerBLL();
            var data = bll.SubmitTraingingItem(model.Data, model.UserId);

            return new { code = result, info = message, data };
        }

        /// <summary>
        /// 负责人保存训练项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UpdateTraingingItem2()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);

            var bll = new DangerBLL();
            var data = bll.UpdateTraingingItem2(model.Data, model.UserId);

            return new { code = result, info = message, data };
        }

        /// <summary>
        /// 负责人提交训练项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SubmitTraingingItem2()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);

            var bll = new DangerBLL();
            var data = bll.SubmitTraingingItem2(model.Data, model.UserId);

            return new { code = result, info = message, data };
        }

        /// <summary>
        /// 作业人训练时保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> DoTraining()
        {
            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Training";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);

                var bll = new DangerBLL();
                bll.DoTraining(model.Data, model.UserId);

                var userbll = new UserBLL();
                var user = userbll.GetEntity(model.UserId);

                var files = new List<FileInfoEntity>();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { RecId = model.Data.Id, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);

                    switch (file.Headers.ContentType.MediaType)
                    {
                        case "image/jpeg":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mpeg":
                            fileentity.Description = "音频";
                            break;
                        case "video/mp4":
                            fileentity.Description = "视频";
                            break;
                        case "image/jpg":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mp3":
                            fileentity.Description = "音频";
                            break;
                        default:
                            break;
                    }
                    files.Add(fileentity);

                }
                var filebll = new FileInfoBLL();
                filebll.SaveFiles(files);

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        /// 作业人训练时提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> SubmitDoTraining()
        {
            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Training";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);

                var bll = new DangerBLL();
                bll.SubmitDoTraining(model.Data, model.UserId);

                var userbll = new UserBLL();
                var user = userbll.GetEntity(model.UserId);

                var files = new List<FileInfoEntity>();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { RecId = model.Data.Id, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);

                    switch (file.Headers.ContentType.MediaType)
                    {
                        case "image/jpeg":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mpeg":
                            fileentity.Description = "音频";
                            break;
                        case "video/mp4":
                            fileentity.Description = "视频";
                            break;
                        case "image/jpg":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mp3":
                            fileentity.Description = "音频";
                            break;
                        default:
                            break;
                    }
                    files.Add(fileentity);

                }
                var filebll = new FileInfoBLL();
                filebll.SaveFiles(files);

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        /// 负责人训练时保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> DoTraining2()
        {
            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Training";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);

                var bll = new DangerBLL();
                bll.DoTraining2(model.Data, model.UserId);

                var userbll = new UserBLL();
                var user = userbll.GetEntity(model.UserId);

                var files = new List<FileInfoEntity>();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { RecId = model.Data.Id, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);

                    switch (file.Headers.ContentType.MediaType)
                    {
                        case "image/jpeg":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mpeg":
                            fileentity.Description = "音频";
                            break;
                        case "video/mp4":
                            fileentity.Description = "视频";
                            break;
                        case "image/jpg":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mp3":
                            fileentity.Description = "音频";
                            break;
                        default:
                            break;
                    }
                    files.Add(fileentity);

                }
                var filebll = new FileInfoBLL();
                filebll.SaveFiles(files);

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        /// 负责人训练时提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> SubmitDoTraining2()
        {
            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Training";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DangerEntity>>(json);

                var bll = new DangerBLL();
                bll.SubmitDoTraining2(model.Data, model.UserId);

                var userbll = new UserBLL();
                var user = userbll.GetEntity(model.UserId);

                var files = new List<FileInfoEntity>();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { RecId = model.Data.Id, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);

                    switch (file.Headers.ContentType.MediaType)
                    {
                        case "image/jpeg":
                            fileentity.Description = "照片";
                            break;
                        case "image/png":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mpeg":
                            fileentity.Description = "音频";
                            break;
                        case "image/jpg":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mp3":
                            fileentity.Description = "音频";
                            break;
                        case "video/mp4":
                            fileentity.Description = "视频";
                            break;
                        default:
                            break;
                    }
                    files.Add(fileentity);

                }
                var filebll = new FileInfoBLL();
                filebll.SaveFiles(files);

                var messagebll = new MessageBLL();
                messagebll.FinishTodo(Config.GetValue("CustomerModel"), model.Data.Id);

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        /// 危险预知训练统计图
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetGroupTrainings(TrainingModel model)
        {


            try
            {
                // dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                var bll = new DangerBLL();
                int year = model.data.year;
                int month = model.data.month;
                UserEntity user = new UserBLL().GetEntity(model.userId);
                var list = bll.GetGroupTraining(year, month, user.DepartmentId);
                var nlist = list.Select(x => new { Name = x.Name, Count = x.Scores });
                return new { code = 0, info = "成功", data = nlist };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }
        /// <summary>
        /// 首页获取宣传信息
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetMainInfo(TrainingModel model)
        {
            try
            {
                var AndroidPhoto = Config.GetValue("AndroidPhoto");
                var QRPhoto = Config.GetValue("QRPhoto");
                var RegistCode = Config.GetValue("RegistCode");
                return new { code = 0, info = "成功", data = new { p1 = AndroidPhoto, p2 = QRPhoto, encode = RegistCode } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }

        /// <summary>
        /// 主页应到人数
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetMeetingInfo(TrainingModel model)
        {
            try
            {
                WorkmeetingBLL meetBll = new WorkmeetingBLL();
                UserEntity user = new UserBLL().GetEntity(model.userId);
                var num = new PeopleBLL().GetListByDept(user.DepartmentId).Count();
                var meet = meetBll.GetAllList().Where(x => x.GroupId == user.DepartmentId && x.MeetingStartTime >= DateTime.Now.Date && x.IsOver).OrderByDescending(x => x.MeetingStartTime).FirstOrDefault();
                var should = 0;
                if (meet != null) should = meet.ActuallyJoin;
                return new { code = 0, info = "成功", data = new { All = num, Should = should } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }

        /// <summary>
        /// 主页指标
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetMainNumbers(TrainingModel model)
        {
            try
            {
                var year = DateTime.Now.Year;
                var curyear = new DateTime(year, 1, 1);
                WorkmeetingBLL meetBll = new WorkmeetingBLL();
                EducationBLL edubll = new EducationBLL();
                ActivityBLL act = new ActivityBLL();
                DangerBLL bll = new DangerBLL();
                EmergencyBLL ebll = new EmergencyBLL();
                UserEntity user = new UserBLL().GetEntity(model.userId);
                int total = 0;
                var educationlist = edubll.GetAllList().Where(x => x.BZId == user.DepartmentId && x.ActivityDate >= curyear);
                decimal educationnumber = 0;
                foreach (var a in educationlist)
                {
                    educationnumber += a.AttendNumber * a.LearnTime;
                }
                var activitynumber = act.GetList(user.DepartmentId, curyear, null, "", 1, 10000, "", out total).Where(x => x.ActivityType == "安全日活动").Count();
                var dangernumber = bll.GetList("").Where(x => x.JobTime >= curyear && x.GroupId == user.DepartmentId && x.Status == 2).Count();

                //var dict = new
                //{
                //    business = "",
                //    data = user.DepartmentId,
                //    userid = user.UserId,
                //    tokenid = ""
                //};
                //string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Home", "getFiveTypeInfo"), "json=" + JsonConvert.SerializeObject(dict));
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                //var emergencynumber = dy.data.drillplannum;
                //var checknumber = dy.data.checknum;
                //var lllegalnumber = dy.data.lllegalnum;
                return new { code = 0, info = "成功", data = new { Education = educationnumber, Activity = activitynumber, Danger = dangernumber } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }

        /// <summary>
        /// 危险预知训练历史记录
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<DangerEntity> GetHistory(ParamBucket<DateLimit> arg)
        {
            var user = new UserBLL().GetEntity(arg.UserId);
            var listdept = new List<string>();
            if (string.IsNullOrEmpty(arg.Data.DeptId))
            {
                var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DepartmentId);
                var depts = new DepartmentBLL().GetSubDepartments(dept.DepartmentId, "班组");
                listdept.AddRange(depts.Select(x => x.DepartmentId));
            }
            else listdept.Add(arg.Data.DeptId);

            var bll = new DangerBLL();
            var total = 0;
            var data = bll.GetHistory(listdept.ToArray(), arg.Data.Key, arg.Data.From, arg.Data.To, arg.Data.IsEvaluate, arg.UserId, arg.PageSize, arg.PageIndex, out total);
            return new ListBucket<DangerEntity>() { Data = data, Total = total };
        }

        /// <summary>
        /// 删除危险预知训练
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket Delete(ParamBucket<string> arg)
        {
            var success = true;
            var message = string.Empty;

            var bll = new DangerBLL();
            var messagebll = new MessageBLL();
            try
            {
                bll.Delete(arg.Data);
                messagebll.FinishTodo(Config.GetValue("CustomerModel"), arg.Data);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }
            return new ResultBucket() { Success = success, Message = message };
        }
    }
}
