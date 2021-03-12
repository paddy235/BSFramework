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
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.IService.LllegalManage;
using BSFramework.Application.Service.LllegalManage;
using BSFramework.Util;
using System.IO;

namespace BSFramework.Application.Busines.LllegalManage
{
    public class LllegalBLL
    {
        private IlllegalService service = new LllegalService();

        public LllegalEntity GetLllegalDetail(string JobId)
        {
            return service.GetLllegalDetail(JobId);
        }
        public LllegalEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }

        public IEnumerable<LllegalEntity> GetListNoPage(string type, string userid, string deptid, string level, string flowstate, string sub, int page, int pagesize, out int total)
        {
            return service.GetListNoPage(type, userid, deptid, level, flowstate, sub, page, pagesize, out total);
        }
        public IEnumerable<LllegalEntity> GetLllegalList(string from, string to, int page, int pagesize, string category, string userid, out int total)
        {
            //UserEntity user = new UserBLL().GetEntity(userId);
            //string departId = user.DepartmentId;
            return service.GetLllegalList(from, to, page, pagesize, category, userid, out total);
        }
        public IEnumerable<LllegalEntity> GetLllegalList(string from, string to, int page, int pagesize, string category, string userid, string state, out int total)
        {
            return service.GetLllegalList(from, to, page, pagesize, category, userid, state, out total);
        }
        public void AddLllegalRegister(LllegalEntity entity)
        {
            service.AddLllegalRegister(entity);
        }
        public void SaveForm(string keyValue, LllegalEntity entity)
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

        public string GetListMonthLllegal(string deptid, string userid)
        {
            return service.GetListMonthLllegal(deptid, userid);
        }

        public IEnumerable<LllegalEntity> GetListMonthLllegal(string userid, DateTime from, DateTime to, string deptid)
        {
            return service.GetListMonthLllegal(userid, from, to, deptid);
        }

        public IEnumerable<LllegalEntity> GetList(string deptid, string filtertype, string filtervalue, DateTime? from, DateTime? to, int page, int pagesize, out int total)
        {
            return service.GetList(deptid, filtertype, filtervalue, from, to, page, pagesize, out total);
        }

        public DataTable getExport(string deptid, string from, string to)
        {
            return service.getExport(deptid, from, to);
        }

        public IEnumerable<LllegalEntity> GetListNew(string deptid, int page, int pagesize, out int total)
        {
            return service.GetListNew(deptid, page, pagesize, out total);
        }

        public DataTable GetLegalsList(Pagination pagination)
        {
            return service.GetLegalsList(pagination);
        }

        public List<LllegalEntity> GetData(string deptid, string deptcode, string no, string person, string level, string category, string state, int pagesize, int page, out int total)
        {
            return service.GetData(deptid, deptcode, no, person, level, category, state, pagesize, page, out total);
        }
        public List<LllegalEntity> GetApproving(string no, string person, string level, string category, int pagesize, int page, out int total)
        {
            return service.GetApproving(no, person, level, category, pagesize, page, out total);
        }

        public void Approve(LllegalEntity model)
        {
            service.Approve(model);
        }

        public string GetCount(string deptid)
        {
            return service.GetCount(deptid);
        }

        public string GetFinish(string deptid)
        {
            return service.GetFinish(deptid);
        }
        public DataTable GetMore(string deptid)
        {
            return service.GetMore(deptid);
        }
        public List<LllegalEntity> GetLllegalDetailByUser(string userId, DateTime? start, DateTime? end, bool allowPaging, int page, int pageSize)
        {
            return service.GetLllegalDetailByUser(userId, start, end, allowPaging, page, pageSize);
        }
    }
}
