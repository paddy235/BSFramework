using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.QuestionManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.QuestionManage;
using BSFramework.Busines.WorkMeeting;
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
    public class EdActiviesController : BaseApiController
    {
        /// <summary>
        /// 2.获取班组活动列表
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
                string title = dy.data.title; //活动主题
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                EdActivityBLL act = new EdActivityBLL();
                int total = 0;
                IList list = act.GetList(int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total, int.Parse(status.ToString()), title, user.DepartmentId);
                return new { code = 0, info = "获取数据成功", count = total, data = new { activies = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 3.获取班组活动详情
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
                string id = dy.data.ActivityId;//记录Id
                EdActivityBLL actBll = new EdActivityBLL();
                EdActivityEntity act = actBll.GetEntity(id);//获取活动详情
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                string persons = string.Join(",", act.ActivityPersons.Where(x => x.IsSigned).Select(y => y.Person));
                var list = fileBll.GetFilesByRecIdNew(id);//获取相关附件集合
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                if (list != null) list = list.Where(x => x.Description != "二维码").ToList();
                foreach (var item in list)
                {
                    item.FilePath = item.FilePath.Replace("~/", url);
                }
                return new { code = 0, info = "获取数据成功", data = new { ActivityId = act.ActivityId, Subject = act.Subject, ActivityPlace = act.ActivityPlace, Persons = string.IsNullOrEmpty(persons) ? "无" : persons, Attachments = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 4.班组活动上传音频或图片
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object Upload()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data.ActivityId;//业务记录Id
                EdActivityBLL actBll = new EdActivityBLL();
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
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
                        FilePath = "~/Resource/AppFile/Activies/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        FileSize = file.ContentLength.ToString(),
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        DeleteMark = 0
                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Activies"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Activies");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Activies\\" + fileId + ext);
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
        /// 获取活动类型
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object rGetActivityCategory([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            //重新排序
            var SortList = new List<ActivityCategoryEntity>();
            var data = default(List<ActivityCategoryEntity>);
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);
                var bll = new ActivityBLL();
                var sbll = new SafetydayBLL();
                data = bll.GetIndex(user.UserId, user.DepartmentId, null);
                foreach (ActivityCategoryEntity ace in data)
                {
                    ace.Total = sbll.GetIndex(user.DepartmentId, userId, ace.ActivityCategory).ToList().Count();
                }
                //重新排序
                var one = data.FirstOrDefault(x => x.ActivityCategory == "安全日活动");
                if (one != null)
                {
                    SortList.Add(one);
                    data.Remove(one);
                }
                var two = data.FirstOrDefault(x => x.ActivityCategory == "政治学习");
                if (two != null)
                {
                    SortList.Add(two);
                    data.Remove(two);
                }
                var three = data.FirstOrDefault(x => x.ActivityCategory == "民主生活会");
                if (three != null)
                {
                    SortList.Add(three);
                    data.Remove(three);
                }
                var four = data.FirstOrDefault(x => x.ActivityCategory == "班务会");
                if (four != null)
                {
                    SortList.Add(four);
                    data.Remove(four);
                }
                data = data.OrderBy(x => x.CreateTime).ToList();
                SortList.AddRange(data);

            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = SortList };
        }

        /// <summary>
        /// 增加班组活动类型
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddActivityCategory([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(ActivityCategoryEntity);
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);

                var category = json.Value<string>("data");
                var model = new ActivityCategoryEntity() { ActivityCategory = category, ActivityCategoryId = Guid.NewGuid().ToString(), CreateTime = DateTime.Now, CreateUserId = userId, DeptId = user.DepartmentId };

                var bll = new ActivityBLL();
                data = bll.AddCategory(model);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data };
        }

        /// <summary>
        /// 删除班组活动类型
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public object DeleteActivityCategory([FromBody] JObject jobject)
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

                var categoryid = json.Value<string>("data");

                var bll = new ActivityBLL();
                bll.DeleteCategory(categoryid);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        /// <summary>
        /// 编辑班组活动类型
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public object UpdateActivityCategory([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var total = 0;
            var data = default(ActivityCategoryEntity);
            try
            {
                string res = jobject.Value<string>("json");
                var json = JObject.Parse(res.ToString());
                string userId = json.Value<string>("userId");
                UserEntity user = new UserBLL().GetEntity(userId);

                var category = JsonConvert.DeserializeObject<ActivityCategoryEntity>(json["data"].ToString());

                var bll = new ActivityBLL();
                data = bll.UpdateActivityCategory(category);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data };
        }

        /// <summary>
        /// 班组活动/按照分类，获取对应的活动信息
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetCurrentActivity([FromBody] JObject jobject)
        {
            var result = 0;
            var message = string.Empty;
            var data = default(EdActivityEntity);
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            try
            {
                string res = jobject.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userId;
                string category = dy.data.category;
                string operate = dy.data.operate;
                string safetydayId = dy.data.safetydayId;
                string safetydayfileid = dy.data.safetydayfileId;

                UserEntity user = new UserBLL().GetEntity(userId);
                var bll = new EdActivityBLL();
                data = bll.GetActivities(category, user.DepartmentId);

                if (data == null)
                {
                    UserBLL userBLL = new UserBLL();
                    SafetydayBLL sbll = new SafetydayBLL();
                    FileInfoBLL fbll = new FileInfoBLL();
                    var users = new PeopleBLL().GetListByDept(user.DepartmentId);
                    var bzaqy = users.FirstOrDefault(x => x.Quarters != null && x.Quarters.Contains("安全员"));
                    var strSubject = string.Empty;
                    var strState = "Ready";
                    if (category == "安全学习日")
                    {
                        strState = "Prepare";
                    }
                    var safetyday = sbll.GetIdEntityList(safetydayId);
                    if (safetyday != null)
                    {
                        foreach (SafetydayEntity se in safetyday)
                        {
                            strSubject += se.Subject + " ";
                            sbll.SaveRead(se.Id, userId);
                        }
                    }
                    data = new EdActivityEntity() { State = strState, Subject = strSubject, PlanStartTime = DateTime.Now, PlanEndTime = DateTime.Now.AddHours(4), ActivityPlace = "班组办公室", ActivityLimited = "4小时", ChairPerson = user.RealName, RecordPerson = bzaqy == null ? string.Empty : bzaqy.Name, AlertType = "提前15分钟", ActivityType = category, GroupId = user.DepartmentId };
                    data.ActivityPersons = users.Select(x => new EdActivityPersonEntity() { ActivityId = data.ActivityId, ActivityPersonId = Guid.NewGuid().ToString(), PersonId = x.ID, Person = x.Name, IsSigned = true }).ToList();
                    //需要修改的文件 是否已经使用
                    foreach (SafetydayEntity se in safetyday)
                    {
                        var file = fbll.GetFilesByRecIdNew(se.Id).ToList();
                        if (data.Files == null) data.Files = new List<FileInfoEntity>();
                        if (file != null)
                        {
                            foreach (FileInfoEntity fie in file.ToList())
                            {
                                if (safetydayfileid.Contains(fie.FileId))
                                {
                                    //var fileid = Guid.NewGuid().ToString();
                                    var query = new FileInfoEntity() { FileId = fie.FileId, FolderId = fie.FileId, CreateDate = DateTime.Now, CreateUserId = user.UserId, CreateUserName = user.RealName, Description = "班组活动", FileExtensions = System.IO.Path.GetExtension(fie.FileName), FileName = fie.FileName, FilePath = fie.FilePath, FileType = fie.FileType, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.RealName, RecId = data.ActivityId };
                                    //fbll.SaveForm(query);
                                    query.RecId = data.ActivityId;
                                    query.Description = "材料";
                                    data.Files.Add(query);

                                }

                            }
                        }
                    }

                }
                if (data.ActivityPersons == null || data.ActivityPersons.Count == 0)
                {
                    UserBLL userBLL = new UserBLL();
                    var users = new PeopleBLL().GetListByDept(user.DepartmentId);

                    if (string.IsNullOrEmpty(data.ActivityId))
                    {
                        var bzaqy = users.FirstOrDefault(x => x.Quarters != null && x.Quarters.Contains("安全员"));
                        data.RecordPerson = bzaqy == null ? string.Empty : bzaqy.Name;
                        data.ChairPerson = user.RealName;
                    }

                    data.ActivityPersons = users.Select(x => new EdActivityPersonEntity() { ActivityId = data.ActivityId, ActivityPersonId = Guid.NewGuid().ToString(), PersonId = x.ID, Person = x.Name, IsSigned = true }).ToList();
                }

                if (category == "安全学习日")
                {
                    //清理文件不存在的试题  转化实体
                    var QuestionList = new List<QuestionBankEntity>();
                    QuestionBankBLL question = new QuestionBankBLL();
                    if (!string.IsNullOrEmpty(safetydayId))
                    {
                        var tosafetydayId = string.Empty;
                        if (safetydayId.Contains(','))
                        {
                            var safetydayIds = safetydayId.Split(',');
                            foreach (var item in safetydayIds)
                            {
                                var questionModel = question.GetDetailbyOutId(item);
                                foreach (var questionentity in questionModel)
                                {
                                    var ckFile = true;

                                    var filesId = questionentity.fileids.Split(',');
                                    foreach (var fileid in filesId)
                                    {
                                        if (!string.IsNullOrEmpty(fileid))
                                        {

                                            if (!safetydayfileid.Contains(fileid))
                                            {
                                                ckFile = false;
                                                break;
                                            }

                                        }
                                    }
                                    if (ckFile)
                                    {
                                        var FilesId = data.Files.Where(x => !string.IsNullOrEmpty(x.ShareLink)).Where(x => questionentity.fileids.Contains(x.ShareLink)).ToList();
                                        questionentity.fileids = string.Join(",", FilesId.Select(x => x.FileId));
                                        QuestionList.Add(questionentity);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var questionModel = question.GetDetailbyOutId(safetydayId);
                            foreach (var questionentity in questionModel)
                            {
                                var ckFile = true;

                                var filesId = questionentity.fileids.Split(',');
                                foreach (var fileid in filesId)
                                {
                                    if (!string.IsNullOrEmpty(fileid))
                                    {

                                        if (!safetydayfileid.Contains(fileid))
                                        {
                                            ckFile = false;
                                            break;
                                        }

                                    }
                                }
                                if (ckFile)
                                {
                                    var FilesId = data.Files.Where(x => !string.IsNullOrEmpty(x.ShareLink)).Where(x => questionentity.fileids.Contains(x.ShareLink)).ToList();
                                    questionentity.fileids = string.Join(",", FilesId.Select(x => x.FileId));
                                    QuestionList.Add(questionentity);
                                }
                            }
                        }
                    }
                    var getQuestionStr = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionList);
                    var QuestionBank = Newtonsoft.Json.JsonConvert.DeserializeObject<List<HistoryQuestionEntity>>(getQuestionStr);
                    data.HistoryQuestion = QuestionBank;
                }
                data.Persons = string.Join(",", data.ActivityPersons.Where(x => x.IsSigned).Select(x => x.Person));
                data.PersonId = string.Join(",", data.ActivityPersons.Where(x => x.IsSigned).Select(x => x.PersonId));
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }


            if (data != null && data.Files != null)
            {
                var diskPath = ConfigurationManager.AppSettings["FilePath"].ToString();
                foreach (var item in data.Files)
                {
                    if (string.IsNullOrEmpty(item.Description)) item.Description = "材料";
                    switch (Path.GetExtension(item.FileName).ToLower())
                    {
                        case ".doc":
                        case ".docx":
                        case ".xls":
                        case ".xlsx":
                        case ".ppt":
                        case ".pptx":
                            if (System.IO.File.Exists(diskPath + ("~/Resource/ActivityPDF/" + item.FileId + ".pdf").Replace("~/Resource", string.Empty)))
                            {
                                item.ViewUrl = url + "/Pdf/ViewPDFPage?httpUrl=" + HttpUtility.UrlEncode(url + ("~/Resource/ActivityPDF/" + item.FileId + ".pdf").Replace("~/", string.Empty));
                                item.CanView = true;
                            }
                            else
                            {
                                using (var factory = new ChannelFactory<IQueueService>("upload"))
                                {
                                    var channel = factory.CreateChannel();
                                    channel.OfficeToPdf(diskPath + item.FilePath.Replace("~/Resource", string.Empty), diskPath + ("/ActivityPDF/" + item.FileId + ".pdf"));
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    item.FilePath = url + item.FilePath.Replace("~/", string.Empty);
                }
            }

            return new { code = result, info = message, data = data };
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostActivity()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "ssss" };
            }

            var root = HttpContext.Current.Server.MapPath("~/logs");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                foreach (MultipartFileData file in provider.FileData)
                {
                }
                return this.Request.CreateResponse<string>(HttpStatusCode.OK, "ok");
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> StartActivity()
        {
            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Activity";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<EdActivityEntity>>(json);
                var isnew = false;

                if (model.Data.Files == null)
                    model.Data.Files = new List<FileInfoEntity>();

                if (string.IsNullOrEmpty(model.Data.ActivityId))
                {
                    model.Data.ActivityId = Guid.NewGuid().ToString();
                    model.Data.Files.Add(this.BuildImage(model.Data.ActivityId, model.UserId));
                    isnew = true;
                }

                var userbll = new UserBLL();
                var user = userbll.GetEntity(model.UserId);
                FileInfoBLL filebll = new FileInfoBLL();

                foreach (FileInfoEntity f in model.Data.Files)
                {
                    if (f.Description != "二维码" && f.Description != "课件")
                    {
                        if (f.State.HasValue)
                        {

                            if (f.State.Value != 2)
                            {
                                var baseFile = filebll.GetEntity(f.ShareLink);
                                if (baseFile == null)
                                {
                                    baseFile = filebll.GetEntity(f.FileId);
                                    f.FileId = Guid.NewGuid().ToString();
                                }
                                if (!baseFile.IsShare.HasValue)
                                {
                                    filebll.ShareFile(baseFile.FileId, 1);
                                }
                                else
                                {
                                    if (baseFile.IsShare.Value == 0)
                                    {

                                        filebll.ShareFile(baseFile.FileId, 1);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var baseFile = filebll.GetEntity(f.ShareLink);
                            if (baseFile == null)
                            {
                                baseFile = filebll.GetEntity(f.FileId);
                                f.FileId = Guid.NewGuid().ToString();
                            }
                            if (!baseFile.IsShare.HasValue)
                            {
                                filebll.ShareFile(baseFile.FileId, 1);
                            }
                            else
                            {
                                if (baseFile.IsShare.Value == 0)
                                {

                                    filebll.ShareFile(baseFile.FileId, 1);
                                }
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(f.FileId))
                    {
                        f.FileId = Guid.NewGuid().ToString();
                    }
                    f.RecId = model.Data.ActivityId;
                    var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
                    if (f.Description == "课件")
                    {
                        f.FilePath = f.FilePath;
                    }
                    else
                    {
                        f.FilePath = f.FilePath.Replace(url, "~/");
                    }
                    if (string.IsNullOrEmpty(f.Description))
                        f.Description = "材料";
                    f.CreateDate = DateTime.Now;
                    f.CreateUserId = user.UserId;
                    f.CreateUserName = user.CreateUserName;
                    if (f.Description == "课件")
                    {
                        f.FileExtensions = "";
                    }
                    else
                    {
                        f.FileExtensions = System.IO.Path.GetExtension(f.FileName);
                    }
                }
                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { RecId = model.Data.ActivityId, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);
                    fileentity.Description = "材料";
                    fileentity.State = 1;
                    model.Data.Files.Add(fileentity);
                }

                var bll = new EdActivityBLL();
                if (isnew)
                {
                    model.Data.StartTime = DateTime.Now;
                    model.Data.State = "Study";
                    bll.SaveFormSafetyday(model.Data.ActivityId, model.Data, new List<SafetydayEntity>(), user.UserId);
                }
                else
                {
                    if (model.Data.State == "Save")
                    {
                        model.Data.State = "Ready";
                        bll.Edit(model.Data);
                    }
                    else
                    {
                        model.Data.StartTime = DateTime.Now;
                        bll.Ready(model.Data);
                    }
                }

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> StartActivityFinish()
        {
            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Activity";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<EdActivityEntity>>(json);
                var isnew = false;

                if (model.Data.Files == null)
                    model.Data.Files = new List<FileInfoEntity>();

                if (string.IsNullOrEmpty(model.Data.ActivityId))
                {
                    model.Data.ActivityId = Guid.NewGuid().ToString();
                    model.Data.Files.Add(this.BuildImage(model.Data.ActivityId, model.UserId));

                }

                var userbll = new UserBLL();
                var user = userbll.GetEntity(model.UserId);
                FileInfoBLL filebll = new FileInfoBLL();

                foreach (FileInfoEntity f in model.Data.Files)
                {

                    f.RecId = model.Data.ActivityId;
                    var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
                    f.Description = "课件";
                    f.FilePath = f.FilePath;

                    if (string.IsNullOrEmpty(f.Description))
                        f.Description = "材料";
                    f.CreateDate = DateTime.Now;
                    f.CreateUserId = user.UserId;
                    f.CreateUserName = user.CreateUserName;
                    if (f.Description == "课件")
                    {
                        f.FileExtensions = "";
                    }
                    else
                    {
                        f.FileExtensions = System.IO.Path.GetExtension(f.FileName);
                    }
                }
                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { RecId = model.Data.ActivityId, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);
                    fileentity.Description = "材料";
                    fileentity.State = 1;
                    model.Data.Files.Add(fileentity);
                }

                var bll = new EdActivityBLL();

                model.Data.StartTime = DateTime.Now;
                model.Data.State = "Study";
                bll.SaveFormSafetyday(model.Data.ActivityId, model.Data, new List<SafetydayEntity>(), user.UserId);

                model.Data.StartTime = DateTime.Now;
                bll.Ready(model.Data);


                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message, data = model.Data });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        [HttpPost]
        public object EndActivity()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<EdActivityEntity>>(json);
            //List<string> pdf = new List<string> { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx" };
            //FileInfoBLL fileBll = new FileInfoBLL();
            //var files = fileBll.GetFilesByRecIdNew(model.Data.ActivityId).Where(x => x.Description != "二维码" && pdf.Contains(x.FileExtensions));
            //using (var factory = new ChannelFactory<IUploadService>("upload"))
            //{
            //    var channel = factory.CreateChannel();
            //    foreach (FileInfoEntity f in files)
            //    {
            //        channel.Upload(f.FileId);
            //    }

            //}
            try
            {
                var bll = new EdActivityBLL();
                if (model.Data.State == "Finish")
                {
                    model.Data.EndTime = DateTime.Now;
                    bll.Over(model.Data);
                    //委托 线程 执行同步培训云平台 
                    //培训记录同步
                    Action<string, string> delegatePostMethod = new Action<string, string>(Postrecord);
                    //培训试题
                    delegatePostMethod += PostTestQuestion;
                    Task PostTask = new Task(() => delegatePostMethod(model.Data.ActivityId, model.UserId));
                    PostTask.Start();
                }
                else
                    bll.Edit(model.Data);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "结束异常");
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }
        #region 推送培训平台

        /// <summary>
        ///培训记录同步
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        private void PostTestQuestion(string id, string userid)
        {
            //将同步结果写入日志文件
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("————————————安全学习日试题同步培训云平台开始————————————");
            try
            {
                var bll = new QuestionBankBLL();
                var deptBll = new DepartmentBLL();
                var userbll = new UserBLL();
                var url = Config.GetValue("TrainingSystem");
                var GetModel = bll.GetHistoryDetailbyActivityId(id);
                var ip = url.Split('/');
                var ipUrl = ip[0] + "//" + ip[2] + "/" + ip[3];
                var usering = userbll.GetEntity(userid);
                var getUrl = ipUrl + "/BzInterface.asmx/GetServiceHelper";
                List<object> questiongList = new List<object>();
                foreach (var item in GetModel)
                {
                    var questiongModel = new
                    {
                        TestId = string.IsNullOrEmpty(item.safetydayid) ? item.Id : item.safetydayid,
                        TestType = item.topictype == "单选题" ? "单选" : item.topictype == "多选题" ? "多选" : "判断",
                        TestKey = item.topictype.Contains("选") ? string.Join("|", item.TheAnswer.Where(x => x.istrue).Select(x => x.answer)) : item.istrue ? "A" : "B",
                        Difficulty = "中",
                        TestQuestion = item.topictitle,
                        TestOption = item.topictype.Contains("选") ? string.Join("|", item.TheAnswer.Select(x => x.answer + "．" + x.description)) : "A．正确|B．错误",
                        BeiYong = item.description
                    };
                    questiongList.Add(questiongModel);
                }
                var CurriculumName = string.Empty;
                var oneDept = deptBll.GetEntity(usering.DepartmentId);
                if (oneDept.Nature == "班组")
                {
                    var twoDept = deptBll.GetEntity(oneDept.ParentId);
                    CurriculumName += twoDept.FullName;
                }
                CurriculumName += oneDept.FullName + "安全学习日题库";
                var BzData = new { CurriculumName = CurriculumName, TestManagers = questiongList };
                var postStr = new { Business = "SynBzTestManage", UserAccount = usering.Account, BzData = BzData };
                var itemdetialbll = new DataItemDetailBLL();
                var itembll = new DataItemBLL();
                var type = itembll.GetEntityByName("试题同步");
                if (type == null)
                {
                    logger.Info("————————————请试题同步检查编码配置————————————");
                    logger.Info("————————————安全学习日同步培训云平台完成————————————");
                    return;
                }
                var content = itemdetialbll.GetList(type.ItemId);
                if (content == null)
                {
                    logger.Info("————————————请试题同步检查编码配置————————————");
                    logger.Info("————————————安全学习日同步培训云平台完成————————————");
                    return;
                }
                foreach (var item in content)
                {
                    try
                    {
                        logger.Info("————————————安全学习日同步培训云平台用户：" + item.ItemValue + "————————————");
                        postStr = new { Business = "SynBzTestManage", UserAccount = item.ItemValue, BzData = BzData };
                        var returnStr = postStr.ToJson();
                        returnStr = "json=" + returnStr;
                        logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据:" + returnStr + "\r\n\r\n");
                        string res = HttpMethods.HttpPost(getUrl, returnStr);
                        logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "同步信息：" + res + "\r\n\r\n");
                    }
                    catch (Exception ex)
                    {
                        logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "同步信息：" + ex.Message + "\r\n\r\n");
                        logger.Info("————————————安全学习日同步培训云平台请求异常————————————");
                    }


                }

            }
            catch (Exception ex)
            {
                logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "同步信息：" + ex.Message + "\r\n\r\n");
                logger.Info("————————————安全学习日同步培训云平台代码异常————————————");
            }
            finally
            {
                logger.Info("————————————安全学习日同步培训云平台完成————————————");
            }

        }
        /// <summary>
        ///培训记录同步
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        private void Postrecord(string id, string userid)
        {
            var bll = new EdActivityBLL();
            //将同步结果写入日志文件
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("————————————安全学习日同步培训云平台开始————————————");
            #region 推送培训平台
            try
            {

                var url = Config.GetValue("TrainingSystem");
                var GetModel = bll.GetEntity(id);
                var ip = url.Split('/');
                var ipUrl = ip[0] + "//" + ip[2] + "/" + ip[3];
                var people = new PeopleBLL();
                var deptUser = people.GetListByDept(GetModel.GroupId);
                var getUrl = ipUrl + "/BzInterface.asmx/GetServiceHelper";
                var ActivityPersons = new List<object>();
                if (GetModel.ActivityPersons.Count > 0)
                {
                    foreach (var item in GetModel.ActivityPersons.Where(x => x.IsSigned))
                    {
                        var IdentityNo = deptUser.FirstOrDefault(x => x.ID == item.PersonId);
                        var persons = new { ActivityPersonId = item.PersonId, ActivityId = item.ActivityId, PersonId = item.PersonId, Person = item.Person, IsSigned = item.IsSigned, Identifyid = IdentityNo == null ? "" : IdentityNo.IdentityNo };
                        ActivityPersons.Add(persons);
                    }
                }

                var Files = new List<object>();
                if (GetModel.Files.Count > 0)
                {
                    foreach (var item in GetModel.Files)
                    {
                        var file = new { FileName = item.FileName };
                        Files.Add(file);
                    }
                }
                var Supplys = new List<object>();
                var SupplysList = bll.GetSupplyById().Where(x => x.ActivityId == GetModel.ActivityId && x.IsOver == true).OrderByDescending(x => x.StartDate).ToList();

                if (SupplysList.Count > 0)
                {
                    foreach (var item in SupplysList)
                    {
                        var user = item.UserName.Split(',').ToList();
                        var IdentityNo = new List<string>();
                        foreach (var useritem in user)
                        {
                            var name = deptUser.FirstOrDefault(x => x.Name == useritem);
                            if (name != null)
                            {
                                IdentityNo.Add(name.IdentityNo);
                            }
                        }
                        var supply = new { StartTime = item.StartDate, EndTime = item.EndDate, UserName = item.UserName, Identifyid = string.Join(",", IdentityNo) };
                        Supplys.Add(supply);
                    }
                }
                var returnBzData = new { ActivityId = GetModel.ActivityId, Subject = GetModel.Subject, ChairPerson = GetModel.ChairPerson, Identifyid = deptUser.FirstOrDefault(x => x.Name == GetModel.ChairPerson).IdentityNo, RecordPerson = GetModel.RecordPerson, Leader = GetModel.Leader, ActivityType = GetModel.ActivityType, StartTime = GetModel.StartTime, EndTime = GetModel.EndTime, ActivityPersons = ActivityPersons, Files = Files, Supplys = Supplys };
                var returnObj = new { Business = "SynBzProjectData", BzData = returnBzData };
                var returnStr = returnObj.ToJson();
                returnStr = "json=" + returnStr;
                logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据:" + returnStr + "\r\n\r\n");
                // getUrl += returnStr;
                string res = HttpMethods.HttpPost(getUrl, returnStr);

                logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "同步信息：" + res + "\r\n\r\n");
                logger.Info("————————————安全学习日同步培训云平台完成————————————");
            }
            catch (Exception ex)
            {
                logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "同步信息：" + ex.Message + "\r\n\r\n");
            }
            finally
            {
                logger.Info("————————————安全学习日同步培训云平台完成————————————");
            }

            #endregion
        }

        #endregion
        [HttpPost]
        public async Task<HttpResponseMessage> PrepareActivity()
        {
            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Activity";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<EdActivityEntity>>(json);

                var userbll = new UserBLL();
                var user = OperatorProvider.Provider.Current();
                if (string.IsNullOrEmpty(model.Data.ActivityId))
                {
                    model.Data.ActivityId = Guid.NewGuid().ToString();
                }
                foreach (FileInfoEntity f in model.Data.Files)
                {
                    f.ShareLink = f.FileId;
                    f.FileId = Guid.NewGuid().ToString();

                    f.RecId = model.Data.ActivityId;
                    var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
                    if (f.Description == "课件")
                    {
                        f.FilePath = f.FilePath;
                    }
                    else
                    {
                        f.FilePath = f.FilePath.Replace(url, "~/");
                    }
                    if (string.IsNullOrEmpty(f.Description))
                        f.Description = "材料";
                    f.FolderId = f.FolderId;
                    f.CreateDate = DateTime.Now;
                    f.CreateUserId = user.UserId;
                    f.CreateUserName = user.UserName;
                    if (f.Description == "课件")
                    {
                        f.FileExtensions = "";
                    }
                    else
                    {
                        f.FileExtensions = System.IO.Path.GetExtension(f.FileName);
                    }
                }
                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { RecId = model.Data.ActivityId, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);
                    fileentity.Description = "材料";

                    //switch (file.Headers.ContentType.MediaType)
                    //{
                    //    case "image/jpeg":
                    //        fileentity.Description = "照片";
                    //        break;
                    //    case "audio/mpeg":
                    //        fileentity.Description = "音频";
                    //        break;
                    //    case "video/mp4":
                    //        fileentity.Description = "视频";
                    //        break;
                    //    default:
                    //        break;
                    //}

                    fileentity.State = 1;
                    model.Data.Files.Add(fileentity);
                }

                var bll = new EdActivityBLL();
                model.Data.State = "Ready";
                model.Data.GroupName = user.DeptName;
                model.Data.StartTime = DateTime.Now;

                bll.SaveFormSafetyday(model.Data.ActivityId, model.Data, new List<SafetydayEntity>(), model.UserId);
                #region 添加试题
                if (model.Data.ActivityType == "安全学习日")
                {
                    QuestionBankBLL question = new QuestionBankBLL();
                    if (model.Data.HistoryQuestion != null)
                    {


                        var QuestionBank = model.Data.HistoryQuestion.ToList();
                        //生成答题台账
                        if (QuestionBank.Count > 0)
                        {
                            DepartmentBLL dept = new DepartmentBLL();
                            UserBLL userBLL = new UserBLL();
                            var list = new List<TheTitleEntity>();
                            var operationUser = userBLL.GetEntity(user.UserId);
                            var mydept = dept.GetEntity(model.Data.GroupId);
                            var i = 0;
                            foreach (var item in QuestionBank)
                            {
                                item.sort = i;
                                i++;
                            }
                            foreach (var item in model.Data.ActivityPersons)
                            {
                                var title = new TheTitleEntity();
                                title.Id = Guid.NewGuid().ToString();
                                title.startTime = DateTime.Now;
                                title.iscomplete = false;
                                title.category = model.Data.ActivityType;
                                title.activityid = model.Data.ActivityId;
                                var workuser = userBLL.GetEntity(item.PersonId);
                                title.userid = workuser.UserId;
                                title.username = workuser.RealName;
                                title.deptcode = mydept.EnCode;
                                title.deptid = mydept.DepartmentId;
                                title.deptname = mydept.FullName;
                                title.endTime = null;
                                title.score = "0";
                                list.Add(title);
                            }
                            foreach (var bank in QuestionBank)
                            {
                                //bank.titleid = title.Id;
                                bank.outkeyvalue = model.Data.ActivityId;
                            }
                            question.SaveFormHistoryQuestion(list, QuestionBank, operationUser, "");

                        }
                    }
                }
                #endregion

                if (model.Data.Files != null && model.Data.Files.Count > 0)
                {
                    using (var factory = new ChannelFactory<IQueueService>("upload"))
                    {
                        var channel = factory.CreateChannel();
                        foreach (var itemfile in model.Data.Files)
                        {
                            if (itemfile.Description != "课件")
                            {

                            }
                            channel.OfficeToPdf(filepath + itemfile.FilePath.Replace("~/Resource", string.Empty), filepath + "/EdActivityPDF/" + itemfile.FileId + ".pdf");
                        }
                    }
                }

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        public object GetActivities()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var from = jobject.SelectToken("data.from").Value<DateTime>();
            var to = jobject.SelectToken("data.to").Value<DateTime>();
            var category = string.Empty;
            var Evaluates = string.Empty;
            var deptid = string.Empty;
            if (json.Contains("category"))
            {
                category = jobject.SelectToken("data.category").Value<string>();

            }
            if (json.Contains("evaluates"))
            {
                Evaluates = jobject.SelectToken("data.evaluates").Value<string>();

            }
            if (json.Contains("deptid"))
            {
                deptid = jobject.SelectToken("data.deptid").Value<string>();

            }
            var isall = false;
            if (json.Contains("isall"))
            {
                isall = jobject.SelectToken("data.isall").Value<bool>();
            }
            var userid = jobject.Value<string>("userId");
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            var data = default(List<EdActivityEntity>);
            var userbll = new UserBLL();
            var user = userbll.GetEntity(userid);
            to = to.AddDays(1).AddMinutes(-1);
            try
            {
                var bll = new EdActivityBLL();
                var deptBll = new DepartmentBLL();
                data = bll.GetActivities2(userid, from, to, category, deptid, isall);
                foreach (var item in data)
                {
                    var evaluatesList = bll.GetEntityList().Where(x => x.Activityid == item.ActivityId).ToList();
                    var myself = evaluatesList.Select(x => x.EvaluateUser).ToList();
                    if (myself.Contains(user.RealName))
                    {
                        item.Evaluates = evaluatesList;
                    }
                    else
                    {
                        item.Evaluates = new List<ActivityEvaluateEntity>();
                    }
                    var dept = deptBll.GetEntity(item.GroupId);
                    if (dept != null)
                    {
                        item.GroupName = dept.FullName;
                    }
                }
                if (!string.IsNullOrEmpty(Evaluates))
                {
                    if (Evaluates == "已评价")
                    {
                        data = data.Where(x => x.Evaluates.Count() > 0).ToList();
                    }
                    else
                    {
                        data = data.Where(x => x.Evaluates.Count() == 0).ToList();
                    }
                }

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

            return new { code = result, info = message, data = data, count = data.Count };
        }

        [HttpPost]
        public object GetActivitiesToTal(ParamBucket<IndexCountries> args)
        {
            var result = 0;
            var message = string.Empty;
            //var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            //var json = dict.Get("json");
            //var jobject = JObject.Parse(json);
            //var from = jobject.SelectToken("data.from").Value<DateTime>();
            //var to = jobject.SelectToken("data.to").Value<DateTime>();
            //var category = jobject.SelectToken("data.category").Value<string>();
            //var userid = jobject.Value<string>("userId");
            var nowTime = DateTime.Now;
            var startTime = new DateTime(nowTime.Year, nowTime.Month, 1);
            var endTime = new DateTime(nowTime.Year, nowTime.Month, 1).AddMonths(1).AddMinutes(-1);
            if (args.Data.startTime.HasValue)
            {
                startTime = args.Data.startTime.Value;
                endTime = args.Data.endTime.Value;
            }
            var data = default(List<EdActivityEntity>);
            try
            {
                var bll = new EdActivityBLL();
                data = bll.GetActivities2(args.UserId, startTime, endTime, args.Data.switchValue, "", false);
                var Resultdata = data.GroupBy(x => x.ActivityType, (x, y) => new { ActivityType = x, sum = y.Count() }).ToList();
                var other = Resultdata.Where(x => x.ActivityType != "安全日活动" && x.ActivityType != "民主管理会" && x.ActivityType != "政治学习" && x.ActivityType != "班务会").ToList();
                var ohtersum = other.Sum(x => x.sum);
                Resultdata.Add(new { ActivityType = "其他", sum = ohtersum });

                var ck = data.FirstOrDefault(x => x.ActivityType == "安全学习日");
                if (ck == null)
                {
                    Resultdata.Add(new { ActivityType = "安全日活动", sum = 0 });
                }
                ck = data.FirstOrDefault(x => x.ActivityType == "民主管理会");
                if (ck == null)
                {
                    Resultdata.Add(new { ActivityType = "民主管理会", sum = 0 });
                }
                ck = data.FirstOrDefault(x => x.ActivityType == "政治学习");
                if (ck == null)
                {
                    Resultdata.Add(new { ActivityType = "政治学习", sum = 0 });
                }
                ck = data.FirstOrDefault(x => x.ActivityType == "班务会");
                if (ck == null)
                {
                    Resultdata.Add(new { ActivityType = "班务会", sum = 0 });
                }
                return new { code = result, info = message, data = Resultdata, count = data.Count };

            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message };
        }

        [HttpPost]
        public object GetActivityDetail()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            var data = default(EdActivityEntity);

            try
            {
                var bll = new EdActivityBLL();
                data = bll.GetDetail(model.Data);
                var list = bll.GetSupplyById().Where(x => x.ActivityId == data.ActivityId && x.IsOver == true).OrderByDescending(x => x.StartDate);
                data.Supplys = list.ToList();
                data.Evaluates = bll.GetEntityList().Where(x => x.Activityid == data.ActivityId).ToList();
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            if (data != null)
            {
                foreach (var item1 in data.Files)
                {
                    item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                    if (item1.Description == "视频")
                    {
                        if (!string.IsNullOrEmpty(item1.OtherUrl))
                        {
                            if (new Uploader().Query(item1.OtherUrl))
                                item1.FilePath = item1.OtherUrl;
                        }
                    }
                }
            }

            return new { code = result, info = message, data = data };
        }

        private FileInfoEntity BuildImage(string activityid, string userid)
        {
            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode(activityid + "|班组活动", Encoding.UTF8);
            var path = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Activity";

            if (!Directory.Exists(Path.Combine(path, fold)))
                Directory.CreateDirectory(Path.Combine(path, fold));

            image.Save(Path.Combine(path, fold, id + ".jpg"));

            var userbll = new UserBLL();
            var user = userbll.GetEntity(userid);

            return new FileInfoEntity()
            {
                FileId = id,
                CreateDate = DateTime.Now,
                CreateUserId = user.UserId,
                CreateUserName = user.Account,
                Description = "二维码",
                FileExtensions = ".jpg",
                FileName = id + ".jpg",
                FilePath = "~/Resource/" + fold + "/" + id + ".jpg",
                FileType = "jpg",
                ModifyDate = DateTime.Now,
                ModifyUserId = user.UserId,
                ModifyUserName = user.Account,
                RecId = activityid
            };
        }
        [HttpPost]
        public object GetSafetydayList([FromBody] JObject json)
        {
            var result = 0;
            var message = string.Empty;
            SafetydayBLL sbll = new SafetydayBLL();
            FileInfoBLL fbll = new FileInfoBLL();
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userid = dy.userId;
            string deptId = dy.data.deptId;//记录Id
            string category = dy.data.category;
            List<SafetydayIEntity> safetyIList = new List<SafetydayIEntity>();
            var safe = sbll.GetSafetydayList(deptId, category).OrderByDescending(x => x.CreateDate);
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            if (safe != null)
            {
                foreach (SafetydayEntity s in safe)
                {
                    SafetydayIEntity si = new SafetydayIEntity();
                    List<FileIEntity> filePath = new List<FileIEntity>();
                    si.Id = s.Id;
                    si.Subject = s.Subject;
                    si.CreateUserName = s.CreateUserName;
                    si.CreateDate = s.CreateDate;
                    si.Explain = s.Explain;
                    var files = fbll.GetFilesByRecIdNew(s.Id);
                    if (files != null)
                    {
                        foreach (FileInfoEntity f in files.OrderByDescending(x => x.CreateDate))
                        {
                            FileIEntity file = new FileIEntity();
                            file.FileId = f.FileId;
                            file.FileName = f.FileName;
                            file.FilePath = f.FilePath;
                            file.FileType = f.FileType;
                            file.FolderId = f.FileId;
                            file.IsShare = f.IsShare.HasValue ? f.IsShare.Value : 0;
                            file.RecId = f.RecId;
                            file.FilePath = url + file.FilePath.Replace("~/", string.Empty);
                            file.ShareLink = f.FileId;
                            filePath.Add(file);
                        }
                    }
                    si.Files = filePath;
                    var read = sbll.GetSafetydayReadEntity(deptId, userid, s.Id);
                    if (read == null)
                    {
                        si.isRead = 1;
                    }
                    else
                    {
                        si.isRead = read.IsRead;
                    }
                    safetyIList.Add(si);
                }
            }
            return new { code = result, info = message, data = safetyIList };
        }
        [HttpPost]
        public object UpdateSafetyDayIsread([FromBody] JObject json)
        {
            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userid = dy.userId;
            string safetydayId = dy.data.safetydayId;
            SafetydayBLL sbll = new SafetydayBLL();
            sbll.SaveRead(safetydayId, userid);
            return new { code = result, info = message };
        }


        /// <summary>
        /// 开始补课
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object ActivitySuplyStart()
        {
            try
            {
                var bll = new EdActivityBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                string id = dy.data;
                UserEntity user = new UserBLL().GetEntity(userId);
                EdActivitySupplyEntity entity = new EdActivitySupplyEntity();
                entity.ActivityId = id;
                entity.ID = Guid.NewGuid().ToString();
                entity.CreateDate = DateTime.Now;
                entity.StartDate = DateTime.Now;
                entity.CreateUserId = userId;
                entity.CreateUserName = user.RealName;
                entity.IsOver = false;
                bll.SaveActivitySupply(entity.ID, entity);
                return new { code = 0, info = "成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 结束补课
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object ActivitySuplyEnd()
        {
            try
            {
                var bll = new EdActivityBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                string id = dy.data.id;
                string name = dy.data.name;
                string userid = dy.data.userid;
                var entity = bll.GetActivitySupplyEntity(id);
                //更新补课状态
                entity.EndDate = DateTime.Now;
                entity.IsOver = true;
                entity.UserName = name;
                bll.SaveActivitySupply(entity.ID, entity);


                var act = bll.GetEntity(entity.ActivityId);
                //保存补课人员信息

                string[] arrname = name.Split(',');
                string[] arruserid = userid.Split(',');
                SupplyPeopleEntity obj = new SupplyPeopleEntity();
                for (int i = 0; i < arrname.Length; i++)
                {
                    obj = new SupplyPeopleEntity();
                    obj.ID = Guid.NewGuid().ToString();
                    obj.SupplyId = id;
                    obj.UserName = arrname[i];
                    obj.UserId = arruserid[i];
                    bll.SaveSupplyPeople(obj.ID, obj);
                    //更新班组活动缺勤人员
                    var person = act.ActivityPersons.Where(x => x.PersonId == arruserid[i]).FirstOrDefault();
                    if (person != null)
                    {
                        person.IsSigned = true;
                        bll.SaveActivityPerson(person);
                    }
                }
                #region 推送培训平台
                var url = Config.GetValue("TrainingSystem");
                var GetModel = bll.GetEntity(entity.ActivityId);
                var ip = url.Split('/');
                var ipUrl = ip[0] + "//" + ip[2] + "/" + ip[3];
                var people = new PeopleBLL();
                var deptUser = people.GetListByDept(GetModel.GroupId);
                var getUrl = ipUrl + "/BzInterface.asmx/GetServiceHelper";
                var ActivityPersons = new List<object>();
                if (GetModel.ActivityPersons.Count > 0)
                {
                    foreach (var item in GetModel.ActivityPersons.Where(x => x.IsSigned))
                    {
                        var IdentityNo = deptUser.FirstOrDefault(x => x.ID == item.PersonId);
                        var persons = new { ActivityPersonId = item.PersonId, ActivityId = item.ActivityId, PersonId = item.PersonId, Person = item.Person, IsSigned = item.IsSigned, Identifyid = IdentityNo == null ? "" : IdentityNo.IdentityNo };
                        ActivityPersons.Add(persons);
                    }
                }

                var Files = new List<object>();
                if (GetModel.Files.Count > 0)
                {
                    foreach (var item in GetModel.Files)
                    {
                        var file = new { FileName = item.FileName };
                        Files.Add(file);
                    }
                }
                var Supplys = new List<object>();
                var SupplysList = bll.GetSupplyById().Where(x => x.ActivityId == GetModel.ActivityId && x.IsOver == true).OrderByDescending(x => x.StartDate).ToList();

                if (SupplysList.Count > 0)
                {
                    foreach (var item in SupplysList)
                    {
                        var userList = item.UserName.Split(',').ToList();
                        var IdentityNo = new List<string>();
                        foreach (var useritem in userList)
                        {
                            var username = deptUser.FirstOrDefault(x => x.Name == useritem);
                            if (username != null)
                            {
                                IdentityNo.Add(username.IdentityNo);
                            }
                        }
                        var supply = new { StartTime = item.StartDate, EndTime = item.EndDate, UserName = item.UserName, Identifyid = string.Join(",", IdentityNo) };
                        Supplys.Add(supply);
                    }
                }
                var returnBzData = new { ActivityId = GetModel.ActivityId, Subject = GetModel.Subject, ChairPerson = GetModel.ChairPerson, Identifyid = deptUser.FirstOrDefault(x => x.Name == GetModel.ChairPerson).IdentityNo, RecordPerson = GetModel.RecordPerson, Leader = GetModel.Leader, ActivityType = GetModel.ActivityType, StartTime = GetModel.StartTime, EndTime = GetModel.EndTime, ActivityPersons = ActivityPersons, Files = Files, Supplys = Supplys };
                var returnObj = new { Business = "SynBzProjectData", BzData = returnBzData };
                var returnStr = returnObj.ToJson();
                returnStr = "json=" + returnStr;
                // getUrl += returnStr;
                string res = HttpMethods.HttpPost(getUrl, returnStr);
                //将同步结果写入日志文件
                string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/logs/Aqrxx")))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/logs/Aqrxx"));
                }
                System.IO.File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath("~/logs/Aqrxx/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "：同步数据:" + returnStr + "\r\n\r\n" + "同步信息：" + res + "\r\n\r\n");
                #endregion

                return new { code = 0, info = "获取数据成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取本人创建的未结束补课信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetActivitySupply()
        {
            try
            {
                var bll = new EdActivityBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                string id = dy.data;
                UserEntity user = new UserBLL().GetEntity(userId);
                var entity = bll.GetSupplyById().Where(x => x.CreateUserId == userId && x.IsOver == false && x.ActivityId == id).FirstOrDefault();
                if (entity == null)
                {
                    entity = new EdActivitySupplyEntity();
                }
                return new { code = 0, info = "获取数据成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取补课记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetActivitySupplyList()
        {
            try
            {
                var bll = new EdActivityBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                string id = dy.data;
                UserEntity user = new UserBLL().GetEntity(userId);
                var list = bll.GetSupplyById().Where(x => x.IsOver == true && x.ActivityId == id);
                foreach (EdActivitySupplyEntity a in list)
                {
                    var peoples = bll.GetPeopleById().Where(x => x.SupplyId == a.ID);
                    string username = "";
                    foreach (SupplyPeopleEntity s in peoples)
                    {
                        username += s.UserName + ",";
                    }
                    if (username.Length > 0)
                    {
                        username = username.Substring(0, username.Length - 1);
                    }
                    a.UserName = username;
                }
                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        [HttpPost]
        public object GetNewFileList([FromBody] JObject json)
        {
            try
            {
                FileInfoBLL fileBll = new FileInfoBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                string url = BSFramework.Util.Config.GetValue("AppUrl");
                var list = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description != "二维码");
                foreach (FileInfoEntity f in list)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                    if (!string.IsNullOrEmpty(f.OtherUrl))
                    {
                        if (new Uploader().Query(f.OtherUrl))
                            f.FilePath = f.OtherUrl;
                    }
                }
                List<string> img = new List<string> { ".jpg", ".png", ".gif", ".jpeg" };
                List<string> video = new List<string> { ".mp3", ".wav", "wma", ".msc", ".mp4", ".aac", ".3gp", ".flv", ".rmvb", ".avi" };
                var files1 = list.Where(x => video.Contains(x.FileExtensions));
                var files2 = list.Where(x => x.OtherUrl != "" && x.OtherUrl != null && !video.Contains(x.FileExtensions));
                var files3 = list.Where(x => img.Contains(x.FileExtensions));
                return new { info = "成功", code = 0, data = new { files1 = files1, files2 = files2, files3 = files3 } };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }
        [HttpPost]
        public object ChangePDF()
        {
            try
            {
                FileInfoBLL fileBll = new FileInfoBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                List<string> pdf = new List<string> { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx" };

                var files = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description != "二维码" && pdf.Contains(x.FileExtensions));
                using (var factory = new ChannelFactory<IQueueService>("upload"))
                {
                    var channel = factory.CreateChannel();
                    foreach (FileInfoEntity f in files)
                    {
                        channel.Upload(f.FileId);
                    }

                }
                return new { info = "成功", code = 0, data = new { } };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        [HttpPost]
        public ListBucket<ActivityEvaluateEntity> GetEvaluates(ParamBucket<string> arg)
        {
            var bll = new ActivityBLL();
            var total = 0;
            var data = bll.GetActivityEvaluateEntity(arg.Data, 1000, 1, out total);
            return new ListBucket<ActivityEvaluateEntity>() { Data = data, Total = total };
        }
    }
}
