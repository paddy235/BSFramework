using Bst.Fx.IMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bst.Fx.MessageData;
using Bst.Bzzd.DataSource;
using BSFramework.Data.Repository;
using BSFramework.Application.Entity.BaseManage;
using Bst.Bzzd.DataSource.Entities;

namespace Bst.Fx.Message
{
    public class ConfigService : IConfigService
    {
        public ConfigEntity GetConfig(string messagekey)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.MessageConfigs.FirstOrDefault(x => x.ConfigKey == messagekey);
                if (entity == null) return null;

                return new ConfigEntity() { ConfigId = entity.ConfigId, Category = entity.Category.ToString(), ConfigKey = entity.ConfigKey, Enabled = entity.Enabled, Title = entity.Title, Template = entity.Template, RecieveType = entity.RecieveType };
            }
        }

        public ConfigEntity GetConfigDetail(Guid guid)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.MessageConfigs.Find(guid);
                if (entity == null) return null;

                return new ConfigEntity() { ConfigId = entity.ConfigId, Category = entity.Category.ToString(), ConfigKey = entity.ConfigKey, Enabled = entity.Enabled, Title = entity.Title, Template = entity.Template, RecieveType = entity.RecieveType };
            }
        }

        public List<ConfigEntity> GetMessageConfigs(string title)
        {
            var result = new List<ConfigEntity>();
            using (var ctx = new DataContext())
            {
                var query = ctx.MessageConfigs.AsQueryable();
                if (!string.IsNullOrEmpty(title)) query = query.Where(x => x.Title.Contains(title));

                var data = query.ToList();
                foreach (var item in data)
                {
                    result.Add(new ConfigEntity()
                    {
                        ConfigId = item.ConfigId,
                        Category = item.Category.ToString(),
                        ConfigKey = item.ConfigKey,
                        Enabled = item.Enabled,
                        Title = item.Title,
                        Template = item.Template,
                        RecieveType = item.RecieveType
                    });
                }
            }
            return result;
        }

        public void ModifyConfig(ConfigEntity config)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.MessageConfigs.Find(config.ConfigId);

                entity.Title = config.Title;
                entity.Template = config.Template;
                entity.Enabled = config.Enabled;
                entity.Category = (MessageCategory)Enum.Parse(typeof(MessageCategory), config.Category);
                entity.RecieveType = config.RecieveType;

                ctx.SaveChanges();
            }
        }
    }
}