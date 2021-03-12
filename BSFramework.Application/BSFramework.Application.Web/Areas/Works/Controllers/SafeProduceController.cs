using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SafeProduceManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.SafeProduceManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Web.Areas.BaseManage.Models;
using BSFramework.Application.Web.Areas.SystemManage.Models;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers

{
    /// <summary>
    /// 安全文明生成检查
    /// </summary>
    public class SafeProduceController : MvcControllerBase
    {
        private SafeProduceBLL _bll = new SafeProduceBLL();
        private DepartmentBLL _dept = new DepartmentBLL();
        private DistrictPersonBLL _DistrictBll = new DistrictPersonBLL();
        #region 
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2()
        {
            return View();
        }
        public ActionResult Form()
        {
            return View();
        }


        #endregion




        #region 查询





        /// <summary>
        /// 获取区域责任人
        /// </summary>
        /// <returns></returns>
        public List<DistrictModel> GetDistrictModelsData()
        {
            //Operator user = OperatorProvider.Provider.Current();
            //var baseUrl = Config.GetValue("ErchtmsApiUrl");
            //var client = new HttpClient();
            //var Company = _dept.GetCompany(user.DeptId);
            //if (Company == null)
            //{
            //    return new List<DistrictModel>();
            //}
            //var param = new { Data = new { companyId = Company.DepartmentId } };
            //var requestContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(param));
            //requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //var json = client.PostAsync(baseUrl + "District/GetDistrict", requestContent).Result.Content.ReadAsStringAsync().Result;
            //var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DistrictModel>>(json);
            //return data;
            var DistrictData = _DistrictBll.GetDistricts();
            List<DistrictModel> list = new List<DistrictModel>();
            List<DistrictModel> ChildrenList = new List<DistrictModel>();
            foreach (var item in DistrictData.OrderBy(x => x.DistrictCode))
            {
                var model = new DistrictModel();
                model.DistrictID = item.DistrictId;
                model.DistrictName = item.DistrictName;
                model.DistrictCode = item.DistrictCode;
                //var hasChildren = DistrictData.Where(x => x.DistrictCode.StartsWith(item.DistrictCode) && x.DistrictId != item.DistrictId);
                //foreach (var items in hasChildren)
                //{
                //    var Children = new DistrictModel();
                //    Children.DistrictID = items.DistrictId;
                //    Children.DistrictName = items.DistrictName;
                //    Children.DistrictCode = items.DistrictCode;
                //    Children.ParentID = item.DistrictId;
                //    ChildrenList.Add(Children);
                //}
                //var ck = ChildrenList.Where(x => x.DistrictID == item.DistrictId);
                //if (ck.Count() > 0)
                //{
                //    continue;
                //}
                model.ParentID = "0";
                list.Add(model);
            }
            //var ChildrenData = DistrictChildren(ChildrenList);
            //list.AddRange(ChildrenData);
            return list;
        }
        /// <summary>
        /// 递归根据code获取下一级
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<DistrictModel> DistrictChildren(List<DistrictModel> list)
        {
            List<DistrictModel> ModelData = new List<DistrictModel>();
            List<DistrictModel> Model = new List<DistrictModel>();

            foreach (var item in list)
            {
                var hasChildren = list.Where(x => x.DistrictCode.StartsWith(item.DistrictCode) && x.DistrictID != item.DistrictID);
                if (hasChildren.Count() > 0)
                {
                    foreach (var items in hasChildren)
                    {
                        var Children = new DistrictModel();
                        Children.DistrictID = items.DistrictID;
                        Children.DistrictName = items.DistrictName;
                        Children.DistrictCode = items.DistrictCode;
                        Children.ParentID = item.DistrictID;
                        Model.Add(Children);
                    }
                }
                else
                {
                    ModelData.Add(item);
                }
            }

            if (Model.Count() > 0)
            {
                var ChildrenList = DistrictChildren(Model);
                ModelData.AddRange(ChildrenList);
            }

            return ModelData;
        }


        /// <summary>
        /// 获取双控编码
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCodeList()
        {
            var baseUrl = Config.GetValue("ErchtmsApiUrl");
            var client = new HttpClient();
            var param = new { Data = "区域责任人设置" };
            var requestContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(param));
            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var json = client.PostAsync(baseUrl + "KbsDeviceManage/GetElements", requestContent).Result.Content.ReadAsStringAsync().Result;
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ListModel<DataItemDetailEntity>>(json);

            return Json(data.Data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取区域责任人
        /// </summary>
        /// <returns></returns>
        public ActionResult getDistrict()
        {
            var ModelsData = GetDistrictModelsData();
            //获取根节点
            var makeDepts = ModelsData.Where(x => x.ParentID == "0");
            var data = makeDepts.Select(x => new TreeModel() { id = x.DistrictID, Code = x.DistrictCode, value = x.DistrictID, text = x.DistrictName, hasChildren = ModelsData.Count(y => y.ParentID == x.DistrictID) > 0, isexpand = ModelsData.Count(y => y.ParentID == x.DistrictID) > 0, ChildNodes = GetChildren(ModelsData, x.DistrictID, false), showcheck = false });


            return Json(data, JsonRequestBehavior.AllowGet);



        }

        /// <summary>
        /// 递归树
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<TreeModel> GetChildren(List<DistrictModel> data, string id, bool showcheck)
        {
            return data.Where(x => x.ParentID == id).Select(x => new TreeModel { id = x.DistrictID, value = x.DistrictID, Code = x.DistrictCode, text = x.DistrictName, isexpand = data.Count(y => y.ParentID == x.DistrictID) > 0, hasChildren = data.Count(y => y.ParentID == x.DistrictID) > 0, ChildNodes = GetChildren(data, x.DistrictID, showcheck), showcheck = showcheck }).ToList();
        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult GetDetail(string keyValue)
        {

            try
            {
                var entity = _bll.getSafeProduceDataById(keyValue);
                return ToJsonResult(entity);
            }
            catch (Exception ex)
            {
                return ToJsonResult(ex);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public ActionResult GetPageSafeProduceList(Pagination pagination, string queryJson)
        {
            try
            {
                var watch = CommonHelper.TimerStart();
                Operator user = OperatorProvider.Provider.Current();
                var data = _bll.GetPageSafeProduceList(pagination, queryJson);
                var JsonData = new
                {
                    rows = data,
                    pagination.total,
                    pagination.page,
                    pagination.records,
                    costtime = CommonHelper.TimerEnd(watch)
                };
                return ToJsonResult(JsonData);
            }
            catch (Exception ex)
            {
                var JsonData = new
                {
                    rows = new List<SafeProduceEntity>(),
                    pagination.total,
                    pagination.page,
                    pagination.records,
                    costtime = "0",
                    ex.Message
                };
                return ToJsonResult(JsonData);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public ActionResult GetPageSafeProduceAndSigninList(Pagination pagination, string queryJson)
        {
            try
            {
                var watch = CommonHelper.TimerStart();
                Operator user = OperatorProvider.Provider.Current();
                var data = _bll.GetPageSafeProduceAndSigninList(pagination, queryJson);
                var JsonData = new
                {
                    rows = data,
                    pagination.total,
                    pagination.page,
                    pagination.records,
                    costtime = CommonHelper.TimerEnd(watch)
                };
                return ToJsonResult(JsonData);
            }
            catch (Exception ex)
            {
                var JsonData = new
                {
                    rows = new object(),
                    pagination.total,
                    pagination.page,
                    pagination.records,
                    costtime = "0",
                    ex.Message
                };
                return ToJsonResult(JsonData);
            }
        }

        #endregion

    }
}