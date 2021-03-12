using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Application.IService.SevenSManage;
using BSFramework.Application.Service.SevenSManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SevenSManage
{
    public class SevenSBLL
    {
        private ISevenSService service = new SevenSService();
        private FileInfoBLL fileBll = new FileInfoBLL();
        // private string url = BSFramework.Util.Config.GetValue("AppUrl");


        #region 技术规范
        public IList<SevenSEntity> GetItems(string key, string typeid, int pagesize, int page, string deptCode, out int total)
        {
            return service.GetItems(key, typeid, pagesize, page, deptCode, out total);
        }
        public IEnumerable<SevenSEntity> GetList(string typename, string typeId, string name, string Id, int pageIndex, int pageSize, bool ispage, string deptid)
        {

            return service.GetList(typename, typeId, name, Id, pageIndex, pageSize, ispage, deptid);

        }
        public IList<SevenSTypeEntity> GetAllType(string deptCode)
        {
            return service.GetAllType(deptCode);
        }
        public void DeleteType(string id)
        {
            service.DeleteType(id);
        }
        public void DeleteItem(string id)
        {
            service.DeleteItem(id);
        }
        public SevenSEntity GetSevenSEntity(string keyValue)
        {
            return service.GetSevenSEntity(keyValue);
        }
        public void EditType(SevenSTypeEntity model)
        {
            service.EditType(model);
        }
        public void SaveSevenSEntity(SevenSEntity entity)
        {
            service.SaveSevenSEntity(entity);

        }
        public void AddType(SevenSTypeEntity model)
        {
            model.TypeId = Guid.NewGuid().ToString();
            service.AddType(model);
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, SevenSEntity entity)
        {
            try
            {
                service.SaveForm(keyValue, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<SevenSTypeEntity> GetIndex(string name, string typeid, string deptCode)
        {
            return service.GetIndex(name, typeid, deptCode);

        }
        #endregion
        #region 定点照片
        /// <summary>
        /// 获取周期设置数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SevenSPictureCycleEntity> getCycle()
        {
            return service.getCycle();
        }

        /// <summary>
        /// 修正周期时间数据
        /// </summary>
        /// <returns></returns>
        public void setCycle(string value)
        {
            service.setCycle(value);
        }
        /// <summary>
        /// 修正周期时间数据
        /// </summary>
        /// <returns></returns>
        public void setCycleTime(string value)
        {
            service.setCycleTime(value);
        }
        /// <summary>
        /// 获取未提交数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SevenSPictureEntity> getState()
        {
            return service.getState();
        }
        /// <summary>
        /// 获取记录数据
        /// </summary>
        public List<SevenSPictureEntity> getSevenSFinish(string deptid)
        {
            return service.getSevenSFinish(deptid);

        }
        /// <summary>
        /// 未推送推送
        /// </summary>
        /// <returns></returns>
        public void sendMessage()
        {
            //获取周期数据
            var cycle = getCycle().FirstOrDefault(x => x.iswork);
            DateTime startTime;
            var nowTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            if (string.IsNullOrEmpty(cycle.starttime))
            {
                startTime = nowTime;
            }
            else
            {
                startTime = Convert.ToDateTime(cycle.starttime).AddDays(-1);
            }
            //等于0或1
            if (Util.Time.DiffDays(startTime, nowTime) > 1)
            {
                return;
            }
            var userBll = new UserBLL();
            var list = getState();
            foreach (var item in list)
            {
                var user = userBll.GetDeptUsers(item.deptid).ToList();
                var content = item.deptname + "班本期的7S照片还未上传提交，请及时通过【班组帮】拍照提交。";
                string[] userList = new string[user.Count()];
                for (int i = 0; i < user.Count(); i++)
                {
                    userList[i] = user[i].Account;
                }
                //MessageClient.SendRequest(userList, item.Id, "7S定点拍照|" + item.Id, "7S定点拍照通知", content);
            }

        }

        public IList<SevenSTypeEntity> GetAllType(string[] depts)
        {
            return service.GetAllType(depts);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public void Start(List<DepartmentEntity> entity)
        {
            try
            {

                //获取周期数据
                var cycle = getCycle().FirstOrDefault(x => x.iswork);
                DateTime startTime;
                var nowTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                int num = 0;
                int totalday = 0;
                int month = 0;
                int day = 0;
                if (string.IsNullOrEmpty(cycle.starttime))
                {
                    startTime = nowTime;
                    num = Util.Time.GetWeekNumberOfDay(nowTime);//星期几
                    totalday = Util.Time.GetDaysOfMonth(nowTime.Year, nowTime.Month);//本月有多少天
                    month = nowTime.Month;//第几月
                    day = nowTime.Day;//第几天
                }
                else
                {
                    startTime = Convert.ToDateTime(cycle.starttime);
                    num = Util.Time.GetWeekNumberOfDay(startTime);//星期几
                    totalday = Util.Time.GetDaysOfMonth(startTime.Year, startTime.Month);//本月有多少天
                    month = startTime.Month;//第几月
                    day = startTime.Day;//第几天
                }
                //等于0或1
                if (Util.Time.DiffDays(nowTime, startTime) > 1)
                {
                    return;
                }
                var messagebll = new MessageBLL();
                foreach (var item in entity)
                {
                    var workFinish = getSevenSFinish(item.DepartmentId);
                    foreach (var finist in workFinish)
                    {
                        messagebll.FinishTodo("7S定点照片到期提醒", finist.Id);

                    }


                }
                //周期间隔时间
                var EndTime = startTime;
                switch (cycle.cycle)
                {
                    case "每周":
                        EndTime = EndTime.AddDays(7 - num);
                        startTime = startTime.AddDays(-num + 1);
                        break;
                    case "每半月":
                        if (day > 15)
                        {
                            startTime = new DateTime(nowTime.Year, month, 16);
                            EndTime = new DateTime(nowTime.Year, month, totalday);
                        }
                        else
                        {
                            startTime = new DateTime(nowTime.Year, month, 1);
                            EndTime = new DateTime(nowTime.Year, month, 15);
                        }
                        break;
                    case "每月":
                        startTime = new DateTime(nowTime.Year, month, 1);
                        EndTime = new DateTime(nowTime.Year, month, totalday);
                        break;
                    case "每季度":
                        if (month < 4)
                        {
                            startTime = new DateTime(nowTime.Year, 1, 1);
                            EndTime = new DateTime(nowTime.Year, 3, 31);
                        }
                        else if (month < 7)
                        {
                            startTime = new DateTime(nowTime.Year, 4, 1);
                            EndTime = new DateTime(nowTime.Year, 6, 30);
                        }
                        else
                        if (month < 9)
                        {
                            startTime = new DateTime(nowTime.Year, 7, 1);
                            EndTime = new DateTime(nowTime.Year, 9, 30);
                        }
                        else
                        {
                            startTime = new DateTime(nowTime.Year, 10, 1);
                            EndTime = new DateTime(nowTime.Year, 12, 31);
                        }
                        break;
                    default:
                        break;
                }

                var Picture = new List<SevenSPictureEntity>();
                foreach (var item in entity)
                {
                    var one = new SevenSPictureEntity();
                    one.Id = Guid.NewGuid().ToString();
                    one.deptname = item.FullName;
                    one.deptid = item.DepartmentId;
                    one.state = "未提交";
                    one.CreateDate = DateTime.Now;
                    one.ModifyDate = DateTime.Now;
                    one.planeStartDate = startTime;
                    one.planeEndDate = EndTime;
                    Picture.Add(one);
                }
                InsertList(Picture);
                var plantime = new SevenSPlanTimeEntity();
                plantime.Id = Guid.NewGuid().ToString();
                plantime.PlanTime = startTime.ToString("yyyy-MM-dd") + "~" + EndTime.ToString("yyyy-MM-dd");
                SavePlanTime(plantime);
                //修改下周期
                setCycleTime(EndTime.AddDays(1).ToString("yyyy-MM-dd"));

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        /// <summary>
        /// 获取地点设置数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SevenSPictureSetEntity> getSet()
        {
            return service.getSet();
        }

        /// <summary>
        /// 获取地点设置数据包含当前记录id
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SevenSPictureSetEntity> getSetAndPicture(string deptId)
        {
            return service.getSetAndPicture(deptId);
        }
        /// <summary>
        /// 添加地点设置
        /// </summary>
        /// <param name="entity"></param>
        public void InsertPhoneSet(SevenSPictureSetEntity entity)
        {
            service.InsertPhoneSet(entity);
        }
        /// <summary>
        /// 获取记录数据
        /// </summary>
        /// <param name="entity"></param>
        public SevenSPictureEntity getEntity(string keyvalue)
        {
            return service.getEntity(keyvalue);
        }
        /// <summary>
        /// 删除地点设置
        /// </summary>
        /// <param name="entityId"></param>
        public void DelPhoneSet(string entityId)
        {
            service.DelPhoneSet(entityId);
        }

        /// <summary>
        ///获取记录数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SevenSPictureEntity> getList(DateTime? planeStart, DateTime? planeEnd, string state, string evaluationState, string space, Pagination pagination, bool ispage, string deptId, bool isFile)
        {
            var pic = service.getList(planeStart, planeEnd, state, evaluationState, space, pagination, ispage, deptId, isFile);
            if (isFile)
            {
                foreach (var item in pic)
                {
                    var file = fileBll.GetFilesByRecIdNew(item.Id);
                    if (!string.IsNullOrEmpty(space) && space != "全部")
                    {
                        item.Files = file.Where(t => t.FileExtensions == space).ToList();
                    }
                    else
                    {
                        item.Files = file.ToList();
                    }
                }
            }
            return pic;
            //if (isFile)
            //{
            //    foreach (var item in query)
            //    {
            //        var ex = LinqExtensions.True<FileInfoEntity>();
            //        ex = ex.And(t => t.RecId == item.Id);
            //        if (!string.IsNullOrEmpty(space) && space != "全部")
            //        {
            //            ex = ex.And(t => t.FileExtensions == space);
            //            item.Files = db.IQueryable(ex).ToList();
            //        }
            //        else
            //        {
            //            item.Files = db.IQueryable(ex).ToList();
            //        }

            //    }
            //}
        }

        /// <summary>
        /// 插入记录数据
        /// </summary>
        /// <param name="entityList"></param>
        public void InsertList(List<SevenSPictureEntity> entityList)
        {
            service.InsertList(entityList);
        }
        /// <summary>
        /// 修改数据提交状态
        /// </summary>
        public void update(string id, string userid)
        {
            service.update(id, userid);
        }
        /// <summary>
        /// 页面操作数据
        /// </summary>
        public void SaveFrom(List<SevenSPictureSetEntity> entityList, string[] del, string setTime, string regulation)
        {
            service.SaveFrom(entityList, del, setTime, regulation);
        }
        /// <summary>
        /// 保存评价
        /// </summary>
        /// <returns></returns>  
        public void SaveEvaluation(string keyValue, string evaluation, string user)
        {
            var messagebll = new MessageBLL();

            try
            {
                service.SaveEvaluation(keyValue, evaluation, user);

                messagebll.SendMessage("7S照片评价", keyValue);

            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 保存时间段  用于app使用
        /// </summary>
        /// <param name="entity"></param>
        public void SavePlanTime(SevenSPlanTimeEntity entity)
        {
            service.SavePlanTime(entity);

        }

        /// <summary>
        /// 查询时间段
        /// </summary>
        public List<SevenSPlanTimeEntity> getPlanTime()
        {

            return service.getPlanTime();
        }

        public List<SevenSPictureEntity> GetListByManager(string state, string userId, bool? evaluateState, string deptid, out int totalCount, int pageIndex = 1, int pageSize = 5)
        {
            return service.GetListByManager(state, userId, evaluateState, deptid, out totalCount, pageIndex, pageSize);
        }


        public List<SevenSPictureEntity> GetListByManager(DateTime? datefrom, DateTime? dateto, string state, string userId, bool? evaluateState, string deptid, out int totalCount, int pageIndex = 1, int pageSize = 5)
        {
            return service.GetListByManager(datefrom, dateto, state, userId, evaluateState, deptid, out totalCount, pageIndex, pageSize);
        }



        #endregion
        #region 精益管理

        /// <summary>
        /// 获取用户精益管理数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<SevenSOfficeEntity> getOfficebyuser(string userid)
        {
            var data = service.getOfficebyuser(userid).OrderByDescending(x => x.createdate).ToList();
            foreach (var item in data)
            {
                item.audit = getAuditByid(item.id).OrderBy(x => x.sort).ToList();
                var fileList = fileBll.GetFilesByRecIdNew(item.id);
                //foreach (var items in fileList)
                //{
                //    items.FilePath = items.FilePath.Replace("~/", url);
                //}
                item.statusquoFiles = fileList.Where(x => x.Description == "xz").ToList();
                item.proposedFiles = fileList.Where(x => x.Description == "ty").ToList();
            }
            return data;
        }
        /// <summary>
        /// 审核人员获取精益管理数据
        /// </summary>
        /// <returns></returns>
        public List<SevenSOfficeEntity> getOfficeByidExtensions(string userid)
        {
            var audit = getAuditByuser(userid).Where(x => string.IsNullOrEmpty(x.state)).ToList();
            if (audit.Count > 0)
            {
                string Strid = string.Join(",", audit.Select(x => x.officeid));
                var data = getOfficebyid(Strid).Where(x => x.aduitstate != "待提交" && string.IsNullOrEmpty(x.aduitresult)).OrderByDescending(x => x.createdate).ToList();
                foreach (var item in data)
                {
                    item.audit = getAuditByid(item.id).OrderBy(x => x.sort).ToList();

                    var fileList = fileBll.GetFilesByRecIdNew(item.id);
                    //foreach (var items in fileList)
                    //{
                    //    items.FilePath = items.FilePath.Replace("~/", url);
                    //}
                    item.statusquoFiles = fileList.Where(x => x.Description == "xz").ToList();
                    item.proposedFiles = fileList.Where(x => x.Description == "ty").ToList();
                }
                return data;
            }
            else
            {
                return new List<SevenSOfficeEntity>();
            }
        }

        /// <summary>
        /// 获取精益管理数据
        /// </summary>
        /// <returns></returns>
        public List<SevenSOfficeEntity> getOfficeByid(string Strid)
        {

            var data = getOfficebyid(Strid).OrderByDescending(x => x.createdate).ToList();
            foreach (var item in data)
            {
                item.audit = getAuditByid(item.id).OrderBy(x => x.sort).ToList();
                var fileList = fileBll.GetFilesByRecIdNew(item.id);
                //foreach (var items in fileList)
                //{
                //    items.FilePath = items.FilePath.Replace("~/", url);
                //}
                item.statusquoFiles = fileList.Where(x => x.Description == "xz").ToList();
                item.proposedFiles = fileList.Where(x => x.Description == "ty").ToList();
            }
            return data;
        }


        /// <summary>
        /// 获取用户精益管理数据
        /// </summary>
        /// <param name="Strid"></param>
        /// <returns></returns>
        public List<SevenSOfficeEntity> getOfficebyid(string Strid)
        {
            return service.getOfficebyid(Strid);
        }


        /// <summary>
        /// 数据操作
        /// </summary>
        /// <param name="add"></param>
        /// <param name="update"></param>
        /// <param name="del"></param>
        /// <param name="audit"></param>
        public void Operation(SevenSOfficeEntity add, SevenSOfficeEntity update, string del, SevenSOfficeAuditEntity audit, SevenSOfficeAuditEntity auditupdate)
        {
            try
            {
                if (audit != null)
                {
                    if (add != null)
                    {
                        if (string.IsNullOrEmpty(add.aduitstate))
                        {
                            add.aduitstate = "待审核";
                        }
                        audit.officeid = add.id;
                    }

                    audit.auditid = Guid.NewGuid().ToString();
                    var list = getAuditByid(audit.officeid);
                    audit.sort = list.Count() + 1;
                }
                service.Operation(add, update, del, audit, auditupdate);
                #region 消息推送
                MessageBLL messagebll = new MessageBLL();
                if (add != null)
                {
                    //待办通知
                    messagebll.SendMessage("待审核7S创新提案", add.id);
                }
                if (auditupdate != null)
                {
                    //修改时无新增
                    if (audit == null && update == null)
                    {
                        if (auditupdate.state == "审核通过")
                        {
                            messagebll.SendMessage("7S创新提案审批通过", auditupdate.officeid);
                        }
                        else
                        {
                            messagebll.SendMessage("7S创新提案审批不通过", auditupdate.officeid);
                        }
                    }
                }
                #endregion

            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 获取提案审核记录
        /// </summary>
        /// <param name="officeid"></param>
        /// <returns></returns>
        public List<SevenSOfficeAuditEntity> getAuditByid(string officeid)
        {

            return service.getAuditByid(officeid);
        }


        /// <summary>
        /// 获取提案审核记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<SevenSOfficeAuditEntity> getAuditByuser(string userid)
        {
            return service.getAuditByuser(userid);

        }

        /// 获取提案审核记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<SevenSOfficeAuditEntity> getAuditId(string Id)
        {
            return service.getAuditId(Id);

        }
        /// <summary>
        /// 平台查询
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public List<SevenSOfficeEntity> SelectOffice(string userid, Dictionary<string, string> keyValue, Pagination pagination)
        {

            var data = service.SelectOffice(userid, keyValue, pagination);
            //foreach (var item in data)
            //{
            //    item.audit = getAuditByid(item.id).OrderBy(x => x.sort).ToList();
            //    var fileList = fileBll.GetFilesByRecIdNew(item.id);
            //    item.auditStatueFiles = fileList.Where(x => x.Description == "2").ToList();
            //    item.auditResultFiles = fileList.Where(x => x.Description == "1").ToList();
            //}
            return data;
        }


        ///// <summary>
        ///// 统计查询
        ///// </summary>
        ///// <param name="keyValue"></param>
        ///// <returns></returns>
        //public List<SevenSOfficeEntity> SelectTotal(Dictionary<string, string> keyValue)
        //{

        //    var data = service.SelectTotal(keyValue);
        //    return data;
        //}

        /// <summary>
        /// 统计查询
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public object SelectTotalByDeptYear(Dictionary<string, string> keyValue, string userid)
        {

            var data = service.SelectTotal(keyValue, userid);
            var month = data.Select(x => new { x.createdate.Value.Month });
            Dictionary<int, int> yearTotal = new Dictionary<int, int>();
            for (int i = 1; i < 13; i++)
            {
                int count = month.Where(x => x.Month == i).Count();
                yearTotal.Add(i, count);
            }
            return yearTotal.Select(x => new { month = x.Key, count = x.Value });
        }
        /// <summary>
        /// 统计查询
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public object GetCount(Dictionary<string, string> keyValue)
        {
            var user = OperatorProvider.Provider.Current();

            DepartmentBLL dept = new BaseManage.DepartmentBLL();
            var root = dept.GetRootDepartment();
            var deptList = dept.GetSubDepartments(root.DepartmentId, "班组");
            var data = service.SelectTotal(keyValue, user.UserId);
            List<object> Total = new List<object>();
            foreach (var item in deptList)
            {
                int count = data.Where(x => x.deptid == item.DepartmentId).Count();
                Total.Add(new { seven = item.FullName, value = count });
            }
            return Total;
        }
        public int GetTodoCount(string userId)
        {
            return service.GetTodoCount(userId);
        }

        #endregion
    }
}
