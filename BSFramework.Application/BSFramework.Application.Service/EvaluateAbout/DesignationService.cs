using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.IService.EvaluateAbout;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Application.Entity.Activity;

namespace BSFramework.Application.Service.EvaluateAbout
{
    /// <summary>
    /// 班组称号管理
    /// </summary>
    public class DesignationService : RepositoryFactory<DesignationEntity>, IDesignationService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<DesignationEntity> GetList(string queryJson)
        {
            return this.BaseRepository().IQueryable().ToList();
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            var db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<DesignationEntity>();
            var queryParam = queryJson.ToJObject();

            if (!queryParam["keyword"].IsEmpty())
            {
                string ClassName = queryParam["keyword"].ToString().Trim();
                // pagination.conditionJson += string.Format(" and ClassName like '%{0}%'", ClassName);
                expression = expression.And(x => x.ClassName.Contains(ClassName));
            }
            var data = db.IQueryable(expression);
            pagination.records = data.Count();
            var datalist = data.OrderBy(x=>x.SortCode).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            var queryTalbe = DataHelper.ConvertToTable(datalist);
            return queryTalbe;
            //DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, DbHelper.DbType);
            //return dt;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DesignationEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        //方法错误  也未使用
        public List<DesignationEntity> GetIdEntityList(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();
            if (!string.IsNullOrEmpty(keyValue) && keyValue.Length > 0)
            {
                keyValue.TrimEnd(',');
                var query = db.FindList<SafetydayEntity>(x => keyValue.Contains(x.Id));

                //string sql = " SELECT Id,CreateDate,CreateUserId,CreateUserName,ModifyDate,ModifyUserId,ModifyUserName,DeptCode,`Subject`,`Explain`,DeptId,DeptName,Remark,ActIds,activitytype from wg_safetyday where Id in ('" + keyValue.Replace(",", "','") + "');";
                //return this.BaseRepository().FindList(sql).ToList();
                //var data = query.Select(x => new DesignationEntity() { Id=x.Id,CreateDate=x.CreateDate,CreateUserId=x.CreateUserId,
                //    CreateUserName=x.CreateUserName,ModifyDate=x.ModifyDate,ModifyUserId=x.ModifyUserId,ModifyUserName=x.ModifyUserName,
                //    DeptCode=x.DeptCode,/*`Subject`,`Explain`*/
                //});
            }
            return new List<DesignationEntity>();
        }


        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            DesignationEntity entity = this.BaseRepository().FindEntity(keyValue);
            try
            {
                if (entity != null)
                {
                    this.BaseRepository().Delete(keyValue);
                }
            }
            catch (Exception er)
            {
                throw;
            }

        }

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <param name="path">目录文件</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DesignationEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (string.IsNullOrEmpty(keyValue))
                {
                    entity.Create();
                    db.Insert(entity);
                }
                else
                {
                    entity.Modify(keyValue);
                    db.Update(entity);
                }
                db.Commit();

            }
            catch (Exception er)
            {
                db.Rollback();
                throw;
            }
        }



        #endregion
    }
}
