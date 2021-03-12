using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;
using BSFramework.Application.IService.Activity;
using BSFramework.Application.Service.Activity;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.Activity
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class SafetydayBLL
    {
        private SafetydayIService service = new SafetydayService();

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<SafetydayEntity> GetList(string queryJson)
        {
            return service.GetList(queryJson);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public SafetydayEntity GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        public List<SafetydayEntity> GetIdEntityList(string keyValue)
        {
            return service.GetIdEntityList(keyValue);
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            return service.GetPageList(pagination, queryJson);
        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<SafetydayEntity> GetPagesList(Pagination pagination, string queryJson) {
            return service.GetPagesList(pagination, queryJson);
        }
        public List<SafetydayMaterialEntity> getMaterial(string deptid, string keyvalue)
        {

            return service.getMaterial(deptid, keyvalue);
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
        /// 获取单位某时间段内的活动开展次数统计
        /// </summary>
        /// <param name="enumerable">单位的主键的集合</param>
        /// <param name="dateTime">开始时间</param>
        /// <param name="date">结束时间</param>
        /// <param name="catagory">活动类型</param>
        /// <returns></returns>
        public List<KeyValue> GetStatistics(IEnumerable<string> enumerable, DateTime dateTime, DateTime date,string catagory= "安全日活动")
        {
            return service.GetStatistics(enumerable, dateTime, date,catagory);
        }

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, SafetydayEntity entity, string path)
        {
            try
            {
                service.SaveForm(keyValue, entity, path);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SafetydayEntity> GetList(string name, int pagesize, int pageindex, out int total)
        {
            return service.GetList(name, pagesize, pageindex, out total);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void RemoveFile(string keyValue)
        {
            try
            {
                service.RemoveFile(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SaveRead(string keyValue, string userId)
        {
            try
            {
                service.SaveRead(keyValue, userId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<SafetydayEntity> GetSafetydayList(string userid, int page, int pagesize, string category, out int total)
        {

            return service.GetSafetydayList(userid, page, pagesize, category, out total);
        }

        public List<SafetydayEntity> GetSafetydayList(string deptId, string category)
        {

            return service.GetSafetydayList(deptId, category);
        }
        public SafetydayReadEntity GetSafetydayReadEntity(string deptId, string userid, string safetydayid)
        {

            return service.GetSafetydayReadEntity(deptId, userid, safetydayid);
        }
        public IList<SafetydayReadEntity> GetIndex(string deptid, string userid, string category)
        {
            return service.GetIndex(deptid, userid, category);
        }
        /// <summary>
        /// 得到学习园地
        /// </summary>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public DataTable GetLearningGardens(int? rowcount = null)
        {
            return service.GetLearningGardens(rowcount);
        }
        #endregion
    }
}
