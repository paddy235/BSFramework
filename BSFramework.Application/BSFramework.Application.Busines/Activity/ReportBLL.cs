using Aspose.Cells;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using BSFramework.Application.Service.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    public class ReportBLL
    {
        private IReportService _reportService;

        public ReportBLL()
        {
            _reportService = new ReportService();
        }


        public List<ReportEntity> GetAllReport(string userid, int pagesize, int pageindex, out int total)
        {
            IReportService service = new ReportService();
            return service.GetAllReport(userid, pagesize, pageindex, out total);
        }

        public List<ReportEntity> GetReports(string type, DateTime? start, DateTime? end, string key, int pagesize, int pageindex, out int total)
        {
            IReportService service = new ReportService();
            return service.GetReports(type, start, end, key, pagesize, pageindex, out total);
        }

        public ReportEntity GetDetail(string id)
        {
            IReportService service = new ReportService();
            return service.GetDetail(id);
        }

        public List<ReportEntity> Build(string reporttype)
        {
            IReportService service = new ReportService();
            var setting = service.GetSetting(reporttype);
            var start = DateTime.MinValue;
            var end = DateTime.MinValue;
            this.BuildPiror(setting, out start, out end);
            var data = service.BuildAllReport(start, end, reporttype);

            var filelist = new List<FileInfoEntity>();
            var folder = ConfigurationManager.AppSettings["FilePath"];

            foreach (var item in data)
            {
                if (item.TaskList != null)
                {
                    var id = Guid.NewGuid();
                    var fileentity = new FileInfoEntity()
                    {
                        FileId = id.ToString(),
                        FileName = string.Format("{0}{1}{2}", item.ReportUser, item.ReportType, ".xlsx"),
                        FilePath = string.Format("~/Resource/Report/{0}.xlsx", id),
                        FileExtensions = ".xlsx",
                        FileType = "xlsx",
                        DeleteMark = 0,
                        EnabledMark = 1,
                        CreateDate = DateTime.Now,
                        CreateUserId = "system",
                        CreateUserName = "bzzd",
                        ModifyDate = DateTime.Now,
                        ModifyUserId = "system",
                        ModifyUserName = "bzzd",
                        RecId = item.ReportId.ToString()
                    };

                    var book = new Workbook();
                    var sheet = book.Worksheets[0];
                    sheet.Name = "任务清单";
                    sheet.Cells[0, 0].PutValue("序号");
                    sheet.Cells[0, 1].PutValue("工作时间");
                    sheet.Cells[0, 2].PutValue("工作任务");
                    sheet.Cells[0, 3].PutValue("作业人");

                    var style = sheet.Cells[0, 0].GetStyle();
                    style.ForegroundColor = System.Drawing.ColorTranslator.FromHtml("#C6EFCE");
                    style.Pattern = BackgroundType.Solid;
                    var range = sheet.Cells.CreateRange(0, 0, 1, 4);
                    range.ApplyStyle(style, new StyleFlag() { All = true });

                    for (int i = 0; i < item.TaskList.Count; i++)
                    {
                        sheet.Cells[i + 1, 0].PutValue(i + 1);
                        sheet.Cells[i + 1, 1].PutValue(item.TaskList[i].TaskPrior);
                        sheet.Cells[i + 1, 2].PutValue(item.TaskList[i].TaskContent);
                        sheet.Cells[i + 1, 3].PutValue(item.TaskList[i].TaskPerson);
                    }

                    sheet.Cells.SetColumnWidthPixel(0, 100);
                    sheet.Cells.SetColumnWidthPixel(1, 200);
                    sheet.Cells.SetColumnWidthPixel(2, 600);
                    sheet.Cells.SetColumnWidthPixel(3, 300);

                    if (!Directory.Exists(Path.Combine(folder, "Report")))
                        Directory.CreateDirectory(Path.Combine(folder, "Report"));

                    book.Save(Path.Combine(folder, "Report", id.ToString() + ".xlsx"), SaveFormat.Xlsx);

                    filelist.Add(fileentity);
                }
            }

            if (filelist.Count > 0)
            {
                var fileservice = new FileInfoService();
                fileservice.SaveFiles(filelist);
            }

            return data;
        }

        private void BuildPiror(ReportSettingEntity setting, out DateTime start, out DateTime end)
        {
            if (setting.SettingName == "周工作总结")
            {
                var now = DateTime.Now;
                var date = new DateTime(now.Year, now.Month, now.Day, setting.StartTime.Hour, setting.StartTime.Minute, setting.StartTime.Second);
                while ((int)date.DayOfWeek != setting.End)
                {
                    date = date.AddDays(-1);
                }
                end = date;
                start = end.AddDays(-7);
            }
            else
            {
                var now = DateTime.Now;
                end = new DateTime(now.Year, now.Month, setting.End, setting.StartTime.Hour, setting.StartTime.Minute, setting.StartTime.Second);
                start = end.AddMonths(-1);
            }
        }

        public List<ReportSettingEntity> GetSettings()
        {
            IReportService service = new ReportService();
            return service.GetSettings();
        }

        public void Setting(List<ReportSettingEntity> list)
        {
            IReportService service = new ReportService();
            service.Setting(list);
        }

        private string GetDay(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "周日";
                case DayOfWeek.Monday:
                    return "周一";
                case DayOfWeek.Tuesday:
                    return "周二";
                case DayOfWeek.Wednesday:
                    return "周三";
                case DayOfWeek.Thursday:
                    return "周四";
                case DayOfWeek.Friday:
                    return "周五";
                case DayOfWeek.Saturday:
                    return "周六";
                default:
                    return null;
            }
        }

        public List<ReportEntity> GetReports(string category, string reportType, string reportuserid, DateTime? startTime, DateTime? endTime, string p, int pageSize, int pageIndex, out int total, string userId, bool? unread)
        {
            IReportService service = new ReportService();
            return service.GetReports(category, reportType, reportuserid, startTime, endTime, p, pageSize, pageIndex, out total, userId, unread);
        }

        public ReportEntity Submit(ReportEntity data)
        {
            IReportService service = new ReportService();
            return service.Submit(data, data.ReportUserId);
        }

        public ReportEntity GetReport(string id, string userid)
        {
            IReportService service = new ReportService();
            return service.GetReport(id, userid);
        }

        public void EditReport(ReportEntity data)
        {
            _reportService.EditReport(data);
        }

        public string[] Share(string id, string[] users)
        {
            IReportService service = new ReportService();
            return service.Share(id, users);
        }

        public List<ReportEntity> GetReportsByUser(string userId, int pageSize, int pageIndex, out int total)
        {
            IReportService service = new ReportService();
            return service.GetReportsByUser(userId, pageSize, pageIndex, out total);
        }

        public void Comment(CommentEntity data)
        {
            IReportService service = new ReportService();
            service.Comment(data);
        }

        public object GetSubmitPerson(string deptid)
        {
            IReportService service = new ReportService();
            return service.GetSubmitPerson(deptid);
        }
    }
}
