using BSFramework.Application.Busines.ToolManage;
using BSFramework.Application.Entity.ToolManage;
using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSFramework.Util.WebControl;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Application.Busines.AttendManage;
using BSFramework.Application.Entity.AttendManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.PublicInfoManage;
using ThoughtWorks.QRCode.Codec;
using System.Text;
using Aspose.Words;
using Aspose.Words.Drawing;
using BSFramework.Util.Offices;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.BaseManage;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class ToolController : MvcControllerBase
    {
        //
        // GET: /Works/Tool/
        private ToolTypeBLL lbll = new ToolTypeBLL();
        private ToolBorrowBLL tbll = new ToolBorrowBLL();
        private ToolInfoBLL ibll = new ToolInfoBLL();
        private SetAttendBLL sbll = new SetAttendBLL();
        private ToolInventoryBLL tibll = new ToolInventoryBLL();
        private ToolCheckBLL tcbll = new ToolCheckBLL();
        private ToolRepairBLL trbll = new ToolRepairBLL();
        private DepartmentBLL deptbll = new DepartmentBLL();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TBList()
        {
            var user = OperatorProvider.Provider.Current();
            var tree = deptbll.GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = tree.DepartmentId;
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }
        public ActionResult Index3()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.code = dept.EnCode;
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptName = user.DeptName;
            return View();
        }
        public ActionResult Detail3(string id)
        {
            var model = ibll.GetEntity(id);
            var repairlist = trbll.GetList().Where(x => x.ToolId == model.ID).ToList();
            var checklist = tcbll.GetList().Where(x => x.ToolId == model.ID).ToList();
            ViewData["rlist"] = repairlist;
            ViewData["clist"] = checklist;
            return View(model);
        }
        [HttpGet]
        public ActionResult GetPageListJson()
        {
            var user = OperatorProvider.Provider.Current();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var name = this.Request.QueryString.Get("name");
            var total = 0;
            var watch = CommonHelper.TimerStart();
            var data = tibll.GetPageList(user.DeptCode, name, "", page, pagesize, out total);
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize), costtime = CommonHelper.TimerEnd(watch) }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetCheck(string keyValue)
        {
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var total = 0;
            var watch = CommonHelper.TimerStart();
            var data = tcbll.GetList().Where(x => x.ToolId == keyValue);
            foreach (ToolCheckEntity t in data)
            {
                var f = fileBll.GetFilesByRecIdNew(t.ID).FirstOrDefault();
                if (f != null)
                {
                    t.CheckPG = Url.Content(f.FilePath);
                }
            }
            total = data.Count();
            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize), costtime = CommonHelper.TimerEnd(watch) }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetToolInfoPageListJson()
        {
            Operator user = OperatorProvider.Provider.Current();
            if (string.IsNullOrEmpty(user.DeptCode)) user.DeptCode = "0";
            DepartmentBLL deptBll = new DepartmentBLL();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var from = this.Request.QueryString.Get("from");
            var to = this.Request.QueryString.Get("to");
            var result = this.Request.QueryString.Get("result");
            var state = this.Request.QueryString.Get("state");
            var code = this.Request.QueryString.Get("code");
            var total = 0;
            var watch = CommonHelper.TimerStart();
            if (string.IsNullOrEmpty(code)) code = user.DeptCode;
            var groups = deptBll.GetAllGroups().Where(x => x.EnCode.StartsWith(code)).Select(x => x.DepartmentId);
            var data = ibll.GetList("").Where(x => groups.Contains(x.BZID));
            var checklist = new List<ToolCheckEntity>();
            foreach (ToolInfoEntity t in data)
            {
                DateTime start = new DateTime();
                t.CheckCycle = t.CheckCycle == null ? "" : t.CheckCycle;
                //if (t.CheckCycle.Contains("月")) 
                //{
                //    start = t.ValiDate.AddMonths(-(int.Parse(t.CheckCycle.Replace("个月", ""))));
                //}
                //if (t.CheckCycle.Contains("年")) 
                //{
                //    start = t.ValiDate.AddMonths(-(int.Parse(t.CheckCycle.Replace("年", ""))));
                //}

                //checklist = tcbll.GetList().Where(x => x.ToolId == t.ID && x.CheckDate >= start && x.CheckDate < t.ValiDate).ToList();
                var check = tcbll.GetList().Where(x => x.ToolId == t.ID).OrderByDescending(x => x.CheckDate).FirstOrDefault();
                if (DateTime.Now < t.ValiDate && check != null)
                {

                    t.CheckState = "已检验";
                    t.CheckDate = check.CheckDate;
                    t.CheckResult = check.CheckResult;
                    t.CheckPeople = check.CheckPeople;
                }
                else
                {
                    t.CheckState = "未检验";
                    t.CheckDate = null;
                    t.CheckResult = "";
                }
            }
            if (!string.IsNullOrEmpty(from))
            {
                DateTime f = Convert.ToDateTime(from);
                data = data.Where(x => x.CheckDate > f);
            }
            if (!string.IsNullOrEmpty(to))
            {
                DateTime t = Convert.ToDateTime(to).AddDays(1);
                data = data.Where(x => x.CheckDate < t);
            }
            if (!string.IsNullOrEmpty(result))
            {
                data = data.Where(x => x.CheckResult == result);
            }
            if (!string.IsNullOrEmpty(state))
            {
                data = data.Where(x => x.CheckState == state);
            }
            total = data.Count();
            data = data.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize), costtime = CommonHelper.TimerEnd(watch) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Detail2(string id)
        {
            var model = tibll.GetEntity(id);
            return View(model);
        }
        public ActionResult Form2(string id)
        {
            DepartmentBLL deptBll = new DepartmentBLL();
            Operator user = OperatorProvider.Provider.Current();
            var dept = deptBll.GetEntity(user.DeptId);
            if (dept == null)
            {
                user.DeptCode = "0";
            }
            else if (dept.Nature == "部门")
            {
                var pdept = deptBll.GetEntity(dept.ParentId);
                user.DeptCode = pdept == null ? dept.EnCode : pdept.EnCode;
            }
            var model = new ToolInventoryEntity();
            model.ID = Guid.NewGuid().ToString();
            model.CreateDate = DateTime.Now;
            model.DeptId = user.DeptId;
            model.DeptCode = user.DeptCode == null ? "0" : user.DeptCode;
            if (!string.IsNullOrEmpty(id))
            {
                model = tibll.GetEntity(id);
            }
            var entity = new DataItemBLL().GetEntityByName("工器具类型");
            var list = new DataItemDetailBLL().GetList(entity.ItemId);
            ViewData["types"] = list.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });
            return View(model);
        }
        [HttpPost]
        public JsonResult SaveToolInventory(string id, ToolInventoryEntity model)
        {
            FileInfoBLL fibll = new FileInfoBLL();

            tibll.SaveForm(model.ID, model);
            var files = fibll.GetFilesByRecIdNew(model.ID).Where(x => x.Description.Contains("二维码"));
            if (files.Count() == 0)
            {
                fibll.SaveForm(this.BuildImage(model.ID));
            }
            return Json(new { success = true, message = "操作成功" });
        }
        private FileInfoEntity BuildImage(string activityid)
        {
            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode(activityid + "|工器具", Encoding.UTF8);
            var path = "~/Resource/DocumentFile/";
            if (!Directory.Exists(Server.MapPath(path)))
                Directory.CreateDirectory(Server.MapPath(path));

            image.Save(Path.Combine(Server.MapPath(path), id + ".jpg"));

            var user = OperatorProvider.Provider.Current();

            return new FileInfoEntity()
            {
                FileId = id,
                CreateDate = DateTime.Now,
                CreateUserId = user.UserId,
                CreateUserName = user.UserName,
                Description = "工器具二维码",
                FileExtensions = ".jpg",
                FileName = id + ".jpg",
                FilePath = path + id + ".jpg",
                FileType = "jpg",
                ModifyDate = DateTime.Now,
                ModifyUserId = user.UserId,
                ModifyUserName = user.UserName,
                RecId = activityid
            };
        }
        public ActionResult Map()
        {
            ViewData["Round"] = Config.GetValue("Round");
            return View();
        }
        public ActionResult SetPlace(string lng, string lat, string round)
        {

            Config.SetValue("Lng", lng);
            Config.SetValue("Lat", lat);
            Config.SetValue("Round", round);

            SetAttendEntity sae = new SetAttendEntity();
            sae.Lng = lng;
            sae.Lat = lat;
            sae.Area = round;
            sae.CreateDate = DateTime.Now;
            sbll.SaveForm(string.Empty, sae);
            return Success("0");
        }
        /// <summary>
        /// 工器具列表（主页，工器具类型）
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            var user = OperatorProvider.Provider.Current();
            var data = lbll.GetList(user.DeptId);
            return View(data);
        }

        public ActionResult Detail(string id)
        {
            ToolInfoEntity model = ibll.GetEntity(id);
            return View(model);
        }
        public ActionResult ToolEdit(string id)
        {
            ToolInfoEntity model = ibll.GetEntity(id);
            return View(model);
        }

        public ActionResult edit(string id)
        {
            ToolTypeEntity model = lbll.GetEntity(id);
            return Success("修改成功！", new { name = model.Name, path = model.Path });
        }
        public ActionResult GetToolBorrowJson(Pagination pagination, string queryJson)
        {
            string departmentId = "0";
            if (queryJson != null)
            {
                var queryParam = queryJson.ToJObject();
                departmentId = queryParam["departmentId"].ToString();
            }

            Operator user = OperatorProvider.Provider.Current();
            //pagination.p_kid = "ID";
            //pagination.p_fields = "ToolName,ToolSpec,BorrowPerson,BorrowDate,BackDate,IsGood,BZId,Remark";
            //pagination.p_tablename = "wg_toolborrow";
            //pagination.conditionJson = "BZId in ( select departmentid as BZId from base_department where find_in_set(departmentid, fn_recursive('" + departmentId + "')) > 0)";

            var watch = CommonHelper.TimerStart();
            var data = tbll.GetToolBorrowPageList(pagination, queryJson);
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
        /// 工器具列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ToolList(string id)
        {
            var data = ibll.GetList(id);
            ViewData["TypeId"] = id;
            var tooltype = lbll.GetEntity(id);
            ViewData["ToolName"] = tooltype.Name;
            return View(data);
        }
        /// <summary>
        /// 上传图片（新增工器具类型）
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadFile()
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string id = Guid.NewGuid().ToString();
            string FileEextension = Path.GetExtension(files[0].FileName);
            string name = Path.GetFileNameWithoutExtension(files[0].FileName);
            string virtualPath = string.Format("/Content/toolpic/{0}{1}", id, FileEextension);
            string fullFileName = Server.MapPath("~" + virtualPath);

            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);

            return Success("上传成功。", new { path = virtualPath, name = name });
        }

        public ActionResult UploadFileNew(string para)
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return Success("", new { path = "", name = "" });
            }
            string id = Guid.NewGuid().ToString();
            string FileEextension = Path.GetExtension(files[0].FileName);
            string name = Path.GetFileNameWithoutExtension(files[0].FileName);
            string virtualPath = string.Format("/Content/toolfile/{0}", files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);

            return Success("上传成功。", new { path = virtualPath, name = name });
        }
        FileInfoBLL fileBll = new FileInfoBLL();
        public ActionResult UploadFileNew1(string uptype, string id)
        {


            IList<FileInfoEntity> fl = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description == uptype).ToList();
            foreach (FileInfoEntity fe in fl)
            {
                string filepath = fileBll.Delete(fe.FileId);
                if (!string.IsNullOrEmpty(filepath) && System.IO.File.Exists(Server.MapPath("~" + filepath)))
                    System.IO.File.Delete(Server.MapPath("~" + filepath));
            }
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string FileEextension = Path.GetExtension(files[0].FileName);
            string type = files[0].ContentType;
            if (uptype == "0" && !type.Contains("image"))  //图片
            {
                return Success("1");
            }

            if (uptype == "1" && !type.Contains("mp4"))
            {
                return Success("1");
            }
            if (uptype == "2" && !type.Contains("pdf"))
            {
                return Success("1");
            }
            string Id = OperatorProvider.Provider.Current().UserId;
            Id = Guid.NewGuid().ToString();
            string virtualPath = string.Format("~/Content/toolfile/{0}{1}", Id, FileEextension);
            string fullFileName = Server.MapPath(virtualPath);
            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);
            FileInfoEntity fi = new FileInfoEntity
            {
                FileId = Id,
                FolderId = id,
                RecId = id,
                FileName = System.IO.Path.GetFileName(files[0].FileName),
                FilePath = virtualPath,
                FileType = FileEextension.Substring(1, FileEextension.Length - 1),
                FileExtensions = FileEextension,
                FileSize = files[0].ContentLength.ToString(),
                DeleteMark = 0,
                Description = uptype
            };
            fileBll.SaveForm(fi);
            return Success("上传成功。", new { path = virtualPath.TrimStart('~'), name = fi.FileName });
        }

        public ActionResult GlassWareVideo(string id)
        {
            ToolInventoryEntity d = tibll.GetEntity(id);
            ViewData["glasswarename"] = d.Name;
            string path = "";
            FileInfoEntity f = new FileInfoEntity();
            f = fileBll.GetFilesByRecIdNew(id).Where(x => x.Description == "1").FirstOrDefault();
            if (f != null) path = f.FilePath;
            ViewBag.path = path;
            return View();
        }
        public JsonResult DeleteOne(string keyValue)
        {

            tibll.RemoveForm(keyValue);
            return Json(new { success = true, message = "删除成功" });
        }

        public ActionResult expword(string ids)
        {
            string n = "工器具";
            string filename = Server.MapPath("~/Content/export/药品二维码导出.docx");
            var doc = new Aspose.Words.Document(filename);
            FileInfo f = new FileInfo(filename);
            bool b = f.IsReadOnly;
            f.IsReadOnly = false;

            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = new Shape(doc, ShapeType.Image);

            string[] fids = ids.TrimEnd(',').Split(',');
            //根据书签找到单元格并输出图片
            for (int i = 0; i < fids.Length; i++)
            {

                var entity = tibll.GetEntity(fids[i]);
                var data = fileBll.GetFilesByRecIdNew(fids[i]).Where(x => x.Description.Contains("二维码")).FirstOrDefault();
                if (data == null) continue;
                string name = Server.UrlDecode(data.FileName);//返回客户端文件名称
                string filepath = this.Server.MapPath(data.FilePath);
                string marks = "img";

                if (System.IO.File.Exists(filepath))
                {
                    marks = marks + (i + 1);
                    builder.MoveToBookmark(marks);
                    builder.InsertImage(filepath, RelativeHorizontalPosition.Margin, 1, RelativeVerticalPosition.Margin, 1, 90, 90, WrapType.Square);
                    builder.InsertHtml("<label>" + entity.Name + "</label>");
                }

            }
            // shape.ImageData.SetImage(photo)
            var path = Server.MapPath("~/Content/export/");
            path = path.Substring(0, path.LastIndexOf("\\") + 1);

            doc.Save(Path.Combine(path, n + "二维码信息.doc"), Aspose.Words.SaveFormat.Doc);
            ExcelHelper.DownLoadFile(Path.Combine(path, n + "二维码信息.doc"), n + "二维码信息.doc");
            return Success("导出成功。", new { });
        }
        /// <summary>
        /// 新增工器具类型
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult AddToolType(string path, string name)
        {
            var user = OperatorProvider.Provider.Current();
            ToolTypeEntity t = new ToolTypeEntity();
            t.Path = path;
            t.Name = name;
            t.ID = Guid.NewGuid().ToString();
            t.BZId = user.DeptId;
            lbll.SaveForm(string.Empty, t);
            return Success("0");
        }
        public ActionResult EditToolType(string path, string name, string id)
        {
            ToolTypeEntity t = lbll.GetEntity(id);
            t.Path = path;
            t.Name = name;
            lbll.SaveForm(id, t);
            return Success("0");
        }

        /// <summary>
        /// 新增工器具
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult AddTool(string typeid, ToolInfoEntity model)
        {
            var user = OperatorProvider.Provider.Current();
            // model.ID = Guid.NewGuid().ToString();
            model.TypeId = typeid;
            model.OutDate = DateTime.Now;
            model.RegDate = DateTime.Now;
            model.ValiDate = DateTime.Now;
            model.RegPersonName = user.UserName;
            model.RegPersonId = user.UserId;
            return View(model);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Sub(string id, ToolInfoEntity model)
        {
            if (string.IsNullOrEmpty(id))
            {
                model.ID = Guid.NewGuid().ToString();
                model.CurrentNumber = model.Total;
                ibll.SaveForm(string.Empty, model);
            }
            else
            {
                ibll.SaveForm(id, model);
            }
            return RedirectToAction("ToolList", new { id = model.TypeId });
        }

        /// <summary>
        /// 借用
        /// </summary>
        /// <param name="id">工具id</param>
        /// <returns></returns>
        public ActionResult Borrow(string id)
        {
            var user = OperatorProvider.Provider.Current();
            ToolInfoEntity t = ibll.GetEntity(id);
            ToolBorrowEntity b = new ToolBorrowEntity();
            if (!string.IsNullOrEmpty(t.CurrentNumber))
            {
                if (t.CurrentNumber != "0")
                {
                    t.CurrentNumber = (Convert.ToInt32(t.CurrentNumber) - 1).ToString();
                    ibll.SaveForm(t.ID, t);

                    b.ID = Guid.NewGuid().ToString();

                    b.IsGood = "";
                    b.Remark = "";
                    b.ToolName = t.Name;
                    b.ToolSpec = t.Spec;
                    b.BorrwoPerson = user.UserName;
                    b.BorrwoPersonId = user.UserId;
                    b.BorrwoDate = DateTime.Now;
                    b.BackDate = null;
                    b.TypeId = t.ID;
                    b.BZId = user.DeptId;
                    tbll.SaveForm(string.Empty, b);
                    return Success("0", new { typeid = t.TypeId });
                }
                else
                {
                    return Success("1");
                }
            }
            else
            {
                return Success("1");
            }

        }
        public ActionResult Back(string id, string isgood, string state)
        {
            var user = OperatorProvider.Provider.Current();

            ToolBorrowEntity tb = tbll.GetEntity(id);
            ToolInfoEntity t = ibll.GetEntity(tb.TypeId);
            if (!string.IsNullOrEmpty(state))
            {

                //state = state.Substring(0, state.Length - 1);
            }
            t.IsGood = isgood;
            t.State = state;
            if (!string.IsNullOrEmpty(t.CurrentNumber))
            {
                t.CurrentNumber = (Convert.ToInt32(t.CurrentNumber) + 1).ToString();  //更新工具当前数量
            }
            ibll.SaveForm(t.ID, t);
            tb.BackDate = DateTime.Now;
            tb.IsGood = isgood;
            tb.Remark = state;
            tbll.SaveForm(tb.ID, tb);
            return Success("0");
        }
        /// <summary>
        /// 删除工器具类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HandlerMonitor(6, "删除工器具类型")]
        public ActionResult Del(string id)
        {
            try
            {
                var tlist = ibll.GetList(id);
                //if (tlist.Count() > 0)
                //{
                //    return Success("2");
                //}
                //else
                //{
                lbll.RemoveForm(id);
                return Success("0");
                //}
            }
            catch
            {
                return Success("1");
            }
        }
        [HandlerMonitor(6, "删除工器具型号")]
        public ActionResult DelTool(string id)
        {
            try
            {
                ibll.RemoveForm(id);
                return Success("0");
                //}
            }
            catch
            {
                return Success("1");
            }
        }

        [HandlerMonitor(6, "删除工器具借用信息")]
        public ActionResult DeleteB(string keyValue)
        {
            tbll.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 借用记录
        /// </summary>
        /// <returns></returns>
        public ActionResult BorrowList(string f, string t, string n, FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();

            var from = f; var to = t; var name = n;
            if (string.IsNullOrEmpty(f))
                from = fc.Get("from");
            if (string.IsNullOrEmpty(t))
                to = fc.Get("to");
            if (string.IsNullOrEmpty(n))
                name = fc.Get("name");

            var dfrom = string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from);
            var dto = string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to);

            var data = tbll.GetList(user.UserId, user.DeptId, dfrom, dto, name);
            ViewData["TList"] = data;

            ViewData["from"] = from;
            ViewData["to"] = to;
            ViewData["name"] = name;
            ViewData["userid"] = OperatorProvider.Provider.Current().UserId;
            return View();

        }

        #region 返回两个经纬度之间的距离
        private const double EARTH_RADIUS = 6378137;
        /// <summary>
        /// 计算两点位置的距离，返回两点的距离，单位 米
        /// 该公式为GOOGLE提供，误差小于0.2米
        /// </summary>
        /// <param name="lat1">第一点纬度</param>
        /// <param name="lng1">第一点经度</param>
        /// <param name="lat2">第二点纬度</param>
        /// <param name="lng2">第二点经度</param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLng1 = Rad(lng1);
            double radLat2 = Rad(lat2);
            double radLng2 = Rad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
            return result;
        }

        /// <summary>
        /// 经纬度转化成弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return (double)d * Math.PI / 180d;
        }
        #endregion
    }
}
