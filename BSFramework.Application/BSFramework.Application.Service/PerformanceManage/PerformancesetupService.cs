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
    /// 绩效管理配置
    /// </summary>
    public class PerformancesetupService : RepositoryFactory<PerformancesetupEntity>, IPerformancesetupService
    {

        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <returns></returns>
        public List<PerformancesetupEntity> AllTitle( string departmentid)
        {
            return this.BaseRepository().IQueryable().Where(x=>x.departmentid==departmentid).OrderBy(x=>x.sort).ToList();
        }
        /// <summary>
        /// 操作配置  对应修改当前月标题和数据
        /// </summary>
        public void operation(List<PerformancesetupEntity> add, List<PerformancesetupEntity> del, List<PerformancesetupEntity> Listupdate,PerformancetitleEntity title,List<PerformanceEntity> Score)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (add.Count > 0)
                {
                    db.Insert(add);
                }
                if (del.Count > 0)
                {
                    db.Delete(del);
                }
                if (Listupdate.Count > 0)
                {
                    db.Update(Listupdate);
                }
                var ck = false;
                //标题头数据操作
                if (title !=null)
                {
                    if (title.name!=null)
                    {
                        var one = db.FindEntity<PerformancetitleEntity>(title.titleid);
                        if (one != null)
                        {
                            one.name = title.name;
                            one.sort = title.sort;
                            db.Update(one);
                            ck = false;
                        }
                        else
                        {
                            ck = true;
                            db.Insert(title);
                        }

                    }
                   
                    
                }
                //数据列表数据操作
                if (Score.Count>0)
                {
                    if (ck)
                    {
                        db.Insert(Score);
                    }
                    else
                    {
                        db.Update(Score);
                    }
                    
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;

            }
        }
    }
}
