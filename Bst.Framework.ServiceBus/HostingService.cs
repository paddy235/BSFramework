using Bst.ServiceContract.MessageQueue;
using Bst.ServiceEngine.MessageQueue;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Bst.Framework.ServiceBus
{
    public partial class HostingService : ServiceBase
    {
        private ServiceHost _host = null;
        private ServiceHost _host2 = null;
        public HostingService()
        {
            InitializeComponent();
        }

        public void Start()
        {
            this.OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            if (_host != null)
            {
                _host.Close();
                _host = null;
            }

            _host = new ServiceHost(typeof(QueueService));
            _host.Open();

            if (_host2 != null)
            {
                _host2.Close();
                _host2 = null;
            }

            _host2 = new ServiceHost(typeof(MsgService));
            _host2.Open();
        }

        protected override void OnStop()
        {
            if (_host != null)
            {
                _host.Close();
                _host = null;
            }

            if (_host2 != null)
            {
                _host2.Close();
                _host2 = null;
            }
        }
    }
}
