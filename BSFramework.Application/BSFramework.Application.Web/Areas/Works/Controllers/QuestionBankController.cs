using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.ExperienceManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.QuestionManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.QuestionManage;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 答题
    /// </summary>
    public class QuestionBankController : MvcControllerBase
    {

        private QuestionBankBLL bll = new QuestionBankBLL();
        private UserBLL userbll = new UserBLL();
        private FileInfoBLL filebll = new FileInfoBLL();

        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Question()
        {
            return View();
        }

        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult webQuestion()
        {
            return View();
        }
        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail(string keyvalue)
        {
            var entity = bll.GetDetailbyId(keyvalue);
            return View(entity);
        }
        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult HistoryDetail(string keyvalue)
        {
            var entity = bll.GetHistoryDetailbyId(keyvalue);
            return View(entity);
        }
        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult webDetail()
        {
         
            return View();
        }
        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult answerscore(string activityid)
        {
            var QuestionList = bll.GetHistoryDetailbyActivityId(activityid);
            ViewData["QuestionList"] = Newtonsoft.Json.JsonConvert.SerializeObject(QuestionList);
            var userul = bll.GetHistoryAnswerTitlebyActivityId(activityid,"");
            ViewData["usertitle"] = Newtonsoft.Json.JsonConvert.SerializeObject(userul);
            return View();
        }
        
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        public ActionResult GetListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            var total = 0;
            //int records = 0;
            //pagination.p_kid = "id";
            //pagination.p_fields = @"topictype,topictitle,description,outkeyvalue,CREATEUSERID,CREATEUSERNAME,MODIFYDATE,MODIFYUSERID,MODIFYUSERNAME,CREATEDATE";
            //pagination.p_tablename = @"wg_questionbank";
            //pagination.sidx = "CREATEDATE";
            //pagination.sord = "desc";
            //pagination.conditionJson = "1=1";
            var queryParam = queryJson.ToJObject();
            var keyvalue = string.Empty;
            if (!queryParam["keyvalue"].IsEmpty())
            {
                keyvalue = queryParam["keyvalue"].ToString();

            }
            var user = OperatorProvider.Provider.Current();
            //var myuser = userbll.GetEntity(user.UserId);
            var data = bll.GetPageList(keyvalue, pagination.rows, pagination.page, out total);
            var JsonData = new
            {
                rows = data,
                total = Math.Ceiling((double)total / pagination.rows),
                page = pagination.page,
                records = total,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }

        public ActionResult getFile(string keyValue)
        {
            var file = filebll.GetFilesByRecId(keyValue);

            return Content(file.ToJson());
        }



        /// <summary>
        /// 保存设置数据
        /// </summary>
        /// <returns></returns>   
        /// [HttpPost]

        public ActionResult SaveData(string entity)
        {
            var user = OperatorProvider.Provider.Current();
            var getEntity = JsonConvert.DeserializeObject<QuestionBankEntity>(entity);
            var userEntity = userbll.GetEntity(user.UserId);
            try
            {
                if (getEntity.TheAnswer==null)
                {
                    getEntity.TheAnswer = new List<TheAnswerEntity>();
                }
                bll.SaveFormQuestion(getEntity, getEntity.TheAnswer, "", userEntity);
                return Success("保存成功");
                //return Success("保存成功");
            }
            catch (Exception)
            {
                return Error("保存失败");
            }

        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult GetDetail(string keyvalue)
        {
            var entity = bll.GetDetailbyId(keyvalue);
            return Content(entity.ToJson());


        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult RemoveForm(string keyvalue)
        {
            try
            {
                bll.RemoveForm(keyvalue);
                return Success("操作成功");

            }
            catch (Exception)
            {
                return Error("操作失败");
            }
        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult RemoveFormByOutId(string keyvalue)
        {
            try
            {
                bll.RemoveFormByOutId(keyvalue);
                return Success("操作成功");

            }
            catch (Exception)
            {
                return Error("操作失败");
            }
        }


    }
}
