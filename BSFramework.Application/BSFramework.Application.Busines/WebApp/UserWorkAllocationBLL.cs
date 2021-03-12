using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Application.IService.WebApp;
using BSFramework.Application.Service.PeopleManage;
using BSFramework.Application.Service.WebApp;
using BSFramework.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WebApp
{

    /// <summary>
    /// 转岗离厂
    /// </summary>
    public class UserWorkAllocationBLL
    {
        private DepartmentBLL departmentBLL = new DepartmentBLL();
        private IUserWorkAllocationService service = new UserWorkAllocationService();
        /// <summary>
        /// 获取部门班组
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<DepartmentEntity> getDepartmentList(string id, string category)
        {

            if (string.IsNullOrEmpty(id))
            {
                var rootdpet = departmentBLL.GetRootDepartment();
                id = rootdpet.DepartmentId;
            }
            var depts = departmentBLL.GetSubDepartments(id, category);
            return depts;
        }


        /// <summary>
        /// 获取转移到部门的成员
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserWorkAllocationEntity> GetSendUser(string deptId)
        {
            return service.GetSendUser(deptId);
        }

        /// <summary>
        /// 获取未推送完成的成员
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public IEnumerable<UserWorkAllocationEntity> GetIsSendUser(string deptId)
        {
            return service.GetIsSendUser(deptId);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        public UserWorkAllocationEntity GetDetail(string id)
        {
            return service.GetDetail(id);
        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        public UserWorkAllocationEntity GetDetailByUser(string id)
        {
            return service.GetDetailByUser(id);
        }
        /// <summary>
        /// 增改实体
        /// </summary>
        /// <param name="entity"></param>
        public string OperationEntity(UserWorkAllocationEntity entity, string userid)
        {

            string id = string.Empty;
            string TID = string.Empty;
            var getCk = GetDetail(entity.Id);
            if (getCk == null)
            {
                TID = entity.Id;
                entity.Id = string.Empty;
            }

            if (string.IsNullOrEmpty(entity.Id))
            {

                if (string.IsNullOrEmpty(TID))
                {
                    if (!string.IsNullOrEmpty(entity.leavetime))
                    {
                        TID = Guid.NewGuid().ToString();
                        return service.OperationEntity(entity, TID);
                    }
                    TID = Guid.NewGuid().ToString();
                    //数据交互
                    var webclient = new WebClient();
                    NameValueCollection postVal = new NameValueCollection();
                    int IsConfirm = 1;
                    if (entity.department.Contains('/'))
                    {
                        entity.department = entity.department.Split('/')[1];
                    }

                    var ErchtmsApiUrl = Config.GetValue("ErchtmsApiUrl");
                    var olddept = departmentBLL.GetEntity(entity.olddepartmentid);
                    var newdept = departmentBLL.GetEntity(entity.departmentid);

                    var ss = "{'data':{'TID' : '" + TID + "','InDeptId':'" + entity.olddepartmentid + "','InDeptName':'" + entity.olddepartment + "','InDeptCode':'" + olddept.EnCode + "','OutDeptName':'" + entity.department + "','OutDeptId':'" + entity.departmentid + "','OutDeptCode':'" + newdept.EnCode + "','UserId':'" + entity.userId + "','UserName':'" + entity.username + "','TransferTime':'" + entity.allocationtime + "','IsConfirm':" + IsConfirm + ",'InJobId':'" + entity.oldquartersid + "','InJobName':'" + entity.oldquarters + "','OutJobId':'" + entity.quartersid + "','OutJobName':'" + entity.quarters + "','InPostId':'" + entity.quartersid + "','InPostName':'" + entity.oldRoleDutyName + "','OutPostId':'" + entity.RoleDutyId + "','OutPostName':'" + entity.RoleDutyName + "'}, 'userId':'" + userid + "', 'tokenId':'','isNew':'0'}";
                    //转岗
                    postVal.Add("json", "{'data':{'TID' : '" + TID + "','InDeptId':'" + entity.olddepartmentid + "','InDeptName':'" + entity.olddepartment + "','InDeptCode':'" + olddept.EnCode + "','OutDeptName':'" + entity.department + "','OutDeptId':'" + entity.departmentid + "','OutDeptCode':'" + newdept.EnCode + "','UserId':'" + entity.userId + "','UserName':'" + entity.username + "','TransferTime':'" + entity.allocationtime + "','IsConfirm':" + IsConfirm + ",'InJobId':'" + entity.oldquartersid + "','InJobName':'" + entity.oldquarters + "','OutJobId':'" + entity.oldquartersid + "','OutJobName':'" + entity.quarters + "','InPostId':'" + entity.oldRoleDutyId + "','InPostName':'" + entity.oldRoleDutyName + "','OutPostId':'" + entity.RoleDutyId + "','OutPostName':'" + entity.RoleDutyName + "'}, 'userId':'" + userid + "', 'tokenId':'','isNew':'0'}");
                    var getData = webclient.UploadValues(ErchtmsApiUrl + "person/Transfer", postVal);
                    var result = System.Text.Encoding.UTF8.GetString(getData);
                    dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(result);
                    var info = dy.Info;
                    var code = dy.Code;
                    if (code == 0)
                    {
                        id = service.OperationEntity(entity, TID);
                    }
                  

                }
                else
                {
                    id = service.OperationEntity(entity, TID);
                }
                var messagebll = new MessageBLL();
                messagebll.SendMessage("转岗确认", id);
                return id;
            }
            else
            {
                return service.OperationEntity(entity, TID);
            }

        }

        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<DepartmentEntity> GetSubDepartments(string id, string category)
        {
            if (string.IsNullOrEmpty(id))
            {
                var rootdpet = departmentBLL.GetRootDepartment();
                id = rootdpet.DepartmentId;
            }
            return service.GetSubDepartments(id, category);
        }
        //转岗离厂
        public string OperationBll(UserWorkAllocationEntity entity, string userId)
        {
            try
            {
                //数据交互
                var webclient = new WebClient();
                NameValueCollection postVal = new NameValueCollection();

                //不等于空离厂时间
                if (!string.IsNullOrEmpty(entity.leavetime))
                {
                    var ErchtmsApiUrl = Config.GetValue("ErchtmsApiUrl");
                    //请求双控
                    //  userid:用户Id time: 离职时间
                    postVal.Add("json", "{userid:'" + entity.userId + "',data:{time:'" + entity.leavetime + "',reson:'" + entity.leaveremark + "'}}");
                    var getData = webclient.UploadValues(ErchtmsApiUrl + "person/leaveJob", postVal);
                    var result = System.Text.Encoding.UTF8.GetString(getData);
                    dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(result);
                    var info = dy.Info;
                    var code = dy.Code;
                    if (code == 0)
                    {
                        service.Operationleave(entity);
                    }
                    return info;
                }
                else
                {
                    if (entity.department.Contains('/'))
                    {
                        entity.department = entity.department.Split('/')[1];
                    }
                    int IsConfirm = 2;
                    var ErchtmsApiUrl = Config.GetValue("ErchtmsApiUrl");
                    //转岗
                    postVal.Add("json", "{'data':{'TID' : '" + entity.Id + "','InDeptId':'" + entity.olddepartmentid + "','InDeptName':'" + entity.olddepartment + "','InDeptCode':'','OutDeptName':'" + entity.department + "','OutDeptId':'" + entity.departmentid + "','OutDeptCode':'','UserId':'" + entity.userId + "','UserName':'" + entity.username + "','TransferTime':'" + entity.allocationtime + "','IsConfirm':" + IsConfirm + ",'InJobId':'" + entity.oldquartersid + "','InJobName':'" + entity.oldquarters + "','OutJobId':'" + entity.quartersid + "','OutJobName':'" + entity.quarters + "','InPostId':'" + entity.oldRoleDutyId + "','InPostName':'" + entity.oldRoleDutyName + "','OutPostId':'" + entity.RoleDutyId + "','OutPostName':'" + entity.RoleDutyName + "'}, 'userId':'" + userId + "', 'tokenId':'','isNew':'1' }");
                    var getData = webclient.UploadValues(ErchtmsApiUrl + "person/Transfer", postVal);
                    var result = System.Text.Encoding.UTF8.GetString(getData);
                    dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(result);
                    var info = dy.Info;
                    var code = dy.Code;
                    if (code == 0)
                    {
                        service.OperationJobs(entity);
                    }
                    return info;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public string OperationBlltoerchtms(UserWorkAllocationEntity entity)
        {
            try
            {
                UserBLL userbll = new UserBLL();
                UserExperiencBLL exbll = new UserExperiencBLL();
                if (entity.userId.Contains(','))
                {
                    var userList = entity.userId.Split(',').ToList();
                    foreach (var item in userList)
                    {
                        UserEntity user = userbll.GetEntity(entity.userId);
                        UserExperiencEntity userEx = new UserExperiencEntity();
                        userEx.ExperiencId = Guid.NewGuid().ToString();
                        userEx.StartTime = DateTime.Now.ToString("yyyy-MM-dd");
                        userEx.EndTime = "";
                        userEx.Isend = true;
                        userEx.isupdate = false;
                        userEx.createuserid = item;
                        userEx.createtime = DateTime.Now;
                        userEx.createuser = user.RealName;
                        userEx.Jobs = "";
                        userEx.Position = "";
                        service.Operationleave(entity);
                        exbll.add(userEx);
                    }
                    return "操作成功";
                }
                else
                {


                    UserEntity user = userbll.GetEntity(entity.userId);
                    var dept = getDepartmentList("", "厂级,部门,班组");
                    UserExperiencEntity userEx = new UserExperiencEntity();
                    userEx.ExperiencId = Guid.NewGuid().ToString();
                    userEx.StartTime = DateTime.Now.ToString("yyyy-MM-dd");
                    userEx.EndTime = "至今";
                    userEx.Isend = true;
                    userEx.isupdate = false;
                    userEx.createuserid = user.UserId;
                    userEx.createtime = DateTime.Now;
                    userEx.createuser = user.RealName;
                    //不等于空离厂时间
                    if (!string.IsNullOrEmpty(entity.leavetime))
                    {
                        userEx.Jobs = "";
                        userEx.Position = "";
                        service.Operationleave(entity);
                        exbll.add(userEx);
                        return "操作成功";
                    }
                    else
                    {
                        //获取部门和班组
                        var userDept = dept.FirstOrDefault(x => x.DepartmentId == entity.departmentid);
                        if (userDept.Nature == "班组")
                        {
                            var userParent = dept.FirstOrDefault(x => x.DepartmentId == userDept.ParentId);
                            var company = dept.FirstOrDefault(x => x.DepartmentId == userParent.ParentId);
                            userEx.Commpany = company.FullName;
                            userEx.Department = userParent.FullName + "/" + userDept.FullName;
                        }
                        else
                        {
                            var company = dept.FirstOrDefault(x => x.DepartmentId == userDept.ParentId);
                            userEx.Commpany = company.FullName;
                            userEx.Department = userDept.FullName;
                        }
                        userEx.Jobs = entity.quarters;
                        userEx.Position = entity.RoleDutyName;
                        service.OperationJobs(entity);
                        exbll.add(userEx);
                        return "操作成功";
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// 公用添加转岗记录
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="deptid"></param>
        /// <param name="job">职务</param>
        /// /// <param name="Position">岗位</param>
        public void edituser(string userid, string deptid, string job, string Position)
        {

            try
            {
                UserExperiencBLL exbll = new UserExperiencBLL();
                UserEntity user = new UserBLL().GetEntity(userid);
                UserExperiencEntity userEx = new UserExperiencEntity();
                userEx.ExperiencId = Guid.NewGuid().ToString();
                userEx.StartTime = DateTime.Now.ToString("yyyy-MM-dd");
                userEx.EndTime = "";
                userEx.Isend = true;
                userEx.isupdate = false;
                userEx.createuserid = user.UserId;
                userEx.createtime = DateTime.Now;
                userEx.createuser = user.RealName;
                var dept = getDepartmentList("", "厂级,部门,班组");
                //获取部门和班组
                var userDept = dept.FirstOrDefault(x => x.DepartmentId == deptid);
                if (userDept.Nature == "班组")
                {
                    var userParent = dept.FirstOrDefault(x => x.DepartmentId == userDept.ParentId);
                    var company = dept.FirstOrDefault(x => x.DepartmentId == userParent.DepartmentId);
                    userEx.Commpany = company.FullName;
                    userEx.Department = userDept.FullName;
                }
                else
                {
                    var company = dept.FirstOrDefault(x => x.DepartmentId == userDept.DepartmentId);
                    userEx.Commpany = company.FullName;
                    userEx.Department = userDept.FullName;
                }
                userEx.Jobs = job;
                userEx.Position = Position;
                exbll.add(userEx);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
