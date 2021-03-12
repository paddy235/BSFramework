using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.IService.PeopleManage;
using BSFramework.Application.Service.PeopleManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using BSFramework.Util.Extension;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Data;
//using ThoughtWorks.QRCode.Codec;
using System.Text;
using System.IO;

namespace BSFramework.Application.Busines.PeopleManage
{
    /// <summary>
    /// 成员管理
    /// </summary>
    public class PeopleBLL
    {
        private IPeopleService service = new PeopleService();

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<PeopleEntity> GetList(string deptid, int page, int pagesize, out int total)
        {

            return service.GetList(deptid, page, pagesize, out total);
        }
        public IEnumerable<LoginInfo> GetLogins()
        {
            return service.GetLogins();
        }
        public void dellogin(LoginInfo l)
        {
            service.dellogin(l);
        }
        public LoginInfo GetLoginInfo(string keyValue)
        {
            return service.GetLoginInfo(keyValue);
        }
        public void SaveLoginInfo(string id, LoginInfo entity)
        {
            service.SaveLoginInfo(id, entity);
        }
        public IEnumerable<PeopleEntity> GetListByDept(string deptid)
        {
            return service.GetListByDept(deptid);
        }
        public DataTable insertBZZ()
        {
            return service.insertBZZ();
        }
        public int CheckTel(string tel)
        {
            return service.CheckTel(tel);
        }

        public DataTable GetTel(string tel)
        {
            return service.GetTel(tel);
        }
        public DataTable GetPeopleJson(Pagination pagination)
        {
            return service.GetPeopleJson(pagination);
        }
        public int CheckTel1(string tel, string id)
        {
            return service.CheckTel1(tel, id);
        }

        public int CheckNo(string labour, string id)
        {
            return service.CheckNo(labour, id);
        }
        public int CheckNo(string labour)
        {
            return service.CheckNo(labour);
        }

        public int CheckIdentity(string identity)
        {
            return service.CheckIdentity(identity);
        }

        public int CheckIdentity(string identity, string id)
        {
            return service.CheckIdentity(identity, id);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public PeopleEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        #endregion


        public PeopleDutyEntity GetPeopleDuty(string keyValue)
        {
            return service.GetPeopleDuty(keyValue);
        }
        public IEnumerable<PeopleDutyEntity> GetPeopleDutyList(string bzid)
        {
            return service.GetPeopleDutyList(bzid);
        }
        public void SavePeopleDuty(string keyValue, PeopleDutyEntity entity)
        {
            service.SavePeopleDuty(keyValue, entity);
        }
        public PeopleDutyEntity GetPeopleDuty(string peopleid, string year, string typeid)
        {
            return service.GetPeopleDuty(peopleid, year, typeid);
        }
        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                service.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, PeopleEntity entity)
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

        public void Update(PeopleEntity entity)
        {
            service.Update(entity);
        }

        /// <summary>
        /// 根据部门编码获取用户信息，包含ActivityPersonId
        /// </summary>
        /// <param name="userDeptCode">部门编码</param>
        /// <returns></returns>
        public IList GetList(string userDeptCode)
        {
            return service.GetList(userDeptCode);
        }
        #endregion

        public DutyEntity GetDutyEntity(string keyValue)
        {
            return service.GetDutyEntity(keyValue);
        }
        public DutyEntity GetDutyEntityByRole(string roleId)
        {
            return service.GetDutyEntityByRole(roleId);
        }
        public DutyDangerEntity GetDutyDangerEntityByRole(string roleId)
        {
            return service.GetDutyDangerEntityByRole(roleId);
        }
        public DutyDangerEntity GetDutyDangerEntity(string keyValue)
        {
            return service.GetDutyDangerEntity(keyValue);
        }
        public void SaveDuty(string id, DutyEntity entity)
        {
            service.SaveDuty(id, entity);
        }
        public void SaveDutyDanger(string id, DutyDangerEntity entity)
        {
            service.SaveDutyDanger(id, entity);
        }

        public PeopleDutyTypeEntity GetDutyTypeEntity(string keyValue)
        {
            return service.GetDutyTypeEntity(keyValue);
        }
        public List<PeopleDutyTypeEntity> GetDutyTypes()
        {
            return service.GetDutyTypes();
        }
        public void DelDutyType(string keyValue)
        {
            service.DelDutyType(keyValue);
        }
        public void SaveDutyType(PeopleDutyTypeEntity entity)
        {
            service.SaveDutyType(entity);
        }

        public void DelDuty(PeopleDutyEntity entity)
        {
            service.DelDuty(entity);
        }
    }
}
