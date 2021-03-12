using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Data;

namespace BSFramework.Application.IService.PeopleManage
{
    public interface IPeopleService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        IEnumerable<PeopleEntity> GetList(string deptid, int page, int pagesize, out int total);
        IEnumerable<LoginInfo> GetLogins();
        IEnumerable<PeopleEntity> GetListByDept(string deptid);

        LoginInfo GetLoginInfo(string keyValue);

        DutyEntity GetDutyEntity(string keyValue);
        DutyEntity GetDutyEntityByRole(string roleId);
        DutyDangerEntity GetDutyDangerEntity(string keyValue);
        DutyDangerEntity GetDutyDangerEntityByRole(string roleId);
        void SaveDuty(string id, DutyEntity entity);
        void SaveDutyDanger(string id, DutyDangerEntity entity);
        void SaveLoginInfo(string id, LoginInfo entity);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        PeopleEntity GetEntity(string keyValue);
        void dellogin(LoginInfo l);
        int CheckTel(string tel);
        DataTable GetTel(string tel);
        int CheckNo(string labour, string id);

        int CheckNo(string labour);

        int CheckTel1(string tel, string id);

        int CheckIdentity(string identity);

        int CheckIdentity(string identity, string id);

        DataTable insertBZZ();
        PeopleDutyEntity GetPeopleDuty(string keyValue);
        IEnumerable<PeopleDutyEntity> GetPeopleDutyList(string bzid);
        void SavePeopleDuty(string keyValue, PeopleDutyEntity entity);
        PeopleDutyEntity GetPeopleDuty(string peopleid, string year,string typeid);
        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, PeopleEntity entity);

        void Update(PeopleEntity entity);


        DataTable GetPeopleJson(Pagination pagination);
        #endregion
        void DelDuty(PeopleDutyEntity entity);
        PeopleDutyTypeEntity GetDutyTypeEntity(string keyValue);
        List<PeopleDutyTypeEntity> GetDutyTypes();

        void DelDutyType(string keyValue);
        void SaveDutyType(PeopleDutyTypeEntity entity);
        System.Collections.IList GetList(string userDeptCode);
    }
}
