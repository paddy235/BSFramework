using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Data.Repository;
using BSFramework.Application.IService.InnovationManage;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using BSFramework.Application.Service.BaseManage;
using System.ServiceModel;
using Bst.ServiceContract.MessageQueue;

namespace BSFramework.Application.Service.InnovationManage
{
    /// <summary>
    /// 合理化建议
    /// </summary>
    public class AdviceService : RepositoryFactory<AdviceEntity>, IAdviceService
    {
        private DepartmentService deptService = new DepartmentService();

        /// <summary>
        /// 获取Advice数据
        /// </summary>
        /// <returns></returns>
        public List<AdviceEntity> getAdviceList(Dictionary<string, string> keyValue, Pagination pagination)
        {

            try
            {
                var db = new RepositoryFactory().BaseRepository();
                var expression = LinqExtensions.True<AdviceEntity>();
                //状态
                if (keyValue.ContainsKey("state"))
                {
                    if (!string.IsNullOrEmpty(keyValue["state"]))
                    {
                        var val = keyValue["state"];
                        expression = expression.And(x => (x.aduitstate == val || x.aduitresult == val));
                    }
                }
                //名称
                if (keyValue.ContainsKey("name"))
                {
                    if (!string.IsNullOrEmpty(keyValue["name"]))
                    {
                        var val = keyValue["name"];
                        expression = expression.And(x => x.title.Contains(val));
                    }
                }
                //类型
                if (keyValue.ContainsKey("advicetype"))
                {
                    if (!string.IsNullOrEmpty(keyValue["advicetype"]))
                    {
                        var val = keyValue["advicetype"];
                        expression = expression.And(x => x.advicetype == val);
                    }
                }

                //用户
                if (keyValue.ContainsKey("userid"))
                {
                    if (!string.IsNullOrEmpty(keyValue["userid"]))
                    {
                        var val = keyValue["userid"];

                        expression = expression.And(x => x.userid == val);

                    }
                }
                var data = db.IQueryable(expression).ToList();
                var Resultdata = new List<AdviceEntity>();
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
                                if (!string.IsNullOrEmpty(items.deptid))
                                {
                                    if (items.deptid.Contains(item.DepartmentId))
                                    {
                                        if (Resultdata.IndexOf(items) < 0)
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
                    Resultdata = Resultdata.OrderByDescending(x => x.reporttime).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
                }
                return Resultdata;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 获取管理数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<AdviceEntity> getAdvicebyuser(string userid)
        {

            try
            {
                var db = new RepositoryFactory().BaseRepository();
                return db.IQueryable<AdviceEntity>(x => x.userid == userid).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 获取用户数据
        /// </summary>
        /// <param name="Strid"></param>
        /// <returns></returns>
        public List<AdviceEntity> getAdvicebyid(string Strid)
        {

            try
            {
                var db = new RepositoryFactory().BaseRepository();
                return db.IQueryable<AdviceEntity>(x => Strid.Contains(x.adviceid)).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 数据操作
        /// </summary>
        /// <param name="add"></param>
        /// <param name="update"></param>
        /// <param name="del"></param>
        /// <param name="audit"></param>
        public void Operation(AdviceEntity add, AdviceEntity update, string del, AdviceAuditEntity audit, AdviceAuditEntity auditupdate)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans(); ;
            try
            {
                if (audit != null)
                {

                    db.Insert(audit);

                }
                if (add != null)
                {
                    db.Insert(add);
                }
                if (update != null)
                {
                    db.Update(update);
                }
                if (del != null)
                {
                    var getOne = db.IQueryable<AdviceEntity>(x => x.adviceid == del).ToList();
                    var List = getAuditByid(del);
                    db.Delete(List);
                    db.Delete(getOne);
                }

                if (auditupdate != null)
                {
                    //修改时无新增
                    if (audit == null && update == null)
                    {
                        var getOne = db.IQueryable<AdviceEntity>(x => x.adviceid == auditupdate.adviceid).ToList();
                        var officeEntity = getOne[0];
                        officeEntity.aduitresult = auditupdate.state;
                        officeEntity.aduitstate = "已审核";
                        db.Update(officeEntity);
                    }
                    db.Update(auditupdate);
                }
                db.Commit();
                ////发消息
                //if (add != null)
                //{
                //    SendMsg(add.adviceid);
                //} else if (audit != null)
                //{
                //    messagebll.FinishTodo("技术问答答题", eduId);
                //    SendMsg(audit.adviceid);
                //} else if (auditupdate !=null)
                //{
                //    SendMsg(auditupdate.adviceid);
                //}
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 获取审核记录
        /// </summary>
        /// <param name="adviceid"></param>
        /// <returns></returns>
        public List<AdviceAuditEntity> getAuditByid(string adviceid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<AdviceAuditEntity>(x => x.adviceid == adviceid).ToList();
        }

        /// <summary>
        /// 获取审核记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<AdviceAuditEntity> getAuditByuser(string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<AdviceAuditEntity>(x => x.userid == userid).ToList();
        }

        /// <summary>
        /// 获取审核记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<AdviceAuditEntity> getAuditId(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<AdviceAuditEntity>(x => x.auditid == id).ToList();
        }

        public int GetSuggestions(string deptid)
        {
            var from = new DateTime(DateTime.Now.Year, 1, 1);
            var to = @from.AddYears(1).AddMinutes(-1);
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<AdviceEntity>()
                        where q.deptid == deptid && q.aduitresult == "审核通过" && q.reporttime > @from && q.reporttime < to
                        select q;
            return query.Count();
        }
        /// <summary>
        /// 查询待办
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetTodoCount(string userId)
        {
            var db = new RepositoryFactory().BaseRepository();
           
            var query = from q in db.IQueryable<AdviceAuditEntity>()
                        join p in db.IQueryable<AdviceEntity>() on q.adviceid equals p.adviceid into into1
                        from t in into1.DefaultIfEmpty()
                        where q.userid == userId && t.aduitresult == "待审核"
                        select t;
            var count = query.Count();
            //var sqlStr = "SELECT COUNT(*) FROM wg_adviceaudit a left join wg_advice b on a.adviceid=b.adviceid WHERE b.aduitresult='待审核' and a.USERID='" + userId + "'";
            //object count = db.FindObject(sqlStr);
            //return count == null ? 0 : Convert.ToInt32(count);
            return count;
        }

        #region 
        //private void SendMsg(string adviceid)
        //{
        //    //发送消息
        //    using (var factory = new ChannelFactory<IMsgService>("message"))
        //    {
        //        var proxy = factory.CreateChannel();
        //        proxy.Send("2469e12a-e5ed-4a38-b9ac-16776ace7313", adviceid);
        //    }
        //}

        #endregion
    }
}
