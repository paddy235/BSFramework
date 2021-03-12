namespace Bst.Bzzd.DataSource.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Bst.Bzzd.DataSource.DataContext>
    {
        public Configuration()
        {
            //AutomaticMigrationsEnabled = true;
            //publish
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
            CodeGenerator = new MySql.Data.Entity.MySqlMigrationCodeGenerator();
        }

        protected override void Seed(Bst.Bzzd.DataSource.DataContext context)
        {
            context.ReportSettings.AddOrUpdate(x => x.SettingName, new Entities.ReportSetting() { SettingId = Guid.NewGuid(), SettingName = "周工作总结", StartTime = DateTime.Now, EndTime = DateTime.Now, Start = 0, End = 0 });
            context.ReportSettings.AddOrUpdate(x => x.SettingName, new Entities.ReportSetting() { SettingId = Guid.NewGuid(), SettingName = "月工作总结", StartTime = DateTime.Now, EndTime = DateTime.Now, Start = 1, End = 1 });

            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "工作提示", Enabled = true, Title = "工作任务", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.JobService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "评价班前班后会", Enabled = true, Title = "工作评价", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.MeetingService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "危险预知训练", Enabled = true, Title = "危险预知训练", Template = "{content}", Category = Entities.MessageCategory.Todo, RecieveType = "接收人", Assembly = "Bst.Fx.Message.TrainingService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "评价危险预知训练", Enabled = true, Title = "工作评价", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.TrainingEvaluationService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "活动材料下发", Enabled = true, Title = "活动材料", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "班长,安全员", Assembly = "Bst.Fx.Message.ActivitySourceService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "评价班组活动", Enabled = true, Title = "工作评价", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.ActivityService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "任务满百", Enabled = true, Title = "更上一层楼", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.JobAlertService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "评价工作总结", Enabled = true, Title = "工作评价", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.ReportEvaluateService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "周工作总结", Enabled = true, Title = "周工作总结", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.WeekReportService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "月工作总结", Enabled = true, Title = "月工作总结", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.MonthReportService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "工器具到期", Enabled = true, Title = "工器具到期", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "领料员,材料员,班长", Assembly = "Bst.Fx.Message.ToolsService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "技术问答答题", Enabled = true, Title = "技术问答答题", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.EducationService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "技术问答评价", Enabled = true, Title = "技术问答评价", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.EducationEvaluateService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "查看技术问答评价", Enabled = true, Title = "查看技术问答评价", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "安全员,技术员", Assembly = "Bst.Fx.Message.EducationAppraiseService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "工器具借用消息", Enabled = true, Title = "工器具借用", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "领料员,材料员", Assembly = "Bst.Fx.Message.ToolBorrowService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "工器具归还消息", Enabled = true, Title = "工器具异常", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "领料员,材料员", Assembly = "Bst.Fx.Message.ToolBackService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "技术问答预警", Enabled = true, Title = "技术问答预警", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "接收人", Assembly = "Bst.Fx.Message.EducationService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "工器具检验到期预警", Enabled = true, Title = "工器具检验到期预警", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "领料员,材料员,班长", Assembly = "Bst.Fx.Message.ToolCheckService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "药品库存预警", Enabled = true, Title = "药品库存预警", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "班长,技术员", Assembly = "Bst.Fx.Message.DrugService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "玻璃器皿库存预警", Enabled = true, Title = "玻璃器皿库存预警", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "班长,技术员", Assembly = "Bst.Fx.Message.GlassService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "化验仪器检验预警", Enabled = true, Title = "化验仪器检验预警", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "班长,技术员", Assembly = "Bst.Fx.Message.InstrumentCheckService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "化验仪器检验到期", Enabled = true, Title = "化验仪器检验到期", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "班长,技术员", Assembly = "Bst.Fx.Message.InstrumentCheckService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "7S定点照片到期提醒", Enabled = true, Title = "7S定点照片到期提醒", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "班长", Assembly = "Bst.Fx.Message.SevenStimeService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "7S照片评价", Enabled = true, Title = "7S照片评价", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "班长", Assembly = "Bst.Fx.Message.SevenSPictureService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "转岗确认", Enabled = true, Title = "转岗确认", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "班长", Assembly = "Bst.Fx.Message.UserWorkAllocationService, Bst.Fx.Message" });

            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "工器具到期", Assembly = "Bst.Fx.Warning.ToolsWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "技术问答预警", Assembly = "Bst.Fx.Warning.EducationWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "拷问讲解预警", Assembly = "Bst.Fx.Warning.KwjjWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "工器具检验到期预警", Assembly = "Bst.Fx.Warning.ToolCheckWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "药品库存预警", Assembly = "Bst.Fx.Warning.DrugCurNumberWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "玻璃器皿库存预警", Assembly = "Bst.Fx.Warning.GlassCurNumberWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "化验仪器检验预警", Assembly = "Bst.Fx.Warning.InstrumentCheckWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "化验仪器检验到期", Assembly = "Bst.Fx.Warning.InstrumentWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "7S定点照片到期提醒", Assembly = "Bst.Fx.Warning.SevenSPictureWarning, Bst.Fx.Warning" });

            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "人员因素风险" });
            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "防护用品风险" });
            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "工器具风险" });
            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "设备设施风险" });
            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "现场环境风险" });
            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "作业过程风险" });
        }
    }
}
