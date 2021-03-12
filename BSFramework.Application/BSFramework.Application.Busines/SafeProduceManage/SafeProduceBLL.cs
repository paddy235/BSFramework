using BSFramework.Application.Entity.SafeProduceManage;
using BSFramework.Application.IService.SafeProduceManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Application.Service.SafeProduceManage;
using BSFramework.Application.Service.SystemManage;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SafeProduceManage
{
    /// <summary>
    /// 安全文明生产检查
    /// </summary>
    public class SafeProduceBLL
    {
        private ISafeProduceService service;
        private IDistrictPersonService Districtservice;

        public SafeProduceBLL()
        {
            service = new SafeProduceService();
            Districtservice = new DistrictPersonService();
        }


        #region 获取数据
        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public SafeProduceEntity getSafeProduceDataById(string keyvalue)
        {
            return service.getSafeProduceDataById(keyvalue);
        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<SafeProduceEntity> GetPageSafeProduceList(Pagination pagination, string queryJson)
        {
            return service.GetPageSafeProduceList(pagination, queryJson);
        }
        /// <summary>
        /// 查询考勤签到
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<OnLocaleEntity> GetPageOnLocaleList(Pagination pagination, string queryJson)
        {
            return service.GetPageOnLocaleList(pagination, queryJson);


        }


        /// <summary>
        /// 查询考勤缺勤
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<OnLocaleModel> GetPageNoOnLocaleList(Pagination pagination, string queryJson)
        {
            int page = pagination.page;
            int rows = pagination.rows;
            pagination.rows = 10000;
            pagination.page = 1;
            var queryParam = queryJson.ToJObject();
            var NowTime = DateTime.Now;
            var UseNowTime = new DateTime(NowTime.Year, NowTime.Month, NowTime.Day);
            DateTime Start = UseNowTime, End = UseNowTime;
            List<OnLocaleModel> obj = new List<OnLocaleModel>();
            if (!queryParam["TimeType"].IsEmpty())
            {
                var TimeType = queryParam["TimeType"].ToString();
                //缺勤只能计算本月
                switch (TimeType)
                {
                    //今天
                    //case "1":
                    //    Start = UseNowTime;
                    //    End = UseNowTime.AddDays(1).AddMilliseconds(-1);
                    //    break;
                    ////本周
                    //case "2":
                    //    var dayNum = Convert.ToInt32(UseNowTime.DayOfWeek);
                    //    dayNum = dayNum == 0 ? 7 : dayNum;
                    //    Start = UseNowTime.AddDays(-(dayNum - 1));
                    //    End = UseNowTime.AddDays(8 - dayNum).AddMilliseconds(-1);
                    //    break;
                    //本月
                    case "3":
                        var monSum = Time.GetDaysOfMonth(UseNowTime.Year, UseNowTime.Month);
                        Start = new DateTime(UseNowTime.Year, UseNowTime.Month, 1);
                        End = new DateTime(UseNowTime.Year, UseNowTime.Month, monSum).AddDays(1).AddMilliseconds(-1);
                        break;
                    default:
                        break;
                }
                if (!queryParam["DistrictId"].IsEmpty())
                {
                    //考勤数据
                    var OnData = service.GetPageNoOnLocaleList(pagination, queryJson);
                    pagination.rows = rows;
                    pagination.page = page;
                    var DistrictId = queryParam["DistrictId"].ToString();
                    //区域管理
                    var DistrictData = Districtservice.GetList(DistrictId);
                    //截至今天
                    End = UseNowTime.AddDays(1).AddMilliseconds(-1);
                    foreach (var item in DistrictData)
                    {

                        if (item.Cycle == "每月")
                        {
                            if (End.Day > 27)
                            {
                                var ck = OnData.Where(x => x.SigninDate >= Start && x.SigninDate <= End && x.DeptId == item.DutyDepartmentId && x.DutyTypeId == item.CategoryId);
                                if (ck.Count() == 0)
                                {
                                    obj.Add(new OnLocaleModel()
                                    {
                                        DeptName = item.DutyDepartmentName,
                                        DutyType = item.CategoryName,
                                        SigninDate = Start.ToString("yyyy-MM-27")
                                    });

                                }

                            }
                        }
                        if (item.Cycle == "每天")
                        {
                            var selectEnd = End;
                            while (true)
                            {

                                if (selectEnd < Start)
                                {
                                    break;
                                }
                                var selectStart = selectEnd.AddDays(-1).AddMilliseconds(1);
                                var ck = OnData.Where(x => x.SigninDate >= selectStart && x.SigninDate <= selectEnd && x.DeptId == item.DutyDepartmentId && x.DutyTypeId == item.CategoryId);
                                if (ck.Count() == 0)
                                {
                                    obj.Add(new OnLocaleModel()
                                    {
                                        DeptName = item.DutyDepartmentName,
                                        DutyType = item.CategoryName,
                                        SigninDate = selectStart.ToString("yyyy-MM-dd")
                                    });

                                }
                                selectEnd = selectEnd.AddDays(-1);
                            }
                        }
                        if (item.Cycle == "每周")
                        {
                            var selectEnd = End;

                            while (true)
                            {
                                var diffDays = Util.Time.DiffDays(Start, selectEnd);
                                var dayNum = Convert.ToInt32(selectEnd.DayOfWeek);
                                dayNum = dayNum == 0 ? 7 : dayNum;
                                var selectStart = selectEnd.AddDays(-dayNum).AddMilliseconds(1);
                                if (diffDays >= 7)
                                {

                                    var ck = OnData.Where(x => x.SigninDate >= selectStart && x.SigninDate <= selectEnd && x.DeptId == item.DutyDepartmentId && x.DutyTypeId == item.CategoryId);
                                    if (ck.Count() == 0)
                                    {
                                        obj.Add(new OnLocaleModel()
                                        {
                                            DeptName = item.DutyDepartmentName,
                                            DutyType = item.CategoryName,
                                            SigninDate = selectEnd.ToString("yyyy-MM-dd")
                                        });
                                    }
                                    selectEnd = selectStart.AddDays(-1);
                                }
                                else
                                {

                                    var ck = OnData.Where(x => x.SigninDate >= selectStart && x.SigninDate <= selectEnd && x.DeptId == item.DutyDepartmentId && x.DutyTypeId == item.CategoryId);
                                    if (ck.Count() == 0)
                                    {
                                        obj.Add(new OnLocaleModel()
                                        {
                                            DeptName = item.DutyDepartmentName,
                                            DutyType = item.CategoryName,
                                            SigninDate = selectEnd.ToString("yyyy-MM-dd")
                                        });
                                    }
                                    break;
                                }
                            }

                        }
                    }

                }

            }

            pagination.records = obj.Count();
            obj = obj.OrderByDescending(x => x.SigninDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
            return obj;


        }


        /// <summary>
        /// 签到统计数据
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public object GetPageSafeProduceAndSigninList(Pagination pagination, string queryJson)
        {
            return service.GetPageSafeProduceAndSigninList(pagination, queryJson);
        }

        #endregion
        #region 数据操作

        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        public void operateSafeProduce(SafeProduceEntity entity)
        {
            service.operateSafeProduce(entity);
        }

        /// <summary>
        /// 保存修改现场终端功能踩点记录
        /// </summary>
        /// <param name="entity"></param>
        public void operateOnLocale(OnLocaleEntity entity)
        {
            service.operateOnLocale(entity);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyvalue"></param>
        public void removeSafeProduce(string keyvalue)
        {

            service.removeSafeProduce(keyvalue);
        }

        #endregion

    }
}
