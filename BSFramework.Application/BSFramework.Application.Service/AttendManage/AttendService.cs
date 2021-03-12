using BSFramework.Application.Entity.AttendManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.AttendManage;
using System.Data;

namespace BSFramework.Application.Service.AttendManage
{
    public class AttendService : RepositoryFactory<AttendEntity>, IAttendSerivce
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<AttendEntity> GetList(string deptid)
        {
            //string deptid = OperatorProvider.Provider.Current().DeptId;
            var query = this.BaseRepository().IQueryable().Where(x => x.BZId == deptid);
            return query.OrderByDescending(x => x.AttendDate).ToList();
        }

        public IEnumerable<AttendEntity> GetPageList(string from, string to, string name, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.UserId == name);
            if (!string.IsNullOrEmpty(from))
            {
                DateTime time1 = DateTime.Parse(from);
                DateTime time2 = time1.AddDays(1);
                query = query.Where(x => x.AttendDate >= time1 && x.AttendDate < time2);
            }

            if (!string.IsNullOrEmpty(to))
            {
                DateTime time2 = DateTime.Parse(to).AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.AttendDate <= time2);
            }
            total = query.Count();
            page = 1;
            var q= query.OrderByDescending(x => x.AttendDate).ToList();
            List<AttendEntity> res = new List<AttendEntity>();
            if (q.Count > 1)
            {
                res.Add(q.First());
                res.Add(q.Last());
                return res;
            }
            else 
            {
                return q;
            }
        }
        private class newCount 
        {
            public List<string> Name { get; set; }
            public Int32 Count { get; set; }
        }
        public string GetCount(string deptid,DateTime atenddate)
        {
            DateTime days = atenddate.Date;  //当天
            DateTime enddate = atenddate.AddDays(1);
            List<newCount> clist = new List<newCount>();
            string sql = ";";
            for (int i = 1; i < 49; i++) 
            {
                sql = " select username from wg_attend where attenddate ;";
            }
            
            DataTable dt = this.BaseRepository().FindTable(sql);
            string r = string.Empty;
            var m = DateTime.Now.Month;
            
            var c = new newCount();
            foreach (DataRow row in dt.Rows)
            {
                c = new newCount();
                sql = "select count(*) from wg_lllegalregister where ApproveResult ='0' and month(LllegalTime)='" + m + "' and LllegalTeamId ='" + row[0].ToString() + "'";
                dt = this.BaseRepository().FindTable(sql);
                c.Name.Add( row[1].ToString());
                c.Count = Convert.ToInt32(dt.Rows[0][0].ToString());
                clist.Add(c);
                r += "{" + string.Format("category:'{0}',value:'{1}'", row[1].ToString(), dt.Rows[0][0].ToString()) + "},";
            }
            r = string.Format("[{0}]", r.TrimEnd(new char[] { ',' }));
            return r;
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public AttendEntity GetEntity(string keyValue)
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
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, AttendEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.CreateDate = DateTime.Now;
                this.BaseRepository().Insert(entity);
                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                //entity1.Name = entity.Name;
                //entity1.HGZ = entity.HGZ;
                //entity1.OutDate = entity.OutDate;
                //entity1.ProFactory = entity.ProFactory;
                //entity1.RegDate = entity.RegDate;
                //entity1.RegPersonId = entity.RegPersonId;
                //entity1.RegPersonName = entity.RegPersonName;
                //entity1.Spec = entity.Spec;
                //entity1.Total = entity.Total;
                //entity1.TypeId = entity.TypeId;
                //entity1.ValiDate = entity.ValiDate;
                //entity1.CurrentNumber = entity.CurrentNumber;

                this.BaseRepository().Update(entity1);
            }
        }
        #endregion
    }
}
