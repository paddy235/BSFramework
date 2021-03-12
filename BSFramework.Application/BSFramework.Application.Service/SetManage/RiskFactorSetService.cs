using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.IService.SetManage;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BSFramework.Util;
using BSFramework.Data.EF;

namespace BSFramework.Application.Service.SetManage
{
    public class RiskFactorSetService : RepositoryFactory<RiskFactorSetEntity>, IRiskFactorSetService
    {
        private System.Data.Entity.DbContext _context;

        public RiskFactorSetService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        /// <summary>
        /// 得到危险因素及防范措施列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<RiskFactorSetEntity>()
                        join b in db.IQueryable<MeasureSetEntity>() on a.ID equals b.RiskFactorId into into1
                        from t in into1.DefaultIfEmpty()
                        select new { tid = a.ID, riskfactor = a.Content, measure = t.Content, a.DeptId, a.CreateDate };


            //pagination.p_kid = "a.id as tid";
            //pagination.p_fields = "a.content as riskfactor,b.CONTENT as measure";
            //pagination.p_tablename = "wg_riskfactorset a left join wg_measureset b on a.ID = b.RISKFACTORID";

            JObject jo = JObject.Parse(queryJson);
            if (jo["deptid"] != null)
            {
                // pagination.conditionJson += string.Format(" and a.DeptId like '%{0}%'", jo["deptid"].ToString());
                var deptid = jo["deptid"].ToString();
                query = query.Where(x => x.DeptId.Contains(deptid));
            }
            //return this.BaseRepository().FindTableByProcPager(pagination, Data.DatabaseType.MySql);

            //var dataStr = JsonConvert.SerializeObject(data);
            pagination.records = query.Count();
            query = query.OrderBy(x => x.CreateDate).ThenBy(x => x.riskfactor).ThenBy(x => x.measure).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            var dataTable = DataHelper.ConvertToTable(query);
            return dataTable;


        }

        /// <summary>
        /// 得到当前班组下的危险因素列表
        /// </summary>
        /// <param name="deptid">部门id 必传</param>
        /// <param name="content">危险因素 （模糊查询）</param>
        /// <returns></returns>
        public IEnumerable<RiskFactorSetEntity> GetList(string deptid, string content)
        {
            var query = this.BaseRepository().IQueryable().Where(t => t.DeptId == deptid);
            if (!string.IsNullOrEmpty(content))
                query = query.Where(t => t.Content.Contains(content));
            return query.OrderByDescending(t => t.CreateDate).ToList();
        }
        /// <summary>
        /// 得到危险因素实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public RiskFactorSetEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }

        /// <summary>
        /// 得到危险因素实体
        /// </summary>
        /// <param name="content">危险因素</param>
        /// <returns></returns>
        public RiskFactorSetEntity GetEntityByContent(string content)
        {
            return this.BaseRepository().FindEntity(t => t.Content == content);
        }

        /// <summary>
        /// 删除危险因素,会把危险因素关联的防范措施一起删除
        /// </summary>
        /// <param name="keyValue"></param>
        public void RemoveForm(string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                throw new ArgumentException("删除时，条件必传");
            }

            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete<RiskFactorSetEntity>(t => t.ID.Equals(keyValue));
                db.Delete<MeasureSetEntity>(t => t.RiskFactorId.Equals(keyValue));
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 保存危险因素及防范措施
        /// </summary>
        /// <param name="entity"></param>
        public void SaveForm(RiskFactorSetEntity entity)
        {
            var res = DbFactory.Base().BeginTrans();
            try
            {
                Repository<RiskFactorSetEntity> resRiskFactorSet = new Repository<RiskFactorSetEntity>(DbFactory.Base());
                //当前班组下，此危险因素是否存在
                RiskFactorSetEntity riskFactorSetEntity = null;
                if (!string.IsNullOrEmpty(entity.ID))
                {
                    riskFactorSetEntity = resRiskFactorSet.FindEntity(x => x.ID == entity.ID);
                }
                else
                {
                    riskFactorSetEntity = resRiskFactorSet.FindEntity(x => x.Content == entity.Content && x.DeptId == entity.DeptId);
                }
                if (riskFactorSetEntity == null)
                {
                    //新增
                    entity.Create();
                    res.Insert(entity);

                    //防范措施处理
                    if (entity.measures != null && entity.measures.Count > 0)
                    {
                        for (int i = 0; i < entity.measures.Count; i++)
                        {
                            var m = entity.measures[i];
                            m.ID = Guid.NewGuid().ToString();
                            m.CreateDate = DateTime.Now;
                            m.RiskFactorId = entity.ID;
                            m.Sort = i;
                            res.Insert(m);
                        }
                    }
                }
                else
                {
                    //编辑
                    riskFactorSetEntity.Modify(riskFactorSetEntity.ID);
                    riskFactorSetEntity.Content = entity.Content;
                    riskFactorSetEntity.DeptId = entity.DeptId;
                    riskFactorSetEntity.DeptName = entity.DeptName;
                    this.BaseRepository().Update(riskFactorSetEntity);

                    //如果有需要删除的防范措施
                    if (entity.measureids != null && entity.measureids.Count > 0)
                    {
                        foreach (string measureid in entity.measureids)
                        {
                            res.Delete<MeasureSetEntity>(measureid);
                        }
                    }
                    //防范措施处理
                    if (entity.measures != null && entity.measures.Count > 0)
                    {
                        var sort = 0;
                        //取防范措施个数
                        Repository<MeasureSetEntity> measureSet = new Repository<MeasureSetEntity>(DbFactory.Base());
                        var ss = measureSet.IQueryable().Where(t => t.RiskFactorId == riskFactorSetEntity.ID).ToList();
                        if (ss.Count > 0) sort = ss.Max(x => x.Sort).Value;

                        for (int i = 0; i < entity.measures.Count; i++)
                        {
                            var m = entity.measures[i];
                            if (string.IsNullOrEmpty(m.ID))
                            {
                                sort++;
                                m.ID = Guid.NewGuid().ToString();
                                m.CreateDate = DateTime.Now;
                                m.RiskFactorId = riskFactorSetEntity.ID;
                                m.Sort = sort;
                                res.Insert(m);
                            }
                            else
                            {
                                MeasureSetEntity measureSetEntity = measureSet.FindEntity(m.ID);
                                measureSetEntity.Content = m.Content;
                                res.Update(measureSetEntity);
                            }
                        }

                        var removed = ss.Where(x => !entity.measures.Any(y => y.ID == x.ID)).ToList();
                        measureSet.Delete(removed);
                    }
                    else
                    {
                        //如果防范措施为0，且keyValue不是空，则说明是编辑时，删除了所有的防范措施，这时危险因素也删除
                        if (!string.IsNullOrEmpty(entity.ID))
                        {
                            res.Delete<RiskFactorSetEntity>(entity.ID);
                        }
                    }
                }
                res.Commit();
            }
            catch (Exception ex)
            {
                res.Rollback();
                throw ex;
            }
        }

        public List<MeasureSetEntity> GetList(string id)
        {
            return _context.Set<MeasureSetEntity>().AsNoTracking().Where(x => x.RiskFactorId == id).OrderBy(x => x.CreateDate).ToList();
        }
    }
}
