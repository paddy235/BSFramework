using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.Entity.ProducingCheck;
using BSFramework.Application.IService.ProducingCheck;
using BSFramework.Data.EF;
using BSFramework.Data.Repository;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Util.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BSFramework.Application.Service.ProducingCheck
{
    /// <summary>
    /// 安全文明生产检查问题类型
    /// </summary>
    public class CheckTemplateService : ICheckTemplateService
    {
        private System.Data.Entity.DbContext _context;

        public CheckTemplateService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        public void Delete(string id)
        {
            var entity = _context.Set<CheckTemplateEntity>().Find(id);
            if (entity != null)
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                _context.SaveChanges();
            }
        }

        public void Edit(CheckTemplateEntity checkTemplate)
        {
            var entity = _context.Set<CheckTemplateEntity>().Find(checkTemplate.TemplateId);
            if (entity == null) _context.Set<CheckTemplateEntity>().Add(checkTemplate);
            else
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;

                entity.DistrictId = checkTemplate.DistrictId;
                entity.DistrictName = checkTemplate.DistrictName;
                entity.DutyDepartmentId = checkTemplate.DutyDepartmentId;
                entity.DutyDepartmentName = checkTemplate.DutyDepartmentName;
                entity.CategoryId = checkTemplate.CategoryId;
                entity.CategoryName = checkTemplate.CategoryName;
                entity.ProblemContent = checkTemplate.ProblemContent;
                entity.ProblemMeasure = checkTemplate.ProblemMeasure;
                entity.ModifyDate = checkTemplate.ModifyDate;
                entity.ModifyUserId = checkTemplate.ModifyUserId;
            }
            _context.SaveChanges();
        }

        public List<CheckTemplateEntity> Get(string categoryid, string districtId, string key, int pageSize, int pageIndex, out int total)
        {
            var query = _context.Set<CheckTemplateEntity>().AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(categoryid)) query = query.Where(x => x.CategoryId == categoryid);
            if (!string.IsNullOrEmpty(districtId)) query = query.Where(x => x.DistrictId == districtId);
            if (!string.IsNullOrEmpty(key)) query = query.Where(x => x.ProblemContent.Contains(key) || x.ProblemMeasure.Contains(key));

            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public CheckTemplateEntity Get(string id)
        {
            var entity = _context.Set<CheckTemplateEntity>().Find(id);
            return entity;
        }

        public void Save(List<CheckTemplateEntity> data)
        {
            if (data != null && data.Count > 0)
            {
                _context.Set<CheckTemplateEntity>().AddRange(data);
                _context.SaveChanges();
            }
        }
    }
}


