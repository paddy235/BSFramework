using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.LllegalManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;
using System.Data;
using BSFramework.Util.Offices;
using ThoughtWorks.QRCode.Codec.Util;
using NPOI.XWPF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Drawing;
using NPOI.SS.UserModel;
using Aspose.Words;
using Aspose.Words.Saving;
using Aspose.Words.Drawing;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Util;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BSFramework.Application.Web.Areas.Works.Models;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class LllegalController : MvcControllerBase
    {
        private LllegalBLL bll = new LllegalBLL();
        private FileInfoBLL fileInfoBLL = new FileInfoBLL();
        UserBLL userbll = new UserBLL();
        /// <summary>
        /// 日历
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HandlerMonitor(5, "违章管理-按月统计当天登记的违章并且当天未验收的违章数量和当天登记的违章的总数量")]
        public ActionResult Index(FormCollection fc)
        {
            var year = int.Parse(fc.Get("year") ?? DateTime.Now.Year.ToString());
            var month = int.Parse(fc.Get("month") ?? DateTime.Now.Month.ToString());
            var date = new DateTime(year, month, 1);
            date = date.AddDays(-date.Day);
            date = date.AddDays(-(int)date.DayOfWeek);

            var years = new List<dynamic>();
            for (int i = 0; i < 10; i++)
            {
                years.Add(new { value = (DateTime.Now.Year - i).ToString(), text = (DateTime.Now.Year - i).ToString() });
            }
            var months = new List<dynamic>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(new { value = i.ToString(), text = (i + "月") });
            }
            #region
            ViewData["HaveOrder"] = "1";
            #endregion
            ViewData["date"] = date;
            ViewData["currentmonth"] = month;
            ViewData["currentyear"] = year;
            ViewData["year"] = new SelectList(years, "value", "text", year);
            ViewData["month"] = new SelectList(months, "value", "text", month);

            return View(GetLllegalRegisterNumByMonth(year, month));
        }

        /// <summary>
        /// 核准
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Approve(string id)
        {
            //ViewBag.Show = "y"; //提交之后弹窗提示
            //model.ApproveResult = Result;
            //bll.SaveForm(id, model);
            LllegalEntity model = bll.GetLllegalDetail(id);
            model.ApprovePerson = OperatorProvider.Provider.Current().UserName;
            model.ApprovePersonId = OperatorProvider.Provider.Current().UserId;

            model.ApproveDate = DateTime.Now;

            var users = userbll.GetDeptUsers(OperatorProvider.Provider.Current().DeptId).ToList();

            ViewData["users"] = users;
            return View(model);

        }
        public ActionResult sub(string id, string Result, LllegalEntity model)
        {


            model.ApproveResult = Result;

            if (model.ApproveResult != "1")
            {
                model.FlowState = "待整改";
            }
            else
            {
                model.FlowState = "核准不通过";
            }

            //所属部门修改为 整改人所在部门  2018-08-24
            model.LllegalTeamId = userbll.GetEntity(model.ReformPeopleId).DepartmentId;

            bll.SaveForm(id, model);
            model = bll.GetLllegalDetail(id);
            var user = OperatorProvider.Provider.Current();
            string lid = bll.GetListMonthLllegal(user.DeptId, user.UserId);
            if (lid != "")
            {
                ViewBag.Show = "y"; //提交之后弹窗提示
            }
            else
            {
                ViewBag.Show = "n"; //提交之后弹窗提示
            }
            return View(model);
        }
        public ActionResult Next()
        {
            var user = OperatorProvider.Provider.Current();
            string id = bll.GetListMonthLllegal(user.DeptId, user.UserId);
            return Success("修改成功", new { next = id });
        }

        public ActionResult Export(FormCollection fc)
        {
            var user = OperatorProvider.Provider.Current();
            //取出数据源
            var from = fc.Get("from");
            var to = fc.Get("to");
            var filtertype = fc.Get("filtertype") ?? "";
            var llevel = fc.Get("filtervalue1") ?? "";
            var ltype = fc.Get("filtervalue2") ?? "";
            var lperson = fc.Get("filtervalue3") ?? "";

            var total = 0;
            //var data = new LllegalBLL().GetList(user.DeptId, filtertype == "全部" ? string.Empty : filtertype, filtervalue, string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from), string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to), page, pagesize, out total);
            var data = this.GetLllegalList(1000000, 1, ltype, llevel, from, to, out total);
            DataTable exportTable = new DataTable();
            exportTable.Columns.Add("lllegalnumber");
            exportTable.Columns.Add("flowstate");
            exportTable.Columns.Add("lllegalperson");
            exportTable.Columns.Add("lllegallevelname");
            exportTable.Columns.Add("lllegaltypename");
            exportTable.Columns.Add("lllegaltime");
            exportTable.Columns.Add("reformfinishdate");
            foreach (var item in data)
            {
                DataRow dr = exportTable.NewRow();
                dr["lllegalnumber"] = item.lllegalnumber;
                dr["flowstate"] = item.flowstate;
                dr["lllegalperson"] = item.lllegalperson;
                dr["lllegallevelname"] = item.lllegallevelname;
                dr["lllegaltypename"] = item.lllegaltypename;
                dr["lllegaltime"] = item.lllegaltime;
                dr["reformfinishdate"] = item.reformfinishdate;

                exportTable.Rows.Add(dr);
            }
            //DataTable exportTable = bll.getExport(user.DeptId, "", "");
            //设置导出格式
            ExcelConfig excelconfig = new ExcelConfig();
            //excelconfig.Title = "违章信息";
            //excelconfig.TitleFont = "微软雅黑";
            //excelconfig.TitlePoint = 25;
            excelconfig.HeadHeight = 50;
            excelconfig.HeadPoint = 12;
            excelconfig.HeadFont = "宋体";
            excelconfig.FileName = "违章信息导出.xls";
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
            excelconfig.ColumnEntity = listColumnEntity;
            ColumnEntity columnentity = new ColumnEntity();
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "lllegalnumber", ExcelColumn = "违章编号", Width = 12, Alignment = "fill" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "flowstate", ExcelColumn = "整改状态", Width = 15 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "lllegalperson", ExcelColumn = "违章人员", Width = 15 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "lllegallevelname", ExcelColumn = "违章等级", Width = 15 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "lllegaltypename", ExcelColumn = "违章类型", Width = 15 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "lllegaltime", ExcelColumn = "违章时间", Width = 15 });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "reformfinishdate", ExcelColumn = "整改完成时间", Width = 15 });
            //调用导出方法
            ExcelHelper.ExcelDownload(exportTable, excelconfig);
            //从泛型Lis导出
            //TExcelHelper<DepartmentEntity>.ExcelDownload(department.GetList().ToList(), excelconfig);
            return View();


        }
        public ActionResult ASPExWord(string id)
        {

            var model = this.GetLllegalDetail(id);
            if (model == null) throw new Exception("该记录已被删除！");
            string filename = Server.MapPath("../../../Content/export/违章详情导出.docx");
            var dic = new Dictionary<string, object>();
            dic.Add("LllegalNumber", model.lllegalnumber);
            dic.Add("LllegalDepart", model.lllegaldepart);
            dic.Add("LllegalType", model.lllegaltypename == null ? "" : model.lllegaltypename);
            dic.Add("LllegalTeam", model.lllegalteam == null ? "" : model.lllegalteam);
            dic.Add("LllegalLevel", model.lllegallevelname);
            dic.Add("LllegalAddress", model.lllegaladdress == null ? "" : model.lllegaladdress);
            dic.Add("RegisterPerson", model.createusername);
            dic.Add("LllegalTime", model.lllegaltime == null ? "" : model.lllegaltime.Value.ToString("yyyy-MM-dd"));
            dic.Add("LllegalPerson", model.lllegalperson);
            dic.Add("LllegalDescribe", model.lllegaldescribe);
            if (model.appentity != null)
            {
                dic.Add("ApprovePerson", model.appentity.approveperson);
                dic.Add("ApproveDate", model.appentity.approvedate == null ? "" : model.appentity.approvedate.Value.ToString("yyyy-MM-dd"));
            }
            else
            {
                dic.Add("ApprovePerson", "");
                dic.Add("ApproveDate", "");
            }
            var doc = new Aspose.Words.Document(filename);
            FileInfo f = new FileInfo(filename);
            bool b = f.IsReadOnly;
            f.IsReadOnly = false;
            doc.MailMerge.Execute(dic.Keys.ToArray(), dic.Values.ToArray());

            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = new Shape(doc, ShapeType.Image);
            string marks = "PHOTO";
            //根据书签找到单元格并输出图片
            if (model.lllegalpic != null)
            {
                try
                {
                    for (int i = 0; i < model.lllegalpic.Count; i++)
                    {
                        marks = marks + i;
                        builder.MoveToBookmark(marks);
                        builder.InsertImage(model.lllegalpic[i].fileurl, RelativeHorizontalPosition.Margin, 1, RelativeVerticalPosition.Margin, 1, 80, 100, WrapType.Square);
                    }
                }
                catch
                {

                }
            }
            // shape.ImageData.SetImage(photo)
            var path = Server.MapPath("../../../Content/export/");
            path = path.Substring(0, path.LastIndexOf("\\") + 1);

            doc.Save(Path.Combine(path, "违章信息.doc"), SaveFormat.Doc);
            ExcelHelper.DownLoadFile(Path.Combine(path, "违章信息.doc"), "违章信息.doc");
            return View();
        }


        public ActionResult ExWord(string id)
        {
            LllegalEntity model = bll.GetLllegalDetail(id);
            if (model != null)
            {
                XWPFDocument doc = new XWPFDocument();
                XWPFParagraph pl = doc.CreateParagraph();
                pl.Alignment = NPOI.XWPF.UserModel.ParagraphAlignment.CENTER;
                XWPFRun runtitle = pl.CreateRun();
                runtitle.SetBold(true);
                runtitle.SetText("违章记录");
                runtitle.FontSize = 18;
                runtitle.FontFamily = "宋体";

                XWPFTable tabletop = doc.CreateTable(8, 4);
                tabletop.Width = 8000;

                tabletop.SetColumnWidth(0, 5 * 256);
                tabletop.SetColumnWidth(1, 7 * 556);
                tabletop.SetColumnWidth(2, 5 * 256);
                tabletop.SetColumnWidth(3, 6 * 556);

                tabletop.GetRow(0).GetCell(0).SetParagraph(SetCellText(doc, tabletop, "违章编号", 40));
                tabletop.GetRow(0).GetCell(1).SetParagraph(SetCellText(doc, tabletop, model.LllegalNumber, 40));
                tabletop.GetRow(0).GetCell(2).SetParagraph(SetCellText(doc, tabletop, "违章部门", 40));
                tabletop.GetRow(0).GetCell(3).SetParagraph(SetCellText(doc, tabletop, model.LllegalDepart, 40));
                XWPFTableCell cell = tabletop.GetRow(0).GetCell(0);
                XWPFTableRow row = tabletop.GetRow(0);
                ICell ce = tabletop.GetRow(0).GetCell(0) as ICell;
                //ce.CellStyle.VerticalAlignment = VerticalAlignment.Center;
                tabletop.GetRow(1).GetCell(0).SetParagraph(SetCellText(doc, tabletop, "违章类型", 40));
                tabletop.GetRow(1).GetCell(1).SetParagraph(SetCellText(doc, tabletop, model.LllegalType, 40));
                tabletop.GetRow(1).GetCell(2).SetParagraph(SetCellText(doc, tabletop, "违章班组", 40));
                tabletop.GetRow(1).GetCell(3).SetParagraph(SetCellText(doc, tabletop, model.LllegalTeam, 40));

                tabletop.GetRow(2).GetCell(0).SetParagraph(SetCellText(doc, tabletop, "违章级别", 40));
                tabletop.GetRow(2).GetCell(1).SetParagraph(SetCellText(doc, tabletop, model.LllegalLevel, 40));
                tabletop.GetRow(2).GetCell(2).SetParagraph(SetCellText(doc, tabletop, "违章地点", 40));
                tabletop.GetRow(2).GetCell(3).SetParagraph(SetCellText(doc, tabletop, model.LllegalAddress, 40));

                tabletop.GetRow(3).GetCell(0).SetParagraph(SetCellText(doc, tabletop, "登记人", 40));
                tabletop.GetRow(3).GetCell(1).SetParagraph(SetCellText(doc, tabletop, model.RegisterPerson, 40));
                tabletop.GetRow(3).GetCell(2).SetParagraph(SetCellText(doc, tabletop, "违章时间", 40));
                tabletop.GetRow(3).GetCell(3).SetParagraph(SetCellText(doc, tabletop, model.LllegalTime.ToString("yyyy-MM-dd"), 40));

                tabletop.GetRow(4).MergeCells(1, 3);
                tabletop.GetRow(4).GetCell(0).SetParagraph(SetCellText(doc, tabletop, "违章人员", 150));
                tabletop.GetRow(4).GetCell(1).SetParagraph(SetCellText(doc, tabletop, model.LllegalPerson, 150));

                tabletop.GetRow(5).MergeCells(1, 3);
                tabletop.GetRow(5).GetCell(0).SetParagraph(SetCellText(doc, tabletop, "违章描述", 150));
                tabletop.GetRow(5).GetCell(1).SetParagraph(SetCellText(doc, tabletop, model.LllegalDescribe, 150));

                tabletop.GetRow(6).MergeCells(1, 3);
                tabletop.GetRow(6).GetCell(0).SetParagraph(SetCellText(doc, tabletop, "违章照片", 150));
                tabletop.GetRow(6).GetCell(1).SetParagraph(SetCellText1(doc, tabletop, "", 150, model));

                tabletop.GetRow(7).GetCell(0).SetParagraph(SetCellText(doc, tabletop, "核准人", 40));
                tabletop.GetRow(7).GetCell(1).SetParagraph(SetCellText(doc, tabletop, model.ApprovePerson, 40));
                tabletop.GetRow(7).GetCell(2).SetParagraph(SetCellText(doc, tabletop, "核准时间", 40));
                string now = DateTime.Now.ToString("yyyy-MM-dd");
                if (model.ApproveDate != null)
                {
                    now = model.ApproveDate.Value.ToString("yyyy-MM-dd");
                }
                tabletop.GetRow(7).GetCell(3).SetParagraph(SetCellText(doc, tabletop, now, 40));

                //setAlign(tabletop);
                System.IO.MemoryStream ms = new MemoryStream();
                doc.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.doc", HttpUtility.UrlEncode("违章记录：" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff"), System.Text.Encoding.UTF8)));
                Response.BinaryWrite(ms.ToArray());
                Response.End();
                ms.Close();
                ms.Dispose();
            }
            return View();
        }
        public void setAlign(XWPFTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                table.GetRow(i).GetCell(0).SetVerticalAlignment(XWPFTableCell.XWPFVertAlign.CENTER);
                table.GetRow(i).GetCell(1).SetVerticalAlignment(XWPFTableCell.XWPFVertAlign.CENTER);
                if (table.GetRow(i).GetCell(2) != null)
                {
                    table.GetRow(i).GetCell(2).SetVerticalAlignment(XWPFTableCell.XWPFVertAlign.CENTER);
                }
                if (table.GetRow(i).GetCell(3) != null)
                {
                    table.GetRow(i).GetCell(3).SetVerticalAlignment(XWPFTableCell.XWPFVertAlign.CENTER);
                }
            }
        }

        /// <summary>
        /// 输出图片
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="table"></param>
        /// <param name="setText"></param>
        /// <param name="textpos"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public XWPFParagraph SetCellText1(XWPFDocument doc, XWPFTable table, string setText, int textpos, LllegalEntity model)
        {
            CT_P para = new CT_P();
            XWPFParagraph pcell = new XWPFParagraph(para, table.Body);
            pcell.Alignment = NPOI.XWPF.UserModel.ParagraphAlignment.LEFT;
            pcell.VerticalAlignment = TextAlignment.CENTER;
            if (setText == null) setText = "";
            XWPFRun r1c1 = pcell.CreateRun();
            for (int i = 0; i < model.Files.Count; i++)
            {
                var data = fileInfoBLL.GetEntity(model.Files[i].FileId);
                string filename = Server.UrlDecode(data.FileName);//返回客户端文件名称
                string filepath = this.Server.MapPath(data.FilePath);

                var img = Image.FromFile(filepath);
                var widthEmus = (int)(400.0 * 9526);
                var heightEmus = (int)(500.0 * 9526);
                using (FileStream picData = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    r1c1.AddPicture(picData, (int)NPOI.XWPF.UserModel.PictureType.PNG, filename, widthEmus, heightEmus);
                }
            }

            //var data = fileInfoBLL.GetEntity("");
            //    string filename = Server.UrlDecode(data.FileName);//返回客户端文件名称
            //    string filepath = this.Server.MapPath(data.FilePath);

            //    var img = Image.FromFile(filepath);
            //    var widthEmus = (int)(1000.0 * 1000);
            //    var heightEmus = (int)(1000.0 * 1000);
            //    using (FileStream picData = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            //    {
            //        r1c1.AddPicture(picData, (int)NPOI.XWPF.UserModel.PictureType.PNG, filename, widthEmus, heightEmus);
            //    }

            return pcell;
        }
        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="table"></param>
        /// <param name="setText"></param>
        /// <returns></returns>
        public XWPFParagraph SetCellText(XWPFDocument doc, XWPFTable table, string setText)
        {
            CT_P para = new CT_P();
            XWPFParagraph pcell = new XWPFParagraph(para, table.Body);
            pcell.Alignment = NPOI.XWPF.UserModel.ParagraphAlignment.CENTER;
            pcell.VerticalAlignment = TextAlignment.CENTER;

            if (setText == null) setText = "";

            XWPFRun r1c1 = pcell.CreateRun();
            r1c1.SetText(setText);
            r1c1.FontSize = 12;
            r1c1.FontFamily = "宋体";

            return pcell;
        }
        /// <summary>
        /// 设置单元格格式
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="table"></param>
        /// <param name="setText"></param>
        /// <returns></returns>
        public XWPFParagraph SetCellText(XWPFDocument doc, XWPFTable table, string setText, int textpos)
        {
            CT_P para = new CT_P();
            XWPFParagraph pcell = new XWPFParagraph(para, table.Body);
            pcell.Alignment = NPOI.XWPF.UserModel.ParagraphAlignment.CENTER;
            pcell.VerticalAlignment = TextAlignment.AUTO;
            if (setText == null) setText = "";
            XWPFRun r1c1 = pcell.CreateRun();
            r1c1.SetText(setText);
            r1c1.FontSize = 12;
            //r1c1.SetBold(true);
            r1c1.FontFamily = "宋体";
            r1c1.SetTextPosition(textpos);

            return pcell;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [HandlerMonitor(5, "违章管理-获取违章列表")]
        public ActionResult List(int page, int pagesize, FormCollection fc)
        {
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            var user = OperatorProvider.Provider.Current();
            var from = fc.Get("from");
            var to = fc.Get("to");
            var filtertype = fc.Get("filtertype") ?? "";
            var llevel = fc.Get("filtervalue1") ?? "";
            var ltype = fc.Get("filtervalue2") ?? "";
            var lperson = fc.Get("filtervalue3") ?? "";

            var total = 0;
            //var data = new LllegalBLL().GetList(user.DeptId, filtertype == "全部" ? string.Empty : filtertype, filtervalue, string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from), string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to), page, pagesize, out total);
            var data = this.GetLllegalList(pagesize, page, ltype, llevel, from, to, out total);

            ViewBag.pagecount = Math.Ceiling((decimal)total / pagesize);

            ViewData["filtertype"] = new SelectList(new List<dynamic>() {
                new { value = "全部", text = "全部" },
                new { value = "违章级别", text = "违章级别" },
                new { value = "违章类型", text = "违章类型" },
            }, "value", "text", fc.Get("filtertype" ?? "全部"));
            List<LllegaLevel> levels = this.GetLllegalLevels();
            if (levels == null)
            {
                ViewData["filtervalue1"] = new SelectList(new List<dynamic>() {new
                {
                    lllegallevelname = "全部",
                    lllegallevelid = ""
                }},
             "lllegallevelid", "lllegallevelname", fc.Get("filtervalue1" ?? ""));
            }
            else
            {
                levels.Insert(0, new LllegaLevel()
                {
                    lllegallevelid = string.Empty,
                    lllegallevelname = "全部"
                });
                ViewData["filtervalue1"] = new SelectList(levels,
                    "lllegallevelid", "lllegallevelname", fc.Get("filtervalue1" ?? ""));
            }
            List<LllegaType.ItemData> types = this.GetLllegaTypes();
            if (types == null)
            {
                ViewData["filtervalue2"] = new SelectList(new List<dynamic>() {
                    new{
                        lllegaltypeid = "",
                        lllegaltypename = "全部"
                    }
                },
                   "lllegaltypeid", "lllegaltypename", fc.Get("filtervalue2" ?? ""));
            }
            else
            {
                types.Insert(0, new LllegaType.ItemData()
                {
                    lllegaltypeid = string.Empty,
                    lllegaltypename = "全部"
                });
                ViewData["filtervalue2"] = new SelectList(types,
                    "lllegaltypeid", "lllegaltypename", fc.Get("filtervalue2" ?? ""));
            }
            ViewData["filtervalue3"] = fc.Get("filtervalue3");
            ViewData["from"] = from;
            ViewData["to"] = to;

            return View(data);
        }

        /// <summary>
        /// 前台日历当天违章列表
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="deptcode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ListToJson(int year, int month, int day, string deptcode)
        {
            int total = 0;
            var data = this.GetLllegalList(100000, 1, string.Empty, string.Empty, string.Empty, string.Empty, out total, year, month, day, deptcode);
            return Json(data);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(string id)
        {
            var model = new LllegaDetailEntity();
            if (!string.IsNullOrEmpty(id))
            {
                model = this.GetLllegalDetail(id);
            }
            ViewBag.Show = "init";
            return View(model);
        }

        #region  -----------------------2018.12.20 双控违章数据对接-------------------------

        private readonly string ErchtmsApiUrl = Config.GetValue("ErchtmsApiUrl");

        /// <summary>
        /// 获取违章类型
        /// </summary>
        /// <returns></returns>
        public List<LllegaType.ItemData> GetLllegaTypes()
        {
            var user = OperatorProvider.Provider.Current();

            var rdm = this.PostHiddenData("GetLllegalType", new
            {
                userid = user.UserId,
                tokenid = string.Empty
            });
            if (rdm == null || rdm.data == null)
                return null;

            return JsonConvert.DeserializeObject<List<LllegaType.ItemData>>(JsonConvert.DeserializeObject<dynamic>(rdm.data.ToString()).itemdata.ToString());
        }

        /// <summary>
        /// 获取违章级别
        /// </summary>
        /// <returns></returns>
        public List<LllegaLevel> GetLllegalLevels()
        {
            var user = OperatorProvider.Provider.Current();
            var rdm = this.PostHiddenData("GetLllegalLevel", new
            {
                userid = user.UserId,
                tokenid = string.Empty
            });
            if (rdm == null || rdm.data == null)
                return null;

            return JsonConvert.DeserializeObject<List<LllegaLevel>>(rdm.data.ToString());
        }
        /// <summary>
        /// 获取违章列表
        /// </summary>
        /// <returns></returns>
        public List<LllegaEntity> GetLllegalList(int pagesize, int pageindex, string ltype, string llevel, string lstartdate, string lenddate, out int total, int? year = null, int? month = null, int? day = null, string deptcode = "")
        {
            var user = OperatorProvider.Provider.Current();
            object rqParam = null;
            if (year.HasValue)
            {
                rqParam = new
                {
                    year = year.ToString(),
                    month = month.ToString(),
                    day = day.ToString(),
                    currdeptcode = user.DeptCode
                };
            }
            else
            {
                rqParam = new
                {
                    action = "10",
                    lllegaltype = ltype,
                    lllegallevel = llevel,
                    lllegalstartdate = lstartdate,
                    lllegalenddate = lenddate
                };
            }
            var rdm = this.PostHiddenData("GetLllegalList", new
            {
                userid = user.UserId,
                pagesize = pagesize,
                pageindex = pageindex,
                tokenid = string.Empty,
                data = rqParam
            });
            if (rdm == null || rdm.data == null)
            {
                total = 0;
                return new List<LllegaEntity>();
            }
            total = rdm.count.Value;
            return JsonConvert.DeserializeObject<List<LllegaEntity>>(rdm.data.ToString());
        }
        /// <summary>
        /// 按月统计当天登记的违章并且当天未验收的违章数量和当天登记的违章的总数量
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<DateNumEntity> GetLllegalRegisterNumByMonth(int year, int month)
        {
            var user = OperatorProvider.Provider.Current();
            var rdm = this.PostHiddenData("GetLllegalRegisterNumByMonth", new
            {
                userid = user.UserId,
                data = new
                {
                    year = year.ToString(),
                    month = month.ToString()
                }
            });
            if (rdm == null || rdm.data == null)
            {
                return new List<DateNumEntity>();
            }
            return JsonConvert.DeserializeObject<List<DateNumEntity>>(rdm.data.ToString());
        }

        /// <summary>
        /// 获取违章详情
        /// </summary>
        /// <returns></returns>
        public LllegaDetailEntity GetLllegalDetail(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var rdm = this.PostHiddenData("GetLllegalDetail", new
            {
                userid = user.UserId,
                data = new
                {
                    lllegalid = id
                }
            });
            if (rdm == null || rdm.data == null)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<LllegaDetailEntity>(rdm.data.ToString());
        }

        public RetDataModel PostHiddenData(string method, object data)
        {
            try
            {
                string url = Path.Combine(ErchtmsApiUrl, "Hidden", method);
                string param = "json=" + JsonConvert.SerializeObject(data);
                string res = HttpMethods.HttpPost(url, param);
                RetDataModel rdm = JsonConvert.DeserializeObject<RetDataModel>(res);
                NLog.LogManager.GetCurrentClassLogger().Info("windows终端-违章管理接口\r\n-->请求地址：{0}\r\n-->请求数据：{1}\r\n-->返回数据：{2}", url, param, JsonConvert.SerializeObject(rdm));
                return rdm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }
        #endregion

        public ActionResult Index2(string type)
        {
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var main = itembll.GetEntityByName("违章类型");
            var content = itemdetialbll.GetList(main.ItemId).ToList();
            content.Insert(0, new Entity.SystemManage.DataItemDetailEntity() { ItemValue = "全部", ItemName = "全部" });
            ViewData["category"] = content.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });

            main = itembll.GetEntityByName("违章等级");
            content = itemdetialbll.GetList(main.ItemId).ToList();
            content.Insert(0, new Entity.SystemManage.DataItemDetailEntity() { ItemValue = "全部", ItemName = "全部" });
            ViewData["level"] = content.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });

            ViewData["state"] = new List<SelectListItem>() { new SelectListItem() { Value = "全部", Text = "全部" }, new SelectListItem() { Value = "待核准", Text = "待核准" }, new SelectListItem() { Value = "待整改", Text = "待整改" }, new SelectListItem() { Value = "待验收", Text = "待验收" }, new SelectListItem() { Value = "验收通过", Text = "验收通过" }, new SelectListItem() { Value = "核准不通过", Text = "核准不通过" } };
            ViewBag.type = type;
            return View();
        }

        public ActionResult ApproveList()
        {
            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var main = itembll.GetEntityByName("违章类型");
            var content = itemdetialbll.GetList(main.ItemId).ToList();
            content.Insert(0, new Entity.SystemManage.DataItemDetailEntity() { ItemValue = "全部", ItemName = "全部" });
            ViewData["category"] = content.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });

            main = itembll.GetEntityByName("违章等级");
            content = itemdetialbll.GetList(main.ItemId).ToList();
            content.Insert(0, new Entity.SystemManage.DataItemDetailEntity() { ItemValue = "全部", ItemName = "全部" });
            ViewData["level"] = content.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });

            return View();
        }

        public JsonResult GetData(string type)
        {
            var user = OperatorProvider.Provider.Current();
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var no = this.Request.QueryString.Get("no");
            var person = this.Request.QueryString.Get("person");
            var category = this.Request.QueryString.Get("category");
            var level = this.Request.QueryString.Get("level");
            var state = this.Request.QueryString.Get("state");
            var total = 0;
            var data = bll.GetData(user.DeptId, user.DeptCode, no, person, level, category, string.IsNullOrEmpty(state) ? "全部" : state, pagesize, page, out total);
            if (type == "4")
            {

            }
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApproving()
        {
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var no = this.Request.QueryString.Get("no");
            var person = this.Request.QueryString.Get("person");
            var category = this.Request.QueryString.Get("category");
            var level = this.Request.QueryString.Get("level");
            var total = 0;

            var data = bll.GetApproving(no, person, level, category, pagesize, page, out total);
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / pagesize) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckApproveItem(string id)
        {
            var model = bll.GetLllegalDetail(id);

            var user = OperatorProvider.Provider.Current();
            //model.ApproveResult = "0";
            //model.ApprovePersonId = user.UserId;
            //model.ApprovePerson = user.UserName;
            //model.ApproveDate = DateTime.Now;
            //model.ApproveReason = string.Empty;

            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var main = itembll.GetEntityByName("考核方式");
            var content = itemdetialbll.GetList(main.ItemId).ToList();
            ViewData["checktype"] = content.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });

            if (model.Files == null) model.Files = new List<FileInfoEntity>();

            model.refrom = new LllegalRefromBLL().GetEntityByLllegalId(model.ID);
            model.accept = new LllegalAcceptBLL().GetEntityByLllegalId(model.ID);
            if (model.refrom == null)
            {
                model.refrom = new LllegalRefromEntity();

                model.refrom.RefromTime = null;
                model.refrom.Files = new List<FileInfoEntity>();

            }
            if (model.accept == null)
            {
                model.accept = new LllegalAcceptEntity();
                model.accept.AcceptTime = null;
                model.accept.Files = new List<FileInfoEntity>();
            }

            return View(model);
        }

        public ActionResult ApproveItem(string id)
        {
            var model = bll.GetLllegalDetail(id);

            var user = OperatorProvider.Provider.Current();
            model.ApproveResult = "0";
            model.ApprovePersonId = user.UserId;
            model.ApprovePerson = user.UserName;
            model.ApproveDate = DateTime.Now;
            model.ApproveReason = string.Empty;

            var itemdetialbll = new DataItemDetailBLL();
            var itembll = new DataItemBLL();
            var main = itembll.GetEntityByName("考核方式");
            var content = itemdetialbll.GetList(main.ItemId).ToList();
            ViewData["checktype"] = content.Select(x => new SelectListItem() { Value = x.ItemName, Text = x.ItemName });

            if (model.Files == null) model.Files = new List<FileInfoEntity>();
            var users = userbll.GetUserList().ToList();

            ViewData["users"] = users;
            return View(model);
        }

        [HttpPost]
        public JsonResult ApproveItem(string id, LllegalEntity model)
        {
            bll.Approve(model);
            return Json(new { success = true, message = "核准结束" });
        }

        [HttpPost]
        public JsonResult GetNewData()
        {
            string deptid = OperatorProvider.Provider.Current().DeptId;
            return Json(new { rows = bll.GetCount(deptid) });
        }

        public JsonResult GetWorks()
        {
            string deptid = OperatorProvider.Provider.Current().DeptId;
            return Json(new { rows = bll.GetFinish(deptid) });
        }


    }
}
