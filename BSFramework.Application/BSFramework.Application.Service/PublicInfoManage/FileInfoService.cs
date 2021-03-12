using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Linq;
using System.Collections;
using BSFramework.Application.Entity.BaseManage;
using System.IO;
using BSFramework.Data.EF;
using System.Linq.Expressions;
using System.Data.Entity;

namespace BSFramework.Application.Service.PublicInfoManage
{
    /// <summary>
    /// 描 述：文件信息
    /// </summary>
    public class FileInfoService : RepositoryFactory<FileInfoEntity>, IFileInfoService
    {
        private System.Data.Entity.DbContext _context;
        private DbSet<FileInfoEntity> fileInfoEntities;

        public FileInfoService()
        {
            _context = (DbFactory.Base() as Data.EF.Database).dbcontext;
            fileInfoEntities = _context.Set<FileInfoEntity>();
        }

        #region 获取数据
        /// <summary>
        /// 所有文件（夹）列表
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetList(string folderId, string userId)
        {
            var folder = "folder";
            var query1 = from q in _context.Set<FileFolderEntity>()
                         where q.DeleteMark == 0
                         select new { FileId = q.FolderId, FolderId = q.ParentId, FileName = q.FolderName, FileSize = string.Empty, FileType = folder, q.CreateUserId, q.ModifyDate, q.IsShare, FilePath = string.Empty };
            var query2 = from q in _context.Set<FileInfoEntity>()
                         where q.DeleteMark == 0
                         select new { q.FileId, q.FolderId, q.FileName, q.FileSize, q.FileType, q.CreateUserId, q.ModifyDate, q.IsShare, q.FilePath };
            var query = query2.Concat(query1);

            if (!string.IsNullOrEmpty(folderId))
                query = query.Where(x => x.FolderId == folderId);
            else query = query.Where(x => x.FolderId == "0");

            var data = query.OrderBy(x => x.ModifyDate).ToList();

            return data.Select(x => new FileInfoEntity { FileId = x.FileId, FolderId = x.FolderId, FileName = x.FileName, FileSize = x.FileSize, FileType = x.FileType, CreateUserId = x.CreateUserId, ModifyDate = x.ModifyDate, IsShare = x.IsShare, FilePath = x.FilePath }).ToList();



            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT  *
            //                FROM    ( SELECT    FolderId AS FileId ,
            //                                    ParentId AS FolderId ,
            //                                    FolderName AS FileName ,
            //                                    '' AS FileSize ,
            //                                    'folder' AS FileType ,
            //                                    CreateUserId,
            //                                    ModifyDate,
            //                                    IsShare,
            //                                    '' as  FilePath
            //                          FROM      Base_FileFolder  where DeleteMark = 0
            //                          UNION
            //                          SELECT    FileId ,
            //                                    FolderId ,
            //                                    FileName ,
            //                                    FileSize ,
            //                                    FileType ,
            //                                    CreateUserId,
            //                                    ModifyDate,
            //                                    IsShare,
            //                                    FilePath
            //                          FROM      Base_FileInfo where DeleteMark = 0
            //                        ) t WHERE ");
            //var parameter = new List<DbParameter>();
            //strSql.Append(" CreateUserId = @userId");
            //parameter.Add(DbParameters.CreateDbParameter("@userId", userId));
            //if (!folderId.IsEmpty())
            //{
            //    strSql.Append(" AND FolderId = @folderId");
            //    parameter.Add(DbParameters.CreateDbParameter("@folderId", folderId));
            //}
            //else
            //{
            //    strSql.Append(" AND FolderId = '0'");
            //}
            //strSql.Append(" ORDER BY ModifyDate ASC");
            //return this.BaseRepository().FindList(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 文档列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetDocumentList(string userId)
        {
            var data = BaseRepository().IQueryable(x => x.DeleteMark == 0 && "'log', 'txt', 'pdf', 'doc', 'docx', 'ppt', 'pptx', 'xls', 'xlsx'".Contains(x.FileType) && x.CreateUserId == userId)
                .OrderByDescending(x => x.ModifyDate)
                .ToList();

            //var strSql = new StringBuilder();
            //var parameter = new List<DbParameter>();
            //strSql.Append(@"SELECT  FileId ,
            //                        FolderId ,
            //                        FileName ,
            //                        FileSize ,
            //                        FileType ,
            //                        CreateUserId ,
            //                        ModifyDate,
            //                        IsShare
            //                FROM    Base_FileInfo
            //                WHERE   DeleteMark = 0
            //                        AND FileType IN ( 'log', 'txt', 'pdf', 'doc', 'docx', 'ppt', 'pptx',
            //                                          'xls', 'xlsx' )
            //                        AND CreateUserId = @userId");

            //parameter.Add(DbParameters.CreateDbParameter("@userId", userId));
            //strSql.Append(" ORDER BY ModifyDate ASC");

            return data;
            //return this.BaseRepository().FindList(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 图片列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetImageList(string userId)
        {
            var data = BaseRepository().IQueryable(x => x.DeleteMark == 0 && "'ico', 'gif', 'jpeg', 'jpg', 'png', 'psd'".Contains(x.FileType) && x.CreateUserId == userId)
             .OrderByDescending(x => x.ModifyDate)
             .ToList();
            return data;
        }
        /// <summary>
        /// 回收站文件（夹）列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetRecycledList(string userId)
        {
            var db = new RepositoryFactory().BaseRepository();
            List<FileInfoEntity> data = new List<FileInfoEntity>();
            var filefolferList = db.IQueryable<FileFolderEntity>(x => x.DeleteMark == 1 && x.CreateUserId == userId)
                .Select(x => new FileInfoEntity()
                {
                    FolderId = x.ParentId,
                    FileName = x.FolderName,
                    FileType = "folder",
                    CreateUserId = x.CreateUserId,
                    ModifyDate = x.ModifyDate
                }).ToList();

            var filelist = db.IQueryable<FileInfoEntity>(x => x.DeleteMark == 1 && x.CreateUserId == userId)
                .Select(x => new FileInfoEntity()
                {
                    FileId = x.FileId,
                    FolderId = x.FolderId,
                    FileName = x.FileName,
                    FileSize = x.FileSize,
                    FileType = x.FileType,
                    CreateUserId = x.CreateUserId,
                    ModifyDate = x.ModifyDate
                });
            data.AddRange(filefolferList);
            data.AddRange(filelist);
            return data.OrderByDescending(x => x.ModifyDate);
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT  *
            //                FROM    ( SELECT    FolderId AS FileId ,
            //                                    ParentId AS FolderId ,
            //                                    FolderName AS FileName ,
            //                                    '' AS FileSize ,
            //                                    'folder' AS FileType ,
            //                                    CreateUserId,
            //                                    ModifyDate
            //                          FROM      Base_FileFolder  where DeleteMark = 1
            //                          UNION
            //                          SELECT    FileId ,
            //                                    FolderId ,
            //                                    FileName ,
            //                                    FileSize ,
            //                                    FileType ,
            //                                    CreateUserId,
            //                                    ModifyDate
            //                          FROM      Base_FileInfo where DeleteMark = 1
            //                        ) t WHERE CreateUserId = @userId");
            //var parameter = new List<DbParameter>();
            //parameter.Add(DbParameters.CreateDbParameter("@userId", userId));
            //strSql.Append(" ORDER BY ModifyDate DESC");
            //return this.BaseRepository().FindList(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 我的文件（夹）共享列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetMyShareList(string userId)
        {
            var db = new RepositoryFactory().BaseRepository();
            List<FileInfoEntity> data = new List<FileInfoEntity>();
            var filefolferList = db.IQueryable<FileFolderEntity>(x => x.DeleteMark == 1 && x.CreateUserId == userId && x.IsShare == 1)
                .Select(x => new FileInfoEntity()
                {
                    FolderId = x.ParentId,
                    FileName = x.FolderName,
                    FileType = "folder",
                    CreateUserId = x.CreateUserId,
                    ModifyDate = x.ModifyDate
                }).ToList();

            var filelist = db.IQueryable<FileInfoEntity>(x => x.DeleteMark == 1 && x.CreateUserId == userId && x.IsShare == 1)
                .Select(x => new FileInfoEntity()
                {
                    FileId = x.FileId,
                    FolderId = x.FolderId,
                    FileName = x.FileName,
                    FileSize = x.FileSize,
                    FileType = x.FileType,
                    CreateUserId = x.CreateUserId,
                    ModifyDate = x.ModifyDate
                });
            data.AddRange(filefolferList);
            data.AddRange(filelist);
            return data.OrderByDescending(x => x.ModifyDate);
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT  *
            //                FROM    ( SELECT    FolderId AS FileId ,
            //                                    ParentId AS FolderId ,
            //                                    FolderName AS FileName ,
            //                                    '' AS FileSize ,
            //                                    'folder' AS FileType ,
            //                                    CreateUserId,
            //                                    ModifyDate
            //                          FROM      Base_FileFolder  WHERE DeleteMark = 0 AND IsShare = 1
            //                          UNION
            //                          SELECT    FileId ,
            //                                    FolderId ,
            //                                    FileName ,
            //                                    FileSize ,
            //                                    FileType ,
            //                                    CreateUserId,
            //                                    ModifyDate
            //                          FROM      Base_FileInfo WHERE DeleteMark = 0 AND IsShare = 1
            //                        ) t WHERE CreateUserId = @userId");
            //var parameter = new List<DbParameter>();
            //parameter.Add(DbParameters.CreateDbParameter("@userId", userId));
            //strSql.Append(" ORDER BY ModifyDate DESC");
            //return this.BaseRepository().FindList(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 他人文件（夹）共享列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetOthersShareList(string userId)
        {

            var db = new RepositoryFactory().BaseRepository();
            List<FileInfoEntity> data = new List<FileInfoEntity>();
            var filefolferList = db.IQueryable<FileFolderEntity>(x => x.DeleteMark == 1 && x.CreateUserId != userId && x.IsShare == 1)
                .Select(x => new FileInfoEntity()
                {
                    FolderId = x.ParentId,
                    FileName = x.FolderName,
                    FileType = "folder",
                    CreateUserId = x.CreateUserId,
                    ModifyDate = x.ModifyDate
                }).ToList();

            var filelist = db.IQueryable<FileInfoEntity>(x => x.DeleteMark == 1 && x.CreateUserId != userId && x.IsShare == 1)
                .Select(x => new FileInfoEntity()
                {
                    FileId = x.FileId,
                    FolderId = x.FolderId,
                    FileName = x.FileName,
                    FileSize = x.FileSize,
                    FileType = x.FileType,
                    CreateUserId = x.CreateUserId,
                    ModifyDate = x.ModifyDate
                });
            data.AddRange(filefolferList);
            data.AddRange(filelist);
            return data.OrderByDescending(x => x.ModifyDate);
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT  *
            //                FROM    ( SELECT    FolderId AS FileId ,
            //                                    ParentId AS FolderId ,
            //                                    FolderName AS FileName ,
            //                                    '' AS FileSize ,
            //                                    'folder' AS FileType ,
            //                                    CreateUserId,
            //                                    CreateUserName,
            //                                    ShareTime AS ModifyDate
            //                          FROM      Base_FileFolder  WHERE DeleteMark = 0 AND IsShare = 1
            //                          UNION
            //                          SELECT    FileId ,
            //                                    FolderId ,
            //                                    FileName ,
            //                                    FileSize ,
            //                                    FileType ,
            //                                    CreateUserId,
            //                                    CreateUserName,
            //                                    ShareTime AS ModifyDate
            //                          FROM      Base_FileInfo WHERE DeleteMark = 0 AND IsShare = 1
            //                        ) t WHERE CreateUserId != @userId");
            //var parameter = new List<DbParameter>();
            //parameter.Add(DbParameters.CreateDbParameter("@userId", userId));
            //strSql.Append(" ORDER BY ModifyDate DESC");
            //return this.BaseRepository().FindList(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 文件实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public FileInfoEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="recId">关联的业务记录Id</param>
        /// <param name="fileName">附件名称</param>
        /// <returns></returns>
        public FileInfoEntity GetEntity(string recId, string fileName)
        {
            var ex = LinqExtensions.True<FileInfoEntity>();
            ex = ex.And(t => t.RecId == recId).And(t => t.FileName == fileName);
            var entity = this.BaseRepository().FindEntity(ex);
            return entity;

        }

        /// <summary>
        /// 获取记录相关联的附件数量
        /// </summary>
        /// <param name="recId">记录ID</param>
        /// <returns></returns>
        public List<FileInfoEntity> GetFiles(string recId)
        {
            var query = BaseRepository().IQueryable(x => x.DeleteMark == 0 && x.FolderId == recId);
            return query.ToList();
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT FileId,FileName  FROM  Base_FileInfo where DeleteMark = 0 ");
            //var parameter = new List<DbParameter>();
            //strSql.Append(" AND FolderId = @folderId");
            //parameter.Add(DbParameters.CreateDbParameter("@folderId", recId));
            //return this.BaseRepository().FindTable(strSql.ToString(), parameter.ToArray());
        }

        /// <summary>
        /// 通过folderId 获取对应的apk文件
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public IEnumerable<FileInfoEntity> GetApkListByObject(string folderId)
        {

            var query = BaseRepository().IQueryable(x => x.DeleteMark == 0 && x.FileType == "apk" && x.FolderId == folderId).OrderBy(x => x.CreateDate);
            var data = query.ToList();
            return data;
            //var strSql = new StringBuilder();
            //strSql.AppendFormat(@"SELECT  FileId ,
            //                        FolderId ,
            //                        FileName ,
            //                        FileSize ,
            //                        FileType ,
            //                        CreateUserId ,
            //                        ModifyDate,
            //                        IsShare,FilePath
            //                FROM    Base_FileInfo
            //                WHERE   DeleteMark = 0
            //                        AND FileType='apk'
            //                        AND folderId = '{0}'  ORDER BY ModifyDate ASC", folderId);
            //return this.BaseRepository().FindList(strSql.ToString());
        }
        public IEnumerable<FileInfoEntity> GetBgImage(string recid)
        {
            var query = BaseRepository().IQueryable(x => x.RecId == recid).OrderByDescending(x => x.CreateDate).ToList();
            return query.ToList();
            //var strSql = new StringBuilder();
            //strSql.AppendFormat(@"SELECT  *
            //                FROM    Base_FileInfo
            //                WHERE   RecId = '{0}'  ORDER BY CreateDate ASC", recid);
            //return this.BaseRepository().FindList(strSql.ToString());
        }
        /// <summary>
        /// 获取记录相关附件信息
        /// </summary>
        /// <param name="recId">业务记录Id</param>
        /// <returns></returns>
        public IList GetFilesByRecId(string recId)
        {
            var query = BaseRepository().IQueryable(x => x.DeleteMark == 0 && (x.FolderId == recId || x.RecId == recId));
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT FileId,FileName,FilePath,FileExtensions,Description FROM Base_FileInfo where DeleteMark = 0 ");
            //var parameter = new List<DbParameter>();
            //strSql.Append(" AND (FolderId = @folderId or RecId=@RecId)");
            //parameter.Add(DbParameters.CreateDbParameter("@folderId", recId));
            //parameter.Add(DbParameters.CreateDbParameter("@RecId", recId));
            var data = query.Select(t => new
            {
                t.FileId,
                t.FileName,
                t.FilePath,
                t.FileExtensions,
                t.Description
            }).ToList();
            return data.Select(x => new
            {
                FileName = !string.IsNullOrWhiteSpace(x.FileName) ? System.IO.Path.GetFileNameWithoutExtension(x.FileName) : x.FileName,
                x.FileId,
                x.FilePath,
                x.FileExtensions,
                x.Description
            }).ToList();
        }
        public IList<FileInfoEntity> GetPics(string recId)
        {
            var query = BaseRepository().IQueryable(x => x.DeleteMark == 0 && (x.FolderId == recId || x.RecId == recId) && (x.FileExtensions == ".gif" || x.FileExtensions == ".jpg" || x.FileExtensions == ".png"));
            return query.OrderByDescending(x => x.CreateDate).ToList();
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT FileId,FileName,FilePath,FileExtensions FROM Base_FileInfo where DeleteMark = 0 ");
            ////var parameter = new List<DbParameter>();
            //strSql.Append(" AND  RecId= '" + recId + "' and (FileExtensions = '.gif' or FileExtensions = '.jpg' or FileExtensions = '.png')");
            ////parameter.Add(DbParameters.CreateDbParameter("@folderId", recId));
            ////parameter.Add(DbParameters.CreateDbParameter("@RecId", recId));
            //return this.BaseRepository().FindList(strSql.ToString()).ToList();
        }
        public IList<FileInfoEntity> GetFilesByRecIdNew(string recId)
        {
            var query = BaseRepository().IQueryable();
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT * FROM Base_FileInfo where 1 = 1 ");
            //var parameter = new List<DbParameter>();
            if (!string.IsNullOrEmpty(recId))
            {
                query = query.Where(x => x.RecId == recId || x.FolderId == recId);
                //strSql.Append(" AND  RecId=@RecId");
                //parameter.Add(DbParameters.CreateDbParameter("@RecId", recId));
            }
            //strSql.Append(" ORDER BY CreateDate DESC");
            //return this.BaseRepository().FindList(strSql.ToString(), parameter.ToArray()).ToList();
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        /// <summary>
        /// 获取记录相关附件信息
        /// </summary>
        /// <param name="recId">业务记录Id</param>
        /// <param name="appUrl">附件所在web应用根目录地址（如：http://10.36.0.170/bzWeb,在配置文件中配置）</param>
        /// <returns></returns>
        public IList GetFilesByRecId(string recId, string appUrl)
        {
            var query = BaseRepository().IQueryable(x => x.DeleteMark == 0 && (x.FolderId == recId || x.RecId == recId));
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT  FileId,FileName,FilePath,FileExtensions FROM Base_FileInfo where DeleteMark = 0 ");
            //var parameter = new List<DbParameter>();
            //strSql.Append(" AND (FolderId = @folderId or RecId=@RecId)");
            //parameter.Add(DbParameters.CreateDbParameter("@folderId", recId));
            //parameter.Add(DbParameters.CreateDbParameter("@RecId", recId));
            return query.OrderByDescending(x => x.CreateDate).Select(t => new
            {
                t.FileId,
                t.FileName,
                t.FilePath,
                t.FileExtensions
            }).ToList().Select(t => new
            {
                t.FileId,
                FileName = System.IO.Path.GetFileNameWithoutExtension(t.FileName),
                FilePath = appUrl + t.FilePath.TrimStart('~'),
                t.FileExtensions

            }).ToList();
        }

        public IEnumerable<FileInfoEntity> GetPeoplePhoto(string recid)
        {
            var query = BaseRepository().IQueryable(x => x.RecId == recid && x.Description == "人员").OrderByDescending(x => x.CreateDate).ToList();
            return query;
            //var strSql = new StringBuilder();
            //strSql.AppendFormat(@"SELECT  FileId FROM Base_FileInfo WHERE recid='{0}' AND description = '人员'  ORDER BY CreateDate Desc", recid);
            //return this.BaseRepository().FindList(strSql.ToString());
        }

        public FileInfoEntity GetFilebyDescription(string recid, string Description)
        {
            var ex = LinqExtensions.True<FileInfoEntity>();
            ex = ex.And(t => t.RecId == recid).And(t => t.Description == Description);
            var entity = this.BaseRepository().FindEntity(ex);
            return entity;

        }
        /// <summary>
        /// 得到文化墙中的风采剪影、班组荣誉
        /// </summary>
        /// <param name="recId"></param>
        /// <returns></returns>
        public IList<FileInfoEntity> GetCultureWallPics(string recId)
        {
            var query = BaseRepository().IQueryable(x => x.RecId == recId).OrderByDescending(x => x.ModifyDate).ToList();
            return query;
            //var strSql = new StringBuilder();
            //strSql.Append(@"SELECT fileid,description,filepath,filetype,createdate,modifydate,sortcode FROM Base_FileInfo where ");
            ////var parameter = new List<DbParameter>();
            //strSql.Append(" RecId= '" + recId + "'");
            //strSql.Append(" order by modifydate desc ");
            ////parameter.Add(DbParameters.CreateDbParameter("@folderId", recId));
            ////parameter.Add(DbParameters.CreateDbParameter("@RecId", recId));
            //return this.BaseRepository().FindList(strSql.ToString()).ToList();
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 还原文件
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RestoreFile(string keyValue)
        {
            FileInfoEntity fileInfoEntity = new FileInfoEntity();
            fileInfoEntity.Modify(keyValue);
            fileInfoEntity.DeleteMark = 0;
            this.BaseRepository().Update(fileInfoEntity);
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            FileInfoEntity fileInfoEntity = new FileInfoEntity();
            fileInfoEntity.Modify(keyValue);
            fileInfoEntity.DeleteMark = 1;
            this.BaseRepository().Update(fileInfoEntity);
        }
        public List<FileInfoEntity> RemoveForm(string Keys, string meetId, UserEntity user)
        {
            var query = BaseRepository().IQueryable(x => Keys.Contains(x.FileId) && x.RecId == meetId && x.DeleteMark == 0 && x.Description == "照片").ToList();
            //List<FileInfoEntity> delList = this.BaseRepository().FindList(string.Format("select * from base_fileinfo where fileid not in('{0}') and RECID='{1}' and DELETEMARK=0 and DESCRIPTION='照片'", Keys, meetId)).ToList();
            return query;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="keyValue">主键</param>
        public string Delete(string id)
        {
            var entity = this.GetEntity(id);
            this.BaseRepository().Delete(id);
            if (entity == null)
            {
                return "";
            }
            return entity.FilePath;
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
            var ex = LinqExtensions.True<FileInfoEntity>();
            ex = ex.And(t => t.RecId == recId).And(t => t.FileName == fileName);
            var entity = this.BaseRepository().FindEntity(ex);
            if (entity != null)
            {
                if (this.BaseRepository().Delete(entity) > 0)
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                }
                return 1;
            }
            return 0;
        }
        /// <summary>
        /// 彻底删除文件
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void ThoroughRemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存文件表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="fileInfoEntity">文件信息实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, FileInfoEntity fileInfoEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                fileInfoEntity.Modify(keyValue);
                this.BaseRepository().Update(fileInfoEntity);
            }
            else
            {
                fileInfoEntity.Create();
                this.BaseRepository().Insert(fileInfoEntity);
            }
        }

        public void SaveFormEmergency(string keyValue, FileInfoEntity fileInfoEntity)
        {
            //fileInfoEntity.Create();
            if (!string.IsNullOrEmpty(keyValue))
            {
                this.BaseRepository().Delete(keyValue);
            }
            this.BaseRepository().Insert(fileInfoEntity);
        }


        /// <summary>
        /// 共享文件
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="IsShare">是否共享：1-共享 0取消共享</param>
        public void ShareFile(string keyValue, int IsShare)
        {
            FileInfoEntity fileInfoEntity = new FileInfoEntity();
            fileInfoEntity = this.GetEntity(keyValue);
            fileInfoEntity.FileId = keyValue;
            fileInfoEntity.IsShare = IsShare;
            fileInfoEntity.ShareTime = DateTime.Now;
            this.BaseRepository().Update(fileInfoEntity);
        }

        public void Save(FileInfoEntity entity)
        {
            this.BaseRepository().Insert(entity);
        }
        #endregion


        public void SaveFiles(List<FileInfoEntity> files)
        {
            var fileSet = _context.Set<FileInfoEntity>();
            fileSet.AddRange(files);

            _context.SaveChanges();

            //var db = new RepositoryFactory().BaseRepository().BeginTrans();

            //try
            //{
            //    db.Insert(files);
            //    db.Commit();
            //}
            //catch (Exception e)
            //{
            //    db.Rollback();
            //    throw e;
            //}
        }

        public List<FileInfoEntity> GetFileList(string data)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<FileInfoEntity>()
                        where q.RecId == data
                        orderby q.CreateDate descending
                        select q;

            return query.ToList();
        }

        public List<FileInfoEntity> GetFileList(List<string> ids)
        {
            var db = new RepositoryFactory().BaseRepository();
            if (ids != null && ids.Count > 0)
            {
                return db.FindList<FileInfoEntity>(p => ids.Contains(p.FileId)).ToList();
            }
            return new List<FileInfoEntity>();
        }

        public List<FileInfoEntity> GetFileListByRecId(List<string> ids)
        {
            var db = new RepositoryFactory().BaseRepository();
            if (ids != null && ids.Count > 0)
            {
                return db.FindList<FileInfoEntity>(p => ids.Contains(p.RecId)).ToList();
            }
            return new List<FileInfoEntity>();
        }
        /// <summary>
        /// 根据文件主键获取文件列表
        /// </summary>
        /// <param name="fileIds">文件的主键  英文逗号隔开</param>
        /// <returns></returns>
        public List<FileInfoEntity> GetFileListByIds(string fileIds)
        {
            var db = new RepositoryFactory().BaseRepository();
            if (!string.IsNullOrWhiteSpace(fileIds))
            {
                List<string> keys = fileIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return db.FindList<FileInfoEntity>(p => keys.Contains(p.FileId)).ToList();
            }
            return new List<FileInfoEntity>();
        }

        public List<FileInfoEntity> DeleteByRecId(string id)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<FileInfoEntity>()
                        where q.RecId == id
                        orderby q.CreateDate
                        select q;

            var data = query.ToList();
            db.Delete(data);

            return data;
        }

        public bool Exist(string filename)
        {
            var db = new RepositoryFactory().BaseRepository();

            var query = from q in db.IQueryable<FileInfoEntity>()
                        where q.FileName == filename
                        select q;

            return query.Count() > 0;
        }
        /// <summary>
        /// 获取操作说明文件的最大排序号，没有则返回0
        /// </summary>
        /// <returns></returns>
        public int GetExplainMaxSortCode()
        {
            try
            {
                var maxCode = new RepositoryFactory().BaseRepository().IQueryable<FileInfoEntity>(p => p.Description == "操作说明书" || p.Description == "操作介绍视频").Max(x => x.SortCode);
                if (maxCode.HasValue)
                    return (int)maxCode.Value;
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public List<FileInfoEntity> GetFilesByDescription(string description)
        {
            var query = from q in _context.Set<FileInfoEntity>()
                        where q.Description.Contains(description)
                        select q;

            return query.OrderBy(x => x.CreateDate).ToList();
        }

        public List<FileInfoEntity> GetFileList(Expression<Func<FileInfoEntity, bool>> exp)
        {
            return BaseRepository().IQueryable(exp).ToList();
        }

        public List<FileInfoEntity> List(string recid, string[] description)
        {
            var query = fileInfoEntities.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(recid))
                query = query.Where(x => x.RecId == recid);

            if (description != null && description.Length > 0)
                query = query.Where(x => description.Contains(x.Description));

            return query.OrderBy(x => x.CreateDate).ToList();
        }

        public FileInfoEntity Get(string id)
        {
            return fileInfoEntities.AsNoTracking().AsQueryable().FirstOrDefault(x => x.FileId == id);
        }

        public void Delete(FileInfoEntity entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
            _context.SaveChanges();
        }
    }
}
