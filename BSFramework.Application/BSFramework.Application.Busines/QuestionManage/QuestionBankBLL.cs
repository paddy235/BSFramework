using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.QuestionManage;
using BSFramework.Application.IService.QuestionManage;
using BSFramework.Application.Service.QuestionManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.QuestionManage
{
    /// <summary>
    /// 答题业务层
    /// </summary>
    public class QuestionBankBLL
    {
        private IQuestionBankService service = new QuestionBankService();

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        //public DataTable GetPageList(Pagination pagination, string queryJson)
        //{
        //    var dt = service.GetPageList(pagination, queryJson);
        //    return dt;
        //}
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <returns></returns>
        public List<QuestionBankEntity> GetPageList(string keyvalue, int pagesize, int page, out int total)
        {
            return service.GetPageList(keyvalue, pagesize, page, out total);
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <returns></returns>
        public List<HistoryQuestionEntity> GetHostoryPageList(string keyvalue, int pagesize, int page, out int total)
        {
            return service.GetHostoryPageList(keyvalue, pagesize, page, out total);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void RemoveForm(string keyvalue)
        {
            service.RemoveForm(keyvalue);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void RemoveFormByOutId(string keyvalue)
        {
            service.RemoveFormByOutId(keyvalue);
        }
        /// <summary>
        /// 清除fileid
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        public void DetailbyFileId(string keyvalue)
        {
            service.DetailbyFileId(keyvalue);

        }
        /// <summary>
        ///add  modify
        /// </summary>
        public void SaveFormQuestion(QuestionBankEntity bankEntity, List<TheAnswerEntity> answerEntity, string delAnswer, UserEntity operationUser)
        {
            service.SaveFormQuestion(bankEntity, answerEntity, delAnswer, operationUser);
        }

        /// <summary>
        ///add  modify
        /// </summary>
        public void SaveFormHistoryQuestion(HistoryQuestionEntity bankEntity, List<HistoryAnswerEntity> answerEntity, string delAnswer, UserEntity operationUser)
        {
            service.SaveFormHistoryQuestion(bankEntity, answerEntity, delAnswer, operationUser);
        }


        /// <summary>
        /// 保存目录
        /// </summary>
        /// <param name="entity"></param>
        public void SaveFormTitle(TheTitleEntity entity)
        {

            service.SaveFormTitle(entity);
        }

        /// <summary>
        /// 开始答题
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="bankEntity"></param>
        /// <param name="operationUser"></param>
        public void SaveFormHistoryQuestion(List<TheTitleEntity> entity, List<HistoryQuestionEntity> bankEntity, UserEntity operationUser,string delstr)
        {
            service.SaveFormHistoryQuestion(entity, bankEntity, operationUser, delstr);

        }

        /// <summary>
        /// 答题
        /// </summary>
        /// <param name="titleid">表</param>
        /// <param name="sort">顺序</param>
        /// <param name="answer">答案</param>
        public void answerWork(string titleid, int sort, string answer)
        {
            service.answerWork(titleid, sort, answer);
        }
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public QuestionBankEntity GetDetailbyId(string keyvalue)
        {

            return service.GetDetailbyId(keyvalue);
        }
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public List<QuestionBankEntity> GetDetailbyOutId(string keyvalue)
        {

            return service.GetDetailbyOutId(keyvalue);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <param name="titleid">目录id</param>
        /// <returns></returns>
        public List<HistoryQuestionEntity> GetHistoryDetailbyOutId(string keyvalue)
        {
            return service.GetHistoryDetailbyOutId(keyvalue);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        public HistoryQuestionEntity GetHistoryDetailbyId(string keyvalue)
        {
            return service.GetHistoryDetailbyId(keyvalue);

        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue">主键</param>
        /// <returns></returns>
        public List<HistoryQuestionEntity> GetHistoryDetailbyActivityId(string keyvalue)
        {
            return service.GetHistoryDetailbyActivityId(keyvalue);
        }

        /// <summary>
        /// 获取用户答题目录
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        public List<TheTitleEntity> GetHistoryAnswerTitle(string userid,string type,string starttime,string endtime,string iscomplete,int index,int size)
        {
            return service.GetHistoryAnswerTitle(userid, type,starttime,endtime,iscomplete,index,size);
        }
        /// <summary>
        /// 获取活动答题目录
        /// </summary>
        /// <param name="activityid">活动id</param>
        /// <returns></returns>
        public List<TheTitleEntity> GetHistoryAnswerTitlebyActivityId(string activityid,string userid)
        {
            return service.GetHistoryAnswerTitlebyActivityId(activityid, userid);
        }
        /// <summary>
        /// 分数 
        /// </summary>
        /// <param name="keyvalue"></param>
        public string TitleScore(string keyvalue)
        {
            return service.TitleScore(keyvalue);
        }
    }
}
