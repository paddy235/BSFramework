using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BSFramework.Application.Service.EducationManage
{
    public class EduAnswerService : RepositoryFactory<EduAnswerEntity>, IEduAnswerService
    {

        public readonly System.Data.Entity.DbContext _context;
        private readonly DbSet<EduAnswerEntity> eduAnswerEntities;

        public EduAnswerService()
        {
            _context = (DbFactory.Base() as Data.EF.Database).dbcontext;
            eduAnswerEntities = _context.Set<EduAnswerEntity>();
        }


        //未使用
        public IEnumerable<MainCount> FindCount(string deptcode)
        {
            //            var query = new RepositoryFactory<MainCount>().BaseRepository();
            //            var sql = string.Format(@"select b1.jsjk BZ_JSJK,b2.jswd BZ_JSWD,b3.sgyx BZ_SGYX,b4.fsgyx BZ_FSGXX,c.total BZ_BQBHH,d.total BZ_AQRHD,
            //		e.total BZ_WXYZXL,f.total BZ_WCRW,g.total BZ_SYRW,h.total BZ_ZZXX,i.total BZ_LDBHJC,j.total BZ_BWH,k.total BZ_MZGLH,
            //		l.total BZ_ZDXX
            //from (select count(1) jsjk from wg_edubaseinfo where edutype = '1' 
            //   and bzid in(select departmentid from base_department where encode like '{0}%')
            //	 and DATE_FORMAT(activitydate,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) b1,
            //(select count(1) jswd from wg_edubaseinfo where (edutype = '2' or edutype = '5')
            //   and bzid in(select departmentid from base_department where encode like '{0}%')
            //	 and DATE_FORMAT(activitydate,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) b2,
            //(select count(1) sgyx from wg_edubaseinfo where edutype = '3'
            //   and bzid in(select departmentid from base_department where encode like '{0}%')
            //   and DATE_FORMAT(activitydate,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) b3,
            //(select count(1) fsgyx from wg_edubaseinfo where edutype = '4'
            //   and bzid in(select departmentid from base_department where encode like '{0}%')
            //   and DATE_FORMAT(activitydate,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) b4,
            //(select count(1) total from wg_workmeeting where isover = 1
            //   and groupid in(select departmentid from base_department where encode like '{0}%')
            //		and DATE_FORMAT(meetingstarttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) c,
            //(select count(1) total from wg_activity where activitytype = '安全日活动'
            //	 and state = 'Finish'
            //   and groupid in(select departmentid from base_department where encode like '{0}%')
            //   and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) d,
            //(select count(1) total from wg_danger where 1 = 1 
            //   and groupid in(select departmentid from base_department where encode like '{0}%')
            //   and DATE_FORMAT(jobtime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) e,
            //(select sum(total) total from (select w.groupid,w.meetingid,count(c.jobid) total from wg_workmeeting w 
            //						left join wg_meetingandjob j
            //            on w.meetingid = j.endmeetingid
            //						left join wg_meetingjob c
            //						on j.jobid = c.jobid
            //             and c.isfinished = 'finish'
            //						where w.meetingtype='班后会' and DATE_FORMAT(w.meetingstarttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')
            //						and w.groupid in(select departmentid from base_department where encode like '{0}%')
            //            group by w.meetingid) f1) f,
            //(select sum(total) total from (select w.groupid,w.meetingid,count(c.jobid) total from wg_workmeeting w 
            //						left join wg_meetingandjob j
            //            on w.meetingid = j.endmeetingid
            //						left join wg_meetingjob c
            //						on j.jobid = c.jobid
            //						where w.meetingtype='班后会' and DATE_FORMAT(w.meetingstarttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')
            //						and w.groupid in(select departmentid from base_department where encode like '{0}%')
            //            group by w.meetingid) f1) g,
            //(select count(1) total from wg_activity where activitytype = '政治学习'
            //						and state = 'Finish'
            //						and groupid in(select departmentid from base_department where encode like '{0}%')
            //						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) h,
            //(select count(1) total from wg_activity where activitytype = '劳动保护监查'
            //						and state = 'Finish'
            //						and groupid in(select departmentid from base_department where encode like '{0}%')
            //						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) i,
            //(select count(1) total from wg_activity where activitytype = '班务会'
            //						and state = 'Finish'
            //						and groupid in(select departmentid from base_department where encode like '{0}%')
            //						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) j,
            //(select count(1) total from wg_activity where activitytype = '民族管理会'
            //						and state = 'Finish'
            //						and groupid in(select departmentid from base_department where encode like '{0}%')
            //						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) k,
            //(select count(1) total from wg_activity where activitytype = '制度学习'
            //						and state = 'Finish'
            //						and groupid in(select departmentid from base_department where encode like '{0}%')
            //						and DATE_FORMAT(starttime,'%Y%m') = DATE_FORMAT(CURDATE(),'%Y%m')) l", deptcode);
            //            var data = query.FindList(sql).ToList();
            //            return data;
            return new List<MainCount>();
        }
        public IEnumerable<EduAnswerEntity> GetList(string eduid)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(eduid))
            {
                query = query.Where(x => x.EduId == eduid);
            }
            var data = query.OrderByDescending(x => x.CreateDate).ToList();
            foreach (EduAnswerEntity an in data)
            {
                an.Files = new Repository<FileInfoEntity>(DbFactory.Base()).IQueryable().Where(x => x.RecId == an.ID).ToList();
            }
            return data;
        }

        public IEnumerable<EduAnswerEntity> GetPageList(string deptid, int page, int pagesize, out int total)
        {
            //string deptid = OperatorProvider.Provider.Current().DeptId;
            var query = this.BaseRepository().IQueryable();
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }

        public EduAnswerEntity GetEntity(string keyValue)
        {
            EduAnswerEntity entity = this.BaseRepository().FindEntity(keyValue);
            if (entity != null)
            {
                entity.Files = new Repository<FileInfoEntity>(DbFactory.Base()).IQueryable().Where(x => x.RecId == entity.ID).ToList();
            }
            return entity;
        }
        public List<TestQuestionsEntity> FindTrainings(string key, int limit)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = db.IQueryable<TestQuestionsEntity>().Where(x => x.Theme.Contains(key)).ToList();

            var data = query.Take(limit).ToList();

            return data;
        }
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
        public void SaveForm(string keyValue, EduAnswerEntity entity)
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
                entity1.Files = null;
                entity1.Files1 = null;
                entity1.Grade = entity.Grade;
                entity1.AppraiseContent = entity.AppraiseContent;
                this.BaseRepository().Update(entity1);
            }
        }

        /// <summary>
        /// 保存技术问答评论
        /// </summary>
        /// <param name="data"></param>
        public void SaveAnswerComment(List<EduAnswerEntity> data, EduBaseInfoEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();

            var oldEntity = db.FindEntity<EduBaseInfoEntity>(entity.ID);
            var list = db.FindList<EduAnswerEntity>(x => x.EduId == entity.ID).ToList();

            oldEntity.Teacher = entity.Teacher;
            oldEntity.TeacherId = entity.TeacherId;
            oldEntity.RegisterPeople = entity.RegisterPeople;
            oldEntity.RegisterPeopleId = entity.RegisterPeopleId;
            oldEntity.Theme = entity.Theme;
            oldEntity.RunWay = entity.RunWay;
            oldEntity.Answers = null;
            oldEntity.Files = null;

            //遍历
            foreach (var item in list)
            {
                var newItem = data.Where(s => s.ID == item.ID).FirstOrDefault();
                if (newItem != null)
                {
                    if (string.IsNullOrEmpty(newItem.Grade)) item.Grade = "0";
                    else item.Grade = newItem.Grade;
                    item.AppraiseContent = newItem.AppraiseContent;
                    item.Files = null;
                }
            }

            try
            {
                db.Update(list);
                db.Update(oldEntity);
                db.Commit();
            }
            catch
            {
                db.Rollback();
            }

        }

        public void Add(EduAnswerEntity entity)
        {
            eduAnswerEntities.Add(entity);
            _context.SaveChanges();
        }

        public EduAnswerEntity Get(string id)
        {
            var entity = eduAnswerEntities.Find(id);
            _context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public void Edit(EduAnswerEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(EduAnswerEntity entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public List<EduAnswerEntity> List(string baseId)
        {
            var query = eduAnswerEntities.AsNoTracking().AsQueryable();
            query = query.Where(x => x.EduId == baseId);
            return query.ToList();
        }
        #endregion
    }
}
