using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.DeviceInspection;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.DeviceInspection;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util.Log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class DeviceInspectionController : BaseApiController
    {
        private readonly DeviceInspectionBLL _inspectionbll = new DeviceInspectionBLL();
        private readonly DeviceInspectionJobBLL _inspectionjobbll = new DeviceInspectionJobBLL();
        private readonly InspectionRecordBLL _recordBll = new InspectionRecordBLL();
        private readonly FileInfoBLL _fileBll = new FileInfoBLL();
        private readonly UserBLL _userbll = new UserBLL();

        /// <summary>
        /// 获取单条设备巡回检查记录的信息
        /// 包括暂存于服务器的信息
        /// </summary>
        /// <param name="json">deviceId设备巡回检查表的主键 recordId设备巡回检查记录的Id</param>
        /// <returns></returns>
        [HttpPost]
        public object GetDeviceRecord([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string deviceId = dy.data.deviceId;//设备巡回检查表Id
                string recordId = dy.data.recordId;//巡查记录的Id
                                                   //1.获取单个检查记录数据
                InspectionRecordEntity inspection = _recordBll.GetEntity(recordId);
                if (inspection != null)
                {
                    //2.获取检查记录各项的信息
                    var items = _recordBll.GetRecordItems(recordId, inspection.DeviceId);
                    //3.附件
                    IList<FileInfoEntity> files = _fileBll.GetFilesByRecIdNew(inspection.Id);
                    var url = BSFramework.Util.Config.GetValue("AppUrl");
                    foreach (var pic in files)
                    {
                        pic.FilePath = pic.FilePath.Replace("~/", url);
                    }
                    files = files.OrderBy(x => x.SortCode).ToList();

                    return new
                    {
                        code = 0,
                        info = "查询成功",
                        data = new
                        {
                            inspection = inspection,
                            items = items,
                            files = files,
                            inspection.IsSubmit
                        }
                    };
                }
                else
                {
                    //返回设备巡回检查表的信息，不包含检查的信息
                    var entity = _inspectionjobbll.GetEntity(deviceId);
                    var items = _inspectionjobbll.GetDeviceInspectionItems(deviceId);
                    return new
                    {
                        code = 0,
                        info = "查询成功",
                        data = new
                        {
                            inspection = entity,
                            items = items,
                            IsSubmit = -1
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message };
            }
        }

        [HttpPost]
        /// <summary>
        /// 上传设备巡回检查记录的数据
        /// 包含图片信息
        /// </summary>
        /// <returns></returns>
        public object SubmitRecord()
        {
            try
            {
                var json = HttpContext.Current.Request["json"];
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json);
                var dy = JsonConvert.DeserializeAnonymousType(json, new
                {
                    userId = string.Empty,
                    data = new
                    {
                        delKeys = string.Empty,
                        record = string.Empty,
                        deviceId = string.Empty,
                        recordId = string.Empty,
                        WorkuserId = string.Empty,
                        Workuser = string.Empty,
                        place = string.Empty,//0 任务  1代办
                        isSubmit = 0,
                        items = new List<DeviceInspectionItemEntity>()
                    }
                });
                string userId = dy.userId;//用户的Id
                string delKeys = dy.data.delKeys;//要删除的文件的Id
                string record = dy.data.record;//检查记录
                string deviceId = dy.data.deviceId;//设备巡回检查的Id
                string recordId = dy.data.recordId;//检查记录的Id
                string Workuser = dy.data.Workuser;//设备巡回检查的Id
                string WorkuserId = dy.data.WorkuserId;//检查记录的Id
                //string jobId = dy.data.jobId;//任务的Id
                int isSubmit = dy.data.isSubmit;//是否提交 
                List<DeviceInspectionItemEntity> items = dy.data.items;
                UserEntity user = new UserBLL().GetEntity(userId);

                InspectionRecordEntity recordEntity;//巡回记录表

                bool isUpdate = true;//是否是修改
                recordEntity = _recordBll.GetEntity(recordId);
                if (recordEntity == null) isUpdate = false;

                if (isUpdate)
                {
                    //修改
                    recordEntity.Record = record;
                    recordEntity.IsSubmit = isSubmit;
                    recordEntity.DeviceId = deviceId;
                    recordEntity.Workuser = Workuser;
                    recordEntity.WorkuserId = WorkuserId;
                    var inspectionEntity = _inspectionjobbll.GetEntity(deviceId);
                    if (inspectionEntity != null)
                    {
                        inspectionEntity.Workuser = recordEntity.Workuser;
                        inspectionEntity.WorkuserId = recordEntity.WorkuserId;
                        _inspectionjobbll.SaveForm(deviceId, inspectionEntity, null);
                    }

                    if (isSubmit == 1)
                    {
                        _inspectionjobbll.SaveInspectionState(deviceId);
                    }

                }
                else
                {
                    //新增
                    recordEntity = new InspectionRecordEntity();
                    recordEntity.Create(user);
                    recordEntity.CreateUserDeptName = new DepartmentBLL().GetEntity(user.DepartmentId)?.FullName;
                    recordEntity.Id = string.IsNullOrEmpty(recordId) ? Guid.NewGuid().ToString() : recordId;
                    recordEntity.Record = record;
                    recordEntity.JobId = recordId;
                    recordEntity.Workuser = Workuser;
                    recordEntity.WorkuserId = WorkuserId;
                    if (dy.data.place == "0")
                    {
                        var inspectionEntity = _inspectionjobbll.GetEntity(deviceId);

                        var getDeviceId = string.Empty;
                        recordEntity.InspectionName = inspectionEntity.InspectionName;
                        recordEntity.DeviceSystem = inspectionEntity.DeviceSystem;
                        getDeviceId = inspectionEntity.Id;
                        recordEntity.IsSubmit = isSubmit;
                        recordEntity.DeviceId = getDeviceId;
                        inspectionEntity.recordId = recordEntity.Id;
                        _inspectionjobbll.SaveForm(deviceId, inspectionEntity, null);
                        if (isSubmit == 1)
                        {
                            _inspectionjobbll.SaveInspectionState(deviceId);
                        }

                    }
                    else
                    {
                        var inspectionEntity = _inspectionbll.GetEntity(deviceId);

                        var getDeviceId = string.Empty;
                        recordEntity.InspectionName = inspectionEntity.InspectionName;
                        recordEntity.DeviceSystem = inspectionEntity.DeviceSystem;
                        getDeviceId = inspectionEntity.Id;
                        recordEntity.IsSubmit = isSubmit;

                        var InspectionItems = _inspectionbll.GetDeviceInspectionItems(inspectionEntity.Id);
                        //转json
                        var inspectionEntityStr = JsonConvert.SerializeObject(inspectionEntity);
                        var InspectionItemsStr = JsonConvert.SerializeObject(InspectionItems);
                        //转化实体
                        var inspectionJobEntity = JsonConvert.DeserializeObject<DeviceInspectionJobEntity>(inspectionEntityStr);
                        var InspectionJobItems = JsonConvert.DeserializeObject<List<DeviceInspectionItemJobEntity>>(InspectionItemsStr);
                        var keyvalue = Guid.NewGuid().ToString();
                        inspectionJobEntity.Id = keyvalue;
                        inspectionJobEntity.recordId = recordEntity.Id;
                        getDeviceId = keyvalue;
                        if (isSubmit == 1)
                        {
                            inspectionJobEntity.State = true;
                        }
                        inspectionJobEntity.Workuser = recordEntity.Workuser;
                        inspectionJobEntity.WorkuserId = recordEntity.WorkuserId;
                        _inspectionjobbll.SaveForm("", inspectionJobEntity, InspectionJobItems);

                        var now = _inspectionjobbll.GetDeviceInspectionItems(keyvalue);
                        foreach (var item in items)
                        {
                            var nowEntity = now.FirstOrDefault(x => x.ItemName == item.ItemName && x.Method == item.Method && x.Standard == item.Standard);
                            if (nowEntity != null)
                            {
                                item.Id = nowEntity.Id;
                            }
                        }
                        recordEntity.DeviceId = getDeviceId;
                    }





                }


                List<ItemResultEntity> results = new List<ItemResultEntity>();
                #region 1、组装检查项
                if (items != null && items.Count > 0)
                {
                    items.ForEach(p =>
                    {
                        results.Add(new ItemResultEntity()
                        {
                            Id = Guid.NewGuid().ToString(),
                            ItemId = p.Id,
                            Result = p.Result,
                            RecordId = recordEntity.Id
                        }); ;
                    });
                }
                else
                {
                    throw new ArgumentNullException("data.inspectionItems检查项为空");
                }
                #endregion
                List<FileInfoEntity> files = new List<FileInfoEntity>();
                #region 2、保存图片
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[i];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    FileInfoEntity fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = recordEntity.Id,
                        RecId = recordEntity.Id,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/Inspection/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName,

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Inspection"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Inspection");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Inspection\\" + fileId + ext);
                    //保存附件信息
                    files.Add(fi);
                }
                #endregion
                List<FileInfoEntity> delFileList = new List<FileInfoEntity>();
                if (!string.IsNullOrWhiteSpace(delKeys))
                {
                    delFileList = _fileBll.GetFileListByIds(delKeys);
                    if (delFileList != null && delFileList.Count > 0)
                    {
                        delFileList.ForEach(p =>
                        {
                            p.FilePath = HttpContext.Current.Server.MapPath(p.FilePath);
                        });
                    }
                }
                _recordBll.SaveRecord(recordEntity, results, files, delFileList, isUpdate);
                return new { code = 0, info = "操作成功" };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "操作失败", data = ex.Message };
            }
        }
        [HttpPost]
        /// <summary>
        /// 分页查询设备巡回检查表数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public object GetInspectionPageList([FromBody] JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userid = string.Empty,
                    pageIndex = 1,
                    pageSize = 5,
                    data = new
                    {
                        keyWord = string.Empty//关键字
                    }
                });
                UserEntity userEntity = _userbll.GetEntity(rq.userid);
                int totalCount = 0;
                List<DeviceInspectionEntity> inspectionEntities = _inspectionbll.GetPageList(rq.pageIndex, rq.pageSize, userEntity.DepartmentCode, rq.data.keyWord, ref totalCount);
                return new { code = 0, info = "查询成功", data = inspectionEntities, count = totalCount };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message, count = 0 };
            }
        }
        [HttpPost]
        /// <summary>
        /// 获取设备巡回检查表的详情 包括检查项
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public object GetInspectionInfo([FromBody] JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userid = string.Empty,
                    data = new
                    {
                        id = string.Empty
                    }
                });
                if (rq.data == null) throw new ArgumentNullException("参数有误：data为空");
                if (rq.data.id == null) throw new ArgumentNullException("参数有误：data.id为空");
                DeviceInspectionEntity inspectionEntity = _inspectionbll.GetEntity(rq.data.id);
                List<DeviceInspectionItemEntity> items = _inspectionbll.GetDeviceInspectionItems(rq.data.id);
                return new { code = 0, info = "查询成功", data = new { inspection = inspectionEntity, items = items } };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message, count = 0 };
            }
        }

        /// <summary>
        /// 新增、修改 设备巡回检查表
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        [HttpPost]
        public object SaveInspection([FromBody] JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userid = string.Empty,
                    data = new
                    {
                        inspection = new DeviceInspectionEntity(),
                        items = new List<DeviceInspectionItemEntity>()
                    }
                });
                if (rq.data == null) throw new ArgumentNullException("参数有误：data为空");
                if (rq.data.inspection == null) throw new ArgumentNullException("参数有误：data.inspection为空");
                if (rq.data.items == null || rq.data.items.Count < 1) throw new ArgumentNullException("参数有误：data.items应至少有一行数据");
                UserEntity user = new UserBLL().GetEntity(rq.userid);
                _inspectionbll.SaveForm(rq.data.inspection.Id, rq.data.inspection, rq.data.items);
                return new { code = 0, info = "操作成功" };
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"参数：{JsonConvert.SerializeObject(json)}\r\n错误消息{ex.Message}", "DeviceInspection");
                return new { code = -1, info = "操作成功", data = ex.Message };
            }
        }

        /// <summary>
        /// 删除 设备巡回检查表
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        [HttpPost]
        public object DelInspection([FromBody] JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userid = string.Empty,
                    data = new
                    {
                        id = string.Empty
                    }
                });
                if (rq.data == null) throw new ArgumentNullException("参数有误：data为空");
                if (rq.data.id == null) throw new ArgumentNullException("参数有误：data.id");
                _inspectionbll.RemoveInspection(rq.data.id);
                return new { code = 0, info = "操作成功" };
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"参数：{JsonConvert.SerializeObject(json)}\r\n错误消息{ex.Message}", "DeviceInspection");
                return new { code = -1, info = "操作失败", data = ex.Message };
            }
        }
        [HttpPost]
        /// <summary>
        /// 分页查询检查记录
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public object GetRecordPageList([FromBody] JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userid = string.Empty,
                    pageIndex = 1,
                    pageSize = 5,
                    data = new
                    {
                        keyWord = string.Empty,//设备系统
                        userId = string.Empty,//检查人
                        jobId = string.Empty,
                        issubmit = string.Empty,
                        time = string.Empty,
                        deptcode = string.Empty
                    }
                }); ;
                int totalCount = 0;
                var deptcode = rq.data.deptcode;
                if (string.IsNullOrEmpty(rq.data.userId) && string.IsNullOrEmpty(rq.data.deptcode))
                {
                    UserEntity user = new UserBLL().GetEntity(rq.userid);
                    deptcode = user.DepartmentCode;
                }
                List<InspectionRecordEntity> record = _recordBll.GetPageList(rq.pageIndex, rq.pageSize, rq.data.keyWord, rq.data.userId, rq.data.jobId, rq.data.issubmit, rq.data.time, deptcode, ref totalCount);
                return new { code = 0, info = "查询成功", data = record, count = totalCount };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message, count = 0 };
            }
        }

        /// <summary>
        /// 检查表名称的唯一验证
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object CheckName([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string name = dy.data.name;
                string id = dy.data.id;
                bool reuslt = _inspectionbll.ExistInspectionName(id, name);
                return new { code = 0, info = "查询成功", data = reuslt };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message };
            }
        }
        [HttpPost]
        /// <summary>
        /// 获取下拉列表的数据 所有的检查表
        /// </summary>
        /// <returns></returns>
        public object GetSelectList()
        {
            try
            {
                var list = _inspectionbll.GetAllInspetionList();
                foreach (var item in list)
                {
                    item.DeviceInspectionItem = _inspectionbll.GetDeviceInspectionItems(item.Id);
                }
                return new { code = 0, info = "查询成功", data = list };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message };
            }
        }

        [HttpPost]
        /// <summary>
        /// 获取代办巡回检查表的详情 包括检查项
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public object GetInspectionInfojob([FromBody] JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userid = string.Empty,
                    data = new
                    {
                        id = string.Empty
                    }
                });
                if (rq.data == null) throw new ArgumentNullException("参数有误：data为空");
                if (rq.data.id == null) throw new ArgumentNullException("参数有误：data.id为空");
                var inspectionEntity = _inspectionjobbll.GetEntity(rq.data.id);
                var items = _inspectionjobbll.GetDeviceInspectionItems(rq.data.id);
                return new { code = 0, info = "查询成功", data = new { inspection = inspectionEntity, items = items } };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message, count = 0 };
            }
        }
        [HttpPost]
        /// <summary>
        /// 获取代办巡回检查表的详情 包括检查项
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public object GetEntityByMeetOrJob([FromBody] JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userid = string.Empty,
                    data = new
                    {
                        meetid = string.Empty,
                        jobid = string.Empty
                    }
                });
                if (rq.data == null) throw new ArgumentNullException("参数有误：data为空");

                var inspectionEntity = _inspectionjobbll.GetEntityByMeetOrJob(rq.data.meetid, rq.data.jobid);

                return new { code = 0, info = "查询成功", data = new { inspection = inspectionEntity } };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message, count = 0 };
            }
        }
        /// <summary>
        /// 修改state
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        [HttpPost]
        public object SaveInspectionState([FromBody] JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userid = string.Empty,
                    data = new
                    {
                        id = string.Empty
                    }
                });

                _inspectionjobbll.SaveInspectionState(rq.data.id);
                return new { code = 0, info = "操作成功" };
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"参数：{JsonConvert.SerializeObject(json)}\r\n错误消息{ex.Message}", "DeviceInspection");
                return new { code = -1, info = "操作失败", data = ex.Message };
            }
        }
        /// <summary>
        /// 新增、修改 设备巡回检查代办
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        [HttpPost]
        public object SaveInspectionjob([FromBody] JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userid = string.Empty,
                    data = new
                    {
                        inspection = new DeviceInspectionJobEntity(),
                        items = new List<DeviceInspectionItemJobEntity>()
                    }
                });
                if (rq.data == null) throw new ArgumentNullException("参数有误：data为空");
                if (rq.data.inspection == null) throw new ArgumentNullException("参数有误：data.inspection为空");
                if (rq.data.items == null || rq.data.items.Count < 1) throw new ArgumentNullException("参数有误：data.items应至少有一行数据");
                UserEntity user = new UserBLL().GetEntity(rq.userid);
                _inspectionjobbll.SaveForm(rq.data.inspection.Id, rq.data.inspection, rq.data.items);
                return new { code = 0, info = "操作成功" };
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"参数：{JsonConvert.SerializeObject(json)}\r\n错误消息{ex.Message}", "DeviceInspection");
                return new { code = -1, info = "操作成功", data = ex.Message };
            }
        }
        [HttpPost]
        /// <summary>
        /// 分页查询设备巡回检查表数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public object GetInspectionPageListjob([FromBody] JObject json)
        {
            try
            {
                var rq = JsonConvert.DeserializeAnonymousType(json.Value<string>("json"), new
                {
                    userid = string.Empty,
                    pageIndex = 1,
                    pageSize = 5,
                    data = new
                    {
                        keyWord = string.Empty,//关键字
                        state = string.Empty,
                        deptcode = string.Empty,
                        userid = string.Empty
                    }
                });
                UserEntity userEntity = _userbll.GetEntity(rq.userid);
                int totalCount = 0;
                List<DeviceInspectionJobEntity> inspectionEntities = _inspectionjobbll.GetPageList(rq.pageIndex, rq.pageSize, rq.data.deptcode, rq.data.keyWord, rq.data.state, rq.data.userid, ref totalCount);
                return new { code = 0, info = "查询成功", data = inspectionEntities, count = totalCount };
            }
            catch (Exception ex)
            {
                return new { code = -1, info = "查询失败", data = ex.Message, count = 0 };
            }
        }
    }
}