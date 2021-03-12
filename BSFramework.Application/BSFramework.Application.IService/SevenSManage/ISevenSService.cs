using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.SevenSManage
{
    public interface ISevenSService
    {
        #region 技术规范
        IEnumerable<SevenSEntity> GetList(string typename, string typeId, string name, string Id, int pageIndex, int pageSize, bool ispage,string deptid);

        IList<SevenSEntity> GetItems(string key, string typeid, int pagesize, int page, string deptCode, out int total);
        IList<SevenSTypeEntity> GetIndex(string name, string typeid, string deptCode);
        IList<SevenSTypeEntity> GetAllType(String deptCode);
        void DeleteType(string id);
        void DeleteItem(string id);
        SevenSEntity GetSevenSEntity(string keyValue);
        void EditType(SevenSTypeEntity model);
        void AddType(SevenSTypeEntity model);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        void SaveForm(string keyValue, SevenSEntity entity);
        void SaveSevenSEntity(SevenSEntity entity);
        #endregion
        #region 定点照片
        /// <summary>
        /// 获取周期设置数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<SevenSPictureCycleEntity> getCycle();

        /// <summary>
        /// 修正周期时间数据
        /// </summary>
        /// <returns></returns>
        void setCycle(string value);
        /// <summary>
        /// 修正周期时间数据
        /// </summary>
        /// <returns></returns>
        void setCycleTime(string value);
        /// <summary>
        /// 获取地点设置数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<SevenSPictureSetEntity> getSet();
        /// <summary>
        /// 获取地点设置数据包含当前记录id
        /// </summary>
        /// <returns></returns>
        IEnumerable<SevenSPictureSetEntity> getSetAndPicture(string deptId);
        /// <summary>
        /// 添加地点设置
        /// </summary>
        /// <param name="entity"></param>
        void InsertPhoneSet(SevenSPictureSetEntity entity);
        /// <summary>
        /// 获取记录数据
        /// </summary>
        /// <param name="entity"></param>
        SevenSPictureEntity getEntity(string keyvalue);
        /// <summary>
        /// 删除地点设置
        /// </summary>
        /// <param name="entityId"></param>
        void DelPhoneSet(string entityId);
        /// <summary>
        /// 修改数据提交状态
        /// </summary>
        void update(string id,string userid);
        /// <summary>
        ///获取记录数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<SevenSPictureEntity> getList(DateTime? planeStart, DateTime? planeEnd, string state, string evaluationState, string space, Pagination pagination, bool ispage, string deptId, bool isFile);

        /// <summary>
        /// 插入记录数据
        /// </summary>
        /// <param name="entityList"></param>
        void InsertList(List<SevenSPictureEntity> entityList);
        /// <summary>
        /// 页面操作数据
        /// </summary>
        void SaveFrom(List<SevenSPictureSetEntity> entityList, string[] del, string setTime,string regulation);

        /// <summary>
        /// 保存评价
        /// </summary>
        /// <returns></returns>  
        void SaveEvaluation(string keyValue, string evaluation, string user);


        /// <summary>
        /// 保存时间段  用于app使用
        /// </summary>
        /// <param name="entity"></param>
        void SavePlanTime(SevenSPlanTimeEntity entity);
        /// <summary>
        /// 获取未提交数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<SevenSPictureEntity> getState();
        /// <summary>
        /// 获取记录数据
        /// </summary>
        List<SevenSPictureEntity> getSevenSFinish(string deptid);
        /// <summary>
        /// 查询时间段
        /// </summary>
        List<SevenSPlanTimeEntity> getPlanTime();
        List<SevenSPictureEntity> GetListByManager(string state, string userId, bool? evaluateState, string deptid, out int totalCount, int pageIndex = 1, int pageSize = 5);
        List<SevenSPictureEntity> GetListByManager(DateTime? datefrom, DateTime? dateto, string state, string userId, bool? evaluateState, string deptid, out int totalCount, int pageIndex = 1, int pageSize = 5);

        #endregion

        #region 精益管理


        /// <summary>
        /// 获取用户精益管理数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<SevenSOfficeEntity> getOfficebyuser(string userid);
        /// <summary>
        /// 获取用户精益管理数据
        /// </summary>
        /// <param name="Strid"></param>
        /// <returns></returns>
        List<SevenSOfficeEntity> getOfficebyid(string Strid);
        /// <summary>
        /// 数据操作
        /// </summary>
        /// <param name="add"></param>
        /// <param name="update"></param>
        /// <param name="del"></param>
        /// <param name="audit"></param>
        void Operation(SevenSOfficeEntity add, SevenSOfficeEntity update, string del, SevenSOfficeAuditEntity audit, SevenSOfficeAuditEntity auditupdate);
        IList<SevenSTypeEntity> GetAllType(string[] depts);

        /// <summary>
        /// 获取提案审核记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<SevenSOfficeAuditEntity> getAuditByuser(string userid);
        /// <summary>
        /// 获取提案审核记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        List<SevenSOfficeAuditEntity> getAuditId(string Id);
        /// <summary>
        /// 获取提案审核记录
        /// </summary>
        /// <param name="officeid"></param>
        /// <returns></returns>
        List<SevenSOfficeAuditEntity> getAuditByid(string officeid);

        /// <summary>
        /// 平台查询
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        List<SevenSOfficeEntity> SelectOffice(string userid, Dictionary<string, string> keyValue, Pagination pagination);

        /// <summary>
        /// 统计查询
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        List<SevenSOfficeEntity> SelectTotal(Dictionary<string, string> keyValue, string userid);

        int GetTodoCount(string userId);

        #endregion
    }
}
