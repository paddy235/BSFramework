using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.SafetyScore;
using BSFramework.Application.Entity.WorkMeeting.ViewModel;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.SafetyScore
{
    public interface ISafetyScoreService
    {
        void SaveForm(string keyValue, SafetyScoreEntity entity);
        IEnumerable<SafetyScoreEntity> GetPagedList(Pagination pagination, string queryJson);
        SafetyScoreEntity GetEntity(string keyValue);
        void Remove(string keyValue);
        object GetUserScorePagedList(Pagination pagination, string queryJson);

        /// <summary>
        /// 个人得分数据前三名
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        object GetUserScorefirstthree(Pagination pagination, string queryJson);
        Dictionary<int, List<KeyValue>> GetUserScoreInfo(string userId);
        void AddScore(string userId, int dataType);
        object GetUserScorePagedList(Pagination pagination, DateTime searchdate, Expression<Func<UserEntity, bool>> userWhere);
    }
}
