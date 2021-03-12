using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SevenSManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class SevenSController : ApiController
    {
        private string url = BSFramework.Util.Config.GetValue("AppUrl");

        private UserBLL ubll = new UserBLL();
        private DepartmentBLL deptBll = new DepartmentBLL();
        private SevenSBLL ebll = new SevenSBLL();
        private UserWorkAllocationBLL bll = new UserWorkAllocationBLL();
        #region 技术规范


        //获取技术规范的类别
        [System.Web.Http.HttpPost]
        public object GetTypeList([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            string keyvalue = dy.data.keyvalue;
            var data = default(List<SevenSTypeEntity>);
            UserEntity user = new UserBLL().GetEntity(userId);

            try
            {
                if (!string.IsNullOrEmpty(keyvalue))
                {
                    data = ebll.GetAllType(user.DepartmentCode).Where(x => x.TypeName.Contains(keyvalue)).ToList();

                }
                else
                {
                    data = ebll.GetAllType(user.DepartmentCode).ToList();

                }

                for (int i = 0; i < data.Count; i++)
                {
                    if (string.IsNullOrEmpty(data[i].ParentCardId))
                    {
                        data.Remove(data[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
            return new { code = result, info = message, data = data, count = data.Count };

        }
        //获取技术规范的明细
        [HttpPost]
        public object GetTypeDetailList([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            bool ispage = true;
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            //ios 无是否分页
            if (res.Contains("allowPaging"))
            {
                ispage = dy.allowPaging;
            }
            string userId = dy.userId;
            string keyvalueTwo = dy.data.keyvalueTwo;
            string Id = dy.data.Id;
            string typeid = dy.data.typeid;
            long pageIndex = dy.data.pageIndex;//当前索引页
            long pageSize = dy.data.pageSize;//每页记录数
            UserEntity user = new UserBLL().GetEntity(userId);
            var data = default(List<SevenSEntity>);
            var returnData = new GetTypeDetailSevenS();

            //var ispush = false;
            try
            {
                // var DeptConfig = Config.GetValue("EmergencyCard");
                var url = Config.GetValue("AppUrl");
                var urlPDF = Config.GetValue("pdfview");
                //var deptName = deptBll.GetEntity(user.DepartmentId).FullName;
                //if (DeptConfig.Contains(","))
                //{
                //    var deptSplit = DeptConfig.Split(',');
                //    for (int i = 0; i < deptSplit.Count(); i++)
                //    {

                //        if (deptName == deptSplit[i])
                //        {
                //            ispush = true;
                //            break;
                //        }
                //    }
                //}
                //else
                //{
                //    if (DeptConfig == deptName)
                //    {
                //        ispush = true;
                //    }
                //}

                data = ebll.GetList("", typeid, keyvalueTwo, Id, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize), ispage, user.DepartmentId).ToList();
                foreach (var item in data)
                {
                    item.FilePath = url + item.FilePath.Substring(2, item.FilePath.Length - 2);
                    item.urlFilePath = urlPDF + item.FileId;
                }
                //returnData.ispush = ispush;
                returnData.entity = data;
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
            return new { code = result, info = message, data = returnData, count = returnData.entity.Count };

        }
        #endregion
        #region 定点照片
        /// <summary>
        /// 定时计划  推送未提交的班组
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object sendMessage(string PictureType)
        {

            var result = 0;
            var message = string.Empty;
            try
            {
                if (PictureType != "7S推送计划")
                {
                    return new { code = result, info = message };
                }
                ebll.sendMessage();
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };

            }
            return new { code = result, info = message };
        }

        /// <summary>
        /// 定时计划  初始化定时数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object StartPicture(string PictureType)
        {

            var result = 0;
            var message = string.Empty;
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                if (PictureType != "7S照片计划")
                {
                    return new { code = result, info = message };
                }
                logger.Info("————————————定点拍照数据生成————————————");
                var entity = bll.GetSubDepartments("", "班组");
                ebll.Start(entity);
                logger.Info("————————————定点拍照数据完成————————————");
            }
            catch (Exception ex)
            {
                logger.Info("————————————定点拍照数据错误————————————");
                var exNext = ex;
                int i = 0;
                while (true)
                {
                    i++;
                    if (i == 5)
                    {
                        break;
                    }
                    if (exNext.InnerException == null)
                    {
                        logger.Info("Message:" + ex.Message);
                        break;
                    }
                    exNext = exNext.InnerException;
                }

                logger.Info("——————————————————————————————————————————");
                return new { code = 1, info = ex.Message };

            }
            return new { code = result, info = message };
        }

        /// <summary>
        /// 查询定点照片数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SelectPicture([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            try
            {
                string userId = dy.userId;
                bool allowPaging = dy.allowPaging;

                string from = dy.data.from;
                string to = dy.data.to;
                string space = dy.data.space;
                string deptId = dy.data.deptId;
                UserEntity user = ubll.GetEntity(userId);
                DateTime? datefrom = null;
                DateTime? dateto = null;
                if (!string.IsNullOrEmpty(from))
                {
                    datefrom = Convert.ToDateTime(from);
                }
                if (!string.IsNullOrEmpty(to))
                {
                    dateto = Convert.ToDateTime(to);
                }
                var list = new List<SevenSPictureEntity>();
                if (allowPaging)
                {
                    var pageIndex = (int)dy.pageIndex;
                    var pageSize = (int)dy.pageSize;
                    var page = new Pagination();
                    page.page = pageIndex;
                    page.rows = pageSize;
                    list = ebll.getList(datefrom, dateto, "", "", space, page, true, deptId, true).ToList();

                }
                else
                {
                    list = ebll.getList(datefrom, dateto, "", "", space, new Pagination(), false, user.DepartmentId, true).ToList();
                }
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                foreach (var item in list)
                {
                    if (item.planeStartDate != null && item.planeEndDate != null)
                    {
                        item.planeTime = Convert.ToDateTime(item.planeStartDate).ToString("yyyy-MM-dd") + "~" + Convert.ToDateTime(item.planeEndDate).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        item.planeTime = null;
                    }

                    if (string.IsNullOrEmpty(item.evaluation))
                    {
                        item.evaluationState = "未评价";
                    }
                    else
                    {
                        item.evaluationState = "已评价";
                    }
                }
                foreach (var item in list)
                {
                    foreach (var pic in item.Files)
                    {
                        pic.FilePath = pic.FilePath.Replace("~/", url);
                    }
                    item.Files = item.Files.OrderBy(x => x.SortCode).ToList();
                }

                return new { code = result, info = message, data = list };
            }
            catch (Exception ex)
            {
               
                return new { code = 1, info = ex.Message };

            }

        }



        /// <summary>
        /// 查询定点设置位置
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SelectCycle([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            try
            {
                var data = ebll.getSet().OrderBy(x => x.createtime).ToList();
                return new { code = result, info = message, data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 查询定点设置位置
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object getRegulation([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            try
            {
                var data = ebll.getCycle().ToList();
                return new { code = result, info = message, data = data[0].regulation };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };

            }

        }

        /// <summary>
        /// 查询时间段
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SelectPlanTime([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            try
            {
                var data = ebll.getPlanTime().OrderByDescending(x => x.PlanTime).ToList();
                return new { code = result, info = message, data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };

            }

        }
        /// <summary>
        /// 查询定点设置位置 给与当前时间数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SelectPictureSpace([FromBody]JObject json)
        {

            var result = 0;
            var message = string.Empty;
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            string userId = dy.userId;
            UserEntity user = ubll.GetEntity(userId);
            try
            {
                var data = ebll.getSetAndPicture(user.DepartmentId);
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                foreach (var item in data)
                {
                    foreach (var pic in item.Files.OrderBy(x => x.SortCode))
                    {
                        pic.FilePath = pic.FilePath.Replace("~/", url);
                    }
                }
                return new { code = result, info = message, data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };

            }

        }
        /// <summary>
        /// 班组定点拍照数据  管理人员
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SelectPictureByManager([FromBody]JObject json)
        {
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            try
            {
                string userId = dy.userid;
                string state = dy.data.state; //已提交/未提交
                bool isOrder = dy.data.isorder;//是否按照默认排序来排列
                string evaluateStateStr = dy.data.evalstate;//评论状态  已评论/未评论
                string deptid = dy.data.deptid;
                bool? evaluateState = null;
                long pageIndexStr = dy.data.pageindex, pageSizeStr = dy.data.pagesize;//每页个数;
                int pageIndex = Convert.ToInt32(pageIndexStr);//页码
                int pageSize = Convert.ToInt32(pageSizeStr);//每页个数
                int totalCount = 0;//总条数
                if (!string.IsNullOrWhiteSpace(evaluateStateStr))
                {
                    if (evaluateStateStr == "已评价")
                        evaluateState = true;
                    else
                        evaluateState = false;

                }
                List<SevenSPictureEntity> date;
                if (isOrder)
                {
                    //按照班组提交的时间排序，最新的在最上方
                    date = ebll.GetListByManager(state, userId, evaluateState, deptid, out totalCount, pageIndex, pageSize);
                }
                else
                {
                    DateTime? datefrom = null;
                    DateTime? dateto = null;
                    string from = dy.data.from;
                    string to = dy.data.to;
                    if (!string.IsNullOrEmpty(from))
                    {
                        datefrom = Convert.ToDateTime(from);
                    }
                    if (!string.IsNullOrEmpty(to))
                    {
                        dateto = Convert.ToDateTime(to);
                    }
                    //按照系统设置的提交周期，获取时间段数据，最新的在最上方
                    date = ebll.GetListByManager(datefrom, dateto, state, userId, evaluateState, deptid, out totalCount, pageIndex, pageSize);
                }
                return new { code = 0, info = "查询成功", data = date, count = totalCount };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", ex.Message };
            }
        }
        /// <summary>
        /// 提交评论信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SubmitEvaluate([FromBody]JObject json)
        {
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            try
            {
                string userId = dy.userid;
                double point = dy.data.point;
                string content = dy.data.content;
                string evaluationdept = dy.data.evaluationdept;
                string evaluatedataid = dy.data.evaluatedataid;
                UserEntity user = ubll.GetEntity(userId);
                if (user != null) throw new Exception("查询不到当前登录人");
                SevensPictureEvaluationEntity evaluationEntity = new SevensPictureEvaluationEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = content,
                    CreateDate = DateTime.Now,
                    CreateUser = user.UserId,
                    CreateUserDeptId = user.DepartmentId,
                    CreateUserDeptName = user.DepartmentName,
                    CreateUserName = user.RealName,
                    EvaluateDataId = evaluatedataid,
                    EvaluationDept = evaluationdept,
                    ModifyDate = DateTime.Now,
                    ModifyUserId = user.UserId,
                    ModifyUserName = user.RealName,
                    Point = point
                };
                new SevensPictureEvaluationBLL().Insert(evaluationEntity);
                return new { code = 0, info = "请求成功" };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "请求失败", ex.Message };
            }
        }
        /// <summary>
        /// 详情  
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object PictureDetailManager([FromBody]JObject json)
        {
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            try
            {
                string userId = dy.userid;
                string dataid = dy.data.dataid;
                System.Collections.IList evaluationEntities = new SevensPictureEvaluationBLL().GetActivityEvaluateList(dataid);
                IList<FileInfoEntity> files = new FileInfoBLL().GetFilesByRecIdNew(dataid);
                var url = BSFramework.Util.Config.GetValue("AppUrl");

                foreach (var item in files)
                {
                    item.FilePath = item.FilePath.Replace("~/", url);
                }
                return new { code = 0, info = "查询成功", data = new { Evaluates = evaluationEntities, Files = files } };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", ex.Message }; ;
            }
        }

        /// <summary>
        /// 用户的历史评价
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object HistoryEvaluate([FromBody]JObject json)
        {
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            try
            {
                string userId = dy.userid;
                IList evaluationEntities = new SevensPictureEvaluationBLL().GetHistory(userId);
                return new { code = 0, info = "查询成功", data = evaluationEntities };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", ex.Message }; ;
            }
        }


        /// 上传图片删除图片
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object UploadSevenSPicture()
        {
            try
            {
                var GetJson = HttpContext.Current.Request["json"];
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(GetJson);
                string id = dy.data.id;//业务记录Id
                string userId = dy.userId;
                string space = dy.data.space;
                UserEntity user = new UserBLL().GetEntity(userId);
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                string getDel = dy.data.DelKeys;
                var DelKeys = getDel.Split(',');
                string keys = string.Empty;
                for (int i = 0; i < DelKeys.Length; i++)
                {
                    if (string.IsNullOrEmpty(DelKeys[i]))
                    {
                        continue;
                    }
                    FileInfoEntity fileList = fileBll.GetEntity(DelKeys[i]);
                    string url = Config.GetValue("FilePath") + fileList.FilePath.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(fileList.FilePath) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                    fileBll.Delete(DelKeys[i]);
                }
                FileInfoEntity fi = null;
                var data = ebll.getSet().OrderBy(x => x.createtime).ToList();
                var spaceEntity = data.FirstOrDefault(x => x.space == space);
                int spaceIndexOf = data.IndexOf(spaceEntity);
                foreach (string key in HttpContext.Current.Request.Files.AllKeys)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[key];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = id,
                        RecId = id,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/SevenS/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = space,
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName,
                        SortCode = spaceIndexOf

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\SevenS"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\SevenS");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\SevenS\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                    //var fileList = fileBll.GetFilesByRecIdNew(id);
                    //var spaceList = ebll.getSet();
                    //var ck = true;
                    //foreach (var item in spaceList)
                    //{
                    //    var one = fileList.Where(x => x.FileExtensions == item.space).ToList();
                    //    if (one.Count() == 0)
                    //    {
                    //        ck = false;
                    //    }
                    //}
                    //if (ck)
                    //{
                    //    ebll.update(id,userId);
                    //}

                }

                return new { code = 0, info = "操作成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        ///
        /// 上传图片删除图片
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object SevenSPictureFinish()
        {
            try
            {
                var GetJson = HttpContext.Current.Request["json"];
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(GetJson);
                string id = dy.data;//业务记录Id
                string userId = dy.userId;

                ebll.update(id, userId);
                var messagebll = new MessageBLL();

                messagebll.FinishTodo("7S定点照片到期提醒", dy.data);
                return new { code = 0, info = "操作成功" };

            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };

            }

        }


        #endregion

        #region 精益管理
        /// <summary>
        ///精益管理app 数据操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SevenSOfficeOperation()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<SevenSOffice>>(json);
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                UserEntity user = new UserBLL().GetEntity(dy.userId);
                string getDel = string.IsNullOrEmpty(dy.data.DelKeys) ? "" : dy.data.DelKeys;
                #region 删除数据
                if (!string.IsNullOrEmpty(dy.data.deldata))
                {
                    var fileList = fileBll.GetFilesByRecIdNew(dy.data.deldata).ToList();
                    getDel = string.Join(",", fileList.Select(x => x.FileId));
                }
                #endregion
                #region 修改删除图片
                var DelKeys = getDel.Split(',');
                string keys = string.Empty;
                for (int i = 0; i < DelKeys.Length; i++)
                {
                    if (string.IsNullOrEmpty(DelKeys[i]))
                    {
                        continue;
                    }
                    FileInfoEntity fileList = fileBll.GetEntity(DelKeys[i]);
                    string url = Config.GetValue("FilePath") + fileList.FilePath.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(fileList.FilePath) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                    fileBll.Delete(DelKeys[i]);
                }
                #endregion
                string id = string.Empty;
                //修改
                if (dy.data.updatedata != null)
                {
                    id = dy.data.updatedata.id;
                    dy.data.updatedata.modifydate = DateTime.Now;
                }
                if (dy.data.adddata != null)
                {
                    id = Guid.NewGuid().ToString();
                    dy.data.adddata.id = id;
                    dy.data.adddata.deptid = user.DepartmentId;
                    var dept = deptBll.GetEntity(user.DepartmentId);
                    var pdept = deptBll.GetEntity(dept.ParentId);
                    dy.data.adddata.deptname = dept.FullName;
                    dy.data.adddata.parentid = pdept.DepartmentId;
                    dy.data.adddata.parentname = pdept.FullName;
                    dy.data.adddata.createdate = DateTime.Now;
                    dy.data.adddata.modifydate = DateTime.Now;

                }
                #region 存储图片
                var Description = string.Empty;
                FileInfoEntity fi = null;
                foreach (string key in HttpContext.Current.Request.Files.AllKeys)
                {

                    HttpPostedFile file = HttpContext.Current.Request.Files[key];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    if (file.FileName.Contains("xz"))
                    {
                        Description = "xz";
                    }
                    if (file.FileName.Contains("ty"))
                    {
                        Description = "ty";
                    }
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = id,
                        RecId = id,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/SevenS/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = Description,
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\SevenS"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\SevenS");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\SevenS\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                #endregion
                ebll.Operation(dy.data.adddata, dy.data.updatedata, dy.data.deldata, dy.data.audit, dy.data.auditupdate);
                return new { info = "操作成功", code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }


        /// <summary>
        ///审核人获取数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetSevenSOfficeExtensions()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel>(json);
                var data = ebll.getOfficeByidExtensions(dy.userId);
                foreach (var item in data)
                {
                    if (item.proposedFiles == null)
                    {
                        item.proposedFiles = new List<FileInfoEntity>();
                    }
                    if (item.statusquoFiles == null)
                    {
                        item.statusquoFiles = new List<FileInfoEntity>();
                    }
                    foreach (var items in item.proposedFiles)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                    foreach (var items in item.statusquoFiles)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                }
                return new { info = "操作成功", code = result, data = data };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }



        /// <summary>
        /// 获取用户精益管理数据
        /// <returns></returns>
        [HttpPost]
        public object GetSevenSOffice()
        {
            var result = 0;
            var message = string.Empty;
            var dict = this.Request.Content.ReadAsFormDataAsync().Result;
            var json = dict.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<string>>(json);
                var data = ebll.getOfficebyuser(dy.userId);
                foreach (var item in data)
                {
                    if (item.proposedFiles == null)
                    {
                        item.proposedFiles = new List<FileInfoEntity>();
                    }
                    if (item.statusquoFiles == null)
                    {
                        item.statusquoFiles = new List<FileInfoEntity>();
                    }
                    foreach (var items in item.proposedFiles)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                    foreach (var items in item.statusquoFiles)
                    {
                        items.FilePath = items.FilePath.Replace("~/", url);
                    }
                }
                return new { info = "操作成功", code = result, data = data };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }
        /// <summary>
        ///统计数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetSevenTotal(BaseDataModel<SevenSTotal> dy)
        {
            var result = 0;
            var message = string.Empty;

            try
            {
                Dictionary<string, string> keyValue = new Dictionary<string, string>();
                keyValue.Add("deptid", dy.data.deptid);
                keyValue.Add("year", dy.data.year);
                var data = ebll.SelectTotalByDeptYear(keyValue, dy.userId);

                return new { info = "操作成功", code = result, data = data };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }


        #endregion
    }
}
