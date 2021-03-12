using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Service.EducationManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.EducationManage
{
    public class EducationBLL
    {
        private IEduAppraiseService appraiseService = new EduAppraiseService();
        private IEduBaseInfoService baseService = new EduBaseInfoService();
        private IEduBookService bookService = new EduBookService();
        private IEduMessageService messageService = new EduMessageService();
        private IEduAnswerService answerService = new EduAnswerService();
        private IEduCommentTagService commentService = new EduCommentTagService();
        private IEduInventoryService inventoryService = new EduInventoryService();
        public IEnumerable<EduInventoryEntity> GetInventoryList(string deptcode, string ids, string type, string name, int pageSize, int pageIndex, out int total)
        {
            return inventoryService.GetList(deptcode, ids, type, name, pageSize, pageIndex, out total);
        }

        public List<EduBaseInfoEntity> List(int pageSize, int pageIndex)
        {
            return baseService.List(pageSize, pageIndex);
        }

        public void DelEducation(string keyValue)
        {
            baseService.RemoveForm(keyValue);
        }

        public EduBaseInfoEntity Get(string id)
        {
            return baseService.Get(id);
        }

        public EduInventoryEntity GetEntity(string keyValue)
        {
            return inventoryService.GetEntity(keyValue);
        }

        public List<EduBaseInfoEntity> FilterByMeeting(string id)
        {
            return baseService.FilterByMeeting(id);
        }

        public void RemoveForm(string keyValue)
        {
            inventoryService.RemoveForm(keyValue);
        }
        public void SaveForm(string keyValue, EduInventoryEntity entity)
        {
            inventoryService.SaveForm(keyValue, entity);
        }
        public IEnumerable<EduBaseInfoEntity> GetAllList()
        {
            return baseService.GetAllList();
        }

        public List<EduBaseInfoEntity> GetList(string[] depts)
        {
            return baseService.GetList(depts);
        }

        public List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to)
        {
            return baseService.GetList(depts, from, to);
        }

        public List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to, string flow)
        {
            return baseService.GetList(depts, from, to, flow);
        }

        public void Add(EduBaseInfoEntity entity)
        {
            baseService.Add(entity);
        }

        public List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to, string flow, string edutype)
        {
            return baseService.GetList(depts, from, to, flow, edutype).ToList();
        }

        public List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to, string flow, string edutype, int pageSize, int pageIndex, out int total)
        {
            return baseService.GetList(depts, from, to, flow, edutype, pageSize, pageIndex, out total).ToList();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<EduBaseInfoEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            return baseService.GetPageList(pagination, queryJson, userid);

        }


        /// <summary>
        /// 教育培训连接安全学习日
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<EduBaseInfoEntity> GetPageListEdAndAc(Pagination pagination, string queryJson, string userid)
        {
            return baseService.GetPageListEdAndAc(pagination, queryJson, userid);

        }
        public IEnumerable<EduBaseInfoEntity> GetAllList(DateTime from, string code)
        {
            return baseService.GetAllList(from, code);
        }
        public string GetLearnCount(string deptid, DateTime f, DateTime t)
        {
            return baseService.GetLearnCount(deptid, f, t);
        }
        public string GetCount(string deptid, DateTime f, DateTime t)
        {
            return baseService.GetCount(deptid, f, t);
        }
        public DataTable GetCountTable(string deptid, DateTime f, DateTime t)
        {
            return baseService.GetCountTable(deptid, f, t);
        }
        public DataTable GetGroupCount(string deptid, DateTime f, DateTime t)
        {
            return baseService.GetGroupCount(deptid, f, t);
        }
        public DataTable GetTimeCount(string deptid, DateTime f, DateTime t, string category)
        {
            return baseService.GetTimeCount(deptid, f, t, category);
        }
        public EduBaseInfoEntity GetBaseInfoEntity(string id)
        {
            return baseService.GetEntity(id);
        }

        public EduAppraiseEntity GetAppraiseEntity(string id)
        {
            return appraiseService.GetEntity(id);
        }

        public EduAnswerEntity GetAnswerEntity(string id)
        {
            return answerService.GetEntity(id);
        }

        public EduBookEntity GetBookEntity(string id)
        {
            return bookService.GetEntity(id);
        }

        public void Modify(EduBaseInfoEntity entity)
        {
            baseService.Modify(entity);
        }

        public EduMessageEntity GetMessageEntity(string id)
        {
            return messageService.GetEntity(id);
        }
        public IEnumerable<EduAppraiseEntity> GetAppraiseList(string deptid, string eduid)
        {
            return appraiseService.GetList(deptid, eduid);
        }
        public IEnumerable<EduBaseInfoEntity> GetBaseInfoList(string deptid)
        {
            return baseService.GetList(new string[] { deptid });
        }
        public IEnumerable<EduBookEntity> GetBookList(string deptid, string eduid)
        {
            return bookService.GetList(deptid, eduid);
        }

        /// <summary>
        /// 获取点评标签数据
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public IEnumerable<EduCommentTagEntity> GetCommentTagList(string deptId)
        {
            return commentService.GetList(deptId);
        }

        /// <summary>
        /// 提交点评标签数据
        /// </summary>
        /// <param name="entity"></param>
        public void PostEduCommentTag(EduCommentTagEntity entity)
        {
            commentService.SaveForm(string.Empty, entity);
        }
        public void DelEduCommentTag(string id)
        {
            commentService.RemoveForm(id);
        }
        public IEnumerable<EduMessageEntity> GetMessageList(string deptid, string eduid)
        {
            return messageService.GetList(deptid, eduid);
        }

        public IEnumerable<EduMessageEntity> GetMessageListByUser(string userid)
        {
            return messageService.GetListByUser(userid);
        }
        public IEnumerable<EduAnswerEntity> GetAnswerList(string eduid)
        {
            return answerService.GetList(eduid);
        }
        //public DataTable GetEducationPageList(Pagination pagination, string queryJson)
        //{
        //    return baseService.GetEducationPageList(pagination, queryJson);
        //}

        public void SaveEduBaseInfo(string id, EduBaseInfoEntity entity)
        {
            baseService.SaveForm(id, entity);
        }
        public void updataEduBaseInfo(EduBaseInfoEntity entity)
        {
            baseService.update(entity);
        }

        public void SaveAnswer(string id, EduAnswerEntity entity)
        {
            answerService.SaveForm(id, entity);
        }

        public void SaveAnswerComment(string id, EduAnswerEntity entity)
        {
            answerService.SaveForm(id, entity);
        }

        public void UdateAnswerComment(List<EduAnswerEntity> data, EduBaseInfoEntity entity)
        {
            answerService.SaveAnswerComment(data, entity);
        }

        public void SaveMessage(string id, EduMessageEntity entity)
        {
            messageService.SaveForm(id, entity);
        }

        public void SaveBook(string id, EduBookEntity entity)
        {
            bookService.SaveForm(id, entity);
        }

        public void SaveAppraise(string id, EduAppraiseEntity entity)
        {
            appraiseService.SaveForm(id, entity);
        }

        /// <summary>
        /// 结束教育培训
        /// </summary>
        /// <param name="eduId"></param>
        public void FinshEduBaseInfo(string eduId)
        {
            baseService.Finsh(eduId);
        }
        public void DelMessage(string id)
        {
            messageService.RemoveForm(id);
        }
        /// <summary>
        /// 随机选取回答人员
        /// </summary>
        /// <param name="eduBaseInfoId"></param>
        /// <returns>随机选取的人员姓名</returns>
        public string RandomAnswerSelectUser(string eduBaseInfoId)
        {
            var entity = baseService.GetEntity(eduBaseInfoId);
            var answerList = answerService.GetList(eduBaseInfoId);

            List<EduAnswerEntity> EduAnswerList = new List<EduAnswerEntity>();
            List<EduAnswerEntity> RandomContainerList = new List<EduAnswerEntity>();

            string[] aryAttendPeople = entity.AttendPeople.Split(',');
            string[] aryAttendPeopleId = entity.AttendPeopleId.Split(',');

            //循环拼装教育培训参加人员
            for (var i = 0; i < aryAttendPeople.Length; i++)
            {
                EduAnswerList.Add(new EduAnswerEntity { AnswerPeopleId = aryAttendPeopleId[i], AnswerPeople = aryAttendPeople[i] });
            }

            //遍历教育培训人员 ， 将为参加的筛选出来
            foreach (var item in EduAnswerList)
            {
                if (answerList.Count(c => c.AnswerPeopleId.Contains(item.AnswerPeopleId)) == 0)
                {
                    RandomContainerList.Add(item);
                }
            }

            //在参加选择的人员的数组中随机选择一个索引
            int index = new Random().Next(0, RandomContainerList.Count);
            string selectUser = string.Empty;

            if (RandomContainerList.Count > 0)
            {
                selectUser = RandomContainerList[index].AnswerPeople;
            }

            return selectUser;

        }

        public int GetNum3(string deptid)
        {
            return baseService.GetNum3(deptid);
        }

        public int GetNum2(string deptid)
        {
            return baseService.GetNum2(deptid);
        }

        public int GetNum1(string deptid)
        {
            return baseService.GetNum1(deptid);
        }

        public dynamic FindTrainings(string key, int limit)
        {
            return answerService.FindTrainings(key, limit);
        }

        /// <summary>
        /// 教育培训统计接口 每月
        /// </summary>
        /// <returns></returns>
        public object GetMonthTotal(string userid)
        {
            var user = new UserBLL().GetEntity(userid);
            var nowTime = DateTime.Now;
            var Start = new DateTime(nowTime.Year, nowTime.Month, 1, 0, 0, 0);
            var End = Start.AddMonths(1).AddMinutes(-1);
            DepartmentBLL deptbll = new DepartmentBLL();
            var deptId = string.Empty;
            var dept = deptbll.GetEntity(user.DepartmentId);
            deptId = user.UserId == "System" || dept.IsSpecial ? deptbll.GetRootDepartment().DepartmentId : user.DepartmentId;
            var DeptData = deptbll.GetSubDepartments(deptId, "班组").Select(x => new { deptId = x.DepartmentId, deptName = x.FullName, Encode = x.EnCode }).ToList();
            var deptStr = "\"" + string.Join("\",\"", DeptData.Select(x => x.deptId)) + "\"";
            var List = baseService.GetListBySql(deptStr, Start, End).ToList();
            //var List = baseService.GetListBySql(string.Format(" and bzid in ({0}) and activitydate  between \"{1}\" and \"{2}\" ", deptStr, Start.ToString(), End.ToString())).ToList();
            var GroupList = List.GroupBy(x => x.BZId);
            var result = new List<object>();
            foreach (var item in DeptData)
            {
                var deptList = GroupList.FirstOrDefault(x => x.Key == item.deptId);
                if (deptList != null && deptList.Count() > 0)
                {

                    var jsjk = deptList.Where(x => x.EduType == "1" || x.EduType == "5").Count();
                    var jswd = deptList.Where(x => x.EduType == "2").Count();
                    var sgyx = deptList.Where(x => x.EduType == "3" || x.EduType == "6").Count();
                    result.Add(new { dept = item.deptName, jsjk = jsjk, jswd = jswd, sgyx = sgyx });

                }
                else
                {
                    result.Add(new { dept = item.deptName, jsjk = 0, jswd = 0, sgyx = 0 });
                }
            }
            //var EnumName = Enum.GetName(typeof(EduTypeEnum), 1);
            //var EnumType = (EduTypeEnum)Enum.Parse(typeof(EduTypeEnum), EnumName);
            //var Description = EnumAttribute.GetDescription(EnumType);
            return result;
        }

        /// <summary>
        /// 随机抽取一条技术问答库的数据
        /// </summary>
        /// <returns></returns>
        public EduInventoryEntity GetRadEntity(string deptCode)
        {
            return inventoryService.GetRadEntity(deptCode);
        }

        public EduBaseInfoEntity GetDetail(string id)
        {
            return baseService.GetDetail(id);
        }

        public void EditEducation(EduBaseInfoEntity data)
        {
            baseService.EditEducation(data);
        }

        public List<EduBaseInfoEntity> GetList(string name, string category, DateTime? start, DateTime? end, string appraise, string[] depts, int rows, int page, out int total)
        {
            return baseService.GetList(name, category, start, end, appraise, depts, rows, page, out total);
        }


    }
}
