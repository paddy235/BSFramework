using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SafetyScore;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SafetyScore;
using BSFramework.Application.Entity.WorkMeeting.ViewModel;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.Log;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    /// 安全积分
    /// </summary>
    public class SafetyScoreController : BaseApiController
    {
        private readonly SafetyScoreBLL _safetyScoreBLL;
        public SafetyScoreController()
        {
            _safetyScoreBLL = new SafetyScoreBLL();
        }
        /// <summary>
        /// 根据安全积分规则 ，添加积分
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object AddScore([FromBody]JObject json)
        {
            WriteLog.AddLog($"/SafetyScore/AddScore 接收到参数{JsonConvert.SerializeObject(json)}", "SafetyScore");
            try
            {
                var dy = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userId = string.Empty,
                    data =-1
                });
                if (dy == null || string.IsNullOrEmpty(dy.userId) || dy.data == -1) throw new ArgumentNullException("参数不能为空");
                _safetyScoreBLL.AddScore(dy.userId, dy.data);
                return new { code = 0, info = "添加成功", data = "添加成功" };
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"/SafetyScore/AddScore  报错{JsonConvert.SerializeObject(ex)}", "SafetyScore");
                return new { code = -1, info = "添加失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 分页查询积分信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetSocrePagedList([FromBody]JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userId = string.Empty,
                    pageIndex = 1,
                    pageSize = 5,
                    data = new
                    {
                        searchDateStr = string.Empty,
                        keyWord = string.Empty,
                        userId = string.Empty
                    }
                });
                

             
            
                JObject jb = new JObject();
               
                if (!string.IsNullOrWhiteSpace(rq.data.keyWord)) jb.Add("keyWord", rq.data.keyWord);
                if (!string.IsNullOrWhiteSpace(rq.data.searchDateStr))
                {
                    DateTime searchDate;
                    if (DateTime.TryParse(rq.data.searchDateStr, out searchDate))
                    {
                        DateTime startTime = new DateTime(searchDate.Year, searchDate.Month, 1);
                        DateTime endTime = startTime.AddMonths(1).AddDays(-1);
                        jb.Add("startDate", startTime);
                        jb.Add("endDate", endTime);
                    }
                }
                var userEntity = new UserBLL().GetEntity(rq.userId);
                jb.Add("deptCode", userEntity.DepartmentCode);
                jb.Add("userId", rq.data.userId);
                Pagination pagination = new Pagination()
                {
                    page = rq.pageIndex,
                    rows = rq.pageSize,
                };

                IEnumerable<SafetyScoreEntity> list = _safetyScoreBLL.GetPagedList(pagination, JsonConvert.SerializeObject(jb));
                return new { code = 0, info = "查询成功", data = list, count = pagination.records };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 分页查询积分信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetSocrePagedListByUser([FromBody]JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userId = string.Empty,
                    pageIndex = 1,
                    pageSize = 5,
                    data = new
                    {
                        startDate = string.Empty,
                        endDate = string.Empty,
                        keyWord = string.Empty,
                        userId = string.Empty
                    }
                });




                JObject jb = new JObject();

                if (!string.IsNullOrWhiteSpace(rq.data.keyWord)) jb.Add("keyWord", rq.data.keyWord);
                if (!string.IsNullOrWhiteSpace(rq.data.startDate)) jb.Add("startDate", rq.data.startDate);
                if (!string.IsNullOrWhiteSpace(rq.data.endDate)) jb.Add("endDate", rq.data.endDate);
                //if (!string.IsNullOrWhiteSpace(rq.data.searchDateStr))
                //{
                //    DateTime searchDate;
                //    if (DateTime.TryParse(rq.data.searchDateStr, out searchDate))
                //    {
                //        DateTime startTime = new DateTime(searchDate.Year, searchDate.Month, 1);
                //        DateTime endTime = startTime.AddMonths(1).AddDays(-1);
                //        jb.Add("startDate", startTime);
                //        jb.Add("endDate", endTime);
                //    }
                //}
                var userEntity = new UserBLL().GetEntity(rq.userId);
                jb.Add("deptCode", userEntity.DepartmentCode);
                jb.Add("userId", rq.data.userId);
                Pagination pagination = new Pagination()
                {
                    page = rq.pageIndex,
                    rows = rq.pageSize,
                };

                IEnumerable<SafetyScoreEntity> list = _safetyScoreBLL.GetPagedList(pagination, JsonConvert.SerializeObject(jb));
                return new { code = 0, info = "查询成功", data = list, count = pagination.records };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 新增、修改
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Update()
        {
            try
            {
                WriteLog.AddLog($"  API   Update 编辑  \r\n {HttpContext.Current.Request["json"]}", "SafetyScore");
                var rq = JsonConvert.DeserializeAnonymousType(HttpContext.Current.Request["json"], new
                {
                    userId = string.Empty,
                    data = new {
                       entity = new SafetyScoreEntity(),
                       delFileIds = string.Empty
                    }
                });

                //删除图片
                if (!string.IsNullOrWhiteSpace(rq.data.delFileIds))
                {
                    Task.Run(() =>
                    {

                        string[] fileIds = rq.data.delFileIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        List<FileInfoEntity> delFiles = new FileInfoBLL().GetFileList(fileIds.ToList());
                        if (delFiles != null && delFiles.Count > 0)
                        {
                            delFiles.ForEach(fileList =>
                            {
                                try
                                {
                                    new FileInfoBLL().Delete(fileList.FileId);
                                    string url = Config.GetValue("FilePath") + fileList.FilePath.Replace("~/Resource", "").Replace("/", "\\");
                                    if (!string.IsNullOrEmpty(fileList.FilePath) && System.IO.File.Exists(url))
                                    {
                                        System.IO.File.Delete(url);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            });
                        }
                    });
                }
                var updateUser = new UserBLL().GetEntity(rq.userId);
                //entity的Id 为空，则代表新增 ，keyValue的值最终会成为entity.Id的值，entity的Id不为空则代表 修改
                if (!string.IsNullOrWhiteSpace(rq.data.entity.Id))
                {
                    //修改
                    SaveScoreFiles(updateUser, rq.data.entity.Id);
                    _safetyScoreBLL.SaveForm(rq.data.entity.Id, rq.data.entity);
                }
                else
                {
                    //新增
                    string keyValue = Guid.NewGuid().ToString();
                    SaveScoreFiles(updateUser, keyValue);
                    _safetyScoreBLL.SaveForm(keyValue, rq.data.entity);
                }
        

                return new { Code = 0, Info = "操作成功", data="操作成功" };
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($" API Update  错误 \r\n   错误信息 ：{ex.Message}\r\n {JsonConvert.SerializeObject(ex)}", "SafetyScore");
                return new { Code = -1, Info = "操作失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Remove([FromBody]JObject json)
        {
            try
            {
                WriteLog.AddLog($"  API Remove 删除  \r\n {JsonConvert.SerializeObject(json)}", "SafetyScore");
                var rq = JsonConvert.DeserializeAnonymousType(HttpContext.Current.Request["json"], new
                {
                    userId = string.Empty,
                    data =  string.Empty,
                });
                _safetyScoreBLL.Remove(rq.data);
                return new { Code = 0, Info = "操作成功", data = "操作成功" };
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"API Remove 错误 \r\n   错误信息 ：{ex.Message}\r\n {JsonConvert.SerializeObject(ex)}", "SafetyScore");
                return new { Code = -1, Info = "操作失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetDetail([FromBody]JObject json)
        {
            string res = json.Value<string>("json");
            dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            try
            {
                string dataid = dy.data;
                SafetyScoreEntity entity = _safetyScoreBLL.GetEntity(dataid);
                IList<FileInfoEntity> files = new FileInfoBLL().GetFilesByRecIdNew(dataid).OrderBy(x => x.CreateDate).ToList();
                var url = BSFramework.Util.Config.GetValue("AppUrl");

                foreach (var item in files)
                {
                    item.FilePath = item.FilePath.Replace("~/", url);
                }
                return new { code = 0, info = "查询成功", data = new { Entity = entity, Files = files } };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", ex.Message }; ;
            }
        }

        /// <summary>
        /// 本班组人员得分统计
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetUserScorePagedList([FromBody]JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userId = string.Empty,
                    pageIndex = 1,
                    pageSize = 5,
                    data = new
                    {
                        searchDateStr = string.Empty,
                        keyWord = string.Empty,
                        sidx = string.Empty,
                        sord = string.Empty,
                        userId = string.Empty
                    }
                });

                DateTime searchDate = DateTime.Parse(rq.data.searchDateStr);
                var userEntity = new UserBLL().GetEntity(rq.userId);
                var subDpetIds = new DepartmentBLL().GetSubDepartments(new string[] { userEntity.DepartmentId }).Select(x => x.DepartmentId);
                Expression<Func<UserEntity, bool>> userWhere = x => subDpetIds.Contains(x.DepartmentId);
                if (!string.IsNullOrWhiteSpace(rq.data.keyWord)) userWhere = userWhere.And(x => x.RealName.Contains(rq.data.keyWord));
                if (!string.IsNullOrWhiteSpace(rq.data.userId)) userWhere = userWhere.And(x => x.UserId == rq.data.userId);

                Pagination pagination = new Pagination()
                {
                    page = rq.pageIndex,
                    rows = rq.pageSize,
                    sidx = rq.data.sidx,
                    sord =rq.data.sord
                };

                object list = _safetyScoreBLL.GetUserScorePagedList(pagination, searchDate, userWhere);
                return new { code = 0, info = "查询成功", data = list, count = pagination.records };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message};
            }
        }



        /// <summary>
        /// 本班组人员得分统计
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetUserScorefirstthree([FromBody]JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userId = string.Empty,
                    pageIndex = 1,
                    pageSize = 5,
                    data = new
                    {
                        searchDateStr = string.Empty,
                        keyWord = string.Empty,
                        userId = string.Empty,
                        deptCode = string.Empty,
                        Type = string.Empty
                    }
                });

                JObject jb = new JObject();
                var url = Config.GetValue("WebIndexUrl");
                if (!string.IsNullOrWhiteSpace(rq.data.keyWord)) jb.Add("keyWord", rq.data.keyWord);
                if (!string.IsNullOrWhiteSpace(rq.data.deptCode)) jb.Add("deptCode", rq.data.deptCode);
                if (!string.IsNullOrWhiteSpace(rq.data.Type)) jb.Add("Type", rq.data.Type);
                //if (!string.IsNullOrWhiteSpace(rq.data.searchDateStr))
                //{
                //    DateTime searchDate;
                //    if (DateTime.TryParse(rq.data.searchDateStr, out searchDate))
                //    {
                //        jb.Add("serachDate", searchDate);
                //    }
                //}
                //var userEntity = new UserBLL().GetEntity(rq.userId);
                //jb.Add("deptCode", userEntity.DepartmentCode);
                //jb.Add("userId", rq.data.userId);
                Pagination pagination = new Pagination()
                {
                    page = rq.pageIndex,
                    rows = rq.pageSize,
                };

                object list = _safetyScoreBLL.GetUserScorefirstthree(pagination, JsonConvert.SerializeObject(jb));
                return new { code = 0, info = "查询成功", data = new { record= list,url= url+ "/SafetyScore/ScoreManage/ScoreIndex" }, count = pagination.records };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message };
            }
        }

        /// <summary>
        /// 个人得分统计
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetUserScoreInfo([FromBody]JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userId = string.Empty,
                });

                JObject jb = new JObject();


                Dictionary<int, List<KeyValue>> list = _safetyScoreBLL.GetUserScoreInfo(rq.userId);
              var data =  list.Select(p =>
                {
                    return new
                    {
                        Year = p.Key,
                        Data = p.Value.Select(x => new { x.Key, x.Value })
                    };
                });
                return new { code = 0, info = "查询成功", data };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message };
            }
        }
        #region  保存附件
        /// <summary>
        /// 保存上传的附件
        /// </summary>
        /// <param name="updateUser">上传者的用户信息</param>
        /// <param name="keyValue">附件绑定的数据的主键</param>
        private static void SaveScoreFiles(UserEntity updateUser, string keyValue)
        {
            if (HttpContext.Current.Request.Files !=null && HttpContext.Current.Request.Files.Count>0)
            {
                int sort = 0;
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[i];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    FileInfoEntity fileEntity = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = keyValue,
                        RecId = keyValue,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/SafetyScore/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = updateUser.UserId,
                        CreateUserName = updateUser.RealName,
                        SortCode = sort++

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\SafetyScore"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\SafetyScore");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\SafetyScore\\" + fileId + ext);
                    //保存附件信息
                    new FileInfoBLL().SaveForm(fileEntity);
                }
            }
        }
        #endregion
    }
}
