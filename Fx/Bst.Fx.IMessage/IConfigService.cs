using Bst.Fx.MessageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.IMessage
{
    public interface IConfigService
    {
        List<ConfigEntity> GetMessageConfigs(string title);
        void ModifyConfig(ConfigEntity entity);
        ConfigEntity GetConfigDetail(Guid guid);
    }
}
