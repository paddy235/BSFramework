using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Util.WebControl;

namespace BSFramework.Application.IService.EvaluateAbout
{
    public interface IEvaluateReviseService
    {
        void Insert(EvaluateMarksRecordsEntity oldMarks, EvaluateMarksRecordsEntity newMarks);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        List<EvaluateReviseEntity> GetPagesList(Pagination pagination, string queryJson);
        void Insert(string deptid, EvaluateMarksRecordsEntity entity);
        List<EvaluateReviseEntity> GetPageList(Pagination pagination, string queryJson);
        List<EvaluateEntity> BindCombobox();
    }
}
