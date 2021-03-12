using BSFramework.Application.Entity.KeyboxManage;
using BSFramework.Application.IService.KeyBoxManage;
using BSFramework.Application.Service.KeyBoxManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.KeyBoxManage
{
    public class KeyBoxBLL
    {
        private IKeyBoxService service = new KeyBoxService();
        #region 获取数据

        #region keyBox
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<KeyBoxEntity> getKeyBoxData()
        {
            return service.getKeyBoxData();
        }

        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public KeyBoxEntity getKeyBoxDataById(string Id)
        {

            return service.getKeyBoxDataById(Id);
        }

        /// <summary>
        /// 获取序号
        /// </summary>
        /// <returns></returns>
        public List<string> getKeyBoxSort(string Category, string name)
        {
            var pinyin = Util.Str.PinYin(name);

            var srot = service.getKeyBoxSort(Category);
            return new List<string>() { pinyin, srot };

        }


        /// <summary>
        /// 获取分类下的数量和借用数量
        /// </summary>
        /// <returns></returns>
        public object getKeyBoxCategoryData(string DeptId, List<string> category)
        {
            return service.getKeyBoxCategoryData(DeptId, category);
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<KeyBoxEntity> GetPageKeyBoxList(Pagination pagination, string queryJson)
        {
            return service.GetPageKeyBoxList(pagination, queryJson);
        }

        #endregion
        #region keyUse
        /// <summary>
        /// id获取数据
        /// </summary>
        /// <returns></returns>
        public KeyUseEntity getKeyUseDataById(string Id)
        {
            return service.getKeyUseDataById(Id);
        }


        /// <summary>
        /// KeyId获取数据
        /// </summary>
        /// <returns></returns>
        public List<KeyUseEntity> getKeyUseDataByKeyId(string Id)
        {
            return service.getKeyUseDataByKeyId(Id);
        }

        /// <summary>
        /// 列表分页 正在借出的纪录
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<KeyUseEntity> GetPageKeyUseListByState(Pagination pagination, string queryJson)
        {
            return service.GetPageKeyUseListByState(pagination, queryJson);
        }


        /// <summary>
        /// 列表分页 历史纪录
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<KeyUseEntity> GetPageKeyUseList(Pagination pagination, string queryJson)
        {
            return service.GetPageKeyUseList(pagination, queryJson);
        }

        #endregion



        #endregion
        #region keyBox and keyuse
        /// <summary>
        /// 归还钥匙
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <param name="userId"></param>
        public void ReturnKey(string keyvalue, string userId) {
             service.ReturnKey(keyvalue, userId);
        }

        /// <summary>
        /// 借用钥匙
        /// </summary>
        public void BorrowKey(KeyUseEntity dataEntity) {
            service.BorrowKey(dataEntity);
        }
        #endregion

        #region 操作数据

        #region keyBox
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        public void operateKeyBox(List<KeyBoxEntity> entity)
        {
            service.operateKeyBox(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyvalue"></param>
        public void removeKeyBox(string keyvalue)
        {
            service.removeKeyBox(keyvalue);
        }
        #endregion

        #region keyuse
        /// <summary>
        /// 保存修改
        /// </summary>
        /// <param name="entity"></param>
        public void operateKeyUse(KeyUseEntity entity)
        {
            service.operateKeyUse(entity);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyvalue"></param>
        public void removeKeyUse(string keyvalue)
        {
            service.removeKeyUse(keyvalue);
        }
        #endregion

        #endregion

    }
}
