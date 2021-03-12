using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.MisManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.MisManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class TicketController : BaseApiController
    {

        private string ticketStr = ConfigurationManager.AppSettings["Ticket"].ToString();
        [HttpPost]
        public ListBucket<string> GetUnits(ParamBucket args)
        {
            var bll = new TicketBLL();

            var data = bll.GetUnits(ticketStr);
            return new ListBucket<string>() { Data = data, Success = true, Total = data.Count };
        }

        [HttpPost]
        public ListBucket<string> GetStatus(ParamBucket args)
        {
            var bll = new TicketBLL();
            var data = bll.GetStatus(ticketStr);
            return new ListBucket<string>() { Data = data, Success = true, Total = data.Count };
        }

        [HttpPost]
        public ListBucket<TicketEntity> GetList(ParamBucket<TicketModel> args)
        {

            var isHumanDanger = ConfigurationManager.AppSettings["isHumanDanger"].ToString();

            if (isHumanDanger != "是")
            {
                return new ListBucket<TicketEntity>() { Data = new List<TicketEntity>(), Success = true, Total = 0 };
            }
            var deptname = string.Empty;
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            if (dept.TeamType == "02")
            {
                deptname = dept.FullName;
                //if (dept.Nature == "班组")
                //{
                //    dept = new DepartmentBLL().GetEntity(dept.ParentId);
                //    deptname = dept.FullName;
                //}
            }
            var bll = new TicketBLL();
            var total = 0;
            var data = bll.GetList(deptname, args.Data.Units, args.Data.DutyPerson, args.Data.Category, args.Data.Status, args.Data.IncludeCode, args.Data.KeyWord,args.PageSize, args.PageIndex, out total, ticketStr);
            return new ListBucket<TicketEntity>() { Data = data, Success = true, Total = total };
        }

        [HttpPost]
        public object GetStatistical(ParamBucket<TicketModel> args)
        {
            var deptname = string.Empty;
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            if (dept.TeamType == "02")
            {
                deptname = dept.FullName;
                //if (dept.Nature == "班组")
                //{
                //    dept = new DepartmentBLL().GetEntity(dept.ParentId);
                //    deptname = dept.FullName;
                //}
            }
            var bll = new TicketBLL();

            var result = new List<object>();
            var nowTime = DateTime.Now;
            if (args.Data.Category == "0")
            {
                //月
                var start = new DateTime(nowTime.Year, 1, 1);
                var end = nowTime;
                var data = bll.GetStatistical(deptname, args.Data.Units, start, end, dept.TeamType, ticketStr).OrderBy(x => x.EndTime).ToList();
                data = data.Where(x => x.EndTime.HasValue).ToList();
                if (data.Count > 0)
                {
                    for (int i = 1; i <= end.Month; i++)
                    {
                        var month = data.Where(x => x.EndTime.Value.Month == i).ToList();
                        //other
                        var other = month.Select(x => x.OtherTickets).ToList();
                        //一级动火证
                        int yjdhz = 0;
                        //二级动火证
                        int ejdhz = 0;
                        //风险作业审批单
                        int fxzyspd = 0;
                        //热控保护措施票
                        int rkbhcsp = 0;
                        //继电保护措施票
                        int jdbhcsp = 0;
                        //作业安全措施票
                        int zyaqcsp = 0;
                        if (month.Count == 0)
                        {
                            result.Add(new { name = i + "月", total = month.Count, dqdyzgzp = 0, dqdezgzp = 0, sljxgzp = 0, rggzp = 0, yjdhz = yjdhz, ejdhz = ejdhz, fxzyspd = fxzyspd, rkbhcsp = rkbhcsp, jdbhcsp = jdbhcsp, zyaqcsp = zyaqcsp });
                            continue;
                        }
                        //电气第一种工作票
                        var dqdyzgzp = new List<TicketEntity>();
                        //电气第二种工作票
                        var dqdezgzp = new List<TicketEntity>();
                        //势力机械工作票
                        var sljxgzp = new List<TicketEntity>();
                        //热工工作票
                        var rggzp = new List<TicketEntity>();
                        switch (ticketStr)
                        {

                            case "红雁池":
                                //电气第一种工作票
                                dqdyzgzp = month.Where(x => x.Category == "电气第一种工作票").ToList();
                                //电气第二种工作票
                                dqdezgzp = month.Where(x => x.Category == "电气第二种工作票").ToList();
                                //热力机械工作票
                                sljxgzp = month.Where(x => x.Category == "热力机械工作票").ToList();
                                //热控工作票
                                rggzp = month.Where(x => x.Category == "热控工作票").ToList();
                                //一级动火证
                                yjdhz = month.Where(x => x.Category == "一级动火工作票").ToList().Count;
                                //二级动火证
                                ejdhz = month.Where(x => x.Category == "二级动火工作票").ToList().Count;
                                //风险作业审批单
                                fxzyspd = month.Where(x => x.Category == "").ToList().Count;
                                //热控保护措施票
                                rkbhcsp = month.Where(x => x.Category == "热控保护措施票").ToList().Count;
                                //继电保护措施票
                                jdbhcsp = month.Where(x => x.Category == "继电保护措施票").ToList().Count;
                                //作业安全措施票
                                zyaqcsp = month.Where(x => x.Category == "作业安全措施票").ToList().Count;
                                break;
                            default:
                                //电气第一种工作票
                                dqdyzgzp = month.Where(x => x.Category == "电气第一种工作票").ToList();
                                //电气第二种工作票
                                dqdezgzp = month.Where(x => x.Category == "电气第二种工作票").ToList();
                                //势力机械工作票
                                sljxgzp = month.Where(x => x.Category == "势力机械工作票").ToList();
                                //热工工作票
                                rggzp = month.Where(x => x.Category == "热工工作票").ToList();
                                foreach (var item in other)
                                {
                                    yjdhz += item["一级动火证"];
                                    ejdhz += item["二级动火证"];
                                    fxzyspd += item["风险作业审批单"];
                                    rkbhcsp += item["热控保护措施票"];
                                    jdbhcsp += item["继电保护措施票"];
                                    zyaqcsp += item["作业安全措施票"];
                                }
                                break;
                        }

                        result.Add(new { name = i + "月", total = month.Count, dqdyzgzp = dqdyzgzp.Count, dqdezgzp = dqdezgzp.Count, sljxgzp = sljxgzp.Count, rggzp = rggzp.Count, yjdhz = yjdhz, ejdhz = ejdhz, fxzyspd = fxzyspd, rkbhcsp = rkbhcsp, jdbhcsp = jdbhcsp, zyaqcsp = zyaqcsp });
                    }

                }
            }
            else if (args.Data.Category == "1")
            {
                //季
                var start = new DateTime(nowTime.Year, 1, 1);
                var end = nowTime;
                var data = bll.GetStatistical(deptname, args.Data.Units, start, end, dept.TeamType, ticketStr).OrderBy(x => x.EndTime).ToList();
                data = data.Where(x => x.EndTime.HasValue).ToList();
                if (data.Count > 0)
                {
                    int ii = 1;
                    for (int i = 3; i <= end.Month; i = i + 3)
                    {
                        var month = data.Where(x => x.EndTime.Value.Month <= i && x.EndTime.Value.Month > (i - 3)).ToList();
                        //other
                        var other = month.Select(x => x.OtherTickets).ToList();
                        //一级动火证
                        int yjdhz = 0;
                        //二级动火证
                        int ejdhz = 0;
                        //风险作业审批单
                        int fxzyspd = 0;
                        //热控保护措施票
                        int rkbhcsp = 0;
                        //继电保护措施票
                        int jdbhcsp = 0;
                        //作业安全措施票
                        int zyaqcsp = 0;
                        if (month.Count == 0)
                        {
                            result.Add(new { name = "第" + ii + "季", total = month.Count, dqdyzgzp = 0, dqdezgzp = 0, sljxgzp = 0, rggzp = 0, yjdhz = yjdhz, ejdhz = ejdhz, fxzyspd = fxzyspd, rkbhcsp = rkbhcsp, jdbhcsp = jdbhcsp, zyaqcsp = zyaqcsp });
                            continue;
                        }
                        //电气第一种工作票
                        var dqdyzgzp = new List<TicketEntity>();
                        //电气第二种工作票
                        var dqdezgzp = new List<TicketEntity>();
                        //势力机械工作票
                        var sljxgzp = new List<TicketEntity>();
                        //热工工作票
                        var rggzp = new List<TicketEntity>();
                        switch (ticketStr)
                        {

                            case "红雁池":
                                //电气第一种工作票
                                dqdyzgzp = month.Where(x => x.Category == "电气第一种工作票").ToList();
                                //电气第二种工作票
                                dqdezgzp = month.Where(x => x.Category == "电气第二种工作票").ToList();
                                //热力机械工作票
                                sljxgzp = month.Where(x => x.Category == "热力机械工作票").ToList();
                                //热控工作票
                                rggzp = month.Where(x => x.Category == "热控工作票").ToList();
                                //一级动火证
                                yjdhz = month.Where(x => x.Category == "一级动火工作票").ToList().Count;
                                //二级动火证
                                ejdhz = month.Where(x => x.Category == "二级动火工作票").ToList().Count;
                                //风险作业审批单
                                fxzyspd = month.Where(x => x.Category == "").ToList().Count;
                                //热控保护措施票
                                rkbhcsp = month.Where(x => x.Category == "热控保护措施票").ToList().Count;
                                //继电保护措施票
                                jdbhcsp = month.Where(x => x.Category == "继电保护措施票").ToList().Count;
                                //作业安全措施票
                                zyaqcsp = month.Where(x => x.Category == "作业安全措施票").ToList().Count;
                                break;
                            default:
                                //电气第一种工作票
                                dqdyzgzp = month.Where(x => x.Category == "电气第一种工作票").ToList();
                                //电气第二种工作票
                                dqdezgzp = month.Where(x => x.Category == "电气第二种工作票").ToList();
                                //势力机械工作票
                                sljxgzp = month.Where(x => x.Category == "势力机械工作票").ToList();
                                //热工工作票
                                rggzp = month.Where(x => x.Category == "热工工作票").ToList();
                                foreach (var item in other)
                                {
                                    yjdhz += item["一级动火证"];
                                    ejdhz += item["二级动火证"];
                                    fxzyspd += item["风险作业审批单"];
                                    rkbhcsp += item["热控保护措施票"];
                                    jdbhcsp += item["继电保护措施票"];
                                    zyaqcsp += item["作业安全措施票"];
                                }
                                break;
                        }
                        result.Add(new { name = "第" + ii + "季", total = month.Count, dqdyzgzp = dqdyzgzp.Count, dqdezgzp = dqdezgzp.Count, sljxgzp = sljxgzp.Count, rggzp = rggzp.Count, yjdhz = yjdhz, ejdhz = ejdhz, fxzyspd = fxzyspd, rkbhcsp = rkbhcsp, jdbhcsp = jdbhcsp, zyaqcsp = zyaqcsp });
                        ii++;
                    }
                }
            }
            else
            {
                //年
                var start = new DateTime(1990, 1, 1);
                var end = nowTime;
                var data = bll.GetStatistical(deptname, args.Data.Units, start, end, dept.TeamType, ticketStr).OrderBy(x => x.EndTime).ToList();
                data = data.Where(x => x.EndTime.HasValue).ToList();
                if (data.Count > 0)
                {
                    var startTime = data[0].EndTime.Value;
                    var endTime = data[data.Count - 1].EndTime.Value;
                    var endYear = endTime.Year;
                    var startYear = startTime.Year;
                    for (int i = startYear; i <= endYear; i++)
                    {
                        var year = data.Where(x => x.EndTime.Value.Year == i).ToList();
                        //other
                        var other = year.Select(x => x.OtherTickets).ToList();
                        //一级动火证
                        int yjdhz = 0;
                        //二级动火证
                        int ejdhz = 0;
                        //风险作业审批单
                        int fxzyspd = 0;
                        //热控保护措施票
                        int rkbhcsp = 0;
                        //继电保护措施票
                        int jdbhcsp = 0;
                        //作业安全措施票
                        int zyaqcsp = 0;
                        if (year.Count == 0)
                        {
                            result.Add(new { name = i + "年", total = year.Count, dqdyzgzp = 0, dqdezgzp = 0, sljxgzp = 0, rggzp = 0, yjdhz = yjdhz, ejdhz = ejdhz, fxzyspd = fxzyspd, rkbhcsp = rkbhcsp, jdbhcsp = jdbhcsp, zyaqcsp = zyaqcsp });
                            continue;
                        }
                        //电气第一种工作票
                        var dqdyzgzp = new List<TicketEntity>();
                        //电气第二种工作票
                        var dqdezgzp = new List<TicketEntity>();
                        //势力机械工作票
                        var sljxgzp = new List<TicketEntity>();
                        //热工工作票
                        var rggzp = new List<TicketEntity>();
                        switch (ticketStr)
                        {

                            case "红雁池":
                                //电气第一种工作票
                                dqdyzgzp = year.Where(x => x.Category == "电气第一种工作票").ToList();
                                //电气第二种工作票
                                dqdezgzp = year.Where(x => x.Category == "电气第二种工作票").ToList();
                                //热力机械工作票
                                sljxgzp = year.Where(x => x.Category == "热力机械工作票").ToList();
                                //热控工作票
                                rggzp = year.Where(x => x.Category == "热控工作票").ToList();
                                //一级动火证
                                yjdhz = year.Where(x => x.Category == "一级动火工作票").ToList().Count;
                                //二级动火证
                                ejdhz = year.Where(x => x.Category == "二级动火工作票").ToList().Count;
                                //风险作业审批单
                                fxzyspd = year.Where(x => x.Category == "").ToList().Count;
                                //热控保护措施票
                                rkbhcsp = year.Where(x => x.Category == "热控保护措施票").ToList().Count;
                                //继电保护措施票
                                jdbhcsp = year.Where(x => x.Category == "继电保护措施票").ToList().Count;
                                //作业安全措施票
                                zyaqcsp = year.Where(x => x.Category == "作业安全措施票").ToList().Count;
                                break;
                            default:
                                //电气第一种工作票
                                dqdyzgzp = year.Where(x => x.Category == "电气第一种工作票").ToList();
                                //电气第二种工作票
                                dqdezgzp = year.Where(x => x.Category == "电气第二种工作票").ToList();
                                //势力机械工作票
                                sljxgzp = year.Where(x => x.Category == "势力机械工作票").ToList();
                                //热工工作票
                                rggzp = year.Where(x => x.Category == "热工工作票").ToList();
                                foreach (var item in other)
                                {
                                    yjdhz += item["一级动火证"];
                                    ejdhz += item["二级动火证"];
                                    fxzyspd += item["风险作业审批单"];
                                    rkbhcsp += item["热控保护措施票"];
                                    jdbhcsp += item["继电保护措施票"];
                                    zyaqcsp += item["作业安全措施票"];
                                }
                                break;
                        }
                        result.Add(new { name = i + "年", total = year.Count, dqdyzgzp = dqdyzgzp.Count, dqdezgzp = dqdezgzp.Count, sljxgzp = sljxgzp.Count, rggzp = rggzp.Count, yjdhz = yjdhz, ejdhz = ejdhz, fxzyspd = fxzyspd, rkbhcsp = rkbhcsp, jdbhcsp = jdbhcsp, zyaqcsp = zyaqcsp });

                    }
                }
            }
            return new { Data = result, Success = true };
        }


        [HttpPost]
        public ModelBucket<TicketEntity> GetDetail(ParamBucket<string> args)
        {
            var bll = new TicketBLL();
            var data = bll.GetDetail(args.Data, ticketStr);
            return new ModelBucket<TicketEntity>() { Data = data, Success = true };
        }

        [HttpPost]
        public ListBucket<string> GetCategories(ParamBucket args)
        {
            var bll = new FaultBLL();
            var data = bll.GetCategories();
            switch (ticketStr)
            {
                case "红雁池":
                    var ticketBll = new TicketBLL();
                    var dataCategories = ticketBll.GetCategories(ticketStr);
                    return new ListBucket<string>() { Data = dataCategories, Success = true, Total = dataCategories.Count };
                default:
                    return new ListBucket<string>() { Data = new List<string>() { "电气第一种工作票", "电气第二种工作票", "热力机械工作票", "热工工作票" }, Success = true, Total = data.Count };

            }

        }
    }
}
