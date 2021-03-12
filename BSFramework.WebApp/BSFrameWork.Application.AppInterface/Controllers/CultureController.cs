using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.ClutureWallManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Service.BusinessExceptions;
using BSFramework.Application.Service.ExperienceManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
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
    public class CultureController : ApiController
    {
        /// <summary>
        /// 获取班组简介
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetMeetAbstract([FromBody]JObject json)
        {
            WorkmeetingBLL wbll = new WorkmeetingBLL();
            DepartmentBLL dbll = new DepartmentBLL();
            var result = 0;
            var message = string.Empty;
            var meetInfo = new MeetAbstract();
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userid = dy.userId;

                UserBLL ubll = new UserBLL();
                UserEntity user = ubll.GetEntity(userid);
                var dept = dbll.GetEntity(user.DepartmentId);
                var depts = dbll.GetSubDepartments(dept.DepartmentId, null);
                var meet = wbll.GetMeetAbstractInfo(new string[] { user.DepartmentId });
                //if (dept.Nature != "班组") meet = new List<PeopleEntity>();
                meetInfo.Member = meet.Count();
                meetInfo.Engineer = meet.Where(x => x.JobName == "工程师").Count();
                meetInfo.Technician = meet.Where(x => x.TecLevel == "技师").Count();
                meetInfo.PartyMember = meet.Where(x => x.Visage == "中共党员").Count();
                meetInfo.AssistantEngineer = meet.Where(x => x.JobName == "助理工程师").Count();
                meetInfo.Expert = meet.Where(x => !string.IsNullOrEmpty(x.TecLevel) && x.TecLevel.Contains("高级")).Count();
                meetInfo.Education = meet.Where(x => x.NewDegree == "大专" || x.NewDegree == "本科" || x.NewDegree == "硕士" || x.NewDegree == "博士").Count();
                meetInfo.ExpertTechnician = meet.Where(x => x.TecLevel == "高级技师").Count();
                meetInfo.AverageAage = meet.Count == 0 ? 0 : meet.Average(x => int.Parse(string.IsNullOrEmpty(x.Age) ? "0" : x.Age));
                meetInfo.BzName = dbll.GetEntity(user.DepartmentId).FullName;
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }
            return new { code = result, info = message, data = meetInfo };
        }
        /// <summary>
        /// 得到文化墙信息
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 得到文化墙信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetWallInfo()
        {
            var result = 0;
            var message = string.Empty;
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            CultureWallInfoBLL cultureWallInfoBLL = new CultureWallInfoBLL();
            FileInfoBLL fileInfoBLL = new FileInfoBLL();
            CultureWallInfoModel model = null;
            try
            {
                string res = HttpContext.Current.Request["json"];
                var rq = JsonConvert.DeserializeAnonymousType(res, new
                {
                    userid = string.Empty,
                    data = new
                    {
                        bzid = string.Empty
                    }
                });
                if (rq.data == null)
                {
                    throw new ArgumentNullException("参数错误：data为空");
                }
                if (string.IsNullOrEmpty(rq.data.bzid))
                {
                    throw new ArgumentNullException("参数错误：data.bzid为空");
                }

                var entity = cultureWallInfoBLL.GetEntity(rq.data.bzid);
                if (entity != null)
                {
                    model = (CultureWallInfoModel)JsonConvert.DeserializeAnonymousType(JsonConvert.SerializeObject(entity), new CultureWallInfoModel());

                    IList<FileInfoEntity> fileInfoList = fileInfoBLL.GetCultureWallPics(entity.wallinfoid.ToString());
                    if (fileInfoList != null && fileInfoList.Count > 0)
                    {
                        model.pics = new List<CultureWallInfoFileModel>();
                    }
                    foreach (FileInfoEntity f in fileInfoList)
                    {
                        model.pics.Add(new CultureWallInfoFileModel()
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
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }
            return new { code = result, info = message, data = model };
        }

        /// <summary>
        /// 班组文化墙信息编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object SaveWallInfo()
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                string res = HttpContext.Current.Request["json"];
                var rq = JsonConvert.DeserializeAnonymousType(res, new
                {
                    userid = string.Empty,
                    data = new CultureWallInfoEntity()
                });
                if (rq.data == null)
                {
                    throw new ArgumentNullException("参数有误：data为空");
                }
                UserBLL ubll = new UserBLL();
                UserEntity user = ubll.GetEntity(rq.userid);
                DepartmentEntity depart = new DepartmentBLL().GetEntity(user.DepartmentId);
                rq.data.departmentid = depart.DepartmentId;
                rq.data.departmentname = depart.FullName;
                rq.data.createuserid = rq.userid;
                CultureWallInfoBLL bll = new CultureWallInfoBLL();
                bll.SaveOrUpdate(rq.data);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }
            return new { code = result, info = message };
        }

        /// <summary>
        /// 上传风采剪影、班组荣誉
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UploadFile()
        {
            var result = 0;
            var message = string.Empty;
            CultureWallInfoBLL cultureWallInfoBLL = new CultureWallInfoBLL();
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
                        bzid = string.Empty,
                        deletefileids = string.Empty,
                        fileinfos = new List<FileInfos>()
                    }
                });
                if (rq.data == null)
                {
                    throw new ArgumentNullException("参数有误：data为空");
                }
                if (string.IsNullOrEmpty(rq.data.bzid))
                {
                    throw new ArgumentNullException("参数有误：data.bzid为空");
                }
                //文件上传路径 
                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                filedir = Path.Combine(filedir, "AppFile", "CultureWallInfo");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
                }
                //用户信息
                UserEntity user = ubll.GetEntity(rq.userid);
                DepartmentEntity depart = new DepartmentBLL().GetEntity(user.DepartmentId);

                var wallinfo = cultureWallInfoBLL.GetEntity(rq.data.bzid);
                if (wallinfo == null)
                {
                    wallinfo = new CultureWallInfoEntity();
                    wallinfo.departmentid = depart.DepartmentId;
                    wallinfo.departmentname = depart.FullName;
                    wallinfo.createuserid = user.UserId;
                    cultureWallInfoBLL.SaveOrUpdate(wallinfo);

                    wallinfo = cultureWallInfoBLL.GetEntity(rq.data.bzid);
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
                //取出相关附件信息
                IList<FileInfoEntity> fileInfoList = fileInfoBLL.GetCultureWallPics(wallinfo.wallinfoid.ToString());
                //汇总班组荣誉
                int fileCount = fileInfoList.Where(x => x.FileType == "1").Count();

                //文件上传
                HttpFileCollection files = HttpContext.Current.Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i] != null)
                    {
                        FileInfos fis = rq.data.fileinfos.ElementAt(i);
                        if (fileCount >= 12 && fis.filetype == "1")
                        {
                            throw new ArgumentNullException("上传数超过最大限制(最多12张)");
                        }

                        HttpPostedFile file = files[i];
                        string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                        string fileId = Guid.NewGuid().ToString();//上传后文件名
                        string filename = fileId + ext;
                        if (fis.modifydate.HasValue)
                        {
                            fis.modifydate = new DateTime(fis.modifydate.Value.Year, fis.modifydate.Value.Month, fis.modifydate.Value.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        }
                        var fileentity = new FileInfoEntity
                        {
                            FileId = fileId,
                            FolderId = wallinfo.wallinfoid.ToString(),
                            RecId = wallinfo.wallinfoid.ToString(),
                            FileName = filename,
                            FilePath = "~/Resource/AppFile/CultureWallInfo/" + filename,
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
                result = 1;
                message = ex.Message;
            }
            return new { code = result, info = message };
        }
        /// <summary>
        /// 班组文化墙 风采剪影专用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UploadFileForWall()
        {
            var result = 0;
            var message = string.Empty;
            CultureWallInfoBLL cultureWallInfoBLL = new CultureWallInfoBLL();
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
                        bzid = string.Empty,
                        fileinfos = new FileInfos()
                    }
                });
                if (rq.data == null)
                {
                    throw new ArgumentNullException("参数有误：data为空");
                }
                if (string.IsNullOrEmpty(rq.data.bzid))
                {
                    throw new ArgumentNullException("参数有误：data.bzid为空");
                }
                //文件上传路径 
                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                filedir = Path.Combine(filedir, "AppFile", "CultureWallInfo");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
                }
                //用户信息
                UserEntity user = ubll.GetEntity(rq.userid);
                DepartmentEntity depart = new DepartmentBLL().GetEntity(user.DepartmentId);

                var wallinfo = cultureWallInfoBLL.GetEntity(rq.data.bzid);
                if (wallinfo == null)
                {
                    wallinfo = new CultureWallInfoEntity();
                    wallinfo.departmentid = depart.DepartmentId;
                    wallinfo.departmentname = depart.FullName;
                    wallinfo.createuserid = user.UserId;
                    cultureWallInfoBLL.SaveOrUpdate(wallinfo);

                    wallinfo = cultureWallInfoBLL.GetEntity(rq.data.bzid);
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
                        FileInfos fis = rq.data.fileinfos;
                        //先查找数据库里有没有该编号的图片，有的话就删除掉再添加
                        var oldFileList = fileInfoBLL.GetCultureWallPics(wallinfo.wallinfoid.ToString()).Where(p => p.FileType == "0" && p.SortCode == fis.key).ToList();
                        foreach (var fileitem in oldFileList)
                        {
                            fileInfoBLL.DeleteFile(fileitem.RecId, fileitem.FileName, HttpContext.Current.Server.MapPath(fileitem.FilePath));
                        }
                        fis.modifydate = new DateTime(fis.modifydate.Value.Year, fis.modifydate.Value.Month, fis.modifydate.Value.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        var fileentity = new FileInfoEntity
                        {
                            FileId = fileId,
                            FolderId = wallinfo.wallinfoid.ToString(),
                            RecId = wallinfo.wallinfoid.ToString(),
                            FileName = filename,
                            FilePath = "~/Resource/AppFile/CultureWallInfo/" + filename,
                            FileType = "0",// 0 - 风采剪影 1 - 班组荣誉

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
                result = 1;
                message = ex.Message;
            }
            return new { code = result, info = message };
        }

        /// <summary>
        /// 获取学习园地
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetLearningGardens()
        {
            var result = 0;
            var message = string.Empty;
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            List<object> list = null;
            try
            {
                string res = HttpContext.Current.Request["json"];
                var rq = JsonConvert.DeserializeAnonymousType(res, new
                {
                    userid = string.Empty
                });
                DataTable dt = new SafetydayBLL().GetLearningGardens();
                if (dt != null && dt.Rows.Count > 0)
                {
                    list = new List<object>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new
                        {
                            filename = dr["filename"].ToString(),
                            filepath = dr["filepath"].ToString().Replace("~/", url)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }
            return new { code = result, info = message, data = list };
        }
        /// <summary>
        /// 获取班组成员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetDeptMembers()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var userid = jobject.Value<string>("userId");

            UserBLL ubll = new UserBLL();
            UserEntity user = ubll.GetEntity(userid);

            DepartmentBLL dbll = new DepartmentBLL();
            var dept = dbll.GetEntity(user.DepartmentId);
            //string bzId = dbll.GetList().Where(x => x.FullName == "电气二次班").ToList()[0].DepartmentId;
            var result = 0;
            var message = string.Empty;
            var list = new List<PeopleEntity>();

            try
            {
                PeopleBLL pbll = new PeopleBLL();
                list = pbll.GetListByDept(user.DepartmentId).ToList();
                //string[] property = new string[] { "Planer", "Name" };
                //bool[] sort = new bool[] { true, true };

                //list = new IListSort<PeopleEntity>(list, property, sort).Sort().ToList();
                //if (dept.Nature != "班组") list = new List<PeopleEntity>();
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = list.Select(x => new { x.ID, x.Name, x.Quarters, x.Photo }), count = list.Count };
        }

        /// <summary>
        /// 月度工作任务评分
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetMonthScore()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var jobject = JObject.Parse(json);
            var userid = jobject.Value<string>("userId");
            var year = jobject.SelectToken("data.year").Value<int>();
            var month = jobject.SelectToken("data.month").Value<int>();

            UserBLL ubll = new UserBLL();
            UserEntity user = ubll.GetEntity(userid);
            DepartmentBLL dbll = new DepartmentBLL();
            //string bzId = dbll.GetList().Where(x => x.FullName == "电气二次班").ToList()[0].DepartmentId;
            var result = 0;
            var message = string.Empty;
            var list = new List<UserScoreEntity>();

            try
            {
                ScoreBLL sbll = new ScoreBLL();
                list = sbll.GetScore1(user.DepartmentId, year, month);
            }
            catch (Exception ex)
            {
                result = 1;
                message = ex.Message;
            }

            return new { code = result, info = message, data = list, count = list.Count };
        }

        [HttpPost]
        public object SaveSummary([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userid;
                string bzId = dy.data.bzid;
                string summary = dy.data.summary;
                UserEntity user = new UserBLL().GetEntity(userId);
                CultureWallInfoBLL cultureWallInfoBLL = new CultureWallInfoBLL();
                var entity = cultureWallInfoBLL.GetEntity(bzId);
                if (entity != null)
                {
                    entity.summary = summary;
                    cultureWallInfoBLL.SaveOrUpdate(entity);
                }
                else
                {
                    return new { Code = -1, Info = "找不到对应的班组" };
                }
                return new { Code = 0, Info = "保存成功" };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "保存失败：" + ex.Message };
            }

        }

        #region 班组文化墙屏保相关接口
        [HttpPost]
        public object SaveScreensaver()
        {
            ScreensaverBLL bll = new ScreensaverBLL();
            try
            {
                string res = HttpContext.Current.Request["json"];
                var rq = JsonConvert.DeserializeAnonymousType(res, new
                {
                    userid = string.Empty,
                    data = new
                    {
                        bzid = string.Empty,
                        deletefileids = string.Empty,
                    }
                });
                if (string.IsNullOrWhiteSpace(rq.userid)) throw new ArgumentNullException("userid不能为空");
                UserEntity user = new UserBLL().GetEntity(rq.userid);
                if (user == null) throw new Exception("未找到用户");
                if (rq.data == null) throw new ArgumentNullException("data不能为空");
                if (string.IsNullOrWhiteSpace(rq.data.bzid)) throw new ArgumentNullException("data.bzid不能为空");
                DepartmentEntity deptEntity = new DepartmentBLL().GetEntity(rq.data.bzid);
                if (deptEntity == null) throw new Exception("未找到对应的班组");
                //上传前检测是否有删除文件
                if (!string.IsNullOrEmpty(rq.data.deletefileids))
                {
                    string[] fileids = rq.data.deletefileids.Split(',');

                    foreach (string fileid in fileids)
                    {
                        ScreensaverEntity fileEntity = bll.GetEntity(fileid);
                        if (fileEntity != null)
                        {
                            bll.DeleteFile(fileEntity.FileId, HttpContext.Current.Server.MapPath(fileEntity.FilePath));
                        }
                    }
                }
                //文件上传路径 
                var filedir = BSFramework.Util.Config.GetValue("FilePath");
                filedir = Path.Combine(filedir, "AppFile", "CultureWallInfo");
                if (!System.IO.Directory.Exists(filedir))
                {
                    System.IO.Directory.CreateDirectory(filedir);
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

                        var fileentity = new ScreensaverEntity
                        {
                            FileId = fileId,
                            FileName = file.FileName,
                            FilePath = "~/Resource/AppFile/CultureWallInfo/" + filename,
                            FileType = file.ContentType,
                            DeptId = deptEntity.DepartmentId,
                            DeptName = deptEntity.FullName,
                            CREATEDATE = DateTime.Now,
                            CREATEUSERID = user.UserId,
                            CREATEUSERNAME = user.RealName
                        };
                        file.SaveAs(Path.Combine(filedir, filename));

                        bll.SaveForm(fileentity);
                    }
                }
                return new { Code = 0, Info = "操作成功", data = "操作成功" };
            }

            catch (Exception ex)
            {
                return new { Code = -1, Info = "操作失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 获取屏保列表数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetScreensaverList([FromBody]JObject json)
        {
            ScreensaverBLL bll = new ScreensaverBLL();
            try
            {
                string res = HttpContext.Current.Request["json"];
                var rq = JsonConvert.DeserializeAnonymousType(res, new
                {
                    userid = string.Empty,
                    data = new
                    {
                        bzid = string.Empty,
                    }
                });
                if (rq.data == null) throw new ArgumentNullException("data不能为空");
                if (rq.data.bzid == null) throw new ArgumentNullException("data.bzid不能为空");
                List<ScreensaverEntity> data = bll.GetList(rq.data.bzid);
                var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
                data.ForEach(p =>
                {
                    p.FilePath = p.FilePath.Replace("~/", url);
                });
                return new { Code = 0, Info = "查询成功", Data = data };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "查询失败", Data = ex.Message };
            }
        }
        #endregion
    }
}
