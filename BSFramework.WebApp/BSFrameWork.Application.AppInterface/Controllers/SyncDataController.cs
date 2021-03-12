using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Busines.InnovationManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PerformanceManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SevenSManage;
//using BSFramework.Application.Busines.HiddenTroubleManage;
//using BSFramework.Application.Busines.RiskDatabase;
//using BSFramework.Application.Busines.SaftyCheck;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Controllers;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFramework.AppSerivce.Controllers
{
    //BSFramework.Application.AppSerivce

    public class SyncDataController : BaseApiController
    {

        /// <summary>
        /// 设置当前操作用户信息并缓存
        /// </summary>
        /// <param name="userEntity">用户实体</param>
        private void SetUserInfo(UserInfoEntity userEntity)
        {
            if (BSFramework.Application.Code.OperatorProvider.Provider.IsOnLine() != 1)
            {
                Operator operators = new Operator();
                operators.Token = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                operators.UserId = userEntity.UserId;
                operators.Code = userEntity.EnCode;
                operators.Account = userEntity.Account;
                operators.UserName = userEntity.RealName;
                operators.Secretkey = userEntity.Secretkey;
                operators.OrganizeId = userEntity.OrganizeId;
                operators.DeptId = userEntity.DepartmentId;
                operators.DeptCode = userEntity.DepartmentCode;
                operators.OrganizeCode = userEntity.OrganizeCode;
                operators.IdentifyID = userEntity.IdentifyID;
                operators.LogTime = DateTime.Now;
                DepartmentEntity dept = new DepartmentBLL().GetEntity(userEntity.DepartmentId);
                operators.TeamType = dept == null ? "" : dept.TeamType;

                OrganizeEntity org = new OrganizeBLL().GetList().Where(t => t.ParentId == "0").FirstOrDefault();
                if (org != null)
                {
                    operators.OrganizeName = userEntity.OrganizeName;
                    operators.OrganizeId = org.OrganizeId;
                    operators.OrganizeCode = org.EnCode;
                }
                BSFramework.Application.Code.OperatorProvider.Provider.AddCurrent(operators);
                OperatorProvider.AppUserId = userEntity.UserId;
            }

        }
        [HttpGet]
        public string Get(string id)
        {
            return id;
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

            var obj = new FileInfoEntity() { FileId = id, CreateDate = DateTime.Now, CreateUserId = user.ID, CreateUserName = user.Name, Description = type, FileExtensions = ".jpg", FileName = id + ".jpg", FilePath = path + id + ".jpg", FileType = "jpg", ModifyDate = DateTime.Now, ModifyUserId = user.ID, ModifyUserName = user.Name, RecId = user.ID };
            fileBll.SaveForm(obj);
            return obj;
        }
        /// <summary>
        /// 新增或保存用户
        /// </summary>
        /// <param name="keyValue">记录Id</param>
        /// <param name="json">用户信息（对象被序列化之后的）</param>
        /// <returns></returns>
        [HttpPost]
        public string SaveUser(string keyValue)
        {

            string json = HttpContext.Current.Request.Params["json"];
            string account = HttpContext.Current.Request.Params["account"];
            string sex = HttpContext.Current.Request.Params["sex"];
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                UserEntity user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserEntity>(json);
                //双控，班组人员信息同一，获取特种人员等字段
                //PeopleEntity newpeople = Newtonsoft.Json.JsonConvert.DeserializeObject<PeopleEntity>(json);

                UserBLL userBll = new UserBLL();
                UserInfoEntity currUser = userBll.GetUserInfoByAccount(account);
                currUser.IdentifyID = user.IDENTIFYID;
                SetUserInfo(currUser);
                OrganizeEntity org = new OrganizeBLL().GetList().Where(t => t.ParentId == "0").FirstOrDefault();
                if (org != null)
                {
                    user.OrganizeId = org.OrganizeId;
                    user.OrganizeCode = org.EnCode;
                }
                if (!String.IsNullOrEmpty(user.Password))
                {
                    if (user.Password.Contains("*"))
                        user.Password = null;
                }

                userBll.SaveForm(keyValue, user);
                PeopleBLL peoplebll = new PeopleBLL();
                PeopleEntity obj = peoplebll.GetEntity(user.UserId);
                if (obj == null) obj = new PeopleEntity() { ID = user.UserId };
                obj.IdentityNo = user.IDENTIFYID;
                obj.LinkWay = user.Mobile;
                obj.RoleDutyName = user.DutyName;
                obj.RoleDutyId = user.DutyId;
                obj.Birthday = Convert.ToDateTime(user.Birthday);
                obj.SpecialtyType = user.SpecialtyType;
                // obj.Files = null;
                obj.EntryDate = user.EnterTime;
                obj.IsSpecial = user.IsSpecial;
                obj.IsSpecialEquipment = user.IsSpecialEqu;
                obj.Quarters = user.PostName;
                obj.Planer = user.PostCode;
                obj.QuarterId = user.PostId;
                obj.Age = user.Age;
                obj.OldDegree = user.Degrees;
                obj.NewDegree = user.LateDegrees;
                obj.LabourNo = user.EnCode;
                obj.TecLevel = user.TechnicalGrade;
                obj.JobName = user.JobTitle;
                obj.WorkKind = user.Craft;
                obj.CurrentWorkAge = user.CraftAge;
                obj.HealthStatus = user.HealthStatus;
                obj.Visage = user.Political;
                obj.Folk = user.Nation;
                obj.Native = user.Native;
                obj.Photo = user.HeadIcon;
                obj.Signature = user.SignImg;
                obj.BZID = user.DepartmentId;
                obj.BZCode = user.DepartmentCode;
                obj.BZName = user.DepartmentName;
                obj.Name = user.RealName;
                obj.FingerMark = "yes";
                obj.Description = user.Description;

                var newfile = this.BuildImage(obj, "人员");
                //obj.Files.Add(newfile);
                // peoplebll.Update(obj);
                peoplebll.SaveForm(obj.ID, obj);

                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,保存用户成功,同步信息：" + json + "\r\n");
                return "success";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,保存用户失败,同步信息：" + json + ",异常信息:" + ex.Message + "\r\n");
                return ex.Message;
            }
        }

        /// <summary>
        /// 新增或保存用户
        /// </summary>
        /// <param name="keyValue">记录Id</param>
        /// <param name="json">用户信息（对象被序列化之后的）</param>
        /// <returns></returns>
        [HttpPost]
        public string SaveUsers()
        {

            string json = HttpContext.Current.Request.Params["json"];
            string account = HttpContext.Current.Request.Params["account"];
            //string sex = HttpContext.Current.Request.Params["sex"];
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                List<UserEntity> users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserEntity>>(json);
                UserBLL userBll = new UserBLL();
                UserInfoEntity currUser = userBll.GetUserInfoByAccount(account);
                SetUserInfo(currUser);
                foreach (var item in users)
                {
                    //currUser.IdentifyID = item.IDENTIFYID;
                    OrganizeEntity org = new OrganizeBLL().GetList().Where(t => t.ParentId == "0").FirstOrDefault();
                    if (org != null)
                    {
                        item.OrganizeId = org.OrganizeId;
                        item.OrganizeCode = org.EnCode;
                    }
                    if (!String.IsNullOrEmpty(item.Password))
                    {
                        if (item.Password.Contains("*"))
                            item.Password = null;
                    }
                    userBll.SaveForm(item.UserId, item);

                    //UserEntity user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserEntity>(json);
                    //双控，班组人员信息同一，获取特种人员等字段
                    //PeopleEntity newpeople = Newtonsoft.Json.JsonConvert.DeserializeObject<PeopleEntity>(json);

                    PeopleBLL peoplebll = new PeopleBLL();
                    PeopleEntity obj = peoplebll.GetEntity(item.UserId);
                    if (obj != null)
                    {
                        obj.IdentityNo = item.IDENTIFYID;
                        obj.LinkWay = item.Mobile;
                        obj.RoleDutyName = item.DutyName;
                        obj.RoleDutyId = item.DutyId;
                        obj.Birthday = Convert.ToDateTime(item.Birthday);
                        // obj.Files = null;
                        obj.EntryDate = item.EnterTime;
                        obj.SpecialtyType = item.SpecialtyType;
                        obj.IsSpecial = item.IsSpecial;
                        obj.IsSpecialEquipment = item.IsSpecialEqu;
                        obj.Quarters = item.PostName;
                        obj.QuarterId = item.PostId;
                        obj.Planer = item.PostCode;
                        obj.Age = item.Age;
                        obj.OldDegree = item.Degrees;
                        obj.NewDegree = item.LateDegrees;
                        obj.LabourNo = item.EnCode;
                        obj.TecLevel = item.TechnicalGrade;
                        obj.JobName = item.JobTitle;
                        obj.WorkKind = item.Craft;
                        obj.CurrentWorkAge = item.CraftAge;
                        obj.HealthStatus = item.HealthStatus;
                        obj.Visage = item.Political;
                        obj.Folk = item.Nation;
                        obj.Native = item.Native;
                        obj.Description = item.Description;

                        var newfile = this.BuildImage(obj, "人员");
                        //obj.Files.Add(newfile);
                        peoplebll.SaveForm(obj.ID, obj);
                    }
                }

                NLog.LogManager.GetCurrentClassLogger().Info("双控人员同步成功，{0}", json);
                //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,保存用户成功,同步信息：" + json + "\r\n");
                return "success";
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error("双控人员同步异常，{0}，{1}", json, ex.Message);

                //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,保存用户失败,同步信息：" + json + ",异常信息:" + ex.Message + "\r\n");
                return ex.Message;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="keyValue">记录Id</param>
        /// <returns></returns>
        [HttpPost]
        public string DeleteUser(string keyValue)
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {

                UserBLL userBll = new UserBLL();
                UserEntity user = userBll.GetEntity(keyValue);
                if (user != null)
                {
                    userBll.RemoveForm(keyValue);
                    System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,删除用户成功,同步信息：" + Newtonsoft.Json.JsonConvert.SerializeObject(user) + "\r\n");
                    return "操作成功";
                }
                return "用户不存在";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,删除用户失败,用户Id：" + keyValue + ",异常信息:" + ex.Message + "\r\n");
                return ex.Message;
            }
        }
        /// <summary>
        /// 新增或保存部门
        /// </summary>
        /// <param name="keyValue">记录Id</param>
        /// <param name="json">部门信息（对象被序列化之后的）</param>
        /// <returns></returns>
        [HttpPost]
        public string SaveDept(string keyValue)
        {
            string json = HttpContext.Current.Request.Params["json"];
            NLog.LogManager.GetCurrentClassLogger().Info($"新增部门，json：{json}");
            try
            {
                string account = HttpContext.Current.Request.Params["account"];
                UserBLL userBll = new UserBLL();
                UserInfoEntity currUser = userBll.GetUserInfoByAccount(account);
                SetUserInfo(currUser);
                DepartmentEntity dept = Newtonsoft.Json.JsonConvert.DeserializeObject<DepartmentEntity>(json);
                DepartmentBLL deptBll = new DepartmentBLL();
                //string isorg = dept.IsOrg;
                //if (isorg == "1") dept.IsSpecial = true;
                //if (isorg == "0") dept.IsSpecial = false; ;
                //dept.IsOrg = null;
                //OrganizeEntity org = new OrganizeBLL().GetList().Where(t => t.ParentId == "0").FirstOrDefault();
                //if (org != null)
                //{
                //    dept.OrganizeId = org.OrganizeId;
                //}
                deptBll.SaveForm(keyValue, dept);
                //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",保存部门成功,同步信息：" + json + "\r\n");
                return "success";
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error($"新增部门异常，{ex.Message}");
                return ex.Message;
            }
        }

        /// <summary>
        /// 同步部门
        /// </summary>
        /// <param name="depts"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> PostDepartments(List<DepartmentEntity> depts)
        {
            string json = await ReadJson();
            NLog.LogManager.GetCurrentClassLogger().Info($"部门同步，json：{json}");

            foreach (var item in depts)
            {
                item.CreateUserName = item.ModifyUserName = "sync";
                item.CreateDate = item.ModifyDate = DateTime.Now;
            }

            var deptbll = new DepartmentBLL();
            deptbll.Save(depts);

            return Ok(depts);
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteDepartment(string id)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"删除部门，id：{id}");
            var deptbll = new DepartmentBLL();
            var dept = deptbll.Delete(id);
            return Ok(dept);
        }

        /// <summary>
        ///删除部门
        /// </summary>
        /// <param name="keyValue">记录Id</param>
        /// <param name="json">部门信息（对象被序列化之后的）</param>
        /// <returns></returns>
        [HttpPost]
        public string DeleteDept(string keyValue)
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                DepartmentBLL deptBll = new DepartmentBLL();
                DepartmentEntity dept = deptBll.GetEntity(keyValue);
                if (dept != null)
                {
                    deptBll.RemoveForm(keyValue);
                    System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,删除部门成功,同步信息：" + Newtonsoft.Json.JsonConvert.SerializeObject(dept) + "\r\n");
                    return "success";
                }
                return "error";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,删除部门失败,部门Id：" + keyValue + ",异常信息:" + ex.Message + "\r\n");
                return ex.Message;
            }
        }
        /// <summary>
        /// 新增或保存角色
        /// </summary>
        /// <param name="keyValue">记录Id</param>
        /// <param name="json">用户信息（对象被序列化之后的）</param>
        /// <returns></returns>
        [HttpPost]
        public string SaveRole(string keyValue)
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            string json = HttpContext.Current.Request.Params["json"];
            try
            {
                string account = HttpContext.Current.Request.Params["account"];
                UserBLL userBll = new UserBLL();
                UserInfoEntity currUser = userBll.GetUserInfoByAccount(account);
                SetUserInfo(currUser);

                RoleEntity role = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleEntity>(json);
                RoleBLL roleBll = new RoleBLL();
                OrganizeEntity org = new OrganizeBLL().GetList().Where(t => t.ParentId == "0").FirstOrDefault();
                if (org != null)
                {
                    role.OrganizeId = org.OrganizeId;
                }
                roleBll.SaveForm(keyValue, role);
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,保存角色成功,同步信息：" + json + "\r\n");
                return "success";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ，删除角色失败,同步信息：" + json + ",异常信息:" + ex.Message + "\r\n");
                return ex.Message;
            }
        }
        ///// <summary>
        ///// 新增或保存角色
        ///// </summary>
        ///// <param name="keyValue">记录Id</param>
        ///// <param name="json">用户信息（对象被序列化之后的）</param>
        ///// <returns></returns>
        //[HttpPost]
        //public string DeleteRole(string keyValue)
        //{
        //    string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
        //    try
        //    {
        //        RoleBLL roleBll = new RoleBLL();
        //        RoleEntity role = roleBll.GetEntity(keyValue);
        //        if (role != null)
        //        {
        //            roleBll.RemoveForm(keyValue);
        //            System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,删除角色成功,操作信息：" + Newtonsoft.Json.JsonConvert.SerializeObject(role) + "\r\n");
        //            return "success";
        //        }
        //        return "error";
        //    }
        //    catch (Exception ex)
        //    {
        //        System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,删除角色失败,角色Id：" + keyValue + ",异常信息:" + ex.Message + "\r\n");
        //        return ex.Message;
        //    }
        //}


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pwd">新密码</param>
        /// <returns></returns>
        [HttpPost]
        public string UpdatePwd(string userId, string pwd)
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";

            try
            {
                UserBLL userBll = new UserBLL();
                UserEntity user = userBll.GetEntity(userId);
                if (user != null)
                {
                    userBll.RevisePassword(userId, pwd.ToLower());
                }
                else
                {
                    return "用户不存在！";
                }
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",修改密码成功,同步信息：" + Newtonsoft.Json.JsonConvert.SerializeObject(user) + "\r\n");
                return "success";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,修改密码失败,同步信息：" + userId + ">" + pwd + ",异常信息:" + ex.Message + "\r\n");
                return ex.Message;
            }
        }

        /// <summary>
        /// 同步头像及签名
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UpdateUrl()
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            string json = HttpContext.Current.Request.Params["json"];

            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
                string userid = dy.userid;
                PeopleBLL peoplebll = new PeopleBLL();
                UserBLL userBll = new UserBLL();
                UserEntity user = userBll.GetEntity(dy.userid);
                PeopleEntity obj = peoplebll.GetEntity(dy.userid);
                long mode = dy.mode;
                if (mode == 0)
                {
                    obj.Signature = dy.filepath;
                }
                else if (mode == 10)
                {
                    obj.Photo = dy.filepath;
                }
                peoplebll.SaveForm(obj.ID, obj);
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",修改头像及签名成功,同步信息：" + Newtonsoft.Json.JsonConvert.SerializeObject(user) + "\r\n");
                return "success";
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/logs/" + fileName), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ,保存用户失败,同步信息：" + json + ",异常信息:" + ex.Message + "\r\n");
                return ex.Message;
            }
        }

        /// <summary>
        /// 提供给双控管理平台的班组待办事项
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<object> GetAdminTodos()
        {
            try
            {
                if (curUser == null)
                {
                    throw new ArgumentNullException("用户不存在");
                }
                //待审核管理创新成果
                WorkInnovationBLL workInnovationBLL = new WorkInnovationBLL();
                int dshcxcg = workInnovationBLL.GetTodoCount(curUser.UserId);
                // EduPlanBLL edubll = new EduPlanBLL();
                // int dshpxjh = edubll.GetTodoInfoCount(curUser.DeptCode);
                string applicationPath = Config.GetValue("AppUrl");

                //====待审核合理化建议 何明======//
                AdviceBLL adviceBLL = new AdviceBLL();
                int hlhjy = adviceBLL.GetTodoCount(curUser.UserId);
                //====待审核合理化建议 END======//
                //====7S创新提案 zwh======//
                SevenSBLL ebll = new SevenSBLL();
                int dshsscxta = ebll.GetTodoCount(curUser.UserId);
                //==========//
                //====待评价 zwh======//
                ActivityBLL acbll = new ActivityBLL();
                int aqrhd = acbll.WorkToDo(curUser.UserId, "安全日活动").Count();
                int zzxx = acbll.WorkToDo(curUser.UserId, "政治学习").Count();
                int mzshh = acbll.WorkToDo(curUser.UserId, "民主生活会").Count();
                int bwh = acbll.WorkToDo(curUser.UserId, "班务会").Count();
                int jswd = acbll.WorkToDo(curUser.UserId, "2").Count();
                int sgyx = acbll.WorkToDo(curUser.UserId, "3").Count();
                int jsjk = acbll.WorkToDo(curUser.UserId, "1").Count();
                int fsgyx = acbll.WorkToDo(curUser.UserId, "4").Count();
                int jypx = jswd + sgyx + jsjk + fsgyx;
                //==========//

                List<object> todos = new List<object>();
                // todos.Add(new { url = Path.Combine(applicationPath, "Works/Education/DetailPlanInfo"), text = "待审核培训计划", count = dshpxjh });
                todos.Add(new { url = Path.Combine(applicationPath, "works/advice/Index?searchtype=todo"), text = "待审核合理化建议", count = hlhjy });
                todos.Add(new { url = Path.Combine(applicationPath, "Works/WorkInnovation/Index?searchtype=todo"), text = "待审核管理创新成果", count = dshcxcg });
                todos.Add(new { url = Path.Combine(applicationPath, "Works/SevenS/IndexOffice?searchtype=todo"), text = "待审核7S创新提案", count = dshsscxta });
                todos.Add(new { url = Path.Combine(applicationPath, "Works/Activity/Index5?category=安全日活动"), text = "待评价的安全日活动", count = aqrhd });
                todos.Add(new { url = Path.Combine(applicationPath, "Works/Activity/Index5?category=政治学习"), text = "待评价的政治学习", count = zzxx });
                todos.Add(new { url = Path.Combine(applicationPath, "Works/Activity/Index5?category=民主生活会"), text = "待评价的民主生活会", count = mzshh });
                todos.Add(new { url = Path.Combine(applicationPath, "Works/Activity/Index5?category=班务会"), text = "待评价的班务会", count = bwh });
                todos.Add(new { url = Path.Combine(applicationPath, "Works/Education/Index2"), text = "待评价的教育培训", count = jypx });

                return new ModelBucket<object> { code = 0, Data = todos, Success = true, Message = "成功" };
            }
            catch (Exception ex)
            {
                return new ModelBucket<object> { code = 1, Data = null, Success = false, Message = ex.Message };
            }
        }
        /// <summary>
        /// 双控 人员档案-绩效档案
        /// </summary>
        /// <param name="year">数字年份</param>
        /// <param name="userid">要查询的用户的ID</param>
        /// <returns></returns>
        [HttpPost]
        public List<BSFramework.Application.Entity.PerformanceManage.ViewModel.PerformanceModel> GetPerformanceList(string year, string userid)
        {
            try
            {
                PerformanceBLL bLL = new PerformanceBLL();
                List<BSFramework.Application.Entity.PerformanceManage.ViewModel.PerformanceModel> data = bLL.getYearScoreByuser(year, userid);
                return data;
            }
            catch (Exception ex)
            {
                return new List<BSFramework.Application.Entity.PerformanceManage.ViewModel.PerformanceModel>();
            }
        }

        /// <summary>
        /// 同步角色
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> PostRoles(List<RoleEntity> roles)
        {
            string json = await ReadJson();
            NLog.LogManager.GetCurrentClassLogger().Info($"角色同步，json：{json}");

            foreach (var item in roles)
            {
                item.Category = 1;
                item.IsPublic = 0;
                item.DeleteMark = 0;
                item.EnabledMark = 1;
                item.CreateDate = DateTime.Now;
                item.CreateUserName = "sync";
                item.ModifyDate = DateTime.Now;
                item.ModifyUserName = "sync";
            }

            var rolebll = new RoleBLL();
            rolebll.Save(roles);

            return Ok(roles);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult DeleteRole(string id)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"角色删除，json：{id}");
            var rolebll = new RoleBLL();
            var role = rolebll.Delete(id);
            return Ok(role);
        }

        /// <summary>
        /// 同步人员
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> PostEmployees(List<UserEntity> employees, bool needEncrypt = true)
        {
            string json = await ReadJson();
            NLog.LogManager.GetCurrentClassLogger().Info($"人员同步，json：{json}");

            foreach (var item in employees)
            {
                if (item.Password != null)
                {
                    if (needEncrypt == true)
                        item.Password = Md5Helper.MD5(DESEncrypt.Encrypt(Md5Helper.MD5(item.Password, 32).ToLower(), item.Secretkey).ToLower(), 32).ToLower();
                }
                item.CreateDate = item.ModifyDate = DateTime.Now;
                item.ModifyUserName = item.ModifyUserName = "sync";
                item.EnabledMark = 1;
            }
            var userbll = new UserBLL();
            userbll.Save(employees);

            return Ok(employees);
        }

        /// <summary>
        /// 删除人员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult DeleteEmployee(string id)
        {
            NLog.LogManager.GetCurrentClassLogger().Info($"人员删除，json：{id}");
            var userBll = new UserBLL();
            var role = userBll.Delete(id);
            return Ok(role);
        }

        private async Task<string> ReadJson()
        {
            var json = string.Empty;
            using (var stream = await this.Request.Content.ReadAsStreamAsync())
            {
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    json = streamReader.ReadToEnd();
                }
            }

            return json;
        }
    }
}