using BSFramework.Application.Entity.SafeProduceManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.SafeProduceManage
{
    /// <summary>
    /// 安全文明生产检查
    /// </summary>
    public interface ISafeProduceService
    {
        #region 获取数据
        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        SafeProduceEntity getSafeProduceDataById(string keyvalue);

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        List<SafeProduceEntity> GetPageSafeProduceList(Pagination pagination, string queryJson);
        /// <summary>
        /// 签到统计数据
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        object GetPageSafeProduceAndSigninList(Pagination pagination, string queryJson);
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        List<OnLocaleEntity> GetPageOnLocaleList(Pagination pagination, string queryJson);



        /// <summary>
        /// 查询考勤缺勤
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        List<OnLocaleEntity> GetPageNoOnLocaleList(Pagination pagination, string queryJson);

        #endregion
        #region 数据操作
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        void operateSafeProduce(SafeProduceEntity entity);

        /// <summary>
        /// 保存修改现场终端功能踩点记录
        /// </summary>
        /// <param name="entity"></param>
        void operateOnLocale(OnLocaleEntity entity);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyvalue"></param>
        void removeSafeProduce(string keyvalue);
        #endregion

    }
}
