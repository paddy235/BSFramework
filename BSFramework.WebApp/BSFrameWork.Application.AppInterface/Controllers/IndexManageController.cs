using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.SystemManage.ViewModel;
using BSFramework.Cache.Factory;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class IndexManageController : ApiController
    {
        /// <summary>
        /// 取终端指标
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetTerminalIndex([FromBody] JObject json)
        {
            try
            {
                AdminPrettyBLL prettyBLL = new AdminPrettyBLL();
                TerminalDataSetBLL terminalDataSetBLL = new TerminalDataSetBLL();
                IndexManageBLL indexManageBLL = new IndexManageBLL();
                string res = json.Value<string>("json");
                var dy = JsonConvert.DeserializeAnonymousType(res, new
                {
                    userId = string.Empty,
                    data = new
                    {
                        terminalType = string.Empty,// 0月  1季  2年
                        templet = 1
                    }
                });

                UserEntity user = new UserBLL().GetEntity(dy.userId);
                if (user != null)
                {
                    DepartmentBLL departmentBLL = new DepartmentBLL();

                    DepartmentEntity companyDept = departmentBLL.GetCompany(user.DepartmentId);
                    #region  旧代码查厂级的， 新需求要查当前用户所在部门的 包括省级
                    //try
                    //{
                    //    if (userdept.Nature.Contains("班组") || userdept.Nature.Contains("部门"))
                    //    {
                    //        DepartmentEntity department = cachedata.FirstOrDefault(x => x.DepartmentId == userdept.ParentId);
                    //        if (department.Nature != "厂级")
                    //        {
                    //            DepartmentEntity parentDept = cachedata.FirstOrDefault(x => x.DepartmentId == department.ParentId);
                    //            if (parentDept.Nature == "厂级")
                    //            {
                    //                deptid = parentDept.DepartmentId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            deptid = department.DepartmentId;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        deptid = userdept.DepartmentId;
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    return new { info = "查询失败：查找厂级指标配置失败", code = -1, data = new List<IndexManageModel>() };
                    //}


                    //List<TerminalDataSetEntity> entitys = terminalDataSetBLL.GetList().Where(p => p.IsOpen == 1).ToList(); //先取所有的指标
                    //                                                                                                       //再取所有的分类
                    //List<IndexManageEntity> manageEntities = indexManageBLL.GetList(deptid, 1).Where(p => p.IsShow == 1).ToList();
                    ////获取分类底下的指标
                    //List<IndexAssocationEntity> indexAssocationEntities = new IIndexAssocationBLL().GetListByTitleId(manageEntities.Select(p => p.Id).ToArray());
                    ////查询并组装指标值的数据
                    //AdminPrettyBLL adminPrettyBLL = new AdminPrettyBLL();
                    //List<KeyValue> keyValues = adminPrettyBLL.FindAllCount(user.UserId, user.DepartmentId, TerminalType);
                    //List<IndexManageModel> models = new List<IndexManageModel>();
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
                    #endregion
                    List<IndexManageModel> IndexData = prettyBLL.GetIndexData(companyDept.DepartmentId, user.UserId, (int)IndexType.安卓终端, user.DepartmentId, dy.data.terminalType, dy.data.templet);


                    return new { info = "成功", code = 0, data = IndexData, IndexData.Count };
                }
                else
                {
                    return new { info = "查询失败：用户不存在", code = -1, data = new List<IndexManageModel>() };
                }
            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = -1, data = new List<IndexManageModel>() };
            }

        }
        [HttpPost]
        /// <summary>
        /// 获取手机APP指标
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public object GetAppIndex([FromBody] JObject json)
        {
            try
            {
                AdminPrettyBLL prettyBLL = new AdminPrettyBLL();
                TerminalDataSetBLL terminalDataSetBLL = new TerminalDataSetBLL();
                IndexManageBLL indexManageBLL = new IndexManageBLL();
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userid;
                UserEntity user = new UserBLL().GetEntity(userId);
                if (user != null)
                {
                    DepartmentBLL departmentBLL = new DepartmentBLL();
                    var deptid = user.DepartmentId;
                    var cachedata = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);
                    if (cachedata == null)
                    {
                        cachedata = departmentBLL.GetList();
                    }
                    DepartmentEntity compnyDept = departmentBLL.GetCompany(deptid);
                    //try
                    //{
                    //    if (userdept.Nature.Contains("班组") || userdept.Nature.Contains("部门"))
                    //    {
                    //        DepartmentEntity department = cachedata.FirstOrDefault(x => x.DepartmentId == userdept.ParentId);
                    //        if (department.Nature != "厂级")
                    //        {
                    //            DepartmentEntity parentDept = cachedata.FirstOrDefault(x => x.DepartmentId == department.ParentId);
                    //            if (parentDept.Nature == "厂级")
                    //            {
                    //                deptid = parentDept.DepartmentId;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            deptid = department.DepartmentId;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        deptid = userdept.DepartmentId;
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    return new { info = "查询失败：查找厂级指标配置失败", code = -1, data = new List<IndexManageModel>() };
                    //}


                    List<TerminalDataSetEntity> entitys = terminalDataSetBLL.GetList().Where(p => p.IsOpen == 1).ToList(); //先取所有的指标
                                                                                                                           //再取所有的分类
                    List<IndexManageEntity> manageEntities = indexManageBLL.GetList(compnyDept.DepartmentId, (int)IndexType.手机APP).Where(p => p.IsShow == 1).ToList();//跟终端区别就是在这里
                    //获取分类底下的指标
                    List<IndexAssocationEntity> indexAssocationEntities = new IIndexAssocationBLL().GetListByTitleId(manageEntities.Select(p => p.Id).ToArray());
                    //查询授权的菜单，未绑定到菜单底下的指标不允许使用
                    //string rsp = terminalDataSetBLL.GetAuthMenuConfigList(user.UserId, 2);
                    //RspModel<List<MenuSettingData>> rspModel = JsonConvert.DeserializeObject<RspModel<List<MenuSettingData>>>(rsp);
                    //if (rspModel.Code != 0)//请求失败，清空所有指标
                    //{
                    //    entitys.Clear();
                    //}
                    //else
                    //{
                    //    List<ChildMenu> menuList = new List<ChildMenu>();
                    //    rspModel.Data.ForEach(p =>
                    //    {
                    //        if (p.HasChild)
                    //        {
                    //            menuList.AddRange(p.Child);
                    //        }
                    //    });
                    //    var menuIds = menuList.Select(p => p.ModuleId).ToList();
                    //    entitys = entitys.Where(p => menuIds.Contains(p.BindModuleId)).ToList();
                    //}
                    //剔除不在
                    //查询并组装指标值的数据
                    AdminPrettyBLL adminPrettyBLL = new AdminPrettyBLL();
                    UserEduncationFileBLL.manageindex index = new UserEduncationFileBLL().GetTotalWork(user.DepartmentId, user.UserId);
                    PropertyInfo[] properties = index.GetType().GetProperties();
                    //查询并组装指标值的数据
                    var dataSetIds = indexAssocationEntities.Select(x => x.DataSetId);//所有的指标的Ids
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    entitys.Where(x => dataSetIds.Contains(x.Id)).ToList().ForEach(x => {
                        dic.Add(x.Code+"-"+x.Name, "");
                    });//应该要绑定的所有的指标

                    List<KeyValue> keyValues =adminPrettyBLL. FindAllCount(userId, user.DepartmentId, dic);

                    //获取班组的之前的指标
                    UserEduncationFileBLL bll = new UserEduncationFileBLL();
                    var total = bll.GetTotalWork(user.DepartmentId, user.UserId);
                    PropertyInfo[] s = total.GetType().GetProperties();
                    foreach (var item in s)
                    {
                        keyValues.Add(new KeyValue()
                        {
                            key = item.Name,
                            value = item.GetValue(total, null) == null ? null : item.GetValue(total, null).ToString()
                        });
                    }
                    List<IndexManageModel> models = new List<IndexManageModel>();
                    if (manageEntities != null && manageEntities.Count > 0)
                    {
                        manageEntities.ForEach(title =>
                        {
                            //生成分类
                            IndexManageModel model = new IndexManageModel()
                            {
                                TitleId = title.Id,
                                TitleName = title.Title,
                                Srot = title.Sort
                            };
                            //生成指标
                            var datasetIds = indexAssocationEntities.Where(p => p.TitleId == title.Id).Select(p => p.DataSetId);
                            List<TerminalDataSetEntity> terminals = entitys.Where(x => datasetIds.Contains(x.Id)).ToList();
                            List<IndexModel> indexModels = new List<IndexModel>();
                            terminals.ForEach(terminal =>
                            {
                                //组装数据

                                IndexModel indexModel = new IndexModel()
                                {
                                    Key = terminal.Code,
                                    Name = terminal.Name,
                                    Sort = terminal.Sort,
                                    IsBZ = terminal.IsBZ,
                                    Unit = terminal.Unint,
                                    CustomCode = terminal.CustomCode,
                                    Icon = string.IsNullOrWhiteSpace(terminal.IconUrl) ? null : Config.GetValue("AppUrl") + terminal.IconUrl
                                    //Value = thisKv == null ? "0" : thisKv.value
                                };
                                if (terminal.IsBZ == "1")
                                {
                                    PropertyInfo pinfo = properties.FirstOrDefault(p => p.Name == terminal.CustomCode);

                                    //var thisKv = keyValues.FirstOrDefault(p => p.key == terminal.Code);
                                    indexModel.Value = pinfo == null ? "0" : pinfo.GetValue(index).ToString();
                                }
                                indexModels.Add(indexModel);
                            });
                            model.AddChilds(indexModels);
                            models.Add(model);
                        });
                    }
                    return new { info = "成功", code = 0, data = models, models.Count };
                }
                else
                {
                    return new { info = "查询失败：用户不存在", code = -1, data = new List<IndexManageModel>() };
                }
            }
            catch (Exception ex)
            {
                return new { info = "查询失败：" + ex.Message, code = -1, data = new List<IndexManageModel>() };
            }
        }

        #region 安卓终端  指标统计

        /// <summary>
        /// 获取任务统计
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<List<IndexKeyValue>> GetJobMonthCount(ParamBucket<string> obj)
        {
            try
            {
                IndexManageBLL indexManageBLL = new IndexManageBLL();
                var data = indexManageBLL.GetJobMonthCount(obj.Data);
                var list = data.Select(x => new IndexKeyValue() { name = x.Key, num = x.Value }).ToList();
                return new ModelBucket<List<IndexKeyValue>> { code = 0, info = "成功", Data = list };
            }
            catch (Exception ex)
            {

                return new ModelBucket<List<IndexKeyValue>> { code = -1, info = ex.Message };
            }

        }
        /// <summary>
        /// 教育培训统计
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<List<IndexKeyValue>> GetEdMonthCount(ParamBucket<string> obj)
        {
            try
            {
                IndexManageBLL indexManageBLL = new IndexManageBLL();
                var data = indexManageBLL.GetEdMonthCount(obj.Data);
                var list = data.Select(x => new IndexKeyValue() { name = x.Key, num = x.Value }).ToList();
                return new ModelBucket<List<IndexKeyValue>> { code = 0, info = "成功", Data = list };
            }
            catch (Exception ex)
            {

                return new ModelBucket<List<IndexKeyValue>> { code = -1, info = ex.Message };
            }

        }
        /// <summary>
        ///  班组活动统计
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<List<IndexKeyValue>> GetACMonthCount(ParamBucket<string> obj)
        {
            try
            {
                IndexManageBLL indexManageBLL = new IndexManageBLL();
                var data = indexManageBLL.GetACMonthCount(obj.Data);
                var list = data.Select(x => new IndexKeyValue() { name = x.Key, num = x.Value }).ToList();
                return new ModelBucket<List<IndexKeyValue>> { code = 0, info = "成功", Data = list };
            }
            catch (Exception ex)
            {

                return new ModelBucket<List<IndexKeyValue>> { code = -1, info = ex.Message };
            }

        }
        /// <summary>
        ///  人身风险预控统计
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<List<IndexKeyValue>> GetHdMonthCount(ParamBucket<string> obj)
        {
            try
            {
                IndexManageBLL indexManageBLL = new IndexManageBLL();
                var data = indexManageBLL.GetHdMonthCount(obj.Data);
                var list = data.Select(x => new IndexKeyValue() { name = x.Key, num = x.Value }).ToList();
                return new ModelBucket<List<IndexKeyValue>> { code = 0, info = "成功", Data = list };
            }
            catch (Exception ex)
            {

                return new ModelBucket<List<IndexKeyValue>> { code = -1, info = ex.Message };
            }

        }

        /// <summary>
        /// 危险预制训练
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<List<IndexKeyValue>> GetDMonthCount(ParamBucket<string> obj)
        {
            try
            {
                IndexManageBLL indexManageBLL = new IndexManageBLL();
                var data = indexManageBLL.GetDMonthCount(obj.Data);
                var list = data.Select(x => new IndexKeyValue() { name = x.Key, num = x.Value }).ToList();
                return new ModelBucket<List<IndexKeyValue>> { code = 0, info = "成功", Data = list };
            }
            catch (Exception ex)
            {

                return new ModelBucket<List<IndexKeyValue>> { code = -1, info = ex.Message };
            }

        }
        #endregion



    }
}