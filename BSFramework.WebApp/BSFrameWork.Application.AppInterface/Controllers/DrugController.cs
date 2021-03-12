using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.DrugManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.DrugManage;
using BSFramework.Application.Service.DrugManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
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
    public class DrugController : BaseApiController
    {
        //
        // GET: /Drug/
        DrugBLL dbll = new DrugBLL();
        FileInfoBLL fileBll = new FileInfoBLL();
        /// <summary>
        /// 药品列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetDrugs()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                string name = dy.data.name;

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                List<DrugStockOutEntity> list = dbll.GetStockOutList(user.DepartmentId, "", "").Where(x => x.DrugName.ToLower().Contains(name)).Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Total < Convert.ToDecimal(list[i].Warn))
                    {
                        DrugStockOutEntity d = list[i];
                        list.Remove(d);
                        list.Insert(0, d);
                    }
                }
                var nlist = list.Select(t => new
                {
                    t.Id,
                    t.DrugLevel,
                    t.DrugLevelName,
                    t.DrugName,
                    t.Total,
                    t.OutTotal,
                    t.DrugUnit,
                    Path = getPath(t)
                }).ToList();
                return new { code = 0, info = "获取数据成功", count = total, data = new { drugList = nlist } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        public string getPath(DrugStockOutEntity item)
        {
            string path = "";
            var seed = Guid.NewGuid().GetHashCode();
            Random r = new Random(seed);
            string type = string.Empty;
            string s = string.Empty;
            int num = 0;
            if (item.DrugUnit == "g")
            {
                // num = r.Next(1, 3);
                num = 1;
            }
            else
            {
                // num = r.Next(1, 4);
                num = 3;
            }
            if (item.Total != 0)
            {
                if (item.Total / item.OutTotal >= Convert.ToDecimal(0.75))
                {

                    path = BSFramework.Util.Config.GetValue("AppUrl") + "/Content/styles/drugimg/4-4-" + num + item.DrugUnit + ".png";
                }
                if (item.Total / item.OutTotal > 0 && item.Total / item.OutTotal < Convert.ToDecimal(0.25))
                {
                    path = BSFramework.Util.Config.GetValue("AppUrl") + "/Content/styles/drugimg/1-4-" + num + item.DrugUnit + ".png";
                }
                if (item.Total / item.OutTotal >= Convert.ToDecimal(0.25) && item.Total / item.OutTotal < Convert.ToDecimal(0.5))
                {
                    path = BSFramework.Util.Config.GetValue("AppUrl") + "/Content/styles/drugimg/2-4-" + num + item.DrugUnit + ".png";
                }
                if (item.Total / item.OutTotal >= Convert.ToDecimal(0.5) && item.Total / item.OutTotal < Convert.ToDecimal(0.75))
                {
                    path = BSFramework.Util.Config.GetValue("AppUrl") + "/Content/styles/drugimg/3-4-" + num + item.DrugUnit + ".png";
                }

            }
            if (item.Total == 0 || item.OutTotal == 0)
            {
                path = BSFramework.Util.Config.GetValue("AppUrl") + "/Content/styles/drugimg/0-4-" + num + item.DrugUnit + ".png";
            }
            if (item.Total < Convert.ToDecimal(item.Warn))
            {
                if (item.DrugUnit == "ml")
                {
                    path = BSFramework.Util.Config.GetValue("AppUrl") + "/Content/styles/drugimg/1-4-1ml.png";
                }
                else
                {
                    path = BSFramework.Util.Config.GetValue("AppUrl") + "/Content/styles/drugimg/9-9-9g.png";
                }
            }
            return path;
        }


        [HttpPost]
        public object DrugOut()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string id = dy.data.tId;
                decimal outnum = Convert.ToDecimal(dy.data.outNum);
                string person = dy.data.guarDianName;
                string userId = dy.userId;
                string personId = "";
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                //更新出库信息
                DrugStockOutEntity dso = dbll.GetStockOutEntity(id);
                if (dso.Total == 0)
                {
                    return new { code = 1, info = "库存不足！" };
                }
                if (dso.Total < outnum)
                {
                    return new { code = 2, info = "取用两不能大于剩余量!" };
                }

                DrugOutEntity entity = new DrugOutEntity();
                entity.OutNum = outnum;
                entity.DrugId = id;
                entity.BZId = user.DepartmentId;
                entity.BZName = dept.FullName;
                entity.OutNum = outnum;
                entity.GuarDianName = person;
                entity.GuarDianId = personId;
                entity.CreateUserId = userId;
                entity.CreateUserName = user.RealName;
                dbll.SaveDrugOutNew(entity.Id, entity);

                return new { code = 0, info = "取用成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        #region 班组终端 药品管理
        /// <summary>
        /// 台帐管理-药品出入库历史记录
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetStocks()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;
                string from = dy.data.from;
                string to = dy.data.to;
                string name = dy.data.name;
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                if (!allowPaging) { pageIndex = 1; pageSize = 10000; }
                int total = 0;
                DateTime? f = null;
                if (!string.IsNullOrEmpty(from)) f = DateTime.Parse(from);
                DateTime? t = null;
                if (!string.IsNullOrEmpty(to))
                {
                    t = DateTime.Parse(to);
                    t = t.Value.AddDays(1);
                }

                List<DrugStockEntity> list = dbll.GetStockList(new string[] { user.DepartmentId }, f, t, name, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize), out total).ToList();
                //if (!string.IsNullOrEmpty(name))
                //{
                //    list = list.Where(x => x.DrugName.Contains(name)).ToList();
                //}
                //if (!string.IsNullOrEmpty(from))
                //{
                //    DateTime f = DateTime.Parse(from);
                //    list = list.Where(x => x.CreateDate >= f).ToList();
                //}
                //if (!string.IsNullOrEmpty(to))
                //{
                //    DateTime t = DateTime.Parse(to);

                //    t = t.AddDays(1);
                //    list = list.Where(x => x.CreateDate < t).ToList();
                //}
                list = list.Where(x => x.BZId == user.DepartmentId).ToList();
                total = list.Count();
                var nlist = list.Select(x => new
                {
                    x.DrugName,
                    x.DrugLevel,
                    DrugUSL = x.DrugUSL + x.DrugUnit + "/瓶",
                    Type = getStockType(x.Type),
                    x.DrugNum,
                    x.CreateUserName,
                    x.CreateDate,
                    x.StockNum,
                    x.BZName,
                    x.Monitor
                });
                return new { code = 0, info = "获取数据成功", count = total, data = nlist };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        public string getStockType(string type)
        {
            string val = "";
            if (type == "0") val = "入库";
            if (type == "1") val = "出库";
            return val;
        }
        /// <summary>
        /// 药品取用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object DrugOutNew()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string id = dy.data.tId;
                decimal outnum = Convert.ToDecimal(dy.data.outNum);
                string person = dy.data.guarDianName;
                string personId = "";
                string userId = dy.userId;

                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                //更新出库信息
                DrugStockOutEntity dso = dbll.GetStockOutEntity(id);
                if (dso.Total == 0)
                {
                    return new { code = 1, info = "库存不足！" };
                }
                if (dso.Total < outnum)
                {
                    return new { code = 2, info = "取用两不能大于剩余量!" };
                }

                DrugOutEntity entity = new DrugOutEntity();
                entity.BZId = user.DepartmentId;
                entity.BZName = dept.FullName;
                entity.OutNum = outnum;
                entity.GuarDianName = person;
                entity.GuarDianId = personId;
                entity.CreateUserId = userId;
                entity.CreateUserName = user.RealName;
                entity.DrugId = id;
                entity.Monitor = dy.data.monitor;
                dbll.SaveDrugOutNew(entity.Id, entity);

                return new { code = 0, info = "取用成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 药品管理-所有库存列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllDrugs()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;
                string userId = dy.userId;
                string name = dy.data;
                UserEntity user = new UserBLL().GetEntity(userId);
                int total = 0;
                List<DrugEntity> list = dbll.GetList(user.DepartmentId).ToList();
                list = list.Where(x => x.DrugName.Contains(name)).ToList();
                //根据名称和等级去重
                list = list.Distinct(new DrugCompareByNameAndLevel()).ToList();
                total = list.Count();
                foreach (DrugEntity d in list)
                {
                    d.Specs = new List<newDrug>();
                    d.DrugNum = 0;
                    var nlist = dbll.GetList(user.DepartmentId).Where(x => x.DrugLevel == d.DrugLevel && x.DrugName == d.DrugName).ToList();
                    foreach (DrugEntity o in nlist)
                    {
                        newDrug n = new newDrug();
                        n.Id = o.Id;
                        n.Name = o.DrugName;
                        n.Spec = o.Spec;
                        n.Level = o.DrugLevel;
                        n.DrugNum = o.DrugNum;
                        n.Unit = o.Unit;
                        n.Unit2 = o.Unit2;
                        n.Used = o.Used;
                        d.Specs.Add(n);
                        d.DrugNum += n.DrugNum;
                    }
                    var dslist = dbll.GetStockOutList(user.DepartmentId, d.DrugName, d.DrugLevel).ToList();
                    d.OutSurplus = 0;
                    foreach (DrugStockOutEntity ds in dslist)
                    {
                        d.OutSurplus += ds.Total;
                    }


                }
                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        //删除库存信息
        public object DeleteDrug()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                //DrugEntity drug = dbll.GetEntity(dy.data);
                //var dlist = dbll.GetList().Where(x => x.DrugName == drug.DrugName && x.DrugLevel == drug.DrugLevel).ToList();
                //var dslist = dbll.GetStockOutList(drug.DrugName, drug.DrugLevel);
                //var num = 0;
                //foreach (DrugEntity d in dlist) 
                //{
                //    num += d.DrugNum;
                //}
                //decimal total = 0;
                //foreach (DrugStockOutEntity ds in dslist) 
                //{
                //    total += ds.Total;
                //}
                //if()
                dbll.DelDrugNew(dy.data, user.DepartmentId);
                //var entity = pbll.GetEntity(userId);
                return new { code = 0, info = "删除数据成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        public object DeleteDrugStockOut()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                dbll.DelDrug(dy.data, user.DepartmentId);
                //var entity = pbll.GetEntity(userId);
                return new { code = 0, info = "删除数据成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 药品管理-取用历史记录
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetOuts()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;
                string userId = dy.userId;
                string from = dy.data.from;
                string to = dy.data.to;
                string name = dy.data.name;
                UserEntity user = new UserBLL().GetEntity(userId);
                if (!allowPaging) { pageIndex = 1; pageSize = 10000; }
                int total = 0;
                List<DrugOutEntity> list = dbll.GetOutList(user.DepartmentId, null, null, "", 1, 10000, out total).ToList();

                if (!string.IsNullOrEmpty(name))
                {
                    list = list.Where(x => x.DrugName.Contains(name)).ToList();
                }
                if (!string.IsNullOrEmpty(from))
                {
                    DateTime f = DateTime.Parse(from);
                    list = list.Where(x => x.CreateDate > f).ToList();
                }
                if (!string.IsNullOrEmpty(to))
                {
                    DateTime t = DateTime.Parse(to);

                    t = t.AddDays(1);
                    list = list.Where(x => x.CreateDate < t).ToList();
                }
                total = list.Count();
                list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        /// <summary>
        /// 药品管理-药品取用（获取已出库可取用的药品）
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetStockOuts()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;
                string name = dy.data;

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                if (!allowPaging) { pageIndex = 1; pageSize = 10000; }
                int total = 0;
                List<DrugStockOutEntity> list = dbll.GetStockOutList(user.DepartmentId, name, "").Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Total < Convert.ToDecimal(list[i].Warn))
                    {
                        DrugStockOutEntity d = list[i];
                        list.Remove(d);
                        list.Insert(0, d);
                    }
                }
                var nlist = list.Select(t => new
                {
                    t.Id,
                    t.DrugLevel,
                    t.DrugLevelName,
                    t.DrugName,
                    t.Total,
                    t.OutTotal,
                    t.DrugUnit,
                    t.Warn,
                    DrugType = getDrugType(t.DrugUnit),
                    Path = getPath(t)
                }).ToList();
                return new { code = 0, info = "获取数据成功", count = total, data = nlist };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        public string getDrugType(string unit)
        {
            var type = "";
            switch (unit)
            {
                case "g":
                    type = "1";
                    break;
                case "ml":
                    type = "2";
                    break;
                default:
                    break;
            }
            return type;
        }
        /// <summary>
        /// 药品管理-新增药品种类
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddDrug()
        {
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
            string userId = dy.userId;
            UserEntity user = new UserBLL().GetEntity(userId);
            var dept = new DepartmentBLL().GetEntity(user.DepartmentId);
            //新增药品种类
            DrugEntity d = new DrugEntity();
            d.DrugName = dy.data.drugName;
            d.DrugLevel = dy.data.drugLevel;
            d.DrugLevelName = dy.data.drugLevelName;
            int num = Convert.ToInt32(dy.data.drugNum);
            d.Spec = dy.data.spec;
            d.Unit = dy.data.unit;
            d.Location = dy.data.location;
            d.StockWarn = dy.data.stockWarn;
            d.CreateDate = DateTime.Now;
            d.CreateUserId = user.UserId;
            d.CreateUserName = user.RealName;
            d.Total = d.Surplus = 0;
            d.Id = Guid.NewGuid().ToString();
            d.BZId = user.DepartmentId;
            d.DrugInventoryId = dy.data.drugInventoryId;
            d.DrugNum = 0;
            d.Unit2 = dy.data.unit2;
            d.Used = 0;

            var dlist = dbll.GetList(user.DepartmentId).Where(x => x.DrugName == d.DrugName && x.DrugLevel == d.DrugLevel && x.Spec == d.Spec);
            if (dlist.Count() > 0)
            {
                return new { code = 0, info = "该药品已存在，请直接入库！" };
            }
            dbll.SaveDrug(d.Id, d);

            //数量大于0，则入库
            if (num > 0)
            {
                DrugStockEntity ds = new DrugStockEntity();
                ds.Id = Guid.NewGuid().ToString();
                ds.DrugId = d.Id;
                ds.DrugName = d.DrugName;
                ds.DrugNum = num;
                ds.DrugUSL = Convert.ToDecimal(d.Spec);
                ds.CreateUserId = user.UserId;
                ds.CreateUserName = user.RealName;
                ds.BZName = dept.FullName;
                dbll.SaveDrugStock(string.Empty, ds);
            }

            //更新所有同类药品预警值
            var list = dbll.GetList(user.DepartmentId).Where(x => x.DrugName == d.DrugName && x.DrugLevel == d.DrugLevel).ToList();
            foreach (DrugEntity entity in list)
            {
                d.StockWarn = d.StockWarn;
                dbll.SaveDrug(entity.Id, entity);
            }

            return new { code = 0, info = "新增成功" };

        }


        /// <summary>
        /// 药品管理-药品出库
        /// 根据药品名称和规格，查询id
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddDrugStockOut()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                //string drugname = dy.data.drugName;
                //string druglevel = dy.data.drguLevel;
                //string spec = dy.data.spec;
                //DrugEntity drug = dbll.GetList().Where(x => x.DrugName == drugname && x.DrugLevel == druglevel && x.Spec == spec).FirstOrDefault();
                string drugId = dy.data.drugId;
                DrugEntity drug = dbll.GetEntity(drugId);
                string drugid = drug.Id;

                int outnum = Convert.ToInt32(dy.data.outNum); //取用数量（瓶）
                string warn = dy.data.warn;//出库预警值

                DrugEntity obj = dbll.GetEntity(drugid);
                var num = obj.DrugNum;
                if (obj.Used > 0)
                    num--;

                if (num == 0)
                {
                    return new { code = 2, info = "库存不足" };
                }
                if (num < outnum)
                {
                    return new { code = 3, info = "出库数量不能大于库存数量" };
                }

                DrugStockEntity DrugOut = new DrugStockEntity();
                DrugOut.Id = Guid.NewGuid().ToString();
                DrugOut.DrugNum = outnum;
                DrugOut.DrugId = drugid;
                DrugOut.DrugUSL = Convert.ToDecimal(obj.Spec);
                DrugOut.BZName = dept.FullName;
                DrugOut.Monitor = dy.data.monitor;
                drug.Monitor = dy.data.monitor;
                //设置药品出库预警值（旧版）
                obj.Warn = warn.ToString();
                dbll.SaveDrug(drugid, obj);

                //更新同种类药品出库预警值
                var list = dbll.GetList(user.DepartmentId).Where(x => x.DrugName == obj.DrugName && x.DrugLevel == obj.DrugLevel).ToList();
                foreach (DrugEntity d in list)
                {
                    d.Warn = obj.Warn;
                    dbll.SaveDrug(d.Id, d);
                }
                DrugOut.CreateUserName = user.RealName;
                DrugOut.CreateUserId = user.UserId;

                dbll.SaveDrugOut(user.DepartmentId, "", DrugOut);


                return new { code = 0, info = "出库成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 库存取用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket AddDrugStockOut2(ParamBucket<DrugEntity> args)
        {
            var success = true;
            var message = string.Empty;
            var user = new UserBLL().GetEntity(args.UserId);
            var dept = new DepartmentBLL().GetEntity(user.DepartmentId);
            DrugEntity drug = dbll.GetEntity(args.Data.Id);
            if (drug.DrugNum <= 0)
            {
                success = false;
                message = "库存不足";

            }
            else
            {
                var left = drug.DrugNum * int.Parse(drug.Spec) - drug.Used;
                if (left < args.Data.Used)
                {
                    success = false;
                    message = "取用两不能大于剩余量";
                }
                else
                {
                    var entity = new DrugOutEntity()
                    {
                        Id = Guid.NewGuid().ToString(),
                        DrugName = drug.DrugName,
                        DrugId = drug.Id,
                        OutNum = (decimal)args.Data.Used,
                        DrugUnit = drug.Unit,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName,
                        CreateDate = DateTime.Now,
                        DrugLevel = drug.DrugLevel,
                        Surplus = 0,
                        GuarDianId = user.UserId,
                        GuarDianName = user.RealName,
                        BZId = dept.DepartmentId,
                        BZName = dept.FullName,
                        Monitor = args.Data.Monitor
                    };

                    dbll.AddDrugStockOut2(entity, args.Data.Used);
                }
            }
            return new ResultBucket() { Success = true };
        }

        /// <summary>
        /// 药品管理-药品入库
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddDrugStock()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                //string drugname = dy.data.drugName;
                //string druglevel = dy.data.drguLevel;
                //string spec = dy.data.spec;
                string drugId = dy.data.drugId;
                DrugEntity d = dbll.GetEntity(drugId);
                //DrugEntity d = dbll.GetList().Where(x => x.DrugName == drugname && x.DrugLevel == druglevel && x.Spec == spec).FirstOrDefault();

                string drugid = d.Id;
                int inNum = Convert.ToInt32(dy.data.inNum); //取用数量（瓶）
                string stockWarn = dy.data.stockWarn;//库存预警值
                string location = dy.data.location;

                DrugEntity obj = dbll.GetEntity(drugid);
                DrugStockEntity DrugStock = new DrugStockEntity();
                DrugStock.DrugNum = inNum;
                DrugStock.DrugId = obj.Id;
                DrugStock.CreateUserId = user.UserId;
                DrugStock.CreateUserName = user.RealName;
                DrugStock.DrugUSL = Convert.ToDecimal(obj.Spec);
                DrugStock.BZName = dept.FullName;
                DrugStock.Monitor = dy.data.monitor;

                obj.Location = location;
                dbll.SaveDrug(drugid, obj);
                dbll.SaveDrugStock("", DrugStock);


                return new { code = 0, info = "入库成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 获取等级规格单位等
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetSelect()
        {
            try
            {
                DataItemBLL ditem = new DataItemBLL();
                DataItemDetailBLL detail = new DataItemDetailBLL();
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string type = dy.data;
                var item = ditem.GetEntityByCode(type);
                List<DataItemDetailEntity> nlist = detail.GetList(item.ItemId).ToList();


                return new { code = 0, info = "成功", data = nlist };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        public object GetSpecByDrug()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                var user = OperatorProvider.Provider.Current();

                string id = dy.data;
                DrugEntity d = dbll.GetEntity(id);
                var list = dbll.GetList(user.DeptId).Where(x => x.DrugName == d.DrugName && x.DrugLevel == d.DrugLevel).ToList();
                List<string> specs = new List<string>();
                foreach (DrugEntity drug in list)
                {
                    specs.Add(drug.Spec);
                }


                return new { code = 0, info = "成功", data = specs };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        [HttpPost]
        public object GetDrugByName()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                string name = dy.data;
                List<DrugInventoryEntity> list = dbll.GetDrugInventoryList().ToList();
                list = list.Where(x => x.DrugName.Contains(name) && ((!string.IsNullOrEmpty(x.DeptCode)) && (user.DepartmentCode.StartsWith(x.DeptCode) || x.DeptCode.StartsWith(user.DepartmentCode)))).ToList();
                var nlist = list.Select(x => new
                {
                    x.Id,
                    x.DrugName
                });

                return new { code = 0, info = "成功", data = nlist, count = list.Count() };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        [HttpPost]
        public object GetDrugInventory()
        {
            var user = OperatorProvider.Provider.Current();

            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                FileInfoBLL fileBll = new FileInfoBLL();
                string id = dy.data.drugId;
                string type = dy.data.type;
                DrugInventoryEntity d = null;
                IList<DrugEntity> drugs = new List<DrugEntity>();
                if (type == "1")
                {
                    DrugEntity entity = dbll.GetEntity(id);
                    d = dbll.GetDrugInventoryEntity(entity.DrugInventoryId);
                }
                else if (type == "0")
                {
                    DrugStockOutEntity entity = dbll.GetStockOutEntity(id);
                    var dlist = dbll.GetList(user.DeptId).Where(x => x.DrugName == entity.DrugName && x.DrugLevel == entity.DrugLevel).ToList();
                    if (dlist.Count() > 0)
                    {
                        d = dbll.GetDrugInventoryEntity(dlist.FirstOrDefault().DrugInventoryId);
                    }
                    //if (entity != null)
                    //{
                    //    d = dbll.GetDrugInventoryList().Where(x => x.DrugName == entity.DrugName).FirstOrDefault();
                    //}
                    //else 
                    //{
                    //    return new { code = 0, info = "失败" };
                    //}
                }
                if (d != null)
                {
                    d.Files = fileBll.GetFilesByRecIdNew(d.Id).Where(x => x.Description == "2").ToList();
                    foreach (FileInfoEntity f in d.Files)
                    {
                        f.FilePath = BSFramework.Util.Config.GetValue("AppUrl") + f.FilePath.TrimStart('~');
                    }
                    var file = fileBll.GetFilesByRecIdNew(d.Id).Where(x => x.Description == "0").ToList().FirstOrDefault();
                    if (file != null)
                    {
                        string msds = BSFramework.Util.Config.GetValue("AppUrl") + file.FilePath.TrimStart('~');
                        var urlPDF = Config.GetValue("pdfview");
                        d.msds = urlPDF + file.FileId;
                    }
                    file = fileBll.GetFilesByRecIdNew(d.Id).Where(x => x.Description == "1").ToList().FirstOrDefault();
                    if (file != null)
                    {
                        d.video = BSFramework.Util.Config.GetValue("AppUrl") + file.FilePath.TrimStart('~');
                    }
                }
                return new { code = 0, info = "成功", data = d };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }



        [HttpPost]
        public object GetDrugDetail()
        {
            var user = OperatorProvider.Provider.Current();

            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                FileInfoBLL fileBll = new FileInfoBLL();
                string id = dy.data.drugId;
                string type = dy.data.type;

                string drugname = "";
                string druglevel = "";
                string warn = "";
                string stockwarn = "";
                string location = "";
                string total = "";
                string unit = "";
                IList<DrugEntity> drugs = new List<DrugEntity>();
                if (type == "1")
                {
                    DrugEntity entity = dbll.GetEntity(id);
                    drugs = dbll.GetList(user.DeptId).Where(x => x.DrugName == entity.DrugName && x.DrugLevel == entity.DrugLevel).ToList();
                    drugname = entity.DrugName;
                    druglevel = entity.DrugLevel;
                    stockwarn = entity.StockWarn.ToString();
                    location = entity.Location;
                    total = entity.Total.ToString();
                    unit = entity.Unit;
                }
                else if (type == "0")
                {
                    DrugStockOutEntity entity = dbll.GetStockOutEntity(id);
                    drugs = dbll.GetList(user.DeptId).Where(x => x.DrugName == entity.DrugName && x.DrugLevel == entity.DrugLevel).ToList();
                    drugname = entity.DrugName;
                    druglevel = entity.DrugLevel;
                    warn = entity.Warn.ToString();
                    unit = entity.DrugUnit;
                    total = entity.Total.ToString();
                }
                return new { code = 0, info = "成功", data = new { list = drugs, DrugName = drugname, DrugLevel = druglevel, StockWarn = stockwarn, Location = location, Warn = warn, Total = total, Unit = unit } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }



        [HttpPost]
        public object EditWarn()
        {
            var user = OperatorProvider.Provider.Current();

            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string type = dy.data.type;
                string id = dy.data.drugId;
                string warn = dy.data.warn;
                if (type == "0")
                {
                    DrugStockOutEntity d = dbll.GetStockOutEntity(id);
                    d.Warn = Convert.ToDecimal(warn);
                    dbll.SaveStockOut(d.Id, d);
                }
                else if (type == "1")
                {
                    DrugEntity model = dbll.GetEntity(id);
                    var list = dbll.GetList(user.DeptId).Where(x => x.DrugName == model.DrugName && x.DrugLevel == model.DrugLevel).ToList();
                    foreach (DrugEntity d in list)
                    {
                        d.StockWarn = warn;
                        dbll.SaveDrug(d.Id, d);
                    }
                }


                return new { code = 0, info = "成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        #endregion

        /// <summary>
        /// 玻璃器皿列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllGlass([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                UserBLL ubll = new UserBLL();
                DepartmentBLL dtbll = new DepartmentBLL();
                string userId = dy.userId;
                string name = dy.data;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                var list = dbll.GetGlassList().Where(x => x.BZId == user.DepartmentId && x.Name.Contains(name)).ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (int.Parse(list[i].Amount) < int.Parse(list[i].Warn))
                    {
                        GlassEntity d = list[i];
                        list.Remove(d);
                        list.Insert(0, d);
                    }
                }
                return new { code = 0, info = "获取数据成功", count = list.Count(), data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 化验仪器列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllInstrument([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                UserBLL ubll = new UserBLL();
                DepartmentBLL dtbll = new DepartmentBLL();
                string userId = dy.userId;
                string name = dy.data.name;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                var list = dbll.GetInstrumentList().Where(x => x.BZID == user.DepartmentId && x.Name.Contains(name)).ToList();
                return new { code = 0, info = "获取数据成功", count = list.Count(), data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 新增玻璃器皿
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object AddGlass()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                DepartmentEntity pdept = new DepartmentBLL().GetEntity(dept.ParentId);
                string glassEntity = JsonConvert.SerializeObject(dy.data);
                GlassEntity entity = JsonConvert.DeserializeObject<GlassEntity>(glassEntity);
                entity.ID = Guid.NewGuid().ToString();
                entity.CreateUserId = userId;
                entity.BZId = dept.DepartmentId;
                entity.DeptId = pdept.DepartmentId;
                entity.CreateDate = DateTime.Now;
                entity.CreateUserName = user.RealName;


                HttpFileCollection files = HttpContext.Current.Request.Files;

                if (files.Count > 0)
                {
                    FileInfoEntity fi = null;
                    HttpPostedFile hf = files[0];

                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/glassfile/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0,
                        Description = "info"
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    entity.Path = fi.FilePath.Replace("~", BSFramework.Util.Config.GetValue("AppUrl"));

                }
                string gid = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(entity.GlassWareId))
                {
                    entity.GlassWareId = gid;
                }
                dbll.SaveGlass(entity.ID, entity);
                var obj = dbll.GetDrugGlassWareEntity(entity.GlassWareId);
                if (obj == null)
                {
                    obj = new DrugGlassWareEntity();
                    obj.GlassWareId = entity.GlassWareId;
                    obj.GlassWareName = entity.Name;
                    obj.CreateUserId = entity.CreateUserId;
                    obj.CreateUserName = entity.CreateUserName;
                    obj.CreateDate = entity.CreateDate;
                    obj.BGImg = entity.Path;
                    obj.GlassWareType = "玻璃器皿";
                    dbll.SaveDrugGlassWare(obj.GlassWareId, obj);
                    var val = "";
                    if (obj.BGImg.Contains("Content")) val = "Content";
                    if (obj.BGImg.Contains("Resource")) val = "Resource";
                    if (!string.IsNullOrEmpty(obj.BGImg))
                    {
                        string ext = obj.BGImg.Substring(obj.BGImg.LastIndexOf("."));
                        string name = obj.BGImg.Substring(obj.BGImg.LastIndexOf("/"));

                        var fi = new FileInfoEntity
                        {
                            FileId = Guid.NewGuid().ToString(),
                            FolderId = obj.GlassWareId,
                            RecId = obj.GlassWareId,
                            FileName = name.Substring(1, name.Length - 1),
                            FilePath = "~/" + obj.BGImg.Substring(obj.BGImg.LastIndexOf(val)),
                            FileExtensions = ext,
                            FileType = ext.Substring(1, ext.Length - 1),
                            FileSize = "",
                            DeleteMark = 0,
                            Description = "4"
                        };
                        fileBll.SaveForm(fi);

                    }

                }
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 新增化验仪器
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object AddInstrument()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                DepartmentEntity pdept = new DepartmentBLL().GetEntity(dept.ParentId);
                string instrumentEntity = JsonConvert.SerializeObject(dy.data);
                InstrumentEntity entity = JsonConvert.DeserializeObject<InstrumentEntity>(instrumentEntity);
                entity.ID = Guid.NewGuid().ToString();
                entity.CreateUserId = userId;
                entity.BZID = dept.DepartmentId;
                entity.DeptId = pdept.DepartmentId;
                entity.CreateDate = DateTime.Now;
                entity.CreateUserName = user.RealName;


                FileInfoBLL fileBll = new FileInfoBLL();
                HttpFileCollection files = HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    FileInfoEntity fi = null;
                    HttpPostedFile hf = files[0];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/glassfile/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    entity.Path = fi.FilePath.Replace("~", BSFramework.Util.Config.GetValue("AppUrl"));
                }

                string gid = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(entity.GlassWareId))
                {
                    entity.GlassWareId = gid;
                }
                dbll.SaveInstrument(entity.ID, entity);

                var obj = dbll.GetDrugGlassWareEntity(entity.GlassWareId);
                if (obj == null)
                {
                    obj = new DrugGlassWareEntity();
                    obj.GlassWareId = entity.GlassWareId;
                    obj.GlassWareName = entity.Name;
                    obj.CreateUserId = entity.CreateUserId;
                    obj.CreateUserName = entity.CreateUserName;
                    obj.CreateDate = entity.CreateDate;
                    obj.BGImg = entity.Path;
                    obj.GlassWareType = "化验仪器";
                    dbll.SaveDrugGlassWare(obj.GlassWareId, obj);

                    if (!string.IsNullOrEmpty(obj.BGImg))
                    {
                        var val = "";
                        if (obj.BGImg.Contains("Content")) val = "Content";
                        if (obj.BGImg.Contains("Resource")) val = "Resource";
                        string ext = obj.BGImg.Substring(obj.BGImg.LastIndexOf("."));
                        string name = obj.BGImg.Substring(obj.BGImg.LastIndexOf("/"));

                        var fi = new FileInfoEntity
                        {
                            FileId = Guid.NewGuid().ToString(),
                            FolderId = obj.GlassWareId,
                            RecId = obj.GlassWareId,
                            FileName = name.Substring(1, name.Length - 1),
                            FilePath = "~/" + obj.BGImg.Substring(obj.BGImg.LastIndexOf(val)),
                            FileExtensions = ext,
                            FileType = ext.Substring(1, ext.Length - 1),
                            FileSize = "",
                            DeleteMark = 0,
                            Description = "4"
                        };
                        fileBll.SaveForm(fi);

                    }

                }
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        public object DeleteGlass()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                dbll.DelGlass(id);
                //var entity = pbll.GetEntity(userId);
                return new { code = 0, info = "删除数据成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        public object DeleteInstrument()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data;
                dbll.DelInstrument(dbll.GetInstrument(id));
                //var entity = pbll.GetEntity(userId);
                return new { code = 0, info = "删除数据成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 编辑玻璃器皿
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object EditGlass()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                DepartmentEntity pdept = new DepartmentBLL().GetEntity(dept.ParentId);
                string glassEntity = JsonConvert.SerializeObject(dy.data);
                GlassEntity entity = JsonConvert.DeserializeObject<GlassEntity>(glassEntity);


                if (string.IsNullOrEmpty(entity.Path))
                {
                    FileInfoBLL fileBll = new FileInfoBLL();
                    HttpFileCollection files = HttpContext.Current.Request.Files;
                    FileInfoEntity fi = null;
                    HttpPostedFile hf = files[0];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/glassfile/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    entity.Path = fi.FilePath.Replace("~", BSFramework.Util.Config.GetValue("AppUrl"));
                }
                dbll.SaveGlass(entity.ID, entity);
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 编辑化验仪器
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object EditInstrument()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                DepartmentEntity pdept = new DepartmentBLL().GetEntity(dept.ParentId);
                string instrumentEntity = JsonConvert.SerializeObject(dy.data);
                InstrumentEntity entity = JsonConvert.DeserializeObject<InstrumentEntity>(instrumentEntity);


                if (string.IsNullOrEmpty(entity.Path))
                {
                    FileInfoBLL fileBll = new FileInfoBLL();
                    HttpFileCollection files = HttpContext.Current.Request.Files;
                    FileInfoEntity fi = null;
                    HttpPostedFile hf = files[0];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/glassfile/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    entity.Path = fi.FilePath.Replace("~", BSFramework.Util.Config.GetValue("AppUrl"));
                }
                dbll.SaveInstrument(entity.ID, entity);
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取玻璃器皿详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetGlassInfo()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                DepartmentEntity pdept = new DepartmentBLL().GetEntity(dept.ParentId);
                string id = dy.data;
                GlassEntity entity = dbll.GetGlass(id);
                DrugGlassWareEntity gw = dbll.GetDrugGlassWareEntity(entity.GlassWareId);


                return new { code = 0, info = "成功", data = new { glass = entity, glassware = gw } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        public object GetGlassWareInfo()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string id = dy.data;
                string glasswareid = "";
                //GlassEntity g = dbll.GetGlass(id);
                //if (g != null) glasswareid = g.GlassWareId;
                //else {
                //    InstrumentEntity i = dbll.GetInstrument(id);
                //    glasswareid = i.GlassWareId;
                //}
                DrugGlassWareEntity gw = dbll.GetDrugGlassWareEntity(id);


                return new { code = 0, info = "成功", data = gw };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取化验仪器详情
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GetInstrumentInfo()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                DepartmentEntity pdept = new DepartmentBLL().GetEntity(dept.ParentId);
                string id = dy.data;
                InstrumentEntity entity = dbll.GetInstrument(id);
                DrugGlassWareEntity gw = dbll.GetDrugGlassWareEntity(entity.GlassWareId);
                //var blist = dbll.GetInstrumentBDList().Where(x => x.InstrumentId == entity.ID);
                //if (blist.Count() > 0) entity.CheckResult = "1";

                return new { code = 0, info = "成功", data = new { glass = entity, glassware = gw } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 入库
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GlassIn()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                string id = dy.data.glassId;
                string amount = dy.data.amount;
                string location = dy.data.location;
                string warn = dy.data.warn;
                var g = dbll.GetGlass(id);
                g.Amount = (int.Parse(g.Amount) + int.Parse(amount)).ToString();
                g.Location = location;
                g.Warn = warn;
                dbll.SaveGlass(id, g);
                //保存入库信息
                GlassStockEntity t = new GlassStockEntity();
                t.Name = g.Name;
                t.Spec = g.Spec;
                t.Remark = user.RealName;
                t.BZId = user.DepartmentId;
                t.Amount = amount;
                t.CreateDate = DateTime.Now;
                t.CreateUserId = user.UserId;
                t.GlassId = id;
                t.CurrentNumber = int.Parse(g.Amount);
                t.ID = Guid.NewGuid().ToString();
                t.Type = "in";
                dbll.SaveGlassStock(string.Empty, t);
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 玻璃器皿入库损耗台账
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllGlassStock([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;

                UserBLL ubll = new UserBLL();
                DepartmentBLL dtbll = new DepartmentBLL();
                string userId = dy.userId;
                string name = dy.data.name;
                string from = dy.data.from;
                string to = dy.data.to;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                int total = 0;
                var dfrom = string.IsNullOrEmpty(from) ? null : (DateTime?)DateTime.Parse(from);
                var dto = string.IsNullOrEmpty(to) ? null : (DateTime?)DateTime.Parse(to);
                var list = dbll.GetGlassStockPageList(dfrom, dto, name, user.DepartmentId, "", int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total);
                return new { code = 0, info = "获取数据成功", count = list.Count(), data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 损耗
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object GlassOut()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                string id = dy.data.glassId;
                string amount = dy.data.amount;
                string location = dy.data.location;
                string reason = dy.data.reason;

                var g = dbll.GetGlass(id);
                g.Amount = (int.Parse(g.Amount) - int.Parse(amount)).ToString();
                g.Location = location;
                dbll.SaveGlass(id, g);

                GlassStockEntity t = new GlassStockEntity();
                t.Name = g.Name;
                t.Spec = g.Spec;
                t.Remark = user.RealName;
                t.BZId = user.DepartmentId;
                t.Amount = amount;
                t.CreateDate = DateTime.Now;
                t.CreateUserId = user.UserId;
                t.GlassId = id;
                t.CurrentNumber = int.Parse(g.Amount);
                t.ID = Guid.NewGuid().ToString();
                t.Type = "out";
                t.Reason = reason;
                dbll.SaveGlassStock(string.Empty, t);
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 新增仪器标定
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object AddInstrumentBD()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                DepartmentEntity pdept = new DepartmentBLL().GetEntity(dept.ParentId);
                string bdEntity = JsonConvert.SerializeObject(dy.data);
                InstrumentBDEntity entity = JsonConvert.DeserializeObject<InstrumentBDEntity>(bdEntity);
                entity.ID = Guid.NewGuid().ToString();
                entity.CreateUserId = userId;
                entity.BZID = dept.DepartmentId;
                entity.DeptId = pdept.DepartmentId;
                entity.CreateDate = DateTime.Now;
                var ins = dbll.GetInstrument(entity.InstrumentId);
                entity.InstrumentName = ins.Name;

                ins.BDValidate = entity.BDValidate;
                ins.BDResult = entity.BDResult;
                dbll.SaveInstrument(ins.ID, ins);
                //FileInfoBLL fileBll = new FileInfoBLL();
                //HttpFileCollection files = HttpContext.Current.Request.Files;
                //FileInfoEntity fi = null;
                //HttpPostedFile hf = files[0];
                //string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                //string fileId = Guid.NewGuid().ToString();//上传后文件名
                //fi = new FileInfoEntity
                //{
                //    FileId = fileId,
                //    FolderId = entity.ID,
                //    RecId = entity.ID,
                //    FileName = System.IO.Path.GetFileName(hf.FileName),
                //    FilePath = "~/Resource/AppFile/glassfile/" + fileId + ext,
                //    FileType = System.IO.Path.GetExtension(hf.FileName),
                //    FileExtensions = ext,
                //    FileSize = hf.ContentLength.ToString(),
                //    DeleteMark = 0
                //};

                ////上传附件到服务器
                //if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile"))
                //{
                //    System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile");
                //}
                //hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile\\" + fileId + ext);
                ////保存附件信息
                //fileBll.SaveForm(fi);
                //entity.Path = fi.FilePath.Replace("~", BSFramework.Util.Config.GetValue("AppUrl"));

                dbll.SaveInstrumentBD(entity.ID, entity);
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 新增仪器检验
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object AddInstrumentCheck()
        {
            try
            {

                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                DepartmentEntity dept = new DepartmentBLL().GetEntity(user.DepartmentId);
                DepartmentEntity pdept = new DepartmentBLL().GetEntity(dept.ParentId);
                string checkEntity = JsonConvert.SerializeObject(dy.data);
                InstrumentCheckEntity entity = JsonConvert.DeserializeObject<InstrumentCheckEntity>(checkEntity);
                entity.ID = Guid.NewGuid().ToString();
                entity.CreateUserId = userId;
                entity.CreateUserName = user.RealName;
                entity.BZID = dept.DepartmentId;
                entity.DeptId = pdept.DepartmentId;
                entity.CreateDate = DateTime.Now;
                var ins = dbll.GetInstrument(entity.InstrumentId);
                entity.InstrumentName = ins.Name;
                ins.Validate = entity.CheckValidate;
                ins.CheckResult = entity.CheckResult;
                //更新有效期
                //if (ins.Cycle.Contains("月")) { 
                //    int month = int.Parse(ins.Cycle.Replace("个月",""));
                //    ins.Validate=entity.CheckDate.AddMonths(month);
                //}
                //else if (ins.Cycle.Contains("年")) 
                //{
                //    int year = int.Parse(ins.Cycle.Replace("年", ""));
                //    ins.Validate = entity.CheckDate.AddYears(year);
                //}
                dbll.SaveInstrument(ins.ID, ins);
                FileInfoBLL fileBll = new FileInfoBLL();
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                if (files.Count > 0)
                {
                    HttpPostedFile hf = files[0];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/glassfile/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\glassfile\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    //entity.Path = fi.FilePath.Replace("~", BSFramework.Util.Config.GetValue("AppUrl"));
                }
                dbll.SaveInstrumentCheck(entity.ID, entity);
                return new { code = 0, info = "成功", data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 标定台账
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllInstrumentBD([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;
                string path = BSFramework.Util.Config.GetValue("AppUrl");
                UserBLL ubll = new UserBLL();
                DepartmentBLL dtbll = new DepartmentBLL();
                string userId = dy.userId;
                string name = dy.data.name;
                string from = dy.data.from;
                string to = dy.data.to;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                var list = dbll.GetInstrumentBDList().Where(x => x.BZID == dept.DepartmentId && x.InstrumentName.Contains(name)).OrderByDescending(x => x.CreateDate).ToList();
                foreach (InstrumentBDEntity i in list)
                {
                    i.Files = fileBll.GetFilesByRecIdNew(i.ID);
                    foreach (FileInfoEntity f in i.Files)
                    {
                        f.FilePath = f.FilePath.Replace("~", path);
                    }
                }
                if (!string.IsNullOrEmpty(from))
                {
                    list = list.Where(x => x.BDDate >= DateTime.Parse(from)).ToList();
                }
                if (!string.IsNullOrEmpty(to))
                {
                    list = list.Where(x => x.BDDate < DateTime.Parse(from).AddDays(1)).ToList();
                }
                int total = list.Count();
                if (allowPaging)
                {
                    list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).OrderByDescending(x => x.CreateDate).ToList();
                }
                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 检验台账
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllInstrumentCheck([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                long pageIndex = dy.pageIndex;//当前索引页
                long pageSize = dy.pageSize;//每页记录数
                bool allowPaging = dy.allowPaging;
                string path = BSFramework.Util.Config.GetValue("AppUrl");
                UserBLL ubll = new UserBLL();
                DepartmentBLL dtbll = new DepartmentBLL();
                string userId = dy.userId;
                string name = dy.data.name;
                string from = dy.data.from;
                string to = dy.data.to;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                int total = 0;
                var list = dbll.GetInstrumentCheckList().Where(x => x.BZID == dept.DepartmentId && x.InstrumentName.Contains(name)).OrderByDescending(x => x.CreateDate).ToList();
                foreach (InstrumentCheckEntity i in list)
                {
                    i.Files = fileBll.GetFilesByRecIdNew(i.ID);
                    foreach (FileInfoEntity f in i.Files)
                    {
                        f.FilePath = f.FilePath.Replace("~", path);
                    }
                    //if (f != null)
                    //{
                    //    i.Path = f.FilePath.Replace("~", path);
                    //}
                }
                if (!string.IsNullOrEmpty(from))
                {
                    list = list.Where(x => x.CheckDate >= DateTime.Parse(from)).ToList();
                }
                if (!string.IsNullOrEmpty(to))
                {
                    list = list.Where(x => x.CheckDate < DateTime.Parse(to).AddDays(1)).ToList();
                }

                total = list.Count();
                if (allowPaging)
                {
                    list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).OrderByDescending(x => x.CreateDate).ToList();
                }
                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 名称查询
        /// type 化验仪器/玻璃器皿
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetGlassWareByType([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                UserBLL ubll = new UserBLL();
                DepartmentBLL dtbll = new DepartmentBLL();
                string userId = dy.userId;
                string type = dy.data;
                UserEntity user = ubll.GetEntity(userId);
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);
                var list = dbll.GetDrugGlassWareList().Where(x => x.GlassWareType == type).Select(x => new
                {
                    x.GlassWareId,
                    x.GlassWareName,
                    x.BGImg
                });
                //int total = list.Count();
                //list = list.Skip(Convert.ToInt32(pageSize) * (Convert.ToInt32(pageIndex) - 1)).Take(Convert.ToInt32(pageSize)).ToList();
                return new { code = 0, info = "获取数据成功", count = list.Count(), data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 根据规格取余量
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<float> GetDrugLeft(ParamBucket<DrugEntity> args)
        {
            var user = OperatorProvider.Provider.Current();

            var drug = dbll.GetList(user.DeptId).FirstOrDefault(x => x.DrugName == args.Data.DrugName && x.DrugLevelName == args.Data.DrugLevelName && x.Spec == args.Data.Spec);
            if (drug == null) return new ModelBucket<float>() { Success = true, Data = 0 };
            else return new ModelBucket<float>() { Success = true, Data = int.Parse(drug.Spec) * drug.DrugNum };
        }

        [HttpPost]
        public ResultBucket Update(ListParam<DrugEntity> args)
        {
            var success = true;
            var message = string.Empty;
            try
            {

                new DrugBLL().Update(args.Data);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return new ResultBucket() { Success = success, Message = message };
        }
    }

    public class DrugCompareByNameAndLevel : IEqualityComparer<DrugEntity>
    {
        public bool Equals(DrugEntity x, DrugEntity y)
        {
            if (x == null || y == null)
            {
                return false;
            }
            if (x.DrugName == y.DrugName && x.DrugLevel == y.DrugLevel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(DrugEntity obj)
        {
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return obj.DrugName.GetHashCode() ^ obj.DrugLevel.GetHashCode();
            }
        }
    }
}
