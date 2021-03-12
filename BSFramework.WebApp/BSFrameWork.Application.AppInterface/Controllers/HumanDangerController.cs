using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class HumanDangerController : BaseApiController
    {
        /// <summary>
        /// 待办
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ListBucket<HumanDangerTrainingEntity> Todo(ParamBucket args)
        {
            var user = OperatorProvider.Provider.Current();
            var people = new PeopleBLL().GetEntity(user.UserId);
            var list_userid = new List<string>();
            if (people != null && people.FingerMark == "yes")
            {
                var q = "," + people.Quarters + ",";
                if (q.Contains("," + "班长" + ",") || q.Contains("," + "副班长" + ",") || q.Contains("," + "技术员" + ",") || (user.RoleName.Contains("负责人")) || user.RoleName.Contains("班组长"))
                {
                    var users = new PeopleBLL().GetListByDept(user.DeptId);
                    list_userid.AddRange(users.Select(x => x.ID));
                }
                else
                    list_userid.Add(user.UserId);

            }
            else
            {
                list_userid.Add(user.UserId);
            }
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var total = 0;
            var data = bll.GetUserTodo(user.UserId, list_userid.ToArray(), args.PageSize, args.PageIndex, out total);
            foreach (var item in data)
            {
                item.Seq = item.UserId == user.UserId ? 0 : 1;
            }
            data = data.OrderBy(x => x.Seq).ThenBy(x => x.IsDone).ThenByDescending(x => x.CreateTime).ToList();
            return new ListBucket<HumanDangerTrainingEntity>() { Success = true, Data = data, Total = total };
        }

        /// <summary>
        /// 历史记录
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<HumanDangerTrainingEntity> GetList(ParamBucket<HumanDangerModel> args)
        {
            if (args.Data != null && args.Data.To != null) args.Data.To = args.Data.To.Value.AddDays(1).AddSeconds(-1);
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var total = 0;
            var data = bll.GetList(args.UserId, args.Data.Users, args.Data.DutyUser, args.Data.From, args.Data.To, args.Data.Status, args.Data.Level, args.Data.EvaluateStatus, args.PageSize, args.PageIndex, out total);
            return new ListBucket<HumanDangerTrainingEntity>() { code = 0, Data = data, Total = total };
        }

        /// <summary>
        /// 添加人身风险预控
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket Add(ParamBucket<HumanDangerTrainingEntity> args)
        {
            var code = 0;
            var msg = string.Empty;
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            args.Data.TrainingId = Guid.NewGuid().ToString();
            args.Data.TrainingUsers.ForEach(x => x.TrainingUserId = Guid.NewGuid().ToString());
            try
            {
                args.Data.TrainingId = Guid.NewGuid().ToString();
                args.Data.CreateTime = DateTime.Now;
                args.Data.CreateUserId = args.UserId;
                args.Data.TrainingUsers.ForEach(x => x.TrainingUserId = Guid.NewGuid().ToString());
                bll.Add(args.Data);

                var messagebll = new MessageBLL();
                foreach (var item in args.Data.TrainingUsers)
                {
                    messagebll.SendMessage("人身风险预控", item.TrainingUserId.ToString());
                }
           
            }
            catch (Exception e)
            {
                code = 1;
                msg = e.Message;
               
            }
            return new ListBucket<string>() { code = code, info = msg,Data =new List<string>() { args.Data.TrainingId } };
        }

        /// <summary>
        /// 删除人身风险预控
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket Delete(ParamBucket<string> args)
        {
            var code = 0;
            var msg = string.Empty;
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            try
            {
                bll.Delete(args.Data);
            }
            catch (Exception e)
            {
                code = 1;
                msg = e.Message;
            }
            return new ListBucket<HumanDangerTrainingEntity>() { code = code, info = msg };
        }

        /// <summary>
        /// 获取风险性质类型
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<DataItemDetailEntity> GetDangerTypes(ParamBucket args)
        {
            DataItemDetailBLL dataitembll = new DataItemDetailBLL();
            var data = dataitembll.GetDataItems("作业性质类型");
            return new ListBucket<DataItemDetailEntity>() { code = 0, Data = data, Total = data.Count };
        }

        /// <summary>
        /// 获取风险类别
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<DangerCategoryEntity> GetDangerCategories(ParamBucket args)
        {
            DangerMeasureBLL bll = new DangerMeasureBLL();
            var data = bll.GetCategories(null).OrderBy(x => x.Sort).ToList();
            return new ListBucket<DangerCategoryEntity>() { code = 0, Data = data, Total = data.Count };
        }

        /// <summary>
        /// 获取风险因素
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<DangerMeasureEntity> GetDangerReasons(ParamBucket<string> args)
        {
            DangerMeasureBLL bll = new DangerMeasureBLL();
            var data = bll.GetDangerReasons(args.Data);
            return new ListBucket<DangerMeasureEntity>() { code = 0, Data = data, Total = data.Count };
        }

        /// <summary>
        /// 获取预控措施
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<DangerMeasureEntity> GetMeasures(ParamBucket<HumanDangerMeasureEntity> args)
        {
            DangerMeasureBLL bll = new DangerMeasureBLL();
            var data = bll.GetMeasures(args.Data.HumanDangerId, args.Data.MeasureId);
            return new ListBucket<DangerMeasureEntity>() { code = 0, Data = data, Total = data.Count };
        }

        /// <summary>
        /// 获取人身风险预控
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<HumanDangerTrainingEntity> GetDetail(ParamBucket<string> args)
        {
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var data = bll.GetDetail(args.Data);
            if (data.HumanDangerId != null)
            {
                var n1 = data.Measures.Count(x => x.State == 1);
                var n2 = (float)data.Measures.Count(x => x.State == 2);
                data.EvaluateContent = string.Format("正确风险{0}个，遗漏风险{1}个", n1, n2);
                var n3 = 0f;
                if (n1 == 0 && n2 == 0) n1 = 1;
                n3 = 1 - n2 / (n1 + n2);
                if (n3 <= 0.33)
                    data.AutoEvaluate = "分析辨识能力亟需加强！";
                else if (0.33 < n3 && n3 <= 0.66)
                    data.AutoEvaluate = "分析能力一般，再接再厉！";
                else
                    data.AutoEvaluate = "分析能力很棒！";
            }

            return new ModelBucket<HumanDangerTrainingEntity>() { code = 0, Data = data };
        }

        /// <summary>
        /// 保存风险因素
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket SaveReasons(ParamBucket<HumanDangerTrainingEntity> args)
        {
            var code = 0;
            var msg = string.Empty;
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            args.Data.Measures.RemoveAll(x => x.MeasureId == null);
            try
            {
                args.Data.Measures.ForEach(x => { if (x.TrainingMeasureId == Guid.Empty) x.TrainingMeasureId = Guid.NewGuid(); });
                args.Data.TaskTypes.ForEach(x => { if (x.TaskTypeId == Guid.Empty) x.TaskTypeId = Guid.NewGuid(); });
                bll.SaveReasons(args.Data);

                if (args.Data.IsSubmit) bll.Finish(args.Data.TrainingId, args.Data.TrainingTime.Value);
            }
            catch (Exception e)
            {
                code = 1;
                msg = e.Message;
            }
            return new ResultBucket() { code = code, info = msg };
        }

        /// <summary>
        /// 保存风险因素
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket SaveMeasures(ParamBucket<HumanDangerTrainingEntity> args)
        {
            var code = 0;
            var msg = string.Empty;
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            try
            {
                bll.SaveMeasures(args.Data);
            }
            catch (Exception e)
            {
                code = 1;
                msg = e.Message;
            }
            return new ResultBucket() { code = code, info = msg };
        }

        /// <summary>
        /// 保存风险因素
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket Finish(ParamBucket<HumanDangerTrainingEntity> args)
        {
            var code = 0;
            var msg = string.Empty;
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            try
            {
                bll.Finish(args.Data.TrainingId, args.Data.TrainingTime.Value);
            }
            catch (Exception e)
            {
                code = 1;
                msg = e.Message;
            }
            return new ResultBucket() { code = code, info = msg };
        }

        /// <summary>
        /// 获取作业区域
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<string> GetTaskAreas(ParamBucket<string> args)
        {
            DangerMeasureBLL bll = new DangerMeasureBLL();
            var total = 0;
            var data = bll.GetTaskAreas(args.Data, args.PageSize, args.PageIndex, out total);
            return new ListBucket<string>() { code = 0, Data = data, Total = total };
        }

        /// <summary>
        /// 获取班组人身风险预控
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<HumanDangerTrainingEntity> GetListByDeptId(ParamBucket<string> args)
        {
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var to = from.AddDays(1).AddMinutes(-1);
            var data = bll.GetListByDeptId(args.Data, from, to);

            return new ListBucket<HumanDangerTrainingEntity>() { code = 0, Data = data, Total = data.Count };
        }

        /// <summary>
        /// 台账
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<HumanModel> GetData(ParamBucket<DateLimit> args)
        {
            var user = new UserBLL().GetEntity(args.UserId);
            var date = args.Data.From.Value;
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var depts = new List<string>();
            var total = 0;
            if (!string.IsNullOrEmpty(args.Data.DeptId))
            {
                depts.AddRange(new DepartmentBLL().GetSubDepartments(args.Data.DeptId, null).Select(x => x.DepartmentId));
            }

            var users = new UserBLL().GetList(depts.ToArray(), args.PageSize, args.PageIndex, out total);
            var data = bll.GetData3(users.Select(x => x.UserId).ToArray(), date, date.AddMonths(1));
            var list = users.GroupJoin(data, x => x.UserId, y => y.UserId, (x, y) => new HumanModel() { Date = date, UserId = x.UserId, UsreName = x.RealName, Total1 = y.Count(n => n.IsDone), Total2 = y.Count(n => n.IsEvaluate), Total3 = y.Count(n => n.Seq == 1) }).ToList();
            //while (date < args.Data.To)
            //{
            //    var data = bll.GetData(depts.ToArray(), date, date.AddMonths(1));
            //    var total = data.GroupBy(x => new { x.UserId, x.UserName }).Select(x => new HumanModel { Date = date, UserId = x.Key.UserId, UsreName = x.Key.UserName, Total1 = x.Count(y => y.IsDone), Total2 = x.Count(y => y.IsEvaluate), Total3 = x.Count(y => y.Seq == 1) }).ToList();
            //    list.AddRange(total);
            //    date = date.AddMonths(1);
            //}
            return new ListBucket<HumanModel>() { code = 0, Data = list, Total = total };
        }

        /// <summary>
        /// 台账
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<HumanDangerTrainingEntity> GetDataList(ParamBucket<HumanDangerModel> args)
        {
            var total = 0;
            var user = new UserBLL().GetEntity(args.UserId);
            if (args.Data.To != null)
                args.Data.To.Value.AddDays(1).AddMinutes(-1);
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var data = bll.GetDataList(args.UserId, args.Data.Users, args.Data.From, args.Data.To, args.Data.Status, args.Data.Level, args.Data.EvaluateStatus, args.PageSize, args.PageIndex, out total);
            return new ListBucket<HumanDangerTrainingEntity>() { code = 0, Data = data, Total = data.Count };
        }

        /// <summary>
        /// 补充预控措施
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket EditMeasure(ParamBucket<HumanDangerTrainingEntity> args)
        {
            var user = OperatorProvider.Provider.Current();
            var code = 0;
            var msg = string.Empty;
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var messagebll = new MessageBLL();
            var setting = new DataItemDetailBLL().GetDetail("人身风险预控", "预控效果及补充措施填写设置时间间隔");
            var mins = 0;
            int.TryParse(setting.ItemValue, out mins);

            if (mins > 0)
            {
                var detail = bll.GetDetail(args.Data.TrainingId);
                var val = DateTime.Now - detail.TrainingTime.Value;
                if (val.Minutes < mins) return new ResultBucket { Success = false, Message = string.Format("分析后{0}分钟内不允许补充预控措施", mins) };
            }

            bll.EditMeasure(args.Data, user.UserId, user.UserName);
            messagebll.FinishTodo("人身风险预控", args.Data.TrainingId.ToString());

            return new ResultBucket() { code = code, info = msg };
        }

        /// <summary>
        /// 评价
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket Evaluate(ParamBucket<ActivityEvaluateEntity> args)
        {
            var user = new UserBLL().GetEntity(args.UserId);
            var dept = new DepartmentBLL().GetEntity(user.DepartmentId);

            args.Data.ActivityEvaluateId = Guid.NewGuid().ToString();
            args.Data.CREATEDATE = DateTime.Now;
            args.Data.CREATEUSERID = user.UserId;
            args.Data.CREATEUSERNAME = user.RealName;
            args.Data.DeptName = dept == null ? null : dept.FullName;
            args.Data.EvaluateDate = DateTime.Now;
            args.Data.EvaluateId = user.UserId;
            args.Data.EvaluateUser = user.RealName;
            args.Data.Nature = dept == null ? null : dept.Nature;

            var code = 0;
            var msg = string.Empty;
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var messagebll = new MessageBLL();
            try
            {
                bll.Evaluate(args.Data);

                messagebll.SendMessage("人身风险预控评价", args.Data.ActivityEvaluateId);

            }
            catch (Exception e)
            {
                code = 1;
                msg = e.Message;
            }
            return new ResultBucket() { code = code, info = msg };
        }

        /// <summary>
        /// 首页任务跳转
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<HumanDangerTrainingEntity> GetListByUserIdJobId(ParamBucket<HumanDangerTrainingEntity> args)
        {
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var total = 0;
            var data = bll.GetListByUserIdJobId(args.Data.TrainingId.ToString(), args.UserId);
            return new ListBucket<HumanDangerTrainingEntity>() { code = 0, Data = data, Total = total };
        }

        /// <summary>
        /// 获取查看规则
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ModelBucket<string> GetRoleContent()
        {
            var bll = new DataItemDetailBLL();
            DataItemDetailEntity entity = bll.GetEntity("szfxykgz");
            return new ModelBucket<string>() { Success = true, Data = entity.ItemValue };
        }

        /// <summary>
        /// 选择人
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<ItemEntity> GetUsers(ParamBucket args)
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetEntity(user.DeptId);
            if (dept.IsSpecial) dept = new DepartmentBLL().GetRootDepartment();

            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var data = bll.GetUsers(dept.DepartmentId);
            return new ListBucket<ItemEntity>() { Success = true, Data = data, Total = data.Count };
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetHumanDangerTrainingTotal(ParamBucket<IndexCountries> args)
        {
            try
            {

                var nowTime = DateTime.Now;
                var startTime = new DateTime(nowTime.Year, nowTime.Month, 1);
                var endTime = new DateTime(nowTime.Year, nowTime.Month, 1).AddMonths(1).AddMinutes(-1);
                if (args.Data.startTime.HasValue)
                {
                    startTime = args.Data.startTime.Value;
                    endTime = args.Data.endTime.Value;
                }
                int Finishtotal = 0;
                int unFinishtotal = 0;
                HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
                bll.GetTrainings(args.UserId, new string[1] { args.Data.DeptId }, "", startTime, endTime, "", 50000, 1, null, null, null, out Finishtotal);
                bll.GetUndo(new string[1] { args.Data.DeptId }, "", startTime, endTime, 4, 50000, 1, out unFinishtotal);
                decimal score = 0;
                if (Finishtotal > 0 || unFinishtotal > 0)
                {
                    score = Finishtotal / (Finishtotal + unFinishtotal);
                }


                return new { code = 0, info = "获取数据成功", data = new { finish = Finishtotal, unfinish = unFinishtotal, score = score } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        [HttpPost]
        public object GetDangerRole()
        {
            try
            {
                var bll = new DataItemDetailBLL();
                DataItemDetailEntity entity = bll.GetEntity("szfxykgz");
                return new { Code = 0, Data = entity == null ? null : entity.ItemValue, Info = "获取数据成功" };
            }
            catch (Exception ex)
            {
                return new { Code = -1, Info = "获取数据失败", ex.Message };
            }

        }

        [HttpPost]
        public ListBucket<HumanDangerTrainingEntity> GetToEvaluate(ParamBucket args)
        {
            var user = OperatorProvider.Provider.Current();
            HumanDangerTrainingBLL bll = new HumanDangerTrainingBLL();
            var total = 0;
            var data = bll.GetToEvaluate(user.DeptId, null, null, null, args.PageSize, args.PageIndex, out total);
            foreach (var item in data)
            {
                item.RangeTime = string.Format("{0}~{1}", item.StartDate.ToString("yyyy/MM/dd"), item.EndDate.ToString("yyyy/MM/dd"));
            }
            return new ListBucket<HumanDangerTrainingEntity>() { Success = true, Data = data, Total = total };
        }
    }
}
