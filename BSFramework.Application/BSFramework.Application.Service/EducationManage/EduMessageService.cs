using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.EducationManage;

namespace BSFramework.Application.Service.EducationManage
{
    public class EduMessageService : RepositoryFactory<EduMessageEntity>, IEduMessageService
    {
        public IEnumerable<EduMessageEntity> GetList(string deptid, string eduid)
        {
            //string deptid = OperatorProvider.Provider.Current().DeptId;
            var query = this.BaseRepository().IQueryable().Where(x => x.EduId == eduid);

            return query.OrderByDescending(x => x.CreateDate).ToList();
        }
        public IEnumerable<EduMessageEntity> GetListByUser(string userid)
        {
            //string deptid = OperatorProvider.Provider.Current().DeptId;
            var query = this.BaseRepository().IQueryable().Where(x => x.InceptPeopleId == userid);

            return query.OrderByDescending(x => x.CreateDate).ToList();
        }
        public IEnumerable<EduMessageEntity> GetPageList(string deptid, int page, int pagesize, out int total)
        {
            //string deptid = OperatorProvider.Provider.Current().DeptId;
            var query = this.BaseRepository().IQueryable();
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }

        public EduMessageEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
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
        public void SaveForm(string keyValue, EduMessageEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.CreateDate = DateTime.Now;
                this.BaseRepository().Insert(entity);
                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {

                this.BaseRepository().Update(entity1);
            }
        }
        #endregion
    }
}
