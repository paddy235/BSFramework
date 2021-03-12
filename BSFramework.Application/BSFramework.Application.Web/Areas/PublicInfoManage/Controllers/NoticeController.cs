using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.PublicInfoManage.Controllers
{
    /// <summary>
    /// 描 述：电子公告
    /// </summary>
    public class NoticeController : MvcControllerBase
    {
        private NoticeBLL noticeBLL = new NoticeBLL();

        #region 视图功能
        /// <summary>
        /// 公告管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 公告表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            var id = this.Request.QueryString.Get("keyValue");
            if (string.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();
            ViewBag.id = id;
            return View();
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Detail()
        {
            var id = this.Request.QueryString.Get("keyValue");
            if (string.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();
            ViewBag.id = id;
            return View();
        }

        public ActionResult NewDetail(string keyValue)
        {
            var model = noticeBLL.GetEntity(keyValue);
            FileInfoBLL fi = new FileInfoBLL();
            var list = fi.GetFilesByRecIdNew(keyValue);
            ViewData["files"] = list;
            return View(model);
        }

        /// <summary>
        /// 新首页通知公告分部页
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexNoticePartView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AjaxNoticData()
        {
            var user = OperatorProvider.Provider.Current();
            var dict = new
            {
                userid = user.UserId,
                data = new
                {
                    type = "",
                    title = "",
                    pageNum = 1,
                    pageSize = 10
                }
            };
            string res = string.Empty;
            if (Debugger.IsAttached)
            {
                res = "{  \"code\": 0,  \"info\": \"获取数据成功\",  \"count\": 1,  \"data\": [    {      \"id\": \"aed65456-2bdd-4ddc-b659-4b0699151e57\",      \"createuserid\": \"6f67b1d3-a2e2-45d0-9245-c3232528c95f\",      \"title\": \"20181203通知公告20181203通知公告120181203通知公告120181203通知公告120181203通知公告120181203通知公告120181203通知公告120181203通知公告120181203通知公告120181203通知公告11\",      \"publisher\": \"安全部负责人\",      \"publisherdept\": \"安环部\",      \"releasetime\": \"2018-12-03 18:31\",      \"isimportant\": \"是\",      \"issend\": \"0\",      \"isread\": \"0\",      \"userid\": \"350c9862-5978-42ae-a779-26e39c67baab,5b7bf18e-0fe8-ab6c-05e1-812b1defec7a,edf09c5f-fbd9-8c4a-d65e-262544ac812a,96bae42b-b0ac-4227-8f48-dfe63ca469e9,aa2f6f58-c3d4-4756-bf03-ee485ec73f8f,d0ac0860-73ca-4673-9206-d5560902fc37,5dca8859-0526-41d1-987e-dada4cf676de,0138dbb0-743d-42fe-85b9-c346a3934565,03f625f9-6b7a-4c0d-8433-26394d0947dc,b7017c89-05f3-4a48-951d-f6f020d87f8e,b63ca02a-375f-4882-bbdf-bf3405a4c048,3c76bfc3-95ed-460b-ad81-89dbf31e9bde,1f6fa724-fba1-41cf-96c7-7bf769fddedc,24f6a3a4-ef85-49d3-b43a-8bc5a5c86631,1f22bcaf-e886-45fb-8733-8e85bc48b247,6aee035f-f586-47c8-b009-27f8228b66f9,0f0f3ad7-acec-41be-85e6-c355bdde9ba8,fcf99146-5ea1-44e7-a9e7-4a983d12774b,9c47f413-b5ab-4ca4-b563-4b58b5cf7b2e,86e31d96-0071-46c5-a718-4d9156bcbfaa,57209043-90c8-48ac-b5b3-7d807937f1a7,bbbdd3ca-f50c-4f69-8377-e6f9fa84cb68,8054f57c-79e5-4775-869d-878dc35b9c8a,32c13338-7ffb-4e15-a635-5a862ec33e99,54f2bb4c-59a2-4b2e-8a0e-60648bd9049d,8aaa854f-aa95-40f7-8976-d298cd4d2ef6,be5a108b-21fb-4ee8-b699-e0da626a6e6f,aef7bb01-af7d-4dad-943f-37b960fbb67c,1852789a-d2e0-48a2-be63-9b951d037c23,228db740-a594-44aa-8ade-77528a9e43d4,1bacb33c-6a3a-40d1-85c1-3b1ae04f3b7d,59f82836-e62d-492f-b373-141756a5e240,6f67b1d3-a2e2-45d0-9245-c3232528c95f,54c5888b-a6b1-4c42-a56a-99def312aa1f,9009b613-04dd-4fa4-8bd0-eab6bdca2774,f44046ab-a373-4508-80df-a2885bffd76e,9945d5fc-35b7-44cc-b71c-6ac7e0edd1d1,591d965e-4f7c-4218-b298-18650eeb03ee,a44d2ce5-c0c0-492e-bce4-6f83782880dc,edbc02b5-e21a-4541-a197-5af1398517be,780f5cf8-51e7-4f6f-84d5-8d84374c18af,11a16aee-233a-4982-88f7-85c33fe2bd57,c13f0d21-cb9b-468e-a028-ea2a674d5b45,f189e14f-61f6-429f-aac0-c78770ac890f,7fa1c4cf-732d-436f-8bd5-a5a4f602b954,1b277532-1f61-41b7-8a44-9f34c49b021b,18ae0ea1-be29-4da8-81d9-0a4e1bc67a68,85cefe9c-7bc1-49d2-a37c-70e946af2f9a,c22e213b-51f9-4301-827e-4fe41240a6c0,c1b69f0e-09b0-47cd-95b9-93e5f71514ce,dd0b76b9-9093-440f-ab8a-189afdcadc80,9a123283-c07a-4aca-bca6-ef5e9ee9ba9e,ff1de75b-d8fb-415b-9e73-a6fa2a3b5f75,1fcdad11-697e-4736-be79-f7ee2d7da947,3048ed6b-63e7-43c4-82b2-5e03518a4c4c,eb72f907-afd5-47d5-8ba7-0944c9c4009a,85658302-91a2-4c8c-8770-6d89fd933b96,f1f2090a-b326-4d59-8e3d-4bee7c0be42cc01de65b-d3d2-48a8-8442-42c6035ac17f,\",      \"content\": \"测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知\",      \"r\": 1    },	 {      \"id\": \"aed65456-2bdd-4ddc-b659-4b0699151e57\",      \"createuserid\": \"6f67b1d3-a2e2-45d0-9245-c3232528c95f\",      \"title\": \"20181203通知公告1\",      \"publisher\": \"安全部负责人\",      \"publisherdept\": \"安环部\",      \"releasetime\": \"2018-12-03 18:31\",      \"isimportant\": \"是\",      \"issend\": \"0\",      \"isread\": \"0\",      \"userid\": \"350c9862-5978-42ae-a779-26e39c67baab,5b7bf18e-0fe8-ab6c-05e1-812b1defec7a,edf09c5f-fbd9-8c4a-d65e-262544ac812a,96bae42b-b0ac-4227-8f48-dfe63ca469e9,aa2f6f58-c3d4-4756-bf03-ee485ec73f8f,d0ac0860-73ca-4673-9206-d5560902fc37,5dca8859-0526-41d1-987e-dada4cf676de,0138dbb0-743d-42fe-85b9-c346a3934565,03f625f9-6b7a-4c0d-8433-26394d0947dc,b7017c89-05f3-4a48-951d-f6f020d87f8e,b63ca02a-375f-4882-bbdf-bf3405a4c048,3c76bfc3-95ed-460b-ad81-89dbf31e9bde,1f6fa724-fba1-41cf-96c7-7bf769fddedc,24f6a3a4-ef85-49d3-b43a-8bc5a5c86631,1f22bcaf-e886-45fb-8733-8e85bc48b247,6aee035f-f586-47c8-b009-27f8228b66f9,0f0f3ad7-acec-41be-85e6-c355bdde9ba8,fcf99146-5ea1-44e7-a9e7-4a983d12774b,9c47f413-b5ab-4ca4-b563-4b58b5cf7b2e,86e31d96-0071-46c5-a718-4d9156bcbfaa,57209043-90c8-48ac-b5b3-7d807937f1a7,bbbdd3ca-f50c-4f69-8377-e6f9fa84cb68,8054f57c-79e5-4775-869d-878dc35b9c8a,32c13338-7ffb-4e15-a635-5a862ec33e99,54f2bb4c-59a2-4b2e-8a0e-60648bd9049d,8aaa854f-aa95-40f7-8976-d298cd4d2ef6,be5a108b-21fb-4ee8-b699-e0da626a6e6f,aef7bb01-af7d-4dad-943f-37b960fbb67c,1852789a-d2e0-48a2-be63-9b951d037c23,228db740-a594-44aa-8ade-77528a9e43d4,1bacb33c-6a3a-40d1-85c1-3b1ae04f3b7d,59f82836-e62d-492f-b373-141756a5e240,6f67b1d3-a2e2-45d0-9245-c3232528c95f,54c5888b-a6b1-4c42-a56a-99def312aa1f,9009b613-04dd-4fa4-8bd0-eab6bdca2774,f44046ab-a373-4508-80df-a2885bffd76e,9945d5fc-35b7-44cc-b71c-6ac7e0edd1d1,591d965e-4f7c-4218-b298-18650eeb03ee,a44d2ce5-c0c0-492e-bce4-6f83782880dc,edbc02b5-e21a-4541-a197-5af1398517be,780f5cf8-51e7-4f6f-84d5-8d84374c18af,11a16aee-233a-4982-88f7-85c33fe2bd57,c13f0d21-cb9b-468e-a028-ea2a674d5b45,f189e14f-61f6-429f-aac0-c78770ac890f,7fa1c4cf-732d-436f-8bd5-a5a4f602b954,1b277532-1f61-41b7-8a44-9f34c49b021b,18ae0ea1-be29-4da8-81d9-0a4e1bc67a68,85cefe9c-7bc1-49d2-a37c-70e946af2f9a,c22e213b-51f9-4301-827e-4fe41240a6c0,c1b69f0e-09b0-47cd-95b9-93e5f71514ce,dd0b76b9-9093-440f-ab8a-189afdcadc80,9a123283-c07a-4aca-bca6-ef5e9ee9ba9e,ff1de75b-d8fb-415b-9e73-a6fa2a3b5f75,1fcdad11-697e-4736-be79-f7ee2d7da947,3048ed6b-63e7-43c4-82b2-5e03518a4c4c,eb72f907-afd5-47d5-8ba7-0944c9c4009a,85658302-91a2-4c8c-8770-6d89fd933b96,f1f2090a-b326-4d59-8e3d-4bee7c0be42cc01de65b-d3d2-48a8-8442-42c6035ac17f,\",      \"content\": \"测试通知\",      \"r\": 1    }  ]}";
            }
            else
            {
                res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "RoutineSafetyWork", "GetAnnouncementList"), "json=" + JsonConvert.SerializeObject(dict));
            }
            var ret = JsonConvert.DeserializeObject<RetDataModel>(res);
            List<NnoticeModel> list = new List<NnoticeModel>();
            if (ret != null && ret.code != "-1")
            {
                list = JsonConvert.DeserializeObject<List<NnoticeModel>>(ret.data.ToString());
            }
            return Content(JsonConvert.SerializeObject(list,new JsonSerializerSettings() {  DateFormatString="yyyy-MM-dd"}));
        }
        /// <summary>
        /// 部门首页通知公告详情页
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexShowDetail()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 公告列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            var data = noticeBLL.GetPageList(pagination, queryJson);
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

        public ActionResult List(int page, int pagesize, string from, string to, string name,FormCollection fc)
        {
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;

            if (string.IsNullOrEmpty(from)) from = fc.Get("MeetingStartTime");
            if (string.IsNullOrEmpty(to)) to = fc.Get("MeetingEndTime");

            ViewData["from"] = from;
            ViewData["to"] = to;

            var user = OperatorProvider.Provider.Current();
            var total = 0;
            var data = noticeBLL.GetAllNotice(user.DeptId, string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from), string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to), name, page, pagesize, out total);
            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;

            return View(data);
        }
        /// <summary>
        /// 公告实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = noticeBLL.GetEntity(keyValue);
            FileInfoBLL fi = new FileInfoBLL();
            IList list = fi.GetFilesByRecId(keyValue);
            return ToJsonResult(new { formData = data, files = list });
        }

       
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除公告信息")]
        public ActionResult RemoveForm(string keyValue)
        {
            noticeBLL.RemoveForm(keyValue);
            return Success("删除成功。");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveFile(string fileName, string recId)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            FileInfoEntity entity = fileBll.GetEntity(recId, fileName);
            int res = 0;
            if (entity != null)
            {
                res = fileBll.DeleteFile(recId, fileName, Server.MapPath(entity.FilePath));
            }
            return res > 0 ? Success("操作成功。") : Error("操作失败");
        }

        /// <summary>
        /// 保存公告表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="newsEntity">公告实体</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        [ValidateInput(false)]
        [HandlerMonitor(5, "保存公告表单(新增、修改)")]
        public ActionResult SaveForm(string keyValue, NewsEntity newsEntity)
        {
            var user = OperatorProvider.Provider.Current();
            newsEntity.NoticeDept = user.DeptName;

            noticeBLL.SaveForm(keyValue, newsEntity);
            return Success("操作成功。");
        }

        public ActionResult CurrentNotice()
        {
            var user = OperatorProvider.Provider.Current();
            int month = 1;
            if (DateTime.Now.Month < 4) month = 1;
            else if (DateTime.Now.Month < 7) month = 4;
            else if (DateTime.Now.Month < 10) month = 7;
            else if (DateTime.Now.Month <= 12) month = 10;

            var data = noticeBLL.GetCurrentNotice(user.DeptId, new DateTime(2000, 1, 1));

            return View(data);
        }

        public ActionResult CurrentNotice2()
        {
            var user = OperatorProvider.Provider.Current();
            var data = noticeBLL.GetCurrentNotice(user.DeptId, DateTime.Today.AddDays(-7));

            return View(data);
        }
        #endregion
    }
}
