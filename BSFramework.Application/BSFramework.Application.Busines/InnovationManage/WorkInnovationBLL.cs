using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.InnovationManage;
using BSFramework.Application.Service.InnovationManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BSFramework.Application.Busines.InnovationManage
{

    /// <summary>
    /// 班组管理创新成果
    /// </summary>
    public class WorkInnovationBLL
    {
        private IWorkInnovationService service = new WorkInnovationService();
        private FileInfoBLL fileBll = new FileInfoBLL();
        private UserBLL userbll = new UserBLL();
        //private DepartmentBLL deptBll = new DepartmentBLL();

        #region  查询
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<WorkInnovationEntity> getList(string queryJson, Pagination pagination, bool ispage = false)
        {

            try
            {
                if (!ispage)
                {
                    if (pagination == null)
                    {
                        pagination = new Pagination();
                        pagination.page = 1;
                        pagination.rows = 2000;
                    }
                }

                var data = service.getList(queryJson, pagination).ToList();
                data = getWorkInnovationFile(data);
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<WorkInnovationEntity> getListApp(string queryJson, Pagination pagination, bool ispage = false)
        {

            try
            {
                if (!ispage)
                {
                    if (pagination == null)
                    {
                        pagination = new Pagination();
                        pagination.page = 1;
                        pagination.rows = 2000;
                    }
                }
                var data = service.getList(queryJson, pagination).ToList();
                data = getWorkInnovationFile(data);
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<WorkInnovationEntity> getWorkInnovationByid(string Strid)
        {

            var data = getWorkInnovationbyid(Strid).OrderByDescending(x => x.reporttime).ToList();
            data = getWorkInnovationFile(data);
            return data;
        }


        /// <summary>
        /// 审核人员获取数据
        /// </summary>
        /// <returns></returns>
        public List<WorkInnovationEntity> getWorkInnovationByidExtensions(string userid)
        {
            var audit = getAuditByuser(userid).Where(x => string.IsNullOrEmpty(x.state)).ToList();
            if (audit.Count > 0)
            {
                string Strid = string.Join(",", audit.Select(x => x.innovationid));
                var data = getWorkInnovationbyid(Strid).Where(x => x.aduitstate == "待审核").OrderByDescending(x => x.reporttime).ToList();
                data = getWorkInnovationFileExtensions(data, userid);
                return data;
            }
            else
            {
                return new List<WorkInnovationEntity>();
            }
        }

        /// <summary>
        /// 根据用户id获取记录表数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<WorkInnovationEntity> getWorkInnovationByuser(string userid)
        {

            var data = getWorkInnovationbyuser(userid).OrderByDescending(x => x.reporttime).ToList();
            data = getWorkInnovationFile(data);
            return data;
        }
        public List<UserEntity> getAuditUser(string userid, string deptname, string type)
        {
            try
            {
                var userlist = new List<UserEntity>();
                var all = deptbll.GetList();
                var deptlist = all.Where(x => x.FullName == deptname);
                if (userid != "0")
                {
                    var user = userbll.GetEntity(userid);
                    deptlist = getDeptList(user.DepartmentId, deptlist);

                }
                foreach (var item in deptlist)
                {
                    var deptuser = userbll.GetDeptUsers(item.DepartmentId);
                    userlist.AddRange(deptuser);
                }

                return userlist;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private DepartmentBLL deptbll = new DepartmentBLL();

        private IEnumerable<DepartmentEntity> getDeptList(string deptid, IEnumerable<DepartmentEntity> deptlist)
        {
            var userDept = deptbll.GetEntity(deptid);
            if (userDept == null)
            {
                return new List<DepartmentEntity>();
            }
            var ckdept = deptlist.Where(x => x.ParentId == userDept.DepartmentId);
            if (ckdept.Count() > 0)
            {
                return ckdept;
            }
            else
            {
                return getDeptList(userDept.ParentId, deptlist);
            }

        }
        #endregion

        #region  查询数据

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <returns></returns>
        public List<WorkInnovationEntity> getWorkInnovationSort(List<WorkInnovationEntity> data)
        {

            try
            {
                var list = new List<WorkInnovationEntity>();
                if (data.Count > 0)
                {
                    var state = data.Where(x => x.aduitresult == "待提交").ToList();
                    if (state.Count > 0)
                    {
                        list.AddRange(state);
                    }
                    state = data.Where(x => x.aduitresult == "待审核").ToList();
                    if (state.Count > 0)
                    {
                        list.AddRange(state);
                    }
                    state = data.Where(x => x.aduitresult == "审核通过").ToList();
                    if (state.Count > 0)
                    {
                        list.AddRange(state);
                    }
                    state = data.Where(x => x.aduitresult == "审核不通过").ToList();
                    if (state.Count > 0)
                    {
                        list.AddRange(state);
                    }
                }

                return list;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int GetInnovation(string deptid)
        {
            return service.GetInnovation(deptid);
        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<WorkInnovationEntity> getWorkInnovationFile(List<WorkInnovationEntity> data)
        {

            try
            {
                foreach (var item in data)
                {
                    item.audit = getAuditByid(item.innovationid).OrderBy(x => x.sort).ToList();
                    var fileList = fileBll.GetFilesByRecIdNew(item.innovationid);
                    //foreach (var items in fileList)
                    //{
                    //    items.FilePath = items.FilePath.Replace("~/", url);
                    //}
                    item.proposedPhoto = fileList.Where(x => x.Description == "ty").ToList();
                    item.statusquoPhoto = fileList.Where(x => x.Description == "xz").ToList();
                    item.proposedFile = fileList.Where(x => x.Description == "fj").ToList();
                }
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<WorkInnovationEntity> getWorkInnovationFileExtensions(List<WorkInnovationEntity> data, string userid)
        {

            try
            {
                List<WorkInnovationEntity> work = new List<WorkInnovationEntity>();
                foreach (var item in data)
                {
                    var audit = getAuditByid(item.innovationid).OrderBy(x => x.sort).ToList();

                    item.audit = audit;
                    var fileList = fileBll.GetFilesByRecIdNew(item.innovationid);
                    //foreach (var items in fileList)
                    //{
                    //    items.FilePath = items.FilePath.Replace("~/", url);
                    //}
                    item.proposedPhoto = fileList.Where(x => x.Description == "ty").ToList();
                    item.statusquoPhoto = fileList.Where(x => x.Description == "xz").ToList();
                    item.proposedFile = fileList.Where(x => x.Description == "fj").ToList();
                    var ck = audit.Where(x => string.IsNullOrEmpty(x.state)).OrderBy(x => x.sort).ToList();
                    if (ck.Count > 1)
                    {
                        if (ck[0].userid == userid)
                        {
                            work.Add(item);
                        }
                    }
                    else
                    {
                        work.Add(item);
                    }
                }
                return work;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 根据用户id获取记录表数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<WorkInnovationEntity> getWorkInnovationbyuser(string userid)
        {

            try
            {
                return service.getWorkInnovationbyuser(userid);
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 根据主键id获取记录表数据
        /// </summary>
        /// <param name="Strid"></param>
        /// <returns></returns>
        public List<WorkInnovationEntity> getWorkInnovationbyid(string Strid)
        {

            try
            {
                return service.getWorkInnovationbyid(Strid);

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 根据关联id获取审核数据
        /// </summary>
        /// <param name="innovationid"></param>
        /// <returns></returns>
        public List<WorkInnovationAuditEntity> getAuditByid(string innovationid)
        {
            return service.getAuditByid(innovationid);
        }

        /// <summary>
        /// 根据用户id获取审核数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<WorkInnovationAuditEntity> getAuditByuser(string userid)
        {
            return service.getAuditByuser(userid);
        }

        /// <summary>
        /// 根据id获取审核数据
        /// </summary>
        /// <param name="auditid"></param>
        /// <returns></returns>
        public List<WorkInnovationAuditEntity> getAuditId(string auditid)
        {
            return service.getAuditId(auditid);

        }
        #endregion

        #region   数据操作
        /// <summary>
        /// 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="audit"></param>
        /// <param name="type">是否存在数据</param>
        public void Operation(WorkInnovationEntity main, List<WorkInnovationAuditEntity> audit, bool type)
        {

            try
            {

                service.Operation(main, audit, type);

                #region 20190709 sx 消息通知 
                MessageBLL messagebll = new MessageBLL();
                if (main != null)
                {
                    if (main.aduitstate == "待审核")
                    {
                        //待办通知
                        messagebll.SendMessage("待审核管理创新成果", main.innovationid);
                    }
                }

                if (main == null && audit != null && audit.Count > 0)
                {
                    if (audit[1].isspecial && audit[1].state == "审核通过")
                    {
                        messagebll.SendMessage("创新成果审批通过", audit[1].innovationid);
                    }
                    if (audit[0].state == "审核不通过")
                    {
                        messagebll.SendMessage("创新成果审批不通过", audit[0].innovationid);
                    }
                    if (audit[1].state == "审核不通过")
                    {
                        messagebll.SendMessage("创新成果审批不通过", audit[1].innovationid);
                    }
                }
                #endregion

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        /// <summary>
        /// 得到当前用户的待审核数量
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int GetTodoCount(string userid)
        {
            return service.GetTodoCount(userid);
        }
    }
}