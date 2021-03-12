using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Service.SystemManage
{
    /// <summary>
    /// 描 述：意见反馈
    /// </summary>
    public class FeedbackService : RepositoryFactory<FeedbackEntity>, IFeedbackService
    {
        #region 获取数据
        /// <summary>
        /// 意见反馈列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<FeedbackEntity> GetPageList(Pagination pagination, string queryJson)
        {
            var query = BaseRepository().IQueryable();
            //var expression = LinqExtensions.True<FeedbackEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["OpinionContent"].IsEmpty())
            {
                string OpinionContent = queryParam["OpinionContent"].ToString();
                //expression = expression.And(t => t.OpinionContent.Contains(OpinionContent));
              query.Where(x => x.OpinionContent.Contains(OpinionContent));
            }
            //if (!queryParam["OrgCode"].IsEmpty())
            //{
            //    string OrgCode = queryParam["OrgCode"].ToString();
            //    expression = expression.And(t => t.CreateUserOrgCode == OrgCode);
            //}
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x=>x.CreateDate), out count);
            pagination.records = count;
            return data;
        }
        /// <summary>
        /// 意见反馈实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public FeedbackEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除意见反馈
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存意见反馈表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="opinionEntity">意见反馈实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, FeedbackEntity opinionEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                opinionEntity.Modify(keyValue);
                this.BaseRepository().Update(opinionEntity); 
            }
            else
            {
                opinionEntity.Create();
                this.BaseRepository().Insert(opinionEntity);
            }
        }
        #endregion
    }
}
