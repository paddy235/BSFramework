using BSFramework.Application.Entity.ProducingCheck;
using BSFramework.Application.IService.ProducingCheck;
using BSFramework.Application.Service.ProducingCheck;
using System;
using System.Collections.Generic;

namespace BSFramework.Busines.ProducingCheck
{
    /// <summary>
    /// 安全文明生产检查问题库类型
    /// </summary>
    public class CheckCategoryBLL
    {
        private ICheckCategoryService checkCategoryService = new CheckCategoryService();

        public List<CheckCategoryEntity> GetCategories()
        {
            return checkCategoryService.GetCategories();
        }

        public void Edit(CheckCategoryEntity checkCategoryEntity)
        {
            checkCategoryService.Edit(checkCategoryEntity);
        }

        public CheckCategoryEntity Get(string id)
        {
            return checkCategoryService.Get(id);
        }

        public void Delete(string id)
        {
            checkCategoryService.Delete(id);
        }
    }
}
