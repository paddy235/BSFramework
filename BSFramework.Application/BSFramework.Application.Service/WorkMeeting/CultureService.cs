using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using BSFramework.Util.Extension;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Data.Common;
using BSFramework.Data;
using System.Text;
using System.Configuration;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.Entity.Activity;

namespace BSFramework.Service.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class CultureService : RepositoryFactory<CultureTemplateEntity>, ICultureService
    {
        public void AddCulture(CultureTemplateEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var contents = model.Contents;
                model.Contents = null;
                db.Insert(model);
                db.Insert(contents);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }

        public void EditCulture(CultureTemplateEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<CultureTemplateEntity>(model.CultureTemplateId);
                entity.CultureTemplateSubject = model.CultureTemplateSubject;
                entity.GroupId = model.GroupId;
                entity.GroupName = model.GroupName;
                db.Update(entity);

                var contents = db.IQueryable<CultureTemplateItemEntity>().Where(x => x.CultureTemplateId == model.CultureTemplateId).ToList();
                foreach (var item in contents)
                {
                    var old = model.Contents.Find(x => x.CultureTemplateItemId == item.CultureTemplateItemId);
                    item.ContentSubject = old.ContentSubject;
                    item.CultureContent = old.CultureContent;
                }
                db.Update(contents);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }

        public double GetAvgage(string deptId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<PeopleEntity>()
                        where q1.BZID == deptId
                        select string.IsNullOrEmpty(q1.Age) ? "0" : q1.Age;
            return query.ToList().Select(x => int.Parse(x)).Average();
        }

        public CultureTemplateEntity GetCulture(string groupid)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<CultureTemplateEntity>()
                        where q1.GroupId == groupid
                        orderby q1.CreateTime descending
                        select q1;

            var entity = query.FirstOrDefault();
            if (entity != null)
                entity.Contents = db.IQueryable<CultureTemplateItemEntity>().Where(x => x.CultureTemplateId == entity.CultureTemplateId).OrderBy(x => x.CreateTime).ToList();

            return entity;
        }

        public IList<CultureTemplateEntity> GetData(string name, int rows, int page, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<CultureTemplateEntity>()
                        select q;

            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.CultureTemplateSubject.Contains(name));

            total = query.Count();
            var data = query.ToList();

            return data;
        }

        public int GetTotal1(string deptid, DateTime now)
        {
            var date1 = new DateTime(now.Year, now.Month, 1);
            var date2 = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);

            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<LllegalEntity>()
                        where q1.LllegalTeamId == deptid && q1.ApproveResult == "0" && q1.LllegalTime >= date1 && q1.LllegalTime < date2
                        select q1;
            return query.Count();
        }

        public int GetTotal2(string deptid, DateTime now)
        {
            var date1 = new DateTime(now.Year, now.Month, 1);
            var date2 = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);

            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<MeetingJobEntity>()
                        join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q3.JobId
                        join q2 in db.IQueryable<WorkmeetingEntity>() on q3.EndMeetingId equals q2.MeetingId
                        where q2.MeetingType == "班后会" && q2.GroupId == deptid && q1.IsFinished == "finish" && q1.CreateDate >= date1 && q1.CreateDate < date2
                        select q1;
            return query.Count();
        }

        public double GetTotal3(string deptid, DateTime now)
        {
            var date1 = new DateTime(now.Year, now.Month, 1);
            var date2 = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);

            var db = new RepositoryFactory().BaseRepository();
            var total1 = (from q1 in db.IQueryable<MeetingJobEntity>()
                          join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q3.JobId
                          join q2 in db.IQueryable<WorkmeetingEntity>() on q3.EndMeetingId equals q2.MeetingId
                          where q2.MeetingType == "班后会" && q2.GroupId == deptid && q1.IsFinished == "finish" && q1.CreateDate >= date1 && q1.CreateDate < date2
                          select q1).Count();
            var total2 = (from q1 in db.IQueryable<MeetingJobEntity>()
                          join q3 in db.IQueryable<MeetingAndJobEntity>() on q1.JobId equals q3.JobId
                          join q2 in db.IQueryable<WorkmeetingEntity>() on q3.EndMeetingId equals q2.MeetingId
                          where q2.MeetingType == "班后会" && q2.GroupId == deptid && q1.CreateDate >= date1 && q1.CreateDate < date2
                          select q1).Count();
            return (double)total1 / (total2 == 0 ? 1 : total2);
        }

        public int GetTotal4(string deptid, DateTime now)
        {
            var date1 = new DateTime(now.Year, now.Month, 1);
            var date2 = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1);

            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<DangerEntity>()
                        where q1.GroupId == deptid && q1.CreateDate >= date1 && q1.CreateDate < date2 && q1.Status == 1
                        select q1;

            return query.Count();
        }

        public int GetPersonTotal(string deptId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<PeopleEntity>()
                        where q1.BZID == deptId
                        select q1;
            return query.Count();
        }

        public CultureTemplateEntity GetTemplate(string id)
        {
            var db = new RepositoryFactory().BaseRepository();

            var entity = db.FindEntity<CultureTemplateEntity>(id);
            if (entity != null)
                entity.Contents = db.IQueryable<CultureTemplateItemEntity>().Where(x => x.CultureTemplateId == entity.CultureTemplateId).OrderBy(x => x.CreateTime).ToList();

            return entity;
        }

        public CultureTemplateItemEntity GetTemplateContent(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<CultureTemplateItemEntity>(id);
            return entity;
        }

        public FileInfoEntity GetImage1(string deptid, DateTime now)
        {
            var date1 = new DateTime(now.Year, now.Month, now.Day).AddDays(-1);
            var date2 = new DateTime(now.Year, now.Month, now.Day);

            var db = new RepositoryFactory().BaseRepository();

            var query1 = from q1 in db.IQueryable<WorkmeetingEntity>()
                         where q1.GroupId == deptid && q1.MeetingType == "班前会" && q1.MeetingStartTime >= date1 && q1.MeetingStartTime < date2
                         select q1;

            var meeting = query1.FirstOrDefault();
            var result = default(FileInfoEntity);
            if (meeting != null)
            {
                var query2 = from q1 in db.IQueryable<FileInfoEntity>()
                             where q1.RecId == meeting.MeetingId && q1.Description == "照片"
                             orderby q1.CreateDate ascending
                             select q1;
                result = query2.FirstOrDefault();
            }

            return result;
        }

        public List<FileInfoEntity> GetImage2(string deptid, DateTime now)
        {
            var date1 = new DateTime(now.Year, now.Month, now.Day).AddDays(-1);
            var date2 = new DateTime(now.Year, now.Month, now.Day);

            var db = new RepositoryFactory().BaseRepository();

            var query1 = from q1 in db.IQueryable<WorkmeetingEntity>()
                         where q1.GroupId == deptid && q1.MeetingType == "班后会" && q1.MeetingStartTime >= date1 && q1.MeetingStartTime < date2
                         select q1;

            var meeting = query1.FirstOrDefault();
            var result = default(List<FileInfoEntity>);
            if (meeting != null)
            {
                var query2 = from q1 in db.IQueryable<FileInfoEntity>()
                             where q1.RecId == meeting.MeetingId && q1.Description == "照片"
                             orderby q1.CreateDate ascending
                             select q1;

                result = query2.ToList().Select(x => new FileInfoEntity() { FileId = x.FileId, RecId = meeting.OtherMeetingId }).ToList();
            }

            return result;
        }

        public List<FileInfoEntity> GetImage3(string deptid, DateTime now)
        {
            var date1 = new DateTime(now.Year, now.Month, now.Day).AddDays(-1);
            var date2 = new DateTime(now.Year, now.Month, now.Day);

            var db = new RepositoryFactory().BaseRepository();

            var query1 = from q1 in db.IQueryable<ActivityEntity>()
                         where q1.GroupId == deptid && q1.ActivityType == "安全日活动"
                         orderby q1.CreateDate descending
                         select q1;

            var activity = query1.FirstOrDefault();
            var result = default(List<FileInfoEntity>);
            if (activity != null)
            {
                var query2 = from q1 in db.IQueryable<FileInfoEntity>()
                             where q1.RecId == activity.ActivityId && q1.Description == "照片"
                             orderby q1.CreateDate ascending
                             select q1;
                result = query2.ToList();
            }

            return result;
        }

        public List<NewsEntity> GetNotices(string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<NewsEntity>()
                        where q1.DeptId.Contains(deptid) && q1.TypeId == 2
                        orderby q1.CreateDate descending
                        select q1;
            return query.Take(5).ToList();
        }

        public List<FileInfoEntity> GetImage4(string deptid, DateTime now)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<FileInfoEntity>()
                        join q2 in db.IQueryable<WorkmeetingEntity>() on q1.RecId equals q2.MeetingId
                        where q1.Description == "照片"
                        orderby q1.CreateDate descending
                        select new { FileId = q1.FileId, RecId = q2.MeetingType == "班前后" ? q2.MeetingId : q2.OtherMeetingId };

            return query.Take(8).ToList().Select(x => new FileInfoEntity() { FileId = x.FileId, RecId = x.RecId }).ToList();
        }
    }
}
