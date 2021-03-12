using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SafeProduceManage;
using BSFramework.Application.Entity.SafeProduceManage;
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
    /// 安全文明生产
    /// </summary>
    public class SafeProduceController : BaseApiController
    {
        private SafeProduceBLL _bll = new SafeProduceBLL();
        private UserBLL _userBLL = new UserBLL();

        #region 获取数据
        /// <summary>
        ///问题台账
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetPageSafeProduceList(ParamBucket<GetPageKeyUseListModel> dy)
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
                var data = _bll.GetPageSafeProduceList(pagination, queryJson);
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
        ///终端页面签到
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetPageOnLocaleList(ParamBucket<GetOnLocale> dy)
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
                var data = _bll.GetPageOnLocaleList(pagination, queryJson);
                foreach (var item in data)
                {
                    if (!string.IsNullOrEmpty(item.photo))
                    {

                        if (!item.photo.Contains("http"))
                        {
                            item.photo = BSFramework.Util.Config.GetValue("AppUrl") + item.photo;
                        }
                    }

                }

                return new ListBucket<OnLocaleEntity>() { code = result, info = "操作成功", Total = pagination.records, Data = data };
            }
            catch (Exception ex)
            {

                return new ResultBucket()
                {
                    code = 1,
                    info = ex.Message
                };
            }
        }

        /// <summary>
        ///终端页面缺勤
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetPageNoOnLocaleList(ParamBucket<GetOnLocale> dy)
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
                var data = _bll.GetPageNoOnLocaleList(pagination, queryJson);


                return new ListBucket<OnLocaleModel>() { code = result, info = "操作成功", Total = pagination.records, Data = data };
            }
            catch (Exception ex)
            {

                return new ResultBucket()
                {
                    code = 1,
                    info = ex.Message
                };
            }
        }
        #endregion

        #region 操作数据
        /// <summary>
        ///添加
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object operateSafeProduce(ParamBucket<SafeProduceEntity> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                //var user = _userBLL.GetEntity(dy.UserId);


                //if (string.IsNullOrEmpty(dy.Data.Id))
                //{
                //    dy.Data.CreateDate = DateTime.Now;
                //    //dy.Data.CreateUserId = user.UserId;
                //    //dy.Data.CreateUserName = user.CreateUserName;
                //}
                //else
                //{
                //    dy.Data.ModifyDate = DateTime.Now;
                //    //dy.Data.ModifyUserId = user.UserId;
                //    //dy.Data.ModifyUserName = user.CreateUserName;
                //}

                _bll.operateSafeProduce(dy.Data);
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
        /// 保存修改现场终端功能踩点记录
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object operateOnLocale(ParamBucket<OnLocaleEntity> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                //var user = _userBLL.GetEntity(dy.UserId);
                dy.Data.SigninDate = DateTime.Now;
                _bll.operateOnLocale(dy.Data);

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
        public object removeSafeProduce(ParamBucket<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                _bll.removeSafeProduce(dy.Data);
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
