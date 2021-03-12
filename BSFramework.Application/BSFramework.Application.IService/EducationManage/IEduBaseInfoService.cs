using BSFramework.Application.Entity.EducationManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.EducationManage
{
    public interface IEduBaseInfoService
    {
        List<EduBaseInfoEntity> GetList(string[] depts);
        IEnumerable<EduBaseInfoEntity> GetListApp(string deptId);
        IEnumerable<EduBaseInfoEntity> GetAllList(DateTime from, string code);
        IEnumerable<EduBaseInfoEntity> GetAllList();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<EduBaseInfoEntity> GetPageList(Pagination pagination, string queryJson, string userid);


        /// <summary>
        /// 教育培训连接安全学习日
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<EduBaseInfoEntity> GetPageListEdAndAc(Pagination pagination, string queryJson, string userid);
        List<EduBaseInfoEntity> List(int pageSize, int pageIndex);

        /// <summary>
        /// 获取教育培训任务
        /// </summary>
        /// <param name="pagination">分页公共类</param>
        /// <param name="queryJson">startTime开始时间|endTime结束时间|deptId部门id|userId用户id|Flow状态|haveEvaluate是否查询评价</param>
        /// <returns></returns>
        List<EduBaseInfoEntity> GetEdJobList(Pagination pagination, string queryJson);
        EduBaseInfoEntity Get(string id);
        IEnumerable<EduBaseInfoEntity> GetListBySql(string StrSql);
        IEnumerable<EduBaseInfoEntity> GetListBySql(string deptid, DateTime start, DateTime end);
        string GetCount(string deptid, DateTime f, DateTime t);
        DataTable GetCountTable(string deptid, DateTime f, DateTime t);
        List<EduBaseInfoEntity> FilterByMeeting(string id);
        List<EduBaseInfoEntity> GetList(string[] depts, DateTime? fromtime, DateTime? to);
        string GetLearnCount(string deptid, DateTime f, DateTime t);

        DataTable GetGroupCount(string deptid, DateTime f, DateTime t);
        DataTable GetTimeCount(string deptid, DateTime f, DateTime t, string category);
        IEnumerable<EduBaseInfoEntity> GetPageList(string deptid, int page, int pagesize, out int total);
        EduBaseInfoEntity GetEntity(string keyValue);
        void Add(EduBaseInfoEntity entity);

        //DataTable GetEducationPageList(Pagination pagination, string queryJson);
        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        int Count(string[] depts, string[] category, DateTime from, DateTime end);

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, EduBaseInfoEntity entity);
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity"></param>
        void update(EduBaseInfoEntity entity);
        List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to, string flow);
        void End(string subActivityId);
        List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to, string flow, string edutype);

        /// <summary>
        /// 结束教育培训
        /// </summary>
        /// <param name="eduId"></param>
        void Finsh(string eduId);
        int GetNum1(string deptid);
        int GetNum2(string deptid);
        List<EduBaseInfoEntity> GetList(string[] depts, DateTime? from, DateTime? to, string flow, string edutype, int pagesize, int pageindex, out int total);
        int GetNum3(string deptid);
        EduBaseInfoEntity GetDetail(string id);
        void EditEducation(EduBaseInfoEntity data);
        List<EduBaseInfoEntity> GetList(string name, string category, DateTime? start, DateTime? end, string appraise, string[] depts, int pagesize, int pageindex, out int total);
        void Modify(EduBaseInfoEntity entity);
        #endregion
    }
}
