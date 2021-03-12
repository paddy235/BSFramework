using BSFramework.Application.Entity.Activity;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    /// <summary>
    /// 描 述：班组台
    /// </summary>
    public class OrderinfoBLL
    {
        private OrderinfoIService service = new OrderinfoService();

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public object GetList(string deptCode)
        {
            return service.GetList(deptCode);
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public object GetDetailData(string userid, string keyValue, int pagesize, int page, out int total)
        {
            return service.GetDetailData(userid, keyValue, pagesize, page, out total);
        }

        public List<OrderinfoEntity> GetMakeList(string userid, string deptCode)
        {
            return service.GetMakeList(userid, deptCode);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public OrderinfoEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        #endregion

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
        ///  <param name="userName">预约人姓名</param>
        /// <returns></returns>
        public string SaveForm(string groupId, string sId, string userName)
        {
            try
            {
                return service.SaveForm(groupId, sId, userName);
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion
    }
}
