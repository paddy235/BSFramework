using BSFramework.Application.Entity.MessageManage;
using BSFramework.Application.IService.MessageManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace BSFramework.Application.Service.MessageManage
{
    /// <summary>
    /// 描 述：即时通信用户管理
    /// </summary>
    public class IMUserService : RepositoryFactory, IMsgUserService
    {
        /// <summary>
        /// 获取联系人列表（即时通信）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMUserModel> GetList(string OrganizeId)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT  u.UserId ,
                                    u.RealName ,
                                    o.OrganizeId AS OrganizeId ,
                                    d.DepartmentId AS DepartmentId ,
                                    o.FullName AS OrganizeName ,
                                    d.FullName AS DepartmentName ,
                                    u.HeadIcon  
                            FROM    Base_User u
                                    LEFT JOIN Base_Organize o ON o.OrganizeId = u.OrganizeId
                                    LEFT JOIN Base_Department d ON d.DepartmentId = u.DepartmentId
                            WHERE   1=1");
            var parameter = new List<DbParameter>();
            //公司主键
            if (!OrganizeId.IsEmpty())
            {
                strSql.Append(" AND u.OrganizeId = @OrganizeId");
                parameter.Add(DbParameters.CreateDbParameter("@OrganizeId", OrganizeId));
            }
            //strSql.Append(" AND u.UserId <> 'System'");
            strSql.Append(" order by d.FullName");
            return this.BaseRepository().FindList<IMUserModel>(strSql.ToString(), parameter.ToArray());
        }
        public IEnumerable<IMUserInfoEntity> GetUserList(string OrganizeId,string userId="")
        {
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT  u.UserId as id ,
                                    u.RealName username ,
                                    '' as sign,
                                    d.DepartmentId AS deptid ,
                                    case when userid='"+userId+"' then 'http://10.36.0.170/ERCHTMS/Content/images/on-line.png' else 'http://10.36.0.170/ERCHTMS/Content/images/off-line.png' end avatar FROM  Base_User u LEFT JOIN Base_Organize o ON o.OrganizeId = u.OrganizeId LEFT JOIN Base_Department d ON d.DepartmentId = u.DepartmentId WHERE   1=1");
            var parameter = new List<DbParameter>();
            //公司主键
            if (!OrganizeId.IsEmpty())
            {
                strSql.Append(" AND u.OrganizeId = @OrganizeId");
                parameter.Add(DbParameters.CreateDbParameter("@OrganizeId", OrganizeId));
            }
            return this.BaseRepository().FindList<IMUserInfoEntity>(strSql.ToString(), parameter.ToArray());
        }
        public DataTable GetUsers(string OrganizeId)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT  u.UserId ,
                                    u.RealName ,
                                    o.OrganizeId AS OrganizeId ,
                                    d.DepartmentId AS DepartmentId ,
                                    o.FullName AS OrganizeName ,
                                    d.FullName AS DepartmentName ,
                                    u.HeadIcon  
                            FROM    Base_User u
                                    LEFT JOIN Base_Organize o ON o.OrganizeId = u.OrganizeId
                                    LEFT JOIN Base_Department d ON d.DepartmentId = u.DepartmentId
                            WHERE   1=1");
            var parameter = new List<DbParameter>();
            //公司主键
            if (!OrganizeId.IsEmpty())
            {
                strSql.Append(" AND u.OrganizeId = @OrganizeId");
                parameter.Add(DbParameters.CreateDbParameter("@OrganizeId", OrganizeId));
            }
            return this.BaseRepository().FindTable(strSql.ToString(), parameter.ToArray());
        }
        public DataTable GetDepts(string OrganizeId)
        {
            var strSql = new StringBuilder();
            strSql.Append(@"select t.departmentid,t.fullname from BASE_DEPARTMENT t where 1=1 ");
            var parameter = new List<DbParameter>();
            //公司主键
            if (!OrganizeId.IsEmpty())
            {
                strSql.Append(" AND OrganizeId = @OrganizeId");
                parameter.Add(DbParameters.CreateDbParameter("@OrganizeId", OrganizeId));
            }
            strSql.Append(" order by t.sortcode,t.encode");
            return this.BaseRepository().FindTable(strSql.ToString(), parameter.ToArray());
        }
    }
}
