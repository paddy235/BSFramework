using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.KeyBoxManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.KeyboxManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Util;
using BSFramework.Util.Log;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 钥匙管理
    /// </summary>
    public class KeyBoxController : MvcControllerBase
    {

        private KeyBoxBLL _bll = new KeyBoxBLL();


        #region 页面
        public ActionResult Index()
        {
            Operator user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            return View();
        }
        public ActionResult Import()
        {
            return View();
        }

        public ActionResult Form() {
            return View();
        }
        #endregion
        #region 查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public ActionResult GetPageKeyBoxList(Pagination pagination, string queryJson)
        {
            try
            {
                var watch = CommonHelper.TimerStart();
                Operator user = OperatorProvider.Provider.Current();
                var data = _bll.GetPageKeyBoxList(pagination, queryJson);
                var JsonData = new
                {
                    rows = data,
                    pagination.total,
                    pagination.page,
                    pagination.records,
                    costtime = CommonHelper.TimerEnd(watch)
                };
                return ToJsonResult(JsonData);
            }
            catch (Exception ex)
            {
                var JsonData = new
                {
                    rows = new List<KeyBoxEntity>(),
                    pagination.total,
                    pagination.page,
                    pagination.records,
                    costtime = "0",
                    ex.Message
                };
                return ToJsonResult(JsonData);
            }
        }

        /// <summary>
        /// 基础返回类
        /// </summary>
        public class BaseResult
        {
            public int Code { get; set; }
            public int Count { get; set; }
            public string Info { get; set; }
        }

        public class BaseResultData : BaseResult
        {
            public List<DataItemDetailEntity> data { get; set; }
        }
        private static T webClientERCHTMS<T>(string url, string val) where T : class
        {
            var webclient = new WebClient();
            var ApiIp = Config.GetValue("ErchtmsApiUrl");
            NameValueCollection postVal = new NameValueCollection();
            postVal.Add("json", val);
            var getData = webclient.UploadValues(ApiIp + url, postVal);
            var result = System.Text.Encoding.UTF8.GetString(getData);
            NLog.LogManager.GetCurrentClassLogger().Info("管理平台-钥匙管理\r\n-->请求地址：{0}\r\n-->请求数据：{1}\r\n-->返回数据：{2}", url, val, result);
            return JsonConvert.DeserializeObject<T>(result);
        }
        /// <summary>
        /// 获取双控编码
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCodeList()
        {
            Operator user = OperatorProvider.Provider.Current();
            var valueStr = string.Format("\"userid\":\"{0}\"", user.UserId);
            var DataStr = string.Format("\"code\":\"{0}\"", "HidMajorClassify");
            DataStr = "{" + DataStr + "}";
            valueStr = "{" + valueStr + ",\"data\":" + DataStr + "}";
            var data = webClientERCHTMS<BaseResultData>("BaseData/GetCodeList", valueStr);

            return Json(data.data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取序号
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult GetSort(string keyValue,string name)
        {

            try
            {
                var entity = _bll.getKeyBoxSort(keyValue,name);
                return ToJsonResult(entity);
            }
            catch (Exception ex)
            {
                return ToJsonResult(ex);
            }
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult GetDetail(string keyValue) {

            try
            {
                var entity = _bll.getKeyBoxDataById(keyValue);
                return ToJsonResult(entity);
            }
            catch (Exception ex)
            {
                return ToJsonResult(ex);
            }
        }
        #endregion

        #region 新增修改

        /// <summary>
        /// 保存实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(KeyBoxEntity entity)
        {
            Operator user = OperatorProvider.Provider.Current();
            WriteLog.AddLog($"用户：{user.UserName}  id：{user.UserId}开始编辑钥匙管理数据 \r\n {JsonConvert.SerializeObject(entity)}", "KeyBox");
            try
            {
                if (string.IsNullOrEmpty(entity.CreateUserId))
                {
                    entity.CreateDate = DateTime.Now;
                    entity.CreateUserId = user.UserId;
                    entity.CreateUserName = user.UserName;
                }
                entity.ModifyDate = DateTime.Now;
                entity.ModifyUserId = user.UserId;
                entity.ModifyUserName = user.UserName;
                List<KeyBoxEntity> dataList = new List<KeyBoxEntity>();
                dataList.Add(entity);
                _bll.operateKeyBox(dataList);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"错误 \r\n用户：{user.UserName}  id：{user.UserId}编辑钥匙管理数据失败 \r\n 错误信息：{JsonConvert.SerializeObject(ex)}", "SafetyScore");
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult Remove(string keyValue)
        {
            Operator user = OperatorProvider.Provider.Current();
            WriteLog.AddLog($"用户：{user.UserName}  id：{user.UserId}开始删除钥匙管理数据{keyValue}", "SafetyScore");
            try
            {
                _bll.removeKeyBox(keyValue);
                return Success("删除成功。");
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"错误 \r\n用户：{user.UserName}  id：{user.UserId}删除钥匙管理数据{keyValue}失败 \r\n 错误信息：{JsonConvert.SerializeObject(ex)}", "SafetyScore");
                return Error("删除失败：" + ex.Message);
            }
        }

        #endregion



        #region 导入


        /// <summary>
        /// 管理平台应急预案导入
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Importxlsx()
        {
            DepartmentBLL departmentBLL = new DepartmentBLL();
            UserBLL userBLL = new UserBLL();
            Operator user = OperatorProvider.Provider.Current();
            var success = true;
            var message = string.Empty;
            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                if (!this.Request.Files[0].FileName.EndsWith(".xlsx")) throw new Exception("请上传 Excel 文件");
                //读取文件
                var book = new Workbook(this.Request.Files[0].InputStream);
                //获取第一个sheet
                var sheet = book.Worksheets[0];
                //列表实体
                var templates = new List<KeyBoxEntity>();
                //主表
                //DateTime dtDate;
                var userList = userBLL.GetUserList();
                var deptList = departmentBLL.GetList();

                //获取CATEGORY
                var valueStr = string.Format("\"userid\":\"{0}\"", user.UserId);
                var DataStr = string.Format("\"code\":\"{0}\"", "HidMajorClassify");
                DataStr = "{" + DataStr + "}";
                valueStr = "{" + valueStr + ",\"data\":" + DataStr + "}";
                var CategoryData = webClientERCHTMS<BaseResultData>("BaseData/GetCodeList", valueStr);
                var CategoryList = CategoryData.data;

                //获取keycode
                var KeycodeList = _bll.getKeyBoxData();

                for (int i = 1; i <= sheet.Cells.MaxDataRow; i++)
                {
                    var entity = new KeyBoxEntity();
                    entity.ID = Guid.NewGuid().ToString();

                    if (!string.IsNullOrEmpty(sheet.Cells[i, 0].StringValue))
                    {

                        entity.KeyCode = sheet.Cells[i, 0].StringValue.Trim();
                        entity.Category = sheet.Cells[i, 1].StringValue.Trim();

                        var CkCategory = CategoryList.FirstOrDefault(row => row.ItemName == entity.Category);

                        if (CkCategory == null)
                        {

                            throw new Exception("第" + (i + 1) + "行,不存在该专业类型！");
                        }
                        entity.CategoryId = CkCategory.ItemValue;

                        var CkCode = KeycodeList.FirstOrDefault(row => row.KeyCode == entity.KeyCode && row.Category == entity.Category);
                        var EntityCode = templates.Where(row => row.KeyCode == entity.KeyCode && row.Category == entity.Category);
                        if (EntityCode.Count() > 0)
                        {
                            throw new Exception("第" + (i + 1) + "行,钥匙编码重复！");
                        }
                        if (CkCode != null)
                        {

                            throw new Exception("第" + (i + 1) + "行,钥匙编码重复！");
                        }


                        //编码规则
                        try
                        {
                            var Sort = new List<int>();
                            for (int j = 0; j < entity.KeyCode.Length; j++)
                            {
                                var ckSort = Util.Str.ContainsNumber(entity.KeyCode[j].ToString());
                                if (ckSort)
                                {
                                    Sort.Add(j);
                                }
                            }

                            if (Sort.Count == 1)
                            {
                                throw new Exception("第" + (i + 1) + "行,钥匙编码格式错误！");
                            }
                            var sortStr = string.Empty;
                            int m = 0;
                            for (int n = Sort[0]; n <= Sort[Sort.Count-1]; n++)
                            {
                                sortStr += entity.KeyCode[n];
                                if (m != Sort.Count - 1)
                                {
                                    if (Sort[m] + 1 != Sort[m + 1])
                                    {
                                        throw new Exception("第" + (i + 1) + "行,钥匙编码格式错误！");
                                    }
                                }

                                m++;
                            }
                            entity.Sort = sortStr;

                        }
                        catch (Exception ex)
                        {

                            throw new Exception("第" + (i + 1) + "行,钥匙编码格式错误！");
                        }

                        entity.KeyPlace = sheet.Cells[i, 2].StringValue.Trim();
                        var depatName = sheet.Cells[i, 3].StringValue.Trim();
                        var CkDept = deptList.FirstOrDefault(row => row.FullName == depatName);
                        if (CkDept == null)
                        {
                            throw new Exception("第" + (i + 1) + "行,系统不存在该部门！");
                        }
                        entity.DeptCode = CkDept.EnCode;
                        entity.DeptId = CkDept.DepartmentId;
                        entity.DeptName = CkDept.FullName;
                        entity.CreateDate = DateTime.Now;
                        entity.CreateUserId = user.UserId;
                        entity.CreateUserName = user.UserName;
                        entity.ModifyDate = DateTime.Now;
                        entity.ModifyUserId = user.UserId;
                        entity.ModifyUserName = user.UserName;
                        entity.State = false;
                        //if (!string.IsNullOrEmpty(sheet.Cells[i, 9].StringValue))
                        //{
                        //    if (DateTime.TryParse(sheet.Cells[i, 9].StringValue, out dtDate))
                        //    {

                        //    }
                        //    else
                        //    {
                        //        throw new Exception("第" + (i + 1) + "行,演练时间格式错误！");
                        //    }
                        //}
                        templates.Add(entity);
                    }
                }
                _bll.operateKeyBox(templates);

            }
            catch (Exception ex)
            {
                success = false;
                message = HttpUtility.JavaScriptStringEncode(ex.Message);
            }
            return Json(new { success, message });
        }
        #endregion

    }
}