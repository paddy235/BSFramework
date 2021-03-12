using BSFramework.Application.Busines;
using BSFramework.Application.Busines.AuthorizeManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.BudgetAbout;
using BSFramework.Application.Busines.EvaluateAbout;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity;
using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.SystemManage.ViewModel;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Application.Service.PublicInfoManage;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Cache.Factory;
using BSFramework.Service.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.Attributes;
using BSFramework.Util.Log;
using BSFramework.Util.Offices;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace BSFramework.Application.Web.Controllers
{
    /// <summary>
    /// 描 述：系统首页
    /// </summary>
    [HandlerLogin(LoginMode.Enforce)]
    public class HomeController : Controller
    {
        UserBLL userbll = new UserBLL();
        DepartmentBLL department = new DepartmentBLL();
        AdminPrettyService service = new AdminPrettyService();

        #region 视图功能
        /// <summary>
        /// 后台框架页
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminDefault()
        {
            return View();
        }
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var userEntity = userbll.GetEntity(user.UserId);
            var config = ConfigurationManager.AppSettings["AllowManage"];
            if (config == null) return View();
            if (config == "true" && !(userEntity.RoleName.Contains("班组级用户") || userEntity.RoleName.Contains("班组长") || userEntity.RoleName.Contains("班组成员")))
                return RedirectToAction("AdminPretty");
            else
                return View();
        }
        public ActionResult AdminLTE()
        {
            return View();
        }
        public ActionResult AdminWindos()
        {
            return View();
        }
        public ActionResult AdminPretty()
        {
            return View();
        }


        /// <summary>
        /// 后台框架页
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminBeyond()
        {
            return View();
        }
        /// <summary>
        /// 我的桌面
        /// </summary>
        /// <returns></returns>
        public ActionResult Desktop()
        {
            return View();
        }
        public ActionResult AdminDefaultDesktop()
        {
            return View();
        }

        public ActionResult AdminPrettyDesktopNew()
        {
            var year = DateTime.Now.Year;
            var season = "";
            if (DateTime.Now.Month < 4) { season = "四"; year = year - 1; }
            else if (DateTime.Now.Month < 7) season = "一";
            else if (DateTime.Now.Month < 10) season = "二";
            else season = "三";


            var name = year + "年第" + season + "季度";
            ViewData["Season"] = name;

            var module = new ModuleEntity();
            ModuleBLL moduleBLL = new ModuleBLL();
            var list = moduleBLL.GetList().Where(x => x.UrlAddress != "" && x.UrlAddress != null);
            module = list.Where(x => x.UrlAddress.Contains("/EvaluateAbout/Evaluate/Index2")).FirstOrDefault();//考评
            ViewBag.kpid = module.ModuleId;
            ViewBag.kpurl = module.UrlAddress;
            ViewBag.kptext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains("/Works/AdminPretty/Photos")).FirstOrDefault();//考评
            ViewBag.ygid = module.ModuleId;
            ViewBag.ygurl = module.UrlAddress;
            ViewBag.ygtext = module.FullName;
            return View();
        }

        public ActionResult AdminPrettyDesktopFirst()
        {
            var traingtype = Config.GetValue("TrainingType");
            var CustomerModel = Config.GetValue("CustomerModel");

            if (traingtype == "人身风险预控")
            {
                ViewBag.title1 = "今日未开展";
                ViewBag.title2 = traingtype;
            }
            else
            {
                ViewBag.title1 = "连续两周";
                ViewBag.title2 = "未开展" + CustomerModel;
            }

            var user = OperatorProvider.Provider.Current();
            ViewBag.deptId = user.DeptId;
            ViewBag.deptName = user.DeptName;
            ModuleBLL moduleBLL = new ModuleBLL();
            var module = new ModuleEntity();
            var list = moduleBLL.GetList().Where(x => x.UrlAddress != "" && x.UrlAddress != null);
            module = list.Where(x => x.UrlAddress.Contains("/Works/WorkMeeting/Index3")).FirstOrDefault();//班会
            ViewBag.meetid = module.ModuleId;
            ViewBag.meeturl = module.UrlAddress;
            ViewBag.meettext = module.FullName;
            module = list.Where(x => x.UrlAddress.Contains("/Works/Education/Index4")).FirstOrDefault();//  /Works/Education/Index2
            ViewBag.eduid = module.ModuleId;
            ViewBag.eduurl = module.UrlAddress;
            ViewBag.edutext = module.FullName;
            if (traingtype == "人身风险预控")
            {
                //人身风险预控
                string dangerUrl = "/Works/HumanDanger/Index3?cometype=1";
                ViewBag.kytid = "";
                ViewBag.kyturl = dangerUrl;
                ViewBag.kyttext = "未开展人身风险预控统计";
            }
            else
            {
                //未开展kyt
                module = list.Where(x => x.UrlAddress.Contains("/Works/Danger/Index2")).FirstOrDefault();
                ViewBag.kytid = module.ModuleId;
                ViewBag.kyturl = module.UrlAddress;
                ViewBag.kyttext = module.FullName;
            }





            #region 绑定首页指标
            IndexManageBLL manageBLL = new IndexManageBLL();
            IIndexAssocationBLL assocationBLL = new IIndexAssocationBLL();
            DataSetBLL dataSetBLL = new DataSetBLL();
            DepartmentBLL departmentBLL = new DepartmentBLL();

            //先查询出所有的分类
            var deptid = user.DeptId;
            var cachedata = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);
            if (cachedata == null)
            {
                cachedata = departmentBLL.GetList();
            }
            DepartmentEntity userdept = cachedata.FirstOrDefault(x => x.DepartmentId == deptid);
            if (userdept == null)
            {
                userdept = cachedata.FirstOrDefault(x => x.ParentId == "0");
            }
            var companyDept = department.GetCompany(user.DeptId);
            TerminalDataSetBLL terminalDataSetBLL = new TerminalDataSetBLL();
            IndexManageBLL indexManageBLL = new IndexManageBLL();

            List<TerminalDataSetEntity> entitys = terminalDataSetBLL.GetList().Where(p => p.IsOpen == 1).ToList(); //先取所有的指标
                                                                                                                   //再取所有的分类
            List<IndexManageEntity> manageEntities = indexManageBLL.GetList(companyDept.DepartmentId, (int)IndexType.平台端, null, (int)Templet.第一套).Where(p => p.IsShow == 1).ToList();
            //获取分类底下的指标
            List<IndexAssocationEntity> indexAssocationEntities = new IIndexAssocationBLL().GetListByTitleId(manageEntities.Select(p => p.Id).ToArray());
            //查询并组装指标值的数据
            var dataSetIds = indexAssocationEntities.Select(x => x.DataSetId);//所有的指标的Ids
            Dictionary<string, string> dic = new Dictionary<string, string>();
            entitys.Where(x => dataSetIds.Contains(x.Id)).ToList().ForEach(x =>
            {
                dic.Add(x.Code + "-" + x.Name, "");
            });//应该要绑定的所有的指标

            List<KeyValue> keyValues = new AdminPrettyBLL().FindAllCount(user.UserId, user.DeptId, dic);
            //List<KeyValue> keyValues = FindAllCount(userId, userDeptId, terminalType);
            List<IndexManageModel> models = new List<IndexManageModel>();
            //if (manageEntities != null && manageEntities.Count > 0)
            //{
            //    manageEntities.ForEach(title =>
            //    {
            //        //生成分类
            //        IndexManageModel model = new IndexManageModel()
            //        {
            //            TitleId = title.Id,
            //            TitleName = title.Title,
            //            Srot = title.Sort
            //        };
            //        //生成指标
            //        var datasetIds = indexAssocationEntities.Where(p => p.TitleId == title.Id).Select(p => p.DataSetId);
            //        List<TerminalDataSetEntity> terminals = entitys.Where(x => datasetIds.Contains(x.Id)).ToList();
            //        List<IndexModel> indexModels = new List<IndexModel>();
            //        terminals.ForEach(terminal =>
            //        {
            //            //组装数据

            //            IndexModel indexModel = new IndexModel()
            //            {
            //                Key = terminal.Code,
            //                Name = terminal.Name,
            //                Sort = terminal.Sort,
            //                IsBZ = terminal.IsBZ,
            //                Unit = terminal.Unint,
            //                Icon = string.IsNullOrWhiteSpace(terminal.IconUrl) ? null : Config.GetValue("AppUrl") + terminal.IconUrl
            //                //Value = thisKv == null ? "0" : thisKv.value
            //            };
            //            if (terminal.IsBZ == "1")
            //            {
            //                var thisKv = keyValues.FirstOrDefault(p => p.key == terminal.Code);
            //                indexModel.Value = thisKv == null ? "0" : thisKv.value;
            //            }
            //            indexModels.Add(indexModel);
            //        });
            //        model.AddChilds(indexModels);
            //        models.Add(model);
            //    });
            //}


            //成本预算
            var year = DateTime.Now.Year;
            var deptbll = new DepartmentBLL();
            var teams = deptbll.GetSubDepartments(user.DeptId, "班组");

            var databll = new DataItemDetailBLL();
            var dataitems = databll.GetDataItems("费用类型");

            var budgetbll = new BudgetBLL();
            var budget = budgetbll.GetBudgetSummary(year, teams.Select(x => x.DepartmentId).ToArray());
            var costbll = new CostBLL();
            var cost = costbll.GetCostSummary(year, teams.Select(x => x.DepartmentId).ToArray());
            var result = new List<BudgetSummaryModel>();
            var result1 = new List<BudgetSummaryModel>();
            result.AddRange(dataitems.GroupJoin(budget, x => x.ItemValue, y => y.Category, (x, y) => new BudgetSummaryModel { Category = x.ItemValue + "预算", Data = new decimal[] { y.Sum(z => z.Budget1), y.Sum(z => z.Budget2), y.Sum(z => z.Budget3), y.Sum(z => z.Budget4), y.Sum(z => z.Budget5), y.Sum(z => z.Budget6), y.Sum(z => z.Budget7), y.Sum(z => z.Budget8), y.Sum(z => z.Budget9), y.Sum(z => z.Budget10), y.Sum(z => z.Budget11), y.Sum(z => z.Budget12), } }));
            result1.AddRange(dataitems.GroupJoin(cost, x => x.ItemValue, y => y.Category, (x, y) => new BudgetSummaryModel { Category = x.ItemValue + "支出", Data = new decimal[] { y.Where(z => z.Month == 1).Sum(z => z.Amount), y.Where(z => z.Month == 2).Sum(z => z.Amount), y.Where(z => z.Month == 3).Sum(z => z.Amount), y.Where(z => z.Month == 4).Sum(z => z.Amount), y.Where(z => z.Month == 5).Sum(z => z.Amount), y.Where(z => z.Month == 6).Sum(z => z.Amount), y.Where(z => z.Month == 7).Sum(z => z.Amount), y.Where(z => z.Month == 8).Sum(z => z.Amount), y.Where(z => z.Month == 9).Sum(z => z.Amount), y.Where(z => z.Month == 10).Sum(z => z.Amount), y.Where(z => z.Month == 11).Sum(z => z.Amount), y.Where(z => z.Month == 12).Sum(z => z.Amount), } }));
            decimal ystotal = 0;
            decimal zctotal = 0;
            foreach (BudgetSummaryModel b in result)
            {
                foreach (decimal d in b.Data)
                {
                    ystotal += d;
                }
            }
            foreach (BudgetSummaryModel b in result1)
            {
                foreach (decimal d in b.Data)
                {
                    zctotal += d;
                }
            }
            var k = keyValues.Where(x => x.key == "BZ_LSZC").FirstOrDefault();
            if (k != null)
            {
                if (ystotal == 0)
                {
                    k.value = "0";
                }
                else
                {
                    k.value = Math.Round(zctotal * 100 / ystotal, 2).ToString() + "%";
                }
            }
            var Ids = indexAssocationEntities.Where(x => manageEntities.Any(p => p.Id == x.TitleId)).Select(p => p.DataSetId).ToList();
            var datasetList = entitys.Where(p => Ids.Contains(p.Id)).ToList();
            if (datasetList.Any(p => p.Name == "安全学习日"))
            {
                //module = list.Where(x => x.UrlAddress.Contains("/Works/EdActivity/Index2")).FirstOrDefault();//安全学习
                module = list.Where(x => x.FullName == "安全学习日").FirstOrDefault();//安全学习
                ViewBag.actid = module?.ModuleId;
                ViewBag.acturl = "/Works/EdActivity/Index4";
                ViewBag.acttext = "安全学习日";
                ViewBag.EdTitle = "未开安全学习日班组";
            }
            else
            {
                module = list.Where(x => x.UrlAddress.Contains("/Works/Activity/Index4")).FirstOrDefault();//安全日活动
                ViewBag.actid = module?.ModuleId;
                ViewBag.acturl = module?.UrlAddress;
                ViewBag.acttext = "安全日活动";
                ViewBag.EdTitle = "未开安全日活动班组";
            }


            ViewBag.IndexManageEntitys = manageEntities;
            ViewBag.IndexAssocationEntitys = indexAssocationEntities;
            ViewBag.DataSetEntitys = entitys;
            ViewBag.KeyValues = keyValues;
            #endregion
            ViewBag.deptid = user.DeptId;
            ViewBag.cpname = Config.GetValue("CustomerModel");

            #region 生成客服二维码
            UserEntity userentity = new UserBLL().GetEntity(user.UserId);
            var deptOrg = cachedata.FirstOrDefault(x => x.DepartmentId == userentity.OrganizeId);
            var url = "https://bsh-safety.s4.udesk.cn/im_client/?web_plugin_id=8715&agent_id=8711&group_id=8385";
            url += $"&c_name={userentity.RealName}";//客户姓名
            url += $"&c_phone={userentity.Mobile}";//电话号码
            url += $"&c_org={ deptOrg?.FullName}";//公司名称
            int rad = new Random().Next(1, 100);
            url += $"&nonce={rad}";//随机数
                                   //TimeSpan timeSpan = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);

            var timer = (DateTime.Now.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000;
            url += $"&timestamp={timer}";//时间戳
            url += $"&web_token={userentity.DepartmentName}";//客户Id

            var sign_str = $"nonce={rad}&timestamp={timer}&web_token={userentity.UserId}&{new DataItemDetailBLL().GetListByName("IMKey").FirstOrDefault()?.ItemValue}";

            byte[] encodeStr = Encoding.UTF8.GetBytes(sign_str);
            byte[] encryStr = SHA1.Create().ComputeHash(encodeStr);

            StringBuilder sb = new StringBuilder();
            foreach (var item in encryStr)
            {
                sb.Append(item.ToString("X2"));
            }



            //var signature = string.Join("", encryStr.Select(x => string.Format("{X2}", x)).ToArray()).ToUpper();
            url += $"&signature={sb.ToString()}";


            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeVersion = 0;
            qrCodeEncoder.QRCodeScale = 2;
            qrCodeEncoder.QRCodeForegroundColor = Color.Black;
            Bitmap bmp = qrCodeEncoder.Encode(url, Encoding.UTF8);//指定utf-8编码， 支持中文
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bs = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(bs, 0, (int)ms.Length);
            ms.Close();
            ms.Dispose();
            bmp.Dispose();
            ViewBag.QRCode = "data:images/jpeg;base64," + Convert.ToBase64String(bs);
            #endregion

            return View();
        }

        /// <summary>
        /// 获取班组文化墙的地址
        /// </summary>
        /// <returns></returns>
        public void GetCultureUrl()
        {
            string cultureurl = "";
            try
            {
                var deptId = OperatorProvider.Provider.Current().DeptId;
                //WebClient wc = new WebClient();
                //wc.Credentials = CredentialCache.DefaultCredentials;
                //System.Collections.Specialized.NameValueCollection nc = new System.Collections.Specialized.NameValueCollection();
                string url = Config.GetValue("ErchtmsApiUrl") + "/MenuConfig/GetCultureUrl";
                if (Debugger.IsAttached)
                {
                    url = "http://localhost/ERCHTMSAPP/API/MenuConfig/GetCultureUrl";
                }
                var result = JsonConvert.DeserializeObject<string>(HttpMethods.HttpGet(url + "?deptId=" + deptId));
                //nc.Add("deptId", deptId);
                //var getData = wc.UploadValues(new Uri(url), nc);
                //var result = System.Text.Encoding.UTF8.GetString(getData);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    cultureurl = $"{result}?userId={OperatorProvider.Provider.Current().UserId}&bzId={deptId}&homePage={HttpUtility.UrlEncode("http://" + Request.Url.Host + Url.Action("index", "home"))}";
                    Response.Redirect(cultureurl);
                }
                else
                {
                    Response.Redirect(Url.Action("index", "home"));
                }
            }
            catch (Exception ex)
            {
                Response.Redirect(Url.Action("index", "home"));
            }

        }
        /// <summary>
        /// 主界面右侧指标数据统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetSSJKNumbers()
        {
            var departmentBLL = new DepartmentBLL();
            var user = OperatorProvider.Provider.Current();
            var curdept = departmentBLL.GetEntity(user.DeptId);
            if (curdept != null)  //System curdept = null
            {
                if (curdept.IsSpecial)
                {
                    curdept = departmentBLL.GetEntity(curdept.ParentId);
                    user.DeptCode = curdept.EnCode;
                    user.DeptId = curdept.DepartmentId;
                }
            }

            var meet = service.GetMonthUndoGroups(user.DeptCode, "1");//未开班会
            var act = service.GetMonthUndoGroups(user.DeptCode, "2");//安全日活动
            var aqxxr = service.GetMonthUndoGroups(user.DeptCode, "7");//安全学习日
            var kyt = service.GetMonthUndoGroups(user.DeptCode, "3");//未开展KYT 危险预知训练
            var appr = service.GetMonthUndoGroups(user.DeptCode, "5");//未评价台账
            var rsfxyk = service.GetMonthUndoGroups(user.DeptId, "8");//人身风险预控
            var data = new
            {
                userid = OperatorProvider.Provider.Current().UserId
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Home", "getBZPlatformIndexInfo"), "json=" + JsonConvert.SerializeObject(data));
            var ret = JsonConvert.DeserializeObject<ResponesModel>(res);
            List<KeyValue> skData = new List<KeyValue>();
            var yh = "0";
            var wz = "0";
            if (ret != null && ret.data != null)
            {
                skData = JsonConvert.DeserializeObject<List<KeyValue>>(ret.data.ToString());
                KeyValue k1 = skData.Where(x => x.key == "SK_WZGYH").FirstOrDefault();
                KeyValue k2 = skData.Where(x => x.key == "SK_WZGWZ").FirstOrDefault();
                yh = k1 == null ? "0" : k1.value;
                wz = k2 == null ? "0" : k2.value;
            }

            return Json(new { meet = meet, act = act, kyt = kyt, appr = appr, yh = yh, wz = wz, aqxxr = aqxxr, rsfxyk = rsfxyk });
        }
        /// <summary>
        /// 右屏通知公告
        /// </summary>
        /// <returns></returns>
        public JsonResult getNewSData()
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
                res = "{  \"code\": 0,  \"info\": \"获取数据成功\",  \"count\": 1,  \"data\": [    {      \"id\": \"aed65456-2bdd-4ddc-b659-4b0699151e57\",      \"createuserid\": \"6f67b1d3-a2e2-45d0-9245-c3232528c95f\",      \"title\": \"20181203通知公告1\",      \"publisher\": \"安全部负责人\",      \"publisherdept\": \"安环部\",      \"releasetime\": \"2018-12-03 18:31\",      \"isimportant\": \"是\",      \"issend\": \"0\",      \"isread\": \"0\",      \"userid\": \"350c9862-5978-42ae-a779-26e39c67baab,5b7bf18e-0fe8-ab6c-05e1-812b1defec7a,edf09c5f-fbd9-8c4a-d65e-262544ac812a,96bae42b-b0ac-4227-8f48-dfe63ca469e9,aa2f6f58-c3d4-4756-bf03-ee485ec73f8f,d0ac0860-73ca-4673-9206-d5560902fc37,5dca8859-0526-41d1-987e-dada4cf676de,0138dbb0-743d-42fe-85b9-c346a3934565,03f625f9-6b7a-4c0d-8433-26394d0947dc,b7017c89-05f3-4a48-951d-f6f020d87f8e,b63ca02a-375f-4882-bbdf-bf3405a4c048,3c76bfc3-95ed-460b-ad81-89dbf31e9bde,1f6fa724-fba1-41cf-96c7-7bf769fddedc,24f6a3a4-ef85-49d3-b43a-8bc5a5c86631,1f22bcaf-e886-45fb-8733-8e85bc48b247,6aee035f-f586-47c8-b009-27f8228b66f9,0f0f3ad7-acec-41be-85e6-c355bdde9ba8,fcf99146-5ea1-44e7-a9e7-4a983d12774b,9c47f413-b5ab-4ca4-b563-4b58b5cf7b2e,86e31d96-0071-46c5-a718-4d9156bcbfaa,57209043-90c8-48ac-b5b3-7d807937f1a7,bbbdd3ca-f50c-4f69-8377-e6f9fa84cb68,8054f57c-79e5-4775-869d-878dc35b9c8a,32c13338-7ffb-4e15-a635-5a862ec33e99,54f2bb4c-59a2-4b2e-8a0e-60648bd9049d,8aaa854f-aa95-40f7-8976-d298cd4d2ef6,be5a108b-21fb-4ee8-b699-e0da626a6e6f,aef7bb01-af7d-4dad-943f-37b960fbb67c,1852789a-d2e0-48a2-be63-9b951d037c23,228db740-a594-44aa-8ade-77528a9e43d4,1bacb33c-6a3a-40d1-85c1-3b1ae04f3b7d,59f82836-e62d-492f-b373-141756a5e240,6f67b1d3-a2e2-45d0-9245-c3232528c95f,54c5888b-a6b1-4c42-a56a-99def312aa1f,9009b613-04dd-4fa4-8bd0-eab6bdca2774,f44046ab-a373-4508-80df-a2885bffd76e,9945d5fc-35b7-44cc-b71c-6ac7e0edd1d1,591d965e-4f7c-4218-b298-18650eeb03ee,a44d2ce5-c0c0-492e-bce4-6f83782880dc,edbc02b5-e21a-4541-a197-5af1398517be,780f5cf8-51e7-4f6f-84d5-8d84374c18af,11a16aee-233a-4982-88f7-85c33fe2bd57,c13f0d21-cb9b-468e-a028-ea2a674d5b45,f189e14f-61f6-429f-aac0-c78770ac890f,7fa1c4cf-732d-436f-8bd5-a5a4f602b954,1b277532-1f61-41b7-8a44-9f34c49b021b,18ae0ea1-be29-4da8-81d9-0a4e1bc67a68,85cefe9c-7bc1-49d2-a37c-70e946af2f9a,c22e213b-51f9-4301-827e-4fe41240a6c0,c1b69f0e-09b0-47cd-95b9-93e5f71514ce,dd0b76b9-9093-440f-ab8a-189afdcadc80,9a123283-c07a-4aca-bca6-ef5e9ee9ba9e,ff1de75b-d8fb-415b-9e73-a6fa2a3b5f75,1fcdad11-697e-4736-be79-f7ee2d7da947,3048ed6b-63e7-43c4-82b2-5e03518a4c4c,eb72f907-afd5-47d5-8ba7-0944c9c4009a,85658302-91a2-4c8c-8770-6d89fd933b96,f1f2090a-b326-4d59-8e3d-4bee7c0be42cc01de65b-d3d2-48a8-8442-42c6035ac17f,\",      \"content\": \"测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知测试通知\",      \"r\": 1    },	 {      \"id\": \"aed65456-2bdd-4ddc-b659-4b0699151e57\",      \"createuserid\": \"6f67b1d3-a2e2-45d0-9245-c3232528c95f\",      \"title\": \"20181203通知公告1\",      \"publisher\": \"安全部负责人\",      \"publisherdept\": \"安环部\",      \"releasetime\": \"2018-12-03 18:31\",      \"isimportant\": \"是\",      \"issend\": \"0\",      \"isread\": \"0\",      \"userid\": \"350c9862-5978-42ae-a779-26e39c67baab,5b7bf18e-0fe8-ab6c-05e1-812b1defec7a,edf09c5f-fbd9-8c4a-d65e-262544ac812a,96bae42b-b0ac-4227-8f48-dfe63ca469e9,aa2f6f58-c3d4-4756-bf03-ee485ec73f8f,d0ac0860-73ca-4673-9206-d5560902fc37,5dca8859-0526-41d1-987e-dada4cf676de,0138dbb0-743d-42fe-85b9-c346a3934565,03f625f9-6b7a-4c0d-8433-26394d0947dc,b7017c89-05f3-4a48-951d-f6f020d87f8e,b63ca02a-375f-4882-bbdf-bf3405a4c048,3c76bfc3-95ed-460b-ad81-89dbf31e9bde,1f6fa724-fba1-41cf-96c7-7bf769fddedc,24f6a3a4-ef85-49d3-b43a-8bc5a5c86631,1f22bcaf-e886-45fb-8733-8e85bc48b247,6aee035f-f586-47c8-b009-27f8228b66f9,0f0f3ad7-acec-41be-85e6-c355bdde9ba8,fcf99146-5ea1-44e7-a9e7-4a983d12774b,9c47f413-b5ab-4ca4-b563-4b58b5cf7b2e,86e31d96-0071-46c5-a718-4d9156bcbfaa,57209043-90c8-48ac-b5b3-7d807937f1a7,bbbdd3ca-f50c-4f69-8377-e6f9fa84cb68,8054f57c-79e5-4775-869d-878dc35b9c8a,32c13338-7ffb-4e15-a635-5a862ec33e99,54f2bb4c-59a2-4b2e-8a0e-60648bd9049d,8aaa854f-aa95-40f7-8976-d298cd4d2ef6,be5a108b-21fb-4ee8-b699-e0da626a6e6f,aef7bb01-af7d-4dad-943f-37b960fbb67c,1852789a-d2e0-48a2-be63-9b951d037c23,228db740-a594-44aa-8ade-77528a9e43d4,1bacb33c-6a3a-40d1-85c1-3b1ae04f3b7d,59f82836-e62d-492f-b373-141756a5e240,6f67b1d3-a2e2-45d0-9245-c3232528c95f,54c5888b-a6b1-4c42-a56a-99def312aa1f,9009b613-04dd-4fa4-8bd0-eab6bdca2774,f44046ab-a373-4508-80df-a2885bffd76e,9945d5fc-35b7-44cc-b71c-6ac7e0edd1d1,591d965e-4f7c-4218-b298-18650eeb03ee,a44d2ce5-c0c0-492e-bce4-6f83782880dc,edbc02b5-e21a-4541-a197-5af1398517be,780f5cf8-51e7-4f6f-84d5-8d84374c18af,11a16aee-233a-4982-88f7-85c33fe2bd57,c13f0d21-cb9b-468e-a028-ea2a674d5b45,f189e14f-61f6-429f-aac0-c78770ac890f,7fa1c4cf-732d-436f-8bd5-a5a4f602b954,1b277532-1f61-41b7-8a44-9f34c49b021b,18ae0ea1-be29-4da8-81d9-0a4e1bc67a68,85cefe9c-7bc1-49d2-a37c-70e946af2f9a,c22e213b-51f9-4301-827e-4fe41240a6c0,c1b69f0e-09b0-47cd-95b9-93e5f71514ce,dd0b76b9-9093-440f-ab8a-189afdcadc80,9a123283-c07a-4aca-bca6-ef5e9ee9ba9e,ff1de75b-d8fb-415b-9e73-a6fa2a3b5f75,1fcdad11-697e-4736-be79-f7ee2d7da947,3048ed6b-63e7-43c4-82b2-5e03518a4c4c,eb72f907-afd5-47d5-8ba7-0944c9c4009a,85658302-91a2-4c8c-8770-6d89fd933b96,f1f2090a-b326-4d59-8e3d-4bee7c0be42cc01de65b-d3d2-48a8-8442-42c6035ac17f,\",      \"content\": \"测试通知\",      \"r\": 1    }  ]}";
            }
            else
            {
                res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "RoutineSafetyWork", "GetAnnouncementList"), "json=" + JsonConvert.SerializeObject(dict));
            }
            var ret = JsonConvert.DeserializeObject<RetDataModel>(res);
            List<NnoticeModel> list = JsonConvert.DeserializeObject<List<NnoticeModel>>(ret.data.ToString());
            var data = list.Select(x => new
            {
                x.Content,
                x.Publisher,
                x.PublisherDept,
                ReleaseTime = x.ReleaseTime.HasValue ? x.ReleaseTime.Value.ToString("yyyy-MM-dd") : "",
                x.Title,
                x.Id
            });
            return Json(data);
        }
        /// <summary>
        /// 右屏考评班组
        /// </summary>
        /// <returns></returns>
        public JsonResult getKPData()
        {
            var user = OperatorProvider.Provider.Current();
            DesignationBLL dbll = new DesignationBLL();
            var chlist = dbll.GetList(string.Empty).OrderBy(x => x.SortCode).Take(3).ToList();
            var list = service.GetKPGroups();
            var bbbz = list;
            var xjbz = list;
            var dbbz = list;

            if (chlist.Count > 0)
            {
                bbbz = list.Where(x => x.TitleName == chlist[0].ClassName).Skip(0).Take(6).ToList();
            }
            if (chlist.Count > 1)
            {
                xjbz = list.Where(x => x.TitleName == chlist[1].ClassName).Skip(0).Take(6).ToList();
            }
            if (chlist.Count > 2)
            {
                dbbz = list.Where(x => x.TitleName == chlist[2].ClassName).Skip(0).Take(100).ToList();
            }
            return Json(new { ch = chlist, bbbz = bbbz, xjbz = xjbz, dbbz = dbbz });
        }

        public JsonResult getYHWZ(string mode)
        {
            var user = OperatorProvider.Provider.Current();
            var dict = new
            {
                userid = user.UserId,
                data = new
                {
                    mode = mode
                }
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Home", "getBZPlatformStatisticsInfo"), "json=" + JsonConvert.SerializeObject(dict));
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            return Json(dy.data);
        }
        public ActionResult AdminLTEDesktop()
        {
            return View();
        }
        public ActionResult AdminWindosDesktop()
        {
            return View();
        }
        public ActionResult AdminPrettyDesktop()
        {
            return View();
        }

        public ActionResult SkinIndex()
        {
            return View();
        }

        public ActionResult Own()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.Department = user.DeptName;
            return View();
        }
        /// <summary>
        /// 通知公告详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult NoticeDetail()
        {
            return View();
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 访问功能
        /// </summary>
        /// <param name="moduleId">功能Id</param>
        /// <param name="moduleName">功能模块</param>
        /// <param name="moduleUrl">访问路径</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VisitModule(string moduleId, string moduleName, string moduleUrl)
        {
            LogEntity logEntity = new LogEntity();
            logEntity.CategoryId = 2;
            logEntity.OperateTypeId = ((int)OperationType.Visit).ToString();
            logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Visit);
            logEntity.OperateAccount = OperatorProvider.Provider.Current().Account;
            logEntity.OperateUserId = OperatorProvider.Provider.Current().UserId;
            logEntity.ModuleId = moduleId;
            logEntity.Module = moduleName;
            logEntity.ExecuteResult = 1;
            logEntity.ExecuteResultJson = "访问地址：" + moduleUrl;
            logEntity.WriteLog();
            return Content(moduleId);
        }
        /// <summary>
        /// 离开功能
        /// </summary>
        /// <param name="moduleId">功能模块Id</param>
        /// <returns></returns>
        public ActionResult LeaveModule(string moduleId)
        {
            return null;
        }
        /// <summary>
        /// 根据菜单名称，获取菜单的信息
        /// </summary>
        /// <param name="moduleName">菜单名称</param>
        /// <returns></returns>
        public ActionResult GetModuleInfo(string moduleName)
        {
            try
            {
                string res = HttpMethods.HttpGet(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "MenuConfig", "GetModuleInfoByName") + "?" + string.Format("moduleIdName={0}", moduleName));
                MenuRspKp<ModuleEntity> models = JsonConvert.DeserializeObject<MenuRspKp<ModuleEntity>>(res);
                return Json(models);
            }
            catch (Exception ex)
            {
                return Json(new MenuRsp<ModuleEntity>()
                {
                    Code = -1,
                    Info = ex.Message
                });
            }

        }

        /// <summary>
        /// 获取双控项目配置的windows终端菜单
        /// </summary>
        public ActionResult GetMenu()
        {
            try
            {
                //正式
                string Value = "{ \"userid\":\"" + OperatorProvider.Provider.Current().UserId + "\", \"data\":{ \"themetype\":0,\"PaltformType\":0} }";
                //测试
                //string Value = "{ userid:\"9098221b-6bda-41bd-84d1-79c31877f6fb\", data:{ themetype:0,PaltformType:0} }";
                string url = Path.Combine(Config.GetValue("ErchtmsApiUrl"), "MenuConfig", "GetMenuConfigList");
                var para = "json=" + Url.Encode(Value);
                string responseStr = HttpMethods.HttpPost(url, para);
                MenuRsp<MenuModel> models = JsonConvert.DeserializeObject<MenuRsp<MenuModel>>(responseStr);
                return Json(models);
            }
            catch (Exception ex)
            {
                return Json(new MenuRsp<MenuModel>()
                {
                    Code = -1,
                    Info = ex.Message
                });
            }

        }

        [HttpGet]
        public ViewResult Doshboard()
        {
            var skUrl = Config.GetValue("ErchtmsApiUrl");
            var client = new HttpClient();
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var json = client.PostAsync($"{skUrl}DataList/GetValue?name=bzWebUrl", content).Result.Content.ReadAsStringAsync().Result;
            var item = Newtonsoft.Json.JsonConvert.DeserializeObject<DataItemDetailEntity>(json);
            ViewBag.bzUrl = item.ItemValue;

            return View();
        }

        public ViewResult DoshboardSetting()
        {
            return View();
        }


        public JsonResult GetDoshboardSetting()
        {
            var user = OperatorProvider.Provider.Current();
            PersonDoshboardBLL doshboardBLL = new PersonDoshboardBLL();

            var skUrl = Config.GetValue("ErchtmsApiUrl");
            var client = new HttpClient();
            var content = new StringContent($"{{\"Data\":\"首页指标\"}}");
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var json = client.PostAsync($"{skUrl}DataList/List", content).Result.Content.ReadAsStringAsync().Result;
            var elements = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataItemDetailEntity>>(json);
            elements = elements.OrderBy(x => x.SortCode).ToList();
            var data = doshboardBLL.GetList(user.UserId);
            //var ss = elements.OrderBy(x => x.SortCode).GroupJoin(data, x => x.ItemDetailId, y => y.SettingId, (x, y) => new { x, y }).ToList();
            data = elements.GroupJoin(data, x => x.ItemDetailId, y => y.SettingId, (x, y) => new PersonDoshboardEntity
            {
                Name = x.ItemName,
                SettingId = x.ItemDetailId,
                Enabled = y.FirstOrDefault() == null ? true : y.First().Enabled,
                PersonDoshboardId = y.FirstOrDefault()?.PersonDoshboardId,
                UserId = y.FirstOrDefault() == null ? user.UserId : y.First().UserId,
                Seq = y.FirstOrDefault() == null ? x.SortCode.Value : y.First().Seq,
                Url = y.FirstOrDefault() == null ? x.ItemValue : y.First().Url
            }).OrderBy(x => x.Seq).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult PostSettings(List<PersonDoshboardEntity> settings)
        {
            var currentUser = OperatorProvider.Provider.Current();
            foreach (var item in settings)
            {
                if (string.IsNullOrEmpty(item.PersonDoshboardId))
                    item.PersonDoshboardId = Guid.NewGuid().ToString();

                item.UserId = currentUser.UserId;
            }

            PersonDoshboardBLL doshboardBLL = new PersonDoshboardBLL();
            doshboardBLL.Save(settings);

            return Json(new AjaxResult() { type = ResultType.success, resultdata = null, message = "保存成功！" });
        }


        public JsonResult GetDoshboard()
        {
            var currentUser = OperatorProvider.Provider.Current();
            PersonDoshboardBLL doshboardBLL = new PersonDoshboardBLL();

            var doshboard = doshboardBLL.GetEnabledList(currentUser.UserId);
            if (doshboard == null || doshboard.Count == 0)
            {
                var skUrl = Config.GetValue("ErchtmsApiUrl");
                var client = new HttpClient();
                var content = new StringContent($"{{\"Data\":\"首页指标\"}}");
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var json = client.PostAsync($"{skUrl}DataList/List", content).Result.Content.ReadAsStringAsync().Result;
                var elements = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataItemDetailEntity>>(json);
                doshboard = elements.OrderBy(x => x.SortCode).Select(x => new PersonDoshboardEntity { Url = x.ItemValue, SettingId = x.ItemDetailId }).ToList();
            }

            return Json(doshboard);
        }

        #endregion
    }
}
