using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.QuestionManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.QuestionManage
{
    public interface IQuestionBankService
    {


        #region 查询
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
       //DataTable GetPageList(Pagination pagination, string queryJson);

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <returns></returns>
        List<QuestionBankEntity> GetPageList(string keyvalue, int pagesize, int page, out int total);

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <returns></returns>
        List<HistoryQuestionEntity> GetHostoryPageList(string keyvalue, int pagesize, int page, out int total);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        QuestionBankEntity GetDetailbyId(string keyvalue);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        List<QuestionBankEntity> GetDetailbyOutId(string keyvalue);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        HistoryQuestionEntity GetHistoryDetailbyId(string keyvalue);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        List<HistoryQuestionEntity> GetHistoryDetailbyOutId(string keyvalue);


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        List<HistoryQuestionEntity> GetHistoryDetailbyActivityId(string keyvalue);


        /// 获取用户答题目录
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        List<TheTitleEntity> GetHistoryAnswerTitle(string userid, string type, string starttime, string endtime, string iscomplete, int index, int size);
        /// <summary>
        /// 获取活动答题目录
        /// </summary>
        /// <param name="activityid">活动id</param>
        /// <returns></returns>
        List<TheTitleEntity> GetHistoryAnswerTitlebyActivityId(string activityid, string userid);
        #endregion

        #region 提交
        /// <summary>
        /// 清除fileid
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        void DetailbyFileId(string keyvalue);
        /// <summary>
        ///add  modify
        /// </summary>
        void SaveFormQuestion(QuestionBankEntity bankEntity, List<TheAnswerEntity> answerEntity, string delAnswer, UserEntity operationUser);

        /// <summary>
        ///history add  modify
        /// </summary>
        void SaveFormHistoryQuestion(HistoryQuestionEntity bankEntity, List<HistoryAnswerEntity> answerEntity, string delAnswer, UserEntity operationUser);

        /// <summary>
        /// 保存目录
        /// </summary>
        /// <param name="entity"></param>
        void SaveFormTitle(TheTitleEntity entity);
        /// <summary>
        /// 开始答题
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="bankEntity"></param>
        /// <param name="operationUser"></param>
        void SaveFormHistoryQuestion(List<TheTitleEntity> entity, List<HistoryQuestionEntity> bankEntity, UserEntity operationUser, string del);

        /// <summary>
        /// 答题
        /// </summary>
        /// <param name="titleid">表</param>
        /// <param name="sort">顺序</param>
        /// <param name="answer">答案</param>
        void answerWork(string titleid, int sort, string answer);
        /// <summary>
        /// 删除
        /// </summary>
        void RemoveForm(string keyvalue);
        /// <summary>
        /// 删除
        /// </summary>
        void RemoveFormByOutId(string keyvalue);
        /// <summary>
        ///history 删除
        /// </summary>
        void RemoveFormHistory(string keyvalue);



        /// <summary>
        /// 分数 
        /// </summary>
        /// <param name="keyvalue"></param>
        string TitleScore(string keyvalue);

        #endregion
    }
}
