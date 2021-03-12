using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.Activity
{
    public interface SafetydayIService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        IEnumerable<SafetydayEntity> GetList(string queryJson);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        SafetydayEntity GetEntity(string keyValue);

        List<SafetydayEntity> GetIdEntityList(string keyValue);
         /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        DataTable GetPageList(Pagination pagination, string queryJson);
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        List<SafetydayEntity> GetPagesList(Pagination pagination, string queryJson);
        /// <summary>
        /// 得到学习园地
        /// </summary>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        DataTable GetLearningGardens(int? rowcount = null);
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
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, SafetydayEntity entity, string path);

        void SaveRead(string keyValue, string userId);
        List<SafetydayMaterialEntity> getMaterial(string deptid, string keyvalue);
        IEnumerable<SafetydayEntity> GetSafetydayList(string userid, int page, int pagesize, string category, out int total);

        List<SafetydayEntity> GetSafetydayList(string deptId, string category);

        SafetydayReadEntity GetSafetydayReadEntity(string deptId, string userid, string safetydayid);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        void RemoveFile(string keyValue);
        IList<SafetydayReadEntity> GetIndex(string deptid, string userid,string category);
        List<SafetydayEntity> GetList(string name, int pagesize, int pageindex, out int total);
        List<KeyValue> GetStatistics(IEnumerable<string> enumerable, DateTime dateTime, DateTime date, string catagory);
        #endregion
    }
}
