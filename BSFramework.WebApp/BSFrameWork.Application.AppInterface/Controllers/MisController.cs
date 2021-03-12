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
using BSFramework.Util.Log;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
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
    public class MisController : BaseApiController
    {
        [HttpPost]
        public ListBucket<string> GetUnits(ParamBucket args)
        {
            var bll = new FaultBLL();
            var data = bll.GetUnits();
            return new ListBucket<string>() { Data = data, Success = true, Total = data.Count };
        }

        [HttpPost]
        public ListBucket<string> GetSpecialties(ParamBucket args)
        {
            var bll = new FaultBLL();
            var data = bll.GetSpecialties();
            return new ListBucket<string>() { Data = data, Success = true, Total = data.Count };
        }

        [HttpPost]
        public ListBucket<string> GetCategories(ParamBucket args)
        {
            var bll = new FaultBLL();
            var data = bll.GetCategories();
            return new ListBucket<string>() { Data = data, Success = true, Total = data.Count };
        }

        [HttpPost]
        public ListBucket<string> GetStatus(ParamBucket args)
        {
            var bll = new FaultBLL();
            var data = bll.GetStatus();
            return new ListBucket<string>() { Data = data, Success = true, Total = data.Count };
        }

        [HttpPost]
        public ListBucket<FaultEntity> GetFaults(ParamBucket<FaultModel> args)
        {

            var deptname = string.Empty;
            string[] deptnameList = null;
            if (args.Data.IsBz)
            {
                var user = OperatorProvider.Provider.Current();
                var dept = new DepartmentBLL().GetEntity(user.DeptId);
                if (dept.TeamType == "02")
                {
                    if (dept.Nature == "班组")
                    {
                        dept = new DepartmentBLL().GetEntity(dept.ParentId);
                        deptname = dept.FullName;
                        deptnameList = new string[] { deptname };

                    }
                }
            }
            else
            {

                //if (args.Data.Deptname == null)
                //{
                //    deptnameList = new string[0];
                //}
                //else
                //{
                deptnameList = args.Data.Deptname;

                //}
            }
            var bll = new FaultBLL();
            var total = 0;
            var data = bll.GetFaults(deptnameList, args.Data.Units, args.Data.Specialty, args.Data.Categories, args.Data.Status, args.PageSize, args.PageIndex, out total);
            return new ListBucket<FaultEntity>() { Data = data, Success = true, Total = total };
        }
        [HttpPost]
        public ListBucket<FaultEntity> GetFaultsByClass(ParamBucket<FaultModel> args)
        {
            var deptname = string.Empty;
            string[] deptnameList = null;
            if (args.Data == null)
            {
                args.Data = new FaultModel();
            }
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            //if (dept.TeamType == "02")
            //{
            //    if (dept.Nature == "班组")
            //    {
            //        dept = new DepartmentBLL().GetEntity(dept.ParentId);
            //        deptname = dept.FullName;
            //        deptnameList[0] = deptname;
            //    }
            //}
            //else
            //{
            //if (args.Data.Deptname == null)
            //{
            //    deptnameList = new string[0];
            //}
            //else
            //{
            deptnameList = args.Data.Deptname;

            //}
            //}

            var bll = new FaultBLL();
            var total = 0;
            var data = bll.GetFaultsByClass(deptnameList, args.Data.Units, args.Data.Specialty, args.PageSize, args.PageIndex, out total);
            return new ListBucket<FaultEntity>() { Data = data, Success = true, Total = total };
        }
        [HttpPost]
        public ListBucket<string> GetFaultsDept()
        {

            var bll = new FaultBLL();
            var data = bll.GetFaultsDept();
            return new ListBucket<string>() { Data = data, Success = true, Total = data.Count };
        }

        [HttpPost]
        public object GetStatistical(ParamBucket<string> args)
        {
            var deptname = string.Empty;
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            deptname = user.DeptName;
            //if (dept.TeamType == "02")
            //{
            //    if (dept.Nature == "班组")
            //    {
            //        dept = new DepartmentBLL().GetEntity(dept.ParentId);
            //        deptname = dept.FullName;
            //    }
            //}
            var bll = new FaultBLL();
            var result = new List<object>();
            var nowTime = DateTime.Now;
            if (args.Data == "0")
            {
                //月
                var start = new DateTime(nowTime.Year, 1, 1);
                var end = nowTime;
                var data = bll.GetStatistical(deptname, start, end, dept.TeamType).OrderBy(x => x.FoundTime).ToList();
                if (data.Count > 0)
                {
                    for (int i = 1; i <= end.Month; i++)
                    {
                        var month = data.Where(x => x.FoundTime.Month == i).ToList();
                        if (month.Count == 0)
                        {
                            result.Add(new { name = i + "月", total = month.Count, xc = 0, wxc = 0, xcl = 0, jsxc = 0, jsxcl = 0 });
                            continue;
                        }
                        var xc = month.Where(x => x.Status == "已消除").ToList();
                        var wxc = month.Where(x => x.Status == "未消除").ToList();
                        var jsxc = month.Where(x => x.TimeStatus == "及时消缺").ToList();

                        result.Add(new { name = i + "月", total = month.Count, xc = xc.Count, wxc = wxc.Count, xcl = Math.Round((decimal)xc.Count / (decimal)month.Count, 2), jsxc = jsxc.Count, jsxcl = Math.Round((decimal)jsxc.Count / (decimal)month.Count, 2) });
                    }

                }
            }
            else if (args.Data == "1")
            {
                //季
                var start = new DateTime(nowTime.Year, 1, 1);
                var end = nowTime;
                var data = bll.GetStatistical(deptname, start, end, dept.TeamType).OrderBy(x => x.FoundTime).ToList();
                if (data.Count > 0)
                {
                    int ii = 1;
                    for (int i = 3; i <= end.Month; i = i + 3)
                    {
                        var month = data.Where(x => x.FoundTime.Month <= i && x.FoundTime.Month > (i - 3)).ToList();
                        if (month.Count == 0)
                        {
                            result.Add(new { name = "第" + ii + "季", total = month.Count, xc = 0, wxc = 0, xcl = 0, jsxc = 0, jsxcl = 0 });
                            continue;
                        }
                        var xc = month.Where(x => x.Status == "已消除").ToList();
                        var wxc = month.Where(x => x.Status == "未消除").ToList();
                        var jsxc = month.Where(x => x.TimeStatus == "及时消缺").ToList();
                        result.Add(new { name = "第" + ii + "季", total = month.Count, xc = xc.Count, wxc = wxc.Count, xcl = Math.Round((decimal)xc.Count / (decimal)month.Count, 2), jsxc = jsxc.Count, jsxcl = Math.Round((decimal)jsxc.Count / (decimal)month.Count, 2) });
                        ii++;
                    }
                }
            }
            else
            {
                //年
                var start = new DateTime(1990, 1, 1);
                var end = nowTime;
                var data = bll.GetStatistical(deptname, start, end, dept.TeamType).OrderBy(x => x.FoundTime).ToList();
                if (data.Count > 0)
                {
                    var startTime = data[0].FoundTime;
                    var endTime = data[data.Count - 1].FoundTime;
                    var endYear = endTime.Year;
                    var startYear = startTime.Year;
                    for (int i = startYear; i <= endYear; i++)
                    {
                        var year = data.Where(x => x.FoundTime.Year == i).ToList();
                        if (year.Count == 0)
                        {
                            result.Add(new { name = i + "年", total = year.Count, xc = 0, wxc = 0, xcl = 0, jsxc = 0, jsxcl = 0 });
                            continue;
                        }
                        var xc = year.Where(x => x.Status == "已消除").ToList();
                        var wxc = year.Where(x => x.Status == "未消除").ToList();
                        var jsxc = year.Where(x => x.TimeStatus == "及时消缺").ToList();
                        result.Add(new { name = i + "年", total = year.Count, xc = xc.Count, wxc = wxc.Count, xcl = Math.Round((decimal)xc.Count / (decimal)year.Count, 2), jsxc = jsxc.Count, jsxcl = Math.Round((decimal)jsxc.Count / (decimal)year.Count, 2) });

                    }
                }
            }


            return new { Data = result, Success = true };
        }

        [HttpPost]
        public ModelBucket<FaultEntity> GetDetail(ParamBucket<decimal> args)
        {
            var bll = new FaultBLL();
            var data = bll.GetDetail(args.Data);
            return new ModelBucket<FaultEntity>() { Data = data, Success = true };
        }

        #region 机长值班交接日志
        /// <summary>
        /// 分页查询班长值班交接日志
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetJJRZ(ParamBucket<Mis_JJRZModel> json)
        {
            try
            {
                WriteLog.AddLog($"/Mis/GetJJRZ 接收到参数{JsonConvert.SerializeObject(json)}","MisLog");
                int totalCount = 0;
                var bll = new FaultBLL();
                DataTable data = bll.GetJJRZ(json.PageIndex, json.PageSize, json.Data.BZMC, json.Data.FL, json.Data.KEYWORD, json.Data.STARTDATE, json.Data.ENDDATE, ref totalCount);
                WriteLog.AddLog($"/Mis/GetFL 查询到{data.Rows.Count}条数据", "MisLog");
                return new { Code = 0, Info = "查询成功", data = data, count = totalCount };
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"/Mis/GetJJRZ  报错{JsonConvert.SerializeObject(ex)}", "MisLog");
                return new { Code = -1, Info = "查询成功", data = ex.Message };
            }
        }
        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="dataBucket"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetFL()
        {
            try
            {
                WriteLog.AddLog($"/Mis/GetFL 开始", "MisLog");
                var bll = new FaultBLL();
                DataTable data = bll.GetFL();
                List<string> strList = new List<string>();
                if (data.Rows !=null && data.Rows.Count>0)
                {
                    var dtEnum = data.Rows.GetEnumerator();
                    while (dtEnum.MoveNext())
                    {
                        DataRow dr = dtEnum.Current as DataRow;
                        if (dr["FL"] !=null)
                        {
                            strList.Add(dr["FL"].ToString());
                        }
                    }
                }
                WriteLog.AddLog($"/Mis/GetFL 查询到{data.Rows.Count}条数据", "MisLog");
                return new { Code = 0, Info = "查询成功", data = strList };
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"/Mis/GetFL  报错{JsonConvert.SerializeObject(ex)}", "MisLog");
                return new { Code = -1, Info = "查询成功", data = ex.Message };
            }
     
        }
        #endregion 
    }
}
