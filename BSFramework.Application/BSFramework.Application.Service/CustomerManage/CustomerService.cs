using BSFramework.Application.Entity.CustomerManage;
using BSFramework.Application.IService.CustomerManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using BSFramework.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Service.CustomerManage
{
    /// <summary>
    /// 描 述：客户信息
    /// </summary>
    public class CustomerService : RepositoryFactory<CustomerEntity>, ICustomerService
    {
        private ICodeRuleService coderuleService = new CodeRuleService();

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CustomerEntity> GetList()
        {
            return this.BaseRepository().IQueryable().OrderByDescending(t => t.CreateDate).ToList();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表</returns>
        public IEnumerable<CustomerEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var expression = LinqExtensions.True<CustomerEntity>();
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
                string condition = queryParam["condition"].ToString();
                string keyword = queryParam["keyword"].ToString();
                switch (condition)
                {
                    case "EnCode":              //客户编号
                        expression = expression.And(t => t.EnCode.Contains(keyword));
                        break;
                    case "FullName":            //客户名称
                        expression = expression.And(t => t.FullName.Contains(keyword));
                        break;
                    case "Contact":             //联系人
                        expression = expression.And(t => t.Contact.Contains(keyword));
                        break;
                    case "TraceUserName":       //跟进人员
                        expression = expression.And(t => t.TraceUserName.Contains(keyword));
                        break;
                    default:
                        break;
                }
            }
            var query = BaseRepository().IQueryable(expression);
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.CreateDate), out count);
            pagination.records = count;
            return data;
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public CustomerEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 客户名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            var expression = LinqExtensions.True<CustomerEntity>();
            expression = expression.And(t => t.FullName == fullName);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.CustomerId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete<CustomerEntity>(keyValue);
                db.Delete<TrailRecordEntity>(t => t.ObjectId.Equals(keyValue));
                db.Delete<CustomerContactEntity>(t => t.CustomerId.Equals(keyValue));
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, CustomerEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.Modify(keyValue);
                this.BaseRepository().Update(entity);
            }
            else
            {
                IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
                try
                {
                    entity.Create();
                    //获得指定模块或者编号的单据号
                    //entity.EnCode = coderuleService.SetBillCode(entity.CreateUserId, moduleid, "", db);
                    db.Insert(entity);
                    db.Commit();
                }
                catch (Exception)
                {
                    db.Rollback();
                    throw;
                }
            }
        }
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <param name="moduleId">模块</param>
        public void SaveForm(string keyValue, CustomerEntity entity, string moduleId)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.Modify(keyValue);
                this.BaseRepository().Update(entity);
            }
            else
            {
                IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
                try
                {
                    entity.Create();
                    //获得指定模块或者编号的单据号
                    entity.EnCode = coderuleService.SetBillCode(entity.CreateUserId, moduleId, "", db);
                    db.Insert(entity);
                    db.Commit();
                }
                catch (Exception)
                {
                    db.Rollback();
                    throw;
                }
            }
        }
        #endregion
    }
}
