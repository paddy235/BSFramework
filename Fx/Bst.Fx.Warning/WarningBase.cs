using Bst.Bzzd.DataSource;
using Bst.Fx.IWarning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Fx.Warning
{
    public abstract class WarningBase
    {
        private string _messagekey;
        public WarningBase(string messagekey)
        {
            _messagekey = messagekey;
        }

        public abstract string[] FindWaringData();

        public virtual void SendWarning()
        {
            var data = this.FindWaringData();

            using (var ctx = new DataContext())
            {
                foreach (var item in data)
                {
                    ctx.Warnings.Add(new Bzzd.DataSource.Entities.Warning() { WarningId = Guid.NewGuid(), BusinessId = item, MessageKey = _messagekey });
                }

                ctx.SaveChanges();
            }
        }
    }
}
