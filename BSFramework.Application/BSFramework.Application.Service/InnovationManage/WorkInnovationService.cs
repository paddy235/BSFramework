using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Application.IService.InnovationManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BSFramework.Application.Service.InnovationManage
{
    /// <summary>
    /// 班组管理创新成果
    /// </summary>
    public class WorkInnovationService : RepositoryFactory<WorkInnovationEntity>, IWorkInnovationService
    {
        private DepartmentService deptService = new DepartmentService();


        #region 首页查询
        /// <summary>
        /// 获取Advice数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkInnovationEntity> getList(string queryJson, Pagination pagination)
        {
            var db = new RepositoryFactory().BaseRepository();
            try
            {
                var expression = LinqExtensions.True<WorkInnovationEntity>();

                var queryParam = queryJson.ToJObject();
                if (!queryParam["name"].IsEmpty())
                {
                    var name = queryParam["name"].ToString();
                    expression = expression.And(x => x.name.Contains(name));
                }
                if (!queryParam["deptid"].IsEmpty())
                {
                    var deptlist = deptService.GetSubDepartments(queryParam["deptid"].ToString(), "厂级,部门,班组");
                    if (deptlist.Count > 1)
                    {
                        var deptStr = string.Join(",", deptlist.Select(x => x.DepartmentId).ToList());
                        expression = expression.And(x => deptStr.Contains(x.deptid));

                    }
                    else
                    {
                        var deptStr = string.Join(",", deptlist.Select(x => x.DepartmentId).ToList());
                        expression = expression.And(x => deptStr.Contains(x.deptid));
                    }
                }

                if (!queryParam["aduitstate"].IsEmpty())
                {
                    var aduitstate = queryParam["aduitstate"].ToString();
                    expression = expression.And(x => x.aduitstate==aduitstate);
                }

                if (!queryParam["aduitresult"].IsEmpty())
                {
                    var aduitresult = queryParam["aduitresult"].ToString();
                    expression = expression.And(x => x.aduitresult == aduitresult);

                }
                if (!queryParam["start"].IsEmpty())
                {
                    var start = Convert.ToDateTime(queryParam["start"]);
                    expression = expression.And(x => x.reporttime >= start);
                }
                if (!queryParam["end"].IsEmpty())
                {
                    var end = Convert.ToDateTime(queryParam["end"]);
                    expression = expression.And(x => x.reporttime <= end);
                }
                if (!queryParam["stateout"].IsEmpty())
                {
                    var stateout = queryParam["stateout"].ToString();
                    expression = expression.And(x => x.aduitstate == stateout);
                }
                var data = db.IQueryable(expression);
                pagination.records = data.Count();
                return  data.OrderByDescending(x=>x.reporttime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();

                //var strSql = new StringBuilder();
                //var parameter = new List<DbParameter>();
                //strSql.Append(@"SELECT  r.innovationid,
                //                    r.name ,
                //                    r.deptname ,
                //                    r.deptid ,
                //                    r.statusquo ,
                //                    r.proposed,
                //                    r.reporttime,
                //                    r.reportuser,
                //                    r.reportuserid,
                //                    r.aduitstate,
                //                    r.aduitresult
                //            FROM    wg_workinnovation r
                //            WHERE   1 = 1 ");
                //if (!queryParam["name"].IsEmpty())
                //{
                //    strSql.Append(" AND r.name  like '%" + queryParam["name"].ToString() + "%' ");
                //    //parameter.Add(DbParameters.CreateDbParameter("@name",));
                //}
                //if (!queryParam["deptid"].IsEmpty())
                //{
                //    var deptlist = deptService.GetSubDepartments(queryParam["deptid"].ToString(), "厂级,部门,班组");
                //    if (deptlist.Count > 1)
                //    {
                //        strSql.Append(" AND ( r.deptid in (");
                //        for (int i = 0; i < deptlist.Count; i++)
                //        {
                //            string key = "@deptid" + i;
                //            string val = deptlist[i].DepartmentId;
                //            strSql.Append(key);
                //            if (i != (deptlist.Count - 1))
                //            {
                //                strSql.Append(",");
                //            }
                //            parameter.Add(DbParameters.CreateDbParameter(key, val));
                //        }
                //        strSql.Append("  ))");
                //    }
                //    else
                //    {
                //        strSql.Append(" AND r.deptid = @deptid ");
                //        parameter.Add(DbParameters.CreateDbParameter("@deptid", queryParam["deptid"].ToString()));
                //    }
                //}

                //if (!queryParam["aduitstate"].IsEmpty())
                //{
                //    strSql.Append(" AND r.aduitstate = @aduitstate ");
                //    parameter.Add(DbParameters.CreateDbParameter("@aduitstate", queryParam["aduitstate"].ToString()));
                //}

                //if (!queryParam["aduitresult"].IsEmpty())
                //{
                //    strSql.Append(" AND r.aduitresult = @aduitresult ");
                //    parameter.Add(DbParameters.CreateDbParameter("@aduitresult", queryParam["aduitresult"].ToString()));
                //}
                //if (!queryParam["start"].IsEmpty())
                //{
                //    strSql.Append(" AND r.reporttime >= @start ");
                //    parameter.Add(DbParameters.CreateDbParameter("@start", Convert.ToDateTime(queryParam["start"])));
                //}
                //if (!queryParam["end"].IsEmpty())
                //{
                //    strSql.Append(" AND r.reporttime <= @end ");
                //    parameter.Add(DbParameters.CreateDbParameter("@end", Convert.ToDateTime(queryParam["end"])));
                //}
                //if (!queryParam["stateout"].IsEmpty())
                //{
                //    strSql.Append(" AND r.aduitstate <> @stateout ");
                //    parameter.Add(DbParameters.CreateDbParameter("@stateout", queryParam["stateout"].ToString()));
                //}
                //pagination.sidx = "reporttime desc";
                //pagination.sord = "";
                // return db.FindList<WorkInnovationEntity>(strSql.ToString(), parameter.ToArray(), pagination);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region  步骤查询
        /// <summary>
        /// 根据用户id获取记录表数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<WorkInnovationEntity> getWorkInnovationbyuser(string userid)
        {

            try
            {
                var db = new RepositoryFactory().BaseRepository();
                return db.IQueryable<WorkInnovationEntity>(x => x.reportuserid == userid).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 根据主键id获取记录表数据
        /// </summary>
        /// <param name="Strid"></param>
        /// <returns></returns>
        public List<WorkInnovationEntity> getWorkInnovationbyid(string Strid)
        {

            try
            {
                var db = new RepositoryFactory().BaseRepository();
                return db.IQueryable<WorkInnovationEntity>(x => Strid.Contains(x.innovationid)).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 根据关联id获取审核数据
        /// </summary>
        /// <param name="innovationid"></param>
        /// <returns></returns>
        public List<WorkInnovationAuditEntity> getAuditByid(string innovationid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<WorkInnovationAuditEntity>(x => x.innovationid == innovationid).ToList();
        }

        /// <summary>
        /// 根据用户id获取审核数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<WorkInnovationAuditEntity> getAuditByuser(string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<WorkInnovationAuditEntity>(x => x.userid == userid).ToList();
        }

        /// <summary>
        /// 根据id获取审核数据
        /// </summary>
        /// <param name="auditid"></param>
        /// <returns></returns>
        public List<WorkInnovationAuditEntity> getAuditId(string auditid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<WorkInnovationAuditEntity>(x => x.auditid == auditid).ToList();
        }
        #endregion


        #region 数据操作
        /// <summary>
        /// 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="audit"></param>
        /// <param name="type">是否新增</param>
        public void Operation(WorkInnovationEntity main, List<WorkInnovationAuditEntity> audit, bool type)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                //是否新增
                if (type)
                {

                    if (main != null)
                    {
                        if (main.name.IsEmpty())
                        {

                            var getOne = db.IQueryable<WorkInnovationEntity>(x => x.innovationid == main.innovationid).ToList();
                            var List = getAuditByid(main.innovationid);
                            db.Delete(List);
                            db.Delete(getOne);
                        }
                        else
                        {
                            main.clearFile();
                            var dept = deptService.GetEntity(main.deptid);
                            if (dept.Nature == "班组")
                            {
                                var parent = deptService.GetEntity(dept.ParentId);
                                main.deptname = parent.FullName + "/" + dept.FullName;
                            }
                            else
                            {
                                main.deptname = dept.FullName;

                            }
                            db.Insert(main);
                        }
                    }
                    if (audit != null)
                    {

                        foreach (var item in audit)
                        {
                            item.sort = audit.IndexOf(item) + 1;
                        }
                        db.Insert(audit);
                    }
                }
                else
                {
                    //修改记录
                    if (main != null)
                    {
                        main.clearFile();
                        if (audit != null)
                        {
                            var oldaudit = getAuditByid(audit[0].innovationid);
                            int sort = oldaudit.Count + 1;
                            if (main.aduitresult == "退回")
                            {
                                main.aduitresult = "";
                                db.Delete(oldaudit);
                                sort = 1;
                            }
                            foreach (var item in audit)
                            {
                                var ck = oldaudit.FirstOrDefault(x => x.auditid == item.auditid);
                                if (ck == null)
                                {
                                    item.sort = sort;
                                    db.Insert(item);
                                    sort++;
                                }
                                else
                                {

                                    db.Update(item);
                                }
                                // var getSort = getAuditByid(item.innovationid);
                            }
                        }
                        var dept = deptService.GetEntity(main.deptid);
                        if (dept.Nature == "班组")
                        {
                            var parent = deptService.GetEntity(dept.ParentId);
                            main.deptname = parent.FullName + "/" + dept.FullName;
                        }
                        else
                        {
                            main.deptname = dept.FullName;

                        }
                        db.Update(main);
                    }
                    //审核操作
                    if (audit != null && main == null)
                    {
                        main = db.FindEntity<WorkInnovationEntity>(audit[0].innovationid);
                        if (main.aduitstate == "待提交")
                        {
                            foreach (var item in audit)
                            {
                                // var getSort = getAuditByid(item.innovationid);
                                item.sort = audit.IndexOf(item) + 1;
                            }
                            db.Insert(audit);
                        }
                        else
                        {
                            //审核结束 或者回退
                            if (audit.Count == 1)
                            {
                                var getSort = getAuditByid(audit[0].innovationid);
                                if (audit[0].state == "退回")
                                {

                                    main.returnnum = main.returnnum + 1;
                                    main.aduitstate = "已审核";
                                    main.aduitresult = audit[0].state;

                                    //清理多余数据
                                    foreach (var ck in getSort)
                                    {
                                        if (ck.state.IsEmpty() && ck.auditid != audit[0].auditid)
                                        {
                                            db.Delete(ck);
                                        }

                                    }
                                    db.Update(main);
                                    db.Update(audit);
                                }
                                else
                                {

                                    if (audit[0].isspecial)
                                    {
                                        main.aduitstate = "已审核";
                                        main.aduitresult = audit[0].state;
                                        db.Update(main);
                                    }
                                    db.Update(audit);
                                }

                            }
                            else
                            {


                                var getSort = getAuditByid(audit[0].innovationid);
                                var ckTh = audit.FirstOrDefault(x => x.state == "退回");
                                if (ckTh != null)
                                {
                                    var getOne = db.IQueryable<WorkInnovationEntity>(x => x.innovationid == main.innovationid).ToList();

                                    getOne[0].returnnum = main.returnnum + 1;
                                    getOne[0].aduitstate = "已审核";
                                    getOne[0].aduitresult = ckTh.state;

                                    //清理多余数据
                                    foreach (var ck in audit)
                                    {
                                        if (ck.state.IsEmpty() && ck.auditid != ckTh.auditid)
                                        {
                                            db.Delete(ck);
                                        }

                                    }
                                    db.Update(ckTh);
                                    db.Update(getOne[0]);
                                }
                                else
                                {
                                    foreach (var item in audit)
                                    {
                                        var ck = getSort.FirstOrDefault(x => x.auditid == item.auditid);
                                        if (ck == null)
                                        {
                                            item.sort = getSort.Count + 1;
                                            db.Insert(item);
                                        }
                                        else
                                        {
                                            db.Update(item);
                                        }
                                    }
                                }
                            }

                        }

                    }
                }

                #endregion


                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }
        }

        public int GetInnovation(string deptid)
        {
            var from = new DateTime(DateTime.Now.Year, 1, 1);
            var to = @from.AddYears(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<SevenSOfficeEntity>()
                        where q.deptid == deptid && q.aduitresult == "审核通过" && q.createdate >= @from && q.createdate <= to
                        select q;
            return query.Count();
        }
        /// <summary>
        /// 得到当前用户的待审核数量
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int GetTodoCount(string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<WorkInnovationEntity>()
                        join q1 in db.IQueryable<WorkInnovationAuditEntity>()
                        on q.innovationid equals q1.innovationid
                        where q.aduitstate == "待审核" && q1.userid == userid
                        select q;
            return query.Count();
        }

    }
}
