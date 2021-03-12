using BSFramework.Entity.WorkMeeting;
using BSFramework.Application.IService.ToolManage;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using BSFramework.Application.Entity.ToolManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using System.Text;
using System.Data;
using BSFramework.Data;
using System;

namespace BSFramework.Application.Service.ToolManage
{
    public class ToolInfoService : RepositoryFactory<ToolInfoEntity>, IToolInfoService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<ToolInfoEntity> GetList(string id)
        {
                var query = this.BaseRepository().IQueryable();
                if (!string.IsNullOrEmpty(id)) query = query.Where(x => x.TypeId == id);
                return query.OrderByDescending(x => x.CreateDate).ToList();
        }

        public IEnumerable<ToolNumberEntity> GetToolNumberList(string id)
        {
            var query = new Repository<ToolNumberEntity>(DbFactory.Base()).IQueryable();
            if (!string.IsNullOrEmpty(id)) query = query.Where(x => x.ToolId == id);
            return query.ToList();
        }
        public ToolNumberEntity GetToolNumberEntity(string keyValue)
        {
            return new Repository<ToolNumberEntity>(DbFactory.Base()).FindEntity(keyValue);
        }
        public void SaveToolNumber(string keyValue, ToolNumberEntity entity)
        {
            var entity1 = this.GetToolNumberEntity(keyValue);
            if (entity1 == null)
            {
                if (string.IsNullOrEmpty(entity.ID))
                {
                    entity.ID = Guid.NewGuid().ToString();
                }
                new Repository<ToolNumberEntity>(DbFactory.Base()).Insert(entity);
            }
            else
            {
                entity1.IsBreak = entity.IsBreak;
                new Repository<ToolNumberEntity>(DbFactory.Base()).Update(entity);
            }
        }
        public IEnumerable<ToolInfoEntity> GetPageList(string name, string tid, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(tid)) query = query.Where(x => x.TypeId == tid);
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Name.Contains(name));
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ToolInfoEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        public void RemoveFormList(List<ToolInfoEntity> list)
        {
            this.BaseRepository().Delete(list);
        }
        public void RemoveToolNumber(string keyValue)
        {
            new Repository<ToolNumberEntity>(DbFactory.Base()).Delete(keyValue);
        }
        public void RemoveToolNumberList(List<ToolNumberEntity> list)
        {
            new Repository<ToolNumberEntity>(DbFactory.Base()).Delete(list);
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, ToolInfoEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (entity1 == null)
            {
                if (string.IsNullOrEmpty(entity.ID))
                {
                    entity.ID = Guid.NewGuid().ToString();
                }
                entity.CreateDate = DateTime.Now;
                this.BaseRepository().Insert(entity);
                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                entity1.Name = entity.Name;
                entity1.HGZ = entity.HGZ;
                entity1.HGZPath = entity.HGZPath;
                entity1.Certificate = entity.Certificate;
                entity1.CerPath = entity.CerPath;
                entity1.CheckReport = entity.CheckReport;
                entity1.CheckPath = entity.CheckPath;
                entity1.OutDate = entity.OutDate;
                entity1.ProFactory = entity.ProFactory;
                entity1.RegDate = entity.RegDate;
                entity1.RegPersonId = entity.RegPersonId;
                entity1.RegPersonName = entity.RegPersonName;
                entity1.Spec = entity.Spec;
                entity1.Total = entity.Total;
                entity1.TypeId = entity.TypeId;
                entity1.ValiDate = entity.ValiDate;
                entity1.CurrentNumber = entity.CurrentNumber;
                entity1.CheckCycle = entity.CheckCycle;
                entity1.Numbers = entity.Numbers;
                entity1.BreakNumbers = entity.BreakNumbers;
                entity1.Remind = entity.Remind;
                entity1.DepositPlace = entity.DepositPlace;
                entity1.ToolcheckDate = entity.ToolcheckDate;
                this.BaseRepository().Update(entity1);
            }
        }

        /// <summary>
        /// 新增单条或多条数据
        /// </summary>
        /// <param name="insertTools">单个实体 或 实体数组</param>
        public void Insert(params ToolInfoEntity[] insertTools)
        {
            if (insertTools !=null && insertTools.Count()>0)
            {
                this.BaseRepository().Insert(insertTools.ToList());
            }
        }
        #endregion
    }
}
