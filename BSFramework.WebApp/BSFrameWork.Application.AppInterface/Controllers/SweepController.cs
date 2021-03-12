using BSFramework.Application.Busines.SweepManage;
using BSFramework.Application.Entity.SweepManage;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    ///保洁管理
    /// </summary>
    public class SweepController : BaseApiController
    {
        private SweepBLL _bll = new SweepBLL();

        #region 获取数据

        /// <summary>
        ///查询SweepList
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetPageSweepList(ParamBucket<GetPageSweepListModel> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                Pagination pagination = new Pagination();
                if (dy.AllowPaging)
                {
                    pagination.page = dy.PageIndex;
                    pagination.rows = dy.PageSize;
                }
                else
                {
                    pagination.page = 1;
                    pagination.rows = 1000;
                }
                var queryJson = JsonConvert.SerializeObject(dy.Data);
                var data = _bll.GetPageSweepList(pagination, queryJson);
                return new { code = result, info = "操作成功", count = pagination.records, data = data };
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
        ///查询SweepAndItemList 获取今天
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetPageSweepAndItemList(ParamBucket<GetPageSweepListModel> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                Pagination pagination = new Pagination();
                if (dy.AllowPaging)
                {
                    pagination.page = dy.PageIndex;
                    pagination.rows = dy.PageSize;
                }
                else
                {
                    pagination.page = 1;
                    pagination.rows = 1000;
                }
                //var queryJson = JsonConvert.SerializeObject(dy.Data);
                var time = DateTime.Now.ToString("yyyy-MM-dd");
                var queryJson = "{\"UserId\":\"" + dy.Data.UserId + "\",\"DistrictId\":\"" + dy.Data.DistrictId + "\",\"StartData\":\"" + time + "\",\"EndData\":\"" + time + "\"}";
                var data = _bll.GetPageSweepAndItemList(pagination, queryJson);
                return new { code = result, info = "操作成功", count = pagination.records, data = data };
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

        /// <summary>
        ///添加
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object operateSweepAndItem(ParamBucket<SweepEntity> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var list = new List<SweepEntity>();
                list.Add(dy.Data);
                _bll.operateSweepAndItem(list);
                return new { code = result, info = "操作成功" };
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
        ///删除
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object removeSweepAndItem(ParamBucket<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                _bll.removeSweepAndItem(dy.Data);
                return new { code = result, info = "操作成功" };
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
