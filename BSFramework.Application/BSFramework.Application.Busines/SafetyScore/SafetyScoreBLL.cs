using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.SafetyScore;
using BSFramework.Application.Entity.WorkMeeting.ViewModel;
using BSFramework.Application.IService.SafetyScore;
using BSFramework.Application.Service.SafetyScore;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SafetyScore
{
   public class SafetyScoreBLL
    {
        private readonly ISafetyScoreService _scoreService;
        public SafetyScoreBLL()
        {
            _scoreService = new SafetyScoreService();
        }

        public IEnumerable<SafetyScoreEntity> GetPagedList(Pagination pagination, string queryJson)
        {
            return _scoreService.GetPagedList(pagination, queryJson);
        }

        public SafetyScoreEntity GetEntity(string keyValue)
        {
            return _scoreService.GetEntity(keyValue);
        }

        /// <summary>
        /// 根据内置的规则类型 ，添加积分数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="dataType">规则类型
        /// 1 登记一般隐患（未整改）
        /// 2 登记一般隐患（已整改）
        /// 3 登记重大隐患（未整改）
        /// 4 登记违章
        /// 5 早安中铝
        /// 6 班前一题
        public void AddScore(string userId, int dataType)
        {
             _scoreService.AddScore(userId, dataType);
        }

        /// <summary>
        /// 保存 ，当用户Id为多个的时候，会新增多条 。
        /// 多用户时用逗号隔开
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void SaveForm(string keyValue, SafetyScoreEntity entity)
        {
             _scoreService.SaveForm(keyValue, entity);
        }

        public void Remove(string keyValue)
        {
            _scoreService.Remove(keyValue);
        }

        /// <summary>
        /// 个人得分数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public object GetUserScorePagedList(Pagination pagination, string queryJson,bool IsNew)
        {
            return _scoreService.GetUserScorePagedList(pagination, queryJson);
        }


        /// <summary>
        /// 个人得分数据统计 （查全厂全年的数据做统计）
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public object GetUserScorePagedList(Pagination pagination, DateTime searchdate, Expression<Func<UserEntity, bool>> userWhere)
        {
            return _scoreService.GetUserScorePagedList(pagination, searchdate,userWhere);
        }


        /// <summary>
        /// 个人得分数据前三名
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public object GetUserScorefirstthree(Pagination pagination, string queryJson)
        {
            return _scoreService.GetUserScorefirstthree(pagination, queryJson);
        }

        /// <summary>
        /// 用户各年份每月的积分信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Dictionary<int, List<KeyValue>> GetUserScoreInfo(string userId)
        {
            return _scoreService.GetUserScoreInfo(userId);
        }
    }
}
