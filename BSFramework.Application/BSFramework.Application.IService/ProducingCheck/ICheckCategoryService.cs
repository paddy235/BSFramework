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
    /// ��ȫ������������������
    /// </summary>
    public interface ICheckCategoryService
    {
        List<CheckCategoryEntity> GetCategories();
        void Edit(CheckCategoryEntity checkCategoryEntity);
        CheckCategoryEntity Get(string id);
        void Delete(string id);
    }
}
