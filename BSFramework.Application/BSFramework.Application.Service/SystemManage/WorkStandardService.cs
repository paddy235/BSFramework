using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.SystemManage
{
    public class WorkStandardService : RepositoryFactory<WorkStandardEntity>, IWorkStandardService
    {
        /// <summary>
        /// 获取所有的工作标准
        /// </summary>
        /// <returns></returns>
        public List<WorkStandardEntity> GetAllList()
        {
            var url = BSFramework.Util.Config.GetValue("AppUrl");
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<WorkStandardEntity>()
                        join q2 in db.IQueryable<FileInfoEntity>() on q1.Id equals q2.RecId into tb1
                        from t1 in tb1.DefaultIfEmpty()
                        select new 
                        {
                            Id = q1.Id,
                            Content = q1.Content,
                            CreateTime = q1.CreateTime,
                            ModuleCode = q1.ModuleCode,
                            ModuleId = q1.ModuleId,
                            ModuleName = q1.ModuleName,
                            CreateDate = t1.CreateDate,
                            FileId = t1.FileId,
                            FileName = t1.FileName,
                            Description = t1.Description,
                            FilePath = t1.FilePath,
                            FileSize = t1.FileSize,
                            FileType = t1.FileType,
                            RecId = t1.RecId,
                        };

            var data = query.ToList();
            var d = data.GroupBy(x => new { x.Id, x.Content, x.CreateTime, x.ModuleId, x.ModuleCode, x.ModuleName }).Select(y => {
                if (y !=null )
                {
                    var file = new WorkStandardEntity()
                    {
                        Id = y.Key.Id,
                        Content = y.Key.Content,
                        CreateTime = y.Key.CreateTime,
                        ModuleName = y.Key.ModuleName,
                        ModuleId = y.Key.ModuleId,
                        FileList = y.Select(x => new FileInfoEntity()
                        {
                            FileId = x.FileId,
                            FileName = x.FileName,
                            Description = x.Description,
                            FilePath =!string.IsNullOrWhiteSpace(x.FilePath) ?  x.FilePath.Replace("~/", url) : null,
                            FileSize = x.FileSize,
                            FileType = x.FileType,
                            RecId = x.RecId,
                        })
                    };
                    return file;
                }
                return null;
            }
           ).ToList();
            return d;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键 </param>
        /// <returns></returns>
        public WorkStandardEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询条件</param>
        /// <returns></returns>
        public List<WorkStandardEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var query = BaseRepository().IQueryable();
            //var expression = LinqExtensions.True<DataSetEntity>();
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                //pagination.conditionJson += string.Format(" and ModuleName like '%{0}%'", keyword.Trim());
                query = query.Where(x => x.ModuleName.Contains(keyword));
            }
            //DatabaseType dataType = DbHelper.DbType;
            //return this.BaseRepository().FindTableByProcPager(pagination, dataType);
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.CreateTime), out count);
            pagination.records = count;
            return data;
        }
        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        public void SaveForm(string keyValue, WorkStandardEntity entity)
        {
            var oldEntity = BaseRepository().FindEntity(keyValue);
            if (oldEntity == null)
            {
                //添加
                entity.CreateTime = DateTime.Now;
                entity.Id = keyValue;
                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.Id = oldEntity.Id;
                this.BaseRepository().Update(entity);
            }
        }
    }
}
