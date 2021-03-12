using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System;
using BSFramework.Data;
using System.Linq;

namespace BSFramework.Application.Service.PublicInfoManage
{
    /// <summary>
    /// 描 述：电子公告
    /// </summary>
    public class NoticeService : RepositoryFactory<NewsEntity>, INoticeService
    {
        #region 获取数据
        /// <summary>
        /// 公告列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<NewsEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var query = BaseRepository().IQueryable(x => x.TypeId == 2);
            //var expression = LinqExtensions.True<NewsEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["FullHead"].IsEmpty())
            {
                string FullHead = queryParam["FullHead"].ToString();
                //expression = expression.And(t => t.FullHead.Contains(FullHead));
                query = query.Where(x => x.FullHead.Contains(FullHead));
            }
            if (!queryParam["Category"].IsEmpty())
            {
                string Category = queryParam["Category"].ToString();
                //expression = expression.And(t => t.Category == Category);
                query = query.Where(x => x.Category == Category);
            }
            //expression = expression.And(t => t.TypeId == 2);
            //return this.BaseRepository().FindList(expression, pagination);
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.CreateDate), out count);
            pagination.records = count;
            return data;
        }
        /// <summary>
        /// 公告实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public NewsEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存公告表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="newsEntity">公告实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, NewsEntity newsEntity)
        {
            var data = this.GetEntity(keyValue);

            if (data != null)
            {
                newsEntity.Modify(keyValue);
                newsEntity.TypeId = 2;
                newsEntity.fileList = null;
                this.BaseRepository().Update(newsEntity);
            }
            else
            {
                newsEntity.NewsId = keyValue;
                newsEntity.Create();
                newsEntity.TypeId = 2;
                newsEntity.fileList = null;
                this.BaseRepository().Insert(newsEntity);
            }
        }

        public IEnumerable<NewsEntity> GetAllNotice(string deptid, DateTime? from, DateTime? to,string name, int page, int pagesize, out int total)
        {
            var query = BaseRepository().IQueryable(x => deptid.Contains(x.DeptId));
            //var query= this.BaseRepository().FindList("select * from base_news where find_in_set(@deptid, deptid) ;", new System.Data.Common.DbParameter[] { DbParameters.CreateDbParameter("@deptid", deptid) });
            if (from != null) query = query.Where(x =>Convert.ToDateTime( x.ReleaseTime )>= from.Value);
            if (to != null)
            {
                to = to.Value.AddDays(1).AddMinutes(-1);
                query = query.Where(x =>Convert.ToDateTime( x.ReleaseTime) <= to);
            }
            if (!string.IsNullOrEmpty(name)) {
                query = query.Where(x => x.FullHead.Contains(name)); 
            }
            total = query.Count();
            var data = query.OrderByDescending(x => x.ReleaseTime.Value).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return data;
        }

        public List<NewsEntity> GetCurrentNotice(string deptId, DateTime date)
        {
            //return this.BaseRepository().FindList("select * from base_news where find_in_set(@deptid, deptid) and releasetime >= @date order by isimport desc, releasetime desc", new System.Data.Common.DbParameter[] { DbParameters.CreateDbParameter("@deptid", deptId), DbParameters.CreateDbParameter("@date", date) }).ToList();
            var data = BaseRepository().IQueryable(x => deptId.Contains(x.DeptId) && x.ReleaseTime >= date).OrderByDescending(x => x.IsImport).ThenByDescending(x => x.ReleaseTime).ToList(); ;
            return data;
        }


        #endregion
    }
}
