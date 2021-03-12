using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.CarcOrCardManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.CarcOrCardManage;
using BSFramework.Application.Service.BaseManage;
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

namespace BSFramework.Application.Service.CarcOrCardManage
{
    /// <summary>
    /// carc 手袋卡
    /// </summary>
    public class CarcOrCardService : RepositoryFactory<CarcEntity>, ICarcOrCardService
    {
        #region 获取
        /// <summary>
        /// 列表分页 carc
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数
        /// { "state":"状态  0 保存 1 结束","type":"类型 carc  card ","deptid":" DeptCode StartsWith查询 无根据userid获取","starttime":"开始时间","endtime":"结束时间" }
        /// </param>
        /// <param name="userid">userid</param>
        /// <returns></returns>
        public List<CarcEntity> GetPageList(Pagination pagination, string queryJson, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var Expression = LinqExtensions.True<CarcEntity>();
            var user = db.FindEntity<UserEntity>(userid);
            if (user.UserId == "System")
            {
                user.DepartmentCode = "0";
            }
            //类型  
            if (!queryParam["type"].IsEmpty())
            {
                var type = queryParam["type"].ToString();
                Expression = Expression.And(x => x.DataType == type);
            }
            //类型  
            if (!queryParam["my"].IsEmpty())
            {
                var my = queryParam["my"].ToString();
                if (my == "1")
                {
                    Expression = Expression.And(x => x.TutelagePersonId.Contains(userid) || x.OperationPersonId.Contains(userid));
                }
            }
            //状态  0 保存 1 结束
            if (!queryParam["state"].IsEmpty())
            {
                var state = Convert.ToInt32(queryParam["state"]);
                Expression = Expression.And(x => x.State == state);
            }
            //模糊查询 任务名称
            if (!queryParam["workname"].IsEmpty())
            {
                var workname = queryParam["workname"].ToString();
                Expression = Expression.And(x => x.WorkName.Contains(workname));
            }
            //开始时间
            if (!queryParam["starttime"].IsEmpty())
            {
                var starttime = Convert.ToDateTime(queryParam["starttime"].ToString());
                Expression = Expression.And(x => x.CreateDate >= starttime);
            }
            //结束时间
            if (!queryParam["endtime"].IsEmpty())
            {
                var endtime = Convert.ToDateTime(queryParam["endtime"].ToString()).AddDays(1).AddMinutes(-1);
                Expression = Expression.And(x => x.CreateDate <= endtime);
            }
            //DeptCode StartsWith查询
            if (!queryParam["deptid"].IsEmpty())
            {
                var deptid = queryParam["deptid"].ToString();
                var dept = db.FindEntity<DepartmentEntity>(deptid);
                Expression = Expression.And(x => x.DeptCode.StartsWith(dept.EnCode));
            }
            else
            {
                Expression = Expression.And(x => x.DeptCode.StartsWith(user.DepartmentCode));
            }
            var entity = db.FindList(Expression).OrderByDescending(x => x.CreateDate);
            pagination.records = entity.Count();
            var entityList = entity.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            return entityList.ToList();
        }
        /// <summary>
        /// 列表分页 card
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数
        /// {"deptid":" DeptCode StartsWith查询 无根据userid获取" }
        /// </param>
        /// <param name="userid">userid</param>
        /// <returns></returns>
        public List<CCardEntity> GetCPageList(Pagination pagination, string queryJson, string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var queryParam = queryJson.ToJObject();
            var Expression = LinqExtensions.True<CCardEntity>();
            var user = db.FindEntity<UserEntity>(userid);
            if (user.UserId == "System")
            {
                user.DepartmentCode = "0";
            }

            //模糊查询 任务名称
            if (!queryParam["workname"].IsEmpty())
            {
                var workname = queryParam["workname"].ToString();
                Expression = Expression.And(x => x.WorkName.Contains(workname));
            }

            //DeptCode StartsWith查询
            if (!queryParam["deptid"].IsEmpty())
            {
                var deptid = queryParam["deptid"].ToString();
                var dept = db.FindEntity<DepartmentEntity>(deptid);
                Expression = Expression.And(x => x.DeptCode.StartsWith(dept.EnCode));
            }
            //else
            //{
            //    Expression = Expression.And(x => x.DeptCode.StartsWith(user.DepartmentCode));
            //}
            var entity = db.FindList(Expression).OrderByDescending(x => x.CreateDate);
            pagination.records = entity.Count();
            var entityList = entity.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
            return entityList.ToList();
        }
        /// <summary>
        /// 列表分页 班会获取
        /// </summary>
        /// <returns></returns>
        public List<CarcEntity> GetPageList(string keyvalue, int pagesize, int page, out int total)
        {

            IRepository db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<CarcEntity>(x => x.MeetId == keyvalue);
            total = query.Count();
            query = query.OrderByDescending(x => x.CreateDate);
            var resutl = query.Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            resutl.ForEach(x =>
            {
                x.CDangerousList = db.IQueryable<CDangerousEntity>(y => y.Cid == x.Id).ToList();
                foreach (var item in x.CDangerousList)
                {
                    item.Measure = db.IQueryable<CMeasureEntity>(y => y.Cmid == item.Id).ToList();
                }
                x.Files = db.IQueryable<FileInfoEntity>(y => y.RecId == x.Id).ToList();
                x.Evaluates = db.IQueryable<ActivityEvaluateEntity>(y => y.Activityid == x.Id).ToList();
            });
            return resutl;
        }

        /// <summary>
        /// 获取详情carc
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public CarcEntity GetDetail(string keyvalue)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var Entity = db.FindEntity<CarcEntity>(keyvalue);
            Entity.CDangerousList = db.IQueryable<CDangerousEntity>(y => y.Cid == keyvalue).ToList();
            foreach (var item in Entity.CDangerousList)
            {
                item.Measure = db.IQueryable<CMeasureEntity>(y => y.Cmid == item.Id).ToList();
            }
            Entity.Files = db.IQueryable<FileInfoEntity>(y => y.RecId == keyvalue).ToList();
            Entity.Evaluates = db.IQueryable<ActivityEvaluateEntity>(y => y.Activityid == keyvalue).ToList();
            return Entity;
        }
        /// <summary>
        /// 获取详情 card
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public CCardEntity GetCDetail(string keyvalue)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var Entity = db.FindEntity<CCardEntity>(keyvalue);
            Entity.CDangerousList = db.IQueryable<CDangerousEntity>(y => y.Cid == keyvalue).ToList();
            foreach (var item in Entity.CDangerousList)
            {
                item.Measure = db.IQueryable<CMeasureEntity>(y => y.Cmid == item.Id).ToList();
            }
            return Entity;
        }

        /// <summary>
        /// 获取模糊查询详情List card
        /// </summary>
        /// <param name="VagueName"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<CCardEntity> GetVagueList(string VagueName, string userid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var Expression = LinqExtensions.True<CCardEntity>();
            var user = db.FindEntity<UserEntity>(userid);
            if (user.UserId == "System")
            {
                user.DepartmentCode = "0";
            }
            Expression = Expression.And(x => x.DeptCode.StartsWith(user.DepartmentCode));
            if (!string.IsNullOrEmpty(VagueName))
            {
                Expression = Expression.And(x => x.WorkName.Contains(VagueName));

            }
            var entity = db.FindList(Expression).OrderByDescending(x => x.CreateDate);
            foreach (var item in entity)
            {
                item.CDangerousList = db.FindList<CDangerousEntity>(x => x.Cid == item.Id).ToList();
                foreach (var Measure in item.CDangerousList)
                {
                    Measure.Measure = db.FindList<CMeasureEntity>(x => x.Cmid == Measure.Id).ToList();
                }
            }
            return entity.ToList();
        }


        /// <summary>
        /// 检索  所有风险因素
        /// </summary>
        /// <param name="VagueName"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<CDangerousEntity> getDanger(string VagueName, int rows, int page, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var Expression = LinqExtensions.True<CDangerousEntity>();
            if (!string.IsNullOrEmpty(VagueName))
            {
                Expression = Expression.And(x => x.DangerSource.Contains(VagueName));
            }
            else
            {
                total = 0;
                return new List<CDangerousEntity>();
            }
            var entity = db.FindList(Expression);
            entity = entity.GroupBy(p => new { p.DangerSource }).Select(g => g.First()).ToList();
            total = entity.Count();
            entity = entity.Skip(rows * (page - 1)).Take(rows).ToList();
            foreach (var item in entity)
            {
                item.Measure = db.FindList<CMeasureEntity>(x => x.Cmid == item.Id).ToList();
            }
            return entity.ToList();
        }
        /// <summary>
        /// 检索  所有措施
        /// </summary>
        /// <param name="VagueName"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<CMeasureEntity> getMeasure(string VagueName, int rows, int page, out int total)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var Expression = LinqExtensions.True<CMeasureEntity>();
            if (!string.IsNullOrEmpty(VagueName))
            {
                Expression = Expression.And(x => x.Measure.Contains(VagueName));
            }
            else
            {
                total = 0;
                return new List<CMeasureEntity>();
            }
            var entity = db.FindList(Expression);
            entity = entity.GroupBy(p => new { p.Measure }).Select(g => g.First()).ToList();
            total = entity.Count();
            entity = entity.Skip(rows * (page - 1)).Take(rows).ToList();
            return entity.ToList();
        }

        #endregion
        #region 操作
        /// <summary>
        /// 新增 修改 carc
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void SaveForm(List<CarcEntity> entity, string userid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var user = db.FindEntity<UserEntity>(userid);

                foreach (var item in entity)
                {

                    var oldentity = db.FindEntity<CarcEntity>(item.Id);
                    if (oldentity != null)
                    {
                        oldentity.WorkName = item.WorkName;
                        oldentity.WorkArea = item.WorkArea;
                        oldentity.WorkType = item.WorkType;
                        oldentity.StartTime = item.StartTime;
                        oldentity.EndTime = item.EndTime;
                        oldentity.DangerWork = item.DangerWork;
                        oldentity.LockedUp = item.LockedUp;
                        oldentity.SafeLocation = item.SafeLocation;
                        oldentity.SafeCommunication = item.SafeCommunication;
                        oldentity.OperationProgram = item.OperationProgram;
                        oldentity.TutelagePerson = item.TutelagePerson;
                        oldentity.TutelagePersonId = item.TutelagePersonId;
                        oldentity.OperationPerson = item.OperationPerson;
                        oldentity.OperationPersonId = item.OperationPersonId;
                        oldentity.MainOperation = item.MainOperation;
                        oldentity.MeetId = item.MeetId;
                        oldentity.State = item.State;
                        oldentity.ModifyDate = DateTime.Now;
                        oldentity.ModifyUserName = user.RealName;
                        oldentity.ModifyUserId = user.UserId;
                        //风险评估
                        var OldCDangerousList = db.FindList<CDangerousEntity>(x => x.Cid == item.Id);
                        //修改 新增项
                        foreach (var CDangerous in item.CDangerousList)
                        {
                            var OldCDangerous = OldCDangerousList.FirstOrDefault(x => x.Id == CDangerous.Id);
                            if (OldCDangerous != null)
                            {
                                OldCDangerous.DangerName = CDangerous.DangerName;
                                OldCDangerous.DangerSource = CDangerous.DangerSource;
                                //OldCDangerous.Measure = CDangerous.Measure;
                                OldCDangerous.IsSafe = CDangerous.IsSafe;

                                //修改 新增项
                                var OldCmeasureList = db.FindList<CMeasureEntity>(x => x.Cmid == OldCDangerous.Id);
                                foreach (var Cmeasure in CDangerous.Measure)
                                {
                                    var OldCmeasure = OldCmeasureList.FirstOrDefault(x => x.Id == Cmeasure.Id);
                                    if (OldCmeasure != null)
                                    {
                                        OldCmeasure.Measure = Cmeasure.Measure;
                                        db.Update(OldCmeasure);
                                    }
                                    else
                                    {


                                        //measureList  新增项
                                        Cmeasure.Id = Guid.NewGuid().ToString();
                                        Cmeasure.CreateDate = DateTime.Now;
                                        Cmeasure.Cmid = CDangerous.Id;
                                        db.Insert(Cmeasure);


                                    }
                                    //measureList  删除项
                                    foreach (var old in OldCmeasureList)
                                    {
                                        var ck = CDangerous.Measure.FirstOrDefault(x => x.Id == old.Id);
                                        if (ck == null)
                                        {
                                            db.Delete(old);
                                        }
                                    }
                                }
                                OldCDangerous.Measure = null;
                                db.Update(OldCDangerous);

                            }
                            else
                            {

                                //CDangerousList 新增项
                                CDangerous.Id = Guid.NewGuid().ToString();
                                CDangerous.Cid = item.Id;
                                CDangerous.CreateDate = DateTime.Now;
                                foreach (var Cmeasure in CDangerous.Measure)
                                {
                                    //measureList  新增项
                                    Cmeasure.Id = Guid.NewGuid().ToString();
                                    Cmeasure.CreateDate = DateTime.Now;
                                    Cmeasure.Cmid = CDangerous.Id;
                                    db.Insert(Cmeasure);
                                }
                                db.Insert(CDangerous);


                            }
                            //CDangerousList  删除项
                            foreach (var Old in OldCDangerousList)
                            {
                                var ck = item.CDangerousList.FirstOrDefault(x => x.Id == Old.Id);
                                if (ck == null)
                                {
                                    db.Delete(Old);
                                }

                            }

                        }
                        if (item.CDangerousList.Count == 0)
                        {
                            //CDangerousList  删除项
                            foreach (var Old in OldCDangerousList)
                            {
                                db.Delete(Old);
                            }

                        }

                        db.Update(oldentity);
                    }
                    else
                    {
                        //班会新增是否跨天存在
                        if (!string.IsNullOrEmpty(item.MeetId))
                        {
                            if (item.StartTime.HasValue && item.EndTime.HasValue)
                            {
                                var ck = db.FindEntity<CarcEntity>(x => x.WorkName == item.WorkName & x.StartTime.Value == item.StartTime & x.EndTime.Value == item.EndTime && x.DeptId == item.DeptId);
                                if (ck != null)
                                {
                                    continue;
                                }
                            }

                        }

                        if (string.IsNullOrEmpty(item.Id))
                        {
                            item.Id = Guid.NewGuid().ToString();
                        }
                        var dept = new DepartmentService();
                        var deptEntity = dept.GetEntity(user.DepartmentId);
                        if (user.UserId == "System")
                        {
                            item.DeptCode = "0";
                            item.DeptId = "0";
                            item.DeptName = "超级管理员";
                        }
                        else
                        {
                            item.DeptCode = deptEntity.EnCode;
                            item.DeptId = user.DepartmentId;
                            item.DeptName = deptEntity.FullName;
                        }
                        item.ModifyDate = DateTime.Now;
                        item.ModifyUserName = user.RealName;
                        item.ModifyUserId = user.UserId;
                        item.CreateDate = DateTime.Now;
                        item.CreateUserName = user.RealName;
                        item.CreateUserId = user.UserId;
                        item.State = 0;
                        if (item.CDangerousList != null)
                        {
                            foreach (var CDangerous in item.CDangerousList)
                            {

                                CDangerous.Id = Guid.NewGuid().ToString();
                                CDangerous.CreateDate = DateTime.Now;
                                CDangerous.Cid = item.Id;
                                foreach (var Cmeasure in CDangerous.Measure)
                                {
                                    Cmeasure.Id = Guid.NewGuid().ToString();
                                    Cmeasure.CreateDate = DateTime.Now;
                                    Cmeasure.Cmid = CDangerous.Id;
                                    db.Insert(Cmeasure);
                                }
                                CDangerous.Measure = null;
                                db.Insert(CDangerous);
                            }

                        }
                        item.CDangerousList = null;
                        db.Insert(item);
                    }
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        public void deleteEntity(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<CarcEntity>(keyvalue);
                if (entity != null)
                {
                    db.Delete(entity);
                }
                var CDangerousList = db.IQueryable<CDangerousEntity>(y => y.Cid == keyvalue).ToList();
                if (CDangerousList != null)
                {
                    foreach (var item in CDangerousList)
                    {
                        var CMeasureList = db.IQueryable<CMeasureEntity>(y => y.Cmid == item.Id).ToList();
                        if (CMeasureList != null)
                        {
                            db.Delete(CMeasureList);
                        }
                    }
                    db.Delete(CDangerousList);
                }

                var Files = db.IQueryable<FileInfoEntity>(y => y.RecId == keyvalue).ToList();
                if (Files != null)
                {
                    db.Delete(Files);
                }
                var Evaluates = db.IQueryable<ActivityEvaluateEntity>(y => y.Activityid == keyvalue).ToList();
                if (Evaluates != null)
                {
                    db.Delete(Evaluates);
                }
                db.Commit();

            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary> 
        /// 新增 修改 card
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userid"></param>
        public void CSaveForm(List<CCardEntity> entity, string userid)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var user = db.FindEntity<UserEntity>(userid);

                foreach (var item in entity)
                {

                    var oldentity = db.FindEntity<CCardEntity>(item.Id);
                    if (oldentity != null)
                    {
                        oldentity.WorkName = item.WorkName;
                        oldentity.WorkArea = item.WorkArea;
                        oldentity.MainOperation = item.MainOperation;
                        oldentity.DutyId = item.DutyId;
                        oldentity.DutyName = item.DutyName;
                        oldentity.ModifyDate = DateTime.Now;
                        oldentity.ModifyUserName = user.RealName;
                        oldentity.ModifyUserId = user.UserId;
                        //风险评估
                        var OldCDangerousList = db.FindList<CDangerousEntity>(x => x.Cid == item.Id);
                        //修改 新增项
                        foreach (var CDangerous in item.CDangerousList)
                        {
                            var OldCDangerous = OldCDangerousList.FirstOrDefault(x => x.Id == CDangerous.Id);
                            if (OldCDangerous != null)
                            {
                                OldCDangerous.DangerName = CDangerous.DangerName;
                                OldCDangerous.DangerSource = CDangerous.DangerSource;
                                //OldCDangerous.Measure = CDangerous.Measure;
                                OldCDangerous.IsSafe = CDangerous.IsSafe;

                                //修改 新增项
                                var OldCmeasureList = db.FindList<CMeasureEntity>(x => x.Cmid == OldCDangerous.Id);
                                foreach (var Cmeasure in CDangerous.Measure)
                                {
                                    var OldCmeasure = OldCmeasureList.FirstOrDefault(x => x.Id == Cmeasure.Id);
                                    if (OldCmeasure != null)
                                    {
                                        OldCmeasure.Measure = Cmeasure.Measure;
                                        db.Update(OldCmeasure);
                                    }
                                    else
                                    {


                                        //measureList  新增项
                                        Cmeasure.Id = Guid.NewGuid().ToString();
                                        Cmeasure.CreateDate = DateTime.Now;
                                        Cmeasure.Cmid = CDangerous.Id;
                                        db.Insert(Cmeasure);


                                    }
                                    //measureList  删除项
                                    foreach (var old in OldCmeasureList)
                                    {
                                        var ck = CDangerous.Measure.FirstOrDefault(x => x.Id == old.Id);
                                        if (ck == null)
                                        {
                                            db.Delete(old);
                                        }
                                    }
                                }
                                OldCDangerous.Measure = null;
                                db.Update(OldCDangerous);

                            }
                            else
                            {

                                //CDangerousList 新增项
                                CDangerous.Id = Guid.NewGuid().ToString();
                                CDangerous.Cid = item.Id;
                                CDangerous.CreateDate = DateTime.Now;
                                foreach (var Cmeasure in CDangerous.Measure)
                                {
                                    //measureList  新增项
                                    Cmeasure.Id = Guid.NewGuid().ToString();
                                    Cmeasure.CreateDate = DateTime.Now;
                                    Cmeasure.Cmid = CDangerous.Id;
                                    db.Insert(Cmeasure);
                                }
                                db.Insert(CDangerous);


                            }
                            //CDangerousList  删除项
                            foreach (var Old in OldCDangerousList)
                            {
                                var ck = item.CDangerousList.FirstOrDefault(x => x.Id == Old.Id);
                                if (ck == null)
                                {
                                    db.Delete(Old);
                                }

                            }

                        }
                        if (item.CDangerousList.Count == 0)
                        {
                            //CDangerousList  删除项
                            foreach (var Old in OldCDangerousList)
                            {
                                db.Delete(Old);
                            }

                        }
                        db.Update(oldentity);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.Id))
                        {
                            item.Id = Guid.NewGuid().ToString();

                        }
                        var dept = new DepartmentService();
                        var deptEntity = dept.GetEntity(item.DeptId);
                        item.DeptCode = deptEntity.EnCode;
                        item.DeptId = user.DepartmentId;
                        item.DeptName = deptEntity.FullName;
                        item.ModifyDate = DateTime.Now;
                        item.ModifyUserName = user.RealName;
                        item.ModifyUserId = user.UserId;
                        item.CreateDate = DateTime.Now;
                        item.CreateUserName = user.RealName;
                        item.CreateUserId = user.UserId;
                        if (item.CDangerousList != null)
                        {
                            foreach (var CDangerous in item.CDangerousList)
                            {

                                CDangerous.Id = Guid.NewGuid().ToString();
                                CDangerous.CreateDate = DateTime.Now;
                                CDangerous.Cid = item.Id;
                                foreach (var Cmeasure in CDangerous.Measure)
                                {
                                    Cmeasure.Id = Guid.NewGuid().ToString();
                                    Cmeasure.CreateDate = DateTime.Now;
                                    Cmeasure.Cmid = CDangerous.Id;
                                    db.Insert(Cmeasure);
                                }
                                CDangerous.Measure = null;
                                db.Insert(CDangerous);
                            }
                        }
                        item.CDangerousList = null;
                        db.Insert(item);

                    }
                }
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        public void deleteCEntity(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<CCardEntity>(keyvalue);
                if (entity != null)
                {
                    db.Delete(entity);
                }
                var CDangerousList = db.IQueryable<CDangerousEntity>(y => y.Cid == keyvalue).ToList();
                if (CDangerousList != null)
                {
                    foreach (var item in CDangerousList)
                    {
                        var CMeasureList = db.IQueryable<CMeasureEntity>(y => y.Cmid == item.Id).ToList();
                        if (CMeasureList != null)
                        {
                            db.Delete(CMeasureList);
                        }
                    }
                    db.Delete(CDangerousList);
                }


                db.Commit();

            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }
        ///// <summary>
        ///// 新增 修改
        ///// </summary>
        //public void SaveFormCMeasure(CMeasureEntity Cmeasure)
        //{
        //    var db = new RepositoryFactory().BaseRepository().BeginTrans();
        //    try
        //    {

        //        var OldCmeasure = db.FindEntity<CMeasureEntity>(Cmeasure.Id);
        //        if (OldCmeasure != null)
        //        {
        //            OldCmeasure.DangerName = Cmeasure.DangerName;
        //            OldCmeasure.DangerSource = Cmeasure.DangerSource;
        //            OldCmeasure.Measure = Cmeasure.Measure;
        //            OldCmeasure.IsSafe = Cmeasure.IsSafe;
        //            db.Update(OldCmeasure);
        //        }
        //        else
        //        {

        //            if (string.IsNullOrEmpty(Cmeasure.Id))
        //            {
        //                Cmeasure.Id = Guid.NewGuid().ToString();
        //            }
        //            db.Insert(Cmeasure);

        //        }
        //        db.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        db.Rollback();
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// 删除
        ///// </summary>
        //public void deleteCMeasureEntity(string keyvalue)
        //{
        //    var db = new RepositoryFactory().BaseRepository().BeginTrans();
        //    try
        //    {
        //        var entity = db.FindEntity<CMeasureEntity>(keyvalue);
        //        if (entity != null)
        //        {
        //            db.Delete(entity);
        //        }
        //        db.Commit();

        //    }
        //    catch (Exception ex)
        //    {
        //        db.Rollback();
        //        throw;
        //    }

        //}

        #endregion
    }
}
