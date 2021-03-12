using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Entity.BaseManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.App_Code
{
    public class SystemRuntime
    {
        public static string GetCompanyName(string userid)
        {
            var name = HttpRuntime.Cache.Get(userid + "/Company");
            if (null == name)
            {
                name = new Busines.BaseManage.DepartmentBLL().GetFactory(userid);
                HttpRuntime.Cache.Insert(userid + "/Company", name);
            }

            return name.ToString();
        }

        public static string GetBzType(string deptid)
        {
            var dept = new Busines.BaseManage.DepartmentBLL().GetEntity(deptid);
            return dept.TeamType;
        }

        public static UserEntity GetUser(string userid)
        {
            return new UserBLL().GetEntity(userid);
        }
    }
}