
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.IService.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public interface ICultureService
    {
        void AddCulture(CultureTemplateEntity model);
        CultureTemplateEntity GetCulture(string groupid);
        IList<CultureTemplateEntity> GetData(string name, int rows, int page, out int total);
        CultureTemplateEntity GetTemplate(string id);
        CultureTemplateItemEntity GetTemplateContent(string id);
        void EditCulture(CultureTemplateEntity model);
        int GetPersonTotal(string deptId);
        double GetAvgage(string deptId);
        int GetTotal1(string deptid, DateTime now);
        int GetTotal2(string deptid, DateTime now);
        double GetTotal3(string deptid, DateTime now);
        int GetTotal4(string deptid, DateTime now);
        FileInfoEntity GetImage1(string deptid, DateTime now);
        List<FileInfoEntity> GetImage2(string deptid, DateTime now);
        List<FileInfoEntity> GetImage3(string deptid, DateTime now);
        List<FileInfoEntity> GetImage4(string deptid, DateTime now);
        List<NewsEntity> GetNotices(string deptid);
    }
}
