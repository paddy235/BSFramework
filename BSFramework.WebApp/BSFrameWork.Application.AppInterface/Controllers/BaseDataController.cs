using BSFramework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Text;
using ThoughtWorks.QRCode.Codec;
using System.IO;
using BSFramework.Application.Code;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Busines.WorkMeeting;
using System.Text.RegularExpressions;
using Aspose.Cells;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class BaseDataController : ApiController
    {
        DepartmentBLL dtbll = new DepartmentBLL();
        PeopleBLL pbll = new PeopleBLL();
        UserBLL ubll = new UserBLL();
        RoleBLL rbll = new RoleBLL();
        FileInfoBLL fileBll = new FileInfoBLL();
        WorkmeetingBLL workmeetingbll = new WorkmeetingBLL();
        WorkOrderBLL orderbll = new WorkOrderBLL();
        WorkSettingBLL settingbll = new WorkSettingBLL();
        DepartmentBLL dept = new DepartmentBLL();
        /// <summary>
        /// 获取工作任务
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllJobTemplates([FromBody]JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("data");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                string name = dy.data.name;
                string jobplantype = dy.data.jobplantype;

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                List<JobTemplateEntity> list = workmeetingbll.GetBaseData(user.DepartmentId, name, jobplantype, int.Parse(pageSize.ToString()), int.Parse(pageIndex.ToString()), out total);


                return new { code = 0, info = "获取数据成功", count = total, data = list };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 新增工作任务
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Add([FromBody]JObject jobject)
        {

            string res = jobject.Value<string>("json");
            var json = JObject.Parse(res.ToString());
            string userId = json.Value<string>("userId");
            UserEntity user = new UserBLL().GetEntity(userId);
            JobTemplateEntity model = JsonConvert.DeserializeObject<JobTemplateEntity>(json["data"].ToString());

            try
            {
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string userId = dy.userId;
                //string jobTemplateEntity = JsonConvert.SerializeObject(dy.data);
                //JobTemplateEntity model = JsonConvert.DeserializeObject<JobTemplateEntity>(jobTemplateEntity);
                //UserEntity user = new UserBLL().GetEntity(userId);
                //string id = Guid.NewGuid().ToString();
                //model.JobId = id;
                string id = null;
                //string time = dy.data.jobTime;
                //var reg = new Regex("^(\\d{2}):(\\d{2}) - (\\d{2}):(\\d{2})$");
                //if (reg.IsMatch(time))
                //{
                //    var matches = reg.Matches(time);
                //    model.JobStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(matches[0].Groups[1].Value), int.Parse(matches[0].Groups[2].Value), 0);
                //    model.JobEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(matches[0].Groups[3].Value), int.Parse(matches[0].Groups[4].Value), 0);
                //}
                //else
                //{
                //    model.JobStartTime = model.JobEndTime = DateTime.Now;
                //}

                model.CycleDate = model.CycleDate ?? string.Empty;
                model.JobId = id;
                model.DeptId = user.DepartmentId;
                model.CreateUser = user.RealName;
                model.CreateUserId = user.UserId;
                model.RedactionPerson = user.RealName;
                if (model.DangerousList == null) model.DangerousList = new List<JobDangerousEntity>();

                if (!string.IsNullOrEmpty(model.Dangerous))
                {
                    var dangerArray = model.Dangerous.Split('。');
                    var dangerMeasureArray = model.Measure?.Split('。');
                    for (int i = 0; i < dangerArray.Length; i++)
                    {
                        var danger = dangerArray[i];
                        if (string.IsNullOrEmpty(danger))
                        {
                            continue;
                        }
                        var templateDangerousEntity = new JobDangerousEntity { Content = danger };
                        if (dangerMeasureArray != null && dangerMeasureArray.Length > i)
                        {
                            templateDangerousEntity.MeasureList = new List<JobMeasureEntity>();
                            var dangerMeasure = dangerMeasureArray[i];
                            if (!string.IsNullOrEmpty(dangerMeasure))
                            {
                                var measureArray = dangerMeasure.Split('；');
                                foreach (var measure in measureArray)
                                {
                                    if (string.IsNullOrEmpty(measure))
                                    {
                                        continue;
                                    }
                                    templateDangerousEntity.MeasureList.Add(new JobMeasureEntity { Content = measure });
                                }
                            }
                        }
                        model.DangerousList.Add(templateDangerousEntity);
                    }
                }
                foreach (var item in model.DangerousList)
                {
                    if (string.IsNullOrEmpty(item.JobDangerousId)) item.JobDangerousId = Guid.NewGuid().ToString();
                    item.CreateTime = DateTime.Now;
                    item.JobId = model.JobId;
                    if (item.MeasureList == null) item.MeasureList = new List<JobMeasureEntity>();
                    foreach (var item1 in item.MeasureList)
                    {
                        if (string.IsNullOrEmpty(item1.JobMeasureId)) item1.JobMeasureId = Guid.NewGuid().ToString();
                        item1.CreateTime = DateTime.Now;
                        item1.JobDangerousId = item.JobDangerousId;
                    }
                }
                string r = workmeetingbll.UpdateJobTemplate(model);
                return new { code = 0, info = r };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 修改工作任务
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Edit([FromBody]JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                string jobTemplateEntity = JsonConvert.SerializeObject(dy.data);
                JobTemplateEntity model = JsonConvert.DeserializeObject<JobTemplateEntity>(jobTemplateEntity);
                UserEntity user = new UserBLL().GetEntity(userId);
                //string time = dy.data.jobTime;
                //var reg = new Regex("^(\\d{2}):(\\d{2}) - (\\d{2}):(\\d{2})$");
                //if (reg.IsMatch(time))
                //{
                //    var matches = reg.Matches(time);
                //    model.JobStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(matches[0].Groups[1].Value), int.Parse(matches[0].Groups[2].Value), 0);
                //    model.JobEndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(matches[0].Groups[3].Value), int.Parse(matches[0].Groups[4].Value), 0);
                //}
                //else
                //{
                //    model.JobStartTime = model.JobEndTime = DateTime.Now;
                //}

                model.CycleDate = model.CycleDate ?? string.Empty;
                //model.JobId = id;
                model.DeptId = user.DepartmentId;
                model.CreateUserId = user.UserId;
                model.CreateUser = user.CreateUserName;


                if (model.DangerousList == null) model.DangerousList = new List<JobDangerousEntity>();

                if (!string.IsNullOrEmpty(model.Dangerous))
                {
                    var dangerArray = model.Dangerous.Split('。');
                    var dangerMeasureArray = model.Measure?.Split('。');
                    for (int i = 0; i < dangerArray.Length; i++)
                    {
                        var danger = dangerArray[i];
                        if (string.IsNullOrEmpty(danger))
                        {
                            continue;
                        }
                        var templateDangerousEntity = new JobDangerousEntity { Content = danger };
                        if (dangerMeasureArray != null && dangerMeasureArray.Length > i)
                        {
                            templateDangerousEntity.MeasureList = new List<JobMeasureEntity>();
                            var dangerMeasure = dangerMeasureArray[i];
                            if (!string.IsNullOrEmpty(dangerMeasure))
                            {
                                var measureArray = dangerMeasure.Split('；');
                                foreach (var measure in measureArray)
                                {
                                    if (string.IsNullOrEmpty(measure))
                                    {
                                        continue;
                                    }
                                    templateDangerousEntity.MeasureList.Add(new JobMeasureEntity { Content = measure });
                                }
                            }
                        }
                        model.DangerousList.Add(templateDangerousEntity);
                    }
                }
                foreach (var item in model.DangerousList)
                {
                    if (string.IsNullOrEmpty(item.JobDangerousId)) item.JobDangerousId = Guid.NewGuid().ToString();
                    item.CreateTime = DateTime.Now;
                    item.JobId = model.JobId;
                    if (item.MeasureList == null) item.MeasureList = new List<JobMeasureEntity>();
                    foreach (var item1 in item.MeasureList)
                    {
                        if (string.IsNullOrEmpty(item1.JobMeasureId)) item1.JobMeasureId = Guid.NewGuid().ToString();
                        item1.CreateTime = DateTime.Now;
                        item1.JobDangerousId = item.JobDangerousId;
                    }
                }

                string r = workmeetingbll.UpdateJobTemplate(model);
                return new { code = 0, info = r };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 删除工作任务
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Delete([FromBody]JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string jobId = dy.data;
                workmeetingbll.DeleteJobTemplate(jobId);
                return new { code = 0, info = "删除数据成功" };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 工作任务班次
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object WorkSetSelect([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string deptid = dy.data;
                //var setting = settingbll.GetList("");
                ////var pDeptid = dept.GetParent(deptid,"部门");
                //var setupid = string.Empty;
                //var createuserid = string.Empty;
                //orderbll.GetWorkSettingByDept(deptid, out setupid, out createuserid);
                //var data = setting.OrderBy(x => x.StartTime).Where(x => x.WorkSetupId == setupid);
                var data = new WorkOrderBLL().getSetValue(deptid);
                var resultdata = data.Select(x => new { text = x.Value, value = x.Key });
                return new { code = 0, info = "成功", data = resultdata };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        /// 获取工作任务详情
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetDetail([FromBody]JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string jobId = dy.data;
                var entity = workmeetingbll.GetJobTemplate(jobId);

                return new { code = 0, info = "获取数据成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }


        /// <summary>
        /// 获取工作任务详情
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetDetailByMeetingJobId([FromBody]JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                string meetingJobId = dy.data;
                var entity = workmeetingbll.GetJobTemplateByMeetingJobId(meetingJobId);

                return new { code = 0, info = "获取数据成功", data = entity };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 人员列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetUsers([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userId;
                string name = dy.data;
                UserEntity user = ubll.GetEntity(userId);
                List<UserEntity> u_list = new List<UserEntity>();
                DepartmentEntity dept = dtbll.GetEntity(user.DepartmentId);

                u_list = ubll.GetDeptUsers(user.DepartmentId).ToList(); //只能选择本班组成员
                u_list = u_list.Where(x => x.RealName.Contains(name) && x.DepartmentId != "0").ToList();


                return new { info = "成功", code = 0, count = u_list.Count, data = u_list };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object DoImport()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                HttpFileCollection files = HttpContext.Current.Request.Files;
                if (files.Count == 0)
                {
                    return new { info = "请上传文件", code = 0, count = 0, data = new { } };
                }
                if (!files[0].FileName.EndsWith(".xlsx"))
                {
                    return new { info = "请上传Excel文件", code = 0, count = 0, data = new { } };
                }
                string userid = dy.userId;
                UserEntity user = ubll.GetEntity(userid);

                var book = new Workbook(files[0].InputStream);
                var sheet = book.Worksheets[0];

                var templates = new List<JobTemplateEntity>();
                var date = DateTime.Now;
                UserBLL userBLL = new UserBLL();
                var users = userBLL.GetDeptUsers(user.DepartmentId).ToList();
                for (int i = 2; i <= sheet.Cells.MaxDataRow; i++)
                {
                    var entity = new JobTemplateEntity();
                    entity.JobId = Guid.NewGuid().ToString();
                    entity.JobContent = sheet.Cells[i, 0].StringValue;
                    if (string.IsNullOrEmpty(entity.JobContent))
                    {

                        if (templates.Count > 0 && string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue))
                        {
                            break;
                        }
                        return new { info = "填写工作任务", code = 1, count = 0, data = new { } };

                    }
                    var sss = sheet.Cells[i, 1].StringValue;
                    if (!string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue))
                    {
                        var itemdetialbll = new DataItemDetailBLL();
                        var itembll = new DataItemBLL();
                        var type = itembll.GetEntityByName("任务库任务类型");
                        var content = itemdetialbll.GetList(type.ItemId).ToList();
                        var typename = sheet.Cells[i, 1].StringValue.Trim();
                        var gettype = content.FirstOrDefault(row => row.ItemName == typename);
                        if (gettype == null)
                        {
                            return new { info = "不存在该类型", code = 1, count = 0, data = new { } };
                        }
                        entity.jobplantype = gettype.ItemName;
                        entity.jobplantypeid = gettype.ItemId;
                    }
                    else
                    {
                        return new { info = "类型不能为空", code = 1, count = 0, data = new { } };
                    }

                    entity.RiskLevel = sheet.Cells[i, 2].StringValue;
                    if (string.IsNullOrEmpty(entity.RiskLevel))
                    {
                        return new { info = "填写风险等级", code = 1, count = 0, data = new { } };
                    }
                    var jobperson = sheet.Cells[i, 3].StringValue;
                    if (jobperson.Contains(","))
                    {
                        var person = string.Empty;
                        var personid = string.Empty;
                        var personList = jobperson.Split(',');
                        for (int j = 0; j < personList.Length; j++)
                        {
                            var ckjobperson = users.FirstOrDefault(row => row.RealName == personList[j]);
                            if (ckjobperson == null)
                            {
                                return new { info = "作业人错误", code = 1, count = 0, data = new { } };
                            }
                            if (j >= personList.Length - 1)
                            {
                                person += personList[j];
                                personid += ckjobperson.DepartmentId;
                            }
                            else
                            {
                                person += personList[j] + ",";
                                personid += ckjobperson.DepartmentId + ",";

                            }

                        }

                        entity.JobPerson = person;
                        entity.JobPersonId = personid;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(jobperson))
                        {
                            var ckjobperson = users.FirstOrDefault(row => row.RealName == jobperson);
                            if (ckjobperson == null)
                            {
                                return new { info = "作业人错误", code = 1, count = 0, data = new { } };
                            }
                            entity.JobPerson = jobperson;
                            entity.JobPersonId = ckjobperson == null ? null : ckjobperson.UserId;
                        }

                    }
                    //string otherperson = sheet.Cells[i, 3].StringValue;
                    //if (!string.IsNullOrEmpty(otherperson))
                    //{
                    //    var ckotherperson = users.FirstOrDefault(row => row.RealName == otherperson);
                    //    if (ckotherperson == null)
                    //    {
                    //        success = false;
                    //        message = "选填作业人错误";
                    //        return Json(new { success, message });
                    //    }
                    //}
                    //if (otherperson == jobperson)
                    //{
                    //    success = false;
                    //    message = "作业人不能相同";
                    //    return Json(new { success, message });
                    //}
                    //entity.otherperson = otherperson;
                    //entity.Dangerous = sheet.Cells[i, 3].StringValue;
                    //entity.Measure = sheet.Cells[i, 4].StringValue;
                    entity.Device = sheet.Cells[i, 4].StringValue;
                    var getNow = DateTime.Now.ToString("yyyy-MM-dd");
                    entity.JobStartTime = sheet.Cells[i, 5].StringValue == "" ? Convert.ToDateTime(getNow + " 08:30") : Convert.ToDateTime(getNow + " " + sheet.Cells[i, 5].StringValue);
                    entity.JobEndTime = sheet.Cells[i, 6].StringValue == "" ? Convert.ToDateTime(getNow + " 08:30") : Convert.ToDateTime(getNow + " " + sheet.Cells[i, 6].StringValue);
                    if (!string.IsNullOrEmpty(sheet.Cells[i, 7].StringValue))
                    {
                        //每年，二月、九月，15日，白班  每月，第一个、第三个，星期五，白班
                        var Cycle = sheet.Cells[i, 7].StringValue.Trim();

                        var cycleType = Cycle.Split('，');
                        if (cycleType[0] != "每天" && cycleType[0] != "每周" && cycleType[0] != "每月" && cycleType[0] != "每年")
                        {
                            return new { info = "周期规则错误", code = 1, count = 0, data = new { } };
                        }

                        entity.Cycle = cycleType[0];
                        var ck = false;
                        var data = string.Empty;
                        for (int j = 1; j < cycleType.Length; j++)
                        {

                            //if (cycleType[j] == "白班" || cycleType[j] == "夜班")
                            //{
                            //    entity.worksetname = cycleType[j];

                            //}
                            //else
                            if (cycleType[j] == "截止")
                            {
                                entity.isend = true;

                            }
                            else
                            if (cycleType[j] == "最后一天")
                            {
                                if ((Cycle.Contains("每月") || Cycle.Contains("每年")) && Cycle.Contains("日"))
                                {
                                    entity.islastday = true;
                                }
                                else
                                {
                                    return new { info = "周期规则错误", code = 1, count = 0, data = new { } };
                                }

                            }
                            else
                            if (cycleType[j].Contains("双休"))
                            {
                                if ((Cycle.Contains("每月") || Cycle.Contains("每年")) && Cycle.Contains("日"))
                                {
                                    entity.isweek = true;
                                }
                                else if (Cycle.Contains("每天"))
                                {
                                    entity.isweek = true;
                                }
                                else
                                {
                                    return new { info = "周期规则错误", code = 1, count = 0, data = new { } };
                                }
                            }
                            else
                            {
                                data += cycleType[j].Replace('日', ' ').Trim().Replace('、', ',') + ";";
                                ck = true;
                            }


                        }

                        if (ck)
                        {
                            data = data.Substring(0, data.Length - 1);
                            entity.CycleDate = data;
                        }
                        else
                        {
                            entity.CycleDate = data;
                        }

                    }
                    else
                    {
                        if (entity.jobplantype != "临时任务")
                        {
                            return new { info = "周期不能为空", code = 1, count = 0, data = new { } };
                        }
                    }
                    entity.Dangerous = sheet.Cells[i, 8].StringValue;
                    entity.Measure = sheet.Cells[i, 9].StringValue;
                    var EnableTraining = sheet.Cells[i, 10].StringValue;
                    entity.EnableTraining = EnableTraining == "是";
                    //entity.EnableTraining = false;
                    entity.worksetname = sheet.Cells[i, 11].StringValue;
                    if (entity.jobplantype == "设备巡回检查")
                        entity.TaskType = "巡回检查";
                    else if (entity.jobplantype == "定期工作")
                        entity.TaskType = "定期工作";
                    else
                        entity.TaskType = "日常工作";

                    var setupid = string.Empty;
                    var createuserid = string.Empty;
                    WorkOrderBLL orderbll = new WorkOrderBLL();
                    orderbll.GetWorkSettingByDept(user.DepartmentId, out setupid, out createuserid);
                    WorkSettingBLL settingbll = new WorkSettingBLL();
                    var setting = settingbll.GetList("");
                    var getbanci = setting.Where(x => x.WorkSetupId == setupid && x.CreateUserId == createuserid);
                    if (entity.worksetname.Contains(","))
                    {
                        var setname = string.Empty;
                        var setnameid = string.Empty;
                        var setList = entity.worksetname.Split(',');
                        for (int j = 0; j < setList.Length; j++)
                        {
                            var ckset = getbanci.FirstOrDefault(x => x.Name == entity.worksetname);
                            if (ckset == null)
                            {
                                return new { info = "不存在该班次" + setList[j], code = 1, count = 0, data = new { } };
                            }
                            if (j >= setList.Length - 1)
                            {
                                setname += setList[j];
                                setnameid += ckset.WorkSettingId;
                            }
                            else
                            {
                                setname += setList[j] + ",";
                                setnameid += ckset.WorkSettingId + ",";

                            }

                        }

                        entity.worksetname = setname;
                        entity.worksetid = setnameid;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(entity.worksetname))
                        {
                            if (entity.jobplantype != "临时任务")
                            {
                                return new { info = "班次不能为空", code = 1, count = 0, data = new { } };
                            }
                        }
                        else
                        {
                            var ckset = getbanci.FirstOrDefault(x => x.Name == entity.worksetname);
                            if (ckset == null)
                            {
                                return new { info = "不存在该班次" + entity.worksetname, code = 1, count = 0, data = new { } };
                            }
                            entity.worksetname = ckset.Name;
                            entity.worksetid = ckset.WorkSettingId;
                        }
                    }
                    entity.DeptId = user.DepartmentId;
                    entity.CreateDate = date.AddMinutes(i);
                    entity.DangerType = "job";
                    templates.Add(entity);
                }
                foreach (var item in templates)
                {
                    if (item.DangerousList == null) item.DangerousList = new List<JobDangerousEntity>();
                    if (!string.IsNullOrEmpty(item.Dangerous))
                    {
                        var dangerArray = item.Dangerous.Split('。');
                        var dangerMeasureArray = item.Measure?.Split('。');
                        for (int i = 0; i < dangerArray.Length; i++)
                        {
                            var danger = dangerArray[i];
                            if (string.IsNullOrEmpty(danger)) continue;
                            var templateDangerousEntity = new JobDangerousEntity { Content = danger };
                            if (dangerMeasureArray != null && dangerMeasureArray.Length > i)
                            {
                                templateDangerousEntity.MeasureList = new List<JobMeasureEntity>();
                                var dangerMeasure = dangerMeasureArray[i];
                                if (!string.IsNullOrEmpty(dangerMeasure))
                                {
                                    var measureArray = dangerMeasure.Split('；');
                                  
                                    foreach (var measure in measureArray)
                                    {
                                        if (string.IsNullOrEmpty(measure)) continue;
                                        templateDangerousEntity.MeasureList.Add(new JobMeasureEntity { Content = measure });
                                    }
                                }
                            }
                            item.DangerousList.Add(templateDangerousEntity);
                        }
                    }
                    foreach (var item1 in item.DangerousList)
                    {
                        if (string.IsNullOrEmpty(item1.JobDangerousId)) item1.JobDangerousId = Guid.NewGuid().ToString();
                        item1.CreateTime = DateTime.Now;
                        item1.JobId = item.JobId;
                        if (item1.MeasureList == null) item1.MeasureList = new List<JobMeasureEntity>();
                        foreach (var item2 in item1.MeasureList)
                        {
                            if (string.IsNullOrEmpty(item2.JobMeasureId)) item2.JobMeasureId = Guid.NewGuid().ToString();
                            item2.CreateTime = DateTime.Now;
                            item2.JobDangerousId = item1.JobDangerousId;
                        }
                    }

                    item.CreateUserId = user.UserId;
                    item.CreateUser = user.RealName;
                    workmeetingbll.UpdateJobTemplate(item);
                }
                return new { info = "导入成功", code = 0, count = 0, data = new { } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }
        [HttpPost]
        public object GetUrl([FromBody]JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                var path = BSFramework.Util.Config.GetValue("AppUrl");
                var url = path + "Content/export/任务导入模板.xlsx";

                return new { code = 0, info = "获取数据成功", data = url };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 根据编码的名称获取编码列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetItemList([FromBody]JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string itemName = dy.data.itemName;
                var data = new DataItemDetailBLL().GetDataItems(itemName);

                return new { code = 0, info = "获取数据成功", data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        [HttpPost]
        public object GetDepetUser([FromBody]JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string userId = dy.userId;
                var userEntity = new UserBLL().GetEntity(userId);
                var data = new UserBLL().GetDeptUsers(userEntity.DepartmentId);
                return new { code = 0, info = "获取数据成功", data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }

        /// <summary>
        /// 根据区域code获取责任人管理
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetDistrictPerson([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string code = dy.data;
                DistrictPersonBLL DistrictBll = new DistrictPersonBLL();
                int total = 0;
                var data = DistrictBll.GetList(code, "", "", "", 1000, 1, out total);
                return new { code = 0, info = "获取数据成功", data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 根据区域code获取责任人管理
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetDistrictPersonById([FromBody] JObject json)
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string DistrictId = dy.data.DistrictId;
                string DeptId = dy.data.DeptId;
                DistrictPersonBLL DistrictBll = new DistrictPersonBLL();
                var data = DistrictBll.GetList(DistrictId);
                if (string.IsNullOrEmpty(DeptId))
                {
                    data = data.Where(x => x.DutyDepartmentId == DeptId).ToList();
                }
                return new { code = 0, info = "获取数据成功", data = data };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
    }
}
