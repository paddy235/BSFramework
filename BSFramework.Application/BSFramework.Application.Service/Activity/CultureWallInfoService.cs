using BSFramework.Application.IService.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.Activity;
using Bst.Bzzd.DataSource;
using BSFramework.Data.Repository;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Application.Entity.EducationManage;
using Bst.Bzzd.DataSource.Entities;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.ClutureWallManage;

namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 工作总结实现
    /// </summary>
    public class CultureWallInfoService : ICultureWallInfoService
    {
        /// <summary>
        /// 保存或编辑文化墙信息
        /// </summary>
        /// <param name="data"></param>
        public void SaveOrUpdate(CultureWallInfoEntity data)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.CultureWallInfos.Where(x => x.departmentid == data.departmentid).FirstOrDefault();
                if (entity != null)
                {
                    //edit
                    if (!string.IsNullOrEmpty(data.savetype))
                    {
                        if (data.savetype.ToLower() == "summary")
                        {
                            entity.summary = data.summary;
                            entity.summarydate = data.summarydate;
                        }
                        if (data.savetype.ToLower() == "slogan")
                        {
                            entity.slogan = data.slogan;
                            entity.slogandate = data.slogandate;
                        }
                        if (data.savetype.ToLower() == "vision")
                        {
                            entity.vision = data.vision;
                            entity.visiondate = data.visiondate;
                        }
                        if (data.savetype.ToLower() == "concept")
                        {
                            entity.concept = data.concept;
                            entity.conceptdate = data.conceptdate;
                        }
                    }
                    ctx.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    entity = new CultureWall();
                    //add
                    entity.wallinfoid = Guid.NewGuid();
                    entity.departmentid = data.departmentid;
                    entity.departmentname = data.departmentname;
                    if (!string.IsNullOrEmpty(data.savetype))
                    {
                        if (data.savetype.ToLower() == "summary")
                        {
                            entity.summary = data.summary;
                        }
                        if (data.savetype.ToLower() == "slogan")
                        {
                            entity.slogan = data.slogan;
                        }
                        if (data.savetype.ToLower() == "vision")
                        {
                            entity.vision = data.vision;
                        }
                        if (data.savetype.ToLower() == "concept")
                        {
                            entity.concept = data.concept;
                        }
                    }
                    entity.createtime = DateTime.Now;
                    entity.createuserid = data.createuserid;
                    ctx.CultureWallInfos.Add(entity);
                }
                ctx.SaveChanges();
            }
        }

        /// <summary>
        /// 得到班组文化墙信息
        /// </summary>
        /// <param name="bzId"></param>
        /// <returns></returns>
        public Entity.ClutureWallManage.CultureWallInfoEntity GetEntity(string bzId)
        {
            using (var ctx = new DataContext())
            {
                var query = from c in ctx.CultureWallInfos select c;
                query = query.Where(x => x.departmentid == bzId);

                var entity = query.Select(x => new CultureWallInfoEntity()
                {
                    wallinfoid = x.wallinfoid,
                    departmentid = x.departmentid,
                    departmentname = x.departmentname,
                    summary = x.summary,
                    summarydate = x.summarydate,
                    slogan = x.slogan,
                    slogandate = x.slogandate,
                    vision = x.vision,
                    visiondate = x.visiondate,
                    concept = x.concept,
                    conceptdate = x.visiondate,
                    createtime = x.createtime,
                    createuserid = x.createuserid
                }).FirstOrDefault();
                return entity;
            }
        }
    }
}
