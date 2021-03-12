using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Data;

namespace BSFramework.Application.Service.EducationManage
{
    public class EduInventoryService : RepositoryFactory<EduInventoryEntity>, IEduInventoryService
    {
        public IEnumerable<EduInventoryEntity> GetList(string deptcode, string ids, string type, string name, int pageSize, int pageIndex, out int total)
        {
            var query = this.BaseRepository().IQueryable();

            if (!string.IsNullOrEmpty(deptcode))
                query = query.Where(x => x.UseDeptCode.StartsWith(deptcode) || x.UseDeptCode == "0");

            if (!string.IsNullOrEmpty(ids))
                query = query.Where(x => ids.Contains(x.ID));

            if (!string.IsNullOrEmpty(type))
            {
                if (type == "2" || type == "5")
                    query = query.Where(x => (x.EduType == "2" || x.EduType == "5") && x.Question != null);
                else if (type == "3" || type == "6")
                    query = query.Where(x => (x.EduType == "3" || x.EduType == "6") && x.Name != null);
                else
                    query = query.Where(x => x.EduType == type && x.Name != null);
            }

            if (!string.IsNullOrEmpty(name))
            {
                if (type == "2" || type == "5")
                    query = query.Where(x => x.Question.Contains(name));
                else
                    query = query.Where(x => x.Name.Contains(name));
            }

            total = query.Count();

            return query.OrderByDescending(x => x.CreateDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        }
        public EduInventoryEntity GetEntity(string keyValue)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            EduInventoryEntity entity = db.FindEntity<EduInventoryEntity>(keyValue);
            return entity;
        }

        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        public void SaveForm(string keyValue, EduInventoryEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (entity1 == null)
            {

                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.kjname = null;
                entity.kjpath = null;
                entity.fm = null;
                entity.Owner = null;
                entity.Files = null;
                this.BaseRepository().Update(entity);
            }
        }

        /// <summary>
        /// 随机抽取一条技术问答库的数据
        /// </summary>
        /// <returns></returns>
        public EduInventoryEntity GetRadEntity(string deptCode)
        {
            ///*  floor(i+rand()*(j-i+1))  i<=r<=j  i起始值  r随机数  j结束值   **/
            //string sql = @"    set @num=0;
            //                            set @allcount = 0;
            //                            set @rad = 1;
            //                            select COUNT(*) from wg_eduinventory into @allcount;
            //                            select floor(1+rand()*(@allcount-1+1)) into @rad;
            //                            select t.* from ( select  id,question,answer,@num:=@num+1 as rownum from wg_eduinventory ) t where  t.rownum =@rad;";
            //DataTable dt = BaseRepository().FindTable(sql);
            //if (dt.Rows !=null && dt.Rows.Count>0)
            //{
            //    EduInventoryEntity entity = new EduInventoryEntity()
            //    {
            //        ID = dt.Rows[0]["id"].ToString(),
            //        Question = dt.Rows[0]["question"].ToString(),
            //        Answer = dt.Rows[0]["answer"].ToString(),
            //    };
            //    return entity;
            //}
            //return null;
            var query = BaseRepository().IQueryable(p => p.UseDeptCode.StartsWith(deptCode));
            int count = query.Count();
            Random rad = new Random();
            int radCount = rad.Next(count);
            return query.OrderBy(p => p.CreateDate).Skip(radCount).Take(1).FirstOrDefault();
        }
    }
}
