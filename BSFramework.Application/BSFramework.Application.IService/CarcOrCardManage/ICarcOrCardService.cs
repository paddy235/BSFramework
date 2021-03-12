using BSFramework.Application.Entity.CarcOrCardManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.CarcOrCardManage
{
    /// <summary>
    /// carc 手袋卡
    /// </summary>
    public interface ICarcOrCardService
    {


        #region 获取
        /// <summary>
        /// 列表分页 carc
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数
        /// { "state":"状态  0 保存 1 结束","type":"类型 carc  card ","deptid":" DeptCode StartsWith查询 无根据userid获取","starttime":"开始时间","endtime":"结束时间" }
        /// </param>
        /// <param name="userid">userid</param>
        /// <returns></returns>
        List<CarcEntity> GetPageList(Pagination pagination, string queryJson, string userid);

        /// <summary>
        /// 列表分页 card
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数
        /// {"deptid":" DeptCode StartsWith查询 无根据userid获取" }
        /// </param>
        /// <param name="userid">userid</param>
        /// <returns></returns>
        List<CCardEntity> GetCPageList(Pagination pagination, string queryJson, string userid);

        /// <summary>
        /// 列表分页 班会获取
        /// </summary>
        /// <returns></returns>
        List<CarcEntity> GetPageList(string keyvalue, int pagesize, int page, out int total);


        /// <summary>
        /// 获取详情carc
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        CarcEntity GetDetail(string keyvalue);

        /// <summary>
        /// 获取详情 card
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        CCardEntity GetCDetail(string keyvalue);

        /// <summary>
        /// 获取模糊查询详情List card
        /// </summary>
        /// <param name="VagueName"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<CCardEntity> GetVagueList(string VagueName, string userid);

        /// <summary>
        /// 检索  所有风险因素
        /// </summary>
        /// <param name="VagueName"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<CDangerousEntity> getDanger(string VagueName, int rows, int page, out int total);

        /// <summary>
        /// 检索  所有措施
        /// </summary>
        /// <param name="VagueName"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        List<CMeasureEntity> getMeasure(string VagueName, int rows, int page, out int total);

        #endregion
        #region 操作
        /// <summary>
        /// 新增 修改 carc
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        void SaveForm(List<CarcEntity> entity, string userid);


        /// <summary>
        /// 删除
        /// </summary>
        void deleteEntity(string keyvalue);

        /// <summary> 
        /// 新增 修改 card
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        void CSaveForm(List<CCardEntity> entity, string userid);


        /// <summary>
        /// 删除 card
        /// </summary>
        void deleteCEntity(string keyvalue);

        ///// <summary>
        ///// 新增 修改
        ///// </summary>
        //void SaveFormCMeasure(CMeasureEntity Cmeasure);


        ///// <summary>
        ///// 删除
        ///// </summary>
        //void deleteCMeasureEntity(string keyvalue);


        #endregion



    }
}
