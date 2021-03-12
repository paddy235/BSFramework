using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.EmergencyManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Bst.Fx.Uploading;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 双控应急管理
    /// </summary>
    public class ERCHTMSEmergencyController : MvcControllerBase
    {



        private static T webClientEmergency<T>(string url, string val) where T : class
        {
            var webclient = new WebClient();
            var ApiIp = Config.GetValue("ErchtmsApiUrl");
            NameValueCollection postVal = new NameValueCollection();
            postVal.Add("json", val);
            var getData = webclient.UploadValues(ApiIp + url, postVal);
            var result = System.Text.Encoding.UTF8.GetString(getData);
            NLog.LogManager.GetCurrentClassLogger().Info("windows终端-应急演练\r\n-->请求地址：{0}\r\n-->请求数据：{1}\r\n-->返回数据：{2}", url, val, result);
            return JsonConvert.DeserializeObject<T>(result);
        }

        /// <summary>
        /// 应急预案
        /// </summary>
        /// <returns></returns>
        public ActionResult drillPlan()
        {
            var user = OperatorProvider.Provider.Current();
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            //test
            // user.UserId = "64465ded-6f94-47a7-83e0-4249721f4e4f";
            //应急预案
            var valueStr = string.Format("\"userid\":\"{0}\"", user.UserId);
            var DataStr = string.Format("\"enCode\":\"{0}\"", "MAE_DirllMode");
            DataStr = "{" + DataStr + "}";
            valueStr = "{" + valueStr + ",\"data\":" + DataStr + "}";
            var dyresult = webClientEmergency<EmergencyList>("EmergencyPlatform/GetDataItemListJson", valueStr);
            var content = new List<DataItemDetailEntity>();
            if (dyresult.data != null && dyresult.data.Count > 0)
            {
                content = dyresult.data.Select(x => new Entity.SystemManage.DataItemDetailEntity { ItemValue = x.ItemValue, ItemName = x.ItemName }).ToList();
            }
            content.Insert(0, new Entity.SystemManage.DataItemDetailEntity() { ItemValue = "全部", ItemName = "全部" });
            //编制部门
            var departBll = new DepartmentBLL();
            var contentDepart = departBll.GetList().ToList();
            contentDepart.Insert(0, new Entity.BaseManage.DepartmentEntity { DepartmentId = "全部", FullName = "全部" });
            ViewBag.Depart = contentDepart;
            ViewBag.content = content;
            ViewData["name"] = "";
            //ViewData["from"] = from;
            //ViewData["to"] = to;
            //获取应急预案列表
            var dyresultS = webClientEmergency<EmergencyDataList>("EmergencyPlatform/GetReserverplanList",
            "{'userid':'" + user.UserId + "','data':{'name':''},'pageindex':1,pagesize:1000}");
            return View(dyresultS);

        }



        //public JsonResult Edit(SaveReserverplanModel model, FormCollection fc)
        //{
        //   var  success = false;
        //    return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode("") });

        //}
        /// <summary>
        /// 终端页面应急预案
        /// </summary>
        /// <param name="deptName"></param>
        /// <param name="EmergencyType"></param>
        /// <returns></returns>
        public ActionResult getEmergencyWorkList(string name)
        {
            var user = OperatorProvider.Provider.Current();
            //test
            var dyresultS = webClientEmergency<EmergencyDataList>("EmergencyPlatform/GetReserverplanList",
               "{'userid':'" + user.UserId + "','data':{'name':'" + name + "'},'pageindex':1,pagesize:1000}");
            return Content(dyresultS.ToJson());
        }

        public ActionResult drillProgramme(string EmergencyId)
        {
            var user = OperatorProvider.Provider.Current();
            //test
            var dyresultS = webClientEmergency<EmergencyDataList>("EmergencyPlatform/GetReserverplanList",
               "{'userid':'" + user.UserId + "','data':{'name':''},'pageindex':1,pagesize:1000}");
            var one = dyresultS.data.FirstOrDefault(x => x.id == EmergencyId);
            return View(one);
        }
        /// <summary>
        /// 应急处置卡
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        #region 应急演练
        /// <summary>
        /// 应急演练
        /// </summary>
        /// <returns></returns>
        public ActionResult drillReady()
        {

            var user = OperatorProvider.Provider.Current();
            //test
            //获取应急演练列表
            var dyresult = webClientEmergency<GetListDrillRecordList>("EmergencyPlatform/GetListDrillRecord",
                          "{\"userid\":\"" + user.UserId + "\"}");

            if (dyresult.Code == 0)
            {
                if (dyresult.data.Count > 0)
                {
                    foreach (var item in dyresult.data)
                    {
                        //跳转到第一步骤
                        if (item.status == "0")
                        {
                            return Redirect("drillObjective?EmergencyReportId=" + item.id);
                        }
                    }

                }
            }

            UserBLL userBLL = new UserBLL();
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            ViewData["users"] = users;
            var bzaqy = users.FirstOrDefault(x => x.DutyName == "安全员");
            //初始化默认数据
            var model = new EmergencyReportEntity()
            {
                EmergencyReportId = Guid.NewGuid().ToString(),
                planstarttime = DateTime.Now,
                emergencyplace = "班组办公室",
                chairperson = user.UserName,
                chairpersonid = user.UserId,
                alerttype = "提前15分钟",
                Persons = string.Join(",", users.Select(x => x.RealName)),
                PersonId = string.Join(",", users.Select(x => x.UserId))
            };
            //获取班组人员  供人员选择
            model.EmergencyPersons = users.Select(x => new EmergencyPersonEntity()
            {
                EmergencyReportId = model.EmergencyReportId,
                EmergencyPersonId = Guid.NewGuid().ToString(),
                PersonId = x.UserId,
                Person = x.RealName,
                IsSigned = true
            }).ToList();
            //演练类型
            var valueStr = string.Format("\"userid\":\"{0}\"", user.UserId);
            var DataStr = string.Format("\"enCode\":\"{0}\"", "MAE_DirllMode");
            DataStr = "{" + DataStr + "}";
            valueStr = "{" + valueStr + ",\"data\":" + DataStr + "}";
            var dyEmergencyList = webClientEmergency<EmergencyList>("EmergencyPlatform/GetDataItemListJson", valueStr);
            var rehearsetype = new List<SelectListItem>();
            if (dyEmergencyList.data != null && dyEmergencyList.data.Count > 0)
            {
                rehearsetype = dyEmergencyList.data.Select(x => new SelectListItem() { Value = x.ItemValue, Text = x.ItemName }).ToList();
            }
            //获取班组下的演练预案
            var dyresultS = webClientEmergency<EmergencyDataList>("EmergencyPlatform/GetReserverplanList",
                "{'userid':'" + user.UserId + "','data':{'name':''},'pageindex':1,pagesize:1000}");
            var emergencyName = new List<SelectListItem>();
            // var emergencyplan = new List<SelectListItem>();
            if (dyresultS.Code == 0)
            {
                if (dyresultS.data.Count > 0)
                {
                    foreach (var item in dyresultS.data)
                    {
                        emergencyName.Add(new SelectListItem() { Value = item.id, Text = item.name });
                    }
                }
            }
            ViewBag.EmergencyReportId = model.EmergencyReportId;
            ViewBag.emergencyName = emergencyName;

            //ViewBag.emergencyplan = emergencyplan;
            ViewBag.rehearsetype = rehearsetype;
            return View(model);
        }
        /// <summary>
        /// 演练第一步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillObjective(string EmergencyReportId, string entity)
        {

            string content = string.Empty;
            var user = OperatorProvider.Provider.Current();
            //test
            ////test


            if (string.IsNullOrEmpty(entity))
            {
                if (Session["Emergency"] != null)
                {
                    entity = Session["Emergency"].ToString();
                    var entitys = JsonConvert.DeserializeObject<EmergencyDataDetailList>(entity);
                    content = JsonConvert.SerializeObject(entitys);
                    ViewBag.EmergencyReportId = entitys.data.id;
                }
                else
                {


                    //获取详情
                    var valueStr = string.Format("\"userid\":\"{0}\"", user.UserId);
                    var DataStr = string.Format("\"keyvalue\":\"{0}\"", EmergencyReportId);
                    DataStr = "{" + DataStr + "}";
                    valueStr = "{" + valueStr + ",\"data\":" + DataStr + "}";
                    var result = webClientEmergency<EmergencyDataDetailList>("EmergencyPlatform/GetDrillRecordForm", valueStr);

                    if (result.Code == 0)
                    {
                        content = JsonConvert.SerializeObject(result);
                    }

                    ViewBag.EmergencyReportId = EmergencyReportId;
                }
            }
            else
            {
                var entitys = JsonConvert.DeserializeObject<EmergencyDataDetailList>(entity);
                content = JsonConvert.SerializeObject(entitys);
                ViewBag.EmergencyReportId = entitys.data.id;
            }


            ViewBag.content = content;
            return View();
        }

        /// <summary>
        /// 演练第二步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillScene(string entity)
        {
            Session["Emergency"] = entity;
            if (Session["Emergency"] != null)
            {
                entity = Session["Emergency"].ToString();
            }

            var entitys = JsonConvert.DeserializeObject<EmergencyDataDetailList>(entity);
            ViewBag.content = JsonConvert.SerializeObject(entitys);
            ViewBag.EmergencyReportId = entitys.data.id;
            return View();
        }
        /// <summary>
        /// 演练第三步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillResponse(string entity)
        {
            Session["Emergency"] = entity;
            if (Session["Emergency"] != null)
            {
                entity = Session["Emergency"].ToString();
            }
            //获取步骤数据 并进行修改
            var entitys = JsonConvert.DeserializeObject<EmergencyDataDetailList>(entity);

            UserBLL userBLL = new UserBLL();
            var user = OperatorProvider.Provider.Current();
            //test
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            var bzaqy = users.FirstOrDefault(x => x.DutyName == "安全员");
            var person = users.Select(x => new SelectListItem() { Value = x.UserId, Text = x.RealName }).ToList();
            //排除未参与的成员
            //获取详情
            var valueStr = string.Format("\"userid\":\"{0}\"", user.UserId);
            var DataStr = string.Format("\"keyvalue\":\"{0}\"", entitys.data.id);
            DataStr = "{" + DataStr + "}";
            valueStr = "{" + valueStr + ",\"data\":" + DataStr + "}";
            var results = webClientEmergency<GetDrillRecordBase>("EmergencyPlatform/GetDrillRecordBase", valueStr);
            var contentlist = new List<SelectListItem>();
            foreach (var item in person)
            {
                if (results.data.drillplanrecordentity.drillpeoplename.Contains(item.Text))
                {
                    contentlist.Add(item);
                }
            }
            ViewBag.person = contentlist;
            ViewBag.context = entitys.data.drillsteplist.OrderBy(x => x.sortid);
            ViewBag.EmergencyReportId = entitys.data.id;
            ViewBag.content = JsonConvert.SerializeObject(entitys);
            return View();


        }
        /// <summary>
        /// 演练第四步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillStep(string entity)
        {
            Session["Emergency"] = entity;
            if (Session["Emergency"] != null)
            {
                entity = Session["Emergency"].ToString();
            }
            //获取步骤数据 并进行修改
            var entitys = JsonConvert.DeserializeObject<EmergencyDataDetailList>(entity);
            ViewBag.context = entitys.data.drillsteplist.OrderBy(x => x.sortid);
            ViewBag.EmergencyReportId = entitys.data.id;
            ViewBag.content = JsonConvert.SerializeObject(entitys);
            return View();
        }
        /// <summary>
        /// 演练第五步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillPoints(string entity)
        {
            Session["Emergency"] = entity;
            if (Session["Emergency"] != null)
            {
                entity = Session["Emergency"].ToString();
            }
            var entitys = JsonConvert.DeserializeObject<EmergencyDataDetailList>(entity);
            ViewBag.content = JsonConvert.SerializeObject(entitys);
            ViewBag.EmergencyReportId = entitys.data.id;
            return View();
        }
        /// <summary>
        /// 演练第六步
        /// </summary>
        /// <returns></returns>
        public ActionResult drillAssess(string entity)
        {
            var user = OperatorProvider.Provider.Current();
            //test
            var entitys = JsonConvert.DeserializeObject<EmergencyDataDetailList>(entity);

            var valueStr = string.Format("\"userid\":\"{0}\"", user.UserId);
            var DataStr = string.Format("\"keyvalue\":\"{0}\"", entitys.data.id);
            DataStr = "{" + DataStr + "}";
            valueStr = "{" + valueStr + ",\"data\":" + DataStr + "}";
            UserBLL userBLL = new UserBLL();
            var results = webClientEmergency<GetDrillRecordBase>("EmergencyPlatform/GetDrillRecordBase", valueStr);
            var users = userBLL.GetDeptUsers(user.DeptId).ToList();
            var person = users.Select(x => new SelectListItem() { Value = x.UserId, Text = x.RealName }).ToList();
            var contentlist = new List<SelectListItem>();
            foreach (var item in person)
            {
                if (results.data.drillplanrecordentity.drillpeoplename.Contains(item.Text))
                {
                    contentlist.Add(item);
                }
            }
            ViewBag.person = contentlist;
            ViewBag.EmergencyReportId = entitys.data.id;
            ViewBag.content = JsonConvert.SerializeObject(entitys);
            return View(results);
        }
        /// <summary>
        /// 提交演练准备数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns> 
        public ActionResult drillReadyGo(string entity)
        {

            try
            {
                var emergencyReport = BSFramework.Util.Json.ToObject<EmergencyReportEntity>(entity);
                var user = OperatorProvider.Provider.Current();
                //test

                var valueStr = string.Format("\"userid\":\"{0}\"", user.UserId);
                var DataStr = string.Format("\"name\":\"{0}\",\"drillmode\":\"{1}\",\"drillmodename\":\"{2}\","
                    + "\"drilltime\":\"{3}\",\"compere\":\"{4}\",\"comperename\":\"{5}\","
                    + "\"drillplace\":\"{6}\",\"drillpeople\":\"{7}\",\"drillpeoplename\":\"{8}\","
                    + "\"drillpeoplenum\":\"{9}\",\"drillplanid\":\"{10}\",\"drillplanname\":\"{11}\","
                    + " \"warntime\":\"{12}\",\"status\":\"0\",\"keyvalue\":\"{13}\"",
                    emergencyReport.emergencyreportname, emergencyReport.rehearsetypeid, emergencyReport.rehearsetype
                    , emergencyReport.planstarttime, emergencyReport.chairpersonid, emergencyReport.chairperson
                    , emergencyReport.emergencyplace, emergencyReport.PersonId, emergencyReport.Persons
                    , emergencyReport.Persons.Split(',').Length, emergencyReport.EmergencyId, emergencyReport.emergencyname,
                    emergencyReport.alerttype, emergencyReport.EmergencyReportId);
                DataStr = "{" + DataStr + "}";
                valueStr = "{" + valueStr + ",\"data\":" + DataStr + "}";
                //dynamic dyresultS = webClientEmergency<BaseResultEmergency>("EmergencyPlatform/SaveDrillRecord",
                //             valueStr);
                NameValueCollection val = new NameValueCollection();
                val.Add("json", valueStr);
                var ApiIp = Config.GetValue("ErchtmsApiUrl");
                var strRsult = string.Empty;
                FileInfoBLL fileInfoBLL = new FileInfoBLL();
                var fileListold = fileInfoBLL.GetFilesByRecIdNew(emergencyReport.EmergencyReportId);
                if (fileListold.Count > 0)
                {
                    strRsult = Util.HttpRequestHelper.HttpUploadFile(ApiIp + "EmergencyPlatform/SaveDrillRecord", Server.MapPath(fileListold[0].FilePath), val);
                    dynamic dyresultS = JsonConvert.DeserializeObject<ExpandoObject>(strRsult);
                    if (dyresultS.Code != 0)
                    {
                        return Success(dyresultS.Info);

                    }
                }
                else
                {
                    return Success("需要上传文件");

                }
                return Success("成功");
            }
            catch (Exception e)
            {

                return Success("失败", new { info = e.Message });
            }

        }

        public JsonResult EmergencySave(string data)
        {

            var success = true;
            var message = string.Empty;
            try
            {
                var user = OperatorProvider.Provider.Current();
                //test
                var entitys = JsonConvert.DeserializeObject<EmergencyDataDetailList>(data);
                var pushData = new SaveDrillAssess();
                pushData.userid = user.UserId;
                var pushDataDetail = new SaveDrillAssessDetail();
                pushDataDetail.keyvalue = entitys.data.id;
                pushDataDetail.drillpurpose = entitys.data.drillpurpose;
                pushDataDetail.scenesimulation = entitys.data.scenesimulation;
                pushDataDetail.drillkeypoint = entitys.data.drillkeypoint;
                pushDataDetail.suitable = entitys.data.suitable;
                pushDataDetail.fullable = entitys.data.fullable;
                pushDataDetail.personstandby = entitys.data.personstandby;
                pushDataDetail.personstandbyduty = entitys.data.personstandbyduty;
                pushDataDetail.sitesupplies = entitys.data.sitesupplies;
                pushDataDetail.sitesuppliesduty = entitys.data.sitesuppliesduty;
                pushDataDetail.wholeorganize = entitys.data.wholeorganize;
                pushDataDetail.dividework = entitys.data.dividework;
                pushDataDetail.effectevaluate = entitys.data.effectevaluate;
                pushDataDetail.reportsuperior = entitys.data.reportsuperior;
                pushDataDetail.rescue = entitys.data.rescue;
                pushDataDetail.evacuate = entitys.data.evacuate;
                pushDataDetail.valuateperson = entitys.data.valuateperson;
                pushDataDetail.valuatepersonname = entitys.data.valuatepersonname;
                pushDataDetail.selfscore = entitys.data.score;
                pushDataDetail.problem = entitys.data.problem;
                pushDataDetail.measure = entitys.data.measure;
                var list = new List<steplist>();
                foreach (var item in entitys.data.drillsteplist)
                {
                    var one = new steplist();
                    one.id = item.stepid;
                    one.sortid = int.Parse(item.sortid);
                    one.dutyperson = item.dutyperson;
                    one.content = item.content;
                    one.dutypersonname = item.dutypersonname;
                    list.Add(one);
                }
                pushDataDetail.steplist = list;
                pushData.data = pushDataDetail;
                var jsonStr = JsonConvert.SerializeObject(pushData);
                var results = webClientEmergency<BaseResultEmergency>("EmergencyPlatform/SaveDrillAssess", jsonStr);
                if (results.Code != 0)
                {
                    success = false; message = results.Info;
                }
                Session.Contents.Remove("Emergency");
            }
            catch (Exception ex)
            {

                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }
            return Json(new { success, message });
        }
        public JsonResult ImportEditPush(string keyvalue)
        {
            var success = true;
            var message = string.Empty;
            try
            {
                FileInfoBLL fileInfoBLL = new FileInfoBLL();
                string newFilePath = "";
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                var file = this.Request.Files[0];
                string fileName = System.IO.Path.GetFileName(file.FileName);
                //if (!fileName.Contains(".pdf"))
                //{
                //    throw new Exception("请检查文件格式！");
                //}
                var fileListold = fileInfoBLL.GetFilesByRecIdNew(keyvalue);
                for (int i = 0; i < fileListold.Count; i++)
                {
                    fileInfoBLL.DeleteFile(fileListold[i].RecId, fileListold[i].FileName, fileListold[i].FilePath);
                }
                string fileGuid = Guid.NewGuid().ToString();
                long filesize = file.ContentLength;
                string FileEextension = Path.GetExtension(fileName);
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                string dir = string.Format("~/Resource/{0}/{1}", "Emergency", uploadDate);
                string newFileName = fileGuid + FileEextension;
                newFilePath = dir + "/" + newFileName;
                if (!Directory.Exists(Server.MapPath(dir)))
                {
                    Directory.CreateDirectory(Server.MapPath(dir));
                }

                FileInfoEntity fileInfoEntity = new FileInfoEntity();
                if (!System.IO.File.Exists(Server.MapPath(newFilePath)))
                {
                    //保存文件
                    file.SaveAs(Server.MapPath(newFilePath));
                    //文件信息写入数据库
                    fileInfoEntity.Create();
                    fileInfoEntity.FolderId = "Emergency";
                    fileInfoEntity.FileId = fileGuid;
                    fileInfoEntity.RecId = keyvalue;
                    fileInfoEntity.FileName = fileName;
                    fileInfoEntity.FilePath = dir + "/" + newFileName;
                    fileInfoEntity.FileSize = (Math.Round(decimal.Parse(filesize.ToString()) / decimal.Parse("1024"), 2)).ToString();//文件大小（kb）
                    fileInfoEntity.FileExtensions = FileEextension;
                    fileInfoEntity.FileType = FileEextension.TrimStart('.');
                    fileInfoBLL.SaveForm("", fileInfoEntity);
                }
                message = fileInfoEntity.FileName;
            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }
            return Json(new { success, message });

        }
        #endregion

        /// <summary>
        /// 应急演练列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public ActionResult emergencyHistoryRecord(int page, int pagesize, string from, string to, FormCollection fc)
        {
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;

            if (string.IsNullOrEmpty(from)) from = fc.Get("StartTime");
            if (string.IsNullOrEmpty(to)) to = fc.Get("EndTime");
            //var name = fc.Get("name");
            //if (name == null)
            //{
            //    name = "";
            //}
            ViewData["from"] = from;
            ViewData["to"] = to;
            //ViewData["name"] = name;
            var user = OperatorProvider.Provider.Current();
            //test
            //获取应急演练列表
            var dyresult = webClientEmergency<GetDrillRecordBaseList>("EmergencyPlatform/GetDrillRecordBaseList",
            "{\"userid\":\"" + user.UserId + "\",\"pageindex\":" + page + ",\"pagesize\":" + pagesize + ",\"data\":{\"startdate\":\"" + from + "\",\"enddate\":\"" + to + "\"} }");
            var total = 0;
            if (dyresult.Code == 0)
            {
                if (dyresult.data.Count > 0)
                {
                    total = dyresult.Count;
                }
            }
            ViewBag.pages = Math.Ceiling((decimal)total / pagesize);
            ViewBag.current = page;
            return View(dyresult.data);
        }
        public ActionResult drillHistoryDetails(string EmergencyReportId)
        {
            if (string.IsNullOrEmpty(EmergencyReportId))
            {
                return View();
            }

            var user = OperatorProvider.Provider.Current();
            //test

            //获取详情
            var valueStr = string.Format("\"userid\":\"{0}\"", user.UserId);
            var DataStr = string.Format("\"keyvalue\":\"{0}\"", EmergencyReportId);
            DataStr = "{" + DataStr + "}";
            valueStr = "{" + valueStr + ",\"data\":" + DataStr + "}";
            var results = webClientEmergency<GetDrillRecordBase>("EmergencyPlatform/GetDrillRecordBase", valueStr);
            ViewBag.content = JsonConvert.SerializeObject(results.data.drillassessentity);
            return View(results.data);
        }



    }
}
