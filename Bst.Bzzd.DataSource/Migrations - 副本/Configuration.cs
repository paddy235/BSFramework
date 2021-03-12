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
            context.ReportSettings.AddOrUpdate(x => x.SettingName, new Entities.ReportSetting() { SettingId = Guid.NewGuid(), SettingName = "�ܹ����ܽ�", StartTime = DateTime.Now, EndTime = DateTime.Now, Start = 0, End = 0 });
            context.ReportSettings.AddOrUpdate(x => x.SettingName, new Entities.ReportSetting() { SettingId = Guid.NewGuid(), SettingName = "�¹����ܽ�", StartTime = DateTime.Now, EndTime = DateTime.Now, Start = 1, End = 1 });

            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "������ʾ", Enabled = true, Title = "��������", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.JobService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "���۰�ǰ����", Enabled = true, Title = "��������", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.MeetingService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "Σ��Ԥ֪ѵ��", Enabled = true, Title = "Σ��Ԥ֪ѵ��", Template = "{content}", Category = Entities.MessageCategory.Todo, RecieveType = "������", Assembly = "Bst.Fx.Message.TrainingService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "����Σ��Ԥ֪ѵ��", Enabled = true, Title = "��������", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.TrainingEvaluationService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "������·�", Enabled = true, Title = "�����", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "�೤,��ȫԱ", Assembly = "Bst.Fx.Message.ActivitySourceService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "���۰���", Enabled = true, Title = "��������", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.ActivityService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "��������", Enabled = true, Title = "����һ��¥", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.JobAlertService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "���۹����ܽ�", Enabled = true, Title = "��������", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.ReportEvaluateService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�ܹ����ܽ�", Enabled = true, Title = "�ܹ����ܽ�", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.WeekReportService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�¹����ܽ�", Enabled = true, Title = "�¹����ܽ�", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.MonthReportService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�����ߵ���", Enabled = true, Title = "�����ߵ���", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "����Ա,����Ա,�೤", Assembly = "Bst.Fx.Message.ToolsService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�����ʴ����", Enabled = true, Title = "�����ʴ����", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.EducationService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�����ʴ�����", Enabled = true, Title = "�����ʴ�����", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.EducationEvaluateService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�鿴�����ʴ�����", Enabled = true, Title = "�鿴�����ʴ�����", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "��ȫԱ,����Ա", Assembly = "Bst.Fx.Message.EducationAppraiseService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�����߽�����Ϣ", Enabled = true, Title = "�����߽���", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "����Ա,����Ա", Assembly = "Bst.Fx.Message.ToolBorrowService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�����߹黹��Ϣ", Enabled = true, Title = "�������쳣", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "����Ա,����Ա", Assembly = "Bst.Fx.Message.ToolBackService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�����ʴ�Ԥ��", Enabled = true, Title = "�����ʴ�Ԥ��", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "������", Assembly = "Bst.Fx.Message.EducationService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�����߼��鵽��Ԥ��", Enabled = true, Title = "�����߼��鵽��Ԥ��", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "����Ա,����Ա,�೤", Assembly = "Bst.Fx.Message.ToolCheckService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "ҩƷ���Ԥ��", Enabled = true, Title = "ҩƷ���Ԥ��", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "�೤,����Ա", Assembly = "Bst.Fx.Message.DrugService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "����������Ԥ��", Enabled = true, Title = "����������Ԥ��", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "�೤,����Ա", Assembly = "Bst.Fx.Message.GlassService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "������������Ԥ��", Enabled = true, Title = "������������Ԥ��", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "�೤,����Ա", Assembly = "Bst.Fx.Message.InstrumentCheckService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "�����������鵽��", Enabled = true, Title = "�����������鵽��", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "�೤,����Ա", Assembly = "Bst.Fx.Message.InstrumentCheckService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "7S������Ƭ��������", Enabled = true, Title = "7S������Ƭ��������", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "�೤", Assembly = "Bst.Fx.Message.SevenStimeService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "7S��Ƭ����", Enabled = true, Title = "7S��Ƭ����", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "�೤", Assembly = "Bst.Fx.Message.SevenSPictureService, Bst.Fx.Message" });
            //context.MessageConfigs.AddOrUpdate(x => x.ConfigKey, new Entities.MessageConfig() { ConfigId = Guid.NewGuid(), ConfigKey = "ת��ȷ��", Enabled = true, Title = "ת��ȷ��", Template = "{content}", Category = Entities.MessageCategory.Message, RecieveType = "�೤", Assembly = "Bst.Fx.Message.UserWorkAllocationService, Bst.Fx.Message" });

            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "�����ߵ���", Assembly = "Bst.Fx.Warning.ToolsWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "�����ʴ�Ԥ��", Assembly = "Bst.Fx.Warning.EducationWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "���ʽ���Ԥ��", Assembly = "Bst.Fx.Warning.KwjjWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "�����߼��鵽��Ԥ��", Assembly = "Bst.Fx.Warning.ToolCheckWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "ҩƷ���Ԥ��", Assembly = "Bst.Fx.Warning.DrugCurNumberWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "����������Ԥ��", Assembly = "Bst.Fx.Warning.GlassCurNumberWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "������������Ԥ��", Assembly = "Bst.Fx.Warning.InstrumentCheckWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "�����������鵽��", Assembly = "Bst.Fx.Warning.InstrumentWarning, Bst.Fx.Warning" });
            context.WarningConfigs.AddOrUpdate(x => x.MessageKey, new Entities.WarningConfig() { ConfigId = Guid.NewGuid(), Enabled = true, MessageKey = "7S������Ƭ��������", Assembly = "Bst.Fx.Warning.SevenSPictureWarning, Bst.Fx.Warning" });

            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "��Ա���ط���" });
            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "������Ʒ����" });
            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "�����߷���" });
            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "�豸��ʩ����" });
            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "�ֳ���������" });
            //context.DangerCategories.AddOrUpdate(x => x.CategoryName, new Entities.DangerCategory() { CategoryId = Guid.NewGuid(), CategoryName = "��ҵ���̷���" });
        }
    }
}
