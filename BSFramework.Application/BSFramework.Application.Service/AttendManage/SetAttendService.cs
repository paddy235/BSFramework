using BSFramework.Application.Entity.AttendManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.AttendManage;
using System.Data;

namespace BSFramework.Application.Service.AttendManage
{
    public class SetAttendService : RepositoryFactory<SetAttendEntity>, ISetAttendService
    {

        public IEnumerable<SetAttendEntity> GetList()
        {
            //string deptid = OperatorProvider.Provider.Current().DeptId;
            var query = this.BaseRepository().IQueryable();
            return query.OrderByDescending(x => x.CreateDate).ToList();
        }
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
        public void SaveForm(string keyValue, SetAttendEntity entity)
        {

            if (string.IsNullOrEmpty(keyValue))
            {
                entity.ID = Guid.NewGuid().ToString();
                this.BaseRepository().Insert(entity);
                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }

        }
    }
}
