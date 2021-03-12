using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Util.WebControl;
using System.Data;

namespace BSFramework.Application.IService.LllegalManage
{
    public interface IlllegalService
    {
        LllegalEntity GetEntity(string keyValue);
        LllegalEntity GetLllegalDetail(string JobId);

        IEnumerable<LllegalEntity> GetListNoPage(string type, string userid, string deptid, string level, string flowstate, string sub, int page, int pagesize, out int total);

        IEnumerable<LllegalEntity> GetLllegalList(string from, string to, int page, int pagesize, string category, string userid, out int total);
        /// <summary>
        /// 查询已核准待整改的违章
        /// </summary>
        /// <param name="from">开始时间</param>
        /// <param name="to">结束时间</param>
        /// <param name="page">当前页</param>
        /// <param name="pagesize">一页显示条数</param>
        /// <param name="category">部门Id</param>
        /// <param name="approveResult">1已核准的隐患</param>
        /// <param name="total">总条数</param>
        /// <returns></returns>
        IEnumerable<LllegalEntity> GetLllegalList(string from, string to, int page, int pagesize, string category, string userid, string approveResult, out int total);
        void AddLllegalRegister(LllegalEntity entity);

        void SaveForm(string keyValue, LllegalEntity entity);

        IEnumerable<LllegalEntity> GetListMonthLllegal(string userid, DateTime from, DateTime to, string deptid);

        string GetListMonthLllegal(string deptid, string userid);

        IEnumerable<LllegalEntity> GetList(string deptid, string filtertype, string filtervalue, DateTime? from, DateTime? to, int page, int pagesize, out int total);

        DataTable getExport(string deptid, string from, string to);

        IEnumerable<LllegalEntity> GetListNew(string deptid, int page, int pagesize, out int total);

        DataTable GetLegalsList(Pagination pagination);
        List<LllegalEntity> GetData(string deptid, string deptcode, string no, string person, string level, string category, string state, int pagesize, int page, out int total);
        List<LllegalEntity> GetApproving(string no, string person, string level, string category, int pagesize, int page, out int total);
        void Approve(LllegalEntity model);

        string GetCount(string deptid);

        string GetFinish(string deptid);

        DataTable GetMore(string deptid);
        List<LllegalEntity> GetLllegalDetailByUser(string userId, DateTime? start, DateTime? end, bool allowPaging, int page, int pageSize);


    }
}
