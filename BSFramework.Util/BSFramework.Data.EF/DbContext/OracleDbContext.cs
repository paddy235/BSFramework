using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;
using System.Linq;
using System.Web;
using System.IO;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;

namespace BSFramework.Data.EF
{
    /// <summary>
    /// 描 述：数据访问(Oracle) 上下文
    /// </summary>
    public class OracleDbContext : DbContext, IDbContext
    {
        #region 构造函数
        /// <summary>
        /// 初始化一个 使用指定数据连接名称或连接串 的数据访问上下文类 的新实例
        /// </summary>
        /// <param name="connString"></param>
        public OracleDbContext(string connString)
            : base(connString)
        {
            //System.Data.Entity.Database.SetInitializer<OracleDbContext>(null);
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }
        #endregion

        #region 重载
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            string[] arr = System.IO.Directory.GetFiles(ass.CodeBase.Substring(0, ass.CodeBase.LastIndexOf('/')).Replace("file:///", ""), "*Mapping*dll");
            foreach (string assembleFileName in arr)
            {
                Assembly asm = Assembly.LoadFile(assembleFileName);
                var typesToRegister = asm.GetTypes()
               .Where(type => !String.IsNullOrEmpty(type.Namespace))
               .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
                foreach (var type in typesToRegister)
                {
                    dynamic configurationInstance = Activator.CreateInstance(type);
                    modelBuilder.Configurations.Add(configurationInstance);
                }
            }
            // modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //此项设置为必须
            modelBuilder.HasDefaultSchema(System.Configuration.ConfigurationManager.AppSettings["SchemaName"]);
            //modelBuilder.Properties().Where(x => x.PropertyType == typeof(string) && x.GetCustomAttributes(typeof(StringLengthAttribute), false).Length == 0).Configure(x => x.HasMaxLength(2000));
            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
