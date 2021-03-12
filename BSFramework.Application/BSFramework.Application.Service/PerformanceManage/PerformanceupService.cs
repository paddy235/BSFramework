using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.IService.PerformanceManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.PerformanceManage
{
    /// <summary>
    /// 提交绩效
    /// </summary>
    public class PerformanceupService : RepositoryFactory<PerformanceupEntity>, IPerformanceupService
    {

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public PerformanceupEntity getList(string titleid)
        {
            return this.BaseRepository().IQueryable().FirstOrDefault(x => x.titleid == titleid);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public  List<PerformanceupEntity> getListByMonth(string month)
        {
            return this.BaseRepository().IQueryable().Where(x => x.usetime == month).ToList();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<PerformanceupEntity> getListByYear(string year)
        {
            return this.BaseRepository().IQueryable().Where(x => x.useyear == year ).ToList();
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <returns></returns>
        public void operation(string titleid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var one=db.IQueryable<PerformanceupEntity>().FirstOrDefault(x => x.titleid == titleid);
                one.isup = true;
                db.Update(one);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns></returns>
        public void add(List<PerformanceupEntity> list)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(list);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
    }
}
