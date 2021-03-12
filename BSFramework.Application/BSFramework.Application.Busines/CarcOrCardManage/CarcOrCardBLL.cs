using BSFramework.Application.Entity.CarcOrCardManage;
using BSFramework.Application.IService.CarcOrCardManage;
using BSFramework.Application.Service.CarcOrCardManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.CarcOrCardManage
{

    /// <summary>
    /// carc  card
    /// </summary>
    public class CarcOrCardBLL
    {
        #region 获取
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数
        /// { "state":"状态  0 保存 1 结束","type":"类型 carc  card ","deptid":" DeptCode StartsWith查询 无根据userid获取","starttime":"开始时间","endtime":"结束时间" }
        /// </param>
        /// <param name="userid">userid</param>
        /// <param name="type">数据类别 carc card</param>
        /// <returns></returns>
        public List<CarcEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            ICarcOrCardService service = new CarcOrCardService();
            return service.GetPageList(pagination, queryJson, userid);
        }
        /// <summary>
        /// 列表分页 card
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数
        /// {"deptid":" DeptCode StartsWith查询 无根据userid获取" }
        /// </param>
        /// <param name="userid">userid</param>
        /// <returns></returns>
        public List<CCardEntity> GetCPageList(Pagination pagination, string queryJson, string userid)
        {
            ICarcOrCardService service = new CarcOrCardService();
            return service.GetCPageList(pagination, queryJson, userid);
        }
        /// <summary>
        /// 列表分页 班会获取
        /// </summary>
        /// <returns></returns>
        public List<CarcEntity> GetPageList(string keyvalue, int pagesize, int page, out int total)
        {
            ICarcOrCardService service = new CarcOrCardService();
            return service.GetPageList(keyvalue, pagesize, page, out total);
        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public CarcEntity GetDetail(string keyvalue)
        {
            ICarcOrCardService service = new CarcOrCardService();
            return service.GetDetail(keyvalue);
        }
        /// <summary>
        /// 获取模糊查询详情List card
        /// </summary>
        /// <param name="VagueName"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<CCardEntity> GetVagueList(string VagueName, string userid)
        {

            ICarcOrCardService service = new CarcOrCardService();
            return service.GetVagueList(VagueName, userid);
        }
        /// <summary>
        /// 检索  所有风险因素
        /// </summary>
        /// <param name="VagueName"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<CDangerousEntity> getDanger(string VagueName, int rows, int page, out int total)
        {
            ICarcOrCardService service = new CarcOrCardService();
            return service.getDanger(VagueName, rows, page, out total);

        }
        /// <summary>
        /// 检索  所有措施
        /// </summary>
        /// <param name="VagueName"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<CMeasureEntity> getMeasure(string VagueName, int rows, int page, out int total)
        {
            ICarcOrCardService service = new CarcOrCardService();
            return service.getMeasure(VagueName, rows, page, out total);
        }
        /// <summary>
        /// 获取详情 card
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public CCardEntity GetCDetail(string keyvalue)
        {
            ICarcOrCardService service = new CarcOrCardService();
            return service.GetCDetail(keyvalue);
        }
        /// <summary>
        /// card to carc 
        /// </summary>
        public CarcEntity ToCard(CCardEntity entity)
        {
            var rEntity = new CarcEntity();
            rEntity.CDangerousList = entity.CDangerousList;
            foreach (var item in rEntity.CDangerousList)
            {
                item.Cid = null;
                item.Id = null;
                foreach (var CMeasure in item.Measure)
                {
                    CMeasure.Id = null;
                    CMeasure.Cmid = null;
                }
            }
            rEntity.Id = null;
            rEntity.WorkArea = entity.WorkArea;
            rEntity.WorkName = entity.WorkName;
            rEntity.MainOperation = entity.MainOperation;
            rEntity.DataType = "carc";
            return rEntity;

        }

        #endregion
        #region 操作
        /// <summary>
        /// 新增 修改 carc
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SaveForm(List<CarcEntity> entity, string userid)
        {
            ICarcOrCardService service = new CarcOrCardService();
            service.SaveForm(entity, userid);

        }

        /// <summary>
        /// 删除 carc
        /// </summary>
        public void deleteEntity(string keyvalue)
        {

            ICarcOrCardService service = new CarcOrCardService();
            service.deleteEntity(keyvalue);
        }
        /// <summary> 
        /// 新增 修改 card
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void CSaveForm(List<CCardEntity> entity, string userid)
        {
            ICarcOrCardService service = new CarcOrCardService();
            service.CSaveForm(entity, userid);
        }


        /// <summary>
        /// 删除 card
        /// </summary>
        public void deleteCEntity(string keyvalue)
        {
            ICarcOrCardService service = new CarcOrCardService();
            service.deleteCEntity(keyvalue);
        }
        ///// <summary>
        ///// 新增 修改
        ///// </summary>
        //public void SaveFormCMeasure(CMeasureEntity Cmeasure)
        //{
        //    ICarcOrCardService service = new CarcOrCardService();
        //    service.SaveFormCMeasure(Cmeasure);
        //}
        ///// <summary>
        ///// 删除
        ///// </summary>
        //public void deleteCMeasureEntity(string keyvalue)
        //{

        //    ICarcOrCardService service = new CarcOrCardService();
        //    service.deleteCMeasureEntity(keyvalue);
        //}
        #endregion
    }
}
