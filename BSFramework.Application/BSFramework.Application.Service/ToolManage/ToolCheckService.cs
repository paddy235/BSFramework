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
    public class ToolCheckService : RepositoryFactory<ToolCheckEntity>, IToolCheckService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<ToolCheckEntity> GetList()
        {

            var query = this.BaseRepository().IQueryable();
            return query.OrderBy(x => x.CreateDate).ToList();
        }

        public IEnumerable<ToolCheckEntity> GetPageList(string from, string to, string checkstate, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            // query = query.Where(x => x.BZId == deptid);
            if (!string.IsNullOrEmpty(from))
            {
                DateTime f = Convert.ToDateTime(from);
                query = query.Where(x => x.CheckDate >= f);
            }
            if (!string.IsNullOrEmpty(to))
            {
                DateTime t = Convert.ToDateTime(to).AddDays(1);
                query = query.Where(x => x.CheckDate < t);
            }
            if (!string.IsNullOrEmpty(checkstate))
            {
                if (checkstate == "已检验")
                {
                    query = query.Where(x => x.ValiDate > DateTime.Now);
                }
                if (checkstate == "未检验")
                {
                    query = query.Where(x => x.ValiDate < DateTime.Now);
                }
            }
            total = query.Count();
            if (page < 0)
            {
                return query.OrderByDescending(x => x.CreateDate).ToList();
            }
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public ToolCheckEntity GetEntity(string keyValue)
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


        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, ToolCheckEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (entity1 == null)
            {
                // entity.ID = Guid.NewGuid().ToString();
                // entity.BZId = OperatorProvider.Provider.Current().DeptId;
                entity.Files = null;
                entity.CreateDate = DateTime.Now;
                this.BaseRepository().Insert(entity);
                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                entity1.Files = null;
                this.BaseRepository().Update(entity1);
            }
        }
        #endregion
    }
}
