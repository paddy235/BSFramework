using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;

namespace BSFramework.Application.IService.DrugManage
{
    public interface IDrugOutService
    {
        /// <summary>
        /// 保存取用记录
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="entity"></param>
        void SaveDrugOut(string Id, DrugOutEntity entity);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键ID</param>
        /// <returns></returns>
        DrugOutEntity GetEntity(string keyValue);
        /// <summary>
        /// 获取取用记录
        /// </summary>
        /// <param name="from">起始时间</param>
        /// <param name="to">结束时间</param>
        /// <param name="DrugName">药品名称or创建人名称</param>
        /// <param name="page">当前页数</param>
        /// <param name="pagesize">一页显示条数</param>
        /// <param name="total">查询总数</param>
        /// <returns></returns>
        IEnumerable<DrugOutEntity> GetOutList(string deptid, DateTime? from, DateTime? to, string DrugName, int page,int pagesize,out int total);

        /// <summary>
        /// 获取取用记录条数
        /// </summary>
        /// <param name="from">起始时间</param>
        /// <param name="to">结束时间</param>
        /// <param name="DrugName">药品名称</param>
        /// <returns></returns>
        int GetOutListCount(string deptid, DateTime? from, DateTime? to, string DrugName);
    }
}
