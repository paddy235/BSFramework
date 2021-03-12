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
    public class CheckCategoryService : ICheckCategoryService
    {
        private System.Data.Entity.DbContext _context;

        public CheckCategoryService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        public List<CheckCategoryEntity> GetCategories()
        {
            var query = from q in _context.Set<CheckCategoryEntity>()
                        orderby q.CreateDate ascending
                        select q;

            return query.ToList();
        }

        public void Edit(CheckCategoryEntity checkCategoryEntity)
        {
            var entity = _context.Set<CheckCategoryEntity>().Find(checkCategoryEntity.CategoryId);
            if (entity == null) _context.Set<CheckCategoryEntity>().Add(checkCategoryEntity);
            else
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                entity.CategoryName = checkCategoryEntity.CategoryName;
                entity.ParentId = checkCategoryEntity.ParentId;
            }
            _context.SaveChanges();
        }

        public CheckCategoryEntity Get(string id)
        {
            var entity = _context.Set<CheckCategoryEntity>().Find(id);
            if (entity != null && !string.IsNullOrEmpty(entity.ParentId))
                entity.ParentCategory = _context.Set<CheckCategoryEntity>().Find(entity.ParentId);

            return entity;
        }

        public void Delete(string id)
        {
            var query = from q in _context.Set<CheckCategoryEntity>()
                        where q.ParentId == id
                        select q;
            if (query.Count() > 0) throw new Exception("有子类别，不能删除！");

            if (_context.Set<CheckTemplateEntity>().AsNoTracking().Where(x => x.CategoryId == id).Count() > 0)
                throw new Exception("该类别下有问题数据，不能删除！");

            var entity = _context.Set<CheckCategoryEntity>().Find(id);
            if (entity != null)
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                _context.SaveChanges();
            }
        }
    }
}


