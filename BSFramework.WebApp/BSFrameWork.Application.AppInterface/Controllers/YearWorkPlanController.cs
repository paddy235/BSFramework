using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.YearWorkPlanManage;
using BSFramework.Application.Entity.YearWorkPlan;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Controllers;
using BSFrameWork.Application.AppInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface
{
    /// <summary>
    /// 
    /// </summary>
    public class YearWorkPlanController : BaseApiController
    {
        private YearWorkPlanBLL bll = new YearWorkPlanBLL();
        #region Models
        public class getList
        {

        }

        #endregion

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetListJson(BaseDataModel dy)
        {

            var result = 0;
            var message = string.Empty;
            try
            {
                Pagination pagination = new Pagination();
                //pagination.p_kid = "id";
                //pagination.p_fields = @"plan,remark,planstart,planend,planfinish,lastprogress,progress,bookmark,deptname,deptid,deptcode,CREATEUSERID,CREATEUSERNAME,MODIFYDATE,MODIFYUSERID,MODIFYUSERNAME,CREATEDATE";
                //pagination.p_tablename = @"(select id,plan,remark,planstart,planend,planfinish,lastprogress,progress,bookmark,deptname,deptid,deptcode,CREATEUSERID,CREATEUSERNAME,MODIFYDATE,MODIFYUSERID,MODIFYUSERNAME,CREATEDATE ";
                //pagination.p_tablename += "  from  (select id,plan,remark,planstart,planend,planfinish,lastprogress,progress,bookmark,deptname,deptid,deptcode,CREATEUSERID,CREATEUSERNAME,MODIFYDATE,MODIFYUSERID,MODIFYUSERNAME,CREATEDATE  from wg_yearworkplan  ";
                //pagination.sidx = "CREATEDATE";
                //pagination.sord = "desc";
                //pagination.conditionJson = "1=1";
                //var user = new UserBLL().GetEntity(dy.userId);
                //pagination.p_tablename += string.Format(" where deptcode like '{0}%'", user.DepartmentCode);
                //pagination.p_tablename += " ORDER BY CREATEDATE DESC) t GROUP BY bookmark ) as t ";
                var user = new UserBLL().GetEntity(dy.userId);
                string queryJson = "{'deptCode':'" + user.DepartmentCode + "'}";
                if (dy.allowPaging)
                {
                    pagination.page = dy.pageIndex;
                    pagination.rows = dy.pageSize;
                }
                else
                {
                    pagination.page = 1;
                    pagination.rows = 2000;
                }
                var data = bll.GetPageList(pagination, queryJson);
                // var dataJson = data.ToJson();
                // var dataList = Json.ToList<OndutyEntity>(dataJson);
                return new { code = result, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object getEntity(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var data = bll.GetEntity(dy.data);
                return new { code = result, info = "操作成功", data = data };
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

        #region 数据操作

        public object OperatingEntity(BaseDataModel<YearWorkPlanEntity> dy)
        {
            try
            {
                var user = new UserBLL().GetEntity(dy.userId);
                bll.SaveForm(user, dy.data);
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
        public object DelEntity(BaseDataModel<string> dy)
        {
            try
            {
                bll.RemoveForm(dy.data);
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

        #endregion

    }
}