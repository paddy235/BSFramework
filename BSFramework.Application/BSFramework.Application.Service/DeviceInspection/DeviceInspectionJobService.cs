using BSFramework.Application.Entity.DeviceInspection;
using BSFramework.Application.IService.DeviceInspection;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.DeviceInspection
{
    /// <summary>
    /// 设备巡回检查Service
    /// </summary>
    public class DeviceInspectionJobService : RepositoryFactory<DeviceInspectionJobEntity>, IDeviceInspectionJobService
    {
        /// <summary>
        /// 设备巡回检查表 检查表名称唯一性检查
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="inspectionName">检查表名称的值</param>
        /// <returns></returns>
        public bool ExistInspectionName(string keyValue, string inspectionName)
        {
            var expression = LinqExtensions.True<DeviceInspectionJobEntity>();
            expression = expression.And(t => t.InspectionName == inspectionName);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.Id != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }

        /// <summary>
        /// 查询所有的检查表
        /// </summary>
        /// <returns></returns>
        public List<DeviceInspectionJobEntity> GetAllInspetionList()
        {
            return BaseRepository().IQueryable().ToList();
        }

        /// <summary>
        /// 根据设备巡回检查表的主键查询检查项列表 (非分页，查所有)
        /// </summary>
        /// <param name="deviceId">设备巡回检查表的主键</param>
        /// <returns></returns>
        public List<DeviceInspectionItemJobEntity> GetDeviceInspectionItems(string deviceId)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<DeviceInspectionItemJobEntity>(p => p.DeviceId == deviceId).ToList();
        }

        /// <summary>
        /// 查询设备巡回检查表单个实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public DeviceInspectionJobEntity GetEntity(string keyValue)
        {
            return BaseRepository().FindEntity(keyValue);
        }


        public List<DeviceInspectionJobEntity> GetEntityByMeetOrJob(string MeetId, string JobId)
        {

            var db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<DeviceInspectionJobEntity>();
            if (!string.IsNullOrEmpty(MeetId))
            {
                expression = expression.And(t => t.MeetId == MeetId);
            }
            if (!string.IsNullOrEmpty(JobId))
            {
                expression = expression.And(t => t.JobId == JobId);
            }
            var query = BaseRepository().IQueryable(expression).ToList();
            foreach (var item in query)
            {
                item.DeviceInspectionItem = db.IQueryable<DeviceInspectionItemJobEntity>(x => x.DeviceId == item.Id).ToList();
            }
            return query;

        }
        /// <summary>
        /// 分页查询设备巡回检查表
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<DeviceInspectionJobEntity> GetPageList(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<DeviceInspectionJobEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyWord = queryParam["keyword"].ToString();
                expression = expression.And(t => t.InspectionName.Contains(keyWord) || t.DeviceSystem.Contains(keyWord));
            }
            if (!queryParam["code"].IsEmpty())
            {
                string code = queryParam["code"].ToString();
                expression = expression.And(x => x.DeptCode.StartsWith(code));
            }
            var query = BaseRepository().IQueryable(expression);
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.CreateDate), out count);
            pagination.records = count;
            return data;
        }

        /// <summary>
        /// 分页查询设备巡回检查表的数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页个数</param>
        /// <param name="departmentCode">部门编码</param>
        /// <param name="keyword">关键词</param>
        /// <param name="totalCount">总数量</param>
        /// <returns></returns>
        public List<DeviceInspectionJobEntity> GetPageList(int pageIndex, int pageSize, string departmentCode, string keyword, string state, string userid, ref int totalCount)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<DeviceInspectionJobEntity>();
            if (!keyword.IsEmpty())
            {
                expression = expression.And(t => t.InspectionName.Contains(keyword) || t.DeviceSystem.Contains(keyword));
            }
            if (!state.IsEmpty())
            {
                switch (state)
                {
                    case "1":
                        expression = expression.And(t => !t.State);
                        break;
                    case "2":
                        expression = expression.And(t => t.State);
                        break;
                    default:
                        break;
                }
            }
            if (!userid.IsEmpty())
            {
                expression = expression.And(x => x.WorkuserId.Contains(userid));
            }
            if (!departmentCode.IsEmpty())
            {
                expression = expression.And(x => x.DeptCode.StartsWith(departmentCode));
            }
            totalCount = db.IQueryable(expression).Count();
            return db.IQueryable(expression).OrderByDescending(p => p.CreateDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }
        /// <summary>
        /// 新增检查表与检查项
        /// </summary>
        /// <param name="inspectionEntities">检查表数据</param>
        /// <param name="itemEntities">检查项数据</param>
        public void Import(List<DeviceInspectionJobEntity> inspectionEntities, List<DeviceInspectionItemJobEntity> itemEntities)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(inspectionEntities);
                db.Insert(itemEntities);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        ///修改代办状态
        /// </summary>
        /// <param name="id"></param>
        public void SaveInspectionState(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<DeviceInspectionJobEntity>(id);
                if (entity != null)
                {
                    entity.State = true;
                    db.Update(entity);
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }

        /// <summary>
        /// 删除设备巡回检查表 包括底下的巡回检查项
        /// </summary>
        /// <param name="keyValue">设备巡回检查表主键</param>
        public void RemoveInspection(string keyValue)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete<DeviceInspectionJobEntity>(keyValue);
                db.Delete<DeviceInspectionItemJobEntity>(p => p.DeviceId == keyValue);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 保存设备巡回检查信息
        /// </summary>
        /// <param name="keyValue">设备巡回检查表主键</param>
        /// <param name="entity">设备巡回检查实体</param>
        /// <param name="items">检查项Json格式数据</param>
        /// <returns></returns>

        public void SaveForm(string keyValue, DeviceInspectionJobEntity entity, List<DeviceInspectionItemJobEntity> items)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (string.IsNullOrWhiteSpace(keyValue))
                {
                    //新增
                    entity.Create();
                    items.ForEach(p => { p.Id = Guid.NewGuid().ToString(); p.DeviceId = entity.Id; });
                    db.Insert(entity);
                    db.Insert(items);
                }
                else
                {
                    //修改
                    entity.Id = keyValue;
                    entity.Modify();
                    db.Update(entity);
                    if (items != null)
                    {
                        items.ForEach(p => { p.Id = Guid.NewGuid().ToString(); p.DeviceId = entity.Id; });
                        db.Delete<DeviceInspectionItemJobEntity>(p => p.DeviceId.Equals(keyValue));
                        db.Insert(items);
                    }

                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
    }
}
