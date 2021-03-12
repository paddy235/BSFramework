using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.AttendManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.AttendManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class AttendController : ApiController
    {
        AttendBLL abll = new AttendBLL();
        SetAttendBLL sbll = new SetAttendBLL();
        UserBLL ubll = new UserBLL();

        /// <summary>
        /// 获取历史打卡记录
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetList([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                string from = dy.data.from;
                string to = dy.data.to;
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                IList list = abll.GetPageList(from, to, userId, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total).ToList();
                return new { code = 0, info = "获取数据成功", count = total, data = new { attends = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 检查考勤距离
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object CheckAttend([FromBody]JObject json)
        {
            try
            {
                var setl = sbll.GetList();

                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                double lat = dy.data.lat;
                double lng = dy.data.lng;
                
                if (setl.Count()>0)
                {
                   
                    double lat1 = Convert.ToDouble(setl.ToList()[0].Lat);
                    double lng1 = Convert.ToDouble(setl.ToList()[0].Lng);
                    double round1 = Convert.ToDouble(setl.ToList()[0].Area);
                    double r = GetDistance(lat, lng, lat1, lng1);
                    if (r < round1)
                    {
                        if (!dy.data.isCheck)
                        {
                            //保存
                            //string attendEntity = JsonConvert.SerializeObject(dy.data.attendEntity);
                            //AttendEntity entity = JsonConvert.DeserializeObject<AttendEntity>(attendEntity);
                            AttendEntity entity = new AttendEntity();
                            entity.ID = Guid.NewGuid().ToString();
                            entity.UserId = userId;
                            entity.UserName = dy.data.userName;
                            entity.Lat =Convert.ToString( lat);
                            entity.Lng = Convert.ToString(lng);
                            entity.AttendDate = DateTime.Now;
                            entity.BZId = user.DepartmentId;
                            abll.SaveForm(string.Empty, entity);
                        }
                        return new { code = 0, info = "打卡成功" };
                    }
                    else
                    {
                        return new { code = 1, info = "不在考勤范围内！" };
                    }
                }
                else 
                {
                    return new { code = 1, info = "尚未设置考勤地点！" };
                }
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
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
