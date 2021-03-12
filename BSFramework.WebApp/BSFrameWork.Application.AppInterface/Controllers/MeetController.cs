using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using BSFramework.Application.Busines.SystemManage;
using System.Configuration;
using System.IO;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Util.WebControl;
using BSFramework.Application.Entity.PeopleManage;
using BSFrameWork.Application.AppInterface.Models;
using BSFramework.Application.Code;
using Bst.Fx.Uploading;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.Busines.SetManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Entity.WorkMeeting;
using System.Data;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.Busines.EducationManage;
using BSFrameWork.Application.AppInterface.Models.QueryModels;
using BSFramework.Application.Entity.CarcOrCardManage;
using BSFramework.Application.Busines.CarcOrCardManage;
using BSFramework.Application.Busines.DeviceInspection;
using BSFramework.Application.Entity.DeviceInspection;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class MeetController : BaseApiController
    {
        /// <summary>
        /// 班会记录 早安中铝
        /// </summary>
        private readonly MeetingRecordBLL _meetingReocrdbll;
        /// <summary>
        /// 班前一题/一课
        /// </summary>
        private readonly MeetingQuestionBLL _questionbll;
        public MeetController()
        {
            _meetingReocrdbll = new MeetingRecordBLL();
            _questionbll = new MeetingQuestionBLL();
        }
        /// <summary>
        /// 5.获取班前班后会列表
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object GetList([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                long status = dy.data.status;  //查询状态
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                string title = ""; //活动主题
                string startTime = dy.data.startTime;
                string endTime = dy.data.endTime;
                WorkmeetingBLL meetBll = new WorkmeetingBLL();
                int total = 0;
                IList list = meetBll.GetList(int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total, int.Parse(status.ToString()), startTime, endTime, user.DepartmentId);
                return new { code = 0, info = "获取数据成功", count = total, data = new { activies = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 5.获取班前班后会列表
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object GetListTotal([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                long status = dy.data.status;  //查询状态
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                string title = ""; //活动主题
                string startTime = dy.data.startTime;
                string endTime = dy.data.endTime;
                WorkmeetingBLL meetBll = new WorkmeetingBLL();
                int total = 0;
                IList list = meetBll.GetList(int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total, int.Parse(status.ToString()), startTime, endTime, user.DepartmentId);
                return new { code = 0, info = "获取数据成功", count = total, data = new { activies = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 3.获取班前班后会详情
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object GetInfo([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string id = dy.data.MeetId;//记录Id
                WorkmeetingBLL actBll = new WorkmeetingBLL();
                WorkmeetingEntity meet = actBll.GetDetail(id);//获取活动详情
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                IList list = fileBll.GetFilesByRecId(id, Config.GetValue("AppUrl"));//获取相关附件集合
                StringBuilder sb = new StringBuilder();
                StringBuilder sb1 = new StringBuilder();
                foreach (MeetingSigninEntity sign in meet.Signins)
                {
                    if (sign.IsSigned)
                    {
                        sb.AppendFormat("{0},", sign.PersonName);
                    }
                    else
                    {
                        sb1.AppendFormat("{0},", sign.PersonName);
                    }
                }
                return new { code = 0, info = "获取数据成功", data = new { MeetId = meet.MeetingId, PlanTime = meet.MeetingStartTime.ToString("yyyy.MM.dd HH:mm") + "-" + meet.MeetingEndTime.ToString("HH:mm"), Compere = meet.MeetingPerson, JoinUsers = sb.Length == 0 ? "无" : sb.ToString().TrimEnd(','), VacancyUser = sb1.Length == 0 ? "无" : sb1.ToString().TrimEnd(','), IsFinished = meet.IsOver ? "已结束" : "未结束", MeetType = meet.MeetingType, Attachments = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 4.班前班后会上传音频或图片
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object Upload()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data.MeetId;//业务记录Id
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                IList DelKeys = dy.data.DelKeys;
                string keys = string.Empty;
                for (int i = 0; i < DelKeys.Count; i++)
                {
                    if (i == DelKeys.Count - 1)
                    {
                        keys += DelKeys[i].ToString();
                    }
                    else
                        keys += DelKeys[i].ToString() + "','";
                }
                List<FileInfoEntity> fileList = fileBll.RemoveForm(keys, id, user);
                for (int i = 0; i < fileList.Count; i++)
                {
                    string path = fileBll.Delete(fileList[i].FileId);
                    string url = Config.GetValue("FilePath") + path.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
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
                        FilePath = "~/Resource/AppFile/Meets/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0
                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Meets"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Meets");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Meets\\" + fileId + ext);
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

        [HttpPost]
        public object GetJobDetail()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var job = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<MeetingAndJobEntity>>(json);
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            var trainingtype = Config.GetValue("TrainingType");

            WorkmeetingBLL meetBll = new WorkmeetingBLL();
            MeetingJobEntity data = meetBll.GetJobDetail(job.Data.JobId, job.Data.MeetingJobId, trainingtype);
            data.FileList1 = data.Files.Where(x => x.Description == "音频" && x.CreateUserId == job.UserId).Select(x => new FileInfoEntity() { FileId = x.FileId, FileName = x.FileName, FilePath = url + x.FilePath.Replace("~/", string.Empty) }).ToList();
            data.FileList2 = data.Files.Where(x => x.Description == "照片" && x.CreateUserId == job.UserId).Select(x => new FileInfoEntity() { FileId = x.FileId, FileName = x.FileName, FilePath = url + x.FilePath.Replace("~/", string.Empty) }).ToList();

            return new { info = string.Empty, code = 0, count = 0, data = data };
        }

        [HttpPost]
        public object ViewJobDetail()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var job = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<MeetingAndJobEntity>>(json);
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            var trainingtype = Config.GetValue("TrainingType");
            WorkmeetingBLL meetBll = new WorkmeetingBLL();
            MeetingJobEntity data = meetBll.GetJobDetail(job.Data.JobId, job.Data.MeetingJobId, trainingtype);
            data.FileList1 = data.Files.Where(x => x.Description == "音频").Select(x => new FileInfoEntity() { FileId = x.FileId, FileName = x.FileName, FilePath = url + x.FilePath.Replace("~/", string.Empty) }).ToList();
            data.FileList2 = data.Files.Where(x => x.Description == "照片").Select(x => new FileInfoEntity() { FileId = x.FileId, FileName = x.FileName, FilePath = url + x.FilePath.Replace("~/", string.Empty) }).ToList();
            var bll2 = new HumanDangerTrainingBLL();
            if (trainingtype == "人身风险预控")
            {
                //var trainings = bll2.GetListByUserIdJobId(data.Relation.MeetingJobId, job.UserId);
                //data.NeedTrain = trainings.Count > 0 ? true : false;
                data.TrainingDone = data.HumanDangerTraining == null ? false : data.HumanDangerTraining.TrainingUsers.All(x => x.IsDone && x.IsMarked);
            }
            else
            {
                data.TrainingDone = data.Training != null && data.Training.Status == 2 ? true : false;
            }
            switch (data.IsFinished)
            {
                case "undo":
                    data.Status = "进行中";
                    break;
                case "finish":
                    data.Status = "已完成";
                    break;
                case "cancel":
                    data.Status = "已取消";
                    break;
                default:
                    break;
            }


            return new { info = string.Empty, code = 0, count = 0, data = data };
        }

        [HttpPost]
        public object PostJob([FromBody] JObject json)
        {
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            UserEntity user = new UserBLL().GetEntity(userId);

            var jobject = JObject.Parse(res);
            var datajson = jobject["data"].ToJson();

            var model = JsonConvert.DeserializeObject<MeetingJobEntity>(datajson);
            model.JobId = Guid.NewGuid().ToString();
            model.CreateDate = DateTime.Now;
            model.CreateUserId = userId;
            if (string.IsNullOrEmpty(model.JobType)) model.JobType = "app";
            model.GroupId = user.DepartmentId;
            model.Relation.MeetingJobId = Guid.NewGuid().ToString();
            model.Relation.JobId = model.JobId;
            model.IsFinished = model.Relation.IsFinished = "undo";
            model.Relation.JobUserId = string.Join(",", model.Relation.JobUsers.Select(x => x.UserId));
            model.Relation.JobUser = string.Join(",", model.Relation.JobUsers.Select(x => x.UserName));
            foreach (var item in model.Relation.JobUsers)
            {
                item.JobUserId = Guid.NewGuid().ToString();
                item.CreateDate = DateTime.Now;
                item.MeetingJobId = model.Relation.MeetingJobId;
            }
            if (model.DangerousList == null) model.DangerousList = new List<JobDangerousEntity>();
            foreach (var item in model.DangerousList)
            {
                item.JobDangerousId = Guid.NewGuid().ToString();
                item.CreateTime = DateTime.Now;
                item.JobId = model.JobId;
                foreach (var item1 in item.MeasureList)
                {
                    item1.JobMeasureId = Guid.NewGuid().ToString();
                    item1.CreateTime = DateTime.Now;
                    item1.JobDangerousId = item.JobDangerousId;
                }
            }
            model.Dangerous = string.Join("。", model.DangerousList.Select(x => x.Content));
            model.Measure = string.Join("。", model.DangerousList.SelectMany(x => x.MeasureList).Select(x => x.Content));

            WorkmeetingBLL meetBll = new WorkmeetingBLL();
            var success = true;
            var message = string.Empty;

            try
            {


                var job = meetBll.PostJob(model);
                #region 设备循环检查
                //班前会开始前新增无效 
                if (!string.IsNullOrEmpty(job.Relation.StartMeetingId) || !string.IsNullOrEmpty(job.Relation.EndMeetingId))
                {

                    var MeetId = job.Relation.StartMeetingId;

                    var jobId = model.JobId;
                    if (!string.IsNullOrEmpty(job.Relation.EndMeetingId))
                    {
                        MeetId = job.Relation.EndMeetingId;
                    }

                    //设备巡回检查表
                    DeviceInspectionBLL _inspectionbll = new DeviceInspectionBLL();
                    //设备巡回检查副本表
                    DeviceInspectionJobBLL _inspectionJobbll = new DeviceInspectionJobBLL();
                    //对任务库模板数据进行业务处理


                    //手机app
                    if (!string.IsNullOrEmpty(model.deviceId))
                    {
                        //获取实体
                        var inspectionEntity = _inspectionbll.GetEntity(model.deviceId);
                        if (inspectionEntity != null)
                        {
                            var InspectionItems = _inspectionbll.GetDeviceInspectionItems(model.deviceId);
                            //转json
                            var inspectionEntityStr = JsonConvert.SerializeObject(inspectionEntity);
                            var InspectionItemsStr = JsonConvert.SerializeObject(InspectionItems);
                            //转化实体
                            var inspectionJobEntity = JsonConvert.DeserializeObject<DeviceInspectionJobEntity>(inspectionEntityStr);
                            var InspectionJobItems = JsonConvert.DeserializeObject<List<DeviceInspectionItemJobEntity>>(InspectionItemsStr);
                            var keyvalue = Guid.NewGuid().ToString();
                            inspectionJobEntity.Id = keyvalue;
                            inspectionJobEntity.MeetId = MeetId;
                            inspectionJobEntity.JobId = jobId;
                            inspectionJobEntity.Workuser = string.Join(",", job.Relation.JobUsers.Select(x => x.UserName));
                            inspectionJobEntity.WorkuserId = string.Join(",", job.Relation.JobUsers.Select(x => x.UserId));
                            _inspectionJobbll.SaveForm("", inspectionJobEntity, InspectionJobItems);
                        }
                    }
                    else
                    {
                        //安卓终端
                        if (!string.IsNullOrEmpty(model.TemplateId))
                        {


                            //获取数据模板
                            var entity = meetBll.GetJobTemplate(model.TemplateId);
                            if (entity != null)
                            {

                                #region 设备巡回检查需要进行处理

                                if (!string.IsNullOrEmpty(entity.RecId))
                                {
                                    //获取实体
                                    var inspectionEntity = _inspectionbll.GetEntity(entity.RecId);
                                    if (inspectionEntity != null)
                                    {
                                        var InspectionItems = _inspectionbll.GetDeviceInspectionItems(entity.RecId);
                                        //转json
                                        var inspectionEntityStr = JsonConvert.SerializeObject(inspectionEntity);
                                        var InspectionItemsStr = JsonConvert.SerializeObject(InspectionItems);
                                        //转化实体
                                        var inspectionJobEntity = JsonConvert.DeserializeObject<DeviceInspectionJobEntity>(inspectionEntityStr);
                                        var InspectionJobItems = JsonConvert.DeserializeObject<List<DeviceInspectionItemJobEntity>>(InspectionItemsStr);
                                        var keyvalue = Guid.NewGuid().ToString();
                                        inspectionJobEntity.Id = keyvalue;
                                        inspectionJobEntity.MeetId = MeetId;
                                        inspectionJobEntity.JobId = jobId;
                                        inspectionJobEntity.Workuser = string.Join(",", job.Relation.JobUsers.Select(x => x.UserName));
                                        inspectionJobEntity.WorkuserId = string.Join(",", job.Relation.JobUsers.Select(x => x.UserId));
                                        _inspectionJobbll.SaveForm("", inspectionJobEntity, InspectionJobItems);
                                    }


                                }
                                #endregion
                            }

                        }


                    }
                }
                #endregion
                if (string.IsNullOrEmpty(job.Relation.StartMeetingId))
                {
                    message = "任务已分配至班前会，等待下发。";
                }
                else
                {
                    var meeting = meetBll.GetEntity(job.Relation.StartMeetingId);
                    if (meeting.IsOver)
                        message = "任务已下发。";
                    else
                        message = "任务已分配至班前会，等待下发。";
                }

                if (!string.IsNullOrEmpty(model.Relation.StartMeetingId))
                {
                    var meeting = meetBll.GetDetail(model.Relation.StartMeetingId);
                    if (meeting != null && meeting.IsOver)
                    {
                        var trainingtype = Config.GetValue("TrainingType");
                        if (trainingtype == "人身风险预控")
                        {
                            if (model.NeedTrain)
                            {
                                var training = new HumanDangerTrainingEntity() { TrainingId = Guid.NewGuid().ToString(), TrainingTask = model.Job, CreateTime = DateTime.Now, CreateUserId = userId, MeetingJobId = model.Relation.MeetingJobId, DeptId = model.GroupId, TrainingPlace = model.JobAddr, No = model.TicketCode };
                                if (!string.IsNullOrEmpty(model.TemplateId)) training.HumanDangerId = model.TemplateId;
                                training.TrainingUsers = model.Relation.JobUsers.Select(x => new TrainingUserEntity() { TrainingUserId = Guid.NewGuid().ToString(), UserId = x.UserId, UserName = x.UserName, TrainingRole = x.JobType == "ischecker" ? 1 : 0 }).ToList();
                                new HumanDangerTrainingBLL().Add(training);
                            }
                        }
                        else if (trainingtype == "CARC")
                        {
                            var card = new CarcEntity
                            {
                                Id = Guid.NewGuid().ToString(),
                                WorkName = model.Job,
                                WorkArea = model.JobAddr,
                                DataType = model.NeedTrain ? "carc" : "card",
                                StartTime = model.StartTime,
                                EndTime = model.EndTime,
                                MeetId = model.Relation.StartMeetingId,
                                TutelagePersonId = model.Relation.JobUsers.Find(x => x.JobType == "ischecker").UserId,
                                TutelagePerson = model.Relation.JobUsers.Find(x => x.JobType == "ischecker").UserName,
                                OperationPersonId = string.Join(",", model.Relation.JobUsers.Where(x => x.JobType == "isdoperson").Select(x => x.UserId)),
                                OperationPerson = string.Join(",", model.Relation.JobUsers.Where(x => x.JobType == "isdoperson").Select(x => x.UserName)),
                            };
                            new CarcOrCardBLL().SaveForm(new List<CarcEntity>() { card }, user.UserId);
                        }
                        else
                        {
                            if (model.NeedTrain)
                            {
                                var dangerbll = new DangerBLL();
                                var messagebll = new MessageBLL();

                                var danger = dangerbll.Save(model);
                                if (danger != null)
                                    messagebll.SendMessage(Config.GetValue("CustomerModel"), danger.Id);
                            }
                        }
                    }
                }

                var msgbll = new MessageBLL();
                msgbll.SendMessage("工作提示", model.Relation.MeetingJobId);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return new { info = message, code = success ? 0 : 1, count = 0 };
        }

        [HttpPost]
        public object GetJobUser([FromBody] JObject json)
        {
            var res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            UserEntity user = new UserBLL().GetEntity(userId);

            WorkmeetingBLL meetBll = new WorkmeetingBLL();
            var success = true;
            var message = string.Empty;
            var data = default(List<MeetingSigninEntity>);

            try
            {
                data = meetBll.GetJobUser(user.DepartmentId);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return new { info = message, code = success ? 0 : 1, count = 0, data = data };
        }

        /// <summary>
        /// 手机上传任务照片和音频，支持删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UpdateJob()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");
            var jobject = JObject.Parse(json);
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
            string userId = dy.userId;
            var jobjson = jobject.Value<JToken>("data").ToJson();
            var postjob = JsonConvert.DeserializeObject<MeetingJobEntity>(jobjson.ToString());
            var meetingjobid = jobject.SelectToken("data.MeetingJobId").Value<string>();
            postjob.Relation = new MeetingAndJobEntity() { MeetingJobId = meetingjobid };
            try
            {
                if (postjob.Files == null) postjob.Files = new List<FileInfoEntity>();
                List<FileInfoEntity> undeletefiles = postjob.Files.ToList();
                foreach (var item in undeletefiles)
                {
                    item.CreateUserId = userId;
                }
                WorkmeetingBLL meetBll = new WorkmeetingBLL();
                MeetingJobEntity job = meetBll.GetJobDetail(postjob.JobId, null, null);
                job.CreateUserId = userId; //重要
                job.TicketId = jobject.SelectToken("data.TicketId").Value<string>();
                job.TicketCode = jobject.SelectToken("data.TicketCode").Value<string>();
                job.NeedTrain = jobject.SelectToken("data.NeedTrain").Value<bool>();
                job.Relation = new MeetingAndJobEntity() { MeetingJobId = meetingjobid };
                var deletefiles = job.Files.Except(undeletefiles, new FileCompare());
                job.Files = undeletefiles;
                UserEntity user = new UserBLL().GetEntity(userId);
                var description = postjob.Description;


                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[i];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    var fileentity = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = postjob.JobId,
                        RecId = postjob.Relation.MeetingJobId,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/Meets/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName,
                        ModifyDate = DateTime.Now,
                        ModifyUserId = user.UserId,
                        ModifyUserName = user.RealName
                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Meets"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Meets");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Meets\\" + fileId + ext);
                    //保存附件信息
                    job.Files.Add(fileentity);
                }

                job.Description = description;

                meetBll.UpdateJob(job);
                foreach (var item in deletefiles)
                {

                }
            }
            catch (Exception e)
            {
                result = 1;
                message = e.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object GetDatabaseJobs([FromBody] JObject json)
        {
            var res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            string query = dy.data;
            UserEntity user = new UserBLL().GetEntity(userId);
            var data = new List<JobTemplateEntity>();
            var trainingtype = Config.GetValue("TrainingType");
            var total = 0;
            if (trainingtype == "人身风险预控")
            {
                HumanDangerBLL bll1 = new HumanDangerBLL();
                var data1 = bll1.Find(query, user.DepartmentId, 10, 1, out total);
                data = data1.Select(x => new JobTemplateEntity() { JobId = x.HumanDangerId.ToString(), JobContent = x.Task, WorkArea = x.TaskArea }).ToList();
            }
            else
            {
                WorkmeetingBLL meetBll = new WorkmeetingBLL();
                var data2 = meetBll.Find(query, user.DepartmentId, 10);
                data.AddRange(data2);
            }

            return new { info = string.Empty, code = 0, count = data.Count, data = data };
        }

        [HttpPost]
        public object GetDatabaseDangrous([FromBody] JObject json)
        {
            var res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            UserEntity user = new UserBLL().GetEntity(userId);
            WorkmeetingBLL meetBll = new WorkmeetingBLL();
            var dangerous = meetBll.GetDangerous(user.DepartmentId);
            return new { info = string.Empty, code = 0, count = dangerous.Count, data = dangerous.Select(x => new { x.JobId, ITEMNAME = x.Dangerous }) };
        }

        [HttpPost]
        public object GetDatabaseMeasure([FromBody] JObject json)
        {
            var res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            UserEntity user = new UserBLL().GetEntity(userId);
            WorkmeetingBLL meetBll = new WorkmeetingBLL();
            var measures = meetBll.GetMeasures(user.DepartmentId);
            return new { info = string.Empty, code = 0, count = measures.Count, data = measures };
        }

        [HttpPost]
        public object GetDeptJobs([FromBody] JObject json)
        {
            var res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            int pagesize = (int)dy.pageSize;
            int pageindex = (int)dy.pageIndex;
            var total = 0;
            UserEntity user = new UserBLL().GetEntity(userId);
            WorkmeetingBLL meetBll = new WorkmeetingBLL();
            FileInfoBLL bll = new FileInfoBLL();
            var data = meetBll.GetDeptJobs(user.DepartmentId, pagesize, pageindex, out total);
            if (data == null) data = new List<MeetingJobEntity>();
            foreach (var item in data)
            {
                var file = bll.GetFilesByRecIdNew(item.Relation.MeetingJobId);
                item.Files = file.Where(x => x.Description == "照片").ToList();
                item.FileList1 = file.Where(x => x.Description == "音频").ToList();
            }

            var data1 = data.Select(x => new
            {
                JobId = x.JobId,
                Job = x.Job,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                IsFinished = x.Relation.IsFinished,
                JobUsers = x.Relation.JobUser,
                piccount = x.Files.Count(),
                audiocount = x.FileList1.Count(),
                Relation = x.Relation,
                x.NeedTrain
            });
            return new { info = string.Empty, code = 0, count = total, data = data1 };
        }

        [HttpPost]
        public object GetMeetingType()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var userid = jobject.Value<string>("userId");
            var data = string.Empty;

            UserEntity user = new UserBLL().GetEntity(userid);

            var bll = new WorkmeetingBLL();
            var meeting = bll.GetWorkMeeting(user.DepartmentId, DateTime.Now);

            if (meeting == null) meeting = new WorkmeetingEntity()
            {
                MeetingType = "班前会"
            };

            //if (meeting == null) return "班前会";
            //else
            //{
            //    data = meeting.MeetingType == "班前会" ? "班后会" : "班前会";
            //}

            return new { code = result, info = message, data = meeting, count = 1 };
        }

        [HttpPost]
        public object GetCurrentMeeting([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;

            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            var trainingtype = Config.GetValue("TrainingType");
            string res = jobject.Value<string>("json");
            var json = JObject.Parse(res.ToString());
            string userId = json.Value<string>("userId");
            UserEntity user = new UserBLL().GetEntity(userId);
            var meeting = default(WorkmeetingEntity);

            var bll = new WorkmeetingBLL();
            var meetingset = new WorkOrderBLL().GetWorkOrderList(DateTime.Now, user.DepartmentId);
            var hasSet = false;
            DateTime? start = null;
            DateTime? over = null;
            string code = null;
            if (meetingset[0] != "无")
            {
                hasSet = true;
                var part = meetingset[1].Split('-');
                start = DateTime.Parse(part[0] + ":00");
                over = DateTime.Parse(part[1] + ":00");
            }
            var begin = start;
            var end = over;
            if (!hasSet)
            {
                begin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                end = begin.Value.AddDays(1).AddSeconds(-1);
            }

            var lastmeeting = bll.GetLastMeeting(user.DepartmentId);
            if (lastmeeting == null)
            {
                //没有班会数据
                var jobs = bll.FindMeetingJobs(user.DepartmentId, DateTime.Today);
                meeting = new WorkmeetingEntity() { Jobs = jobs, ShouldStartTime = start, ShouldEndTime = over, MeetingCode = code };
            }
            else
            {
                if (lastmeeting.MeetingType == "班前会")
                {
                    //班前会
                    if (lastmeeting.IsOver)
                    {
                        var jobs = bll.GetMeetingJobs(lastmeeting.MeetingId);
                        meeting = new WorkmeetingEntity() { Jobs = jobs, OtherMeetingId = lastmeeting.MeetingId, OtherMeetingStartTime = lastmeeting.MeetingStartTime, ShouldStartTime = lastmeeting.ShouldStartTime, ShouldEndTime = lastmeeting.ShouldEndTime };
                        //结束
                    }
                    else
                    {
                        //未结束
                        meeting = bll.GetDetail(lastmeeting.MeetingId);
                    }
                }
                else
                {
                    //班后会
                    if (lastmeeting.IsOver)
                    {
                        List<MeetingJobEntity> jobs = null;
                        if (hasSet)
                        {
                            //有排班
                            if (lastmeeting.ShouldStartTime == start && lastmeeting.ShouldEndTime == over)
                            {
                                //已开班会
                                var jobs2 = bll.FindMeetingJobs2(user.DepartmentId, DateTime.Now);
                                jobs = jobs2;
                                var longjobs = bll.FindLongJobs(user.DepartmentId, DateTime.Now);
                                longjobs.RemoveAll(x => jobs2.Any(y => y.JobId == x.JobId));
                                if (longjobs.Count > 0) bll.AddLongJobs(longjobs);
                                jobs.AddRange(longjobs);
                            }
                            else
                            {
                                //未开班会
                                jobs = bll.FindMeetingJobs(user.DepartmentId, DateTime.Now);
                                var longjobs = bll.FindLongJobs(user.DepartmentId, DateTime.Now);
                                longjobs.RemoveAll(x => jobs.Any(y => y.JobId == x.JobId));
                                if (longjobs.Count > 0) bll.AddLongJobs(longjobs);
                                jobs.AddRange(longjobs);
                            }
                        }
                        else
                        {
                            //无排班
                            if (lastmeeting.MeetingStartTime.Date == DateTime.Today)
                            {
                                //已开班会
                                var jobs2 = bll.FindMeetingJobs2(user.DepartmentId, DateTime.Now);
                                jobs = jobs2;
                                var longjobs = bll.FindLongJobs(user.DepartmentId, DateTime.Now);
                                longjobs.RemoveAll(x => jobs2.Any(y => y.JobId == x.JobId));
                                if (longjobs.Count > 0) bll.AddLongJobs(longjobs);
                                jobs.AddRange(longjobs);
                            }
                            else
                            {
                                //未开班会
                                jobs = bll.FindMeetingJobs(user.DepartmentId, DateTime.Now);
                                var longjobs = bll.FindLongJobs(user.DepartmentId, DateTime.Now);
                                longjobs.RemoveAll(x => jobs.Any(y => y.JobId == x.JobId));
                                if (longjobs.Count > 0) bll.AddLongJobs(longjobs);
                                jobs.AddRange(longjobs);
                            }
                        }
                        meeting = new WorkmeetingEntity() { Jobs = jobs, ShouldStartTime = start, ShouldEndTime = over, MeetingCode = code };
                    }
                    else
                    {
                        //未结束
                        meeting = bll.GetDetail(lastmeeting.MeetingId);
                        meeting.OtherMeetingStartTime = lastmeeting.MeetingStartTime;
                    }
                }
            }


            var path = Config.GetValue("FilePath");
            if (meeting.Jobs != null)
            {
                meeting.Jobs = meeting.Jobs.OrderBy(x => x.StartTime).ThenBy(x => x.Job).ToList();
                foreach (var item in meeting.Jobs)
                {
                    if (item.HumanDangerTraining != null && item.HumanDangerTraining.TrainingUsers != null)
                    {
                        item.HumanDangerTraining.TrainingUsers.ForEach(x => x.Training = null);
                    }
                    item.Status = item.IsFinished == "finish" ? "完成" : item.IsFinished == "undo" ? "进行中" : "已取消";
                    item.piccount = 0;
                    item.audiocount = 0;

                    if (item.Files != null)
                    {
                        item.piccount = item.Files.Count(x => x.Description == "照片");
                        item.audiocount = item.Files.Count(x => x.Description == "音频");
                        foreach (var item1 in item.Files)
                        {
                            if (item1.Description == "音频")
                            {
                                var Pathurl = path + item1.FilePath.Replace("~/Resource", string.Empty).Replace("/", "\\");
                                item1.duration = BSFramework.Util.WMP.GetDurationByWMPLib(Pathurl);
                            }
                            item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                        }
                    }

                    if (item.Relation != null)
                    {
                        foreach (var item1 in item.Relation.JobUsers)
                        {
                            var people = new PeopleBLL().GetEntity(item1.UserId);
                            if (people != null) item1.ImageUrl = people.Photo;
                        }
                    }
                }
            }   //循环测试获取时间
            if (meeting.Files != null)
            {
                foreach (var item in meeting.Files)
                {
                    if (item.Description == "音频")
                    {
                        var Pathurl = path + item.FilePath.Replace("~/Resource", string.Empty).Replace("/", "\\");
                        item.duration = BSFramework.Util.WMP.GetDurationByWMPLib(Pathurl);
                    }
                }
            }

            return new { code = result, info = message, data = meeting, count = 1 };
        }
        //public object GetCurrentMeeting([FromBody]JObject jobject)
        //{
        //    var result = 0;
        //    var message = string.Empty;
        //    var total = 0;
        //    var data = default(WorkmeetingEntity);

        //    var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
        //    var trainingtype = Config.GetValue("TrainingType");

        //    try
        //    {
        //        string res = jobject.Value<string>("json");
        //        var json = JObject.Parse(res.ToString());
        //        string userId = json.Value<string>("userId");
        //        //string meetingtype = json.SelectToken("data.meetingtype").Value<string>();
        //        UserEntity user = new UserBLL().GetEntity(userId);

        //        var bll = new WorkmeetingBLL();
        //        data = bll.GetWorkMeeting(user.DepartmentId, DateTime.Now);

        //        if (data == null)
        //        {
        //            var set = new WorkOrderBLL().GetWorkOrderList(DateTime.Now, user.DepartmentId);
        //            if (set[0] != "无")
        //            {
        //                var part = set[1].Split('-');
        //                var start = DateTime.Parse(part[0] + ":00");
        //                var end = DateTime.Parse(part[1] + ":00");
        //                data = bll.CreateStartMeeting(user.DepartmentId, DateTime.Now, start, end, set[0]);
        //            }
        //            else
        //                data = bll.CreateStartMeeting(user.DepartmentId, DateTime.Now, null, null, null);
        //            //旧版不支持提前准备
        //            //data = bll.BuildWorkMeeting(string.Empty, user.DepartmentId, DateTime.Now);
        //        }
        //        else
        //        {
        //            if (data.MeetingType == "班前会")
        //            {
        //                if (data.IsOver)
        //                {
        //                    data = bll.BuildWorkMeeting(data.MeetingId, user.DepartmentId, DateTime.Now, trainingtype);
        //                }
        //                else
        //                {
        //                    bll.ReBuildMeeting(data.MeetingId);
        //                    data = bll.GetDetail(data.MeetingId);
        //                }
        //            }
        //            else
        //            {
        //                if (data.IsOver)
        //                {

        //                    var set = new WorkOrderBLL().GetWorkOrderList(DateTime.Now, user.DepartmentId);
        //                    if (set[0] != "无")
        //                    {
        //                        var part = set[1].Split('-');
        //                        var start = DateTime.Parse(part[0] + ":00");
        //                        var end = DateTime.Parse(part[1] + ":00");
        //                        data = bll.CreateStartMeeting(user.DepartmentId, DateTime.Now, start, end, set[0]);
        //                    }
        //                    else
        //                        data = bll.CreateStartMeeting(user.DepartmentId, DateTime.Now, null, null, null);
        //                    //旧版不支持提前准备
        //                    //data = bll.BuildWorkMeeting(string.Empty, user.DepartmentId, DateTime.Now);
        //                }
        //                else
        //                    data = bll.GetDetail(data.MeetingId);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = 1;
        //        message = ex.Message;
        //    }

        //    var path = Config.GetValue("FilePath");
        //    if (data.Jobs != null)
        //    {
        //        foreach (var item in data.Jobs)
        //        {
        //            item.Status = item.IsFinished == "finish" ? "完成" : item.IsFinished == "undo" ? "进行中" : "已取消";
        //            item.piccount = 0;
        //            item.audiocount = 0;

        //            if (item.Files != null)
        //            {
        //                item.piccount = item.Files.Count(x => x.Description == "照片");
        //                item.audiocount = item.Files.Count(x => x.Description == "音频");
        //                foreach (var item1 in item.Files)
        //                {
        //                    if (item1.Description == "音频")
        //                    {
        //                        var Pathurl = path + item1.FilePath.Replace("~/Resource", string.Empty).Replace("/", "\\");
        //                        item1.duration = BSFramework.Util.WMP.GetDurationByWMPLib(Pathurl);
        //                    }
        //                    item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
        //                }
        //            }

        //            if (item.Relation != null)
        //            {
        //                foreach (var item1 in item.Relation.JobUsers)
        //                {
        //                    var people = new PeopleBLL().GetEntity(item1.UserId);
        //                    if (people != null) item1.ImageUrl = people.Photo;
        //                }
        //            }
        //        }
        //    }   //循环测试获取时间
        //    if (data.Files != null)
        //    {
        //        foreach (var item in data.Files)
        //        {
        //            if (item.Description == "音频")
        //            {
        //                var Pathurl = path + item.FilePath.Replace("~/Resource", string.Empty).Replace("/", "\\");
        //                item.duration = BSFramework.Util.WMP.GetDurationByWMPLib(Pathurl);
        //            }
        //        }
        //    }

        //    return new { code = result, info = message, data = data, count = 1 };
        //}

        [HttpPost]
        public object StartMeeting()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<WorkmeetingEntity>>(json);

            try
            {
                var now = DateTime.Now;
                var user = new UserBLL().GetEntity(model.UserId);
                var dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                var bll = new WorkmeetingBLL();

                model.Data.MeetingId = Guid.NewGuid().ToString();
                model.Data.MeetingType = "班前会";
                model.Data.MeetingStartTime = DateTime.Now;
                model.Data.GroupId = user.DepartmentId;
                model.Data.GroupName = dept.FullName;
                model.Data.IsStarted = true;
                if (model.Data.Jobs != null)
                {
                    //针对新增的班会任务
                    var job = model.Data.Jobs.Where(x => !string.IsNullOrEmpty(x.TemplateId)).ToList();
                    if (job.Count > 0)
                    {
                        //设备巡回检查表
                        DeviceInspectionBLL _inspectionbll = new DeviceInspectionBLL();
                        //设备巡回检查副本表
                        DeviceInspectionJobBLL _inspectionJobbll = new DeviceInspectionJobBLL();
                        //对任务库模板数据进行业务处理  
                        foreach (var item in job)
                        {
                            //获取数据模板
                            var entity = bll.GetJobTemplate(item.TemplateId);

                            #region 设备巡回检查需要进行处理
                            if(entity != null)
                            {
                                if (!string.IsNullOrEmpty(entity.RecId))
                                {
                                    //获取实体
                                    var inspectionEntity = _inspectionbll.GetEntity(entity.RecId);
                                    if (inspectionEntity == null)
                                    {
                                        continue;
                                    }
                                    var InspectionItems = _inspectionbll.GetDeviceInspectionItems(entity.RecId);
                                    //转json
                                    var inspectionEntityStr = JsonConvert.SerializeObject(inspectionEntity);
                                    var InspectionItemsStr = JsonConvert.SerializeObject(InspectionItems);
                                    //转化实体
                                    var inspectionJobEntity = JsonConvert.DeserializeObject<DeviceInspectionJobEntity>(inspectionEntityStr);
                                    var InspectionJobItems = JsonConvert.DeserializeObject<List<DeviceInspectionItemJobEntity>>(InspectionItemsStr);
                                    var keyvalue = Guid.NewGuid().ToString();
                                    inspectionJobEntity.Id = keyvalue;
                                    inspectionJobEntity.MeetId = model.Data.MeetingId;
                                    inspectionJobEntity.JobId = item.JobId;
                                    inspectionJobEntity.Workuser = string.Join(",", item.Relation.JobUsers.Select(x => x.UserName));
                                    inspectionJobEntity.WorkuserId = string.Join(",", item.Relation.JobUsers.Select(x => x.UserId));
                                    _inspectionJobbll.SaveForm("", inspectionJobEntity, InspectionJobItems);

                                }
                            }
                            #endregion
                        }
                    }


                }

                bll.StartMeeting(model.Data, DateTime.Now, user.RealName);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        /// <summary>
        /// 开始班前会
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object PostMeeting()
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            try
            {
                string res = HttpContext.Current.Request.Form["json"];
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var meeting = JsonConvert.DeserializeObject<WorkmeetingEntity>(json["data"].ToString());
                UserEntity user = new UserBLL().GetEntity(userId);

                meeting.MeetingPerson = user.RealName;

                for (int i = 0; i < meeting.Files.Count; i++)
                {
                    if (meeting.Files[i].State != 1)
                    {
                        meeting.Files.Remove(meeting.Files[i]);
                        i--;
                    }
                }

                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
                }

                if (!Directory.Exists(Path.Combine(filedir, "AppFile", "Meet")))
                {
                    Directory.CreateDirectory(Path.Combine(filedir, "AppFile", "Meet"));
                }

                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[i];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    var fileentity = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = meeting.MeetingId,
                        RecId = meeting.MeetingId,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/Meet/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = file.ContentType.StartsWith("audio") ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now
                    };
                    file.SaveAs(Path.Combine(filedir, "AppFile", "Meet", fileId + ext));
                    //保存附件信息
                    meeting.Files.Add(fileentity);
                }

                meeting.MeetingId = Guid.NewGuid().ToString();
                var bll = new WorkmeetingBLL();
                bll.AddStartMeeting(meeting);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, count = 1 };
        }

        [HttpPost]
        public object FindJobs([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(List<JobTemplateEntity>);
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var key = json.Value<string>("data");
                int pagesize = json.Value<int>("pageSize");
                int pageindex = json.Value<int>("pageIndex");

                var trainingtype = Config.GetValue("TrainingType");
                if (trainingtype == "人身风险预控")
                {
                    HumanDangerBLL bll1 = new HumanDangerBLL();
                    var data1 = bll1.Find(key, user.DepartmentId, 10, pageindex, out total);
                    data = data1.Select(x => new JobTemplateEntity() { JobId = x.HumanDangerId.ToString(), JobContent = x.Task, WorkArea = x.TaskArea }).ToList();
                }
                else
                {
                    WorkmeetingBLL meetBll = new WorkmeetingBLL();
                    var data2 = meetBll.Find(key, user.DepartmentId, 10);
                    data = data2;
                }
                total = data.Count;
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = total };
        }

        public ListBucket<string> Query(ParamBucket<string> args)
        {
            var user = OperatorProvider.Provider.Current();
            WorkmeetingBLL meetBll = new WorkmeetingBLL();
            var total = 0;
            var data = meetBll.Query(args.Data, user.DeptId, args.PageSize, args.PageIndex, out total);
            return new ListBucket<string>() { Data = data, Total = data.Count };
        }

        public object QueryNew(ParamBucket<string> args)
        {
            try
            {
                var user = OperatorProvider.Provider.Current();
                WorkmeetingBLL meetBll = new WorkmeetingBLL();
                var total = 0;
                var data = meetBll.QueryNew(args.Data, user.DeptId, args.PageSize, args.PageIndex, out total);
                return new { code = 0, data, total = data.Count };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message };
            }
        }


        [HttpPost]
        public object FindDangerous([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(List<JobTemplateEntity>);
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var key = json.Value<string>("data");
                int pagesize = json.Value<int>("pageSize");

                var bll = new WorkmeetingBLL();
                data = bll.GetDangerous(user.DepartmentId);
                total = data.Count;
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = total };
        }

        //[HttpPost]
        //public object PostDangerous([FromBody]JObject jobject)
        //{
        //    var result = 0;
        //    var message = string.Empty;
        //    var total = 0;
        //    var data = default(List<JobTemplateEntity>);
        //    try
        //    {
        //        string res = jobject.Value<string>("json");
        //        var json = JObject.Parse(res.ToString());
        //        string userId = json.Value<string>("userId");
        //        UserEntity user = new UserBLL().GetEntity(userId);
        //        var dangerous = json.Value<string>("data");
        //        var entity = new JobTemplateEntity() { JobId = Guid.NewGuid().ToString(), DeptId = user.DepartmentId, Dangerous = dangerous, CreateDate = DateTime.Now, DangerType = "dangerous" };

        //        var bll = new WorkmeetingBLL();
        //        bll.AddDangerous(entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = 1;
        //        message = ex.Message;
        //    }

        //    return new { code = result, info = message };
        //}

        [HttpPost]
        public object PostMeasure([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(List<JobTemplateEntity>);
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var measure = json.Value<string>("data");
                var entity = new JobTemplateEntity() { JobId = Guid.NewGuid().ToString(), DeptId = user.DepartmentId, Measure = measure, CreateDate = DateTime.Now, DangerType = "measure" };

                var bll = new WorkmeetingBLL();
                bll.AddMeasure(entity);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        //[HttpPost]
        //public object DeleteDangerous([FromBody]JObject jobject)
        //{
        //    var result = 0;
        //    var message = string.Empty;
        //    var total = 0;
        //    var data = default(List<JobTemplateEntity>);
        //    try
        //    {
        //        string res = jobject.Value<string>("json");
        //        var json = JObject.Parse(res.ToString());
        //        string userId = json.Value<string>("userId");
        //        UserEntity user = new UserBLL().GetEntity(userId);
        //        var id = json.Value<string>("data");

        //        var bll = new WorkmeetingBLL();
        //        bll.DeleteMeasure(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = 1;
        //        message = ex.Message;
        //    }

        //    return new { code = result, info = message };
        //}

        //[HttpPost]
        //public object DeleteMeasure([FromBody]JObject jobject)
        //{
        //    var result = 0;
        //    var message = string.Empty;
        //    var total = 0;
        //    var data = default(List<JobTemplateEntity>);
        //    try
        //    {
        //        string res = jobject.Value<string>("json");
        //        var json = JObject.Parse(res.ToString());
        //        string userId = json.Value<string>("userId");
        //        UserEntity user = new UserBLL().GetEntity(userId);
        //        var id = json.Value<string>("data");

        //        var bll = new WorkmeetingBLL();
        //        bll.DeleteMeasure(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = 1;
        //        message = ex.Message;
        //    }

        //    return new { code = result, info = message };
        //}

        [HttpPost]
        public object FindMeasures([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(List<JobTemplateEntity>);
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var key = json.Value<string>("data");
                int pagesize = json.Value<int>("pageSize");

                var bll = new WorkmeetingBLL();
                data = bll.GetMeasures(user.DepartmentId);
                total = data.Count;
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = total };
        }

        [HttpPost]
        public object GetMeetings([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(List<WorkmeetingEntity>);
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var from = json.SelectToken("data.from").Value<DateTime?>();
                var to = json.SelectToken("data.to").Value<DateTime?>();
                var deptid = json.SelectToken("data.deptid").Value<string>();
                int pagesize = json.Value<int>("pageSize");
                int pageindex = json.Value<int>("pageIndex");
                bool? isEvaluate = json.Value<bool?>("data.isEvaluate");
                var list_dept = new List<string>();
                if (!string.IsNullOrEmpty(deptid))
                {
                    var deptids = new DepartmentBLL().GetSubDepartments(deptid, null);
                    list_dept.AddRange(deptids.Select(x => x.DepartmentId));
                }
                else
                {
                    var depts = new WorkOrderBLL().GetWorkOrderGroup(user.DepartmentId);
                    if (depts != null && depts.Count() > 0) list_dept.AddRange(depts.Select(x => x.departmentid));
                    else list_dept.Add(user.DepartmentId);
                }

                var bll = new WorkmeetingBLL();
                data = bll.GetList(list_dept.ToArray(), from, to, isEvaluate, userId, pageindex, pagesize, out total).ToList();
                //total = data.Count;
                var path = Config.GetValue("FilePath");
                foreach (var item in data)
                {
                    foreach (var item1 in item.Files)
                    {
                        if (item1.Description == "音频")
                        {
                            var Pathurl = path + item1.FilePath.Replace("~/Resource", string.Empty).Replace("/", "\\");
                            item1.duration = BSFramework.Util.WMP.GetDurationByWMPLib(Pathurl);

                        }
                        item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
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
        [HttpPost]
        public object GetMeetingsTotal([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var resultCount = 0;
            var data = default(List<WorkmeetingEntity>);
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var from = json.SelectToken("data.from").Value<DateTime?>();
                var to = json.SelectToken("data.to").Value<DateTime?>();
                var bll = new WorkmeetingBLL();
                data = bll.GetList(new string[] { user.DepartmentId }, from, to, null, null, 1, 100000, out total).ToList();
                data.ForEach(x => x.MeetingStartTime = Convert.ToDateTime(x.MeetingStartTime.ToString("yyyy-MM-dd")));
                var timeTotal = data.GroupBy(x => x.MeetingStartTime, (x, y) => new
                {
                    day = x,
                    sum =
                        y.Select(g => g.MeetingType == "班后会").Count() > 0 && y.Select(g => g.MeetingType == "班后会").Count() > 0 ? 1 : 0
                }).ToList();
                resultCount = timeTotal.Sum(x => x.sum);

            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }


            return new { code = result, info = message, count = resultCount };
        }
        [HttpPost]
        public object GetMeetingDetail([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = new List<WorkmeetingEntity>();
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var meetingid = json.Value<string>("data");
                var url = ConfigurationManager.AppSettings["AppUrl"].ToString();

                var bll = new WorkmeetingBLL();
                var startmeeting = bll.GetDetail(meetingid);
                var path = Config.GetValue("FilePath");


                var endmeeting = bll.GetDetail(startmeeting.OtherMeetingId);

                total = data.Count;
                data.Add(startmeeting);
                if (endmeeting == null)
                    endmeeting = new WorkmeetingEntity();
                data.Add(endmeeting);

                foreach (var item in data)
                {
                    if (item.Files != null)
                    {
                        foreach (var item1 in item.Files)
                        {
                            if (item1.Description == "音频")
                            {
                                var Pathurl = path + item1.FilePath.Replace("~/Resource", string.Empty).Replace("/", "\\");
                                item1.duration = BSFramework.Util.WMP.GetDurationByWMPLib(Pathurl);
                                item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                            }
                            else if (item1.Description == "视频")
                            {
                                item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                                if (!string.IsNullOrEmpty(item1.OtherUrl))
                                {
                                    if (new Uploader().Query(item1.OtherUrl))
                                        item1.FilePath = item1.OtherUrl;
                                }
                            }
                            else
                            {
                                item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                            }
                        }
                    }
                    if (item.Jobs != null)
                    {
                        foreach (var item1 in item.Jobs)
                        {
                            item1.Status = item1.IsFinished == "finish" ? "已完成" : item1.IsFinished == "undo" ? item1.StartTime.Date < item1.EndTime.Date ? "进行中" : "未完成" : "已取消";
                            item1.piccount = 0;
                            item1.audiocount = 0;
                            if (item1.Relation.JobUsers != null)
                            {
                                foreach (var item2 in item1.Relation.JobUsers)
                                {
                                    var people = new PeopleBLL().GetEntity(item2.UserId);
                                    if (people != null) item2.ImageUrl = people.Photo;
                                }
                            }
                            if (item1.Files != null)
                            {
                                item1.piccount = item1.Files.Count(x => x.Description == "照片");
                                item1.audiocount = item1.Files.Count(x => x.Description == "音频");

                                foreach (var item2 in item1.Files)
                                {
                                    if (item2.Description == "音频")
                                    {
                                        var Pathurl = path + item2.FilePath.Replace("~/Resource", string.Empty).Replace("/", "\\");
                                        item2.duration = BSFramework.Util.WMP.GetDurationByWMPLib(Pathurl);
                                        item2.FilePath = url + item2.FilePath.Replace("~/", string.Empty);
                                    }
                                    else if (item2.Description == "视频")
                                    {
                                        if (!string.IsNullOrEmpty(item2.OtherUrl))
                                        {
                                            if (new Uploader().Query(item2.OtherUrl))
                                                item2.FilePath = item2.OtherUrl;
                                        }
                                    }
                                    else
                                    {
                                        item2.FilePath = url + item2.FilePath.Replace("~/", string.Empty);
                                    }
                                }
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

            return new { code = result, info = message, data, count = total };
        }

        [HttpPost]
        public object AddJob([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(MeetingJobEntity);
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var model = JsonConvert.DeserializeObject<MeetingJobEntity>(json["data"].ToString());
                model.JobId = Guid.NewGuid().ToString();
                model.CreateDate = DateTime.Now;
                model.CreateUserId = userId;
                model.JobType = "班前班后会";
                model.GroupId = user.DepartmentId;
                model.Relation.JobId = model.JobId;
                model.Relation.IsFinished = model.IsFinished = "undo";
                model.Relation.MeetingJobId = Guid.NewGuid().ToString();
                model.Relation.JobUserId = string.Join(",", model.Relation.JobUsers.Select(x => x.UserId));
                model.Relation.JobUser = string.Join(",", model.Relation.JobUsers.Select(x => x.UserName));
                foreach (var item in model.Relation.JobUsers)
                {
                    item.JobUserId = Guid.NewGuid().ToString();
                    item.CreateDate = DateTime.Now;
                    item.MeetingJobId = model.Relation.MeetingJobId;
                }
                if (model.DangerousList == null) model.DangerousList = new List<JobDangerousEntity>();
                foreach (var item in model.DangerousList)
                {
                    item.JobDangerousId = Guid.NewGuid().ToString();
                    item.CreateTime = DateTime.Now;
                    item.JobId = model.JobId;
                    foreach (var item1 in item.MeasureList)
                    {
                        item1.JobMeasureId = Guid.NewGuid().ToString();
                        item1.CreateTime = DateTime.Now;
                        item1.JobDangerousId = item.JobDangerousId;
                    }
                }
                model.Dangerous = string.Join("。", model.DangerousList.Select(x => x.Content));
                model.Measure = string.Join("。", model.DangerousList.SelectMany(x => x.MeasureList).Select(x => x.Content));

                var bll = new WorkmeetingBLL();

                data = bll.AddNewJob(model);
                #region 设备循环检查
                //班前会开始前新增无效 
                if (!string.IsNullOrEmpty(data.Relation.StartMeetingId) || !string.IsNullOrEmpty(data.Relation.EndMeetingId))
                {

                    var MeetId = data.Relation.StartMeetingId;

                    var jobId = model.JobId;
                    if (!string.IsNullOrEmpty(data.Relation.EndMeetingId))
                    {
                        MeetId = data.Relation.EndMeetingId;
                    }


                    //设备巡回检查表
                    DeviceInspectionBLL _inspectionbll = new DeviceInspectionBLL();
                    //设备巡回检查副本表
                    DeviceInspectionJobBLL _inspectionJobbll = new DeviceInspectionJobBLL();
                    //对任务库模板数据进行业务处理


                    //手机app
                    if (!string.IsNullOrEmpty(model.deviceId))
                    {
                        //获取实体
                        var inspectionEntity = _inspectionbll.GetEntity(model.deviceId);
                        if (inspectionEntity != null)
                        {
                            var InspectionItems = _inspectionbll.GetDeviceInspectionItems(model.deviceId);
                            //转json
                            var inspectionEntityStr = JsonConvert.SerializeObject(inspectionEntity);
                            var InspectionItemsStr = JsonConvert.SerializeObject(InspectionItems);
                            //转化实体
                            var inspectionJobEntity = JsonConvert.DeserializeObject<DeviceInspectionJobEntity>(inspectionEntityStr);
                            var InspectionJobItems = JsonConvert.DeserializeObject<List<DeviceInspectionItemJobEntity>>(InspectionItemsStr);
                            var keyvalue = Guid.NewGuid().ToString();
                            inspectionJobEntity.Id = keyvalue;
                            inspectionJobEntity.Workuser = string.Join(",", data.Relation.JobUsers.Select(x => x.UserName));
                            inspectionJobEntity.WorkuserId = string.Join(",", data.Relation.JobUsers.Select(x => x.UserId));
                            inspectionJobEntity.MeetId = MeetId;
                            inspectionJobEntity.JobId = jobId;
                            _inspectionJobbll.SaveForm("", inspectionJobEntity, InspectionJobItems);
                        }
                    }
                    else
                    {
                        //安卓终端
                        if (!string.IsNullOrEmpty(model.TemplateId))
                        {


                            //获取数据模板
                            var entity = bll.GetJobTemplate(model.TemplateId);
                            if (entity != null)
                            {

                                #region 设备巡回检查需要进行处理

                                if (!string.IsNullOrEmpty(entity.RecId))
                                {
                                    //获取实体
                                    var inspectionEntity = _inspectionbll.GetEntity(entity.RecId);
                                    if (inspectionEntity != null)
                                    {
                                        var InspectionItems = _inspectionbll.GetDeviceInspectionItems(entity.RecId);
                                        //转json
                                        var inspectionEntityStr = JsonConvert.SerializeObject(inspectionEntity);
                                        var InspectionItemsStr = JsonConvert.SerializeObject(InspectionItems);
                                        //转化实体
                                        var inspectionJobEntity = JsonConvert.DeserializeObject<DeviceInspectionJobEntity>(inspectionEntityStr);
                                        var InspectionJobItems = JsonConvert.DeserializeObject<List<DeviceInspectionItemJobEntity>>(InspectionItemsStr);
                                        var keyvalue = Guid.NewGuid().ToString();
                                        inspectionJobEntity.Id = keyvalue;
                                        inspectionJobEntity.MeetId = MeetId;
                                        inspectionJobEntity.JobId = jobId;
                                        inspectionJobEntity.Workuser = string.Join(",", data.Relation.JobUsers.Select(x => x.UserName));
                                        inspectionJobEntity.WorkuserId = string.Join(",", data.Relation.JobUsers.Select(x => x.UserId));
                                        _inspectionJobbll.SaveForm("", inspectionJobEntity, InspectionJobItems);
                                    }


                                }
                                #endregion
                            }

                        }

                    }

                }
                #endregion
                var messagebll = new MessageBLL();
                if (!string.IsNullOrEmpty(model.Relation.StartMeetingId))
                {
                    var meeting = bll.GetEntity(model.Relation.StartMeetingId);
                    if (meeting != null && meeting.IsOver)
                    {
                        var trainingtype = Config.GetValue("TrainingType");
                        if (trainingtype == "人身风险预控")
                        {
                            if (model.NeedTrain)
                            {
                                var training = new HumanDangerTrainingEntity() { TrainingId = Guid.NewGuid().ToString(), TrainingTask = model.Job, CreateTime = DateTime.Now, CreateUserId = userId, MeetingJobId = model.Relation.MeetingJobId, DeptId = model.GroupId, TrainingPlace = model.JobAddr, No = model.TicketCode, TicketId = model.TicketId };
                                if (!string.IsNullOrEmpty(model.TemplateId)) training.HumanDangerId = model.TemplateId;
                                training.TrainingUsers = model.Relation.JobUsers.Select(x => new TrainingUserEntity() { TrainingUserId = Guid.NewGuid().ToString(), UserId = x.UserId, UserName = x.UserName, TrainingRole = x.JobType == "ischecker" ? 1 : 0 }).ToList();
                                new HumanDangerTrainingBLL().Add(training);
                                foreach (var item in training.TrainingUsers)
                                {
                                    messagebll.SendMessage("人身风险预控", item.TrainingUserId.ToString());
                                }
                            }
                        }
                        else if (trainingtype == "CARC")
                        {
                            var card = new CarcEntity
                            {
                                Id = Guid.NewGuid().ToString(),
                                WorkName = model.Job,
                                WorkArea = model.JobAddr,
                                DataType = model.NeedTrain ? "carc" : "card",
                                StartTime = model.StartTime,
                                EndTime = model.EndTime,
                                MeetId = model.Relation.StartMeetingId,
                                TutelagePersonId = model.Relation.JobUsers.Find(x => x.JobType == "ischecker").UserId,
                                TutelagePerson = model.Relation.JobUsers.Find(x => x.JobType == "ischecker").UserName,
                                OperationPersonId = string.Join(",", model.Relation.JobUsers.Where(x => x.JobType == "isdoperson").Select(x => x.UserId)),
                                OperationPerson = string.Join(",", model.Relation.JobUsers.Where(x => x.JobType == "isdoperson").Select(x => x.UserName)),
                            };
                            new CarcOrCardBLL().SaveForm(new List<CarcEntity>() { card }, user.UserId);
                        }
                        else
                        {
                            if (model.NeedTrain)
                            {
                                var dangerbll = new DangerBLL();

                                var danger = dangerbll.Save(data);
                                if (danger != null)
                                    messagebll.SendMessage(Config.GetValue("CustomerModel"), danger.Id);
                            }
                        }
                    }

                    if (meeting.IsOver)
                    {
                        //var users = new UserBLL().GetUsersByIds(entity.Relation.JobUsers.Select(x => x.UserId));
                        //MessageClient.SendRequest(users.Select(x => x.Account).ToArray(), entity.JobId, "任务", "新任务", entity.Job);
                        messagebll.SendMessage("工作提示", model.Relation.MeetingJobId);
                    }
                }

                if (data.StartTime.Date != DateTime.Today)
                {
                    data = null;
                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data };
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UpdateJob2([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            string res = jobject.Value<string>("json");
            var json = JObject.Parse(res.ToString());
            string userId = json.Value<string>("userId");
            UserEntity user = new UserBLL().GetEntity(userId);
            var entity = JsonConvert.DeserializeObject<MeetingJobEntity>(json["data"].ToString());
            entity.CreateUserId = userId;
            if (entity.DangerousList == null) entity.DangerousList = new List<JobDangerousEntity>();
            foreach (var item in entity.DangerousList)
            {
                if (string.IsNullOrEmpty(item.JobDangerousId))
                {
                    item.JobDangerousId = Guid.NewGuid().ToString();
                    item.CreateTime = DateTime.Now;
                    item.JobId = entity.JobId;
                    if (item.MeasureList == null) item.MeasureList = new List<JobMeasureEntity>();
                    foreach (var item1 in item.MeasureList)
                    {
                        if (string.IsNullOrEmpty(item1.JobMeasureId))
                        {
                            item1.JobMeasureId = Guid.NewGuid().ToString();
                            item1.JobDangerousId = item.JobDangerousId;
                            item1.CreateTime = DateTime.Now;
                        }
                    }
                }
            }
            entity.Dangerous = string.Join("。", entity.DangerousList.Select(x => x.Content));
            entity.Measure = string.Join("。", entity.DangerousList.SelectMany(x => x.MeasureList).Select(x => x.Content));

            if (entity.Relation != null && entity.Relation.JobUsers != null)
            {
                foreach (var item in entity.Relation.JobUsers)
                {
                    if (string.IsNullOrEmpty(item.JobUserId))
                    {
                        item.MeetingJobId = entity.Relation.MeetingJobId;
                        item.JobUserId = Guid.NewGuid().ToString();
                        item.CreateDate = DateTime.Now;
                    }
                }
            }

            try
            {
                var trainingtype = Config.GetValue("TrainingType");
                var trainingtype2 = Config.GetValue("CustomerModel");
                var bll = new WorkmeetingBLL();
                bll.UpdateJob2(entity, trainingtype, trainingtype2);

                if (entity.IsFinished == "cancel")
                    new MessageBLL().SendMessage("工作任务取消", entity.Relation.MeetingJobId);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            if (entity.StartTime.Date > DateTime.Today)
                entity = null;

            return new { code = result, info = message, data = entity };
        }

        [HttpPost]
        public object DeleteJob()
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var job = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<MeetingAndJobEntity>>(json);

            try
            {
                var bll = new WorkmeetingBLL();



                //设备巡回检查副本表
                DeviceInspectionJobBLL _inspectionJobbll = new DeviceInspectionJobBLL();
                var entity = _inspectionJobbll.GetEntityByMeetOrJob("", job.Data.JobId).FirstOrDefault();
                if (entity != null)
                {
                    //对任务库模板数据进行业务处理
                    _inspectionJobbll.RemoveInspection(entity.Id);
                }


                bll.DeleteJob(job.Data);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object Signin([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var data = default(List<MeetingSigninEntity>);
            var total = 0;
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var signins = JsonConvert.DeserializeObject<List<MeetingSigninEntity>>(json["data"].ToString());

                var bll = new WorkmeetingBLL();
                data = bll.Signin(signins);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = data == null ? 0 : data.Count };
        }

        [HttpPost]
        public object SyncState([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var data = default(List<MeetingSigninEntity>);
            var total = 0;
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var signins = JsonConvert.DeserializeObject<List<MeetingSigninEntity>>(json["data"].ToString());

                var bll = new WorkmeetingBLL();
                data = bll.SyncState(signins);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data, count = data == null ? 0 : data.Count };
        }

        [HttpPost]
        public object Score([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var job = JsonConvert.DeserializeObject<MeetingJobEntity>(json["data"].ToString());

                var bll = new WorkmeetingBLL();
                bll.Score(job);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object UploadVideo()
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            try
            {
                string res = HttpContext.Current.Request.Form["json"];
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                var meetingid = JsonConvert.DeserializeObject<string>(json["data"].ToString());
                UserEntity user = new UserBLL().GetEntity(userId);

                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
                }

                if (!Directory.Exists(Path.Combine(filedir, "AppFile", "Meet")))
                {
                    Directory.CreateDirectory(Path.Combine(filedir, "AppFile", "Meet"));
                }

                var files = new List<FileInfoEntity>();
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[i];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    var fileentity = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = meetingid,
                        RecId = meetingid,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/Meet/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = "视频",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateUserId = userId,
                        CreateDate = DateTime.Now
                    };
                    file.SaveAs(Path.Combine(filedir, "AppFile", "Meet", fileId + ext));
                    //保存附件信息
                    files.Add(fileentity);
                }

                var bll = new WorkmeetingBLL();
                bll.PostVideo(files);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, count = 1 };
        }

        [HttpPost]
        public ListBucket<HumanDangerTrainingEntity> OverMeeting([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;

            var data = new List<HumanDangerTrainingEntity>();
            string res = jobject.Value<string>("json");
            var json = JObject.Parse(res.ToString());
            string userId = json.Value<string>("userId");
            UserEntity user = new UserBLL().GetEntity(userId);
            var meetingid = json.Value<string>("data");

            var bll = new WorkmeetingBLL();
            bll.OverMeeting(meetingid, userId, DateTime.Now);

            var meeting = bll.GetDetail(meetingid);

            bll.finishwork(meeting);
            var isHumanDanger = ConfigurationManager.AppSettings["isHumanDanger"].ToString();

            if (isHumanDanger == "是")
            {
                data = bll.HumanDangerMeeting(meeting, user);
            }
            var messagebll = new MessageBLL();
            if (meeting.MeetingType == "班前会")
            {
                foreach (var item in meeting.Jobs)
                {
                    //var users = new UserBLL().GetUsersByIds(item.Relation.JobUsers.Select(x => x.UserId));
                    //MessageClient.SendRequest(users.Select(x => x.Account).ToArray(), item.JobId, "任务", "新任务", item.Job);
                    messagebll.SendMessage("工作提示", item.Relation.MeetingJobId);
                }
            }
            else
            {
                foreach (var item in meeting.Jobs)
                {
                    messagebll.FinishTodo("工作提示", item.Relation.MeetingJobId);
                    if (item.Training != null)
                        messagebll.FinishTodo(Config.GetValue("CustomerModel"), item.Training.Id);
                    if (item.HumanDangerTraining != null)
                    {
                        foreach (var item1 in item.HumanDangerTraining.TrainingUsers)
                        {
                            messagebll.FinishTodo("人身风险预控", item1.TrainingUserId.ToString());
                        }
                    }
                }
            }

            //return new { code = result, info = message, data = data };
            return new ListBucket<HumanDangerTrainingEntity>() { Data = data, Success = true };
        }

        [HttpPost]
        public object StartEndMeeting([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = string.Empty;
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var meeting = JsonConvert.DeserializeObject<WorkmeetingEntity>(json["data"].ToString());

                meeting.MeetingPerson = user.RealName;
                meeting.MeetingStartTime = DateTime.Now;
                var bll = new WorkmeetingBLL();


                data = bll.StartEndMeeting(meeting);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data };
        }
        /// <summary>
        /// 获取部门组织结构列表
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetDepartmentData([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;

            var data = new List<DepartmentEntity>();
            var dept = new List<DepartmentData>();
            try
            {
                var departmentBLL = new DepartmentBLL();

                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString()); ;
                var departId = json.Value<string>("data");
                string userId = json.Value<string>("userId");

                //  var rootdpet = departmentBLL.GetRootDepartment();
                UserEntity user = new UserBLL().GetEntity(userId);
                if (string.IsNullOrEmpty(departId) || departId == "0" || departId == "-1")
                {
                    var tree = departmentBLL.GetAuthorizationDepartmentApp(user.DepartmentId);

                    departId = tree.DepartmentId;
                    var root = departmentBLL.GetEntity(departId);
                    data.Add(root);
                }
                else
                {
                    data = departmentBLL.GetDepartmentList(departId).ToList();

                }
                //data = data.Where(row => row.Nature == "部门").ToList();
                foreach (DepartmentEntity de in data)
                {
                    de.NumberOfPeople = departmentBLL.GetPeopleNumber(de.DepartmentId).ToString();
                    DepartmentData dt = new DepartmentData();
                    dt.DepartmentId = de.DepartmentId;
                    dt.FullName = de.FullName;
                    dt.Nature = de.Nature;
                    dt.NumberOfPeople = int.Parse(de.NumberOfPeople);
                    dept.Add(dt);
                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = dept };
        }
        /// <summary>
        /// 获取成员列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetUserList([FromBody] JObject jobject)
        {
            UserBLL ubll = new UserBLL();
            var result = 0;
            var message = string.Empty;
            var dept = new List<UserEntity>();
            var userList = new List<UserData>();
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString()); ;
                var departId = json.Value<string>("data");
                string userId = json.Value<string>("userId");
                UserWorkStateBLL bll = new UserWorkStateBLL();
                dept = ubll.GetDeptUserBook(departId, userId).ToList();
                foreach (UserEntity ue in dept)
                {
                    UserData ul = new UserData();
                    ul.UserId = ue.UserId;
                    ul.RealName = ue.RealName.Trim();
                    ul.DepartmentId = ue.DepartmentId;
                    var getState = bll.selectState(ue.UserId);
                    ul.State = getState == null ? "" : getState.State;
                    userList.Add(ul);
                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }
            return new { code = result, info = message, data = userList };
        }
        /// <summary>
        /// 获取成员信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetUserInfo([FromBody] JObject jobject)
        {
            UserBLL ubll = new UserBLL();
            DepartmentBLL dbll = new DepartmentBLL();
            var result = 0;
            var message = string.Empty;
            var user = new UserEntity();
            var uie = new UserIEntity();
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString()); ;
                var userId = json.Value<string>("data");
                //var res = jobject.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                //string userId = dy.userId;
                UserWorkStateBLL bll = new UserWorkStateBLL();
                user = ubll.GetEntity(userId);
                uie = new UserIEntity();
                uie.UserId = user.UserId;
                uie.RealName = user.RealName;
                uie.Mobile = user.Mobile;
                var getState = bll.selectState(userId);

                uie.State = getState == null ? "" : getState.State;
                var dept = dbll.GetEntity(user.DepartmentId);
                if (dept != null)
                {
                    uie.FullName = dept.FullName;
                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }
            return new { code = result, info = message, data = uie };
        }

        [HttpPost]
        public object PostMeetingRemark()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<WorkmeetingEntity>>(json);
            var data = default(WorkmeetingEntity);

            try
            {
                var bll = new WorkmeetingBLL();
                data = bll.UpdateRemark(model.Data);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = data };
        }
        /// <summary>
        /// 获取值班人信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetBeOnDutyStaff([FromBody] JObject json)
        {
            WorkmeetingBLL wbll = new WorkmeetingBLL();
            var result = 0;
            var message = string.Empty;
            var staffList = new List<UserData>();
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.data.userId;

                var staff = wbll.GetBeOnDutyStaffInfo();
                foreach (UserInfoEntity ue in staff)
                {
                    var ck = staffList.FirstOrDefault(x => x.UserId == ue.UserId);
                    if (ck == null)
                    {
                        UserData ul = new UserData();
                        ul.UserId = ue.UserId;
                        ul.RealName = ue.RealName.Trim();
                        ul.DepartmentId = ue.DepartmentId;
                        ul.DeptName = ue.DeptName;
                        ul.Mobile = ue.Mobile;
                        ul.State = ue.Description;
                        staffList.Add(ul);

                    }
                    else
                    {
                        ck.State = ck.State + "," + ue.Description;
                    }

                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }
            return new { code = result, info = message, data = staffList };
        }

        /// <summary>
        /// 双控的旁站监督任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object PostMonitorJob()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var models = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MeetingJobEntity>>(json);
            NLog.LogManager.GetCurrentClassLogger().Info("收到旁站监督 {0}", json);
            try
            {
                var bll = new WorkmeetingBLL();
                foreach (var item in models)
                {
                    item.JobId = Guid.NewGuid().ToString();
                    item.JobType = "旁站监督";
                    item.IsFinished = "undo";
                    item.CreateDate = DateTime.Now;
                    bll.PostMonitorJob(item);
                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object UpdateMonitorJob()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<MeetingJobEntity>(json);
            NLog.LogManager.GetCurrentClassLogger().Info("更新旁站监督 {0}", json);

            try
            {
                var bll = new WorkmeetingBLL();

                bll.UpdateMonitorJob(model);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object GetJobHistory()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);
            var data = default(List<MeetingJobEntity>);
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();

            try
            {
                var bll = new WorkmeetingBLL();
                data = bll.GetJobHistory(new MeetingJobEntity() { JobId = model.Data });
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.Files != null)
                        {
                            foreach (var item1 in item.Files)
                            {
                                item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
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

            return new { code = result, info = message, data };
        }

        [HttpPost]
        public ModelBucket<WorkmeetingEntity> GetStartMeeting(ParamBucket<string> arg)
        {
            var bll = new WorkmeetingBLL();
            var data = bll.GetDetail(arg.Data);
            var path = Config.GetValue("FilePath");
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            if (data.Files != null)
            {
                foreach (var item1 in data.Files)
                {
                    if (item1.Description == "音频")
                    {
                        var Pathurl = path + item1.FilePath.Replace("~/Resource", string.Empty).Replace("/", "\\");
                        item1.duration = BSFramework.Util.WMP.GetDurationByWMPLib(Pathurl);
                        item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                    }
                    else if (item1.Description == "视频")
                    {
                        item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                        if (!string.IsNullOrEmpty(item1.OtherUrl))
                        {
                            if (new Uploader().Query(item1.OtherUrl))
                                item1.FilePath = item1.OtherUrl;
                        }
                    }
                    else
                    {
                        item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                    }
                }
            }
            if (data.Jobs != null)
            {
                foreach (var item1 in data.Jobs)
                {
                    item1.audiocount = 0;
                    item1.piccount = 0;
                    if (item1.Relation.JobUsers != null)
                    {
                        foreach (var item2 in item1.Relation.JobUsers)
                        {
                            var people = new PeopleBLL().GetEntity(item2.UserId);
                            if (people != null) item2.ImageUrl = people.Photo;
                        }
                    }
                    if (item1.Files != null)
                    {
                        item1.piccount = item1.Files.Count(x => x.Description == "照片");
                        item1.audiocount = item1.Files.Count(x => x.Description == "音频");
                        foreach (var item2 in item1.Files)
                        {
                            if (item2.Description == "音频")
                            {
                                var Pathurl = path + item2.FilePath.Replace("~/Resource", string.Empty).Replace("/", "\\");
                                item2.duration = BSFramework.Util.WMP.GetDurationByWMPLib(Pathurl);
                                item2.FilePath = url + item2.FilePath.Replace("~/", string.Empty);
                            }
                            else if (item2.Description == "视频")
                            {
                                if (!string.IsNullOrEmpty(item2.OtherUrl))
                                {
                                    if (new Uploader().Query(item2.OtherUrl))
                                        item2.FilePath = item2.OtherUrl;
                                }
                            }
                            else
                            {
                                item2.FilePath = url + item2.FilePath.Replace("~/", string.Empty);
                            }
                        }
                    }
                }
            }

            return new ModelBucket<WorkmeetingEntity>() { Data = data };
        }

        /// <summary>
        /// 获取默认出勤设置
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<MeetingSigninEntity> GetDefaultSigns(ParamBucket<string> arg)
        {
            var bll = new WorkmeetingBLL();
            var data = bll.GetDefaultSigns(arg.Data);
            return new ListBucket<MeetingSigninEntity>() { Success = true, Data = data, Total = data.Count };
        }

        /// <summary>
        /// 设置默认出勤
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket SetDefaultSigns(ParamBucket<List<MeetingSigninEntity>> arg)
        {
            var success = true;
            var message = string.Empty;
            var bll = new WorkmeetingBLL();
            try
            {
                bll.SetDefaultSigns(arg.Data);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return new ResultBucket() { Success = success, Message = message };
        }

        /// <summary>
        /// 获取下面所有班组
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<DepartmentEntity> GetSubTeams(ParamBucket<string> arg)
        {
            var user = new UserBLL().GetEntity(arg.UserId);
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DepartmentId);
            var data = new DepartmentBLL().GetSubDepartments(dept.DepartmentId, "班组");
            return new ListBucket<DepartmentEntity>() { Success = true, Data = data, Total = data.Count };
        }

        /// <summary>
        /// 获取班会台账列表
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<MeetingEntity> GetMeetingList(ParamBucket<DateLimit> arg)
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

            var total = 0;
            var bll = new WorkmeetingBLL();
            var data = bll.GetMeetingList(listdept.ToArray(), user.UserId, arg.Data.IsEvaluate, arg.Data.From, arg.Data.To, arg.PageSize, arg.PageIndex, out total);
            return new ListBucket<MeetingEntity>() { Success = true, Data = data, Total = total };
        }

        /// <summary>
        /// 获取危险因素和防范措施
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<RiskFactorSetEntity> GetDangerous(ParamBucket<string> arg)
        {
            var user = new UserBLL().GetEntity(arg.UserId);
            RiskFactorSetBLL riskFactorSetBLL = new RiskFactorSetBLL();
            var list = riskFactorSetBLL.GetList(user.DepartmentId, arg.Data).ToList();
            MeasureSetBLL measuresbll = new MeasureSetBLL();
            foreach (var item in list)
            {
                item.measures = measuresbll.GetList(item.ID).ToList();
            }
            return new ListBucket<RiskFactorSetEntity>() { Success = true, Data = list, Total = list.Count };
        }

        /// <summary>
        /// 提交危险因素
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket PostDangerous(ParamBucket<RiskFactorSetEntity> arg)
        {
            var success = true;
            var message = string.Empty;
            var user = new UserBLL().GetEntity(arg.UserId);
            var dept = new DepartmentBLL().GetEntity(user.DepartmentId);
            RiskFactorSetBLL riskFactorSetBLL = new RiskFactorSetBLL();
            if (string.IsNullOrEmpty(arg.Data.ID))
            {
                arg.Data.ID = Guid.NewGuid().ToString();
                arg.Data.CreateDate = DateTime.Now;
                arg.Data.CreateUserId = user.UserId;
                arg.Data.CreateUserName = user.RealName;
                arg.Data.CreateDeptId = user.DepartmentId;
                arg.Data.CreateDeptCode = user.DepartmentCode;
                arg.Data.CreateDeptName = dept.FullName;
            }
            arg.Data.DeptId = user.DepartmentId;
            arg.Data.ModifyDate = DateTime.Now;
            arg.Data.ModifyUserId = user.UserId;
            arg.Data.ModifyUserName = user.RealName;
            foreach (var item in arg.Data.measures)
            {
                item.CreateDate = DateTime.Now;
                item.RiskFactorId = arg.Data.ID;
            }

            try
            {
                riskFactorSetBLL.SaveForm(arg.Data);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }
            return new ResultBucket() { Success = success, Message = message };
        }

        /// <summary>
        /// 删除危险因素
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket DeleteDangerous(ParamBucket<string> arg)
        {
            var success = true;
            var message = string.Empty;
            var user = new UserBLL().GetEntity(arg.UserId);
            RiskFactorSetBLL riskFactorSetBLL = new RiskFactorSetBLL();
            try
            {
                riskFactorSetBLL.RemoveForm(arg.Data);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }
            return new ResultBucket() { Success = success, Message = message };
        }

        /// <summary>
        /// 删除防范措施
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket DeleteMeasure(ParamBucket<string> arg)
        {
            var success = true;
            var message = string.Empty;
            var user = new UserBLL().GetEntity(arg.UserId);
            MeasureSetBLL measuresetbll = new MeasureSetBLL();
            try
            {
                measuresetbll.Delete(arg.Data);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }
            return new ResultBucket() { Success = success, Message = message };
        }

        /// <summary>
        /// 获取轮班的班组
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<WorkGroupSetEntity> GetGroups(ParamBucket args)
        {
            var user = OperatorProvider.Provider.Current();
            var data = new WorkOrderBLL().GetWorkOrderGroup(user.DeptId);
            return new ListBucket<WorkGroupSetEntity>() { Success = true, Data = data.ToList() };
        }

        #region 当班记事
        /// <summary>
        /// 新增或修改当班记事
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SaveMeetingNote()
        {
            var result = 0;
            var message = "操作成功";
            WorkMeetingNoteBLL noteBLL = new WorkMeetingNoteBLL();
            FileInfoBLL fileInfoBLL = new FileInfoBLL();
            UserBLL ubll = new UserBLL();

            try
            {
                string res = HttpContext.Current.Request["json"];
                var rq = JsonConvert.DeserializeAnonymousType(res, new
                {
                    userid = string.Empty,
                    data = new
                    {
                        id = string.Empty,
                        meetingid = string.Empty,
                        describes = string.Empty,
                        deletefileids = string.Empty,
                        fileinfos = new List<FileInfos>()
                    }
                });
                if (rq.data == null)
                {
                    throw new ArgumentNullException("参数有误：data为空");
                }
                if (string.IsNullOrEmpty(rq.data.meetingid))
                {
                    throw new ArgumentNullException("参数有误：data.bzid为空");
                }
                //文件上传路径 
                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                filedir = Path.Combine(filedir, "AppFile", "MeetingNote");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
                }
                //用户信息
                UserEntity user = ubll.GetEntity(rq.userid);

                WorkMeetingNoteEntity noteEntity = noteBLL.GetEntity(rq.data.id);
                if (noteEntity == null)
                {
                    noteEntity = new WorkMeetingNoteEntity()
                    {
                        Id = Guid.NewGuid().ToString(),
                        MeetingId = rq.data.meetingid,
                        CreateUserId = user.UserId,
                        CreateUser = user.RealName,
                        MODIFYUser = user.RealName,
                        MODIFYUserId = user.UserId,
                        CreateDate = DateTime.Now,
                        MODIFYDATE = DateTime.Now,
                        Describes = rq.data.describes
                    };
                    noteBLL.Insert(noteEntity);
                }
                else
                {
                    noteEntity.MODIFYUser = user.RealName;
                    noteEntity.MODIFYUserId = user.UserId;
                    noteEntity.MODIFYDATE = DateTime.Now;
                    noteEntity.Describes = rq.data.describes;
                    noteBLL.Update(noteEntity);
                }
                //上传前检测是否有删除文件
                if (!string.IsNullOrEmpty(rq.data.deletefileids))
                {
                    string[] fileids = rq.data.deletefileids.Split(',');

                    foreach (string fileid in fileids)
                    {
                        var fileEntity = fileInfoBLL.GetEntity(fileid);
                        if (fileEntity != null)
                        {
                            fileInfoBLL.DeleteFile(fileEntity.RecId, fileEntity.FileName, HttpContext.Current.Server.MapPath(fileEntity.FilePath));
                        }
                    }
                }
                //文件上传
                HttpFileCollection files = HttpContext.Current.Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i] != null)
                    {
                        HttpPostedFile file = files[i];
                        string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                        string fileId = Guid.NewGuid().ToString();//上传后文件名
                        string filename = fileId + ext;
                        FileInfos fis = rq.data.fileinfos.ElementAt(i);
                        fis.modifydate = new DateTime(fis.modifydate.Value.Year, fis.modifydate.Value.Month, fis.modifydate.Value.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        var fileentity = new FileInfoEntity
                        {
                            FileId = fileId,
                            FolderId = noteEntity.Id,
                            RecId = noteEntity.Id,
                            FileName = filename,
                            FilePath = "~/Resource/AppFile/MeetingNote/" + filename,
                            FileType = fis.filetype,
                            FileExtensions = ext,
                            Description = fis.description,
                            FileSize = file.ContentLength.ToString(),
                            DeleteMark = 0,
                            CreateUserId = rq.userid,
                            CreateDate = DateTime.Now,
                            ModifyDate = fis.modifydate,
                            ModifyUserId = fis.modifyuserid,
                            ModifyUserName = fis.modifyusername,
                            SortCode = fis.key
                        };
                        file.SaveAs(Path.Combine(filedir, filename));

                        fileInfoBLL.SaveForm(fileentity);
                    }
                }
            }
            catch (Exception ex)
            {
                result = -1;
                message = ex.Message;
            }
            return new { code = result, info = message };
        }
        /// <summary>
        /// 获取当班记事
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetMeetingNote([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userid;
                string id = dy.data.meetingid;
                UserEntity user = new UserBLL().GetEntity(userId);
                if (user == null)
                {
                    throw new ArgumentNullException("参数有误：用户为空");
                }
                FileInfoBLL fileInfoBLL = new FileInfoBLL();
                WorkMeetingNoteBLL noteBLL = new WorkMeetingNoteBLL();
                var entity = noteBLL.GetEntityByMeetingId(id);
                if (entity != null)
                {
                    IList<FileInfoEntity> fileInfoList = fileInfoBLL.GetCultureWallPics(entity.Id);
                    if (fileInfoList != null && fileInfoList.Count > 0)
                    {
                        entity.Files = new List<FileClass>();
                    }
                    var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
                    foreach (FileInfoEntity f in fileInfoList)
                    {
                        entity.Files.Add(new FileClass()
                        {
                            fileid = f.FileId,
                            description = f.Description,
                            createdate = f.CreateDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            modifydate = f.ModifyDate.HasValue ? f.ModifyDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                            filepath = f.FilePath.Replace("~/", url),
                            filetype = f.FileType,
                            key = (int)(f.SortCode ?? 0m)
                        });
                    }
                }
                return new { Code = 0, Message = "操作成功", Data = entity };
            }
            catch (Exception ex)
            {
                return new { Code = -1, ex.Message };
            }
        }
        #endregion

        [HttpPost]
        public ModelBucket<bool> IsEndMeetingOver(ParamBucket args)
        {
            var user = OperatorProvider.Provider.Current();
            var data = new WorkmeetingBLL().IsEndMeetingOver(user.DeptId, DateTime.Now);
            return new ModelBucket<bool>() { Success = true, Data = data };
        }

        [HttpPost]
        public ModelBucket<JobTemplateEntity> Detail(ParamBucket<string> args)
        {
            var user = OperatorProvider.Provider.Current();
            var data = new WorkmeetingBLL().Detail(user.DeptId, args.Data);
            return new ModelBucket<JobTemplateEntity>() { Data = data };
        }

        #region  早安中铝 班会记录
        [HttpPost]
        public object SaveMeetingRecord()
        {
            try
            {
                var json = HttpContext.Current.Request["json"];
                var dy = JsonConvert.DeserializeAnonymousType(json, new
                {
                    userId = string.Empty,
                    data = new MeetingRecordEntity()
                });
                if (dy.data == null) throw new ArgumentNullException("data不能为空");
                if (!string.IsNullOrEmpty(dy.data.Id))
                {
                    //修改
                    dy.data.Modify();
                    _meetingReocrdbll.Update(dy.data);
                }
                else
                {
                    //新增
                    DepartmentEntity dept = new DepartmentBLL().GetEntity(OperatorProvider.Provider.Current().DeptId);
                    dy.data.Create(dept);
                    _meetingReocrdbll.Insert(dy.data);
                }
                return new { code = 0, info = "操作成功", data = "操作成功" };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "操作失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 根据班会的ID查询班会记录
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetMeetingRecord([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                if (dy.data == null) throw new ArgumentNullException("data不能为空");
                string meetingId = dy.data;
                MeetingRecordEntity entity = _meetingReocrdbll.GetEntityByMeetingId(meetingId);
                return new { code = 0, info = "查询成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message };
            }
        }
        #endregion

        #region 班前一题/一课
        /// <summary>
        /// 获取班前一题  根据班会Id
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetMeetingQuestion([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                if (dy.data == null) throw new ArgumentNullException("data不能为空");
                string meetingId = dy.data;
                MeetingQuestionEntity entity = _questionbll.GetEntityByMeetingId(meetingId);
                if (entity == null)
                {
                    entity = new MeetingQuestionEntity();
                    UserEntity userinfo = new UserBLL().GetEntity(dy.userId);
                    if (userinfo == null) throw new Exception("用户不存在");
                    EduInventoryEntity question = new EducationBLL().GetRadEntity(userinfo.DepartmentCode);
                    if (question != null)
                    {
                        entity.Question = question.Question;
                        entity.Answer = question.Answer;
                    }
                }
                return new { code = 0, info = "查询成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 保存班前一题
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SaveMeetingQuestion()
        {
            try
            {
                var json = HttpContext.Current.Request["json"];
                var dy = JsonConvert.DeserializeAnonymousType(json, new
                {
                    userId = string.Empty,
                    data = new MeetingQuestionEntity()
                });
                if (dy.data == null) throw new ArgumentNullException("data不能为空");
                if (!string.IsNullOrEmpty(dy.data.Id))
                {
                    //修改
                    dy.data.Modify();
                    _questionbll.Update(dy.data);
                }
                else
                {
                    //新增
                    DepartmentEntity dept = new DepartmentBLL().GetEntity(OperatorProvider.Provider.Current().DeptId);
                    dy.data.Create(dept);
                    _questionbll.Insert(dy.data);
                }
                return new { code = 0, info = "操作成功", data = "操作成功" };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "操作失败", data = ex.Message };
            }
        }

        #endregion

        #region 班会任务-工时管理
        /// <summary>
        /// 登记/修改工时
        /// </summary>
        /// <param name="userentity"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket SaveTaskHour(ListParam<JobUserEntity> userentity)
        {
            new WorkmeetingBLL().SaveTaskHour(userentity.Data);
            return new Models.ResultBucket();
        }

        /// <summary>
        /// 登记/修改工时
        /// </summary>
        /// <param name="userentity"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetJobHourPagedList(ParamBucket<MeetingQuery> query)
        {
            int total = 0;
            System.Collections.IList list = new WorkmeetingBLL().GetJobHourPagedList(query.Data.StartTime, query.Data.EndTime, query.Data.KeyWord, query.Data.DeptCode, query.PageIndex, query.PageSize, out total);
            return new { code = 0, data = list, count = total, total = total };
        }

        #endregion
    }

    public class FileCompare : IEqualityComparer<FileInfoEntity>
    {

        public bool Equals(FileInfoEntity x, FileInfoEntity y)
        {
            return x.FileId == y.FileId;
        }

        public int GetHashCode(FileInfoEntity obj)
        {
            return obj.GetHashCode();
        }

    }

}