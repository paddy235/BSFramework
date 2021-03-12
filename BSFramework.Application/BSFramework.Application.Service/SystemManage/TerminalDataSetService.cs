using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.SystemManage
{
    public class TerminalDataSetService : RepositoryFactory<TerminalDataSetEntity>, ITerminalDataSetService
    {
        public TerminalDataSetEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }

        public List<TerminalDataSetEntity> GetList()
        {
            return this.BaseRepository().IQueryable().ToList();
        }

        public List<TerminalDataSetEntity> GetPageList(Pagination pagination, string queryJson, string dataSetType)
        {
            var query = BaseRepository().IQueryable();
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                //pagination.conditionJson += string.Format(" and Name like '%{0}%'", keyword.Trim());
                query = query.Where(x => x.Name.Contains(keyword));
            }
            if (dataSetType == "2")
            {
                //pagination.conditionJson += string.Format(" and BK4='{0}'", dataSetType);//数据库里 DataSetType的实际名称叫BK4
                query = query.Where(x => x.DataSetType==dataSetType);
            }
            else
            {
                //pagination.conditionJson += string.Format(" and (BK4 = '{0}' OR BK4 IS NULL)", dataSetType);//数据库里 DataSetType的实际名称叫BK4
                query = query.Where(x => x.DataSetType == dataSetType || x.DataSetType==null);
            }
            //DatabaseType dataType = DbHelper.DbType;
            //return this.BaseRepository().FindTableByProcPager(pagination, dataType);
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.CreateDate), out count);
            pagination.records = count;
            return data;
        }

        public void RemoveForm(string keyValue)
        {
            this.BaseRepository().Delete(keyValue);
        }

        public void SaveForm(string keyValue, TerminalDataSetEntity ds)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                ds.Create();
                this.BaseRepository().Insert(ds);
            }
            else
            {
                ds.Modify(keyValue);
                this.BaseRepository().Update(ds);
            }
        }
    }
}
