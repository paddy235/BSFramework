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
using BSFramework.Entity.EvaluateAbout;


namespace BSFramework.Application.Service.EvaluateAbout
{
    /// <summary>
    /// 班组权重管理
    /// </summary>
    public class WeightSetService : RepositoryFactory<WeightSetEntity>, IWeightSetService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<WeightSetEntity> GetList(string queryJson)
        {
            return this.BaseRepository().IQueryable().ToList();
        }

        //未使用 
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            //InsertClass();//数据同步
            //var queryParam = queryJson.ToJObject();
            //if (!queryParam["keyword"].IsEmpty())
            //{
            //    string ClassName = queryParam["keyword"].ToString().Trim();
            //    pagination.conditionJson += string.Format(" and ClassName like '%{0}%'", ClassName);
            //}

            //DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, DbHelper.DbType);
            //return dt;
            return new DataTable();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public List<WeightSetEntity> GetPagesList(Pagination pagination, string queryJson)
        {
            InsertClass();//数据同步
            var db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<WeightSetEntity>();
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                expression = expression.And(x => x.ClassName.Contains(keyword));
            }
            var data = db.IQueryable(expression).OrderByDescending(x => x.CreateDate);
            pagination.records = data.Count();
            return data.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
        }
        /// <summary>
        /// 同步考评标准类别数据
        /// </summary>
        public void InsertClass()
        {
            var db = new RepositoryFactory().BaseRepository();
            IEnumerable<EvaluateCategoryEntity> list = db.IQueryable<EvaluateCategoryEntity>().OrderBy(x => x.CreateTime).ToList().Where(t => t.ParentCategoryId == null);
            foreach (EvaluateCategoryEntity Rows in list)
            {
                WeightSetEntity entity = this.GetEntity(Rows.CategoryId);
                if (entity == null)
                {
                    entity = new WeightSetEntity();
                    entity.Id = Rows.CategoryId;
                    entity.IsFiring = 1;
                    entity.ClassName = Rows.Category;
                    entity.SortCode = 0;
                    entity.Weight = 1;
                    this.SaveForm("", entity);
                }
            }
        }


        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public WeightSetEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        //错误方法 未使用
        public List<WeightSetEntity> GetIdEntityList(string keyValue)
        {
            //if (!string.IsNullOrEmpty(keyValue) && keyValue.Length > 0)
            //{
            //    keyValue.TrimEnd(',');
            //    string sql = " SELECT Id,CreateDate,CreateUserId,CreateUserName,ModifyDate,ModifyUserId,ModifyUserName,DeptCode,`Subject`,`Explain`,DeptId,DeptName,Remark,ActIds,activitytype from wg_safetyday where Id in ('" + keyValue.Replace(",", "','") + "');";
            //    return this.BaseRepository().FindList(sql).ToList();
            //}
            return new List<WeightSetEntity>();
        }


        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            WeightSetEntity entity = this.BaseRepository().FindEntity(keyValue);
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
        public void SaveForm(string keyValue, WeightSetEntity entity)
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
