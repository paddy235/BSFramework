using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 描 述：班组台
    /// </summary>
    public class OrderinfoService : RepositoryFactory<OrderinfoEntity>, OrderinfoIService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="deptCode">部门编码</param>
        /// <returns>返回列表</returns>
        public object GetList(string deptCode)
        {
            //var resp=new Repository<SafetydayEntity>(DbFactory.Base());
            //SafetydayEntity entity=resp.IQueryable().Where(t=>t.DeptCode.StartsWith(deptCode)).OrderByDescending(t=>t.CreateDate).ToList().FirstOrDefault();
            //if(entity!=null)
            //{
            //    string actIds = entity.ActIds;
            //    if (!string.IsNullOrEmpty(actIds) && actIds != "")
            //    {

            //    }
            //}
            //return null;

            //DataTable dt = this.BaseRepository().FindTable(string.Format("select activityid,groupid,state,planstarttime,leader,fullname from wg_activity inner join base_department on groupid=DEPARTMENTID where state='Ready'"));
            //return new { list = dt };
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<ActivityEntity>()
                        join b in db.IQueryable<DepartmentEntity>()
                        on a.GroupId equals b.DepartmentId
                        where a.State == "Ready"
                        select new
                        {
                            activityid = a.ActivityId,
                            groupid = a.GroupId,
                            state = a.State,
                            planstarttime = a.PlanStartTime,
                            leader = a.Leader,
                            fullname = b.FullName
                        };

            return new { list = query.ToList() };

        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public OrderinfoEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="groupId">班组Id</param>
        /// <param name="sId">主表记录Id</param>
        ///  <param name="userName">预约人姓名</param>
        /// <returns></returns>
        public string SaveForm(string groupId, string sId, string userName)
        {
            var resp = new Repository<ActivityEntity>(DbFactory.Base());
            ActivityEntity entity = resp.FindEntity(sId);
            if (entity != null)
            {
                if (entity.Leader != null && entity.Leader.Contains(userName))
                {
                    return "-1";
                }
                if (entity.Leader == null)
                {
                    entity.Leader = userName;
                }
                else
                {
                    entity.Leader = entity.Leader + ',' + userName;
                }
                resp.ExecuteBySql(string.Format("update wg_activity set Leader='{0}' where activityid='{1}'", entity.Leader, sId));
                return entity.Leader;
            }
            return "";
        }


        public List<OrderinfoEntity> GetMakeList(string userid, string deptCode)
        {
            var user = new UserService().GetEntity(userid);

            List<OrderinfoEntity> dtList = this.BaseRepository().FindList(string.Format("select activityid,groupid,state,planstarttime,leader,fullname from wg_activity inner join base_department on groupid=DEPARTMENTID where leader like('{0}') and state='Finish'", user.RealName)).ToList();
            return dtList;
        }

        public List<ActivityEntity> GetDetailData(string userid, string keyValue, int pagesize, int page, out int total)
        {
            var user = new UserService().GetEntity(userid);

            var db = new RepositoryFactory().BaseRepository();
            List<ActivityEntity> dtList = db.FindList<ActivityEntity>(string.Format("select activityid,groupid,state,planstarttime,leader,fullname GroupName,subject,ActivityType,ActivityPlace,PlanStartTime,planendtime from wg_activity inner join base_department on groupid=DEPARTMENTID where leader like('{0}') and state='Finish'", user.RealName)).ToList();
            foreach (ActivityEntity ae in dtList)
            {
                ae.ActivityLimited = ae.PlanStartTime.ToString("yyyy-MM-dd HH:mm") + "-" + ae.PlanEndTime.ToString("HH:mm");
            }
            total = dtList.Count();
            return dtList.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }

        #endregion
    }
}
