using BSFramework.Application.Entity.Activity;
using BSFramework.Application.IService.Activity;
using BSFramework.Data.EF;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.Activity
{
    public class ActivitySubjectService : IActivitySubjectService
    {

        private System.Data.Entity.DbContext _context;

        public ActivitySubjectService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        public void DeleteSubject(string id)
        {
            var set = _context.Set<ActivitySubjectEntity>();
            var entity = set.Find(id);
            if (entity != null)
            {
                set.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void EditSubject(ActivitySubjectEntity activitySubjectEntity)
        {
            var set = _context.Set<ActivitySubjectEntity>();
            var entity = set.Find(activitySubjectEntity.ActivitySubjectId);
            if (entity == null) set.Add(activitySubjectEntity);
            else
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                entity.ActivitySubject = activitySubjectEntity.ActivitySubject;
                entity.SubjectType = activitySubjectEntity.SubjectType;
                entity.Status = activitySubjectEntity.Status;
                entity.Seq = activitySubjectEntity.Seq;
            }

            _context.SaveChanges();
        }

        public ActivitySubjectEntity Get(string id)
        {
            var set = _context.Set<ActivitySubjectEntity>();
            var entity = set.Find(id);
            return entity;
        }

        public List<ActivitySubjectEntity> GetActiveSubjects()
        {
            var set = _context.Set<ActivitySubjectEntity>();
            var query = set.AsNoTracking().AsQueryable()
                .Where(x => x.Status == "有效");
            var data = query.OrderBy(x => x.Seq).ToList();
            return data;
        }

        public List<ActivitySubjectEntity> GetSubjects(int pagesize, int pageindex, out int total)
        {
            var set = _context.Set<ActivitySubjectEntity>();
            var query = set.AsNoTracking().AsQueryable();
            total = query.Count();
            var data = query.OrderBy(x => x.Seq)
            .Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList();
            return data;
        }
    }
}
