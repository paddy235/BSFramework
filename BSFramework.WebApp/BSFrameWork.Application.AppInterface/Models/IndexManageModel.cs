using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BSFramework.Application.Entity.SystemManage;

namespace BSFrameWork.Application.AppInterface.Models
{

    #region 菜单
    public class MenuSettingData
    {
        public MenuSettingData()
        {
            this.HasChild = false;
            Child = new List<ChildMenu>();
        }
        public string Id { get; set; }
        public int? Sort { get; set; }
        public string Remark { get; set; }
        public string Name { get; set; }
        public List<ChildMenu> Child { get; set; }
        public bool HasChild { get; set; }
        public string Icon { get; set; }

    }

    public class ChildMenu
    {
        public int? Sort { get; set; }
        public string ModuleName { get; set; }
        public string ModuleId { get; set; }
        public string Remark { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public string MenuIcon { get; set; }

        public string ModuleCode { get; set; }

    }
    #endregion
    public class RspModel<T>
    {
        public int Code { get; set; }
        public string Info { get; set; }
        public T Data { get; set; }
    }

    public class IndexKeyValue
    {
        public string name { get; set; }
        public int num { get; set; }
    }
}