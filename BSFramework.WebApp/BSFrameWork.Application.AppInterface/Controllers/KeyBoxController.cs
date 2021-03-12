using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.KeyBoxManage;
using BSFramework.Application.Entity.KeyboxManage;
using BSFramework.Application.Entity.WebApp;
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
    public class KeyBoxController : BaseApiController
    {

        private KeyBoxBLL _bll = new KeyBoxBLL();

        private UserBLL _userBLL = new UserBLL();
        

        #region 获取数据
        /// <summary>
        ///获取台账左侧分类总结数据
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object getKeyBoxCategoryData(ParamBucket<getKeyBoxCategoryDataModel> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var Category = dy.Data.Category.Split(',').ToList();
                var data = _bll.getKeyBoxCategoryData(dy.Data.DeptId, Category);
                return new { code = result, info = "操作成功",  data = data };
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
        ///查询钥匙数据
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetPageKeyBoxList(ParamBucket<GetPageKeyBoxListModel> dy)
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
                var data = _bll.GetPageKeyBoxList(pagination, queryJson);
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
        ///获取编码
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object getKeyBoxSort(ParamBucket<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var data = _bll.getKeyBoxSort(dy.Data,dy.Data);
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


        /// <summary>
        ///借出纪录台账
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetPageKeyUseList(ParamBucket<GetPageKeyUseListModel> dy)
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
                var data = _bll.GetPageKeyUseList(pagination, queryJson);
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
        ///正在借出的纪录
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetPageKeyUseListByState(ParamBucket<GetPageKeyUseListByStateModel> dy)
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
                var data = _bll.GetPageKeyUseListByState(pagination, queryJson);
                return new { code = result, info = "操作成功",data = data };
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
        ///添加钥匙
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object operateKeyBox(ParamBucket<List<KeyBoxEntity>> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var user = _userBLL.GetEntity(dy.UserId);
                foreach (var item in dy.Data)
                {
                    if (string.IsNullOrEmpty(item.ID))
                    {
                        
                        item.CreateDate = DateTime.Now;
                        item.CreateUserId = user.UserId;
                        item.CreateUserName = user.CreateUserName;
                        item.ModifyDate = DateTime.Now;
                        item.ModifyUserId = user.UserId;
                        item.ModifyUserName = user.ModifyUserName;
                    }
                    else
                    {
                        item.ModifyDate = DateTime.Now;
                        item.ModifyUserId = user.UserId;
                        item.ModifyUserName = user.ModifyUserName;
                    }
                }
               _bll.operateKeyBox(dy.Data);
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
        ///删除钥匙
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object removeKeyBox(ParamBucket<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                _bll.removeKeyBox(dy.Data);
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
        ///钥匙借用记录操作
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object operateKeyUse(ParamBucket<KeyUseEntity> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
              
                _bll.operateKeyUse(dy.Data);
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
        ///归还钥匙
        /// </summary>
        [HttpPost]
        public object ReturnKey(ParamBucket<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                _bll.ReturnKey(dy.Data,dy.UserId);
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
        //借用钥匙
        /// </summary>
        [HttpPost]
        public object BorrowKey(ParamBucket<KeyUseEntity> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var user = _userBLL.GetEntity(dy.UserId);
                if (string.IsNullOrEmpty(dy.Data.ID))
                {
                    dy.Data.LoanUser = user.RealName;
                    dy.Data.LoanUserId = user.UserId;
                    dy.Data.LoanDate = DateTime.Now;
                }
                _bll.BorrowKey(dy.Data);
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
