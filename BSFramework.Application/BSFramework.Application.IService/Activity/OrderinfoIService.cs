using BSFramework.Application.Entity.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.Activity
{
    /// <summary>
    /// 描 述：班组台
    /// </summary>
    public interface OrderinfoIService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        object GetList(string deptCode);

        List<ActivityEntity> GetDetailData(string userid, string keyValue, int pagesize, int page, out int total);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        OrderinfoEntity GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="groupId">班组Id</param>
        /// <param name="sId">主表记录Id</param>
        /// <param name="userName">预约人姓名</param>
        /// <returns></returns>
        string SaveForm(string groupId, string sId,string userName);


        List<OrderinfoEntity> GetMakeList(string userid, string deptCode);
        #endregion
    }
}
