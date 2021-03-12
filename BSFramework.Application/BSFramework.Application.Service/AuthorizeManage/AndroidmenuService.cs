using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.BaseManage
{
    public class AndroidmenuService : RepositoryFactory<AndroidmenuEntity>, IAndroidmenuService
    {

        #region 获取数据
        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AndroidmenuEntity> GetList()
        {

            return this.BaseRepository().IQueryable();

        }
        #endregion
        #region 提交数据
        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="entity"></param>
        public void addAndroidmenu(AndroidmenuEntity entity)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                if (string.IsNullOrEmpty(entity.worktype))
                {
                    entity.Create();
                    db.Insert(entity);
                }
                else
                {
                    entity.Modify(entity.MenuId);
                    entity.worktype = null;
                    db.Update(entity);
                }

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="entity"></param>
        public void modifyAndroidmenu(AndroidmenuEntity entity)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                db.Update(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        ///删除菜单
        /// </summary>
        /// <param name="id"></param>
        public List<FileInfoEntity> delAndroidmenu(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var delFiles = new List<FileInfoEntity>();
                var entity = db.IQueryable<AndroidmenuEntity>().FirstOrDefault(x => x.MenuId == id);
                var oldfiles = (from q in db.IQueryable<FileInfoEntity>()
                                where q.RecId == id
                                select q).ToList();
                if (entity != null)
                {
                    if (entity.IsMenu)
                    {
                        var list = db.IQueryable<AndroidmenuEntity>().Where(x => x.ParentId == entity.MenuId);
                        foreach (var item in list)
                        {
                            var detailfiles = (from q in db.IQueryable<FileInfoEntity>()
                                               where q.RecId == item.MenuId
                                               select q).ToList();

                            db.Delete(item);
                            delFiles.AddRange(detailfiles);
                        }
                    }
                    db.Delete(entity);

                    delFiles.AddRange(oldfiles);
                }
                db.Commit();
                return delFiles;
            }
            catch (Exception)
            {
                db.Rollback();

                throw;
            }
        }

        #endregion
    }
}
