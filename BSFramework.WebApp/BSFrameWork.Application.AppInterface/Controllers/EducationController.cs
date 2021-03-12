using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Busines.OndutyManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Bst.Fx.Uploading;
using Bst.ServiceContract.MessageQueue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class EducationController : BaseApiController
    {
        DepartmentBLL dtbll = new DepartmentBLL();
        EducationBLL ebll = new EducationBLL();
        FileInfoBLL fileBll = new FileInfoBLL();
        UserBLL ubll = new UserBLL();
        PeopleBLL pbll = new PeopleBLL();
        /// <summary>
        /// 答题
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Answer()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string aId = Guid.NewGuid().ToString();
                string eduId = dy.data.eduId;
                string userId = dy.userId;
                PeopleEntity p = pbll.GetEntity(userId);
                if (p == null)
                {
                    return new { code = 0, info = "非班组人员无法答题！", data = new { } };
                }

                var edu = ebll.GetBaseInfoEntity(eduId);
                edu.ActivityEndDate = DateTime.Now;
                if (edu.EduType == "1")
                {
                    aId = eduId;
                    if (!string.IsNullOrEmpty(edu.Describe))
                    {
                        edu.Describe += "；" + dy.data.answerContent;
                    }
                    else
                    {
                        edu.Describe += dy.data.answerContent;
                    }
                    ebll.SaveEduBaseInfo(eduId, edu);
                }
                else
                {
                    EduAnswerEntity ea = new EduAnswerEntity();
                    ea.AnswerPeople = p.Name;
                    ea.AnswerPeopleId = p.ID;
                    ea.CreateDate = DateTime.Now;
                    ea.CreateUser = p.ID;
                    ea.ID = aId;
                    ea.EduId = eduId;
                    ea.AnswerContent = dy.data.answerContent;
                    ea.Description = dy.data.description;
                    ea.Reason = dy.data.reason;
                    //保存答题信息
                    ebll.SaveAnswer(string.Empty, ea);

                    //20190222 新技术问答，答题后修改答题状态及评价状态
                    if (edu.EduType == "5" || edu.EduType == "7" || edu.EduType == "6")
                    {
                        edu.AppraiseFlow = "0";
                        edu.AnswerFlow = "1";
                        ebll.SaveEduBaseInfo(eduId, edu);
                        if (edu.EduType == "5")
                        {
                            var messagebll = new MessageBLL();
                            messagebll.FinishTodo("技术问答答题", eduId);
                            messagebll.SendMessage("技术问答评价", eduId);
                        }
                        if (edu.EduType == "7")
                        {
                            var messagebll = new MessageBLL();
                            messagebll.FinishTodo("考问讲解", eduId);
                            messagebll.SendMessage("考问讲解评价", eduId);
                        }
                    }

                }
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                string name = "";
                string des = "";
                string type = "";//文件类型
                //保存答题附件 ： 需用Description字段分别，前端页面用以分类显示  关键字+音频/图片
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    name = System.IO.Path.GetFileName(hf.FileName);

                    if (name.Substring(0, 2) == "sg")
                    {
                        des = "事故现象";
                    }
                    else if (name.Substring(0, 2) == "cs")
                    {
                        des = "采取措施";
                    }
                    else if (name.Substring(0, 2) == "wd")
                    {
                        des = "技术问答";
                    }
                    else if (name.Substring(0, 2) == "jk")
                    {
                        des = "技术讲课";
                    }
                    else if (name.Substring(0, 2) == "fx")
                    {
                        des = "原因分析";
                    }
                    else if (name.Substring(0, 2) == "kw")
                    {
                        des = "考问讲解";
                    }
                    type = hf.ContentType;
                    if (type.Contains("image"))
                    {
                        des += "图片";
                    }
                    else if (type.Contains("audio"))
                    {
                        des += "音频";
                    }
                    name = name.Substring(3, name.Length - 3);
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = aId,
                        RecId = aId,
                        FileName = name,
                        FilePath = "~/Resource/AppFile/Education/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0,
                        Description = des
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);


                }
                return new { code = 0, info = "答题成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        /// <summary>
        /// 获取活动详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetDetail([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string eduid = dy.data;
                EduBaseInfoEntity edu = ebll.GetBaseInfoEntity(eduid);
                string url = BSFramework.Util.Config.GetValue("AppUrl");
                var list = fileBll.GetFilesByRecIdNew(edu.ID).Where(x => x.Description == "课件" || x.Description == "1");
                foreach (FileInfoEntity f in list)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                }
                edu.Answers = ebll.GetAnswerList(eduid).ToList();
                foreach (EduAnswerEntity e in edu.Answers)
                {
                    var files = fileBll.GetFilesByRecIdNew(e.ID);
                    foreach (FileInfoEntity f in files)
                    {
                        f.FilePath = f.FilePath.Replace("~/", url);
                    }
                    e.Files = files.Where(x => x.Description.Contains("图片")).ToList();
                    e.Files1 = files.Where(x => x.Description.Contains("音频")).ToList();
                }
                edu.Files = list.ToList();
                return new { info = "成功", code = 0, data = edu };

            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = 1, data = new EduBaseInfoEntity() };
            }

        }

        /// <summary>
        /// 获取通知列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetMessageList([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.data.userId;

                var list = ebll.GetMessageListByUser(userId);
                //foreach (EduMessageEntity m in list) 
                //{
                //    var edu = ebll.GetBaseInfoEntity(m.EduId);
                //    if (edu.Flow == "已完成") 
                //    {

                //    }
                //}

                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 删除短消息
        /// </summary>
        /// <returns></returns>
        [HandlerMonitor(6, "APP接口删除短消息")]
        public object DeleteMessage([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                string mId = dy.data;
                ebll.DelMessage(mId);
                //foreach (EduMessageEntity m in list) 
                //{
                //    var edu = ebll.GetBaseInfoEntity(m.EduId);
                //    if (edu.Flow == "已完成") 
                //    {

                //    }
                //}

                return new { code = 0, info = "删除成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }




        /// <summary>
        /// 获取本部门人员
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetUsers()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                var list = ubll.GetDeptUsers(user.DepartmentId).ToList();
                list.Select(t => new
                {
                    t.DepartmentId,
                    t.RealName
                });
                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取点评
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAppraise()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                var list = ebll.GetCommentTagList(user.DepartmentId);
                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 新增点评
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddAppraise()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                string tag = dy.data;
                UserEntity user = ubll.GetEntity(userId);
                EduCommentTagEntity e = new EduCommentTagEntity();
                e.ID = Guid.NewGuid().ToString();
                e.Tag = tag;
                e.CreateDate = DateTime.Now;
                e.CreateUser = user.RealName;
                e.CreateUserId = user.UserId;
                e.DeptId = user.DepartmentId;
                ebll.PostEduCommentTag(e);
                // ebll.
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }
        [HttpPost]
        public object DelAppraise()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                string id = dy.data;
                ebll.DelEduCommentTag(id);
                // ebll.
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }
        [HttpPost]
        public object DelEducation()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                string id = dy.data;
                ebll.DelEducation(id);
                // ebll.
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }
        /// <summary>
        /// 活动准备/开始
        /// </summary>
        /// <param name="json"></param>
        [HttpPost]
        public object EduStart()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string tientity = JsonConvert.SerializeObject(dy.data.eduBase);

                EduBaseInfoEntity entity = JsonConvert.DeserializeObject<EduBaseInfoEntity>(tientity);
                string type = dy.data.type;
                string ids = dy.data.ids;

                string userId = dy.userId;
                string delIds = dy.data.delIds;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                if (type == "add")
                {
                    entity.ID = Guid.NewGuid().ToString();
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUser = user.UserId;
                    entity.BZId = user.DepartmentId;
                    entity.BZName = dept.FullName;
                    entity.Flow = "2"; //活动状态 准备中
                    int num = entity.AttendPeopleId.Split(',').Count();
                    entity.AttendNumber = num;

                    if (entity.ActivityTime.EndsWith("小时"))
                    {
                        entity.ActivityEndDate = entity.ActivityDate.Value.AddHours(Convert.ToDouble(entity.ActivityTime.Substring(0, entity.ActivityTime.Length - 2)));
                    }
                    if (entity.ActivityTime.EndsWith("分钟"))
                    {
                        entity.ActivityEndDate = entity.ActivityDate.Value.AddMinutes(Convert.ToDouble(entity.ActivityTime.Substring(0, entity.ActivityTime.Length - 2)));
                    }
                }
                else if (type == "edit")
                {
                    int num = entity.AttendPeopleId.Split(',').Count();
                    entity.AttendNumber = num;
                    entity.Flow = "0";//进行中
                }
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;


                var deskPath = BSFramework.Util.Config.GetValue("FilePath");
                // 保存新上传的附件
                for (int i = 0; i < files.Count; i++)
                {
                    var name = files.AllKeys[i].ToString();
                    HttpPostedFile hf = files[i];

                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/Content/Education/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0,
                        Description = "课件"
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education");
                    }
                    hf.SaveAs(deskPath + "\\Content\\Education\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    switch (ext.ToLower())
                    {
                        case ".doc":
                        case ".docx":
                        case ".xls":
                        case ".xlsx":
                        case ".ppt":
                        case ".pptx":
                            if (!System.IO.File.Exists(deskPath + ("\\Content\\EducationPDF\\" + fileId + ".pdf")))
                            {
                                using (var factory = new ChannelFactory<IQueueService>("upload"))
                                {
                                    var channel = factory.CreateChannel();
                                    channel.OfficeToPdf(deskPath + "\\Content\\Education\\" + fileId + ext, deskPath + ("\\Content\\EducationPDF\\" + fileId + ".pdf"));
                                }

                            }
                            break;
                        default:
                            break;
                    }
                }
                //while (dept.Nature != "厂级")
                //{
                //    dept = dtbll.GetEntity(dept.ParentId);
                //}
                //新上传文件，创建自制课件库
                for (int i = 0; i < files.Count; i++)
                {
                    var name = files.AllKeys[i].ToString();
                    HttpPostedFile hf = files[i];
                    //创建课件
                    var obj = new EduInventoryEntity();
                    obj.ID = Guid.NewGuid().ToString();
                    obj.CreateDate = DateTime.Now;
                    obj.CreateUserId = user.UserId;
                    obj.CreateUserName = user.RealName;
                    obj.EduType = "1";
                    obj.BZID = user.DepartmentId;
                    obj.DeptCode = dept.EnCode;
                    obj.UseDeptCode = dept.EnCode;
                    obj.UseDeptId = dept.DepartmentId;
                    obj.UseDeptName = dept.FullName;
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = obj.ID,
                        RecId = obj.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/Content/Education/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    obj.Name = name;
                    ebll.SaveForm(obj.ID, obj);
                    switch (ext.ToLower())
                    {
                        case ".doc":
                        case ".docx":
                        case ".xls":
                        case ".xlsx":
                        case ".ppt":
                        case ".pptx":
                            if (!System.IO.File.Exists(deskPath + ("\\Content\\EducationPDF\\" + fileId + ".pdf")))
                            {
                                using (var factory = new ChannelFactory<IQueueService>("upload"))
                                {
                                    var channel = factory.CreateChannel();
                                    channel.OfficeToPdf(deskPath + "\\Content\\Education\\" + fileId + ext, deskPath + ("\\Content\\EducationPDF\\" + fileId + ".pdf"));
                                }

                            }
                            break;
                        default:
                            break;
                    }
                }
                //选择的课件，保存至教育培训基本信息
                if (entity.Files != null)
                {
                    foreach (var item in entity.Files)
                    {
                        if (string.IsNullOrEmpty(item.FileId))
                        {
                            item.RecId = entity.ID;
                            item.FileId = Guid.NewGuid().ToString();
                            fileBll.SaveForm(item);
                        }
                        else
                        {
                            var file = fileBll.GetEntity(item.FileId);
                            if (file != null)
                            {
                                var file1 = file;
                                file1.RecId = entity.ID;
                                file1.Description = "课件";
                                file1.FileId = Guid.NewGuid().ToString();
                                //保存附件信息
                                fileBll.SaveForm(file1);
                            }
                        }
                    }
                }


                ebll.SaveEduBaseInfo(entity.ID, entity);



                //选择的课件，保存至教育培训基本信息
                foreach (string id in ids.Split(','))
                {
                    if (!string.IsNullOrEmpty(id))
                    {

                        var file = fileBll.GetFilesByRecIdNew(id).FirstOrDefault();
                        if (file != null)
                        {
                            var file1 = file;
                            file1.RecId = entity.ID;
                            file1.Description = "课件";
                            file1.FileId = Guid.NewGuid().ToString();
                            //保存附件信息
                            fileBll.SaveForm(file1);
                        }
                    }
                }
                //删除原有附件
                foreach (string id in delIds.Split(','))
                {
                    if (!string.IsNullOrEmpty(id))
                    {

                        var file = fileBll.GetEntity(id); //fileBll.GetFilesByRecIdNew(id).FirstOrDefault();
                        if (file != null)
                        {
                            fileBll.Delete(file.FileId);
                        }
                    }
                }


                string url = BSFramework.Util.Config.GetValue("AppUrl");
                var list = fileBll.GetFilesByRecIdNew(entity.ID).Where(x => x.Description == "课件" || x.Description == "1");
                foreach (FileInfoEntity f in list)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                    //if (!string.IsNullOrEmpty(f.OtherUrl))
                    //{
                    //    if (new Uploader().Query(f.OtherUrl))
                    //        f.FilePath = f.OtherUrl;
                    //}
                }
                return new { code = 0, info = "成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 活动准备/开始
        /// </summary>
        /// <param name="json"></param>
        [HttpPost]
        public object EduFinish()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string tientity = JsonConvert.SerializeObject(dy.data.eduBase);

                EduBaseInfoEntity entity = JsonConvert.DeserializeObject<EduBaseInfoEntity>(tientity);
                string type = dy.data.type;
                string ids = dy.data.ids;

                string userId = dy.userId;
                string delIds = dy.data.delIds;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);


                entity.ID = Guid.NewGuid().ToString();
                entity.CreateDate = DateTime.Now;
                entity.CreateUser = user.UserId;
                entity.BZId = user.DepartmentId;
                entity.BZName = dept.FullName;
                entity.Flow = "0";//进行中
                int num = entity.AttendPeopleId.Split(',').Count();
                entity.AttendNumber = num;

                if (entity.ActivityTime.EndsWith("小时"))
                {
                    entity.ActivityEndDate = entity.ActivityDate.Value.AddHours(Convert.ToDouble(entity.ActivityTime.Substring(0, entity.ActivityTime.Length - 2)));
                }
                if (entity.ActivityTime.EndsWith("分钟"))
                {
                    entity.ActivityEndDate = entity.ActivityDate.Value.AddMinutes(Convert.ToDouble(entity.ActivityTime.Substring(0, entity.ActivityTime.Length - 2)));
                }

                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;


                // 保存新上传的附件
                for (int i = 0; i < files.Count; i++)
                {
                    var name = files.AllKeys[i].ToString();
                    HttpPostedFile hf = files[i];

                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/Content/Education/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0,
                        Description = "课件"
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                //while (dept.Nature != "厂级")
                //{
                //    dept = dtbll.GetEntity(dept.ParentId);
                //}
                //新上传文件，创建自制课件库
                for (int i = 0; i < files.Count; i++)
                {
                    var name = files.AllKeys[i].ToString();
                    HttpPostedFile hf = files[i];
                    //创建课件
                    var obj = new EduInventoryEntity();
                    obj.ID = Guid.NewGuid().ToString();
                    obj.CreateDate = DateTime.Now;
                    obj.CreateUserId = user.UserId;
                    obj.CreateUserName = user.RealName;
                    obj.EduType = "1";
                    obj.BZID = user.DepartmentId;
                    obj.DeptCode = dept.EnCode;
                    obj.UseDeptCode = dept.EnCode;
                    obj.UseDeptId = dept.DepartmentId;
                    obj.UseDeptName = dept.FullName;
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = obj.ID,
                        RecId = obj.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/Content/Education/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\Content\\Education\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    obj.Name = name;
                    ebll.SaveForm(obj.ID, obj);
                }
                //选择的课件，保存至教育培训基本信息
                if (entity.Files != null)
                {
                    foreach (var item in entity.Files)
                    {
                        if (string.IsNullOrEmpty(item.FileId))
                        {
                            item.RecId = entity.ID;
                            item.Description = "课件";
                            item.FileId = Guid.NewGuid().ToString();
                            fileBll.SaveForm(item);
                        }
                        else
                        {
                            var file = fileBll.GetEntity(item.FileId);
                            if (file != null)
                            {
                                var file1 = file;
                                file1.RecId = entity.ID;
                                file1.Description = "课件";
                                file1.FileId = Guid.NewGuid().ToString();
                                //保存附件信息
                                fileBll.SaveForm(file1);
                            }
                        }
                    }
                }

                TimeSpan ts = DateTime.Now - entity.ActivityDate.Value;
                entity.LearnTime += Convert.ToDecimal(Math.Round(ts.TotalHours, 2));
                ebll.SaveEduBaseInfo(entity.ID, entity);



                //选择的课件，保存至教育培训基本信息
                foreach (string id in ids.Split(','))
                {
                    if (!string.IsNullOrEmpty(id))
                    {

                        var file = fileBll.GetFilesByRecIdNew(id).FirstOrDefault();
                        if (file != null)
                        {
                            var file1 = file;
                            file1.RecId = entity.ID;
                            file1.Description = "课件";
                            file1.FileId = Guid.NewGuid().ToString();
                            //保存附件信息
                            fileBll.SaveForm(file1);
                        }
                    }
                }
                //删除原有附件
                foreach (string id in delIds.Split(','))
                {
                    if (!string.IsNullOrEmpty(id))
                    {

                        var file = fileBll.GetEntity(id); //fileBll.GetFilesByRecIdNew(id).FirstOrDefault();
                        if (file != null)
                        {
                            fileBll.Delete(file.FileId);
                        }
                    }
                }


                string url = BSFramework.Util.Config.GetValue("AppUrl");
                var list = fileBll.GetFilesByRecIdNew(entity.ID);

                foreach (FileInfoEntity f in list)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);

                    //if (!string.IsNullOrEmpty(f.OtherUrl))
                    //{
                    //    if (new Uploader().Query(f.OtherUrl))
                    //        f.FilePath = f.OtherUrl;
                    //}
                }
                entity.Files = list;
                return new { code = 0, info = "成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 活动结束
        /// </summary>
        /// <param name="json"></param>
        [HttpPost]
        public object EduEnd()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string tientity = JsonConvert.SerializeObject(dy.data.eduBase);

                EduBaseInfoEntity entity = JsonConvert.DeserializeObject<EduBaseInfoEntity>(tientity);
                string type = dy.data.type;
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                entity.Flow = "1";
                entity.ActivityEndDate = DateTime.Now;
                int num = entity.AttendPeopleId.Split(',').Count();
                entity.AttendNumber = num;
                TimeSpan ts = DateTime.Now - entity.ActivityDate.Value;
                entity.LearnTime += Convert.ToDecimal(Math.Round(ts.TotalHours, 2));
                ebll.SaveEduBaseInfo(entity.ID, entity);

                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                string des = ""; string name = "";
                //保存答题附件 ： 需用Description字段分别，前端页面用以分类显示  关键字+音频/图片
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    name = System.IO.Path.GetFileName(hf.FileName);

                    type = hf.ContentType;
                    if (type.Contains("image"))
                    {
                        des += "现场照片";
                    }
                    else if (type.Contains("video"))
                    {
                        des += "现场视频";
                    }
                    name = name.Substring(3, name.Length - 3);
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = name,
                        FilePath = "~/Resource/AppFile/Education/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0,
                        Description = des
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }

                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 答题完毕，保存答题记录
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AnswerNew()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string tientity = JsonConvert.SerializeObject(dy.data.eduAnswer);
                EduAnswerEntity ea = JsonConvert.DeserializeObject<EduAnswerEntity>(tientity);
                string eduId = dy.data.eduId;
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);

                ea.ID = Guid.NewGuid().ToString();
                ea.EduId = eduId;
                ea.CreateUser = user.UserId;
                ea.CreateDate = DateTime.Now;
                //保存答题信息
                ebll.SaveAnswer(string.Empty, ea);

                //20190222 新技术问答，答题后修改答题状态及评价状态
                var edu = ebll.GetBaseInfoEntity(eduId);
                edu.ActivityEndDate = DateTime.Now;
                if (edu.EduType == "5" || edu.EduType == "6" || edu.EduType == "7")
                {
                    edu.AppraiseFlow = "0";
                    edu.AnswerFlow = "1";
                    ebll.SaveEduBaseInfo(eduId, edu);
                }
                if (edu.EduType == "1")
                {
                    edu.NewAppraiseContent = ea.AppraiseContent;
                    edu.AppraiseContent = null;
                    edu.Describe = ea.Description;
                    ebll.SaveEduBaseInfo(eduId, edu);
                }
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                string name = "";
                string des = "";
                string type = "";//文件类型
                //保存答题附件 ： 需用Description字段分别，前端页面用以分类显示  关键字+音频/图片
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    name = System.IO.Path.GetFileName(hf.FileName);
                    des = files.AllKeys[i].ToString();

                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = edu.EduType == "1" ? edu.ID : ea.ID,
                        RecId = edu.EduType == "1" ? edu.ID : ea.ID,
                        FileName = name,
                        FilePath = "~/Resource/AppFile/Education/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0,
                        Description = des
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                return new { code = 0, info = "答题成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetList()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                var json = HttpContext.Current.Request["json"];
                bool allowPaging = dy.allowPaging;
                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                string from = dy.data.from;
                string to = dy.data.to;
                string flow = dy.data.flow;
                string type = dy.data.eduType;
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                int total = 0;
                //var jobject = JObject.Parse(json);
                string deptid = user.DepartmentId;
                //if (json.Contains("deptId"))
                //{
                //    var getDeptId = jobject.SelectToken("data.deptId").Value<string>();
                //    if (!string.IsNullOrEmpty(getDeptId))
                //    {
                //        deptid = getDeptId;
                //    }

                //}
                //var list = ebll.GetAllList().Where(x => x.BZId == deptid).Where(x => x.Category != "安全日活动").ToList();
                //if (!string.IsNullOrEmpty(type) && type != "0")
                //{
                //    var types = type.Split(',');
                //    list = list.Where(x => types.Contains(x.EduType)).ToList();
                //}
                //if (!string.IsNullOrEmpty(from))
                //{
                //    DateTime f = DateTime.Parse(from);
                //    list = list.Where(x => x.ActivityDate >= f).ToList();
                //}
                //if (!string.IsNullOrEmpty(to))
                //{
                //    DateTime t = DateTime.Parse(to);
                //    t = t.AddDays(1);
                //    list = list.Where(x => x.ActivityDate < t).ToList();
                //}
                Pagination pagination = new Pagination();
                if (allowPaging)
                {
                    pagination.page = Convert.ToInt32(pageIndex);
                    pagination.rows = Convert.ToInt32(pageSize);
                }
                else
                {
                    pagination.page = 1;
                    pagination.rows = 1000;
                }

                //List<EduBaseInfoEntity> list = ebll.GetPageList(pagination, JsonConvert.SerializeObject(dy.data), userId);
                List<EduBaseInfoEntity> list = ebll.GetPageListEdAndAc(pagination, JsonConvert.SerializeObject(dy.data), userId);

                if (!string.IsNullOrEmpty(flow))  //活动状态
                {
                    if (flow == "1")
                    {
                        //list = list.Where(x => x.Flow == flow).ToList();
                    }
                    else
                    {
                        //list = list.Where(x => x.Flow != "1").ToList();
                        OndutyMeetBLL ondutybll = new OndutyMeetBLL();
                        foreach (var item in list)
                        {
                            var Ondy = ondutybll.GetList(item.ID);
                            item.hasSign = Ondy.Count > 0;
                        }


                    }
                }
                var diskPath = BSFramework.Util.Config.GetValue("FilePath");
                var deptBll = new DepartmentBLL();
                foreach (EduBaseInfoEntity e in list)
                {
                    var files = fileBll.GetFilesByRecIdNew(e.ID);
                    foreach (var f in files)
                    {
                        switch (Path.GetExtension(f.FileName).ToLower())
                        {
                            case ".doc":
                            case ".docx":
                            case ".xls":
                            case ".xlsx":
                            case ".ppt":
                            case ".pptx":
                                if (System.IO.File.Exists(diskPath + ("\\Content\\EducationPDF\\" + f.FileId + ".pdf")))
                                {
                                    f.ViewUrl = url + "/Pdf/ViewPDFPage?httpUrl=" + HttpUtility.UrlEncode(url + ("/Resource/Content/EducationPDF/" + f.FileId + ".pdf"));
                                    f.CanView = true;
                                }
                                else
                                {
                                    using (var factory = new ChannelFactory<IQueueService>("upload"))
                                    {
                                        var channel = factory.CreateChannel();
                                        channel.OfficeToPdf(diskPath + f.FilePath.Replace("~/Resource", string.Empty), diskPath + ("/Content/EducationPDF/" + f.FileId + ".pdf"));
                                    }

                                }
                                break;
                            default:
                                break;
                        }
                        f.FilePath = f.FilePath.Replace("~/", url);
                    }
                    e.Files = files;
                    files = files.Where(x => x.Description == "照片").ToList();
                    if (files.FirstOrDefault() != null)
                    {
                        e.BgImage = files[0].FilePath;
                    }
                    if (string.IsNullOrEmpty(e.BZName))
                    {
                        var dept = deptBll.GetEntity(e.BZId);
                        if (dept != null)
                        {
                            e.BZName = dept.FullName;
                        }
                    }


                }

                #region 教育培训

                // var bll = new EdActivityBLL();
                //var deptBll = new DepartmentBLL();
                //var start = new DateTime(1999, 1, 1);
                //var end = DateTime.Now;
                //string category = "";
                //if (!string.IsNullOrEmpty(from))
                //{
                //    start = DateTime.Parse(from);
                //}
                //if (!string.IsNullOrEmpty(to))
                //{
                //    end = DateTime.Parse(to);
                //    end = end.AddDays(1).AddMinutes(-1);
                //}
                //var data = new List<EdActivityEntity>();

                //if (!string.IsNullOrEmpty(type) && type != "0")
                //{
                //    if (type.Contains("安全学习日"))
                //    {
                //        category = "安全学习日";
                //        data = bll.GetActivities2(userId, start, end, category, deptid, true);

                //    }

                //}
                //foreach (var item in data)
                //{
                //    //var evaluatesList = bll.GetEntityList().Where(x => x.Activityid == item.ActivityId).ToList();
                //    //var myself = evaluatesList.Select(x => x.EvaluateUser).ToList();
                //    //if (myself.Contains(user.RealName))
                //    //{
                //    //    item.Evaluates = evaluatesList;
                //    //}
                //    //else
                //    //{
                //    //    item.Evaluates = new List<ActivityEvaluateEntity>();
                //    //}
                //    var dept = deptBll.GetEntity(item.GroupId);
                //    if (dept != null)
                //    {
                //        item.GroupName = dept.FullName;
                //    }
                //}

                //foreach (var item in data)
                //{
                //    var ed = new EduBaseInfoEntity();
                //    ed.ID = item.ActivityId;
                //    ed.CreateDate = item.CreateDate;
                //    ed.Theme = item.Subject;
                //    ed.ActivityDate = item.StartTime;
                //    ed.ActivityEndDate = item.EndTime;
                //    foreach (var f in item.Files)
                //    {
                //        f.FilePath = f.FilePath.Replace("~/", url);
                //    }
                //    ed.EduType = item.ActivityType;
                //    ed.Files = item.Files;
                //    var files = item.Files.Where(x => x.Description == "照片").ToList();
                //    if (files.FirstOrDefault() != null)
                //    {

                //        ed.BgImage = files[0].FilePath;
                //    }
                //    list.Add(ed);
                //}
                //list = list.OrderByDescending(x => x.CreateDate).ToList();
                #endregion

                total = pagination.records;

                list = list.OrderByDescending(x => x.ActivityDate).Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();

                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetListTotal(ParamBucket<IndexCountries> args)
        {
            try
            {
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //var url = BSFramework.Util.Config.GetValue("AppUrl");
                //string userId = dy.userId;
                //string from = dy.data.from;
                //string to = dy.data.to;
                var nowTime = DateTime.Now;
                var startTime = new DateTime(nowTime.Year, nowTime.Month, 1);
                var endTime = new DateTime(nowTime.Year, nowTime.Month, 1).AddMonths(1).AddMinutes(-1);
                if (args.Data.startTime.HasValue)
                {
                    startTime = args.Data.startTime.Value;
                    endTime = args.Data.endTime.Value;
                }
                int total = 0;
                var list = ebll.GetAllList().Where(x => x.BZId == args.Data.DeptId);

                list = list.Where(x => x.Flow == "1").ToList();
                list = list.Where(x => x.ActivityDate >= startTime).ToList();
                list = list.Where(x => x.ActivityDate < endTime).ToList();
                total = list.Count();

                var data = list.GroupBy(x => x.EduType, (x, y) => new
                {
                    educationType =
                         x == "1" ? "技术讲课" : x == "2" ? "技术问答" : x == "3" ? "事故预想" : x == "4" ? "反事故演习" : x == "5" ? "技术问答" : x == "6" ? "事故预想" : "考问讲解",//考问讲解 分散式7 集中式8都算作考问讲解
                    sum = y.Count()
                }).ToList();
                var ck = data.FirstOrDefault(x => x.educationType == "技术讲课");
                if (ck == null)
                {
                    data.Add(new { educationType = "技术讲课", sum = 0 });
                }
                ck = data.FirstOrDefault(x => x.educationType == "技术问答");
                if (ck == null)
                {
                    data.Add(new { educationType = "技术问答", sum = 0 });
                }
                ck = data.FirstOrDefault(x => x.educationType == "事故预想");
                if (ck == null)
                {
                    data.Add(new { educationType = "事故预想", sum = 0 });
                }
                ck = data.FirstOrDefault(x => x.educationType == "反事故演习");
                if (ck == null)
                {
                    data.Add(new { educationType = "反事故演习", sum = 0 });
                }
                ck = data.FirstOrDefault(x => x.educationType == "考问讲解");
                if (ck == null)
                {
                    data.Add(new { educationType = "考问讲解", sum = 0 });
                }
                int AQcount = new EdActivityBLL().GetMonthCount(args.Data.DeptId);//安全学习日活动次数
                data.Add(new { educationType = "安全学习日", sum = AQcount });
                data = data.GroupBy(x => x.educationType, (x, y) => new { educationType = x, sum = y.Sum(g => g.sum) }).ToList();

                return new { code = 0, info = "获取数据成功", count = total, data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetDetailNew()
        {
            var fileInfoBLL = new FileInfoBLL();
            try
            {
                ActivityBLL actBll = new ActivityBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string eduid = dy.data;
                EduBaseInfoEntity entity = ebll.GetBaseInfoEntity(eduid);
                var fileList = new FileInfoBLL().List(entity.ID, null);
                if (!string.IsNullOrEmpty(entity.MeetingId))
                {
                    var meetingFiles = fileInfoBLL.List(entity.MeetingId, new string[] { "视频" });
                    if (meetingFiles != null && fileList != null)
                        fileList.AddRange(meetingFiles);
                }

                var url = BSFramework.Util.Config.GetValue("AppUrl");
                foreach (FileInfoEntity f in fileList)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                    if (!string.IsNullOrEmpty(f.OtherUrl))
                    {
                        if (new Uploader().Query(f.OtherUrl))
                            f.FilePath = f.OtherUrl;
                    }
                }
                entity.Files1 = fileList.Where(x => x.Description == "照片").ToList();
                entity.Files = fileList.Where(x => x.Description == "视频").ToList();
                entity.Files2 = fileList.Where(x => x.Description == "课件" || x.Description == "1").ToList();

                entity.Answers = ebll.GetAnswerList(entity.ID).ToList();
                foreach (EduAnswerEntity e in entity.Answers)
                {
                    var files = fileBll.GetFilesByRecIdNew(entity.EduType == "1" ? e.EduId : e.ID).Where(x => x.Description != null);
                    foreach (FileInfoEntity f in files)
                    {
                        f.FilePath = f.FilePath.Replace("~/", url);
                    }
                    e.Files = files.Where(x => x.Description.Contains("图片")).ToList();
                    e.Files1 = files.Where(x => x.Description.Contains("音频")).ToList();
                }
                entity.Appraises = actBll.GetEntityList().Where(x => x.Activityid == entity.ID).ToList();
                return new { code = 0, info = "获取数据成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = 1, data = new { } };
            }

        }

        /// <summary>
        /// 查看答题详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetAnswerDetail()
        {
            try
            {
                ActivityBLL actBll = new ActivityBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string eduid = dy.data;
                EduAnswerEntity entity = ebll.GetAnswerEntity(eduid);
                EduBaseInfoEntity baseentity = ebll.GetBaseInfoEntity(entity.EduId);
                var fileslist = fileBll.GetFilesByRecIdNew(entity.ID);
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                if (baseentity.EduType == "2")
                {
                    var files = fileslist.Where(x => x.Description == "技术问答图片");
                    files.Select(x => x.FilePath = x.FilePath.Replace("~/", url));
                    entity.Files = files.ToList();
                }
                else if (baseentity.EduType == "3")
                {
                    var files = fileslist.Where(x => x.Description == "事故现象图片");
                    var files1 = fileslist.Where(x => x.Description == "采取措施图片");
                    files.Select(x => x.FilePath = x.FilePath.Replace("~/", url));
                    files1.Select(x => x.FilePath = x.FilePath.Replace("~/", url));
                    entity.Files = files.ToList();
                    entity.Files1 = files1.ToList();
                }
                else if (baseentity.EduType == "1")
                {
                    var files = fileslist.Where(x => x.Description == "技术讲课图片");
                    files.Select(x => x.FilePath = x.FilePath.Replace("~/", url));
                    entity.Files = files.ToList();
                }
                return new { code = 0, info = "获取数据成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = 1, data = new { } };
            }

        }
        /// <summary>
        /// 答题对标
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object CheckAnswer()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                return new { code = 0, info = "答题成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        /// <summary>
        /// 课件查询
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetEduInventorys()
        {
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
            var url = BSFramework.Util.Config.GetValue("AppUrl");
            string eduType = dy.data.eduType;
            long pageIndex = dy.pageIndex;//当前索引页
            long pageSize = dy.pageSize;//每页记录数
            string name = dy.data.name;
            string userId = dy.userId;
            UserEntity user = new UserBLL().GetEntity(userId);
            var total = 0;
            var list = ebll.GetInventoryList(user.DepartmentCode, null, eduType, name, (int)pageSize, (int)pageIndex, out total);
            //var list = ebll.GetInventoryList("").Where(x => x.UseDeptCode == user.DepartmentCode || x.UseDeptCode == "0").ToList();
            //if (eduType == "2" || eduType == "5")
            //{
            //    list = list.Where(x => x.Question != null).ToList();
            //    list = list.Where(x => (x.EduType == "2" || x.EduType == "5") && x.Question.Contains(name)).ToList();
            //}
            //else if (eduType == "3" || eduType == "6")
            //{
            //    list = list.Where(x => x.Name != null).ToList();
            //    list = list.Where(x => (x.EduType == "3" || x.EduType == "6") && x.Name.Contains(name)).ToList();
            //}
            //else if (eduType == "1")
            //{
            //    list = list.Where(x => x.Name != null).ToList();
            //    list = list.Where(x => x.EduType == "1" && x.Name.Contains(name)).ToList();
            //}
            //else
            //{
            //    list = list.Where(x => x.Name != null).ToList();
            //    list = list.Where(x => x.EduType == eduType && x.Name.Contains(name)).ToList();
            //}
            //list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
            var diskPath = ConfigurationManager.AppSettings["FilePath"].ToString();
            foreach (EduInventoryEntity e in list)
            {
                if (e.EduType == "1" || e.EduType == "3" || e.EduType == "6")
                {
                    e.Question = e.Name;
                }
                e.Owner = "n";
                if (e.BZID == user.DepartmentId)
                {
                    e.Owner = "y";
                }
                e.Files = fileBll.GetFilesByRecIdNew(e.ID).Where(x => x.Description != "封面").ToList();
                foreach (FileInfoEntity f in e.Files)
                {
                    switch (Path.GetExtension(f.FileName))
                    {
                        case ".doc":
                        case ".docx":
                        case ".xls":
                        case ".xlsx":
                        case ".ppt":
                        case ".pptx":
                            if (System.IO.File.Exists(diskPath + ("~/Resource/ActivityPDF/" + f.FileId + ".pdf").Replace("~/", string.Empty)))
                            {
                                f.ViewUrl = url + "/Pdf/ViewPDFPage?httpUrl=" + HttpUtility.UrlEncode(url + ("~/Resource/ActivityPDF/" + f.FileId + ".pdf").Replace("~/", string.Empty));
                                f.CanView = true;
                            }
                            else
                            {
                                using (var factory = new ChannelFactory<IQueueService>("upload"))
                                {
                                    var channel = factory.CreateChannel();
                                    channel.OfficeToPdf(diskPath + f.FilePath.Replace("~/Resource", string.Empty), diskPath + ("/ActivityPDF/" + f.FileId + ".pdf"));
                                }

                            }
                            break;
                        default:

                            break;
                    }
                    f.FilePath = f.FilePath.Replace("~/", url);
                }
                var fm = fileBll.GetFilesByRecIdNew(e.ID).Where(x => x.Description == "封面").FirstOrDefault();
                if (fm != null)
                {
                    switch (Path.GetExtension(fm.FileName))
                    {
                        case ".doc":
                        case ".docx":
                        case ".xls":
                        case ".xlsx":
                        case ".ppt":
                        case ".pptx":
                            if (System.IO.File.Exists(diskPath + ("~/Resource/ActivityPDF/" + fm.FileId + ".pdf").Replace("~/", string.Empty)))
                            {
                                fm.ViewUrl = url + "/Pdf/ViewPDFPage?httpUrl=" + HttpUtility.UrlEncode(url + ("~/Resource/ActivityPDF/" + fm.FileId + ".pdf").Replace("~/", string.Empty));
                                fm.CanView = true;
                            }
                            else
                            {
                                using (var factory = new ChannelFactory<IQueueService>("upload"))
                                {
                                    var channel = factory.CreateChannel();
                                    channel.OfficeToPdf(diskPath + fm.FilePath.Replace("~/Resource", string.Empty), diskPath + ("/ActivityPDF/" + fm.FileId + ".pdf"));
                                }
                            }
                            break;
                        default:

                            break;
                    }
                    fm.FilePath = fm.FilePath.Replace("~/", url);
                    e.fm = fm.FilePath;
                }
            }

            return new { code = 0, count = total, info = "获取数据成功", data = list };

        }
        /// <summary>
        /// 新增课件
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddEduInventory()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                string name = "";
                string des = "";
                FileInfoEntity fm = null;

                var dept = dtbll.GetEntity(user.DepartmentId);
                //while (dept.Nature != "厂级") 
                //{
                //    dept = dtbll.GetEntity(dept.ParentId);
                //}
                //遍历上传的附件，判断是否有封面，创建封面信息  fm
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    name = System.IO.Path.GetFileName(hf.FileName);
                    des = files.AllKeys[i].ToString();
                    if (des == "封面")
                    {
                        string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                        string fileId = Guid.NewGuid().ToString();//上传后文件名
                        fm = new FileInfoEntity
                        {
                            FileId = fileId,
                            FolderId = "",
                            RecId = "",
                            FileName = name,
                            FilePath = "~/Resource/AppFile/Education/" + fileId + ext,
                            FileType = System.IO.Path.GetExtension(hf.FileName),
                            FileExtensions = ext,
                            FileSize = hf.ContentLength.ToString(),
                            DeleteMark = 0,
                            Description = des
                        };
                        //上传附件到服务器
                        if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education"))
                        {
                            System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education");
                        }
                        hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education\\" + fileId + ext);
                    }
                }
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    name = System.IO.Path.GetFileName(hf.FileName);
                    des = files.AllKeys[i].ToString();
                    if (des != "封面")
                    {
                        var obj = new EduInventoryEntity();
                        obj.ID = Guid.NewGuid().ToString();
                        obj.CreateDate = DateTime.Now;
                        obj.CreateUserId = user.UserId;
                        obj.CreateUserName = user.RealName;
                        obj.EduType = "1";
                        obj.ModifyDate = DateTime.Now;
                        obj.Name = name;
                        obj.BZID = user.DepartmentId;
                        obj.DeptCode = dept.EnCode;
                        obj.UseDeptCode = dept.EnCode;
                        obj.UseDeptId = dept.DepartmentId;
                        obj.UseDeptName = dept.FullName;
                        string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                        string fileId = Guid.NewGuid().ToString();//上传后文件名
                        fi = new FileInfoEntity
                        {
                            FileId = fileId,
                            FolderId = obj.ID,
                            RecId = obj.ID,
                            FileName = name,
                            FilePath = "~/Resource/AppFile/Education/" + fileId + ext,
                            FileType = System.IO.Path.GetExtension(hf.FileName),
                            FileExtensions = ext,
                            FileSize = hf.ContentLength.ToString(),
                            DeleteMark = 0,
                            Description = des
                        };

                        //上传附件到服务器
                        if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education"))
                        {
                            System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education");
                        }
                        hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education\\" + fileId + ext);
                        //保存附件信息
                        fileBll.SaveForm(fi);

                        //保存封面信息
                        if (fm != null)
                        {
                            fm.FileId = Guid.NewGuid().ToString();
                            fm.RecId = obj.ID;

                            fileBll.SaveForm(fm);
                        }

                        //保存附件信息

                        ebll.SaveForm(obj.ID, obj);
                    }
                }


                return new { code = 0, info = "操作成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 修改课件
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object EditEduInventory()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                string id = dy.data.id;
                string ename = dy.data.name;
                UserEntity user = new UserBLL().GetEntity(userId);
                var obj = ebll.GetEntity(id);
                obj.Name = ename;
                obj.ModifyDate = DateTime.Now;
                obj.ModifyUserId = user.UserId;
                obj.ModifyUserName = user.RealName;


                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                string name = "";
                string des = "";
                //保存答题附件 ： 需用Description字段分别，前端页面用以分类显示  关键字+音频/图片
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    name = System.IO.Path.GetFileName(hf.FileName);
                    des = files.AllKeys[i].ToString();
                    if (des == "封面")
                    {
                        obj.Files = fileBll.GetFilesByRecIdNew(obj.ID).Where(x => x.Description == "封面").ToList();
                        foreach (FileInfoEntity f in obj.Files)
                        {
                            fileBll.Delete(f.FileId);
                        }
                    }
                    else
                    {
                        obj.Files = fileBll.GetFilesByRecIdNew(obj.ID).Where(x => x.Description != "封面").ToList();
                        foreach (FileInfoEntity f in obj.Files)
                        {
                            fileBll.Delete(f.FileId);
                        }
                    }
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = obj.ID,
                        RecId = obj.ID,
                        FileName = name,
                        FilePath = "~/Resource/AppFile/Education/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0,
                        Description = des
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Education\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                ebll.SaveForm(obj.ID, obj);
                return new { code = 0, info = "操作成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 删除课件
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object DelEduInventory()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                ebll.RemoveForm(id);
                return new { code = 0, info = "操作成功", data = new { } };
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
        public object GetExplainFile()
        {
            try
            {
                FileInfoBLL fileBll = new FileInfoBLL();

                string url = BSFramework.Util.Config.GetValue("AppUrl");
                var list1 = fileBll.GetFilesByDescription("操作介绍视频");
                var list2 = fileBll.GetFilesByDescription("操作说明书");
                foreach (FileInfoEntity f in list1)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                    //if (!string.IsNullOrEmpty(f.OtherUrl))
                    //{
                    //    if (new Uploader().Query(f.OtherUrl))
                    //        f.FilePath = f.OtherUrl;
                    //}
                }
                var urlPDF = Config.GetValue("pdfview");
                foreach (FileInfoEntity f in list2)
                {
                    // f.FilePath = f.FilePath.Replace("~/", url);
                    f.FilePath = urlPDF + f.FileId;
                    //if (!string.IsNullOrEmpty(f.OtherUrl))
                    //{
                    //    if (new Uploader().Query(f.OtherUrl))
                    //        f.FilePath = f.OtherUrl;
                    //}
                }
                return new { info = "成功", code = 0, data = new { files1 = list1, files2 = list2 } };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }



        /// <summary>
        /// 新增技术问答
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object New()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                EduBaseInfoEntity e = new EduBaseInfoEntity();
                string ids = dy.data.RegisterPeopleId;
                string names = dy.data.RegisterPeople;
                var arrnames = names.Split(',');
                var arrids = ids.Split(',');
                for (int i = 0; i < arrids.Length; i++)
                {
                    e.ID = Guid.NewGuid().ToString();
                    e.CreateDate = DateTime.Now;
                    e.CreateUser = userId;
                    e.Teacher = dy.data.Teacher;
                    e.TeacherId = dy.data.TeacherId;
                    e.RegisterPeople = arrnames[i];
                    e.RegisterPeopleId = arrids[i];
                    e.BZId = dept.DepartmentId;
                    e.BZName = dept.FullName;
                    e.ActivityDate = Convert.ToDateTime(dy.data.ActivityDate);
                    e.ActivityEndDate = Convert.ToDateTime(dy.data.ActivityEndDate);
                    e.InventoryId = dy.data.InventoryId;
                    e.Theme = dy.data.Theme;
                    e.EduType = "5";
                    e.AnswerFlow = "0";
                    ebll.SaveEduBaseInfo(e.ID, e);
                    var messagebll = new MessageBLL();
                    messagebll.SendMessage("技术问答答题", e.ID);
                }
                return new { code = 0, info = "新增成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        public object Newkwjj()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                EduBaseInfoEntity e = new EduBaseInfoEntity();
                string ids = dy.data.RegisterPeopleId;
                string names = dy.data.RegisterPeople;
                var arrnames = names.Split(',');
                var arrids = ids.Split(',');
                for (int i = 0; i < arrids.Length; i++)
                {
                    e.ID = Guid.NewGuid().ToString();
                    e.CreateDate = DateTime.Now;
                    e.CreateUser = userId;
                    e.Teacher = dy.data.Teacher;
                    e.TeacherId = dy.data.TeacherId;
                    e.RegisterPeople = arrnames[i];
                    e.RegisterPeopleId = arrids[i];
                    e.BZId = dept.DepartmentId;
                    e.BZName = dept.FullName;
                    e.ActivityDate = Convert.ToDateTime(dy.data.ActivityDate);
                    e.ActivityEndDate = Convert.ToDateTime(dy.data.ActivityEndDate);
                    e.InventoryId = dy.data.InventoryId;
                    e.Theme = dy.data.Theme;
                    e.EduType = "7";
                    e.AnswerFlow = "0";
                    ebll.SaveEduBaseInfo(e.ID, e);
                    var messagebll = new MessageBLL();
                    messagebll.SendMessage("考问讲解", e.ID);
                }
                return new { code = 0, info = "新增成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 新增事故预想（新）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object Newsgyx()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                EduBaseInfoEntity e = new EduBaseInfoEntity();
                string ids = dy.data.RegisterPeopleId;
                string names = dy.data.RegisterPeople;
                var arrnames = names.Split(',');
                var arrids = ids.Split(',');
                for (int i = 0; i < arrids.Length; i++)
                {
                    e.ID = Guid.NewGuid().ToString();
                    e.CreateDate = DateTime.Now;
                    e.CreateUser = userId;
                    e.Teacher = dy.data.Teacher;
                    e.TeacherId = dy.data.TeacherId;
                    e.RegisterPeople = arrnames[i];
                    e.RegisterPeopleId = arrids[i];
                    e.BZId = dept.DepartmentId;
                    e.BZName = dept.FullName;
                    e.ActivityDate = Convert.ToDateTime(dy.data.ActivityDate);
                    e.ActivityEndDate = Convert.ToDateTime(dy.data.ActivityEndDate);
                    e.InventoryId = dy.data.InventoryId;
                    e.Theme = dy.data.Theme;
                    e.EduType = "6";
                    e.AnswerFlow = "0";
                    ebll.SaveEduBaseInfo(e.ID, e);
                    var messagebll = new MessageBLL();
                    messagebll.SendMessage("事故预想答题", e.ID);
                }
                return new { code = 0, info = "新增成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 新增点评
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Appraise()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                string eduid = dy.data.eduId;
                var e = ebll.GetAnswerList(eduid).FirstOrDefault();
                e.Grade = dy.data.grade;
                e.AppraiseContent = dy.data.appraiseContent;

                ebll.SaveAnswer(e.ID, e);

                var edu = ebll.GetBaseInfoEntity(eduid);
                edu.AnswerFlow = "2";
                edu.Flow = "1";
                edu.AppraiseDate = DateTime.Now;
                ebll.SaveEduBaseInfo(eduid, edu);
                if (edu.EduType == "5")
                {
                    var messagebll = new MessageBLL();
                    messagebll.FinishTodo("技术问答评价", eduid);
                }
                if (edu.EduType == "7")
                {
                    var messagebll = new MessageBLL();
                    messagebll.FinishTodo("考问讲解评价", eduid);
                }
                return new { code = 0, info = "新增成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取技术问答列表(app)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetNewList()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                string from = dy.data.from;
                string to = dy.data.to;
                string teacherid = dy.data.teacherid;
                string regid = dy.data.regid;
                bool allowpaging = dy.allowPaging;
                string flow = dy.data.flow;
                string owner = dy.data.owner;
                string edutype = dy.data.edutype;
                string type = dy.data.type;
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                int total = 0;

                // var list = ebll.GetAllList().Where(x => x.EduType == edutype && x.BZId == user.DepartmentId);
                //    .Where(x.AnswerFlow != "2" && (x.TeacherId == userId || (x.RegisterPeopleId == userId && x.AnswerFlow != "1")));
                //if (!string.IsNullOrEmpty(owner) || !string.IsNullOrEmpty(flow))
                //{
                //    list = list.Where(x => x.AnswerFlow != "2" && (x.TeacherId == userId || (x.RegisterPeopleId == userId && x.AnswerFlow != "1")));
                //    if (owner == "1")
                //    {
                //        list = list.Where(x => x.TeacherId == userId);
                //    }
                //    if (!string.IsNullOrEmpty(flow))
                //    {
                //        list = list.Where(x => x.AnswerFlow == flow).ToList();
                //    }
                //}

                //待办
                //if (type == "1")
                //{
                //    list = list.Where(x => (x.TeacherId == userId && x.AnswerFlow != "2") || (x.RegisterPeopleId == userId && x.AnswerFlow == "0"));
                //}
                ////全部
                //if (type == "2")
                //{
                //    list = list.Where(x => x.AnswerFlow == "2"); //&& (x.TeacherId == userId || x.RegisterPeopleId == userId)
                //}
                //// 终端列表，出题人且未结束(未回答或已回答)
                //if (type == "3")
                //{
                //    list = list.Where(x => x.AnswerFlow != "2" && x.TeacherId == userId);
                //}

                //if (!string.IsNullOrEmpty(from))
                //{
                //    var fromdate = Convert.ToDateTime(from);
                //    list = list.Where(x => x.ActivityDate >= fromdate);
                //}
                //if (!string.IsNullOrEmpty(to))
                //{
                //    var todate = Convert.ToDateTime(to).AddDays(1);
                //    list = list.Where(x => x.ActivityEndDate < todate);
                //}
                //if (!string.IsNullOrEmpty(teacherid))
                //{
                //    list = list.Where(x => x.TeacherId == teacherid);
                //}
                //if (!string.IsNullOrEmpty(regid))
                //{
                //    list = list.Where(x => x.RegisterPeopleId == regid);
                //}

                Pagination pagination = new Pagination();
                pagination.page = Convert.ToInt32(pageIndex);
                pagination.rows = Convert.ToInt32(pageSize);
                List<EduBaseInfoEntity> list = ebll.GetPageList(pagination, JsonConvert.SerializeObject(dy.data), userId);
                total = pagination.records;
                foreach (EduBaseInfoEntity e in list)
                {
                    var files = fileBll.GetFilesByRecIdNew(e.ID);
                    foreach (var f in files)
                    {
                        f.FilePath = f.FilePath.Replace("~/", url);
                    }
                    e.Files = files;
                    files = files.Where(x => x.Description == "照片").ToList();
                    if (files.FirstOrDefault() != null)
                    {
                        e.BgImage = files[0].FilePath;
                    }
                    if (e.AnswerFlow == "0" && e.RegisterPeopleId == userId) e.Describe = "待回答";
                    if (e.AnswerFlow == "1" && e.TeacherId == userId) e.Describe = "待评价";

                }
                //if (allowpaging)
                //{
                //    list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                //}

                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取技术问答列表(app)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetNewListkwjj()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                string from = dy.data.from;
                string to = dy.data.to;
                string teacherid = dy.data.teacherid;
                string regid = dy.data.regid;
                bool allowpaging = dy.allowPaging;
                string flow = dy.data.flow;
                string owner = dy.data.owner;
                string type = dy.data.type;
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                int total = 0;

                var list = ebll.GetAllList().Where(x => x.EduType == "7" && x.BZId == user.DepartmentId);
                //    .Where(x.AnswerFlow != "2" && (x.TeacherId == userId || (x.RegisterPeopleId == userId && x.AnswerFlow != "1")));
                //if (!string.IsNullOrEmpty(owner) || !string.IsNullOrEmpty(flow))
                //{
                //    list = list.Where(x => x.AnswerFlow != "2" && (x.TeacherId == userId || (x.RegisterPeopleId == userId && x.AnswerFlow != "1")));
                //    if (owner == "1")
                //    {
                //        list = list.Where(x => x.TeacherId == userId);
                //    }
                //    if (!string.IsNullOrEmpty(flow))
                //    {
                //        list = list.Where(x => x.AnswerFlow == flow).ToList();
                //    }
                //}

                //待办
                if (type == "1")
                {
                    list = list.Where(x => (x.TeacherId == userId && x.AnswerFlow != "2") || (x.RegisterPeopleId == userId && x.AnswerFlow == "0"));
                }
                //全部
                if (type == "2")
                {
                    list = list.Where(x => x.AnswerFlow == "2"); //&& (x.TeacherId == userId || x.RegisterPeopleId == userId)
                }
                // 终端列表，出题人且未结束(未回答或已回答)
                if (type == "3")
                {
                    list = list.Where(x => x.AnswerFlow != "2" && x.TeacherId == userId);
                }

                if (!string.IsNullOrEmpty(from))
                {
                    var fromdate = Convert.ToDateTime(from);
                    list = list.Where(x => x.ActivityDate >= fromdate);
                }
                if (!string.IsNullOrEmpty(to))
                {
                    var todate = Convert.ToDateTime(to).AddDays(1);
                    list = list.Where(x => x.ActivityEndDate < todate);
                }
                if (!string.IsNullOrEmpty(teacherid))
                {
                    list = list.Where(x => x.TeacherId == teacherid);
                }
                if (!string.IsNullOrEmpty(regid))
                {
                    list = list.Where(x => x.RegisterPeopleId == regid);
                }
                total = list.Count();
                foreach (EduBaseInfoEntity e in list)
                {
                    var files = fileBll.GetFilesByRecIdNew(e.ID);
                    foreach (var f in files)
                    {
                        f.FilePath = f.FilePath.Replace("~/", url);
                    }
                    e.Files = files;
                    files = files.Where(x => x.Description == "照片").ToList();
                    if (files.FirstOrDefault() != null)
                    {
                        e.BgImage = files[0].FilePath;
                    }
                    if (e.AnswerFlow == "0" && e.RegisterPeopleId == userId) e.Describe = "待回答";
                    if (e.AnswerFlow == "1" && e.TeacherId == userId) e.Describe = "待评价";

                }
                if (allowpaging)
                {
                    list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                }

                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取事故预想列表（新）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetNewListsgyx()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                string from = dy.data.from;
                string to = dy.data.to;
                string teacherid = dy.data.teacherid;
                string regid = dy.data.regid;
                bool allowpaging = dy.allowPaging;
                //string flow = dy.data.flow;
                // string owner = dy.data.owner;
                string type = dy.data.type;
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                int total = 0;

                var list = ebll.GetAllList().Where(x => x.EduType == "6" && x.BZId == user.DepartmentId);
                //    .Where(x.AnswerFlow != "2" && (x.TeacherId == userId || (x.RegisterPeopleId == userId && x.AnswerFlow != "1")));
                //if (!string.IsNullOrEmpty(owner) || !string.IsNullOrEmpty(flow))
                //{
                //    list = list.Where(x => x.AnswerFlow != "2" && (x.TeacherId == userId || (x.RegisterPeopleId == userId && x.AnswerFlow != "1")));
                //    if (owner == "1")
                //    {
                //        list = list.Where(x => x.TeacherId == userId);
                //    }
                //    if (!string.IsNullOrEmpty(flow))
                //    {
                //        list = list.Where(x => x.AnswerFlow == flow).ToList();
                //    }
                //}

                //待办
                if (type == "1")
                {
                    list = list.Where(x => (x.TeacherId == userId && x.AnswerFlow != "2") || (x.RegisterPeopleId == userId && x.AnswerFlow == "0"));
                }
                //全部
                if (type == "2")
                {
                    list = list.Where(x => x.AnswerFlow == "2"); //&& (x.TeacherId == userId || x.RegisterPeopleId == userId)
                }
                // 终端列表，出题人且未结束(未回答或已回答)
                if (type == "3")
                {
                    list = list.Where(x => x.AnswerFlow != "2" && x.TeacherId == userId);
                }

                if (!string.IsNullOrEmpty(from))
                {
                    var fromdate = Convert.ToDateTime(from);
                    list = list.Where(x => x.ActivityDate >= fromdate);
                }
                if (!string.IsNullOrEmpty(to))
                {
                    var todate = Convert.ToDateTime(to).AddDays(1);
                    list = list.Where(x => x.ActivityEndDate < todate);
                }
                if (!string.IsNullOrEmpty(teacherid))
                {
                    list = list.Where(x => x.TeacherId == teacherid);
                }
                if (!string.IsNullOrEmpty(regid))
                {
                    list = list.Where(x => x.RegisterPeopleId == regid);
                }
                total = list.Count();
                foreach (EduBaseInfoEntity e in list)
                {
                    var files = fileBll.GetFilesByRecIdNew(e.ID);
                    foreach (var f in files)
                    {
                        f.FilePath = f.FilePath.Replace("~/", url);
                    }
                    e.Files = files;
                    files = files.Where(x => x.Description == "照片").ToList();
                    if (files.FirstOrDefault() != null)
                    {
                        e.BgImage = files[0].FilePath;
                    }
                    if (e.AnswerFlow == "0" && e.RegisterPeopleId == userId) e.Describe = "待回答";
                    if (e.AnswerFlow == "1" && e.TeacherId == userId) e.Describe = "待评价";

                }
                if (allowpaging)
                {
                    list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                }

                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 查看事故预想详情（新）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetDetailNewsgyx()
        {
            try
            {
                ActivityBLL actBll = new ActivityBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string eduid = dy.data;
                EduBaseInfoEntity entity = ebll.GetBaseInfoEntity(eduid);
                var fileList = new FileInfoBLL().GetFilesByRecIdNew(entity.ID);
                var Files1 = fileList.Where(x => x.Description == "照片").ToList();
                var Files = fileList.Where(x => x.Description == "视频").ToList();
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                foreach (FileInfoEntity f in Files1)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                }
                foreach (FileInfoEntity f in Files)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                }
                foreach (FileInfoEntity f in fileList)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                    if (!string.IsNullOrEmpty(f.OtherUrl))
                    {
                        if (new Uploader().Query(f.OtherUrl))
                            f.FilePath = f.OtherUrl;
                    }
                }
                entity.Files1 = Files1;
                entity.Files = Files;
                entity.Files2 = fileList.Where(x => x.Description == "课件" || x.Description == "1").ToList();

                var answers = ebll.GetAnswerList(entity.ID).ToList();
                var appraises = actBll.GetEntityList().Where(x => x.Activityid == entity.ID).ToList();
                entity.Answers = answers;

                foreach (EduAnswerEntity e in entity.Answers)
                {
                    var files = fileBll.GetFilesByRecIdNew(e.ID);
                    foreach (FileInfoEntity f in files)
                    {
                        f.FilePath = f.FilePath.Replace("~/", url);
                    }
                    e.Files = files;
                }
                entity.Appraises = appraises;
                return new { code = 0, info = "获取数据成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = 1, data = new { } };
            }

        }

        /// <summary>
        /// 获取点评选项
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetEduCommentTag()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                var list = ebll.GetCommentTagList(user.DepartmentId);
                return new { code = 0, info = "获取数据成功", count = list.Count(), data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 新增点评选项
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PostEduCommentTag()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                EduCommentTagEntity e = new EduCommentTagEntity();
                e.Tag = dy.data.tag;
                e.CreateDate = DateTime.Now;
                e.DeptId = user.DepartmentId;
                e.CreateUser = user.RealName;
                e.CreateUserId = user.UserId;
                e.ID = Guid.NewGuid().ToString();
                ebll.PostEduCommentTag(e);
                return new { code = 0, info = "获取数据成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        #region 教育培训计划
        EduPlanBLL epbll = new EduPlanBLL();
        #region   不再使用的方法
        /// <summary>
        /// 获取教育培训计划
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetEduPlanList(ListModel model)
        {
            try
            {
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = model.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                var list = epbll.GetPlanList().Where(x => x.BZID == user.DepartmentId).ToList();
                foreach (EduPlanEntity e in list)
                {
                    e.State = e.SubmitState;
                    if (e.VerifyState != "待审核" && !string.IsNullOrEmpty(e.VerifyState)) e.State = e.VerifyState;

                    var vlist = epbll.GetVerifyList(e.ID).Where(x => x.Read == "n");
                    e.NewMessage = vlist.Count().ToString();
                }

                return new { code = 0, info = "成功", count = list.Count, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 创建教育培训计划
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddEduPlan(ListModel model)
        {
            try
            {
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = model.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                EduPlanEntity e = new EduPlanEntity();
                e.CreateDate = DateTime.Now;
                e.CreateUser = user.RealName;
                e.CreateUserId = user.UserId;
                e.BZID = dept.DepartmentId;
                e.BZName = dept.FullName;
                e.Name = model.data.name;
                e.SubmitState = "待提交";
                e.SubmitDate = DateTime.Now;
                e.VerifyState = "";
                e.State = e.SubmitState;
                e.ID = Guid.NewGuid().ToString();
                epbll.SaveEduPlan(e.ID, e);
                // ebll.PostEduCommentTag(e);
                return new { code = 0, info = "成功", data = e };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 删除教育培训计划
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object DelEduPlan(DelModel model)
        {
            try
            {
                // dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                epbll.RemoveEduPlan(model.data.Id);
                var list = epbll.GetPlanInfoList(model.data.Id);
                foreach (EduPlanInfoEntity e in list)
                {
                    epbll.RemoveEduPlanInfo(e.ID);
                }

                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        #endregion
        /// <summary>
        /// 获取教育培训计划内容
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetEduPlanInfoList(ListModel model)
        {
            try
            {
                // dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                DataItemBLL ditem = new DataItemBLL();
                DataItemDetailBLL detail = new DataItemDetailBLL();
                var DrugLevel = ditem.GetEntityByCode("edutype");
                List<DataItemDetailEntity> dlist = detail.GetList(DrugLevel.ItemId).ToList();
                string userId = model.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                string month = model.data.month;
                string edutype = model.data.eduType;
                string year = model.data.year;
                var list = epbll.GetPlanInfoList("").OrderByDescending(x => x.CreateDate).ToList();
                list = list.Where(x => !string.IsNullOrEmpty(x.GroupCode)).ToList();
                list = list.Where(x => x.GroupCode.StartsWith(user.DepartmentCode)).ToList();
                if (!string.IsNullOrEmpty(month)) list = list.Where(x => x.TrainDateMonth == month).ToList();
                if (!string.IsNullOrEmpty(year)) list = list.Where(x => x.TrainDateYear == year).ToList();
                if (!string.IsNullOrEmpty(edutype))
                {
                    if (edutype != "0")
                    {
                        list = list.Where(x => x.TrainType == edutype).ToList();

                    }
                }
                string url = BSFramework.Util.Config.GetValue("AppUrl");
                foreach (var item in list)
                {
                    item.Files = fileBll.GetFilesByRecIdNew(item.ID);
                    foreach (var items in item.Files)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                }

                return new { code = 0, info = "成功", count = list.Count(), data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        public object GetEduPlanInfoDetail(ListModel model)
        {

            var epbll = new EduPlanBLL();
            FileInfoBLL fileBll = new FileInfoBLL();
            string id = model.data.planId;
            var obj = epbll.GetPlanInfoEntity(id);
            obj.Files = fileBll.GetFilesByRecIdNew(id);
            return new { code = 0, info = "成功", data = obj };

        }
        /// <summary>
        /// 获取教育培训计划内容
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetEduPlanInfo(ListModel model)
        {
            try
            {
                // dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string url = BSFramework.Util.Config.GetValue("AppUrl");
                string id = model.data.planId;
                var obj = epbll.GetPlanInfoEntity(id);
                obj.Files = fileBll.GetFilesByRecIdNew(id);
                foreach (var item in obj.Files)
                {
                    item.FilePath = item.FilePath.Replace("~/", url);
                }
                return new { code = 0, info = "成功", data = obj };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取审核消息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetEduPlanVerifyList(ListModel model)
        {
            try
            {
                // dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = model.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                string planid = model.data.planId;

                var list = epbll.GetVerifyList(planid).OrderByDescending(x => x.CreateDate).ToList();
                if (string.IsNullOrEmpty(planid))
                {
                    var planids = epbll.GetPlanInfoList("").Where(x => x.GroupCode.StartsWith(dept.EnCode)).Select(x => x.ID).ToList();
                    list = list.Where(x => planids.Contains(x.PlanId)).ToList();
                }
                return new { code = 0, info = "成功", total = list.Count(), data = list };
            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = 1, data = new { } };
            }

        }

        /// <summary>
        /// 新增修改教育培训计划内容
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object EduPlanInfoForm(/*NewPlanModel model*/)
        {
            try
            {
                // dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                var model = JsonConvert.DeserializeObject<NewPlanModel>(HttpContext.Current.Request["json"]);

                string userId = model.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                string getDel = model.data.DelKeys;
                var DelKeys = getDel.Split(',');
                string keys = string.Empty;
                EduPlanInfoEntity e = model.data.entity;

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
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");

                FileInfoEntity fi = null;
                foreach (string key in HttpContext.Current.Request.Files.AllKeys)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[key];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        RecId = e.ID,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/EdFilePlanInfo/" + uploadDate + "/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = "文件",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\EdFilePlanInfo\\" + uploadDate))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\EdFilePlanInfo\\" + uploadDate);
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\EdFilePlanInfo\\" + uploadDate + "\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }

                e.ModifyDate = DateTime.Now;
                e.ModifyUserName = user.RealName;
                e.ModifyUserId = user.UserId;
                e.ModifyDeptId = dept.DepartmentId;
                e.ModifyDeptName = dept.FullName;
                if (string.IsNullOrEmpty(e.ID))
                {
                    e.ID = Guid.NewGuid().ToString();

                    e.CreateDate = DateTime.Now;
                    e.CreateUser = user.RealName;
                    e.CreateUserId = user.UserId;
                    e.createDeptid = dept.DepartmentId;
                    e.createDeptName = dept.FullName;
                    e.SubmitState = "未提交";
                    e.SubmitDate = DateTime.Now;
                    e.GroupCode = user.DepartmentCode;
                    e.GroupId = user.DepartmentId;
                    e.GroupName = user.DepartmentName;

                }
                if (e.TrainTarget == "本班组")
                {
                    e.TrainUserId = string.Empty;
                    e.TrainUserName = string.Empty;
                    var pbll = new PeopleBLL();
                    var list = new List<PeopleEntity>();
                    list = pbll.GetListByDept(user.DepartmentId).ToList();
                    foreach (PeopleEntity p in list)
                    {
                        e.TrainUserId += p.ID + ',';
                        e.TrainUserName += p.Name + ',';
                    }
                    if (e.TrainUserId.EndsWith(",")) e.TrainUserId = e.TrainUserId.Substring(0, e.TrainUserId.Length - 1);
                    if (e.TrainUserName.EndsWith(",")) e.TrainUserName = e.TrainUserName.Substring(0, e.TrainUserName.Length - 1);
                }
                epbll.SaveEduPlanInfo(e.ID, e);
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 提交教育培训计划
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SubmitEduPlanOld(DelModel model)
        {
            try
            {
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = model.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                var e = epbll.GetPlanInfoEntity(model.data.Id/*.PlanId*/);
                e.SubmitState = "已提交";
                e.SubmitDate = DateTime.Now;

                e.VerifyState = "待审核";
                epbll.SaveEduPlanInfo(e.ID, e);
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 提交教育培训计划
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SubmitEduPlan(PlanInfoModel model)
        {
            try
            {
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = model.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                var data = epbll.GetPlanInfoList("").OrderByDescending(x => x.CreateDate).ToList();
                var deptCode = user.DepartmentCode;
                var e = model.data;
                if (!string.IsNullOrEmpty(deptCode))
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.GroupCode)).ToList();
                    data = data.Where(x => x.GroupCode.StartsWith(deptCode)).ToList();
                }
                if (e.workState == "月度")
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.TrainDateYear)).ToList();
                    data = data.Where(x => x.TrainDateYear.Contains(e.TrainDateYear)).ToList();
                    data = data.Where(x => !string.IsNullOrEmpty(x.TrainDateMonth)).ToList();
                    data = data.Where(x => x.TrainDateMonth.Contains(e.TrainDateMonth)).ToList();
                }
                else if (e.workState == "年度")
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.TrainDateYear)).ToList();
                    data = data.Where(x => x.TrainDateYear.Contains(e.TrainDateYear)).ToList();
                }
                else
                {
                    data = data.Where(x => !string.IsNullOrEmpty(x.TrainDateYear)).ToList();
                    data = data.Where(x => x.TrainDateMonth.Contains(e.TrainDateYear)).ToList();
                    data = data.Where(x => !string.IsNullOrEmpty(x.TrainDateMonth)).ToList();
                    var ck = false;
                    switch (e.TrainDateMonth)
                    {
                        case "1":
                            data = data.Where(x => (x.TrainDateMonth.Contains("1") || x.TrainDateMonth.Contains("2") || x.TrainDateMonth.Contains("3"))).ToList();
                            break;
                        case "2":
                            data = data.Where(x => (x.TrainDateMonth.Contains("4") || x.TrainDateMonth.Contains("5") || x.TrainDateMonth.Contains("6"))).ToList();
                            break;
                        case "3":
                            data = data.Where(x => (x.TrainDateMonth.Contains("7") || x.TrainDateMonth.Contains("8") || x.TrainDateMonth.Contains("9"))).ToList();
                            break;
                        case "4":
                            data = data.Where(x => (x.TrainDateMonth.Contains("10") || x.TrainDateMonth.Contains("11") || x.TrainDateMonth.Contains("12"))).ToList();
                            break;
                        default:
                            ck = true;
                            break;
                    }
                    if (ck)
                    {
                        return new { code = 1, info = "不存在该季度", data = new { } };
                    }
                }
                foreach (var item in data)
                {
                    item.SubmitState = "已提交";
                    item.SubmitDate = DateTime.Now;

                    item.VerifyState = "待审核";
                    epbll.SaveEduPlanInfo(item.ID, item);
                }
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 审核教育培训计划
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddEduPlanVerify(VerifyModel model)
        {
            try
            {
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = model.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                // string entity = JsonConvert.SerializeObject(dy.data);
                //EduPlanVerifyEntity e = JsonConvert.DeserializeObject<EduPlanVerifyEntity>(entity);
                EduPlanVerifyEntity e = model.data;
                e.CreateDate = DateTime.Now;
                e.CreateUser = user.RealName;
                e.CreateUserId = user.UserId;
                e.ID = Guid.NewGuid().ToString();
                epbll.SaveEduPlanVerify(e.ID, e);

                var ep = epbll.GetPlanInfoEntity(e.PlanId);
                if (e.VerifyResult == "0")
                {
                    ep.VerifyState = "审核通过";
                }
                if (e.VerifyResult == "1")
                {
                    ep.VerifyState = "审核不通过";
                }
                epbll.SaveEduPlanInfo(ep.ID, ep);
                // ebll.PostEduCommentTag(e);
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取教育培训类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetEduType()
        {
            try
            {
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                DataItemBLL ditem = new DataItemBLL();
                DataItemDetailBLL detail = new DataItemDetailBLL();
                var DrugLevel = ditem.GetEntityByCode("edutype");
                List<DataItemDetailEntity> list = detail.GetList(DrugLevel.ItemId).ToList();

                return new { code = 0, info = "成功", count = list.Count, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 删除教育培训计划内容
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object DelEduPlanInfo(DelModel model)
        {
            try
            {
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                epbll.RemoveEduPlanInfo(model.data.Id);

                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }



        /// <summary>
        /// APP台账
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetAppList(AppListModel model)
        {
            long pageIndex = model.pageIndex;//当前索引页
            long pageSize = model.pageSize;//每页记录数
            bool allowpaging = model.allowPaging;
            string from = model.data.from;
            string to = model.data.to;
            string bzid = model.data.bzId;
            string appraise = model.data.appraise;
            string type = model.data.eduType;
            var url = BSFramework.Util.Config.GetValue("AppUrl");
            string userId = model.userId;
            UserEntity user = new UserBLL().GetEntity(userId);
            int total = 0;
            var dept = new DepartmentBLL().GetEntity(user.DepartmentId);
            if (dept.IsSpecial)
            {
                dept = new DepartmentBLL().GetEntity(dept.ParentId);
            }
            var depts = new DepartmentBLL().GetList().Where(x => x.EnCode.StartsWith(dept.EnCode)).Select(x => x.DepartmentId);

            if (!string.IsNullOrEmpty(bzid)) depts = new string[] { bzid };

            DateTime? start = null;
            if (!string.IsNullOrEmpty(from)) start = DateTime.Parse(from);
            DateTime? end = null;
            if (!string.IsNullOrEmpty(to)) end = DateTime.Parse(to);

            var list = ebll.GetList(depts.ToArray(), start, end, "1", type, (int)pageSize, (int)pageIndex, out total);
            //var list = ebll.GetAllList().Where(x => depts.Contains(x.BZId) && x.Flow == "1").ToList();
            //if (!string.IsNullOrEmpty(bzid))
            //{
            //    list = list.Where(x => x.BZId == bzid).ToList();
            //}
            //if (!string.IsNullOrEmpty(type)) //类型
            //{
            //    list = list.Where(x => x.EduType == type).ToList();
            //}
            //if (!string.IsNullOrEmpty(from))
            //{
            //    DateTime f = DateTime.Parse(from);
            //    list = list.Where(x => x.ActivityDate >= f).ToList();
            //}
            //if (!string.IsNullOrEmpty(to))
            //{
            //    DateTime t = DateTime.Parse(to);
            //    t = t.AddDays(1);
            //    list = list.Where(x => x.ActivityDate < t).ToList();
            //}

            ActivityBLL actBll = new ActivityBLL();
            var elist = actBll.GetActivityEvaluateEntity(list.Select(x => x.ID).ToList());
            foreach (EduBaseInfoEntity e in list)
            {
                int i = elist.Where(x => x.EvaluateId == userId && x.Activityid == e.ID).ToList().Count();
                if (i > 0) e.AppraiseContent = "已评价";
                else e.AppraiseContent = "未评价";
            }
            if (!string.IsNullOrEmpty(appraise))
            {
                list = list.Where(x => x.AppraiseContent == appraise).ToList();
            }

            return new { code = 0, info = "获取数据成功", count = total, data = list };

        }
        /// <summary>
        /// APP台账详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetAppDetail(AppDetailModel model)
        {
            try
            {
                ActivityBLL actBll = new ActivityBLL();
                string eduid = model.Id;
                EduBaseInfoEntity entity = ebll.GetBaseInfoEntity(eduid);
                var fileList = new FileInfoBLL().GetFilesByRecIdNew(entity.ID);

                var url = BSFramework.Util.Config.GetValue("AppUrl");
                foreach (FileInfoEntity f in fileList)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                    if (!string.IsNullOrEmpty(f.OtherUrl))
                    {
                        if (new Uploader().Query(f.OtherUrl))
                            f.FilePath = f.OtherUrl;
                    }
                }
                entity.Files = fileList;
                entity.Answers = ebll.GetAnswerList(entity.ID).ToList();
                var diskPath = ConfigurationManager.AppSettings["FilePath"].ToString();
                if (entity.Files != null && entity.Files.Count > 0)
                {
                    foreach (var f in entity.Files)
                    {
                        switch (Path.GetExtension(f.FileName).ToLower())
                        {
                            case ".doc":
                            case ".docx":
                            case ".xls":
                            case ".xlsx":
                            case ".ppt":
                            case ".pptx":
                                if (System.IO.File.Exists(diskPath + ("\\Content\\EducationPDF\\" + f.FileId + ".pdf")))
                                {
                                    f.ViewUrl = url + "/Pdf/ViewPDFPage?httpUrl=" + HttpUtility.UrlEncode(url + ("/Resource/Content/EducationPDF/" + f.FileId + ".pdf"));
                                    f.CanView = true;
                                }
                                else
                                {
                                    using (var factory = new ChannelFactory<IQueueService>("upload"))
                                    {
                                        var channel = factory.CreateChannel();
                                        channel.OfficeToPdf(diskPath + f.FilePath.Replace("~/Resource", string.Empty), diskPath + ("/Content/EducationPDF/" + f.FileId + ".pdf"));
                                    }

                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                foreach (EduAnswerEntity e in entity.Answers)
                {
                    var files = fileBll.GetFilesByRecIdNew(entity.EduType == "1" ? e.EduId : e.ID);
                    foreach (FileInfoEntity f in files)
                    {

                        f.FilePath = f.FilePath.Replace("~/", url);
                    }
                    e.Files = files;
                }
                entity.Appraises = actBll.GetEntityList().Where(x => x.Activityid == entity.ID).ToList();
                return new { code = 0, info = "获取数据成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = 1, data = new { } };
            }

        }
        /// <summary>
        /// APP评价 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public object SaveEvaluate(AppDetailModel model)
        {
            try
            {
                ActivityBLL actBll = new ActivityBLL();
                string eduid = model.Id;
                string userId = model.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                var dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                ActivityEvaluateEntity entity = new ActivityEvaluateEntity();
                entity.ActivityEvaluateId = Guid.NewGuid().ToString();
                entity.EvaluateDate = entity.CREATEDATE = DateTime.Now;
                entity.EvaluateId = entity.CREATEUSERID = userId;
                entity.EvaluateUser = entity.CREATEUSERNAME = user.RealName;
                entity.Activityid = eduid;
                entity.DeptName = dept.FullName;
                entity.Score = Convert.ToDecimal(model.data.Score);
                entity.EvaluateContent = model.data.EvaluateContent;
                entity.EvaluateDeptId = dept.DepartmentId;
                if (entity.EvaluateContent == null)
                {
                    entity.EvaluateContent = "";
                }
                actBll.SaveEvaluate(entity.ActivityEvaluateId, model.type, entity);
                return new { code = 0, info = "评价成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { info = "评价失败：" + ex.Message, code = 1, data = new { } };
            }

        }

        /// <summary>
        /// 评价历史记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetEvaluate(AppDetailModel model)
        {
            try
            {
                WorkmeetingBLL meetbll = new WorkmeetingBLL();
                ActivityBLL actBll = new ActivityBLL();
                EducationBLL edubll = new EducationBLL();
                DangerBLL dangerbll = new DangerBLL();
                HumanDangerBLL hdbll = new HumanDangerBLL();
                var list = actBll.GetEntityList().OrderByDescending(x => x.CREATEDATE).ToList();
                string type = model.type;
                var idlist = new List<string>();
                int total = 0;
                if (model.type == "班组活动") idlist = actBll.GetList(1, "", null, null).Select(x => x.ActivityId).ToList();
                else if (model.type == "班前班后会") idlist = meetbll.GetAllList().Select(x => x.MeetingId).ToList();
                else if (model.type == "教育培训") idlist = edubll.GetAllList().Select(x => x.ID).ToList();
                else if (model.type == "危险预知训练") idlist = dangerbll.GetList("").Select(x => x.Id).ToList();
                else if (model.type == "人身风险预控")
                {
                    idlist = hdbll.GetData("", 10000, 1, "", out total).Select(x => x.HumanDangerId.ToString()).ToList();
                }
                if (model.type == "定点拍照")
                {
                    list = list.Where(x => x.EvaluateId == model.userId).Take(10).ToList();
                }
                else
                {
                    list = list.Where(x => x.EvaluateId == model.userId && idlist.Contains(x.Activityid)).ToList();
                }
                list = list.GroupBy(x => x.EvaluateContent).Select(x => x.First()).Take(10).ToList();
                return new { code = 0, info = "成功", data = list };
            }
            catch (Exception ex)
            {
                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }

        }

        [HttpPost]
        public ModelBucket<EduBaseInfoEntity> GetInfo(ParamBucket<string> paramBucket)
        {
            var data = ebll.GetDetail(paramBucket.Data);
            return new ModelBucket<EduBaseInfoEntity> { Success = true, Data = data };
        }

        [HttpPost]
        public ResultBucket EditEducation(ParamBucket<EduBaseInfoEntity> paramBucket)
        {
            ebll.EditEducation(paramBucket.Data);
            return new ResultBucket { Success = true };
        }
        #endregion
    }
}
