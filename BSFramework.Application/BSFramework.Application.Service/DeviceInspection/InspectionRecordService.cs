using BSFramework.Application.Entity.DeviceInspection;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.DeviceInspection;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.DeviceInspection
{
    /// <summary>
    /// 巡回检查记录
    /// </summary>
    public class InspectionRecordService : RepositoryFactory<InspectionRecordEntity>, IInspectionRecordService
    {
        /// <summary>
        /// 获取单个实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public InspectionRecordEntity GetEntity(string keyValue)
        {
            return BaseRepository().FindEntity(keyValue);
        }

        /// <summary>
        /// 分页查询设备巡回检查表
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<InspectionRecordEntity> GetPageList(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<InspectionRecordEntity>();
            expression = expression.And(p => p.IsSubmit == 1);
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyWord = queryParam["keyword"].ToString();
                expression = expression.And(t => t.InspectionName.Contains(keyWord) || t.DeviceSystem.Contains(keyWord));
            }
            if (!queryParam["code"].IsEmpty())
            {
                string code = queryParam["code"].ToString();
                expression = expression.And(x => x.CreateUserDeptCode.StartsWith(code));
            }
            var query = BaseRepository().IQueryable(expression);
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.CreateDate), out count);
            pagination.records = count;
            return data;
        }

        /// <summary>
        /// 分页查询检查记录
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="keyWord">设备系统关键字</param>
        /// <param name="userId">检查人</param>
        /// <param name="jobId">任务的Id</param>
        /// <param name="totalCount">总条数</param>
        /// <returns></returns>
        public List<InspectionRecordEntity> GetPageList(int pageIndex, int pageSize, string keyWord, string userId, string jobId, string issubmit, string time, string deptcode, int totalCount)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<InspectionRecordEntity>();

            if (!issubmit.IsEmpty())
            {
                if (issubmit == "1")
                {
                    expression = expression.And(p => p.IsSubmit == 0);
                }
                if (issubmit == "2")
                {
                    expression = expression.And(p => p.IsSubmit == 1);
                }
            }
            if (!deptcode.IsEmpty()) expression = expression.And(p => p.CreateUserDeptCode.StartsWith(deptcode));
            if (!time.IsEmpty())
            {
                var start = Convert.ToDateTime(time);
                var end = start.AddDays(1).AddMilliseconds(-1);
                expression = expression.And(p => p.CreateDate >= start && p.CreateDate <= end);
            }
            if (!keyWord.IsEmpty()) expression = expression.And(p => p.DeviceSystem.Contains(keyWord));
            if (!userId.IsEmpty()) expression = expression.And(p => p.WorkuserId.Contains(userId));
            if (!jobId.IsEmpty()) expression = expression.And(p => p.JobId == jobId);

            totalCount = db.IQueryable(expression).Count();
            return db.IQueryable(expression).OrderByDescending(p => p.CreateDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 获取检查记录中各检查项的检查结果
        /// </summary>
        /// <param name="recordId">检查记录的主键Id</param>
        /// <param name="deviceId">设备巡回检查表的Id</param>
        /// <returns></returns>
        public List<DeviceInspectionItemJobEntity> GetRecordItems(string recordId, string deviceId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q1 in db.IQueryable<DeviceInspectionItemJobEntity>()
                        join q2 in db.IQueryable<ItemResultEntity>() on q1.Id equals q2.ItemId
                        where q1.DeviceId == deviceId && q2.RecordId == recordId
                        select new
                        {
                            Id = q1.Id,
                            DeviceId = q1.DeviceId,
                            ItemName = q1.ItemName,
                            Method = q1.Method,
                            Standard = q1.Standard,
                            ResultId = q2.Id,
                            Result = q2.Result
                        };
            return query.ToList().Select(p => new DeviceInspectionItemJobEntity()
            {
                Id = p.Id,
                DeviceId = p.DeviceId,
                ItemName = p.ItemName,
                Method = p.Method,
                Standard = p.Standard,
                ResultId = p.Id,
                Result = p.Result
            }).ToList();
        }


        /// <summary>
        /// 新增设备巡回检查记录  包括检查结果与附件信息
        /// </summary>
        /// <param name="recordEntity">检查记录的实体</param>
        /// <param name="results">各检查项的检查结果</param>
        /// <param name="files">要新增的附件的实体</param>
        /// <param name="delFiles">要删除的附件的集合（FilePath属性未服务器的物理路径，用于System.IO的删除用，请先处理）</param>
        /// <param name="isUpdate">是否是修改，否按照新增处理</param>
        public void SaveRecord(InspectionRecordEntity recordEntity, List<ItemResultEntity> results, List<FileInfoEntity> files, List<FileInfoEntity> delFiles, bool isUpdate)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (isUpdate)
                    //修改
                    db.Update(recordEntity);
                else
                    //新增
                    db.Insert(recordEntity);

                //先删除所有的检查记录项的数据，然后再新增 
                db.Delete<ItemResultEntity>(p => p.RecordId == recordEntity.Id);
                db.Insert(results);
                //先新增所有的附件信息,在删除文件信息
                db.Insert(files);

                db.Commit();

                //删除需要删除的文件
                Task.Run(() =>
                {
                    if (delFiles != null && delFiles.Count > 0)
                    {
                        var newdb = new RepositoryFactory().BaseRepository();
                        var delIds = delFiles.Select(p => p.FileId).ToList();
                        newdb.Delete<FileInfoEntity>(p => delIds.Contains(p.FileId));
                        //删除存在服务器上的图片
                        delFiles.ForEach(p =>
                        {
                            if (System.IO.File.Exists(p.FilePath))
                            {
                                System.IO.File.Delete(p.FilePath);
                            }
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }
        }
    }
}
