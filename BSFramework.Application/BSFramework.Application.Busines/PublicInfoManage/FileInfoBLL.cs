using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Application.Service.PublicInfoManage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace BSFramework.Application.Busines.PublicInfoManage
{
    /// <summary>
    /// 描 述：文件信息
    /// </summary>
    public class FileInfoBLL
    {
        private IFileInfoService service = new FileInfoService();

        #region 获取数据
        /// <summary>
        /// 所有文件（夹）列表
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetList(string folderId, string userId)
        {
            return service.GetList(folderId, userId);
        }
        /// <summary>
        /// 文档列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetDocumentList(string userId)
        {
            return service.GetDocumentList(userId);
        }
        /// <summary>
        /// 图片列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetImageList(string userId)
        {
            return service.GetImageList(userId);
        }
        /// <summary>
        /// 回收站文件（夹）列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetRecycledList(string userId)
        {
            return service.GetRecycledList(userId);
        }
        /// <summary>
        /// 我的文件（夹）共享列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetMyShareList(string userId)
        {
            return service.GetMyShareList(userId);
        }
        /// <summary>
        /// 他人文件（夹）共享列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetOthersShareList(string userId)
        {
            return service.GetOthersShareList(userId);
        }
        /// <summary>
        /// 文件信息实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public FileInfoEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }

        /// <summary>
        ///获取记录相关联的附件数量
        /// </summary>
        /// <param name="recId">记录ID</param>
        /// <returns></returns>
        public List<FileInfoEntity> GetFiles(string recId)
        {
            return service.GetFiles(recId);
        }

        /// <summary>
        /// 获取apk文件列表
        /// </summary>
        public IEnumerable<FileInfoEntity> GetApkListByObject(string folderId)
        {
            return service.GetApkListByObject(folderId);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 还原文件
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RestoreFile(string keyValue)
        {
            try
            {
                service.RestoreFile(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Delete(string id)
        {
            return service.Delete(id);
        }
        /// <summary>
        /// 删除文件信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<FileInfoEntity> RemoveForm(string keys, string meetId, UserEntity user)
        {
            try
            {
                return service.RemoveForm(keys, meetId, user);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 彻底删除文件信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void ThoroughRemoveForm(string keyValue)
        {
            try
            {
                service.ThoroughRemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        ///删除附件信息及物理文件 
        /// </summary>
        /// <param name="recId"></param>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public int DeleteFile(string recId, string fileName, string filePath)
        {
            return service.DeleteFile(recId, fileName, filePath);
        }
        /// <summary>
        /// 保存文件信息表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="fileInfoEntity">文件信息实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, FileInfoEntity fileInfoEntity)
        {
            try
            {
                service.SaveForm(keyValue, fileInfoEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void SaveFormEmergency(string keyValue, FileInfoEntity fileInfoEntity)
        {
            try
            {
                service.SaveFormEmergency(keyValue, fileInfoEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveForm(FileInfoEntity fileInfoEntity)
        {
            try
            {

                service.Save(fileInfoEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 共享文件
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="IsShare">是否共享：1-共享 0取消共享</param>
        public void ShareFile(string keyValue, int IsShare = 1)
        {
            try
            {
                service.ShareFile(keyValue, IsShare);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        public IList<FileInfoEntity> GetFilesByRecIdNew(string recId)
        {
            return service.GetFilesByRecIdNew(recId);
        }

        public IList<FileInfoEntity> GetPics(string recId)
        {
            return service.GetPics(recId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recId"></param>
        /// <returns></returns>
        public IList GetFilesByRecId(string recId)
        {
            return service.GetFilesByRecId(recId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recid"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public List<FileInfoEntity> List(string recid, string[] description)
        {
            return service.List(recid, description);
        }

        public FileInfoEntity Get(string id)
        {
            return service.Get(id);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="recId">关联的业务记录Id</param>
        /// <param name="fileName">附件名称</param>
        /// <returns></returns>
        public FileInfoEntity GetEntity(string recId, string fileName)
        {
            return service.GetEntity(recId, fileName);
        }

        /// <summary>
        /// 得到文化墙中的风采剪影、班组荣誉
        /// </summary>
        /// <param name="recId"></param>
        /// <returns></returns>
        public IList<FileInfoEntity> GetCultureWallPics(string recId)
        {
            return service.GetCultureWallPics(recId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recId"></param>
        /// <param name="appUrl"></param>
        /// <returns></returns>
        public IList GetFilesByRecId(string recId, string appUrl)
        {
            return service.GetFilesByRecId(recId, appUrl);
        }

        public IEnumerable<FileInfoEntity> GetPeoplePhoto(string recid)
        {
            return service.GetPeoplePhoto(recid);
        }
        public FileInfoEntity GetFilebyDescription(string recid, string Description)
        {
            return service.GetFilebyDescription(recid, Description);
        }

        public List<FileInfoEntity> GetFilesByDescription(string description)
        {
            return service.GetFilesByDescription(description);
        }

        public void SaveFiles(List<FileInfoEntity> files)
        {
            service.SaveFiles(files);
        }

        public List<FileInfoEntity> GetFileList(string data)
        {
            return service.GetFileList(data);
        }
        public List<FileInfoEntity> GetFileList(List<string> ids)
        {
            return service.GetFileList(ids);
        }

        public List<FileInfoEntity> GetFileListByRecId(List<string> ids)
        {
            return service.GetFileListByRecId(ids);
        }

        public void Delete(FileInfoEntity entity)
        {
            service.Delete(entity);
        }

        /// <summary>
        /// 根据文件主键获取文件列表
        /// </summary>
        /// <param name="fileIds">文件的主键  英文逗号隔开</param>
        /// <returns></returns>
        public List<FileInfoEntity> GetFileListByIds(string fileIds)
        {
            return service.GetFileListByIds(fileIds);
        }
        public List<FileInfoEntity> DeleteByRecId(string id)
        {
            return service.DeleteByRecId(id);
        }

        public bool Exist(string filename)
        {
            return service.Exist(filename);
        }
        /// <summary>
        /// 获取操作说明文件的最大排序号，没有则返回0
        /// </summary>
        /// <returns></returns>
        public int GetExplainMaxSortCode()
        {
            return service.GetExplainMaxSortCode();
        }

        public List<FileInfoEntity> GetFileList(Expression<Func<FileInfoEntity, bool>> exp)
        {
            return service.GetFileList(exp);
        }
    }
}
