using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.IService.InnovationManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.InnovationManage
{
    /// <summary>
    /// qc活动
    /// </summary>
    public class QcActivityService : RepositoryFactory<QcActivityEntity>, IQcActivityService
    {

        private DepartmentService deptService = new DepartmentService();
        /// <summary>
        /// 获取qc数据
        /// </summary>
        /// <returns></returns>
        public List<QcActivityEntity> getQcList(Dictionary<string, string> keyValue, Pagination pagination)
        {

            try
            {
                var db = new RepositoryFactory().BaseRepository();

                var expression = LinqExtensions.True<QcActivityEntity>();
                expression = expression.And(x => 1 == 1);
                //课题状态
                if (keyValue.ContainsKey("subjectstate"))
                {
                    if (!string.IsNullOrEmpty(keyValue["subjectstate"]))
                    {
                        var val = keyValue["subjectstate"];
                        expression = expression.And(x => x.subjectstate == val);
                    }
                }
                //年份
                if (keyValue.ContainsKey("year"))
                {
                    var val = keyValue["year"];
                    expression = expression.And(x => x.subjecttime.ToString().Contains(val));
                }
                //课题名称
                if (keyValue.ContainsKey("name"))
                {
                    var val = keyValue["name"];
                    expression = expression.And(x => x.subjectname.Contains(val));
                }
                var data = db.IQueryable(expression).ToList();
                var Resultdata = new List<QcActivityEntity>();
                //部门
                if (keyValue.ContainsKey("deptid"))
                {
                    var val = keyValue["deptid"];
                    if (val != "0")
                    {
                        var deptlist = deptService.GetSubDepartments(val, "厂级,部门,班组");
                        foreach (var item in deptlist)
                        {

                            foreach (var items in data)
                            {
                                if (!string.IsNullOrEmpty(items.workdeptid))
                                {
                                    if (items.workdeptid.Contains(item.DepartmentId))
                                    {
                                        if (Resultdata.IndexOf(items)<0)
                                        {
                                            Resultdata.Add(items);
                                        }
                                       
                                    }
                                }

                            }

                        }
                    }
                    else
                    {
                        Resultdata = data;
                    }
                }
                else
                {
                    Resultdata = data;
                }
                if (Resultdata.Count > 0)
                {
                    pagination.records = Resultdata.Count;
                    Resultdata = Resultdata.OrderByDescending(x => x.subjecttime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
                   
                }
                return Resultdata;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public QcActivityEntity getQcById(string keyvalue)
        {
            return this.BaseRepository().FindEntity(keyvalue);
        }

        /// <summary>
        /// 新增数据qc活动数据
        /// </summary>
        public void addEntity(QcActivityEntity qc)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (qc != null)
                {
                    db.Insert(qc);
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
        /// 修改数据
        /// </summary>
        /// <param name="qc"></param>
        public void EditEntity(QcActivityEntity qc)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (qc != null)
                {
                    db.Update(qc);
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
        /// 删除数据
        /// </summary>
        /// <param name="qc"></param>
        public void delEntity(QcActivityEntity qc)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (qc != null)
                {
                    db.Delete(qc);
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public int GetQcTimes(string deptid)
        {
            var from = new DateTime(DateTime.Now.Year, 1, 1);
            var to = @from.AddYears(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            var query = from q in db.IQueryable<QcActivityEntity>()
                        where q.deptid == deptid && q.subjecttime >= @from && q.subjecttime < to
                        select q;
            return query.Count();
        }
    }
}
