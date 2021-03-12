using Bst.Fx.Uploading;
using Bst.ServiceContract.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bst.ServiceEngine.MessageQueue
{
    public class QueueService : IQueueService
    {
        public void OfficeToPdf(string sourcePath, string targetPath)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info($"开始执行Office文件转Pdf，sourcePath:{sourcePath}  targetPath:{targetPath}");

            var convertService = new Uploader();
            try
            {
                convertService.ConvertPdf(sourcePath, targetPath);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        public void Upload(string id)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info(id);

            var uploader = new Uploader();
            try
            {
                uploader.UploadVideo(id);
            }
            catch (Exception e)
            {
                //logger.Error("上传程序异常，{0}", e.Message);
                logger.Error(e);
            }
        }
    }
}
