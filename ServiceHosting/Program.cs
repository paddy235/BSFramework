using Bst.ServiceEngine.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHosting
{
    class Program
    {

        static void Main(string[] args)
        {
            var host1 = new ServiceHost(typeof(QueueService));
            host1.Opened += (s, e) => Console.WriteLine("queue service started");
            host1.Open();

            var host2 = new ServiceHost(typeof(MsgService));
            host2.Opened += (s, e) => Console.WriteLine("message service started");
            host2.Open();

            while (Console.ReadLine() != "exit")
            {

            }
        }
    }
}
