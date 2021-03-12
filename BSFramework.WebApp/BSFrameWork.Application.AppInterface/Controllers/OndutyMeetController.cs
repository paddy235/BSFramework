using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Busines.OndutyManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.OndutyManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;
namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class OndutyMeetController : BaseApiController
    {
        private FileInfoBLL fileBll = new FileInfoBLL();
        private string url = BSFramework.Util.Config.GetValue("AppUrl");
        private OndutyMeetBLL bll = new OndutyMeetBLL();
        private EducationBLL edbll = new EducationBLL();
        private EdActivityBLL edacbll = new EdActivityBLL();
        private ActivityBLL acbll = new ActivityBLL();
        private PeopleBLL ppbll = new PeopleBLL();
        private OndutyMeetOnoffBLL onoff = new OndutyMeetOnoffBLL();
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetRecordListJson(BaseDataModel<OndutyMeetimg> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                Pagination pagination = new Pagination();
                //pagination.p_kid = "id";
                //pagination.p_fields = "CreateUserName,CreateDate,ondutyuser,ondutyuserid,ondutyshift,ondutycontext,ondutytime,ondutydeptid,ondutydeptcode,ondutydept";
                //pagination.p_tablename = "wg_ondutymeet";
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
                //if (!string.IsNullOrEmpty(dy.data.keyvalue))
                //{
                //    pagination.conditionJson += string.Format(" and otherid='{0}'", dy.data.keyvalue);
                //}
                //if (!string.IsNullOrEmpty(dy.data.meettype))
                //{
                //    pagination.conditionJson += string.Format(" and ondutyshift like '%{0}%'", dy.data.meettype);
                //}
                //if (!string.IsNullOrEmpty(dy.data.moduletype))
                //{
                //    pagination.conditionJson += string.Format(" and ondutyshift like '%{0}%'", dy.data.moduletype);
                //}
                var data = bll.GetPagesList(pagination, JsonConvert.SerializeObject(dy.data),dy.userId);
                //var dataJson = data.ToJson();
                //var dataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OndutyMeetEntity>>(dataJson);

                foreach (var item in data)
                {
                    var people = ppbll.GetEntity(item.ondutyuserid);
                    if (string.IsNullOrEmpty(people.Photo))
                    {
                        item.photo = "";
                    }
                    else
                    {
                        item.photo = people.Photo.Replace("~/", url);

                    }
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
        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket IndutyMeet(ParamBucket<IndutyMeetimg> args)
        {
            var user = OperatorProvider.Provider.Current();
            var nowTime = DateTime.Now;
            var start = DateTime.Today;
            var end = start.AddDays(1).AddMinutes(-1);

            try
            {
                var users = default(List<UserEntity>);
                using (var faceutil = new FaceUtil())
                {
                    users = faceutil.Valid(args.Data.faces.ToArray());
                }
                if (users == null) return new ResultBucket() { Success = false };
                List<OndutyMeetEntity> ondutyList = new List<OndutyMeetEntity>();
                foreach (var item in users)
                {
                    var ondutyCk = bll.GetList(start, end, item.UserId, item.DepartmentId).Where(x => x.otherid == args.Data.keyvalue);

                    if (ondutyCk.Count() == 0)
                    {
                        OndutyMeetEntity entity = new OndutyMeetEntity();
                        entity.id = Guid.NewGuid().ToString();
                        entity.CreateUserId = user.UserId;
                        entity.CreateUserName = user.UserName;
                        entity.CreateDate = nowTime;
                        entity.ondutycontext = "";
                        var deptID = item.DepartmentId;
                        var dept = new DepartmentBLL().GetEntity(deptID);
                        entity.ondutydept = dept.FullName;
                        entity.ondutydeptcode = dept.EnCode;
                        entity.ondutydeptid = dept.DepartmentId;
                        entity.ondutyuser = item.RealName;
                        entity.ondutyuserid = item.UserId;
                        entity.ondutytime = nowTime;
                        entity.ondutyshift = args.Data.moduletype + "," + args.Data.meettype;
                        entity.otherid = args.Data.keyvalue;
                        //bll.SaveForm("", entity);
                        ondutyList.Add(entity);
                    }
                }
                switch (args.Data.moduletype)
                {
                    case "2":
                        activityOnduty(ondutyList, args.Data.keyvalue);
                        break;
                    case "1":
                        edOnduty(ondutyList, args.Data.keyvalue);
                        break;
                    default:
                        return new ResultBucket { Success = false, Message = "不存在定义模块" };
                }

                return new ResultBucket() { Success = true };
            }
            catch (Exception ex)
            {
                return new ResultBucket { Success = false, Message = ex.Message };
            }
        }
        public class meetOnoffmodel
        {
            public string keyvalue { get; set; }
            public string onoff { get; set; }
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost]
        public object MeetOnOff(BaseDataModel<meetOnoffmodel> dy)
        {

            onoff.OnOff(dy.data.keyvalue, dy.data.onoff);
            return new { code = 0, info = "操作成功" };
        }

        private bool activityOnduty(List<OndutyMeetEntity> list, string keyvlaue)
        {
            var acinfo = acbll.GetEntity(keyvlaue);
            if (acinfo != null)
            {
                list = list.Where(x => x.ondutydeptid == acinfo.GroupId).ToList();
                var ckUser = string.Join(",", list.Select(x => x.ondutyuserid));
                foreach (var item in acinfo.ActivityPersons)
                {
                    if (ckUser.Contains(item.PersonId))
                    {
                        item.IsSigned = true;
                        acbll.SaveActivityPerson(item);
                    }
                }
                foreach (var item in list)
                {
                    bll.SaveForm("", item);
                }
                if (list.Count > 0)
                    return true;
                else
                    return false;

            }
            return false;
        }

        private bool edOnduty(List<OndutyMeetEntity> list, string keyvlaue)
        {
            var edinfo = edbll.GetBaseInfoEntity(keyvlaue);
            if (edinfo == null)
            {
                var edacinfo = edacbll.GetEntity(keyvlaue);
                if (edacinfo != null)
                {
                    list = list.Where(x => x.ondutydeptid == edacinfo.GroupId).ToList();

                    var ckUser = string.Join(",", list.Select(x => x.ondutyuserid));
                    foreach (var item in edacinfo.ActivityPersons)
                    {
                        if (ckUser.Contains(item.PersonId))
                        {
                            item.IsSigned = true;
                            edacbll.SaveActivityPerson(item);
                        }
                    }
                    foreach (var item in list)
                    {
                        bll.SaveForm("", item);
                    }
                    if (list.Count > 0)
                        return true;
                    else
                        return false;

                }
            }
            else
            {
                list = list.Where(x => x.ondutydeptid == edinfo.BZId).ToList();
                var newBool = false;
                if (list.Count > 0)
                {
                    var Ondy = bll.GetList(list[0].otherid);
                    if (Ondy.Count() == 0)
                    {
                        newBool = true;
                    }
                }
                edinfo.AttendPeople = newBool ? string.Join(",", list.Select(x => x.ondutyuser)) : edinfo.AttendPeople + "," + string.Join(",", list.Select(x => x.ondutyuser));
                edinfo.AttendPeopleId = newBool ? string.Join(",", list.Select(x => x.ondutyuserid)) : edinfo.AttendPeopleId + "," + string.Join(",", list.Select(x => x.ondutyuserid));
                edbll.updataEduBaseInfo(edinfo);
                foreach (var item in list)
                {
                    bll.SaveForm("", item);
                }
                if (list.Count > 0)
                    return true;
                else
                    return false;

            }
            return false;
        }
        [HttpPost]
        public object GetCkImg(BaseDataModel<OndutyMeetimg> dy)
        {

            var keyvalue = dy.data.keyvalue;
            var type = dy.data.moduletype;
            if (string.IsNullOrEmpty(type))
            {
                return new { code = 1, info = "类型不能为空" };
            }
            var path = "";
            GetGuId(keyvalue, type, out path);
            return new { code = 0, info = "成功", data = path };
        }
        private void GetGuId(string keyvalue, string type, out string newpath)
        {
            try
            {
                var user = OperatorProvider.Provider.Current();
                FileInfoBLL fb = new FileInfoBLL();
                var flist = fb.GetFilebyDescription(keyvalue, "人脸二维码");
                if (flist != null)
                {
                    newpath = flist.FilePath;
                }
                else
                {

                    //二维码画图
                    var encoder = new QRCodeEncoder();
                    var image = encoder.Encode(keyvalue + "|" + type, Encoding.UTF8);
                    var filedir = BSFramework.Util.Config.GetValue("FilePath");
                    if (!System.IO.Directory.Exists(filedir))
                    {
                        System.IO.Directory.CreateDirectory(filedir);
                    }

                    if (!Directory.Exists(Path.Combine(filedir, "DocumentFile", "OndutyMeet")))
                    {
                        Directory.CreateDirectory(Path.Combine(filedir, "DocumentFile", "OndutyMeet"));
                    }
                    //保存路径
                    var newurl = Path.Combine(filedir, "DocumentFile", "OndutyMeet", keyvalue + ".jpg");
                    //保存图片
                    image.Save(newurl);
                    //创建数据实体 
                    var fileentity = new FileInfoEntity
                    {
                        FileId = Guid.NewGuid().ToString(),
                        RecId = keyvalue,
                        FileName = System.IO.Path.GetFileName(newurl),
                        FilePath = "~/Resource/DocumentFile/OndutyMeet/" + keyvalue + ".jpg",
                        FileType = "jpg",
                        FileExtensions = ".jpg",
                        Description = "人脸二维码",
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


        /// <summary>



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
                var dy = JsonConvert.DeserializeObject<BaseDataModel<OndutyMeetModel>>(GetJson);
                string userId = dy.userId;
                var Entity = dy.data.entity;
                var ck = false;
                if (string.IsNullOrEmpty(Entity.id))
                {
                    ck = true;
                    Entity.id = Guid.NewGuid().ToString();
                }
                string id = Entity.id;
                var OnOff = onoff.getCk(Entity.otherid);
                if (OnOff == null)
                {
                    return new { code = 1, info = "请打开终端人脸签到" };
                }
                else
                {
                    if (!OnOff.onoff)
                        return new { code = 1, info = "请打开终端人脸签到" };
                }
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
                List<OndutyMeetEntity> list = new List<OndutyMeetEntity>();
                var info = false;
                if (ck)
                {
                    Entity.ondutydeptid = dept.DepartmentId;
                    Entity.ondutydeptcode = dept.EnCode;
                    Entity.ondutydept = dept.FullName;
                    //bll.SaveForm("", Entity);

                    var start = DateTime.Today;
                    var end = start.AddDays(1).AddMinutes(-1);
                    var ondutyCk = bll.GetList(start, end, Entity.ondutyuserid, Entity.ondutydeptid).Where(x => x.otherid == Entity.otherid);
                    if (ondutyCk.Count() > 0)
                    {
                        return new { code = 1, info = "已经签到" };
                    }
                    list.Add(Entity);
                    if (Entity.ondutyshift.Contains("2"))
                    {
                        info = activityOnduty(list, Entity.otherid);
                    }
                    else
                    {
                        info = edOnduty(list, Entity.otherid);
                    }
                }
                else
                {
                    bll.SaveForm(Entity.id, Entity);
                }
                if (info)
                {
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
                            FilePath = "~/Resource/AppFile/OndutyMeet/" + fileId + ext,
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
                        if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\OndutyMeet"))
                        {
                            System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\OndutyMeet");
                        }
                        file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\OndutyMeet\\" + fileId + ext);
                        //保存附件信息
                        fileBll.SaveForm(fi);


                    }
                }
                else
                {
                    return new { code = 1, info = "非本班人员不能签到" };

                }
                return new { code = 0, info = "操作成功" };

            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


    }
}
