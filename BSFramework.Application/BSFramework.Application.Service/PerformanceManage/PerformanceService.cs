using BSFramework.Application.Entity.PerformanceManage;
using BSFramework.Application.Entity.PerformanceManage.ViewModel;
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
    /// 绩效管理数据
    /// </summary>
    public class PerformanceService : RepositoryFactory<PerformanceEntity>, IPerformanceService
    {

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public IQueryable<PerformanceEntity> getScore(string time,string departmentid)
        {
            return this.BaseRepository().IQueryable().Where(x => x.usetime == time&&x.deparmentid== departmentid);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public PerformanceEntity getScoreByuser(string time, string userid) {
            return this.BaseRepository().IQueryable().FirstOrDefault(x => x.usetime == time && x.userid == userid);

        }
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="entity"></param>
        public void operation(List<PerformanceEntity> entity) {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Update(entity);
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
        /// <param name="entity"></param>
        public void add(List<PerformanceEntity> entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public PerformanceEntity getUserScore(string time, string userid)
        {
            return this.BaseRepository().IQueryable().FirstOrDefault(x => x.usetime == time && x.userid == userid);
        }

        public List<PerformanceEntity> getScore(string time, List<string> departIds)
        {
            return this.BaseRepository().IQueryable().Where(x => x.usetime == time && departIds.Contains(x.deparmentid)).ToList();
        }

        public void Insert(List<PerformanceEntity> addData)
        {
            this.BaseRepository().Insert(addData);
        }

        public void Delete(List<PerformanceEntity> delData)
        {
            this.BaseRepository().Delete(delData);
        }

        public List<PerformanceModel> getYearScoreByuser(string year, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<PerformancetitleEntity>()
                        join q2 in db.IQueryable<PerformanceEntity>() on q1.titleid equals q2.titleid
                        where q1.usetime.Contains(year) && q2.userid == userid
                        orderby q1.usetime ascending
                        select new { q1.usetime, q1.name,  q2.score, ScoreSort=q2.sort,q2.performanceid};

            var dataList = query.ToList();
            List<PerformanceModel> modelList = new List<PerformanceModel>();
            if (dataList !=null && dataList.Count>0)
            {
                dataList.ForEach(p => {
                    PerformanceModel perf = new PerformanceModel()
                    {
                        PerformanceId = p.performanceid,
                        Month = ProcessMonth( p.usetime.Split('-')[1]),
                        Title = p.name,
                        UseTime = p.usetime
                    };
                    string scoreStr = string.Empty;
                    var scoreList = p.score.Split(',').ToList();
                    p.ScoreSort.Split(',').ToList().ForEach(x => {
                        scoreStr += scoreList[int.Parse(x)]+",";
                    });
                    perf.Score = scoreStr.TrimEnd(',');
                    modelList.Add(perf);
                });
            }
            return modelList;
        }


        private string ProcessMonth(string month)
        {
            string monthStr = string.Empty;
            month = month.TrimStart('0');//去掉开头的0
            switch (month)
            {
                case "1":
                    monthStr = "一月";
                    break;
                case "2":
                    monthStr = "二月";
                    break;
                case "3":
                    monthStr = "三月";
                    break;
                case "4":
                    monthStr = "四月";
                    break;
                case "5":
                    monthStr = "五月";
                    break;
                case "6":
                    monthStr = "六月";
                    break;
                case "7":
                    monthStr = "七月";
                    break;
                case "8":
                    monthStr = "八月";
                    break;
                case "9":
                    monthStr = "九月";
                    break;
                case "10":
                    monthStr = "十月";
                    break;
                case "11":
                    monthStr = "十一月";
                    break;
                case "12":
                    monthStr = "十二月";
                    break;
                default:
                    break;
            }
            return monthStr;
        }
    }

}
