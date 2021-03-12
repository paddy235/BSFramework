using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace BSFramework.Application.IService.PublicInfoManage
{
    /// <summary>
    /// 描 述：文件信息
    /// </summary>
    public interface IFileInfoService
    {
        #region 获取数据
        /// <summary>
        /// 所有文件（夹）列表
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        IEnumerable<FileInfoEntity> GetList(string folderId, string userId);
        /// <summary>
        /// 文档列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        IEnumerable<FileInfoEntity> GetDocumentList(string userId);
        /// <summary>
        /// 图片列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        IEnumerable<FileInfoEntity> GetImageList(string userId);
        /// <summary>
        /// 回收站文件（夹）列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        IEnumerable<FileInfoEntity> GetRecycledList(string userId);
        /// <summary>
        /// 我的文件（夹）共享列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        IEnumerable<FileInfoEntity> GetMyShareList(string userId);
        /// <summary>
        /// 他人文件（夹）共享列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        IEnumerable<FileInfoEntity> GetOthersShareList(string userId);
        /// <summary>
        /// 文件信息实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        FileInfoEntity GetEntity(string keyValue);

        /// <summary>
        /// 获取记录相关联的附件数量
        /// </summary>
        /// <param name="recId">记录ID</param>
        /// <returns></returns>
        List<FileInfoEntity> GetFiles(string recId);
        /// <summary>
        /// 获取apk包文件列表
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        IEnumerable<FileInfoEntity> GetApkListByObject(string folderId);

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="recId">关联的业务记录Id</param>
        /// <param name="fileName">附件名称</param>
        /// <returns></returns>
        FileInfoEntity GetEntity(string recId, string fileName);

        /// <summary>
        /// 得到文化墙中的风采剪影、班组荣誉
        /// </summary>
        /// <param name="recId"></param>
        /// <returns></returns>
        IList<FileInfoEntity> GetCultureWallPics(string recId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 还原文件
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RestoreFile(string keyValue);
        /// <summary>
        /// 删除文件信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        List<FileInfoEntity> RemoveForm(string keys, string meetId, UserEntity user);
        /// <summary>
        /// 彻底删除文件信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        void ThoroughRemoveForm(string keyValue);
        /// <summary>
        /// 保存文件信息表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="fileInfoEntity">文件信息实体</param>
        /// <returns></returns>
        void SaveForm(string keyValue, FileInfoEntity fileInfoEntity);

        void SaveFormEmergency(string keyValue, FileInfoEntity fileInfoEntity);
        /// <summary>
        /// 共享文件
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="IsShare">是否共享：1-共享 0取消共享</param>
        void ShareFile(string keyValue, int IsShare);

        void Save(FileInfoEntity entity);
        /// <summary>
        ///删除附件信息及物理文件 
        /// </summary>
        /// <param name="recId"></param>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        int DeleteFile(string recId, string fileName, string filePath);
        string Delete(string id);
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recId"></param>
        /// <returns></returns>
        IList GetFilesByRecId(string recId);
        IList<FileInfoEntity> GetPics(string recId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recId"></param>
        /// <param name="appUrl"></param>
        /// <returns></returns>
        IList GetFilesByRecId(string recId, string appUrl);

        IEnumerable<FileInfoEntity> GetPeoplePhoto(string recid);
        FileInfoEntity GetFilebyDescription(string recid, string Description);
        IList<FileInfoEntity> GetFilesByRecIdNew(string recId);
        void SaveFiles(List<FileInfoEntity> files);
        List<FileInfoEntity> GetFileList(string data);
        List<FileInfoEntity> DeleteByRecId(string id);
        bool Exist(string filename);
        int GetExplainMaxSortCode();
        List<FileInfoEntity> GetFileList(List<string> ids);
        List<FileInfoEntity> GetFileListByRecId(List<string> ids);
        List<FileInfoEntity> GetFileListByIds(string fileIds);
        List<FileInfoEntity> GetFilesByDescription(string description);
        List<FileInfoEntity> GetFileList(Expression<Func<FileInfoEntity, bool>> exp);
        List<FileInfoEntity> List(string recid, string[] description);
        FileInfoEntity Get(string id);
        void Delete(FileInfoEntity entity);
    }
}
