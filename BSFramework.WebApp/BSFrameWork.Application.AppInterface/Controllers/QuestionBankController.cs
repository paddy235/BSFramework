using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.InnovationManage;
using BSFramework.Application.Busines.QuestionManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.QuestionManage;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class QuestionBankController : BaseApiController
    {

        private QuestionBankBLL bll = new QuestionBankBLL();
        #region 获取数据
        /// <summary>
        ///个人答题获取目录
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetHistoryAnswerTitle(BaseDataModel<HistoryAnswerTitleModel> dy)
        {

            var result = 0;
            var message = string.Empty;
            try
            {
                if (!dy.allowPaging)
                {
                    dy.pageSize = 10000;
                    dy.pageIndex = 1;
                }
                

                var data = bll.GetHistoryAnswerTitle(dy.userId,dy.data.category,dy.data.startTime,dy.data.endTime,dy.data.iscomplete,dy.pageIndex,dy.pageSize);
                return new { code = result, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        ///人员目录
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetHistoryAnswerTitlebyActivityId(BaseDataModel<string> dy)
        {

            var result = 0;
            var message = string.Empty;
            try
            {
                var data = bll.GetHistoryAnswerTitlebyActivityId(dy.data,"");
                return new { code = result, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取答题内容
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetHistoryDetailbyActivityId(BaseDataModel<string> dy)
        {

            var result = 0;
            var message = string.Empty;
            try
            {
                var userid = dy.userId;
                var data = bll.GetHistoryDetailbyActivityId(dy.data);
                return new { code = result, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }

        }


        /// <summary>
        /// 获取用户答题
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetuserAnswer(BaseDataModel<string> dy)
        {

            var result = 0;
            var message = string.Empty;
            try
            {
                var userid = dy.userId;
                var data = bll.GetHistoryAnswerTitlebyActivityId(dy.data,dy.userId);
                return new { code = result, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }

        }


        #endregion

        #region 数据操作
        /// <summary>
        /// 答题接口  
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public object TheAnswer(BaseDataModel<answerPost> dy)
        {
            try
            {

                bll.answerWork(dy.data.titleid, dy.data.sort,dy.data.answer);
                return new { code = 0, info = "操作成功" };
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 1,
                    info = ex.Message
                };
            }

        }


        /// <summary>
        /// 答题完成 分数计算
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public object TheAnswerComplete(BaseDataModel<string> dy)
        {
            try
            {
                var score = bll.TitleScore(dy.data);
                return new { code = 0, info = "操作成功", data = score };
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 1,
                    info = ex.Message
                };
            }

        }
        #endregion

    }
}
