using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Bst.ServiceContract.MessageQueue
{
    [ServiceContract]
    public interface IQueueService
    {
        [OperationContract(IsOneWay = true)]
        void Upload(string id);

        [OperationContract(IsOneWay = true)]
        void OfficeToPdf(string sourcePath, string targetPath);
    }
}
