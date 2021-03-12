using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.BaseManage
{
    public interface IAndroidmenuService
    {
        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<AndroidmenuEntity> GetList();
        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="entity"></param>
        void addAndroidmenu(AndroidmenuEntity entity);

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="entity"></param>
        void modifyAndroidmenu(AndroidmenuEntity entity);
        /// <summary>
        ///删除菜单
        /// </summary>
        /// <param name="id"></param>
        List<FileInfoEntity> delAndroidmenu(string id);
        }
}
