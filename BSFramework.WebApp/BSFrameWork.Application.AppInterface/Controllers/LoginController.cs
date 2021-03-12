using BSFramework.Application.Busines.AuthorizeManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Busines.AuthorizeManage;
using BSFramework.Util;
using BSFramework.Util.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFrameWork.Application.AppInterface.Models;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Busines.PublicInfoManage;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class LoginController : BaseApiController
    {
        PeopleBLL pbll = new PeopleBLL();
        // GET api/login
        public object Get()
        {
            BSFramework.Application.Busines.BaseManage.UserBLL ub = new BSFramework.Application.Busines.BaseManage.UserBLL();
            return ub.GetList();

        }

        [HttpPost]
        public ModelBucket<UserModel> CheckLogin()
        {
            var username = string.Empty;
            var password = string.Empty;
            //string res = json.Value<string>("json");
            //dynamic br = JsonConvert.DeserializeObject<ExpandoObject>(res);
            //string userAccount = br.data.userAccount;
            //string pwd = br.data.password;
            if (this.Request.Content.IsFormData())
            {
                var nv = this.Request.Content.ReadAsFormDataAsync().Result;
                var json = nv.Get("json");
                if (json == null)
                {
                    username = nv.Get("username");
                    password = nv.Get("password");
                }
                else
                {
                    var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<LoginModel>>(json);
                    username = model.Data.UserAccount;
                    password = model.Data.Password;
                }
            }
            else
            {
                var json = this.Request.Content.ReadAsStringAsync().Result;
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<LoginModel>(json);
                username = model.UserName;
                password = model.Password;
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return new ModelBucket<UserModel> { Success = false, Message = "账号或者密码不能为空！" };

            #region 内部账户验证
            UserBLL userBLL = new UserBLL();
            var pwd = Md5Helper.MD5(password, 32);

            UserEntity userEntity = null;

            //远程验证
            if (new LoginUtility().ValidRemote(username, password))
            {
                userEntity = userBLL.GetUserByAccount(username);
            }
            else
            {
                //本地验证
                userEntity = userBLL.CheckLogin(username, pwd);
            }

            DepartmentBLL bll = new DepartmentBLL();
            if (userEntity == null)
            {
                return new ModelBucket<UserModel> { Success = false, Message = "账号或密码错误！" };
            }
            else
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
                //operators.OrganizeName = userEntity.or;
                operators.PostName = userBLL.GetObjectName(userEntity.UserId, 3);
                operators.RoleName = userEntity.RoleName;
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

                var dept = bll.GetEntity(userEntity.DepartmentId);
                if (dept == null) dept = bll.GetRootDepartment();

                operators.DeptName = dept.FullName;
                operators.DeptCode = dept.EnCode;
                userEntity.DepartmentName = dept.FullName;
                userEntity.DepartmentCode = dept.EnCode;

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
                logEntity.OperateAccount = userEntity.Account;
                logEntity.OperateUserId = userEntity.UserId;
                logEntity.Module = Config.GetValue("SoftName");
                logEntity.ExecuteResult = 1;
                logEntity.ExecuteResultJson = "登录成功";
                logEntity.WriteLog();
            }

            var company = bll.GetFactory(userEntity.UserId);

            var people = new PeopleBLL().GetEntity(userEntity.UserId);
            var deparment = bll.GetEntity(userEntity.DepartmentId);
            var filelist = new FileInfoBLL().GetFileList(userEntity.UserId);
            var fileinfo = filelist.FirstOrDefault(x => x.Description == "Photo");
            if (fileinfo != null)
            {
                people.Photo = BSFramework.Util.Config.GetValue("AppUrl") + fileinfo.FilePath.Substring(2, fileinfo.FilePath.Length - 2);
            }
            else
            {
                if (!string.IsNullOrEmpty(people.Photo))
                {
                    if (people.Photo.IndexOf("http") < 0)
                        people.Photo = BSFramework.Util.Config.GetValue("AppUrl") + people.Photo.Substring(people.Photo.IndexOf("/") + 1);
                }
            }

            #endregion
            return new ModelBucket<UserModel>
            {
                Success = true,
                Message = "登陆成功",
                Data = new UserModel
                {
                    userId = userEntity.UserId,
                    userAccount = userEntity.Account,
                    userName = userEntity.RealName,
                    telephone = userEntity.Telephone,
                    mobile = userEntity.Mobile,
                    deptId = userEntity.DepartmentId,
                    deptCode = userEntity.DepartmentCode,
                    deptName = userEntity.DepartmentName,
                    roleName = userEntity.RoleName,
                    postName = userEntity.PostName,
                    faceUrl = people == null ? null : people.Photo,
                    userNum = userEntity.EnCode,
                    //breakRuleAdmin = isbreakruleadmin(userEntity),
                    Factory = company,
                    Quarters = people == null ? null : people.Quarters,
                    TeamType = deparment == null ? null : deparment.TeamType
                }
            };
        }

        //private string isbreakruleadmin(UserInfoEntity entity)
        //{
        //    string r = "0";
        //    DepartmentEntity dept = new DepartmentBLL().GetEntity(entity.DepartmentId);
        //    if (dept.Description == "安全部")
        //    {
        //        r = "1";
        //    }
        //    else
        //    {
        //        PeopleEntity p = new PeopleBLL().GetEntity(entity.UserId);
        //        if (p != null)
        //        {
        //            if (p.Quarters == "班长" || p.Quarters == "安全员")
        //            {
        //                r = "1";
        //            }
        //        }
        //    }
        //    return r;

        //}

        [HttpPost]
        public object GetAllFingers()
        {
            try
            {

                var list = pbll.GetLogins();
                var nlist = list.Select(x => new
                {
                    x.UserId,
                    x.Finger,
                    x.Face,
                    Tel = getTel(x.UserId)
                });
                return new { code = 0, info = "成功", data = nlist };

            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = ex.Message };
            }
        }

        private string getTel(string userid)
        {
            string tel = "";
            var p = pbll.GetEntity(userid);
            if (p != null) tel = p.LinkWay;
            return tel;
        }

        //[HttpPost]
        //public object CheckLoginNew([FromBody] JObject json)
        //{
        //    try
        //    {
        //        string res = json.Value<string>("json");
        //        dynamic br = JsonConvert.DeserializeObject<ExpandoObject>(res);
        //        string key = br.data.key;
        //        string value = br.data.value;
        //        #region 内部账户验证
        //        UserBLL userBLL = new UserBLL();
        //        PeopleBLL pbll = new PeopleBLL();
        //        UserEntity user = new UserEntity();
        //        UserInfoEntity userEntity = new UserInfoEntity();
        //        if (key == "1") //指纹
        //        {
        //            user = userBLL.GetEntity(value);
        //            if (user != null)
        //            {
        //                userEntity = userBLL.GetUserInfoEntity(user.UserId);
        //            }
        //        }
        //        else //身份证
        //        {
        //            int total = 0;
        //            //var list = pbll.GetList(user.value, 1, 10000, out total);
        //            if (key == "2") list = list.Where(x => x.IdentityNo == value);
        //            if (key == "3") list = list.Where(x => x.LabourNo == value);
        //            if (key == "4") list = list.Where(x => x.LinkWay == value);
        //            PeopleEntity p = list.FirstOrDefault();
        //            if (p != null)
        //            {
        //                user = userBLL.GetEntity(p.ID);
        //                userEntity = userBLL.GetUserInfoEntity(user.UserId);
        //            }
        //            else
        //            {
        //                return new { Code = -1, Info = "用户不存在！" };
        //            }
        //        }
        //        if (userEntity != null)
        //        {
        //            AuthorizeBLL authorizeBLL = new AuthorizeBLL();
        //            Operator operators = new Operator();
        //            operators.UserId = userEntity.UserId;
        //            operators.Code = userEntity.EnCode;
        //            operators.Account = userEntity.Account;
        //            operators.UserName = userEntity.RealName;
        //            operators.Password = userEntity.Password;
        //            operators.Secretkey = userEntity.Secretkey;
        //            operators.OrganizeId = userEntity.OrganizeId;
        //            operators.DeptId = userEntity.DepartmentId;
        //            operators.DeptCode = userEntity.DepartmentCode;
        //            operators.OrganizeCode = userEntity.OrganizeCode;
        //            operators.DeptName = userEntity.DeptName;
        //            operators.OrganizeName = userEntity.OrganizeName;
        //            operators.PostName = userBLL.GetObjectName(userEntity.UserId, 3);
        //            operators.RoleName = userBLL.GetObjectName(userEntity.UserId, 2);
        //            operators.DutyName = userBLL.GetObjectName(userEntity.UserId, 4);
        //            operators.IPAddress = Net.Ip;
        //            operators.ObjectId = new PermissionBLL().GetObjectStr(userEntity.UserId);
        //            operators.LogTime = DateTime.Now;
        //            operators.Token = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
        //            //写入当前用户数据权限
        //            AuthorizeDataModel dataAuthorize = new AuthorizeDataModel();
        //            dataAuthorize.ReadAutorize = authorizeBLL.GetDataAuthor(operators);
        //            dataAuthorize.ReadAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators);
        //            dataAuthorize.WriteAutorize = authorizeBLL.GetDataAuthor(operators, true);
        //            dataAuthorize.WriteAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators, true);
        //            operators.DataAuthorize = dataAuthorize;
        //            //判断是否系统管理员
        //            if (userEntity.Account == "System")
        //            {
        //                operators.IsSystem = true;
        //            }
        //            else
        //            {
        //                operators.IsSystem = false;
        //            }
        //            OperatorProvider.Provider.AddCurrent(operators);
        //            //登录限制
        //            //LoginLimit(username, operators.IPAddress, operators.IPAddressName);
        //            //写入日志
        //            LogEntity logEntity = new LogEntity();
        //            logEntity.CategoryId = 1;
        //            logEntity.OperateTypeId = ((int)OperationType.Login).ToString();
        //            logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Login);
        //            logEntity.OperateAccount = userEntity.Account;
        //            logEntity.OperateUserId = userEntity.UserId;
        //            logEntity.Module = Config.GetValue("SoftName");
        //            logEntity.ExecuteResult = 1;
        //            logEntity.ExecuteResultJson = "登录成功";
        //            logEntity.WriteLog();
        //        }
        //        #endregion
        //        DepartmentBLL bll = new DepartmentBLL();
        //        var dept = bll.GetFactory(userEntity.UserId);

        //        var people = new PeopleBLL().GetEntity(userEntity.UserId);
        //        var deparment = bll.GetEntity(userEntity.DepartmentId);
        //        return new
        //        {
        //            Code = 0,
        //            Info = "登陆成功",
        //            data = new
        //            {
        //                userId = userEntity.UserId,
        //                userAccount = userEntity.Account,
        //                userName = userEntity.RealName,
        //                telephone = userEntity.Telephone,
        //                mobile = userEntity.Mobile,
        //                deptId = userEntity.DepartmentId,
        //                deptCode = userEntity.DepartmentCode,
        //                deptName = userEntity.DeptName,
        //                roleName = userEntity.RoleName,
        //                postName = userEntity.PostName,
        //                faceUrl = people == null ? null : people.Photo,
        //                userNum = userEntity.EnCode,
        //                //breakRuleAdmin = isbreakruleadmin(userEntity),
        //                Factory = dept,
        //                Quarters = people == null ? null : people.Quarters,
        //                TeamType = deparment == null ? null : deparment.TeamType
        //            }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new { Code = -1, Info = ex.Message };
        //    }
        //}

        /// <summary>
        /// 博安云后台注册用户调用接口
        /// </summary>
        /// <param name="json">用户信息（如：json:{userName:"",mobile:"",deptName:""}）</param>
        ///参数说明：userName：姓名，mobile：手机号(作为系统账号)，deptName：单位名称
        /// <returns></returns>
        [HttpPost]
        public object RegisterUser()
        {
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            var register = Newtonsoft.Json.JsonConvert.DeserializeObject<RegisterModel>(json);

            try
            {
                if (string.IsNullOrEmpty(register.Mobile))
                {
                    return new { code = 1, data = "", info = "手机号不能为空" };
                }
                //判断账号是否在系统中存在
                var userBll = new UserBLL();
                bool result = userBll.ExistAccount(register.Mobile, "");
                if (result)
                {
                    var deptbll = new DepartmentBLL();
                    var rootdept = deptbll.GetRootDepartment();
                    var allteam = deptbll.GetSubDepartments(rootdept.DepartmentId, "班组");
                    var testteam = allteam.Find(x => x.FullName == "测试班组");
                    if (testteam == null) testteam = allteam.FirstOrDefault();
                    if (testteam == null) return new { code = 1, data = "", info = "试用单位未配置" };


                    string pathurl = Config.GetValue("SystemUrl"); //web平台对应的地址，放在编码配置也可后台直接后去，请根据实际情况处理
                    UserEntity user = new UserEntity();
                    user.UserId = Guid.NewGuid().ToString();
                    user.Account = register.Mobile; //手机号作为账号
                    user.RealName = register.UserName; //姓名
                    user.Password = "bs123456"; //密码
                    user.DepartmentId = testteam.DepartmentId; //所属部门ID
                    user.DepartmentCode = testteam.EnCode;//所属部门Code
                    user.OrganizeId = testteam.OrganizeId;//所属机构ID
                    user.OrganizeCode = testteam.EnCode;//所属机构Code
                    user.Mobile = register.Mobile;//手机号
                    user.CreateUserId = "System";
                    user.CreateUserName = "System";
                    user.RoleName = "班组成员";   //注册用户默认给公司管理员角色，此处大家根据情况配置
                    user.RoleId = "e503d929-daa6-472d-bb03-42533a11f9c6"; //对应角色ID,此处大家根据情况配置
                    user.AllowStartTime = DateTime.Now; //注册账号有效期开始时间
                    user.AllowEndTime = Convert.ToDateTime(user.AllowStartTime).AddDays(15);//注册账号有效期截止时间，默认试用有效期为15天，此期限大家可以放在后台配置，注意请在用户登录系统的时候判断账号是否过期
                    user.Description = null;//此处用备注字段存储单位名称，大家可根据情况处理
                                            //下列数据需返回给调用平台，调用平台会根据此信息以短信形式发送给注册用户的
                    var tempdata = new
                    {
                        account = register.Mobile,//账号
                        password = "bs123456",//密码
                        allowEndTime = Convert.ToDateTime(user.AllowEndTime).ToString("yyyy-MM-dd HH:mm:ss"), //账号有效截止日期
                        path = pathurl  // web系统调用地址，请大家区对应的外网部署的试用系统地址，不清楚的问对应的产品负责人或测试人员，在 博安云上都有系统入口的
                    };

                    userBll.SaveForm(user.UserId, user); //请大家调用自己的业务方法处理
                                                         //此处大家记得下操作日期，已记录用户信息，便于后台排查
                                                         //写日志……
                    return new { code = 0, data = tempdata, info = "操作成功" };
                }
                else
                {
                    return new { code = 1, data = "", info = "手机号已存在!" };
                }
            }
            catch (Exception ex)
            {
                //此处可记录异常日志
                return new { code = 1, data = "", info = ex.Message };
            }
        }

        [HttpPost]
        public ModelBucket<object> Valid([FromBody] ParamBucket<string> args)
        {
            var user = default(UserEntity);
            try
            {
                using (var face = new FaceUtil())
                {
                    user = face.Valid(args.Data);
                }
            }
            catch (Exception e)
            {
                return new ModelBucket<object>() { Message = e.Message, Success = false };
            }
            if (user == null) return new ModelBucket<object>() { Data = null, Success = false, Message = "未能识别！" };

            DepartmentBLL bll = new DepartmentBLL();
            var dept = bll.GetFactory(user.UserId);

            var people = new PeopleBLL().GetEntity(user.UserId);
            var deparment = bll.GetEntity(user.DepartmentId);
            var obj = new
            {
                userId = user.UserId,
                userAccount = user.Account,
                userName = user.RealName,
                telephone = user.Telephone,
                mobile = user.Mobile,
                deptId = deparment.DepartmentId,
                deptCode = deparment.EnCode,
                deptName = deparment.FullName,
                roleName = user.RoleName,
                postName = user.PostName,
                faceUrl = people == null ? null : people.Photo,
                userNum = user.EnCode,
                //breakRuleAdmin = isbreakruleadmin(new UserInfoEntity() { UserId = user.UserId, DepartmentId = user.DepartmentId }),
                Factory = dept,
                Quarters = people == null ? null : people.Quarters,
                TeamType = deparment == null ? null : deparment.TeamType
            };

            return new ModelBucket<object>() { Data = obj, Success = true };
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Register()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "No Files" };

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "face";
            var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            try
            {
                var provider = new MultipartFormDataStreamProvider(root);
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<UserFaceEntity>(json);
                //var userid = provider.FormData.Get("UserId");
                //var facestream = provider.FormData.Get("FaceStream");
                //var model = new UserFaceEntity() { UserId = userid, FaceStream = facestream };

                var userbll = new UserBLL();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');

                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { RecId = model.UserId, CreateDate = DateTime.Now, CreateUserId = model.UserId, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = model.UserId, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);

                    fileentity.Description = "人脸";
                    model.FaceFile = fileentity;
                }

                userbll.Register(model);

                return this.Request.CreateResponse<ResultBucket>(HttpStatusCode.OK, new ResultBucket { Success = true, info = string.Empty });
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse<ResultBucket>(HttpStatusCode.OK, new ResultBucket { Success = false, info = ex.Message });
            }
        }
    }
}
