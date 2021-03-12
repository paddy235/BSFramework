using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.IService.EvaluateAbout;
using BSFramework.Application.Service.EvaluateAbout;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.EvaluateAbout
{
    public class EvaluateReviseBLL
    {
        /// <summary>
        /// 服务方法
        /// </summary>
        private IEvaluateReviseService service;
        public EvaluateReviseBLL()
        {
            service = new EvaluateReviseService();
        }

        /// <summary>
        /// 根据前后打分的内容，生成打分修改记录
        /// </summary>
        /// <param name="oldMarks">修正前</param>
        /// <param name="newMarks">修正后</param>
        public void Insert(EvaluateMarksRecordsEntity oldMarks, EvaluateMarksRecordsEntity newMarks)
        {
            service.Insert(oldMarks, newMarks);
        }

        /// <summary>
        /// 如果是公司级删除的部门级的数据，则会新增一条，否则的话就不会新增
        /// </summary>
        /// <param name="entity">所删除的部门级的数据</param>
        public void Insert(string deptid, EvaluateMarksRecordsEntity entity)
        {
            service.Insert(deptid, entity);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public List<EvaluateReviseEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public List<EvaluateReviseEntity> GetPagesList(Pagination pagination, string queryJson)
        {
            return service.GetPagesList(pagination, queryJson);

        }
        public List<EvaluateEntity> BindCombobox()
        {
            return service.BindCombobox();
        }
    }
}
