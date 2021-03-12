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
    public class DepartmentPublishService : RepositoryFactory<DepartmentPublishEntity>, IDepartmentPublishService
    {
        public DepartmentPublishEntity Add(DepartmentPublishEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (model.Files != null)
                {
                    foreach (var item in model.Files)
                    {
                        item.RecId = model.PublishId;
                    }
                }
                db.Insert(model);
                db.Insert(model.Files);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

            return model;
        }

        public void Delete(string data)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete<DepartmentPublishEntity>(data);
                db.Delete<FileInfoEntity>(x => x.RecId == data);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public DepartmentPublishEntity Edit(DepartmentPublishEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = (from q in db.IQueryable<DepartmentPublishEntity>()
                              where q.PublishId == model.PublishId
                              select q).FirstOrDefault();

                var files = (from q in db.IQueryable<FileInfoEntity>()
                             where q.RecId == model.PublishId
                             select q).ToList();

                var deleteitems = files.Where(x => model.Files.All(y => y.FileId != x.FileId)).ToList();
                var newitems = model.Files.Where(x => files.All(y => y.FileId != x.FileId)).ToList();

                if (entity != null)
                {
                    entity.Content = model.Content;
                    entity.DeptId = model.DeptId;
                    entity.DeptName = model.DeptName;

                    db.Update(entity);
                }

                db.Delete(deleteitems);
                newitems.ForEach(x => x.RecId = model.PublishId);
                db.Insert(newitems);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

            return model;
        }

        public List<DepartmentPublishEntity> List(string deptId)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q1 in db.IQueryable<DepartmentPublishEntity>()
                        join q2 in db.IQueryable<FileInfoEntity>() on q1.PublishId equals q2.RecId into into2
                        where q1.DeptId == deptId
                        orderby q1.CreateDate descending
                        select new { q1, q2 = into2 };
            var data = query.Take(5);
            foreach (var item in data)
            {
                item.q1.Files = item.q2.ToList();
            }
            return data.Select(x => x.q1).ToList();
        }
    }
}
