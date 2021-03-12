namespace Bst.Bzzd.DataSource
{
    using Entities;
    using System;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;

    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class DataContext : DbContext
    {
        //您的上下文已配置为从您的应用程序的配置文件(App.config 或 Web.config)
        //使用“DataContext”连接字符串。默认情况下，此连接字符串针对您的 LocalDb 实例上的
        //“Bst.Fx.DataSource.DataContext”数据库。
        // 
        //如果您想要针对其他数据库和/或数据库提供程序，请在应用程序配置文件中修改“DataContext”
        //连接字符串。
        public DataContext()
            : base("name=BaseDb")
        {
            this.Database.Log = x => Debug.WriteLine(x);
            //this.Configuration.AutoDetectChangesEnabled = false;
            //this.Configuration.ValidateOnSaveEnabled = false;
            //this.Configuration.LazyLoadingEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("BZZD");
            base.OnModelCreating(modelBuilder);
        }

        //为您要在模型中包含的每种实体类型都添加 DbSet。有关配置和使用 Code First  模型
        //的详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=390109。

        public virtual DbSet<Scheduler> Schedulers { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<ReportSetting> ReportSettings { get; set; }
        public virtual DbSet<CultureWall> CultureWallInfos { get; set; }
        public virtual DbSet<Budget> Budgets { get; set; }
        public virtual DbSet<CostRecord> CostRecords { get; set; }
        public virtual DbSet<CostItem> CostItems { get; set; }
        public virtual DbSet<MessageConfig> MessageConfigs { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<WarningConfig> WarningConfigs { get; set; }
        public virtual DbSet<Warning> Warnings { get; set; }
        public virtual DbSet<DangerCategory> DangerCategories { get; set; }
        public virtual DbSet<DangerMeasure> DangerMeasures { get; set; }
        public virtual DbSet<HumanDanger> HumanDangers { get; set; }
        public virtual DbSet<HumanDangerTraining> HumanDangerTrainings { get; set; }
        public virtual DbSet<HumanDangerTrainingUser> HumanDangerTrainingUsers { get; set; }
    }
}