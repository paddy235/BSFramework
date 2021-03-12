using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Cache.Factory;
using Bst.Fx.IMessage;
using Bst.Fx.Message;
using Bst.Fx.MessageData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Busines.SystemManage
{
    /// <summary>
    /// 描 述：消息设置
    /// </summary>
    public class MessageConfigBLL
    {
        public List<ConfigEntity> GetMessageConfigs(string title)
        {
            IConfigService service = new ConfigService();
            return service.GetMessageConfigs(title);
        }

        public ConfigEntity GetConfigDetail(Guid guid)
        {
            IConfigService service = new ConfigService();
            return service.GetConfigDetail(guid);
        }

        public void ModifyConfig(ConfigEntity config)
        {
            IConfigService service = new ConfigService();
            service.ModifyConfig(config);
        }
    }
}
