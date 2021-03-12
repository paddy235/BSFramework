using BSFramework.Application.Busines.CustomParameterManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.CustomParameterManage;
using BSFramework.Util;
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
    /// 自定义台账  模板
    /// </summary>
    public class CustomParameterController : MvcControllerBase
    {
        #region 模板
        /// <summary>
        ///模板台账页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Template()
        {
            return View();
        }
        /// <summary>
        ///模板编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult TemplateForm()
        {
            return View();
        }

        /// <summary>
        ///布局展示页面
        /// </summary>
        /// <returns></returns>
        public ActionResult TemplateShow()
        {
            return View();
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>   
        public ActionResult GetCTPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            //var queryParam = queryJson.ToJObject();
            var ctBll = new  CustomTemplateBLL();
            var user = OperatorProvider.Provider.Current();
            var data = ctBll.GetPageList(pagination, queryJson, user.UserId);
            var jsonStr = JsonConvert.SerializeObject(data);
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult getEntity(string keyvalue)
        {
            var ctBll = new CustomTemplateBLL();
            var data = ctBll.getEntity(keyvalue);
            return Content(data.ToJson());
        }


        

      
        #region 操作
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(CustomTemplateEntity entity)
        {
            try
            {
                var user = OperatorProvider.Provider.Current();
                var ctBll = new CustomTemplateBLL();
                ctBll.SaveForm(entity, user.UserId);
                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]

        public ActionResult delEntity(string keyvalue)
        {
            try
            {
                var ctBll = new CustomTemplateBLL();
                var cpBll = new CustomParameterBLL();
                var entity = cpBll.getListbyCTId(keyvalue);
                if (entity.Count()>0)
                {
                    return Error("已存在台账数据，无法删除。");
                }
                ctBll.deleteEntity(keyvalue);
                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }



        #endregion
        #endregion

        #region 
        /// <summary>
        ///台账页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CParameter()
        {
            return View();
        }
        /// <summary>
        ///详情页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CParameterShow()
        {
            return View();
        }

        
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>   
        public ActionResult GetCPPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            //var queryParam = queryJson.ToJObject();
            var cpBll = new CustomParameterBLL();
            var user = OperatorProvider.Provider.Current();
            var data = cpBll.GetPageList(pagination, queryJson, user.UserId);
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult getCPEntity(string keyvalue)
        {
            var cpBll = new CustomParameterBLL();
            var fileBll = new FileInfoBLL();
            var data = cpBll.getEntity(keyvalue);
            data.Files = fileBll.GetFilesByRecIdNew(keyvalue).ToList();
            return Content(data.ToJson());
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult setSelect(string keyvalue)
        {
            var cpBll = new CustomTemplateBLL();
            var data = cpBll.setSelect(keyvalue);
            return Content(data.ToJson());
        }

        #endregion

    }
}