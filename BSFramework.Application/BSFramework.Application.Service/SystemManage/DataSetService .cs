using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using BSFramework.Data;
using System.Data;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Entity.SystemManage;

namespace BSFramework.Application.Service.SystemManage
{
    /// <summary>
    /// 描 述：数据设置
    /// </summary>
    public class DataSetService : RepositoryFactory<DataSetEntity>, IDataSetService
    {
        #region 获取数据

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataSetEntity> GetList(string deptCode)
        {

            if (!string.IsNullOrWhiteSpace(deptCode))
            {
                var expression = LinqExtensions.True<DataSetEntity>();
                expression = expression.And(t => t.DeptCode == deptCode);
                return this.BaseRepository().IQueryable(expression).OrderBy(t => t.SortCode).ToList();
            }
            else
            {
                return this.BaseRepository().IQueryable().OrderBy(x=>x.SortCode).ToList();
            }
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public  List<DataSetEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var query = BaseRepository().IQueryable();
            //var expression = LinqExtensions.True<DataSetEntity>();
            var queryParam = queryJson.ToJObject();
            //查询条件
            if ( !queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                //pagination.conditionJson += string.Format(" and Itemname like '%{0}%'", keyword.Trim());
                query = query.Where(x => x.ItemName.Contains(keyword));
            }
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderBy(x => x.SortCode), out count);
            pagination.records = count;
            return data;
            //DatabaseType dataType = DbHelper.DbType;
            //return this.BaseRepository().FindTableByProcPager(pagination, dataType);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DataSetEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 验证数据
        
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="list">实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DataSetEntity ds)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                ds.Modify(keyValue);
                this.BaseRepository().Update(ds);
            }
            else
            {
                ds.Create();
                this.BaseRepository().Insert(ds);
            }
             
        }
        #endregion
    }
}
