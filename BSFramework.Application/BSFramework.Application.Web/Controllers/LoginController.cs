using BSFramework.Application.Busines;
using BSFramework.Application.Busines.AuthorizeManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity;
using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Busines.AuthorizeManage;
using BSFramework.Util;
using BSFramework.Util.Attributes;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using Bst.ServiceContract.MessageQueue;
using System;
using System.Data.Common;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ThoughtWorks.QRCode.Codec;

namespace BSFramework.Application.Web.Controllers
{
    /// <summary>
    /// 描 述：系统登录
    /// </summary>
    [HandlerLogin(LoginMode.Ignore)]
    public class LoginController : MvcControllerBase
    {
        #region 视图功能
        /// <summary>
        /// 默认页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Default()
        {
            return View();
        }
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult VerifyCode()
        {
            return File(new VerifyCode().GetVerifyCode(), @"image/Gif");
        }
        /// <summary>
        /// 安全退出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult OutLogin()
        {
            LogEntity logEntity = new LogEntity();
            logEntity.CategoryId = 1;
            logEntity.OperateTypeId = ((int)OperationType.Exit).ToString();
            logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Exit);
            logEntity.OperateAccount = OperatorProvider.Provider.Current().Account;
            logEntity.OperateUserId = OperatorProvider.Provider.Current().UserId;
            logEntity.ExecuteResult = 1;
            logEntity.ExecuteResultJson = "退出系统";
            logEntity.Module = Config.GetValue("SoftName");
            logEntity.WriteLog();
            Session.Abandon();                                          //清除当前会话
            Session.Clear();                                            //清除当前浏览器所有Session
            OperatorProvider.Provider.EmptyCurrent(); ;                  //清除登录者信息
            WebHelper.RemoveCookie("autologin");                  //清除自动登录
            return Content(new AjaxResult { type = ResultType.success, message = "退出系统" }.ToJson());
        }

        [HttpGet]
        [HttpPost]
        [AjaxOnly(true)]
        public ActionResult SignOut()
        {
            if (OperatorProvider.Provider.Current() == null)
            {
                return Redirect("/");
            }
            else
            {
                LogEntity logEntity = new LogEntity();
                logEntity.CategoryId = 1;
                logEntity.OperateTypeId = ((int)OperationType.Exit).ToString();
                logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Exit);
                logEntity.OperateAccount = OperatorProvider.Provider.Current().Account;
                logEntity.OperateUserId = OperatorProvider.Provider.Current().UserId;
                logEntity.ExecuteResult = 1;
                logEntity.ExecuteResultJson = "退出系统";
                logEntity.Module = Config.GetValue("SoftName");
                logEntity.WriteLog();
                Session.Abandon();                                          //清除当前会话
                Session.Clear();                                            //清除当前浏览器所有Session
                OperatorProvider.Provider.EmptyCurrent(); ;                  //清除登录者信息
                WebHelper.RemoveCookie("autologin");                  //清除自动登录                                                    //return RedirectToAction("Index", "Home");
                return View();
            }

        }
        public ActionResult Ref()
        {
            var id = Guid.NewGuid().ToString();
            var encoder = new QRCodeEncoder();
            var image = encoder.Encode("login" + id, Encoding.UTF8);

            var path = "/Content/styles/static/images/";
            if (!Directory.Exists(Server.MapPath("~" + path)))
                Directory.CreateDirectory(Server.MapPath("~" + path));
            //if (System.IO.File.Exists(Server.MapPath("~" + path)))
            //{
            //    System.IO.File.Delete(Server.MapPath("~" + path));
            //}
            image.Save(Path.Combine(Server.MapPath("~" + path), id + ".jpg"));

            //FileInfoEntity f= new FileInfoEntity() { FileId = id, CreateDate = DateTime.Now, CreateUserId = "", CreateUserName = "", Description = "login", FileExtensions = ".jpg", FileName = id + ".jpg", FilePath ="~"+ path + id + ".jpg", FileType = "jpg", ModifyDate = DateTime.Now, ModifyUserId = "", ModifyUserName = "", RecId = "" };
            return Success("", new { path = path + id + ".jpg" });
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="verifycode">验证码</param>
        /// <param name="autologin">下次自动登录</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult CheckLogin(string username, string password, string verifycode, int autologin)
        {
            LogEntity logEntity = new LogEntity();
            logEntity.CategoryId = 1;
            logEntity.OperateTypeId = ((int)OperationType.Login).ToString();
            logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Login);
            logEntity.OperateAccount = username;
            logEntity.OperateUserId = username;
            logEntity.Module = Config.GetValue("SoftName");

            try
            {
                #region 验证码验证
                if (autologin == 0)
                {
                    //verifycode = Md5Helper.MD5(verifycode.ToLower(), 16);
                    //if (Session["session_verifycode"].IsEmpty() || verifycode != Session["session_verifycode"].ToString())
                    //{
                    //    throw new Exception("验证码错误，请重新输入");
                    //}
                }
                #endregion

                #region 第三方账户验证 关闭该验证
                //AccountEntity accountEntity = accountBLL.CheckLogin(username, password);
                //if (accountEntity != null)
                //{
                //    Operator operators = new Operator();
                //    operators.UserId = accountEntity.AccountId;
                //    operators.Code = accountEntity.MobileCode;
                //    operators.Account = accountEntity.MobileCode;
                //    operators.UserName = accountEntity.FullName;
                //    operators.Password = accountEntity.Password; 
                //    operators.IPAddress = Net.Ip;
                //    operators.IPAddressName = IPLocation.GetLocation(Net.Ip);
                //    operators.LogTime = DateTime.Now;
                //    operators.Token = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                //    operators.IsSystem = true;
                //    OperatorProvider.Provider.AddCurrent(operators);
                //    //登录限制
                //    LoginLimit(username, operators.IPAddress, operators.IPAddressName);
                //    return Success("登录成功。");
                //}
                #endregion

                #region 内部账户验证
                UserBLL userBLL = new UserBLL();
                UserEntity userEntity = userBLL.CheckLogin(username, password);
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
                    operators.DeptName = userEntity.DepartmentName;
                    //operators.OrganizeName = userEntity.OrganizeName;
                    operators.PostName = userBLL.GetObjectName(userEntity.UserId, 3);
                    operators.RoleName = userBLL.GetObjectName(userEntity.UserId, 2);
                    operators.DutyName = userBLL.GetObjectName(userEntity.UserId, 4);
                    operators.IPAddress = Net.Ip;
                    operators.ObjectId = new PermissionBLL().GetObjectStr(userEntity.UserId);
                    operators.LogTime = DateTime.Now;
                    operators.Token = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                    //写入当前用户数据权限
                    AuthorizeDataModel dataAuthorize = new AuthorizeDataModel();
                    //dataAuthorize.ReadAutorize = authorizeBLL.GetDataAuthor(operators.UserId);
                    //dataAuthorize.ReadAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators.UserId);
                    //dataAuthorize.WriteAutorize = authorizeBLL.GetDataAuthor(operators.UserId, true);
                    //dataAuthorize.WriteAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators.UserId, true);
                    operators.DataAuthorize = dataAuthorize;

                    var dept = new DepartmentBLL().GetEntity(userEntity.DepartmentId);
                    if (dept == null) dept = new DepartmentBLL().GetRootDepartment();

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
                    OperatorProvider.AppUserId = operators.UserId;
                    OperatorProvider.Provider.AddCurrent(operators);
                    //登录限制
                    //LoginLimit(username, operators.IPAddress, operators.IPAddressName);
                    //写入日志
                    logEntity.ExecuteResult = 1;
                    logEntity.ExecuteResultJson = "登录成功";
                    logEntity.WriteLog();

                    return Success("登录成功。");
                }
                else
                {
                    return Error("账号或密码错误！");
                }
                //return Success("登录成功。");
                #endregion
            }
            catch (Exception ex)
            {
                WebHelper.RemoveCookie("autologin");                  //清除自动登录
                logEntity.ExecuteResult = -1;
                logEntity.ExecuteResultJson = ex.Message;
                logEntity.WriteLog();
                return Error(ex.Message);
            }
        }
        #endregion

        #region 注册账户、登录限制
        private AccountBLL accountBLL = new AccountBLL();
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="mobileCode">手机号码</param>
        /// <returns>返回6位数验证码</returns>
        [HttpGet]
        public ActionResult GetSecurityCode(string mobileCode)
        {
            if (!ValidateUtil.IsValidMobile(mobileCode))
            {
                throw new Exception("手机格式不正确,请输入正确格式的手机号码。");
            }
            var data = accountBLL.GetSecurityCode(mobileCode);
            if (!string.IsNullOrEmpty(data))
            {
                SmsModel smsModel = new SmsModel();
                smsModel.account = Config.GetValue("SMSAccount");
                smsModel.pswd = Config.GetValue("SMSPswd");
                smsModel.url = Config.GetValue("SMSUrl");
                smsModel.mobile = mobileCode;
                smsModel.msg = "验证码 " + data + "，(请确保是本人操作且为本人手机，否则请忽略此短信)";
                SmsHelper.SendSmsByJM(smsModel);
            }
            return Success("获取成功。");
        }
        /// <summary>
        /// 注册账户
        /// </summary>
        /// <param name="mobileCode">手机号</param>
        /// <param name="securityCode">短信验证码</param>
        /// <param name="fullName">姓名</param>
        /// <param name="password">密码（md5）</param>
        /// <param name="verifycode">图片验证码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(string mobileCode, string securityCode, string fullName, string password, string verifycode)
        {
            AccountEntity accountEntity = new AccountEntity();
            accountEntity.MobileCode = mobileCode;
            accountEntity.SecurityCode = securityCode;
            accountEntity.FullName = fullName;
            accountEntity.Password = password;
            accountEntity.IPAddress = Net.Ip;
            accountEntity.IPAddressName = IPLocation.GetLocation(accountEntity.IPAddress);
            accountEntity.AmountCount = 30;
            accountBLL.Register(accountEntity);
            return Success("注册成功。");
        }
        /// <summary>
        /// 登录限制
        /// </summary>
        /// <param name="account">账户</param>
        /// <param name="iPAddress">IP</param>
        /// <param name="iPAddressName">IP所在城市</param>
        public void LoginLimit(string account, string iPAddress, string iPAddressName)
        {
            //if (account == "System")
            //{
            //    return;
            //}
            string platform = Net.Browser;
            accountBLL.LoginLimit(platform, account, iPAddress, iPAddressName);
        }
        #endregion



        /// <summary>
        /// 单点登录入口
        /// </summary>
        /// <param name="args">验证解密的参数</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SignIn(string args)
        {
            var currentUser = OperatorProvider.Provider.Current();
            //OperatorProvider.Provider.EmptyCurrent();                 //清除登录者信息


            //string url = "http://10.36.1.70/erchtms/HtStat/GetHtNumReadjustChart?queryJson={year:2018,level:'一般隐患'}";
            //args = BSFramework.Util.DESEncrypt.Encrypt("fahbfzr^"+url+"^" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^DLBZ");
            string encryptStr = BSFramework.Util.DESEncrypt.Decrypt(args);
            string[] arr = encryptStr.Split('^');
            string account = arr[0]; //账号
            string moduleNo = arr[1];//模块编号或者地址
            string time = arr[2]; //时间戳
            string appKey = arr[3];//应用app名称，用于授权


            //TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - DateTime.Parse(time).Ticks);
            //if (ts.TotalMinutes>60)
            //{
            //    string msg = "验证数据失败：链接已经失效！";
            //    return RedirectToAction("ErrorMsg", "Error", new { Message = msg });
            //}

            if (currentUser == null || currentUser.Account != account)
            {
                UserBLL userBLL = new UserBLL();
                UserEntity user = userBLL.GetUserByAccount(account);
                bool result = SetUserInfo(user);
            }
            //if (result)
            //{
            //BSFramework.Application.Code.OperatorProvider.AppUserId = user.UserId;
            if (moduleNo.Contains("/"))
            {
                return Redirect(moduleNo);
            }
            else
            {
                ModuleBLL moduleBll = new ModuleBLL();
                ModuleEntity module = moduleBll.GetEntityByCode(moduleNo);
                if (module == null)
                {
                    string msg = "验证数据失败：错误的地址！";
                    return this.HttpNotFound();
                }
                else
                {
                    return Redirect(Request.ApplicationPath + module.UrlAddress);
                }
            }
            //}

            return this.HttpNotFound();
        }
        private bool SetUserInfo(UserEntity userEntity)
        {
            if (userEntity != null)
            {
                UserBLL userBLL = new UserBLL();

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
                operators.DeptName = userEntity.DepartmentName;
                //operators.OrganizeName = userEntity.OrganizeName;
                operators.PostName = userBLL.GetObjectName(userEntity.UserId, 3);
                operators.RoleName = userBLL.GetObjectName(userEntity.UserId, 2);
                operators.DutyName = userBLL.GetObjectName(userEntity.UserId, 4);
                operators.IPAddress = Net.Ip;
                if (userEntity.HeadIcon != null)
                {
                    if (userEntity.HeadIcon.StartsWith("http"))
                        operators.Photo = userEntity.HeadIcon;
                    else
                    {

                        if (System.IO.File.Exists(Server.MapPath("~" + userEntity.HeadIcon)))
                        {
                            operators.Photo = userEntity.HeadIcon;
                        }
                        else
                        {
                            operators.Photo = "/Content/images/on-line.png";
                        }
                    }
                }

                var dept = new DepartmentBLL().GetEntity(userEntity.DepartmentId);
                if (dept == null) dept = new DepartmentBLL().GetRootDepartment();

                operators.DeptName = dept.FullName;
                operators.DeptCode = dept.EnCode;


                //operators.SendDeptID = userEntity.SendDeptID;
                //operators.IPAddressName = IPLocation.GetLocation(Net.Ip);
                //operators.IdentifyID = userEntity.IdentifyID; //身份证号码
                operators.ObjectId = new PermissionBLL().GetObjectStr(userEntity.UserId);
                operators.LogTime = DateTime.Now;
                operators.Token = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                //写入当前用户数据权限
                AuthorizeDataModel dataAuthorize = new AuthorizeDataModel();
                //dataAuthorize.ReadAutorize = authorizeBLL.GetDataAuthor(operators.UserId);
                //dataAuthorize.ReadAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators.UserId);
                //dataAuthorize.WriteAutorize = authorizeBLL.GetDataAuthor(operators.UserId, true);
                //dataAuthorize.WriteAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators.UserId, true);
                operators.DataAuthorize = dataAuthorize;
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
                //登录限制
                //LoginLimit(username, operators.IPAddress, operators.IPAddressName);
                //写入日志
                LogEntity logEntity = new LogEntity();
                logEntity.CategoryId = 1;
                logEntity.OperateTypeId = ((int)OperationType.Login).ToString();
                logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Login);
                logEntity.OperateAccount = userEntity.RealName;
                logEntity.OperateUserId = userEntity.RealName;
                logEntity.Module = Config.GetValue("SoftName");
                logEntity.OperateAccount = userEntity.Account + "(" + userEntity.RealName + ")";
                logEntity.ExecuteResult = 1;
                logEntity.ExecuteResultJson = "登录成功";
                logEntity.WriteLog();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
