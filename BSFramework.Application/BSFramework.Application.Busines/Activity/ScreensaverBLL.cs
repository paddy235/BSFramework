using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
   public class ScreensaverBLL
    {
        private IScreensaverService service;
        public ScreensaverBLL()
        {
            service = new ScreensaverService();
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileids">文件ID的集合</param>
        public void DeleteFile(string fileId, string filePath)
        {
            service.DeleteFile(fileId, filePath);
        }

        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <param name="fileid">主键ID</param>
        /// <returns></returns>
        public ScreensaverEntity GetEntity(string fileid)
        {
            return service.GetEntity(fileid);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="fileentity"></param>
        public void SaveForm(ScreensaverEntity fileentity)
        {
            service.SaveForm(fileentity);
        }

        /// <summary>
        /// 列表查询
        /// </summary>
        /// <param name="bzid">班组的Id</param>
        /// <returns></returns>
        public List<ScreensaverEntity> GetList(string bzid)
        {
            return service.GetList(bzid);
        }
    }
}
