using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.DrugManage;
using BSFramework.Application.IService.DrugManage;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using BSFramework.Util.Extension;
using BSFramework.Util;

namespace BSFramework.Application.Service.DrugManage
{
    public class DrugGlassWareService : RepositoryFactory<DrugGlassWareEntity>, IDrugGlassWareService
    {
        public IEnumerable<DrugGlassWareEntity> GetPageList(Pagination pagination, string queryJson, string type)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<DrugGlassWareEntity>();
            var queryParam = queryJson.ToJObject();
            if (!queryParam["GlassWareName"].IsEmpty())
            {
                string jobcontent = queryParam["GlassWareName"].ToString();
                expression = expression.And(t => t.GlassWareName.Contains(jobcontent));
            }
            //if (!queryParam["Category"].IsEmpty())
            //{
            //    string Category = queryParam["Category"].ToString();
            //    expression = expression.And(t => t.Category == Category);
            //}
            //expression = expression.And(t => t.TypeId == 2);
            expression = expression.And(t => t.GlassWareType == type);
            var query = BaseRepository().IQueryable(expression);
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.CreateDate), out count);
            pagination.records = count;
            return data;
            //return db.FindList(expression, pagination);
        }
    }
}
