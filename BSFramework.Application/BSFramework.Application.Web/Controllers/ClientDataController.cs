using BSFramework.Application.Busines.AuthorizeManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.SystemManage.ViewModel;
using BSFramework.Busines.AuthorizeManage;
using BSFramework.Util;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Controllers
{
    /// <summary>
    /// 描 述：客户端数据
    /// </summary>
    public class ClientDataController : MvcControllerBase
    {
        //private DataItemCache dataItemCache = new DataItemCache();
        //private OrganizeCache organizeCache = new OrganizeCache();
        //private DepartmentCache departmentCache = new DepartmentCache();
        //private PostCache postCache = new PostCache();
        //private RoleCache roleCache = new RoleCache();
        //private UserGroupCache userGroupCache = new UserGroupCache();
        //private UserCache userCache = new UserCache();
        private AuthorizeBLL authorizeBLL = new AuthorizeBLL();
        private ModuleBLL moduleBLL = new ModuleBLL();
        private DepartmentBLL departmentbll = new DepartmentBLL();
        private UserGroupBLL usergroupbll = new UserGroupBLL();
        private UserBLL userbll = new UserBLL();
        #region 获取数据
        /// <summary>
        /// 批量加载数据给客户端（把常用数据全部加载到浏览器中 这样能够减少数据库交互）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult GetClientDataJson()
        {
            var user = OperatorProvider.Provider.Current();

            var jsonData = new
            {
                organize = this.GetOrganizeData(),            //公司
                //department = this.GetDepartmentData(),          //部门
                // post = this.GetPostData(),                      //岗位
                // role = this.GetRoleData(),                      //角色
                //userGroup = this.GetUserGroupData(),            //用户组
                //user = this.GetUserData(),                      //用户
                //dataItem = this.GetDataItem(),                  //字典
                authorizeMenu = authorizeBLL.GetModuleList(user.UserId),         //导航菜单
                authorizeButton = this.GetModuleButtonData(),   //功能按钮
                //authorizeColumn = this.GetModuleColumnData(),   //功能视图
            };
            return ToJsonResult(jsonData);
        }
        #endregion
        /// <summary>
        /// 获取用户功能菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult GetMenuDataJson()
        {
            return Json(this.GetModuleData());
        }

        #region 处理基础数据
        /// <summary>
        /// 获取公司数据
        /// </summary>
        /// <returns></returns>
        private object GetOrganizeData()
        {
            var data = new OrganizeBLL().GetList();//organizeCache.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (OrganizeEntity item in data)
            {
                var fieldItem = new
                {
                    EnCode = item.EnCode,
                    FullName = item.FullName
                };
                dictionary.Add(item.OrganizeId, fieldItem);
            }
            return dictionary;
        }
        /// <summary>
        /// 获取部门数据
        /// </summary>
        /// <returns></returns>
        private object GetDepartmentData()
        {
            var data = departmentbll.GetList();//departmentCache.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (DepartmentEntity item in data)
            {
                var fieldItem = new
                {
                    EnCode = item.EnCode,
                    FullName = item.FullName,
                    OrganizeId = item.OrganizeId
                };
                dictionary.Add(item.DepartmentId, fieldItem);
            }
            return dictionary;
        }
        /// <summary>
        /// 获取岗位数据
        /// </summary>
        /// <returns></returns>
        private object GetUserGroupData()
        {
            var data = usergroupbll.GetList();//userGroupCache.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (RoleEntity item in data)
            {
                var fieldItem = new
                {
                    EnCode = item.EnCode,
                    FullName = item.FullName
                };
                dictionary.Add(item.RoleId, fieldItem);
            }
            return dictionary;
        }
        /// <summary>
        /// 获取岗位数据
        /// </summary>
        /// <returns></returns>
        private object GetPostData()
        {
            var data = new PostBLL().GetList();//postCache.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (RoleEntity item in data)
            {
                var fieldItem = new
                {
                    EnCode = item.EnCode,
                    FullName = item.FullName
                };
                dictionary.Add(item.RoleId, fieldItem);
            }
            return dictionary;
        }
        /// <summary>
        /// 获取角色数据
        /// </summary>
        /// <returns></returns>
        private object GetRoleData()
        {
            var data = new RoleBLL().GetList();//roleCache.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (RoleEntity item in data)
            {
                var fieldItem = new
                {
                    EnCode = item.EnCode,
                    FullName = item.FullName
                };
                dictionary.Add(item.RoleId, fieldItem);
            }
            return dictionary;
        }
        /// <summary>
        /// 获取用户数据
        /// </summary>
        /// <returns></returns>
        private object GetUserData()
        {
            var data = userbll.GetList();//userCache.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (UserEntity item in data)
            {
                var fieldItem = new
                {
                    EnCode = item.EnCode,
                    Account = item.Account,
                    RealName = item.RealName,
                    OrganizeId = item.OrganizeId,
                    DepartmentId = item.DepartmentId,
                    Photo = item.HeadIcon
                };
                dictionary.Add(item.UserId, fieldItem);
            }
            return dictionary;
        }
        /// <summary>
        /// 获取数据字典
        /// </summary>
        /// <returns></returns>
        private object GetDataItem()
        {
            var dataList = new DataItemDetailBLL().GetDataItemList();//dataItemCache.GetDataItemList();
            var dataSort = dataList.Distinct(new Comparint<DataItemModel>("EnCode"));
            Dictionary<string, object> dictionarySort = new Dictionary<string, object>();
            foreach (DataItemModel itemSort in dataSort)
            {
                var dataItemList = dataList.Where(t => t.EnCode.Equals(itemSort.EnCode));
                Dictionary<string, string> dictionaryItemList = new Dictionary<string, string>();
                foreach (DataItemModel itemList in dataItemList)
                {
                    dictionaryItemList.Add(itemList.ItemValue, itemList.ItemName);
                }
                foreach (DataItemModel itemList in dataItemList)
                {
                    dictionaryItemList.Add(itemList.ItemDetailId, itemList.ItemName);
                }
                dictionarySort.Add(itemSort.EnCode, dictionaryItemList);
            }
            return dictionarySort;
        }
        #endregion

        #region 处理授权数据
        /// <summary>
        /// 获取功能数据
        /// </summary>
        /// <returns></returns>
        private object GetModuleData()
        {
            return authorizeBLL.GetModuleList(SystemInfo.CurrentUserId);
        }
        /// <summary>
        /// 获取功能按钮数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<ModuleButtonEntity>> GetModuleButtonData()
        {
            var data = authorizeBLL.GetModuleButtonList(SystemInfo.CurrentUserId);
            return data.Where(x => !string.IsNullOrEmpty(x.ModuleId)).GroupBy(x => x.ModuleId).ToDictionary(x => x.Key, y => y.ToList());
            //var dataModule = data.Distinct(new Comparint<ModuleButtonEntity>("ModuleId"));
            //Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //foreach (ModuleButtonEntity item in dataModule)
            //{
            //    if (!string.IsNullOrEmpty(item.ModuleId))
            //    {
            //        var buttonList = data.Where(t => t.ModuleId.Equals(item.ModuleId));
            //        dictionary.Add(item.ModuleId, buttonList);
            //    }
            //}
            //return dictionary;

            //var data = authorizeBLL.GetModuleButtonListByUserId(SystemInfo.CurrentUserId);
            //var dataModule = moduleBLL.GetModuleIds();
            //Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //foreach(DataRow item in dataModule.Rows)
            //{
            //    string moduleId = item[0].ToString();
            //    if (!string.IsNullOrEmpty(moduleId))
            //    {
            //        var buttonList = data.Select("ModuleId='" + moduleId + "'");
            //        dictionary.Add(moduleId, buttonList.ToList());
            //    }
            //}
            //return dictionary;
        }
        /// <summary>
        /// 获取功能视图数据
        /// </summary>
        /// <returns></returns>
        private object GetModuleColumnData()
        {
            var data = authorizeBLL.GetModuleColumnList(SystemInfo.CurrentUserId);
            var dataModule = data.Distinct(new Comparint<ModuleColumnEntity>("ModuleId"));
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (ModuleColumnEntity item in dataModule)
            {
                var columnList = data.Where(t => t.ModuleId.Equals(item.ModuleId));
                dictionary.Add(item.ModuleId, columnList);
            }
            return dictionary;
        }
        #endregion
    }
}
