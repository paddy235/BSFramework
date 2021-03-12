using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.IService.SevenSManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.SevenSManage
{
    public class SevenSService : RepositoryFactory<SevenSEntity>, ISevenSService
    {
        private DepartmentService dpetservice = new DepartmentService();
        #region 技术规范
        public IEnumerable<SevenSEntity> GetList(string typename, string typeId, string name, string Id, int pageIndex, int pageSize, bool ispage, string deptid)
        {


            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(typeId))
            {
                query = query.Where(x => typeId.Contains(x.TypeId));
            }
            if (!string.IsNullOrEmpty(typename))
            {
                var db = new RepositoryFactory().BaseRepository();
                var typeList = db.IQueryable<SevenSTypeEntity>().FirstOrDefault(x => x.TypeName.Contains(typename));
                query = query.Where(x => x.TypeId == typeList.TypeId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            List<SevenSEntity> List = new List<SevenSEntity>();
            if (!string.IsNullOrEmpty(Id))
            {
                if (Id.Contains(","))
                {
                    var id = Id.Split(',');
                    for (int i = 0; i < id.Count(); i++)
                    {
                        var selectId = id[i];
                        var get = query.Where(x => x.ID == selectId).ToList();
                        List.AddRange(get);
                    }
                }
                else
                {
                    query = query.Where(x => x.ID == Id);
                    List = query.ToList();
                }
            }
            else
            {
                List = query.ToList();
            }
            if (List.Count > 0)
            {
                var dept = dpetservice.GetEntity(deptid);
                List<SevenSEntity> get = new List<SevenSEntity>();

                foreach (var item in List)
                {
                    if (item.BZId == "0")
                    {
                        get.Add(item);
                        continue;
                    }
                    var itemBz = dpetservice.GetEntity(item.BZId);
                    if (itemBz != null)
                    {
                        var parent = dpetservice.GetEntity(itemBz.ParentId);
                        if (dept.EnCode.StartsWith(parent.EnCode))
                        {
                            get.Add(item);
                        }
                    }

                }
                if (ispage)
                {
                    List = get.OrderByDescending(x => x.CreateDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    List = get.OrderByDescending(x => x.CreateDate).ToList();
                }
            }

            foreach (var item in List)
            {
                var db = new RepositoryFactory().BaseRepository();
                var typeList = db.IQueryable<FileInfoEntity>().FirstOrDefault(x => x.RecId == item.ID);
                var type = db.FindEntity<SevenSTypeEntity>(item.TypeId);
                item.TypeName = type.TypeName;
                item.FileId = typeList.FileId;
                item.FilePath = typeList.FilePath;

            }
            return List;
        }
        public IList<SevenSTypeEntity> GetIndex(string name, string typeid, string deptCode)
        {

            var db = new RepositoryFactory().BaseRepository();

            if (string.IsNullOrEmpty(typeid)) typeid = null;

            var node = from q in db.IQueryable<SevenSTypeEntity>()
                       where q.ParentCardId == null
                       select q;

            if (typeid != null)
            {
                node = from q in db.IQueryable<SevenSTypeEntity>()
                       where q.TypeId == typeid
                       select q;
            }

            var categories = node;

            var current = from q1 in node
                          join q2 in db.IQueryable<SevenSTypeEntity>() on q1.TypeId equals q2.ParentCardId
                          where q2.deptcode.StartsWith(deptCode)
                          select q2;

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<SevenSTypeEntity>() on q1.TypeId equals q2.ParentCardId
                          where q2.deptcode.StartsWith(deptCode)
                          select q2;

            }
            List<SevenSTypeEntity> entity = categories.ToList();
            var returnentity = new List<SevenSTypeEntity>();
            foreach (var item in entity)
            {
                var emergencyList = (from q1 in db.IQueryable<SevenSEntity>()
                                     where q1.TypeId == item.TypeId
                                     select q1).ToList();
                if (!string.IsNullOrEmpty(name))
                {
                    emergencyList = emergencyList.Where(x => x.Name.Contains(name)).ToList();
                }
                item.childList = emergencyList;
                if (string.IsNullOrEmpty(item.ParentCardId))
                {
                    continue;
                }
                returnentity.Add(item);
            }

            return returnentity;

        }
        public IList<SevenSEntity> GetItems(string key, string typeid, int pagesize, int page, string deptCode, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            if (string.IsNullOrEmpty(typeid)) typeid = null;

            var node = from q in db.IQueryable<SevenSTypeEntity>()
                       where q.ParentCardId == null
                       select q;

            if (typeid != null)
            {
                node = from q in db.IQueryable<SevenSTypeEntity>()
                       where q.TypeId == typeid
                       select q;
            }

            var categories = node;

            var current = from q1 in node
                          join q2 in db.IQueryable<SevenSTypeEntity>() on q1.TypeId equals q2.ParentCardId
                          select q2;

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<SevenSTypeEntity>() on q1.TypeId equals q2.ParentCardId
                          select q2;

            }

            var query = from q1 in db.IQueryable<SevenSEntity>()
                        join q2 in categories on q1.TypeId equals q2.TypeId
                        into t1
                        from tb1 in t1.DefaultIfEmpty()
                        join q3 in db.IQueryable<FileInfoEntity>() on q1.ID equals q3.RecId
                        into t2
                        from tb2 in t2.DefaultIfEmpty()
                        join q4 in db.IQueryable<DepartmentEntity>() on tb1.deptid equals q4.DepartmentId
                        where /*q4.EnCode.StartsWith(deptCode)*/ deptCode.Contains(q4.DepartmentId)
                        select new { tb2.FileId, q1.Path, tb2.FilePath, q1.ID, q1.Name, q1.TypeId, tb1.TypeName, q1.CreateDate, q1.CREATEUSERNAME, q1.MODIFYDATE, q1.MODIFYUSERNAME, q1.seenum };


            total = query.Count();
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(x => x.Name.Contains(key));
            }
            query = query.OrderBy(x => x.seenum).Skip(pagesize * (page - 1)).Take(pagesize);
            return query.ToList().Select(x => new SevenSEntity() { FileId = x.FileId, FilePath = x.FilePath, Path = x.Path, ID = x.ID, Name = x.Name, TypeName = x.TypeName, TypeId = x.TypeId, CREATEUSERNAME = x.MODIFYDATE < DateTime.Now.AddYears(-3) ? x.CREATEUSERNAME : x.MODIFYUSERNAME, CreateDate = x.MODIFYDATE < DateTime.Now.AddYears(-3) ? x.CreateDate : x.MODIFYDATE, seenum = x.seenum }).ToList();
        }
        public IList<SevenSTypeEntity> GetAllType(string deptCode)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<SevenSTypeEntity>(x => deptCode.StartsWith(x.deptcode)).OrderBy(x => x.CreateTime).ToList();
        }

        public void DeleteType(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var cnt = db.IQueryable<SevenSTypeEntity>(x => x.ParentCardId == id).Count();
                if (cnt > 0) throw new Exception("请先删除子类别");
                cnt = db.IQueryable<SevenSEntity>(x => x.TypeId == id).Count();
                if (cnt > 0) throw new Exception("请先删除内容");
                db.Delete<SevenSTypeEntity>(id);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        public void DeleteItem(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete<SevenSEntity>(id);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
            //var card = db.FindEntity<EmergencyEntity>(id);
            //var fileInfo = db.IQueryable<FileInfoEntity>().FirstOrDefault(x => x.RecId == card.ID);
            //db.Delete<FileInfoEntity>(fileInfo.FileId);

        }

        public SevenSEntity GetSevenSEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }


        public void AddType(SevenSTypeEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(model);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }


        }
        public void EditType(SevenSTypeEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<SevenSTypeEntity>(model.TypeId);
                var cnt = db.IQueryable<SevenSTypeEntity>(x => x.TypeName == model.TypeName).Count();
                if (cnt > 0) throw new Exception("请类别名称已经存在！");
                entity.TypeName = model.TypeName;
                db.Update(entity);
                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        public void SaveSevenSEntity(SevenSEntity entity)
        {

            this.BaseRepository().Update(entity);

        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, SevenSEntity entity)
        {
            var entity1 = this.GetSevenSEntity(keyValue);
            if (string.IsNullOrEmpty(keyValue))
            {
                this.BaseRepository().Insert(entity);
                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                entity1.Path = entity.Path;
                entity1.Name = entity.Name;

                //var user = OperatorProvider.Provider.Current();
                entity1.MODIFYDATE = entity.MODIFYDATE == null ? DateTime.Now : entity.MODIFYDATE;
                entity1.MODIFYUSERID = entity.MODIFYUSERID;
                entity1.MODIFYUSERNAME = entity.MODIFYUSERNAME;
                this.BaseRepository().Update(entity1);
            }
        }
        #endregion

        #region 定点照片
        /// <summary>
        /// 获取周期设置数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SevenSPictureCycleEntity> getCycle()
        {

            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<SevenSPictureCycleEntity>().OrderBy(x => x.sort);
        }
        /// <summary>
        /// 修正周期时间数据
        /// </summary>
        /// <returns></returns>
        public void setCycle(string value)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var one = db.IQueryable<SevenSPictureCycleEntity>().FirstOrDefault(x => x.iswork == true);
                var two = db.IQueryable<SevenSPictureCycleEntity>().FirstOrDefault(x => x.cycle == value);
                if (one.cycle == value)
                {
                    return;
                }
                //starttime 用于存储上一次周期结束时间
                one.iswork = false;
                two.iswork = true;
                two.starttime = one.starttime;
                one.starttime = null;
                db.Update(one);
                db.Update(two);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }

        }

        /// <summary>
        /// 修正周期时间数据
        /// </summary>
        /// <returns></returns>
        public void setCycleTime(string value)
        {


            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var list = db.IQueryable<SevenSPictureCycleEntity>();

                foreach (var item in list)
                {
                    item.starttime = value;
                }
                db.Update(list.ToList());
                db.Commit();

            }
            catch (Exception ex)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 获取地点设置数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SevenSPictureSetEntity> getSet()
        {

            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<SevenSPictureSetEntity>().OrderBy(x => x.createtime);
        }
        /// <summary>
        /// 获取地点设置数据包含当前记录id
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SevenSPictureSetEntity> getSetAndPicture(string deptId)
        {

            var db = new RepositoryFactory().BaseRepository();
            try
            {
                var spaceList = db.IQueryable<SevenSPictureSetEntity>().OrderBy(x => x.createtime);
                var picture = db.IQueryable<SevenSPictureEntity>().Where(x => x.deptid == deptId).OrderByDescending(x => x.CreateDate).ToList();
                if (picture.Count == 0)
                {
                    return new List<SevenSPictureSetEntity>();
                }
                var ID = picture[0].Id;
                if (picture[0].state == "已提交")
                {
                    return new List<SevenSPictureSetEntity>();
                }
                var Files = db.IQueryable<FileInfoEntity>().Where(x => x.RecId == ID).ToList();
                foreach (var item in spaceList)
                {
                    //获取当前数量
                    item.Files = Files.Where(x => x.FileExtensions == item.space).ToList();

                    item.PictureId = ID;
                }
                return spaceList;
            }
            catch (Exception)
            {

                throw;
            }


        }

        /// <summary>
        /// 添加地点设置
        /// </summary>
        /// <param name="entity"></param>
        public void InsertPhoneSet(SevenSPictureSetEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                entity.Create();
                entity.Files = null;
                db.Insert(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 删除地点设置
        /// </summary>
        /// <param name="entityId"></param>
        public void DelPhoneSet(string entityId)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var one = db.IQueryable<SevenSPictureSetEntity>().FirstOrDefault(x => x.Id == entityId);
                db.Delete(one);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 获取未提交数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SevenSPictureEntity> getState()
        {

            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<SevenSPictureEntity>().Where(x => x.state == "未提交");
        }

        private IDepartmentService service = new DepartmentService();
        private IOrganizeService Oservice = new OrganizeService();
        /// <summary>
        ///获取记录数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SevenSPictureEntity> getList(DateTime? planeStart, DateTime? planeEnd, string state, string evaluationState, string space, Pagination pagination, bool ispage, string deptId, bool isFile)
        {

            var db = new RepositoryFactory().BaseRepository();
            var queryex = LinqExtensions.True<SevenSPictureEntity>();

            if (!string.IsNullOrEmpty(deptId) && deptId != "0")
            {
                var one = Oservice.GetEntity(deptId);
                if (one == null)
                {
                    var entity = service.GetSubDepartments(deptId, "省级,厂级,部门,班组");
                    var listDept = new List<string>();
                    foreach (var item in entity)
                    {
                        listDept.Add(item.DepartmentId);
                    }
                    queryex = queryex.And(x => listDept.Contains(x.deptid));
                }
            }
            if (planeStart != null)
            {
                queryex = queryex.And(x => x.planeStartDate >= planeStart);
            }
            if (planeEnd != null)
            {
                queryex = queryex.And(x => x.planeEndDate <= planeEnd);
            }
            if (state != "全部")
            {
                if (state == "未提交")
                {
                    queryex = queryex.And(x => x.state == "未提交");
                }
                else if (string.IsNullOrEmpty(state))
                {
                    queryex = queryex.And(x => x.state == "已提交");
                }
                else
                {
                    queryex = queryex.And(x => x.state == "已提交");
                }

            }
            if (!string.IsNullOrEmpty(evaluationState))
            {
                if (evaluationState == "未评价")
                {
                    queryex = queryex.And(x => string.IsNullOrEmpty(x.evaluation));
                }
                else
                {
                    queryex = queryex.And(x => !string.IsNullOrEmpty(x.evaluation));
                }

            }
            var query = db.IQueryable(queryex);
            pagination.records = query.Count();
            if (query.Count() > 0)
            {
                if (ispage)
                {
                    query = query.OrderByDescending(x => x.CreateDate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows);
                }
                else
                {
                    query = query.OrderByDescending(x => x.CreateDate);
                }
            }

            //if (isFile)
            //{
            //    foreach (var item in query)
            //    {
            //        var ex = LinqExtensions.True<FileInfoEntity>();
            //        ex = ex.And(t => t.RecId == item.Id);
            //        if (!string.IsNullOrEmpty(space) && space != "全部")
            //        {
            //            ex = ex.And(t => t.FileExtensions == space);
            //            item.Files = db.IQueryable(ex).ToList();
            //        }
            //        else
            //        {
            //            item.Files = db.IQueryable(ex).ToList();
            //        }

            //    }
            //}
            return query;
        }

        /// <summary>
        /// 修改数据提交状态
        /// </summary>
        public void update(string id, string userid)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var one = db.FindEntity<SevenSPictureEntity>(id);
                var user = db.FindEntity<UserEntity>(userid);

                one.state = "已提交";
                one.ModifyDate = DateTime.Now;
                one.ModifyUserName = user.RealName;
                one.ModifyUserId = userid;
                db.Update(one);
                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 获取记录数据
        /// </summary>
        public SevenSPictureEntity getEntity(string keyvalue)
        {
            var db = new RepositoryFactory().BaseRepository();

            return db.FindEntity<SevenSPictureEntity>(keyvalue);



        }
        /// <summary>
        /// 获取记录数据
        /// </summary>
        public List<SevenSPictureEntity> getSevenSFinish(string deptid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var List = db.IQueryable<SevenSPictureEntity>().Where(x => x.deptid == deptid && x.state == "未提交").ToList();

            return List;
        }
        /// <summary>
        /// 插入记录数据
        /// </summary>
        /// <param name="entityList"></param>
        public void InsertList(List<SevenSPictureEntity> entityList)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(entityList);
                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 页面操作数据
        /// </summary>
        public void SaveFrom(List<SevenSPictureSetEntity> entityList, string[] del, string setTime, string regulation)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var TimeList = db.IQueryable<SevenSPictureCycleEntity>().ToList();

                foreach (var item in TimeList)
                {
                    item.regulation = regulation;
                }


                if (!string.IsNullOrEmpty(setTime))
                {
                    var Time = TimeList.FirstOrDefault(x => x.Id == setTime);
                    if (Time != null)
                    {
                        var oldTime = TimeList.FirstOrDefault(x => x.iswork);
                        oldTime.iswork = false;
                        Time.iswork = true;
                        //db.Update(Time);
                        //db.Update(oldTime);
                    }
                }
                db.Update(TimeList);

                var dataList = db.IQueryable<SevenSPictureSetEntity>();
                var delList = new List<SevenSPictureSetEntity>();
                for (int i = 0; i < del.Length; i++)
                {
                    if (!string.IsNullOrEmpty(del[i]))
                    {
                        var oneid = del[i];
                        var one = dataList.FirstOrDefault(x => x.Id == oneid);
                        delList.Add(one);
                    }

                }
                if (delList.Count > 0)
                {
                    db.Delete(delList);
                }
                if (entityList.Count > 0)
                {
                    db.Insert(entityList);
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
        /// 保存评价
        /// </summary>
        /// <returns></returns>  
        public void SaveEvaluation(string keyValue, string SaveEvaluation, string user)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var one = db.FindEntity<SevenSPictureEntity>(keyValue);
                one.evaluation = SaveEvaluation;
                one.evaluationDate = DateTime.Now.ToString();
                one.evaluationUser = user;
                db.Update(one);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

        }
        /// <summary>
        /// 保存时间段  用于app使用
        /// </summary>
        /// <param name="entity"></param>
        public void SavePlanTime(SevenSPlanTimeEntity entity)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(entity);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 查询时间段
        /// </summary>
        public List<SevenSPlanTimeEntity> getPlanTime()
        {

            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<SevenSPlanTimeEntity>().ToList();
        }

        #region  7S定点拍照 管理人员方法
        /// <summary>
        /// 根据状态获取最新的班组的提交数据
        /// </summary>
        /// <param name="state">状态 已提交 未提交</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public List<SevenSPictureEntity> GetListByManager(string state, string userId, bool? evaluateState, string deptid, out int totalCount, int pageIndex = 1, int pageSize = 5)
        {
            #region 旧
            //string sql = "SELECT * FROM 	wg_sevenspicture WHERE ID IN(  SELECT  SUBSTRING_INDEX(GROUP_CONCAT(ID ORDER BY ModifyDate DESC),',',1) FROM wg_sevenspicture where 1=1";
            //if (!string.IsNullOrWhiteSpace(state) && state != "全部")
            //{
            //    sql +=string.Format( " AND state='{0}'",state);
            //}

            //if (!string.IsNullOrWhiteSpace(deptid))
            //{
            //    sql += string.Format(" AND DeptId='{0}'", deptid);
            //}
            //sql += " GROUP BY DeptId)";
            //var data = new RepositoryFactory().BaseRepository().FindList<SevenSPictureEntity>(sql).OrderByDescending(x => x.ModifyDate).ToList();
            //var allIds = data.Select(p => p.Id).ToList();
            //var evaluteIds = new RepositoryFactory().BaseRepository().IQueryable<ActivityEvaluateEntity>(p => p.CREATEUSERID == userId && allIds.Contains(p.Activityid)).Select(p => p.Activityid).ToList();//已经评价的定点拍照ID
            //if (data.Count() > 0)
            //{
            //    if (evaluateState.HasValue)
            //    {

            //        if (evaluateState.Value)
            //        {
            //            //var idsQuery = from q1 in evalute join q2 in data on q1.Activityid equals q2.Id into into1 from t1 in into1.DefaultIfEmpty() where q1.CREATEUSERID == userId select q1.Activityid;
            //            //var ids = idsQuery.Distinct().ToList();
            //            ////已评价
            //            //var query = data.Where(p => ids.Contains(p.Id));
            //            var list = data.Where(p => evaluteIds.Contains(p.Id)).ToList();
            //            list.ForEach(x => { x.evaluationState = "已评价"; });
            //            return list;
            //        }
            //        else
            //        {
            //            //未评价
            //            //var evalute = new RepositoryFactory().BaseRepository().IQueryable<ActivityEvaluateEntity>();
            //            //var idsQuery = from q1 in evalute join q2 in data on q1.Activityid equals q2.Id into into1 from t1 in into1.DefaultIfEmpty() where q1.CREATEUSERID != userId select q1.Activityid;
            //            //var ids = idsQuery.Distinct().ToList();
            //            //var query = data.Where(p => ids.Contains(p.Id));
            //            var list = data.Where(p => !evaluteIds.Contains(p.Id)).ToList();
            //            list.ForEach(x => { x.evaluationState = "未评价"; });
            //            return list;
            //        }
            //    }
            //    else
            //    {
            //        //var evalute = new RepositoryFactory().BaseRepository().IQueryable<ActivityEvaluateEntity>();
            //        //var idsQuery = from q1 in evalute join q2 in data on q1.Activityid equals q2.Id into into1 from t1 in into1.DefaultIfEmpty() where q1.CREATEUSERID == userId select q1.Activityid;
            //        //var ids = idsQuery.Distinct().ToList();//已评价的Id

            //        //var dataList = data.ToList();
            //        data.ForEach(x => {
            //            if (evaluteIds.Contains(x.Id))
            //            {
            //                x.evaluationState = "已评价";
            //            }
            //            else
            //            {
            //                x.evaluationState = "未评价";
            //            }
            //        });

            //        return data;
            //    }
            //}
            //return new List<SevenSPictureEntity>();
            #endregion
            var query = new RepositoryFactory().BaseRepository().IQueryable<SevenSPictureEntity>();
            if (!string.IsNullOrWhiteSpace(state) && state != "全部") query = query.Where(p => p.state == state);
            if (!string.IsNullOrWhiteSpace(deptid)) query = query.Where(p => p.deptid == deptid);
            var evaluteQuery = new RepositoryFactory().BaseRepository().IQueryable<ActivityEvaluateEntity>();
            var allIds = query.Select(p => p.Id).ToList();
            var evaluteIds = new RepositoryFactory().BaseRepository().IQueryable<ActivityEvaluateEntity>(p => p.CREATEUSERID == userId && allIds.Contains(p.Activityid)).Select(p => p.Activityid).ToList();//已经评价的定点拍照ID
            if (evaluateState.HasValue)
            {

                if (evaluateState.Value)
                {
                    //已评价


                    query = query.Where(p => evaluteIds.Contains(p.Id));
                    //var linq = from q1 in query
                    //           select new SevenSPictureEntity()
                    //           {
                    //               Id = q1.Id,
                    //               CreateDate = q1.CreateDate,
                    //               CreateUserId = q1.CreateUserId,
                    //               CreateUserName = q1.CreateUserName,
                    //               deptid = q1.deptid,
                    //               deptname = q1.deptname,
                    //               evaluation = q1.evaluation,
                    //               evaluationDate = q1.evaluationDate,
                    //               evaluationUser = q1.evaluationUser,
                    //               ModifyDate = q1.ModifyDate,
                    //               ModifyUserId = q1.ModifyUserId,
                    //               ModifyUserName = q1.ModifyUserName,
                    //               planeEndDate = q1.planeEndDate,
                    //               planeStartDate = q1.planeStartDate,
                    //               state = q1.state,
                    //               evaluationState = evaluteQuery.any
                    //           }


                }
                else
                {
                    //未评价

                    query = query.Where(p => !evaluteIds.Contains(p.Id));
                }
                totalCount = query.Count();
                var data = query.OrderByDescending(p => p.CreateDate).Skip(pageSize * pageIndex - pageSize).Take(pageSize).ToList();
                data.ForEach(p => { p.evaluationState = evaluateState.Value ? "已评价" : "未评价"; });
                return data;
            }
            else
            {
                //var linq = from q1 in query
                //           orderby q1.CreateDate descending
                //           select new SevenSPictureEntity()
                //           {
                //               Id = q1.Id,
                //               CreateDate = q1.CreateDate,
                //               CreateUserId = q1.CreateUserId,
                //               CreateUserName = q1.CreateUserName,
                //               deptid = q1.deptid,
                //               deptname = q1.deptname,
                //               evaluation = q1.evaluation,
                //               evaluationDate = q1.evaluationDate,
                //               evaluationUser = q1.evaluationUser,
                //               ModifyDate = q1.ModifyDate,
                //               ModifyUserId = q1.ModifyUserId,
                //               ModifyUserName = q1.ModifyUserName,
                //               planeEndDate = q1.planeEndDate,
                //               planeStartDate = q1.planeStartDate,
                //               state = q1.state,
                //               evaluationState = evaluteQuery.Any(p => p.CREATEUSERID == userId && p.Activityid == q1.Id) ? "已评价" : "未评价"
                //           };
                //totalCount = linq.Count();
                totalCount = query.Count();
                var data = query.OrderByDescending(p => p.CreateDate).Skip(pageSize * pageIndex - pageSize).Take(pageSize).ToList();
                data.ForEach(p =>
                {
                    if (evaluteIds.Contains(p.Id))
                    {
                        p.evaluationState = "已评价";
                    }
                    else
                    {
                        p.evaluationState = "未评价";
                    }
                });
                return data;
            }
        }



        public List<SevenSPictureEntity> GetListByManager(DateTime? datefrom, DateTime? dateto, string state, string userId, bool? evaluateState, string deptid, out int totalCount, int pageIndex = 1, int pageSize = 5)
        {
            var data = new RepositoryFactory().BaseRepository().IQueryable<SevenSPictureEntity>();
            if (datefrom.HasValue)
                data = data.Where(p => p.planeStartDate >= datefrom.Value);
            if (dateto.HasValue)
                data = data.Where(p => p.planeEndDate <= dateto.Value);
            if (!string.IsNullOrWhiteSpace(state) && state != "全部")
                data = data.Where(p => p.state == state);
            if (!string.IsNullOrWhiteSpace(deptid))
                data = data.Where(p => p.deptid == deptid);
            var allIds = data.Select(p => p.Id).ToList();
            var evaluteIds = new RepositoryFactory().BaseRepository().IQueryable<ActivityEvaluateEntity>(p => p.CREATEUSERID == userId && allIds.Contains(p.Activityid)).Select(p => p.Activityid).ToList();//已经评价的定点拍照ID
            if (evaluateState.HasValue)
            {
                if (evaluateState.Value)
                {
                    data = data.Where(p => evaluteIds.Contains(p.Id)).OrderByDescending(p => p.CreateDate);
                    totalCount = data.Count();
                    var list = data.Skip(pageSize * pageIndex - pageSize).Take(pageSize).ToList();
                    list.ForEach(x => { x.evaluationState = "已评价"; });
                    return list;
                }
                else
                {
                    //未评价
                    data = data.Where(p => !evaluteIds.Contains(p.Id)).OrderByDescending(p => p.CreateDate);
                    totalCount = data.Count();
                    var list = data.Skip(pageSize * pageIndex - pageSize).Take(pageSize).ToList();
                    list.ForEach(x => { x.evaluationState = "未评价"; });
                    return list;
                }
            }
            else
            {
                //var evalute = new RepositoryFactory().BaseRepository().IQueryable<ActivityEvaluateEntity>();
                //var idsQuery = from q1 in evalute join q2 in data on q1.Activityid equals q2.Id into into1 from t1 in into1.DefaultIfEmpty() where q1.CREATEUSERID == userId select q1.Activityid;
                //var ids = idsQuery.Distinct().ToList();//已评价的Id
                totalCount = data.Count();
                var dataList = data.OrderByDescending(p => p.CreateDate).Skip(pageIndex * pageSize - pageSize).Take(pageSize).ToList();
                dataList.ForEach(x =>
                {
                    if (evaluteIds.Contains(x.Id))
                    {
                        x.evaluationState = "已评价";
                    }
                    else
                    {
                        x.evaluationState = "未评价";
                    }
                });

                return dataList;
            }
        }


        #endregion
        #endregion
        #region 精益管理

        /// <summary>
        /// 获取用户精益管理数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<SevenSOfficeEntity> getOfficebyuser(string userid)
        {

            try
            {
                var db = new RepositoryFactory().BaseRepository();
                return db.IQueryable<SevenSOfficeEntity>(x => x.createuserid == userid).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 获取用户精益管理数据
        /// </summary>
        /// <param name="Strid"></param>
        /// <returns></returns>
        public List<SevenSOfficeEntity> getOfficebyid(string Strid)
        {

            try
            {
                var db = new RepositoryFactory().BaseRepository();
                return db.IQueryable<SevenSOfficeEntity>(x => Strid.Contains(x.id)).ToList();
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
        public void Operation(SevenSOfficeEntity add, SevenSOfficeEntity update, string del, SevenSOfficeAuditEntity audit, SevenSOfficeAuditEntity auditupdate)
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
                    var getOne = db.IQueryable<SevenSOfficeEntity>(x => x.id == del).ToList();
                    var List = getAuditByid(del);
                    db.Delete(List);
                    db.Delete(getOne);
                }

                if (auditupdate != null)
                {
                    //修改时无新增
                    if (audit == null && update == null)
                    {
                        var getOne = db.IQueryable<SevenSOfficeEntity>(x => x.id == auditupdate.officeid).ToList();
                        var officeEntity = getOne[0];
                        officeEntity.aduitresult = auditupdate.state;
                        officeEntity.aduitstate = "已审核";
                        db.Update(officeEntity);
                    }
                    db.Update(auditupdate);
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
        /// 获取提案审核记录
        /// </summary>
        /// <param name="officeid"></param>
        /// <returns></returns>
        public List<SevenSOfficeAuditEntity> getAuditByid(string officeid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<SevenSOfficeAuditEntity>(x => x.officeid == officeid).ToList();
        }

        /// <summary>
        /// 获取提案审核记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<SevenSOfficeAuditEntity> getAuditByuser(string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<SevenSOfficeAuditEntity>(x => x.userid == userid).ToList();
        }

        /// <summary>
        /// 获取提案审核记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<SevenSOfficeAuditEntity> getAuditId(string id)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<SevenSOfficeAuditEntity>(x => x.auditid == id).ToList();
        }
        /// <summary>
        /// 平台查询
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public List<SevenSOfficeEntity> SelectOffice(string userid, Dictionary<string, string> keyValue, Pagination pagination)
        {

            try
            {
                //var user = OperatorProvider.Provider.Current();
                var list = getAuditByuser(userid);
                var Id = string.Join(",", list.Select(x => x.officeid));
                var db = new RepositoryFactory().BaseRepository();
                var expression = LinqExtensions.True<SevenSOfficeEntity>();

                if (keyValue.ContainsKey("aduitstate"))
                {
                    if (!string.IsNullOrEmpty(keyValue["aduitstate"]))
                    {
                        var val = keyValue["aduitstate"];
                        expression = expression.And(x => x.aduitstate == val);
                    }
                }
                if (keyValue.ContainsKey("aduitresult"))
                {
                    if (!string.IsNullOrEmpty(keyValue["aduitresult"]))
                    {
                        var val = keyValue["aduitresult"];
                        expression = expression.And(x => x.aduitresult == val);

                    }
                }
                if (keyValue.ContainsKey("start"))
                {
                    var val = Convert.ToDateTime(keyValue["start"]);
                    expression = expression.And(x => x.createdate >= val);
                }
                if (keyValue.ContainsKey("end"))
                {
                    var val = Convert.ToDateTime(keyValue["end"]);
                    expression = expression.And(x => x.createdate <= val);
                }
                if (keyValue.ContainsKey("name"))
                {
                    var val = keyValue["name"];
                    expression = expression.And(x => x.name.Contains(val));
                }
                expression = expression.And(x => x.aduitstate != "待提交");
                expression = expression.And(x => Id.Contains(x.id));
                var data = db.IQueryable(expression).OrderByDescending(x => x.createdate).Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 统计查询
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public List<SevenSOfficeEntity> SelectTotal(Dictionary<string, string> keyValue, string userid)
        {

            try
            {
                var list = getAuditByuser(userid);
                var Id = string.Join(",", list.Select(x => x.officeid));
                var db = new RepositoryFactory().BaseRepository();
                var expression = LinqExtensions.True<SevenSOfficeEntity>();
                if (keyValue.ContainsKey("year"))
                {
                    var val = Convert.ToInt32(keyValue["year"]);
                    var start = new DateTime(val, 1, 1);
                    var end = new DateTime(val, 12, 31);
                    expression = expression.And(x => x.createdate >= start);
                    expression = expression.And(x => x.createdate <= end);
                }
                if (keyValue.ContainsKey("start"))
                {
                    var val = Convert.ToDateTime(keyValue["start"]);
                    expression = expression.And(x => x.createdate >= val);
                }
                if (keyValue.ContainsKey("end"))
                {
                    var val = Convert.ToDateTime(keyValue["end"]);
                    expression = expression.And(x => x.createdate <= val);
                }
                if (keyValue.ContainsKey("deptid"))
                {
                    var val = keyValue["deptid"];
                    if (val != "0")
                    {
                        expression = expression.And(x => x.deptid == (val));
                    }
                }
                expression = expression.And(x => x.aduitstate != "待提交");
                var data = db.IQueryable(expression).ToList();
                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 得到当前用户的待审核数量
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int GetTodoCount(string userid)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = from q in db.IQueryable<SevenSOfficeEntity>()
                        join q1 in db.IQueryable<SevenSOfficeAuditEntity>()
                        on q.id equals q1.officeid
                        where q.aduitstate == "待审核" && q1.userid == userid
                        select q;
            return query.Count();
        }

        public IList<SevenSTypeEntity> GetAllType(string[] depts)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<SevenSTypeEntity>(x => depts.Contains(x.deptid)).OrderBy(x => x.CreateTime).ToList();
        }
        #endregion
    }
}
