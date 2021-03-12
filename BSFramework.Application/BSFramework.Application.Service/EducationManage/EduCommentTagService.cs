using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Entity.PublicInfoManage;


namespace BSFramework.Application.Service.EducationManage
{
    public class EduCommentTagService : RepositoryFactory<EduCommentTagEntity>, IEduCommentTagService
    {
        public IEnumerable<EduCommentTagEntity> GetList(string deptId)
        {
            // string deptid = OperatorProvider.Provider.Current().DeptId;
            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(deptId))
            {
                query = query.Where(x => x.DeptId == deptId);
            }
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }


        public EduCommentTagEntity GetEntity(string keyValue)
        {
            EduCommentTagEntity entity = this.BaseRepository().FindEntity(keyValue);
            return entity;
        }

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }


        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, EduCommentTagEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.CreateDate = DateTime.Now;
                this.BaseRepository().Insert(entity);
                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
        }
        #endregion
    }
}
