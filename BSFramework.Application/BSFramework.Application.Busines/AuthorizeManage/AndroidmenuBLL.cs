using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.Service.BaseManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.BaseManage
{
    public class AndroidmenuBLL
    {
        private IAndroidmenuService bll = new AndroidmenuService();

        #region 获取数据
        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AndroidmenuEntity> GetList()
        {

            return bll.GetList();

        }
        #endregion
        #region 提交数据
        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="entity"></param>
        public void addAndroidmenu(AndroidmenuEntity entity)
        {
            bll.addAndroidmenu(entity);
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="entity"></param>
        public void modifyAndroidmenu(AndroidmenuEntity entity)
        {
            bll.modifyAndroidmenu(entity);
        }
        /// <summary>
        ///删除菜单
        /// </summary>
        /// <param name="id"></param>
        public List<FileInfoEntity> delAndroidmenu(string id)
        {
           return  bll.delAndroidmenu(id);
        }

        #endregion
    }
}
