using Bst.Fx.IWarning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bst.Fx.WarningData;
using Bst.Bzzd.DataSource;
using Bst.Fx.IMessage;
using Bst.Fx.Message;
using Bst.ServiceContract.MessageQueue;
using System.ServiceModel;
using BSFramework.Data.Repository;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.SystemManage;

namespace Bst.Fx.Warning
{
    public class EvaluateService : IEvaluateService
    {
        private string key = "人身风险预控评价";
        public void Execute(string category)
        {
            var now = DateTime.Now;
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            var setting = (from q in db.IQueryable<DataItemDetailEntity>()
                           where q.ItemName == "定期推送人身风险预控台账评价任务"
                           select q).FirstOrDefault();

            if (setting.ItemValue == "否") return;
            else setting = null;

            var entities = new List<ToEvaluateEntity>();

            if (category == "人身风险预控周评")
            {
                var begin = now.Date.AddDays(-(now.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)now.DayOfWeek)).AddDays(1).AddDays(-7);
                var end = begin.AddDays(7);

                setting = (from q in db.IQueryable<DataItemDetailEntity>()
                           where q.ItemName == "定期推送人身风险预控台账评价任务按周"
                           select q).FirstOrDefault();
                var pct = 1f;
                if (setting != null)
                {
                    pct = float.Parse(setting.ItemValue) / 100;
                }

                var deptsQuery = from q in db.IQueryable<DepartmentEntity>()
                                 where q.Nature == "班组"
                                 select q;
                var depts = deptsQuery.ToList();
                foreach (var item in depts)
                {
                    //var users = (from q1 in db.IQueryable<UserEntity>()
                    //             join q2 in db.IQueryable<PeopleEntity>() on q1.UserId equals q2.ID
                    //             let quarters = "," + q2.Quarters + ","
                    //             where q1.DepartmentId == item.DepartmentId && (quarters.Contains(",班长,") || quarters.Contains(",副班长,") || quarters.Contains(",技术员,"))
                    //             select q1).ToList();

                    var users2 = (from q in db.IQueryable<UserEntity>()
                                  where q.DepartmentId == item.DepartmentId
                                  select q).ToList();

                    var total = (int)Math.Round(users2.Count * pct, MidpointRounding.AwayFromZero);
                    var list = new List<string>();
                    var random = new Random();
                    while (list.Count < total)
                    {
                        var index = random.Next(users2.Count);
                        list.Add(users2[index].UserId);
                        users2.RemoveAt(index);
                    }

                    var trainingQuery = from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                                        join q2 in db.IQueryable<ActivityEvaluateEntity>() on q1.TrainingUserId.ToString() equals q2.Activityid into into2
                                        where list.Contains(q1.UserId) && q1.IsDone == true && q1.IsMarked && q1.TrainingTime >= begin && q1.TrainingTime < end && into2.Count(x => x.EvaluateDeptId == item.DepartmentId) == 0
                                        select q1;
                    var trainings = trainingQuery.ToList();
                    entities.AddRange(trainings.Select(x => new ToEvaluateEntity() { BusinessId = x.TrainingUserId.ToString(), EvaluateDeptId = item.DepartmentId, StartDate = begin, EndDate = end, ToEvaluateId = Guid.NewGuid().ToString() }));
                }
            }
            else
            {
                var begin = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                var end = begin.AddMonths(1);

                setting = (from q in db.IQueryable<DataItemDetailEntity>()
                           where q.ItemName == "定期推送人身风险预控台账评价任务按月"
                           select q).FirstOrDefault();
                var pct = 1f;
                if (setting != null)
                {
                    pct = float.Parse(setting.ItemValue) / 100;
                }

                var deptsQuery = from q in db.IQueryable<DepartmentEntity>()
                                 where q.Nature == "部门"
                                 select q;
                var depts = deptsQuery.ToList();
                foreach (var item in depts)
                {
                    //var users = (from q1 in db.IQueryable<UserEntity>()
                    //             where q1.DepartmentId == item.DepartmentId
                    //             select q1).ToList();

                    var subDeptsQuery = from q in db.IQueryable<DepartmentEntity>()
                                        where q.ParentId == item.DepartmentId
                                        select q;
                    var subQuery = from q1 in subDeptsQuery
                                   join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                                   select q2;
                    while (subQuery.Count() > 0)
                    {
                        subDeptsQuery = subDeptsQuery.Concat(subQuery);
                        subQuery = from q1 in subQuery
                                   join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                                   select q2;
                    }
                    subDeptsQuery = subDeptsQuery.Where(x => x.Nature == "班组");
                    var users2 = (from q1 in db.IQueryable<UserEntity>()
                                  join q2 in subDeptsQuery on q1.DepartmentId equals q2.DepartmentId
                                  select q1).ToList();

                    var total = (int)Math.Round(users2.Count * pct, MidpointRounding.AwayFromZero);
                    var list = new List<string>();
                    var random = new Random();
                    while (list.Count < total)
                    {
                        var index = random.Next(users2.Count);
                        list.Add(users2[index].UserId);
                        users2.RemoveAt(index);
                    }

                    var trainingQuery = from q1 in db.IQueryable<HumanDangerTrainingUserEntity>()
                                        join q2 in db.IQueryable<ActivityEvaluateEntity>() on q1.TrainingUserId.ToString() equals q2.Activityid into into2
                                        where list.Contains(q1.UserId) && q1.IsDone == true && q1.IsMarked && q1.TrainingTime >= begin && q1.TrainingTime < end && into2.Count(x => x.EvaluateDeptId == item.DepartmentId) == 0
                                        select q1;
                    var trainings = trainingQuery.ToList();
                    entities.AddRange(trainings.Select(x => new ToEvaluateEntity() { BusinessId = x.TrainingUserId.ToString(), EvaluateDeptId = item.DepartmentId, StartDate = begin, EndDate = end, ToEvaluateId = Guid.NewGuid().ToString() }));
                }
            }

            try
            {
                db.Insert(entities);
                IWarningService service = new WarningService();

                foreach (var item in entities)
                {
                    service.SendWarning(category, item.ToEvaluateId.ToString());
                }

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
