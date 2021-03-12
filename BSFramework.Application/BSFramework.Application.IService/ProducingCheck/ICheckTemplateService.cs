using BSFramework.Application.Entity.BaseManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using BSFramework.Entity.EvaluateAbout;
using System.Data;
using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.Entity.ProducingCheck;

namespace BSFramework.Application.IService.ProducingCheck
{
    /// <summary>
    /// 安全文明生产检查问题库
    /// </summary>
    public interface ICheckTemplateService
    {
        List<CheckTemplateEntity> Get(string categoryid, string districtId, string key, int pageSize, int pageIndex, out int total);
        void Edit(CheckTemplateEntity checkTemplate);
        CheckTemplateEntity Get(string id);
        void Delete(string id);
        void Save(List<CheckTemplateEntity> data);
    }
}
