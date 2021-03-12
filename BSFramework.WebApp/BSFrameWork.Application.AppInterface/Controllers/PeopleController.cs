using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Text;
using ThoughtWorks.QRCode.Codec;
using System.IO;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.CertificateManage;
using BSFramework.Application.Busines.CertificateManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Service.ExperienceManage;
using BSFrameWork.Application.AppInterface.Models;
using BSFrameWork.Application.AppInterface.Models.Sync;
using System.Collections.Specialized;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class PeopleController : BaseApiController
    {
        DepartmentBLL dtbll = new DepartmentBLL();
        PeopleBLL pbll = new PeopleBLL();
        UserBLL ubll = new UserBLL();
        RoleBLL rbll = new RoleBLL();
        DataItemBLL ditem = new DataItemBLL();
        DataItemDetailBLL detail = new DataItemDetailBLL();
        private FileInfoBLL fileInfoBLL = new FileInfoBLL();
        private UserWorkAllocationBLL bll = new UserWorkAllocationBLL();
        /// <summary>
        /// 获取成员列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetList([FromBody] JObject json)
        {
            var baseUrl = BSFramework.Util.Config.GetValue("AppUrl");

            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                int total = 0;
                var list = new List<PeopleEntity>();
                list = pbll.GetListByDept(user.DepartmentId).ToList();

                foreach (var item in list)
                {
                    var filelist = fileInfoBLL.GetFileList(item.ID).ToList();
                    var fileinfo = filelist.FirstOrDefault(x => x.Description == "Photo");
                    if (fileinfo != null)
                    {
                        item.Photo = BSFramework.Util.Config.GetValue("AppUrl") + fileinfo.FilePath.Substring(2, fileinfo.FilePath.Length - 2);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.Photo))
                        {
                            if (item.Photo.IndexOf("http") < 0)
                                item.Photo = BSFramework.Util.Config.GetValue("AppUrl") + item.Photo.Substring(item.Photo.IndexOf("/") + 1);

                        }
                    }
                }
                //string[] property = new string[] { "Planer","Name" };
                //bool[] sort = new bool[] { true,true };

                //list = new IListSort<PeopleEntity>(list, property, sort).Sort().ToList();
                //if (dept.Nature == "班组")
                //{
                //    var plist = pbll.GetListByDept(user.DepartmentId, out total);
                //    list = plist.Select(x => new 
                //    {
                //        Id=x.ID,
                //        Name=x.Name
                //    }).ToList();
                //}
                //else 
                //{
                //    var ulist = ubll.GetDeptUsers(user.DepartmentId).Where(x => x.DepartmentId != "0");
                //    total = ulist.Count();
                //    list = ulist.Select(x => new
                //    {
                //        Id = x.UserId,
                //        Name = x.RealName
                //    }).ToList();
                //}
                //foreach (PeopleEntity p in list) 
                //{
                //    p.Photo = BSFramework.Util.Config.GetValue("AppUrl") + p.Photo;
                //}
                return new { code = 0, info = "获取数据成功", count = list.Count, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取部门与人员的层级数据 
        /// (管理人员专用)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object ManagerGetList([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                if (user == null) throw new Exception("当前用户不存在");
                string userDeptCode = new DepartmentBLL().GetEntity(user.DepartmentId).EnCode;
                var deptList = new DepartmentBLL().GetList().Where(p => p.EnCode.StartsWith(userDeptCode)).ToList();
                //var userList = new UserBLL().GetUserList().Where(p => p.DepartmentCode.StartsWith(userDeptCode)).ToList();
                IList peopleList = new PeopleBLL().GetList(userDeptCode);
                //var url = BSFramework.Util.Config.GetValue("AppUrl");
                //userList.ForEach(x => {
                //    x.HeadIcon = !string.IsNullOrWhiteSpace(x.HeadIcon) ? x.HeadIcon.Replace("~/", url) : null;
                //});
                return new { Code = 0, Info = "查询成功", data = new { DeptList = deptList, UserList = peopleList } };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "查询失败", data = ex.Message };
            }

        }
        /// <summary>
        /// 获取部门与人员的层级数据 
        /// (管理人员专用)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object ManagerAllGetList([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                if (user == null) throw new Exception("当前用户不存在");
                string userDeptCode = new DepartmentBLL().GetCompany(user.DepartmentId).EnCode;
                var deptList = new DepartmentBLL().GetList().Where(p => p.EnCode.StartsWith(userDeptCode)).ToList();
                //var userList = new UserBLL().GetUserList().Where(p => p.DepartmentCode.StartsWith(userDeptCode)).ToList();
                IList peopleList = new PeopleBLL().GetList(userDeptCode);
                //var url = BSFramework.Util.Config.GetValue("AppUrl");
                //userList.ForEach(x => {
                //    x.HeadIcon = !string.IsNullOrWhiteSpace(x.HeadIcon) ? x.HeadIcon.Replace("~/", url) : null;
                //});
                return new { Code = 0, Info = "查询成功", data = new { DeptList = deptList, UserList = peopleList } };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "查询失败", data = ex.Message };
            }

        }
        /// <summary>
        /// 获取成员列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetListForm([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数

                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                var list = new List<PeopleEntity>();
                list = pbll.GetListByDept(user.DepartmentId).ToList();
                //string[] property = new string[] { "Planer", "Name" };
                //bool[] sort = new bool[] { true, true };

                //list = new IListSort<PeopleEntity>(list, property, sort).Sort().ToList();
                #region  转岗读取信息
                var old = bll.GetIsSendUser(user.DepartmentId);//原部门
                var newuser = bll.GetSendUser(user.DepartmentId);//转向部门
                foreach (var item in old)
                {
                    var one = list.FirstOrDefault(x => x.ID == item.userId);
                    if (one == null)
                    {
                        continue;
                    }
                    one.state = "转岗确认中";
                    one.AllocationId = item.Id;
                    var ck = false;
                    if (item.oldquarters != one.Quarters)
                    {
                        item.oldquarters = one.Quarters;
                        ck = true;
                    }
                    if (item.oldRoleDutyName != one.RoleDutyName)
                    {
                        item.oldRoleDutyName = one.RoleDutyName;
                        ck = true;
                    }
                    if (item.username != one.Name)
                    {
                        item.username = one.Name;
                        ck = true;
                    }
                    if (ck)
                    {
                        bll.OperationEntity(item, userId);

                    }

                }
                foreach (var item in newuser)
                {
                    var one = pbll.GetEntity(item.userId);
                    if (one == null)
                    {
                        continue;
                    }
                    one.state = "转岗待确认";
                    one.AllocationId = item.Id;
                    var ck = false;
                    if (item.oldquarters != one.Quarters)
                    {
                        item.oldquarters = one.Quarters;
                        ck = true;
                    }
                    if (item.oldRoleDutyName != one.RoleDutyName)
                    {
                        item.oldRoleDutyName = one.RoleDutyName;
                        ck = true;
                    }
                    if (item.username != one.Name)
                    {
                        item.username = one.Name;
                        ck = true;
                    }
                    if (ck)
                    {
                        bll.OperationEntity(item, userId);
                    }
                    list.Add(one);
                }
                #endregion
                //if (dept.Nature == "班组")
                //{
                //    var plist = pbll.GetListByDept(user.DepartmentId, out total);
                //    list = plist.Select(x => new 
                //    {
                //        Id=x.ID,
                //        Name=x.Name
                //    }).ToList();
                //}
                //else 
                //{
                //    var ulist = ubll.GetDeptUsers(user.DepartmentId).Where(x => x.DepartmentId != "0");
                //    total = ulist.Count();
                //    list = ulist.Select(x => new
                //    {
                //        Id = x.UserId,
                //        Name = x.RealName
                //    }).ToList();
                //}
                //foreach (PeopleEntity p in list) 
                //{
                //    p.Photo = BSFramework.Util.Config.GetValue("AppUrl") + p.Photo;
                //}
                var a = list.Where(p => p.IsEpiboly != "1").ToList();
                var d = a.Where(p => !string.IsNullOrWhiteSpace(p.Planer)).OrderBy(p => p.Planer).ThenBy(p => p.Name);
                var e = a.Where(p => string.IsNullOrWhiteSpace(p.Planer)).OrderBy(p => p.Planer).ThenBy(p => p.Name);
                var b = list.Where(p => p.IsEpiboly == "1");
                var c = new List<PeopleEntity>();
                c.AddRange(d);
                c.AddRange(e);
                c.AddRange(b);

                var baseUrl = BSFramework.Util.Config.GetValue("AppUrl");

                foreach (var item in c)
                {
                    var filelist = fileInfoBLL.GetFileList(item.ID).ToList();
                    var fileinfo = filelist.FirstOrDefault(x => x.Description == "Photo");
                    if (fileinfo != null)
                    {
                        item.Photo = BSFramework.Util.Config.GetValue("AppUrl") + fileinfo.FilePath.Substring(2, fileinfo.FilePath.Length - 2);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.Photo))
                        {
                            if (item.Photo.IndexOf("http") < 0)
                                item.Photo = BSFramework.Util.Config.GetValue("AppUrl") + item.Photo.Substring(item.Photo.IndexOf("/") + 1);

                        }
                    }
                }
                return new { code = 0, info = "获取数据成功", count = c.Count, data = c };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取成员详情
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<PeopleEntity> GetDetail(ParamBucket<string> paramBucket)
        {
            //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

            var userEntity = new UserBLL().GetEntity(paramBucket.Data);
            var entity = pbll.GetEntity(paramBucket.Data);
            var loginfo = pbll.GetLogins().Where(x => x.UserId == paramBucket.Data).FirstOrDefault();
            entity.hasface = "n";
            entity.hasfinger = "n";
            entity.RoleId = userEntity.RoleId;
            entity.RoleName = userEntity.RoleName;
            entity.UserAccount = userEntity.Account;
            entity.UserType = userEntity.UserType;
            if (loginfo != null)
            {
                if (!string.IsNullOrEmpty(loginfo.Face))
                {
                    entity.hasface = "y";
                }
                if (!string.IsNullOrEmpty(loginfo.Finger))
                {
                    entity.hasfinger = "y";
                }
            }
            var list = fileInfoBLL.GetFileList(entity.ID).ToList();
            if (list.Count > 0)
            {
                var fileinfo = list.FirstOrDefault(x => x.Description == "Photo");
                if (fileinfo != null)
                {
                    entity.Photo = BSFramework.Util.Config.GetValue("AppUrl") + fileinfo.FilePath.Substring(2, fileinfo.FilePath.Length - 2);
                }
                if (!string.IsNullOrEmpty(entity.Photo))
                {
                    if (entity.Photo.IndexOf("http") < 0)
                        entity.Photo = BSFramework.Util.Config.GetValue("AppUrl") + entity.Photo.Substring(entity.Photo.IndexOf("/") + 1);

                }
                fileinfo = list.FirstOrDefault(x => x.Description == "人员");
                if (fileinfo != null)
                {
                    entity.Scores = BSFramework.Util.Config.GetValue("AppUrl") + fileinfo.FilePath.Substring(2, fileinfo.FilePath.Length - 2);
                }
            }

            return new ModelBucket<PeopleEntity>() { Data = entity, Success = true };// new { code = 0, info = "获取数据成功", data = entity };
        }

        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerMonitor(6, "安卓平板删除成员")]
        public object Delete([FromBody] JObject json)
        {
            var currentUser = OperatorProvider.Provider.Current();
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                NLog.LogManager.GetCurrentClassLogger().Info("新增人员，{0}，{1}", currentUser.UserId, dy);

                UserBLL userbll = new UserBLL();
                string userId = dy.data;
                UserEntity user = userbll.GetEntity(userId);


                var url = Config.GetValue("ErchtmsApiUrl");
                var dict = new Dictionary<string, string>
                {
                    {"account", user.Account },
                    {"json", Newtonsoft.Json.JsonConvert.SerializeObject(user) }
                };
                var content = new FormUrlEncodedContent(dict);
                var client = new HttpClient();
                var response = client.PostAsync($"{url}syncdata/DeleteUser?keyvalue={user.UserId}", content).Result;


                //e.ErchtmsSynchronoous("DeleteUser", user, user.Account);

                pbll.RemoveForm(userId);
                ubll.RemoveForm(userId);
                var list = pbll.GetLogins().Where(x => x.UserId == userId);
                foreach (LoginInfo l in list)
                {
                    pbll.dellogin(l);
                }
                //var entity = pbll.GetEntity(userId);
                return new { code = 0, info = "删除数据成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 修改成员
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Edit()
        {
            var currentUser = OperatorProvider.Provider.Current();
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
            NLog.LogManager.GetCurrentClassLogger().Info("修改人员，{0}，{1}", currentUser.UserId, dy);

            string json = JsonConvert.SerializeObject(dy.data.userEntity);
            PeopleEntity model = JsonConvert.DeserializeObject<PeopleEntity>(json);
            UserEntity user = ubll.GetEntity(model.ID);

            var identitys = pbll.CheckTel1(model.LinkWay.Trim(), model.ID);
            if (identitys > 0)
            {
                return new { code = 1, info = "该手机号已存在！", data = new { } };
            }
            string path = "";
            FileInfoBLL fileBll = new FileInfoBLL();
            HttpFileCollection nfiles = HttpContext.Current.Request.Files;
            FileInfoEntity fi = null;
            bool isuploadsign = false;
            bool isuploadhead = false;
            //修改照片信息
            for (int i = 0; i < nfiles.Count; i++)
            {
                HttpPostedFile hf = nfiles[i];
                var key = nfiles.AllKeys[i].ToString();
                string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                string fileId = Guid.NewGuid().ToString();//上传后文件名
                fi = new FileInfoEntity
                {
                    FileId = fileId,
                    FolderId = model.ID,
                    RecId = model.ID,
                    FileName = System.IO.Path.GetFileName(hf.FileName),
                    FilePath = "~/Resource/PhotoFile/" + fileId + ext,
                    FileType = System.IO.Path.GetExtension(hf.FileName),
                    FileExtensions = ext,
                    FileSize = hf.ContentLength.ToString(),
                    Description = key,
                    DeleteMark = 0
                };

                //上传附件到服务器
                if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\PhotoFile"))
                {
                    System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\PhotoFile");
                }
                hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\PhotoFile\\" + fileId + ext);
                //保存附件信息
                fileBll.SaveForm(fi);
                string url = BSFramework.Util.Config.GetValue("AppUrl") + fi.FilePath.Substring(2, fi.FilePath.Length - 2);
                if (key == "Photo")
                {
                    model.Photo = fi.FilePath;
                    path = BSFramework.Util.Config.GetValue("FilePath") + "\\PhotoFile\\" + fileId + ext; isuploadhead = true;
                }
                if (key == "Signature") { model.Signature = url; path = BSFramework.Util.Config.GetValue("FilePath") + "\\PhotoFile\\" + fileId + ext; isuploadsign = true; }
            }

            //刷新二维码
            var files = fileBll.GetPeoplePhoto(model.ID).ToList();
            if (files.Count() == 0)  //file表中无二维码信息
            {
                var file = this.BuildImage(model, "人员");
                model.Files.Add(file);
            }
            else
            {
                var file = fileBll.GetEntity(files[0].FileId);
                string npath = "";
                if (file != null)
                {
                    npath = System.Web.HttpContext.Current.Server.MapPath(file.FilePath);
                    if (!System.IO.File.Exists(npath))
                    {
                        var newfile = this.BuildImage(model, "人员");
                        model.Files.Add(newfile);
                    }
                }

            }
            //model.Planer = getplaner(model.Quarters, user.OrganizeId);
            pbll.SaveForm(model.ID, model);
            //修改user表信息
            if (user != null)
            {
                user.RealName = model.Name;
                if (model.Sex == "男")
                {
                    user.Gender = 1;
                }
                else
                {
                    user.Gender = 0;
                }
                user.IsEpiboly = model.IsEpiboly;
                //user.Account = model.LinkWay;
                user.Mobile = model.LinkWay;
                user.DutyName = model.RoleDutyName;
                user.DutyId = model.RoleDutyId;
                user.IDENTIFYID = model.IdentityNo;
                user.EnterTime = model.EntryDate;
                user.Birthday = model.Birthday;
                user.RoleId = model.RoleId;
                user.RoleName = model.RoleName;

                user.Secretkey = null;
                user.Password = null;

                //先保存user信息，同步时重置roleid
                //人员同步角色
                //var role1 = rbll.GetList().Where(x => x.FullName == "班组成员").FirstOrDefault();
                //var role2 = rbll.GetList().Where(x => x.FullName == "班组长").FirstOrDefault();
                //if (model.RoleDutyName == "班长" || model.RoleDutyName == "班组长")
                //{
                //    user.RoleId = role2.RoleId;
                //    user.RoleName = role2.FullName;
                //}
                //else
                //{
                //    user.RoleId = role1.RoleId;
                //    user.RoleName = role1.FullName;
                //}
                //ubll.SaveForm(model.ID, user);

                //user.RoleId = model.RoleDutyId;
                //user.RoleName = model.RoleDutyName;

                user.LabourNo = model.LabourNo;
                user.Craft = model.WorkKind;
                user.Folk = model.Folk;
                user.CurrentWorkAge = model.CurrentWorkAge;
                user.OldDegree = model.OldDegree;
                user.NewDegree = model.NewDegree;
                user.Quarters = model.Quarters;
                user.Planer = model.Planer;
                user.Visage = model.Visage;
                user.Age = model.Age;
                user.Native = model.Native;
                user.TechnicalGrade = model.TecLevel;
                user.JobName = model.JobName;
                user.HealthStatus = model.HealthStatus;
                user.IsSpecial = model.IsSpecial;
                user.IsSpecialEqu = model.IsSpecialEquipment;
                user.EnterTime = model.EntryDate;
                user.SpecialtyType = model.SpecialtyType;
                user.IsEpiboly = model.IsSpecial;
                user.IsFourPerson = model.IsFourPerson;
                //user.HeadIcon = model.Photo;
                if (isuploadsign)
                {
                    user.SignImg = ToBase64String(path);
                }
                else
                {
                    user.SignImg = null;
                }
                if (isuploadhead)
                {
                    user.HeadIcon = ToBase64String(path);
                }
                else
                {
                    user.HeadIcon = null;
                }

                var url = Config.GetValue("ErchtmsApiUrl");
                //var dict = new Dictionary<string, string>
                //{
                //    {"account", currentUser.Account },
                //    {"json", Newtonsoft.Json.JsonConvert.SerializeObject(user) }
                //};
                //var content = new FormUrlEncodedContent(dict);
                //var client = new HttpClient();
                //var response = client.PostAsync($"{url}syncdata/SaveUser?keyvalue={user.UserId}", content).Result.Content.ReadAsStringAsync().Result;

                var syncUrl = Config.GetValue("ErchtmsApiUrl");
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(currentUser.Account), "account");
                content.Add(new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(user)), "json");
                var client = new HttpClient();
                var response = client.PostAsync($"{syncUrl}syncdata/SaveUser?keyvalue={user.UserId}", content).Result.Content.ReadAsStringAsync().Result;
                NLog.LogManager.GetCurrentClassLogger().Info(response);


                //e.ErchtmsSynchronoous("SaveUser", user, user.Account);
                ubll.SaveForm(user.UserId, user);
            }
            string face = dy.data.face;
            string fingers = dy.data.fingers;
            LoginInfo l = pbll.GetLogins().Where(x => x.UserId == model.ID).FirstOrDefault();
            if (l != null)
            {
                if (!string.IsNullOrEmpty(face))
                    l.Face = face;
                if (!string.IsNullOrEmpty(fingers))
                    l.Finger = fingers;
                pbll.SaveLoginInfo(l.ID, l);
            }
            else
            {
                l = new LoginInfo();
                l.ID = Guid.NewGuid().ToString();
                l.IdentityNo = model.IdentityNo;
                l.LabourNo = model.LabourNo;
                l.Finger = fingers;
                l.Face = face;
                l.UserId = model.ID;
                pbll.SaveLoginInfo(l.ID, l);
            }
            return new { code = 0, info = "修改数据成功", data = new { } };
        }

        /// <summary>
        /// 获取岗位信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetQuarters([FromBody] JObject json)
        {
            PostBLL pbll = new PostBLL();
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                var user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                var data = pbll.GetQuartersList(dept.OrganizeId);

                var list = data.Select(x => new
                {
                    Text = x.FullName,
                    Value = x.EnCode
                }).ToList();
                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取省份
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetProvins()
        {
            AreaBLL areaBLL = new AreaBLL();
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                var data = areaBLL.GetAreaList("0");

                var list = data.Select(x => new
                {
                    Text = x.AreaName,
                    Value = x.AreaName
                }).ToList();
                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        [HttpPost]
        public object GetQuartersNew(ListModel model)
        {
            PostBLL pbll = new PostBLL();
            try
            {
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = model.userId;
                var user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                var data = pbll.GetQuartersList(dept.DepartmentId);
                while (data.Count() == 0 && dept != null)
                {
                    data = pbll.GetQuartersList(dept.ParentId);
                    dept = dtbll.GetEntity(dept.ParentId);
                }

                var list = data.Select(x => new
                {
                    Text = x.FullName,
                    Value = x.FullName
                }).ToList();
                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        public List<RoleSyncModel> getNewData(string userid, string deptid)
        {
            var dict = new
            {
                data = deptid,
                userid = userid,
                tokenid = ""
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Post", "GetPostByDeptId"), "json=" + JsonConvert.SerializeObject(dict));
            var ret = JsonConvert.DeserializeObject<RetDataModel>(res);
            return JsonConvert.DeserializeObject<List<RoleSyncModel>>(ret.data.ToString());
        }

        public List<RoleEntity> getSKQuarters(string userid, string deptid)
        {
            var dict = new
            {
                //data = "4ad1d7d0-50af-4966-8c48-48570075f009",
                //userid = "System",
                //tokenid = ""
                data = deptid,
                userid = userid,
                tokenid = ""
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Post", "GetJobByDeptId"), "json=" + JsonConvert.SerializeObject(dict));
            var ret = JsonConvert.DeserializeObject<RetDataModel>(res);
            List<RoleEntity> data = JsonConvert.DeserializeObject<List<RoleEntity>>(ret.data.ToString());
            data = data.OrderBy(x => x.EnCode).ToList();
            return data;

            //DataItemBLL dataItemBLL = new DataItemBLL();
            //DataItemDetailBLL dataItemDetailBLL = new DataItemDetailBLL();
            //PostBLL pbll = new PostBLL();
            //JobBLL jbll = new JobBLL();


            //var user = OperatorProvider.Provider.Current();
            //var data = pbll.GetQuartersList(user.OrganizeId).OrderBy(x => x.EnCode).ToList();
            //return data;

        }
        [HttpPost]
        public object GetRoleDuty([FromBody] JObject json)
        {
            PostBLL pbll = new PostBLL();
            PeopleBLL peobll = new PeopleBLL();
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                var user = ubll.GetEntity(userId);
                var dept = dtbll.GetEntity(user.DepartmentId);

                var deptid = dept.DepartmentId;
                //if (dept.Nature == "班组") deptid = dept.ParentId;
                var data = getNewData(user.UserId, deptid);

                return new { code = 0, info = "获取数据成功", data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        [HttpPost]
        public object GetSKQuarters([FromBody] JObject json)
        {
            PostBLL pbll = new PostBLL();
            PeopleBLL peobll = new PeopleBLL();
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                var user = ubll.GetEntity(userId);
                var dept = dtbll.GetEntity(user.DepartmentId);

                var deptid = dept.DepartmentId;
                //if (dept.Nature == "班组") deptid = dept.ParentId;
                var data = getSKQuarters(user.UserId, deptid);
                var list = data.Select(x => new
                {
                    Id = x.RoleId,
                    Text = x.FullName,
                    Value = x.EnCode
                }).ToList();
                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        public object GetDutyContent([FromBody] JObject json)
        {
            PostBLL pobll = new PostBLL();
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                PeopleEntity p = pbll.GetEntity(userId);
                var content = "";
                var content1 = "";
                var duty = pbll.GetDutyEntityByRole(p.RoleDutyId);
                var dutydanger = pbll.GetDutyDangerEntityByRole(p.RoleDutyId);
                if (duty != null) content = duty.DutyContent;
                if (dutydanger != null) content1 = dutydanger.DutyContent;
                return new { code = 0, info = "获取数据成功", data = new { DutyContent = content, DutyDangerContent = content1 } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetDept([FromBody] JObject json)
        {
            JobBLL jbll = new JobBLL();
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                UserEntity u = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(u.DepartmentId);
                DepartmentEntity pdept = dtbll.GetEntity(dept.ParentId);

                return new { code = 0, info = "获取数据成功", data = new { bz = dept.FullName, depart = pdept.FullName } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取职称信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetJobs([FromBody] JObject json)
        {
            JobBLL jbll = new JobBLL();
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                var user = ubll.GetEntity(userId);
                var data = jbll.GetJobList(user.OrganizeId);
                var list = data.Select(x => new
                {
                    Text = x.FullName,
                    Value = x.FullName
                }).ToList();
                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        [HttpPost]
        public object GetFlow([FromBody] JObject json)
        {
            JobBLL jbll = new JobBLL();
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                var flow = ditem.GetEntityByCode("Nation");
                List<DataItemDetailEntity> data = detail.GetList(flow.ItemId).ToList();
                var list = data.Select(x => new
                {
                    Text = x.ItemName,
                    Value = x.ItemName
                }).ToList();
                return new { code = 0, info = "获取数据成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 根据部门ID得到部门下的用户信息(20190925 sx 用于班组终端对接培训平台时，部门级用户看台账使用 未录入Yapi)
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetUserByDeptId(string deptId)
        {
            if (string.IsNullOrEmpty(deptId))
            {
                return new { code = 1, info = "部门ID不能为空" };
            }
            IEnumerable<UserEntity> users = ubll.GetDeptUsers(deptId);
            if (users != null && users.Count() > 0)
            {
                return new { code = 0, info = "", data = users.FirstOrDefault() };
            }
            return new { code = 1, info = "当前部门下没有人员信息", data = "" };
        }
        /// <summary>
        /// 新增成员
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Add()
        {
            var currentUser = OperatorProvider.Provider.Current();

            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

            NLog.LogManager.GetCurrentClassLogger().Info("新增人员，{0}，{1}", currentUser.UserId, dy);

            string userId = dy.userId;
            string fingers = dy.data.fingers;
            string face = dy.data.face;
            UserEntity loginuser = ubll.GetEntity(userId);
            string userEntity = JsonConvert.SerializeObject(dy.data.userEntity);
            PeopleEntity model = JsonConvert.DeserializeObject<PeopleEntity>(userEntity);
            string id = Guid.NewGuid().ToString();
            //验证身份证
            int i = pbll.CheckIdentity(model.IdentityNo);
            if (i != 0)
            {
                return new { code = 3, info = "身份证已存在！", data = new { } };
            }
            //验证手机号
            int t = pbll.CheckTel(model.LinkWay);
            if (t != 0)
            {
                return new { code = 4, info = "手机号已存在！", data = new { } };
            }
            if (model.LabourNo != "")
            {
                //验证工号
                int l = pbll.CheckNo(model.LabourNo);
                if (l != 0)
                {
                    return new { code = 4, info = "工号已存在！", data = new { } };
                }
            }
            UserEntity users = new UserBLL().GetEntity(userId);
            model.ID = id;
            //技术等级新增证书
            if (!string.IsNullOrEmpty(model.TecLevel))
            {
                addCertificate(users, model.ID, "职业资格证书-" + model.TecLevel);
            }
            //职称新增证书
            if (!string.IsNullOrEmpty(model.JobName))
            {
                addCertificate(users, model.ID, "专业技术资格证(职称)-" + model.JobName);
            }
            model.UserAccount = model.UserAccount;
            model.PassWord = "Abc123456";
            //model.PassWord = model.IdentityNo.Substring(model.IdentityNo.Length - 4, 4);
            var file = this.BuildImage(model, "人员");
            model.Files.Add(file);
            string path = "";
            FileInfoBLL fileBll = new FileInfoBLL();
            HttpFileCollection files = HttpContext.Current.Request.Files;
            FileInfoEntity fi = null;
            bool isuploadsign = false;
            bool isuploadhead = false;
            for (int j = 0; j < files.Count; j++)
            {
                HttpPostedFile hf = files[j];
                var key = files.AllKeys[j].ToString();
                string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                string fileId = Guid.NewGuid().ToString();//上传后文件名
                fi = new FileInfoEntity
                {
                    FileId = fileId,
                    FolderId = model.ID,
                    RecId = model.ID,
                    FileName = System.IO.Path.GetFileName(hf.FileName),
                    FilePath = "~/Resource/PhotoFile/" + fileId + ext,
                    FileType = System.IO.Path.GetExtension(hf.FileName),
                    FileExtensions = ext,
                    FileSize = hf.ContentLength.ToString(),
                    Description = key,
                    DeleteMark = 0
                };

                //上传附件到服务器
                if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\PhotoFile"))
                {
                    System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\PhotoFile");
                }
                hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\PhotoFile\\" + fileId + ext);
                //保存附件信息
                fileBll.SaveForm(fi);
                string url = BSFramework.Util.Config.GetValue("AppUrl") + fi.FilePath.Substring(2, fi.FilePath.Length - 2);
                if (key == "Photo")
                {
                    model.Photo = fi.FilePath;
                    path = BSFramework.Util.Config.GetValue("FilePath") + "\\PhotoFile\\" + fileId + ext;
                    isuploadhead = true;
                }
                if (key == "Signature")
                {
                    model.Signature = url;
                    path = BSFramework.Util.Config.GetValue("FilePath") + "\\PhotoFile\\" + fileId + ext;
                    isuploadsign = true;
                }
            }
            //model.Planer = getplaner(model.Quarters,loginuser.OrganizeId);
            model.FingerMark = "yes";
            //model.Photo=ViewData["uploadPreview"]

            //user表信息
            UserEntity user = new UserEntity();
            user.Account = model.UserAccount;
            //user.Password = model.IdentityNo.Substring(model.IdentityNo.Length - 4, 4);
            user.Password = "Abc123456";
            DepartmentEntity dept = new DepartmentEntity();

            dept = dtbll.GetEntity(loginuser.DepartmentId);
            DepartmentEntity pdept = dtbll.GetEntity(dept.ParentId);
            user.DepartmentId = dept.DepartmentId;
            user.DepartmentCode = dept.EnCode;
            user.Mobile = model.LinkWay;
            if (model.Sex == "男")
            {
                user.Gender = 1;
            }
            else
            {
                user.Gender = 0;
            }

            user.RoleId = model.RoleDutyId;
            user.RoleName = model.RoleDutyName;

            user.UserId = model.ID; //用户表关联
            user.RealName = model.Name;
            user.OrganizeId = pdept.OrganizeId;
            user.LogOnCount = 0;
            user.PreviousVisit = DateTime.Now;
            user.UserOnLine = 1;
            if (ubll.ExistAccount(user.Account, ""))
            {

                model.BZID = dept.DepartmentId;
                model.BZName = dept.FullName;
                model.BZCode = dept.EnCode;
                model.DeptId = pdept.DepartmentId;
                model.DeptName = pdept.FullName;
                model.DeptCode = pdept.EnCode;
                pbll.SaveForm(model.ID, model);

                //同步人员
                user.DutyName = model.RoleDutyName;
                user.DutyId = model.RoleDutyId;
                user.EnterTime = model.EntryDate;
                user.Birthday = model.Birthday;


                user.CreateUserId = loginuser.UserId;
                user.CreateUserName = loginuser.RealName;
                user.IDENTIFYID = model.IdentityNo;


                user.LabourNo = model.LabourNo;
                user.Craft = model.WorkKind;
                user.Folk = model.Folk;
                user.CurrentWorkAge = model.CurrentWorkAge;
                user.OldDegree = model.OldDegree;
                user.NewDegree = model.NewDegree;
                user.Quarters = model.Quarters;
                user.Planer = model.Planer;
                user.Visage = model.Visage;
                user.Age = model.Age;
                user.Native = model.Native;
                user.TechnicalGrade = model.TecLevel;
                user.JobName = model.JobName;
                user.HealthStatus = model.HealthStatus;
                user.IsSpecial = model.IsSpecial;
                user.IsSpecialEqu = model.IsSpecialEquipment;
                user.EnterTime = model.EntryDate;
                user.WorkKind = model.WorkKind;
                user.TecLevel = model.TecLevel;
                //user.HeadIcon = model.Photo;
                if (isuploadsign)
                {
                    user.SignImg = ToBase64String(path);
                }
                else
                {
                    user.SignImg = null;
                }
                if (isuploadhead)
                {
                    user.HeadIcon = ToBase64String(path);
                }
                else
                {
                    user.HeadIcon = null;
                }



                var syncUrl = Config.GetValue("ErchtmsApiUrl");
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(currentUser.Account), "account");
                content.Add(new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(user)), "json");
                var client = new HttpClient();
                var response = client.PostAsync($"{syncUrl}syncdata/SaveUser?keyvalue={user.UserId}", content).Result.Content.ReadAsStringAsync().Result;
                NLog.LogManager.GetCurrentClassLogger().Info(response);




                //e.ErchtmsSynchronoous("SaveUser", user, loginuser.Account);
                user.RoleId = model.RoleId;
                user.RoleName = model.RoleName;
                user.HeadIcon = model.Photo;
                ////同步完成之后，重置roleid
                //var role1 = rbll.GetList().Where(x => x.FullName == "班组成员").FirstOrDefault();
                //var role2 = rbll.GetList().Where(x => x.FullName == "班组长").FirstOrDefault();
                //if (model.RoleDutyName == "班长" || model.RoleDutyName == "副班长" || model.RoleDutyName == "技术员")
                //{
                //    user.RoleId = role2.RoleId;
                //    user.RoleName = role2.FullName;
                //}
                //else
                //{
                //    user.RoleId = role1.RoleId;
                //    user.RoleName = role1.FullName;
                //}
                user.IsEpiboly = model.IsEpiboly;
                ubll.InsertUser(user);

                LoginInfo l = new LoginInfo();
                l.ID = Guid.NewGuid().ToString();
                l.IdentityNo = model.IdentityNo;
                l.LabourNo = model.LabourNo;
                l.Finger = fingers;
                l.Face = face;
                l.UserId = model.ID;
                pbll.SaveLoginInfo(l.ID, l);
                var url = BSFramework.Util.Config.GetValue("AppUrl") + file.FilePath.Substring(2, file.FilePath.Length - 2);
                var password = model.PassWord;
                return new { code = 0, info = "新增成功", data = new { UserId = user.UserId, url = url, password = password } };
            }
            else
            {
                return new { code = 2, info = "新增失败，账号已存在！", data = new { } };
            }
        }
        /// <summary>
        /// 人员信息新增证书
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="CertificateName"></param>
        private void addCertificate(UserEntity user, string userId, string CertificateName)
        {
            string newpath = string.Empty;//数据库路径
            string id = string.Empty;//Guid
            GetGuId(userId, "人员证书", out newpath, out id);
            UserCertificateEntity entity = new UserCertificateEntity();
            entity.Id = id;
            entity.userid = userId;
            entity.CertificateName = CertificateName;
            entity.createtime = DateTime.Now;
            entity.createuser = user.RealName;
            entity.createuserid = user.UserId;
            UserCertificateBLL ucbll = new UserCertificateBLL();
            ucbll.addUserCertificate(entity);
        }
        private void GetGuId(string userId, string type, out string newpath, out string id)
        {
            try
            {
                FileInfoBLL fb = new FileInfoBLL();
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
                    RecId = id,
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
            catch (Exception ex)
            {

                throw;
            }
        }
        public static string getplaner(string quarter, string organizeid)
        {
            string val = "";
            PostBLL pbll = new PostBLL();
            JobBLL jbll = new JobBLL();
            var data = pbll.GetQuartersList(organizeid);
            var role = data.Where(x => x.FullName == quarter).FirstOrDefault();
            if (role != null) val = role.EnCode;
            //if (!string.IsNullOrEmpty(quarter))
            //{
            //    switch (quarter)
            //    {
            //        case "班长":
            //            val = "00";
            //            break;
            //        case "副班长":
            //            val = "01";
            //            break;
            //        case "技术员":
            //            val = "02";
            //            break;
            //        case "安全员":
            //            val = "03";
            //            break;
            //        case "培训员":
            //            val = "04";
            //            break;
            //        case "劳动保护监督员":
            //            val = "05";
            //            break;
            //        case "政治宣传员":
            //            val = "06";
            //            break;
            //        case "成本控制员":
            //            val = "07";
            //            break;
            //        case "考勤员":
            //            val = "08";
            //            break;
            //        case "领料员":
            //            val = "09";
            //            break;
            //        case "其他成员":
            //            val = "10";
            //            break;
            //        default:
            //            break;
            //    }
            //}
            return val;
        }
        private FileInfoEntity BuildImage(PeopleEntity user, string type)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            var flist = fileBll.GetPeoplePhoto(user.ID);
            foreach (FileInfoEntity f in flist)
            {
                fileBll.Delete(f.FileId);
            }
            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode(user.ID + "|" + type, Encoding.UTF8);
            var path = "~/Resource/DocumentFile/";
            string rpath = BSFramework.Util.Config.GetValue("FilePath") + "\\DocumentFile\\";
            if (!Directory.Exists(rpath))
                Directory.CreateDirectory(rpath);
            image.Save(Path.Combine(rpath, id + ".jpg"));



            return new FileInfoEntity() { FileId = id, CreateDate = DateTime.Now, CreateUserId = user.ID, CreateUserName = user.Name, Description = type, FileExtensions = ".jpg", FileName = id + ".jpg", FilePath = path + id + ".jpg", FileType = "jpg", ModifyDate = DateTime.Now, ModifyUserId = user.ID, ModifyUserName = user.Name, RecId = user.ID };
        }
        private FileInfoEntity NewBuildImage(PeopleDutyEntity duty, string type)
        {
            FileInfoBLL fileBll = new FileInfoBLL();

            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode(duty.ID + "|" + type, Encoding.UTF8);
            var path = "~/Resource/DocumentFile/";
            string rpath = BSFramework.Util.Config.GetValue("FilePath") + "\\DocumentFile\\";
            if (!Directory.Exists(rpath))
                Directory.CreateDirectory(rpath);
            image.Save(Path.Combine(rpath, id + ".jpg"));



            return new FileInfoEntity() { FileId = id, CreateDate = DateTime.Now, CreateUserId = duty.CreateUserId, Description = type, FileExtensions = ".jpg", FileName = id + ".jpg", FilePath = path + id + ".jpg", FileType = "jpg", ModifyDate = DateTime.Now, RecId = duty.ID };
        }

        [HttpPost]
        public object Reset([FromBody] JObject json)
        {
            JobBLL jbll = new JobBLL();
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                string pwd = "Abc123456";
                if (dy.data != null && !string.IsNullOrEmpty(dy.data)) pwd = dy.data;
                string md5pwd = Md5Helper.MD5(pwd, 32).ToLower();
                ubll.RevisePassword(userId, md5pwd.ToLower());

                string[] strList = new string[] { userId, pwd };


                var syncUrl = Config.GetValue("ErchtmsApiUrl");
                var dict = new Dictionary<string, string>
                {
                    {"userId", userId },
                    {"pwd", pwd }
                };
                var content = new FormUrlEncodedContent(dict);
                var client = new HttpClient();
                var response = client.PostAsync($"{syncUrl}syncdata/UpdatePwd", content).Result;




                //e.ErchtmsSynchronoous("UpdatePwd", strList, "");

                return new { code = 0, info = "获取数据成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        #region 责任书
        /// <summary>
        /// 初始化数据，当年数据（类型及责任书数据）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object CreateDutys()
        {
            try
            {
                FileInfoBLL fileBll = new FileInfoBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                PeopleEntity people = pbll.GetEntity(userId);

                if (people == null) return new { code = 0, info = "登陆用户错误，无法获取此班组成员信息！", data = new { } };

                DepartmentEntity dept = dtbll.GetEntity(people.BZID);

                //若岗位为null，筛选会报错
                var list = pbll.GetListByDept(people.BZID).Where(x => x.Quarters != null).ToList();

                PeopleEntity bzz = list.Where(x => x.Quarters.Contains("班长") || x.Quarters.Contains("班组长")).FirstOrDefault();
                //若无班长，bzz=null
                if (bzz == null) bzz = new PeopleEntity();

                PeopleDutyEntity entity = new PeopleDutyEntity();
                var year = DateTime.Now.Year;
                var nyear = "";

                //循环类型，区分班组（20190708-未区分班组，造成初始化数据多条）
                var tlist = pbll.GetDutyTypes().Where(x => x.BZID == dept.DepartmentId).ToList();
                PeopleDutyTypeEntity o = tlist.Where(x => x.BZID == dept.DepartmentId && x.Name == "安全生产责任书").FirstOrDefault();
                //初始化该班组责任书类型数据
                if (o == null)
                {
                    o = new PeopleDutyTypeEntity();
                    o.ID = Guid.NewGuid().ToString();
                    o.Name = "安全生产责任书";
                    o.BZID = dept.DepartmentId;
                    o.BZName = dept.FullName;
                    o.CreateDate = DateTime.Now;
                    pbll.SaveDutyType(o);
                    tlist.Add(o);
                }

                //查询已有责任书数据，关联类型id
                var dlist = pbll.GetPeopleDutyList(dept.DepartmentId).Where(x => x.TypeId == null);
                foreach (PeopleDutyEntity duty in dlist)
                {
                    duty.TypeId = o.ID;
                    pbll.SavePeopleDuty(duty.ID, duty);
                }

                //初始化责任书数据 (新增类型，到第二年重新生成)
                foreach (PeopleDutyTypeEntity t in tlist)
                {
                    foreach (PeopleEntity p in list)
                    {

                        for (int y = 0; y < 1; y++)
                        {
                            nyear = (year - y).ToString();
                            entity = pbll.GetPeopleDuty(p.ID, nyear, t.ID);
                            if (entity == null)
                            {
                                entity = new PeopleDutyEntity();
                                entity.ID = Guid.NewGuid().ToString();
                                entity.CreateDate = DateTime.Now;
                                entity.SignDate = DateTime.Now;
                                entity.Year = nyear + "年";
                                entity.Name = t.Name;
                                entity.TypeId = t.ID;
                                entity.TypeName = t.Name;
                                entity.PeopleId = p.ID;
                                entity.DutyMan = p.Name;
                                if (p.Quarters == "班长") { entity.ParentDutyMan = ""; }
                                else
                                {
                                    entity.ParentDutyMan = bzz.Name;
                                }
                                entity.State = "0";
                                entity.BZId = p.BZID;
                                entity.DeptId = p.DeptId;
                                pbll.SavePeopleDuty(null, entity);
                                fileBll.SaveForm(this.NewBuildImage(entity, "责任书二维码"));
                            }
                        }
                    }
                }
                return new { code = 0, info = "新增成功", data = new { } };

            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 查看责任书详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetDutyInfo()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                string id = dy.data;
                PeopleDutyEntity entity = pbll.GetPeopleDuty(id);
                var url = BSFramework.Util.Config.GetValue("AppUrl");

                entity.Files = fileInfoBLL.GetFilesByRecIdNew(entity.ID).Where(x => x.Description != "责任书二维码").ToList();
                foreach (FileInfoEntity f in entity.Files)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                }
                entity.qr = fileInfoBLL.GetFilesByRecIdNew(entity.ID).Where(x => x.Description == "责任书二维码").FirstOrDefault().FilePath.Replace("~/", url);


                return new { code = 0, info = "成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        public object GetDutyInfoNew()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                string year = dy.data.year;
                string typeid = dy.data.typeId;
                PeopleDutyEntity entity = pbll.GetPeopleDuty(userId, year, typeid);
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                //为空处理
                if (entity == null) entity = new PeopleDutyEntity();

                entity.Files = fileInfoBLL.GetFilesByRecIdNew(entity.ID).Where(x => x.Description != "责任书二维码").ToList();
                foreach (FileInfoEntity f in entity.Files)
                {
                    f.FilePath = f.FilePath.Replace("~/", url);
                }
                entity.qr = fileInfoBLL.GetFilesByRecIdNew(entity.ID).Where(x => x.Description == "责任书二维码").FirstOrDefault().FilePath.Replace("~/", url);


                return new { code = 0, info = "成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 编辑责任书
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object EditDuty()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                string dutyEntity = JsonConvert.SerializeObject(dy.data.dutyEntity);
                PeopleDutyEntity entity = JsonConvert.DeserializeObject<PeopleDutyEntity>(dutyEntity);
                entity.State = "1";


                pbll.SavePeopleDuty(entity.ID, entity);

                string fids = dy.data.fileIds;
                if (!string.IsNullOrEmpty(fids))
                {
                    string[] ids = fids.Split(',');
                    foreach (string id in ids)
                    {
                        fileInfoBLL.Delete(id);
                    }
                }
                FileInfoBLL fileBll = new FileInfoBLL();
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/PeopleDuty/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\PeopleDuty"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\PeopleDuty");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\PeopleDuty\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                return new { code = 0, info = "成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取成员列表（责任书用）
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetPeopleDutys([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);


                string userId = dy.userId;
                string year = dy.data.year;
                string typeid = dy.data.typeId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                int total = 0;
                IList<PeopleEntity> nlist = new List<PeopleEntity>();
                var list = pbll.GetListByDept(user.DepartmentId).ToList();
                //string[] property = new string[] { "Planer", "Name" };
                //bool[] sort = new bool[] { true, true };

                //list = new IListSort<PeopleEntity>(list, property, sort).Sort().ToList();
                var list1 = pbll.GetPeopleDutyList(user.DepartmentId).Where(x => x.Year.Contains(year) && x.State == "1" && x.TypeId == typeid).ToList();
                double percent = 0;
                if (list1.Count > 0 && list.Count != 0)
                {
                    percent = Math.Round((double)list1.Count / list.Count * 100, 2);
                }
                foreach (PeopleEntity p in list)
                {
                    var pd = pbll.GetPeopleDuty(p.ID, year, typeid);
                    if (pd != null)
                    {
                        p.Remark = pd.State;
                    }
                }
                list = list.OrderBy(x => x.Remark).ToList();
                //nlist.Union(list.Where(x => x.Remark == "0"));
                //nlist.Union(list.Where(x => x.Remark == "1"));
                //nlist.Insert(0, list.Where(x => x.Quarters == "班长").FirstOrDefault());
                var bz = list.Where(x => x.Quarters == "班长").FirstOrDefault();
                if (bz != null)
                {
                    list.Remove(bz);
                    list.Insert(0, bz);
                }
                return new { code = 0, info = "获取数据成功", count = list.Count, data = new { data = list, count1 = list.Count, count2 = list1.Count, percent = percent } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取成员责任书列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetDutysByUser([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);


                string userId = dy.userId;
                string year = dy.data.year;
                string typeid = dy.data.typeId;
                string yearn = dy.data.yearn;
                string typeidn = dy.data.typeIdn;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                var list = pbll.GetPeopleDutyList(user.DepartmentId).Where(x => x.PeopleId == userId).OrderBy(x => x.CreateDate).ToList();
                if (!string.IsNullOrEmpty(year)) list = list.Where(x => x.Year.Contains(year)).ToList();
                if (!string.IsNullOrEmpty(typeid)) list = list.Where(x => x.TypeId == typeid).ToList();

                if (!string.IsNullOrEmpty(yearn) && !string.IsNullOrEmpty(typeidn))
                {
                    var pd = list.Where(x => x.Year.Contains(yearn) && x.TypeId == typeidn).FirstOrDefault();
                    if (pd != null)
                    {
                        bool b = list.Remove(pd);
                        list.Insert(0, pd);
                    }
                }
                return new { code = 0, info = "获取数据成功", count = list.Count(), data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 新增/编辑 责任书类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public object SaveDutyType(DutyTypeModel model)
        {
            try
            {

                FileInfoBLL fileBll = new FileInfoBLL();

                string userId = model.userId;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                PeopleDutyTypeEntity typeentity = model.data;
                //数据重复验证
                var tlist = pbll.GetDutyTypes().Where(x => x.Name == typeentity.Name && x.BZID == dept.DepartmentId);
                if (tlist.Count() > 0) return new { code = 0, info = "该类型已存在！" };

                //新增类型，初始化数据
                if (string.IsNullOrEmpty(typeentity.ID))
                {
                    typeentity.ID = Guid.NewGuid().ToString();
                    typeentity.BZID = dept.DepartmentId;
                    typeentity.BZName = dept.FullName;
                    typeentity.CreateDate = DateTime.Now;

                    PeopleEntity bzz = new PeopleEntity();
                    var list = pbll.GetListByDept(user.DepartmentId).ToList();
                    bzz = list.Where(x => x.Quarters.Contains("班长") || x.Quarters.Contains("班组长")).FirstOrDefault();

                    PeopleDutyEntity entity = new PeopleDutyEntity();
                    var year = DateTime.Now.Year;
                    var nyear = "";

                    foreach (PeopleEntity p in list)
                    {

                        for (int y = 0; y < 1; y++)
                        {
                            nyear = (year - y).ToString();
                            entity = pbll.GetPeopleDuty(p.ID, nyear, typeentity.ID);
                            if (entity == null)
                            {
                                entity = new PeopleDutyEntity();
                                entity.ID = Guid.NewGuid().ToString();
                                entity.CreateDate = DateTime.Now;
                                entity.SignDate = DateTime.Now;
                                entity.Year = nyear + "年";
                                entity.Name = typeentity.Name;
                                entity.PeopleId = p.ID;
                                entity.DutyMan = p.Name;
                                entity.TypeId = typeentity.ID;
                                entity.TypeName = typeentity.Name;
                                if (p.Quarters == "班长") { entity.ParentDutyMan = ""; }
                                else
                                {
                                    entity.ParentDutyMan = bzz.Name;
                                }
                                entity.State = "0";
                                entity.BZId = p.BZID;
                                entity.DeptId = p.DeptId;
                                pbll.SavePeopleDuty(null, entity);
                                fileBll.SaveForm(this.NewBuildImage(entity, "责任书二维码"));
                            }
                        }
                    }

                }
                else
                {
                    var list = pbll.GetPeopleDutyList(user.DepartmentId).Where(x => x.TypeId == typeentity.ID);
                    foreach (PeopleDutyEntity pd in list)
                    {
                        pd.TypeName = typeentity.Name;
                        pbll.SavePeopleDuty(pd.ID, pd);
                    }
                }

                pbll.SaveDutyType(typeentity);

                return new { code = 0, info = "成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 删除责任书类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public object DeleteDutyType(AppDetailModel model)
        {
            try
            {
                string id = model.Id;
                string userid = model.userId;
                UserEntity user = ubll.GetEntity(userid);
                pbll.DelDutyType(id);
                var list = pbll.GetPeopleDutyList(user.DepartmentId).Where(x => x.TypeId == id);
                foreach (PeopleDutyEntity d in list)
                {
                    pbll.DelDuty(d);
                }
                return new { code = 0, info = "成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取责任书类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetDutyType(AppDetailModel model)
        {
            try
            {
                string userid = model.userId;
                UserEntity user = ubll.GetEntity(userid);
                var list = pbll.GetDutyTypes().Where(x => x.BZID == user.DepartmentId).OrderBy(x => x.CreateDate);
                return new { code = 0, info = "成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        public string ToBase64String(string path)
        {
            //string app = HttpContext.Current.Request.ApplicationPath;
            //int index = path.IndexOf(app) + app.Length;
            //path = "~" + path.Substring(index, path.Length - index);
            string imgStr = "";
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 20480, true);
            byte[] imgb = new byte[fs.Length];
            fs.Read(imgb, 0, (int)fs.Length);
            imgStr = Convert.ToBase64String(imgb);
            fs.Close();
            return imgStr;
        }
        #endregion


    }
}
