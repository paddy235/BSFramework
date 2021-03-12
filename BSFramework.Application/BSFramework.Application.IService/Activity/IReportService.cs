using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;

namespace BSFramework.Application.IService.Activity
{
    public interface IReportService
    {
        List<ReportEntity> GetReports(string type, DateTime? start, DateTime? end, string key, int pagesize, int pageindex, out int total);
        ReportEntity GetDetail(string id);
        List<ReportEntity> BuildAllReport(DateTime start, DateTime end, string reporttype);
        ReportSettingEntity GetSetting(string reporttype);
        List<ReportEntity> GetReports(string category, string reportType, string reportuserid, DateTime? startTime, DateTime? endTime, string p, int pageSize, int pageIndex, out int total, string userId, bool? unread);
        ReportEntity Submit(ReportEntity data, string userid);
        ReportEntity GetReport(string id, string userid);
        string[] Share(string id, string[] users);
        List<ReportEntity> GetReportsByUser(string userId, int pageSize, int pageIndex, out int total);
        void Comment(CommentEntity data);
        void Setting(List<ReportSettingEntity> list);
        List<ReportSettingEntity> GetSettings();
        List<ItemEntity> GetSubmitPerson(string deptid);
        List<ReportEntity> GetAllReport(string userid, int pagesize, int pageindex, out int total);
        void EditReport(ReportEntity data);
    }
}
