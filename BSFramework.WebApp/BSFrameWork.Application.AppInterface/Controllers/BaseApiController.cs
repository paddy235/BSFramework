using BSFramework.Application.Busines.AuthorizeManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Busines.AuthorizeManage;
using BSFramework.Util;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    /// Api基类对象，用于获取用户对象，以甄别当前登陆用户是否失效
    /// </summary>
    public class BaseApiController : ApiController
    {
        public Operator curUser = new Operator();
        public BaseApiController()
        {
            try
            {
                HttpContext httpContext = HttpContext.Current;
                string userId = string.Empty;
                string json = null;
                if (httpContext.Request.ContentType.Contains("multipart/form-data"))
                    json = httpContext.Request.Form["json"];
                else if (httpContext.Request.ContentType.Contains("application/x-www-form-urlencoded"))
                    json = httpContext.Request.Form["json"];
                else if (httpContext.Request.ContentType.Contains("application/json"))
                {
                    StreamReader sr = new StreamReader(httpContext.Request.InputStream);
                    json = sr.ReadToEnd();
                    //把当前流的读取位置重新设置为0
                    httpContext.Request.InputStream.Position = 0;
                }

                if (!string.IsNullOrEmpty(json))
                {
                    json = json.ToLower();
                    dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
                    if (json.Contains("json"))
                    {
                        if (json.Contains("userid"))
                            userId = dy.json.userid.ToString();
                    }
                    else
                    {
                        if (json.Contains("userid"))
                            userId = dy.userid.ToString();
                    }
                }

                if (string.IsNullOrEmpty(userId))
                {
                    userId = httpContext.Request.Headers.Get("bst-user");
                }

                OperatorProvider.AppUserId = userId;  //设置当前用户
                curUser = OperatorProvider.Provider.Current();
                if (null == curUser)
                {
                    //用户id不为空
                    if (!string.IsNullOrEmpty(userId))
                    {
                        curUser = GetOperator(userId);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private Operator GetOperator(string userId)
        {
            UserBLL userBLL = new UserBLL();
            UserEntity userEntity = userBLL.GetEntity(userId);

            DepartmentBLL bll = new DepartmentBLL();

            if (userEntity != null)
            {
                AuthorizeBLL authorizeBLL = new AuthorizeBLL();
                Operator operators = new Operator();
                operators.UserId = userEntity.UserId;
                operators.Code = userEntity.EnCode;
                operators.Account = userEntity.Account;
                operators.UserName = userEntity.RealName;
                operators.Password = userEntity.Password;
                operators.Secretkey = userEntity.Secretkey;
                operators.OrganizeId = userEntity.OrganizeId;
                operators.DeptId = userEntity.DepartmentId;
                operators.DeptCode = userEntity.DepartmentCode;
                operators.OrganizeCode = userEntity.OrganizeCode;
                //operators.DeptName = userEntity.DeptName;
                //operators.OrganizeName = userEntity.OrganizeName;
                operators.PostName = userBLL.GetObjectName(userEntity.UserId, 3);
                operators.RoleName = userEntity.RoleName;
                operators.DutyName = userBLL.GetObjectName(userEntity.UserId, 4);
                operators.IPAddress = Net.Ip;
                operators.ObjectId = new PermissionBLL().GetObjectStr(userEntity.UserId);
                operators.LogTime = DateTime.Now;
                operators.Token = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                //写入当前用户数据权限
                AuthorizeDataModel dataAuthorize = new AuthorizeDataModel();
                dataAuthorize.ReadAutorize = authorizeBLL.GetDataAuthor(operators.UserId);
                dataAuthorize.ReadAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators.UserId);
                dataAuthorize.WriteAutorize = authorizeBLL.GetDataAuthor(operators.UserId, true);
                dataAuthorize.WriteAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators.UserId, true);
                operators.DataAuthorize = dataAuthorize;

                var dept = bll.GetEntity(userEntity.DepartmentId);
                if (dept == null) dept = bll.GetRootDepartment();

                operators.DeptName = dept.FullName;
                operators.DeptCode = dept.EnCode;

                //判断是否系统管理员
                if (userEntity.Account == "System")
                {
                    operators.IsSystem = true;
                }
                else
                {
                    operators.IsSystem = false;
                }
                OperatorProvider.Provider.AddCurrent(operators);

                return operators;
            }
            return null;
        }

    }
}