using BSFramework.Application.IService.Activity;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.Activity
{
    public class ScreensaverService : RepositoryFactory<ScreensaverEntity>, IScreensaverService
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileId">文件的ID</param>
        /// <param name="filePath">文件的地址</param>
        public void DeleteFile(string fileId, string filePath)
        {
            if (this.BaseRepository().Delete(fileId) > 0)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }
        /// <summary>
        /// 查询单个实体
        /// </summary>
        /// <param name="fileid">主键ID</param>
        /// <returns></returns>
        public ScreensaverEntity GetEntity(string fileid)
        {
            return this.BaseRepository().FindEntity(fileid);
        }

        /// <summary>
        /// 列表查询
        /// </summary>
        /// <param name="bzid">班组ID</param>
        /// <returns></returns>
        public List<ScreensaverEntity> GetList(string bzid)
        {
            var data = this.BaseRepository().IQueryable(p => p.DeptId == bzid).ToList();
            return data;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="fileentity">实体</param>
        public void SaveForm(ScreensaverEntity fileentity)
        {
            this.BaseRepository().Insert(fileentity);
        }
    }
}
