using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Data.EF;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.WorkMeeting
{
    /// <summary>
    /// 危险辨识
    /// </summary>
    public class DangerAnalysisService : IDangerAnalysisService
    {
        private System.Data.Entity.DbContext _context;

        /// <summary>
        /// ctor
        /// </summary>
        public DangerAnalysisService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        public void Copy(JobDangerousEntity model)
        {
            var set1 = _context.Set<JobDangerousEntity>();
            var set2 = _context.Set<JobMeasureEntity>();

            var danger = set1.Find(model.JobDangerousId);
            var measures = (from q in set2
                            where q.JobDangerousId == model.JobDangerousId
                            select q).ToList();

            var newdanger = new JobDangerousEntity()
            {
                JobDangerousId = Guid.NewGuid().ToString(),
                Content = danger.Content,
                CreateTime = DateTime.Now,
                JobId = model.JobId
            };
            var newmeasures = measures.Select(x => new JobMeasureEntity { JobDangerousId = newdanger.JobDangerousId, Content = x.Content, CreateTime = x.CreateTime, JobMeasureId = Guid.NewGuid().ToString() }).ToList();
            set1.Add(newdanger);
            set2.AddRange(newmeasures);
            _context.SaveChanges();
        }

        public void DeleteDanger(string id)
        {
            var set1 = _context.Set<JobDangerousEntity>();
            var set2 = _context.Set<JobMeasureEntity>();

            var danger = set1.Find(id);
            var measures = (from q in set2
                            where q.JobDangerousId == id
                            select q).ToList();

            set1.Remove(danger);
            set2.RemoveRange(measures);

            _context.SaveChanges();
        }

        public void Edit(DangerAnalysisEntity analysis)
        {
            var set = _context.Set<DangerAnalysisEntity>();
            var entity = set.Find(analysis.AnalysisId);
            _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            entity.MeetingId = analysis.MeetingId;
            entity.MeetingName = analysis.MeetingName;
            entity.MeetingTime = analysis.MeetingTime;
            entity.MeetingDate = analysis.MeetingDate;
            _context.SaveChanges();
        }

        /// <summary>
        /// 新增风险点
        /// </summary>
        /// <param name="danger"></param>
        public void EditDanger(JobDangerousEntity danger)
        {
            var set1 = _context.Set<JobDangerousEntity>();
            var set2 = _context.Set<JobMeasureEntity>();

            var entity = set1.Find(danger.JobDangerousId);
            if (entity == null)
            {
                set1.Add(danger);
                set2.AddRange(danger.MeasureList);
            }
            else
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                entity.Content = danger.Content;

                var subitems = (from q in set2
                                where q.JobDangerousId == danger.JobDangerousId
                                select q).ToList();
                set2.RemoveRange(subitems);
                set2.AddRange(danger.MeasureList);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// 查找风险点
        /// </summary>
        /// <param name="query"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<RiskFactorSetEntity> FindDanger(string key, int limit, string deptid)
        {
            var query = from q1 in _context.Set<RiskFactorSetEntity>()
                        join q2 in _context.Set<MeasureSetEntity>() on q1.ID equals q2.RiskFactorId into into2
                        where q1.DeptId.Contains(deptid) && q1.Content.Contains(key)
                        orderby q1.Content
                        select new { q1, q2 = into2 };
            var data = query.Take(limit).ToList();
            data.ForEach(x => x.q1.measures = x.q2.ToList());
            return data.Select(x => x.q1).ToList();
        }

        public DangerAnalysisEntity GetByMeeting(string meetingid)
        {
            var set = _context.Set<DangerAnalysisEntity>();

            var query = from q in set
                        where q.MeetingId == meetingid
                        orderby q.MeetingDate descending
                        select q;
            var entity = query.FirstOrDefault();
            if (entity != null)
            {
                var set2 = _context.Set<JobDangerousEntity>();
                var set3 = _context.Set<JobMeasureEntity>();
                var query2 = from q1 in set2
                             join q2 in set3 on q1.JobDangerousId equals q2.JobDangerousId into into2
                             where q1.JobId == entity.AnalysisId
                             select new { q1, q2 = into2 };
                var items = query2.ToList();
                items.ForEach(x => x.q1.MeasureList = x.q2.OrderBy(y => y.CreateTime).ToList());
                entity.Dangers = items.Select(x => x.q1).ToList();
            }

            return entity;
        }

        /// <summary>
        /// 查看风险点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JobDangerousEntity GetDanger(string id)
        {
            var query = from q1 in _context.Set<JobDangerousEntity>()
                        join q2 in _context.Set<JobMeasureEntity>() on q1.JobDangerousId equals q2.JobDangerousId into into2
                        where q1.JobDangerousId == id
                        select new { q1, q2 = into2 };
            var data = query.FirstOrDefault();
            if (data != null)
            {
                data.q1.MeasureList = data.q2.OrderBy(x => x.CreateTime).ToList();
            }
            return data?.q1;
        }

        /// <summary>
        /// 获取最后一次安全交底
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public DangerAnalysisEntity GetLast(string deptid)
        {
            var set = _context.Set<DangerAnalysisEntity>();

            var query = from q in set
                        where q.DeptId == deptid && q.MeetingId == null
                        orderby q.MeetingDate descending
                        select q;
            var entity = query.FirstOrDefault();
            if (entity != null)
            {
                var set2 = _context.Set<JobDangerousEntity>();
                var set3 = _context.Set<JobMeasureEntity>();
                var query2 = from q1 in set2
                             join q2 in set3 on q1.JobDangerousId equals q2.JobDangerousId into into2
                             where q1.JobId == entity.AnalysisId
                             select new { q1, q2 = into2 };
                var items = query2.ToList();
                items.ForEach(x => x.q1.MeasureList = x.q2.OrderBy(y => y.CreateTime).ToList());
                entity.Dangers = items.Select(x => x.q1).ToList();
            }

            return entity;
        }

        /// <summary>
        /// 初始化安全交底
        /// </summary>
        /// <param name="entity"></param>
        public void Init(DangerAnalysisEntity entity)
        {
            var set = _context.Set<DangerAnalysisEntity>();

            set.Add(entity);

            _context.SaveChanges();
        }

        public DangerAnalysisEntity Prev(string deptid, string id)
        {
            var set = _context.Set<DangerAnalysisEntity>();

            var query = from q in set
                        where q.DeptId == deptid && q.AnalysisId != id
                        orderby q.MeetingDate descending
                        select q;
            var entity = query.FirstOrDefault();
            if (entity != null)
            {
                var set2 = _context.Set<JobDangerousEntity>();
                var set3 = _context.Set<JobMeasureEntity>();
                var query2 = from q1 in set2
                             join q2 in set3 on q1.JobDangerousId equals q2.JobDangerousId into into2
                             where q1.JobId == entity.AnalysisId
                             select new { q1, q2 = into2 };
                var items = query2.ToList();
                items.ForEach(x => x.q1.MeasureList = x.q2.ToList());
                entity.Dangers = items.Select(x => x.q1).ToList();
            }

            return entity;
        }
    }
}
