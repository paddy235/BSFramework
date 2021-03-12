using BSFramework.Application.Entity.ProducingCheck;
using BSFramework.Application.IService.ProducingCheck;
using BSFramework.Application.Service.ProducingCheck;
using System;
using System.Collections.Generic;

namespace BSFramework.Busines.ProducingCheck
{
    /// <summary>
    /// 安全文明生产检查问题库
    /// </summary>
    public class CheckTemplateBLL
    {
        private ICheckTemplateService checkTemplateService = new CheckTemplateService();

        public List<CheckTemplateEntity> Get(string categoryid, string districtId, string key, int pageSize, int pageIndex, out int total)
        {
            return checkTemplateService.Get(categoryid, districtId, key, pageSize, pageIndex, out total);
        }

        public void Edit(CheckTemplateEntity checkTemplate)
        {
            checkTemplateService.Edit(checkTemplate);
        }

        public CheckTemplateEntity Get(string id)
        {
            return checkTemplateService.Get(id);
        }

        public void Delete(string id)
        {
            checkTemplateService.Delete(id);
        }

        public void Save(List<CheckTemplateEntity> data)
        {
            checkTemplateService.Save(data);
        }
    }
}
