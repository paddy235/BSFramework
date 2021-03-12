using BSFramework.Application.Entity.KeyboxManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.KeyBoxManage
{
    /// <summary>
    /// 钥匙管理
    /// </summary>
    public interface IKeyBoxService
    {
        #region 获取数据

        #region keyBox

        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        List<KeyBoxEntity> getKeyBoxData();
        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        KeyBoxEntity getKeyBoxDataById(string Id);


        /// <summary>
        /// 获取序号
        /// </summary>
        /// <returns></returns>
        string getKeyBoxSort(string Category);




        /// <summary>
        /// 获取分类下的数量和借用数量
        /// </summary>
        /// <returns></returns>
        object getKeyBoxCategoryData(string DeptId, List<string> category);

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        List<KeyBoxEntity> GetPageKeyBoxList(Pagination pagination, string queryJson);


        #endregion
        #region keyUse
        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        KeyUseEntity getKeyUseDataById(string Id);



        /// <summary>
        /// KeyId获取数据
        /// </summary>
        /// <returns></returns>
        List<KeyUseEntity> getKeyUseDataByKeyId(string Id);


        /// <summary>
        /// 列表分页 正在借出的纪录
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        List<KeyUseEntity> GetPageKeyUseListByState(Pagination pagination, string queryJson);



        /// <summary>
        /// 列表分页 历史纪录
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        List<KeyUseEntity> GetPageKeyUseList(Pagination pagination, string queryJson);


        #endregion



        #endregion
        #region 操作数据

        #region keyBox
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        void operateKeyBox(List<KeyBoxEntity> entity);


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyvalue"></param>
        void removeKeyBox(string keyvalue);

        #endregion

        #region keyBox and keyuse
        /// <summary>
        /// 归还钥匙
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <param name="userId"></param>
        void ReturnKey(string keyvalue, string userId);

        /// <summary>
        /// 借用钥匙
        /// </summary>
        void BorrowKey(KeyUseEntity dataEntity);
        #endregion
        #region keyuse
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        void operateKeyUse(KeyUseEntity entity);


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyvalue"></param>
        void removeKeyUse(string keyvalue);

        #endregion

        #endregion

    }
}
