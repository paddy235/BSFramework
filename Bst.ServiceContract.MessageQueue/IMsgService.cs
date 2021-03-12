using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Bst.ServiceContract.MessageQueue
{
    [ServiceContract]
    public interface IMsgService
    {
        [OperationContract(IsOneWay = true)]
        void Send(string messagekey, string businessId);
    }
}
