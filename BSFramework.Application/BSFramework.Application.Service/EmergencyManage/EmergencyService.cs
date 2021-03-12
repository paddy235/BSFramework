using BSFramework.Application.Entity.EmergencyManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.EmergencyManage;
using BSFramework.Util.Extension;
using System.Data;
using BSFramework.Data;
using BSFramework.Util.WebControl;
using BSFramework.Util;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
namespace BSFramework.Application.Service.EmergencyManage
{
    public class EmergencyService : RepositoryFactory<EmergencyEntity>, IEmergencyService
    {


        public IEnumerable<EmergencyEntity> GetList(string typename, string typeId, string name, string cardId, int pageIndex, int pageSize, bool ispage)
        {


            var query = this.BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(typeId))
            {
                query = query.Where(x => typeId.Contains(x.TypeId));
            }
            if (!string.IsNullOrEmpty(typename))
            {
                var db = new RepositoryFactory().BaseRepository();
                var typeList = db.IQueryable<EmergencyCardTypeEntity>().FirstOrDefault(x => x.TypeName.Contains(typename));
                query = query.Where(x => x.TypeId == typeList.TypeId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            List<EmergencyEntity> cardList = new List<EmergencyEntity>();
            if (!string.IsNullOrEmpty(cardId))
            {
                if (cardId.Contains(","))
                {
                    var id = cardId.Split(',');
                    for (int i = 0; i < id.Count(); i++)
                    {
                        var selectId = id[i];
                        var get = query.Where(x => x.ID == selectId).ToList();
                        cardList.AddRange(get);
                    }
                }
                else
                {
                    query = query.Where(x => x.ID == cardId);
                    cardList = query.ToList();
                }
            }
            else
            {
                cardList = query.ToList();
            }
            if (cardList.Count > 0)
            {
                if (ispage)
                {
                    cardList = cardList.OrderByDescending(x => x.CreateDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
                }
                else
                {
                    cardList = cardList.OrderByDescending(x => x.CreateDate).ToList();
                }
            }

            foreach (var item in cardList)
            {
                var db = new RepositoryFactory().BaseRepository();
                var typeList = db.IQueryable<FileInfoEntity>().FirstOrDefault(x => x.RecId == item.ID);
                var type = db.FindEntity<EmergencyCardTypeEntity>(item.TypeId);
                item.TypeName = type.TypeName;
                if (typeList != null)
                {
                    item.FileId = typeList.FileId;
                    item.FilePath = typeList.FilePath;
                }


            }
            return cardList;
        }

        public IList<EmergencyCardTypeEntity> GetAllCardType(string deptCode)
        {
            var db = new RepositoryFactory().BaseRepository();
            return db.IQueryable<EmergencyCardTypeEntity>(x => deptCode.StartsWith(x.deptcode)).OrderBy(x => x.CreateTime).ToList();
        }
        public IEnumerable<EmergencyEntity> GetPageList(string from, string to, string name, string deptid, int page, int pagesize, out int total)
        {
            var query = this.BaseRepository().IQueryable();
            //query = query.Where(x => x.BZId == deptid);
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Name == name);
            if (!string.IsNullOrEmpty(from))
            {
                DateTime time1 = DateTime.Parse(from);
                DateTime time2 = time1.AddDays(1);
                query = query.Where(x => x.CreateDate >= time1);
            }

            if (!string.IsNullOrEmpty(to))
            {
                DateTime time2 = DateTime.Parse(to).AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.CreateDate <= time2);
            }
            total = query.Count();
            return query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();

        }


        public EmergencyEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }

        public EmergencyWorkEntity GetWorkEntity(string keyValue)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var entity = db.FindEntity<EmergencyWorkEntity>(keyValue);
            var entityStepList = new List<EmergencyStepsEntity>();
            entityStepList = db.IQueryable<EmergencyStepsEntity>().Where(x => x.EmergencyId == keyValue).OrderBy(x => x.EmergencySort).ToList();
            string strSte = "";
            foreach (EmergencyStepsEntity ese in entityStepList)
            {
                strSte += ese.EmergencyContext + "\r\n";
            }
            entity.ImplementStep = strSte;
            entity.ToCompileDeptId = db.FindEntity<UserEntity>(entity.ToCompileUserid).DepartmentId;
            var file = db.FindEntity<FileInfoEntity>(x => x.RecId == keyValue);
            if (file != null)
            {
                entity.AttachmentId = db.FindEntity<FileInfoEntity>(x => x.RecId == keyValue).FileId;
            }
            return entity;
        }

        public void AddCardType(EmergencyCardTypeEntity model)
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
        public IList<EmergencyCardTypeEntity> GetIndex(string name, string deptCode, string typeid)
        {

            var db = new RepositoryFactory().BaseRepository();

            if (string.IsNullOrEmpty(typeid)) typeid = null;

            var node = from q in db.IQueryable<EmergencyCardTypeEntity>()
                       where q.ParentCardId == null
                       select q;

            if (typeid != null)
            {
                node = from q in db.IQueryable<EmergencyCardTypeEntity>()
                       where q.TypeId == typeid
                       select q;
            }

            var categories = node;

            var current = from q1 in node
                          join q2 in db.IQueryable<EmergencyCardTypeEntity>() on q1.TypeId equals q2.ParentCardId
                          where deptCode.StartsWith(q2.deptcode)
                          select q2;

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<EmergencyCardTypeEntity>() on q1.TypeId equals q2.ParentCardId
                          where deptCode.StartsWith(q2.deptcode)
                          select q2;

            }
            List<EmergencyCardTypeEntity> entity = categories.ToList();
            var returnentity = new List<EmergencyCardTypeEntity>();
            foreach (var item in entity)
            {
                var emergencyList = (from q1 in db.IQueryable<EmergencyEntity>()
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
        public IList<EmergencyEntity> GetCarkItems(string key, string typeid, int pagesize, int page, string deptCode, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();

            if (string.IsNullOrEmpty(typeid)) typeid = null;

            var node = from q in db.IQueryable<EmergencyCardTypeEntity>()
                       where q.ParentCardId == null
                       select q;

            if (typeid != null)
            {
                node = from q in db.IQueryable<EmergencyCardTypeEntity>()
                       where q.TypeId == typeid
                       select q;
            }

            var categories = node;

            var current = from q1 in node
                          join q2 in db.IQueryable<EmergencyCardTypeEntity>() on q1.TypeId equals q2.ParentCardId
                          where deptCode.StartsWith(q2.deptcode)

                          select q2;

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<EmergencyCardTypeEntity>() on q1.TypeId equals q2.ParentCardId
                          where deptCode.StartsWith(q2.deptcode)

                          select q2;

            }

            var query = from q1 in db.IQueryable<EmergencyEntity>()
                        join q2 in categories on q1.TypeId equals q2.TypeId
                        join q3 in db.IQueryable<FileInfoEntity>() on q1.ID equals q3.RecId
                        select new { q3.FileId, q1.Path, q3.FilePath, q1.ID, q1.Name, q1.TypeId, q2.TypeName, q1.CreateDate, q1.CREATEUSERNAME, q1.MODIFYDATE, q1.MODIFYUSERNAME, q1.seenum };


            total = query.Count();
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(x => x.Name.Contains(key));
            }
            query = query.OrderBy(x => x.seenum).Skip(pagesize * (page - 1)).Take(pagesize);
            return query.ToList().Select(x => new EmergencyEntity() { FileId = x.FileId, FilePath = x.FilePath, Path = x.Path, ID = x.ID, Name = x.Name, TypeName = x.TypeName, TypeId = x.TypeId, CREATEUSERNAME = x.MODIFYDATE < DateTime.Now.AddYears(-3) ? x.CREATEUSERNAME : x.MODIFYUSERNAME, CreateDate = x.MODIFYDATE < DateTime.Now.AddYears(-3) ? x.CreateDate : x.MODIFYDATE, seenum = x.seenum }).ToList();
        }

        public void EditCardType(EmergencyCardTypeEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.FindEntity<EmergencyCardTypeEntity>(model.TypeId);
                var cnt = db.IQueryable<EmergencyCardTypeEntity>(x => x.TypeName == model.TypeName).Count();
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
        public void DeleteCardType(string id)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var cnt = db.IQueryable<EmergencyCardTypeEntity>(x => x.ParentCardId == id).Count();
                if (cnt > 0) throw new Exception("请先删除子类别");
                cnt = db.IQueryable<EmergencyEntity>(x => x.TypeId == id).Count();
                if (cnt > 0) throw new Exception("请先删除内容");
                db.Delete<EmergencyCardTypeEntity>(id);
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
            //var card = db.FindEntity<EmergencyEntity>(id);
            //var fileInfo = db.IQueryable<FileInfoEntity>().FirstOrDefault(x => x.RecId == card.ID);
            //db.Delete<FileInfoEntity>(fileInfo.FileId);
            try
            {
                db.Delete<EmergencyEntity>(id);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
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
        public void SaveForm(string keyValue, EmergencyEntity entity)
        {
            var entity1 = this.GetEntity(keyValue);
            if (string.IsNullOrEmpty(keyValue))
            {
                this.BaseRepository().Insert(entity);
                // new Repository<FileInfoEntity>(DbFactory.Base()).Insert(entity.Files.ToList());

            }
            else
            {
                entity1.Path = entity.Path;
                entity1.Name = entity.Name;
                entity1.MODIFYDATE = entity.MODIFYDATE;
                entity1.MODIFYUSERID = entity.MODIFYUSERID;
                entity1.MODIFYUSERNAME = entity.MODIFYUSERNAME;

                //var user = OperatorProvider.Provider.Current();
                //entity1.MODIFYDATE = entity.MODIFYDATE == null ? DateTime.Now : entity.MODIFYDATE;
                //entity1.MODIFYUSERID = user.UserId;
                //entity1.MODIFYUSERNAME = user.UserName;
                this.BaseRepository().Update(entity1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void SaveEmergencyEntity(EmergencyEntity entity)
        {

            this.BaseRepository().Update(entity);

        }
        public void SaveFormWork(string keyValue, EmergencyWorkEntity workEntity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var user = db.FindEntity<DepartmentEntity>(x => x.DepartmentId == workEntity.ToCompileDeptId);

                var dept = db.FindEntity<UserEntity>(x => x.UserId == workEntity.ToCompileUserid);
                if (user != null)
                {
                    workEntity.ToCompileDept = user.FullName;
                }
                if (dept != null)
                {
                    workEntity.ToCompileUser = dept.RealName;
                }
                if (string.IsNullOrEmpty(keyValue) || keyValue == "undefined")
                {
                    workEntity.EmergencyId = Guid.NewGuid().ToString();
                    workEntity.CREATEDATE = DateTime.Now;
                    //workEntity.CREATEUSERID = OperatorProvider.Provider.Current().UserId;
                    //workEntity.CREATEUSERNAME = OperatorProvider.Provider.Current().UserName;
                    string[] array = workEntity.ImplementStep.Split(new char[] { '\r', '\n' });
                    var esList = new List<EmergencyStepsEntity>();
                    int i = 0;
                    foreach (string str in array)
                    {
                        if (str.Trim() != "" && !string.IsNullOrEmpty(str))
                        {

                            EmergencyStepsEntity esEntity = new EmergencyStepsEntity();
                            esEntity.EmergencyStepsId = Guid.NewGuid().ToString();
                            esEntity.EmergencyId = workEntity.EmergencyId;
                            esEntity.EmergencyContext = str;
                            esEntity.EmergencySort = i;
                            esEntity.CREATEDATE = DateTime.Now;
                            //esEntity.CREATEUSERID = OperatorProvider.Provider.Current().UserId;
                            //esEntity.CREATEUSERNAME = OperatorProvider.Provider.Current().UserName;
                            esList.Add(esEntity);
                            i++;
                        }
                    }
                    db.Insert<EmergencyStepsEntity>(esList);
                    db.Insert(workEntity);
                    db.Commit();

                }
                else
                {
                    var entity1 = GetWorkEntity(keyValue);
                    //entity1.Name = entity.Name;
                    entity1.Name = workEntity.Name;
                    entity1.EmergencyType = workEntity.EmergencyType;
                    entity1.ToCompileUser = workEntity.ToCompileUser;
                    entity1.ToCompileUserid = workEntity.ToCompileUserid;
                    entity1.ToCompileDeptId = workEntity.ToCompileDeptId;
                    entity1.ToCompileDept = workEntity.ToCompileDept;
                    entity1.Attachment = workEntity.Attachment;
                    entity1.Purpose = workEntity.Purpose;
                    entity1.RehearseDate = workEntity.RehearseDate;
                    entity1.RehearsePlace = workEntity.RehearsePlace;
                    entity1.RehearseType = workEntity.RehearseType;
                    entity1.RehearseScenario = workEntity.RehearseScenario;
                    entity1.RehearseName = workEntity.RehearseName;
                    //entity1.Name = workEntity.Name;
                    entity1.MainPoints = workEntity.MainPoints;
                    entity1.MODIFYDATE = DateTime.Now;
                    //entity1.MODIFYUSERID = OperatorProvider.Provider.Current().UserId;
                    //entity1.MODIFYUSERNAME = OperatorProvider.Provider.Current().UserName;
                    entity1.ImplementStep = null;
                    //添加实施步骤
                    db.Delete<EmergencyStepsEntity>(x => x.EmergencyId.Equals(keyValue));
                    string[] array = workEntity.ImplementStep.Split(new char[] { '\r', '\n' });
                    var esList = new List<EmergencyStepsEntity>();
                    int i = 0;
                    foreach (string str in array)
                    {
                        if (str.Trim() != "" && !string.IsNullOrEmpty(str))
                        {

                            EmergencyStepsEntity esEntity = new EmergencyStepsEntity();
                            esEntity.EmergencyStepsId = Guid.NewGuid().ToString();
                            esEntity.EmergencyId = keyValue;
                            esEntity.EmergencyContext = str;
                            esEntity.EmergencySort = i;
                            esEntity.CREATEDATE = DateTime.Now;
                            //esEntity.CREATEUSERID = OperatorProvider.Provider.Current().UserId;
                            //esEntity.CREATEUSERNAME = OperatorProvider.Provider.Current().UserName;
                            esList.Add(esEntity);
                            i++;
                        }
                    }
                    db.Insert<EmergencyStepsEntity>(esList);
                    db.Update<EmergencyWorkEntity>(entity1);
                    db.Commit();
                }

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        public IList<EmergencyWorkEntity> GetEvaluations(string deptid, string name, int pagesize, int page, string ToCompileDeptIdSearch, string EmergencyTypeSearch, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<EmergencyWorkEntity>().ToList();
            //string deptid = OperatorProvider.Provider.Current().DeptId;

            var dept = db.FindEntity<DepartmentEntity>(deptid);
            if (dept != null)
            {
                if (dept.Nature == "部门")
                {
                    query = this.GetDeptUserWork(deptid);
                }
            }
            foreach (EmergencyWorkEntity ewe in query)
            {
                //var user = db.FindEntity<UserEntity>(ewe.ToCompileUserid);
                //if (user != null)
                //{
                //    ewe.ToCompileUser = user.RealName;
                //}
                //ewe.ToCompileDept = db.FindEntity<DepartmentEntity>(ewe.ToCompileDeptId).FullName;
                if (string.IsNullOrEmpty(ewe.Attachment))
                {
                    ewe.Attachment = "";
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                //query = query.Where(x => x.EvaluateSeason.Contains(name));
            }
            if (!string.IsNullOrEmpty(ToCompileDeptIdSearch) && ToCompileDeptIdSearch != "全部") query = query.Where(x => x.ToCompileDeptId == ToCompileDeptIdSearch).ToList();
            if (!string.IsNullOrEmpty(EmergencyTypeSearch) && EmergencyTypeSearch != "全部") query = query.Where(x => x.EmergencyType == EmergencyTypeSearch).ToList();
            total = query.Count();
            return query.OrderByDescending(x => x.CREATEDATE).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public List<EmergencyWorkEntity> GetDeptUserWork(string keyValue)
        {

            var db = new RepositoryFactory().BaseRepository();

            var categorygroups = from q1 in db.IQueryable<DepartmentEntity>().Where(x => x.DepartmentId == keyValue)
                                 join q2 in db.IQueryable<DepartmentEntity>() on q1.ParentId equals q2.DepartmentId
                                 select q1;
            var categories = from q1 in categorygroups
                             select new { DepartmentId = q1.DepartmentId, ParentId = q1.DepartmentId };

            var current = from q1 in db.IQueryable<DepartmentEntity>()
                          join q2 in categorygroups on q1.ParentId equals q2.DepartmentId
                          select new { DepartmentId = q1.DepartmentId, ParentId = q2.DepartmentId };

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                          select new { DepartmentId = q2.DepartmentId, ParentId = q1.ParentId };
            }
            var query = from q1 in db.IQueryable<EmergencyWorkEntity>()
                        join q2 in categories on q1.ToCompileDeptId equals q2.DepartmentId
                        select q1;
            return query.ToList();
        }
        public IList<EmergencyReportEntity> GetEvaluationsManoeuvre(string deptid, string name, int pagesize, int page, string ToCompileDeptIdSearch, string EmergencyTypeSearch, string meetingstarttime, string meetingendtime, out int total)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<EmergencyReportEntity>().ToList();
            //string deptid = OperatorProvider.Provider.Current().DeptId;

            var dept = db.FindEntity<DepartmentEntity>(deptid);
            if (dept != null)
            {
                if (dept.Nature == "部门")
                {
                    query = this.GetDeptUserReport(deptid);
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                //query = query.Where(x => x.EvaluateSeason.Contains(name));
            }
            if (!string.IsNullOrEmpty(ToCompileDeptIdSearch) && ToCompileDeptIdSearch != "全部") query = query.Where(x => x.deptid == ToCompileDeptIdSearch).ToList();
            if (!string.IsNullOrEmpty(EmergencyTypeSearch) && EmergencyTypeSearch != "全部") query = query.Where(x => x.emergencytype == EmergencyTypeSearch).ToList();
            if (!string.IsNullOrEmpty(meetingstarttime) && meetingstarttime != "") query = query.Where(x => x.planstarttime >= DateTime.Parse(meetingstarttime)).ToList();
            if (!string.IsNullOrEmpty(meetingendtime) && meetingendtime != "") query = query.Where(x => x.planstarttime <= DateTime.Parse(meetingendtime).AddDays(1).AddMinutes(-1)).ToList();
            total = query.Count();
            return query.OrderByDescending(x => x.CREATEDATE).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
        }

        public IList<EmergencyReportEntity> GetAllList()
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = db.IQueryable<EmergencyReportEntity>().ToList();

            return query.OrderByDescending(x => x.CREATEDATE).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public List<EmergencyReportEntity> GetDeptUserReport(string keyValue)
        {

            var db = new RepositoryFactory().BaseRepository();

            var categorygroups = from q1 in db.IQueryable<DepartmentEntity>().Where(x => x.DepartmentId == keyValue)
                                 join q2 in db.IQueryable<DepartmentEntity>() on q1.ParentId equals q2.DepartmentId
                                 select q1;
            var categories = from q1 in categorygroups
                             select new { DepartmentId = q1.DepartmentId, ParentId = q1.DepartmentId };

            var current = from q1 in db.IQueryable<DepartmentEntity>()
                          join q2 in categorygroups on q1.ParentId equals q2.DepartmentId
                          select new { DepartmentId = q1.DepartmentId, ParentId = q2.DepartmentId };

            while (current.Count() > 0)
            {
                categories = categories.Concat(current);

                current = from q1 in current
                          join q2 in db.IQueryable<DepartmentEntity>() on q1.DepartmentId equals q2.ParentId
                          select new { DepartmentId = q2.DepartmentId, ParentId = q1.ParentId };
            }
            var query = from q1 in db.IQueryable<EmergencyReportEntity>()
                        join q2 in categories on q1.deptid equals q2.DepartmentId
                        select q1;
            return query.ToList();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void DelEmergency(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete<EmergencyWorkEntity>(keyValue);
                db.Delete<EmergencyStepsEntity>(x => x.EmergencyId == keyValue);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        #endregion

        #region 终端应急演练

        public EmergencyReportEntity GetReportEntity(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();
            var query = db.FindEntity<EmergencyReportEntity>(keyValue);
            query.EmergencyReportSteps = db.IQueryable<EmergencyReportStepsEntity>().Where(x => x.EmergencyReportId == query.EmergencyReportId).ToList();
            var dbs = new Repository<FileInfoEntity>(DbFactory.Base());
            query.File = dbs.IQueryable().Where(x => x.RecId == keyValue).ToList();
            return query;
        }
        /// <summary>
        /// 应急预案列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<EmergencyReportEntity> EmergencyReportGetPageList(string DeptId, string name, DateTime? from, DateTime? to, int page, int pagesize, out int total)
        {
            var query = new RepositoryFactory<EmergencyReportEntity>().BaseRepository().IQueryable();
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.emergencyreportname.Contains(name));
            if (!string.IsNullOrEmpty(DeptId)) query = query.Where(x => x.deptid == DeptId);
            if (from != null) query = query.Where(x => x.planstarttime >= from.Value);
            if (to != null)
            {
                to = to.Value.AddDays(1).AddMinutes(-1);
                query = query.Where(x => x.planstarttime <= to);
            }
            total = query.Count();
            var data = query.OrderByDescending(x => x.planstarttime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
            var db = new Repository<FileInfoEntity>(DbFactory.Base());
            data.ForEach(x => x.File = db.IQueryable().Where(y => y.RecId == x.EmergencyReportId && y.Description == "照片").OrderByDescending(y => y.CreateDate).Take(1).ToList());
            return data;
        }

        /// <summary>
        /// 终端页面应急预案
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="deptName"></param>
        /// <param name="EmergencyType"></param>
        /// <returns></returns>
        public IEnumerable<EmergencyWorkEntity> getEmergencyWorkList(DateTime? from, DateTime? to, string deptName, string EmergencyType)
        {
            var list = new List<EmergencyWorkEntity>();
            try
            {
                var expression = LinqExtensions.True<EmergencyWorkEntity>();
                if (!string.IsNullOrEmpty(EmergencyType))
                {
                    expression = expression.And(t => t.EmergencyType == EmergencyType);
                }
                if (!string.IsNullOrEmpty(deptName))
                {
                    expression = expression.And(t => t.ToCompileDept == deptName);
                }
                if (from != null)
                {
                    expression = expression.And(t => t.ToCompileDate >= from);
                }
                if (to != null)
                {
                    expression = expression.And(t => t.ToCompileDate <= to);
                }
                var db = new RepositoryFactory().BaseRepository();
                list = db.IQueryable<EmergencyWorkEntity>(expression).ToList();
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }

        }
        /// <summary>
        /// 开始演练新增记录
        /// </summary>
        /// <param name="entity"></param>
        public void InsertEmergencyReport(EmergencyReportEntity entity, List<EmergencyPersonEntity> entitys)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                entity.Create();
                foreach (var item in entitys)
                {
                    item.Create();
                    item.EmergencyReportId = entity.EmergencyReportId;
                }
                db.Insert(entity);
                db.Insert(entitys);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

        }
        /// <summary>
        /// 步骤修改数据
        /// </summary>
        public void updateEmergencyReport(string EmergencyReportId, string Purpose, string RehearsesceNario, string MainPoints, bool radio, string score, string effectreport, string planreport)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var one = db.FindEntity<EmergencyReportEntity>(x => x.EmergencyReportId == EmergencyReportId);
                if (!string.IsNullOrEmpty(Purpose))
                {
                    one.purpose = Purpose;
                }
                if (!string.IsNullOrEmpty(RehearsesceNario))
                {
                    one.rehearsescenario = RehearsesceNario;
                }
                if (!string.IsNullOrEmpty(MainPoints))
                {
                    one.mainpoints = MainPoints;
                }
                one.isupdate = radio;
                if (!string.IsNullOrEmpty(score))
                {
                    one.evaluationscore = decimal.Parse(score);
                }
                if (!string.IsNullOrEmpty(effectreport))
                {
                    one.effectreport = effectreport;
                }
                if (!string.IsNullOrEmpty(planreport))
                {
                    one.planreport = planreport;
                }
                //清理非实体的数据实例
                one.EmergencyPersons = null;
                one.File = null;
                one.Modify(one.EmergencyReportId);
                db.Update(one);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }


        }

        /// <summary>
        /// 添加演练步骤
        /// </summary>
        /// <param name="entity"></param>
        public void saveReportSteps(List<EmergencyReportStepsEntity> entity)
        {

            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                foreach (var item in entity)
                {
                    var one = db.IQueryable<EmergencyReportStepsEntity>().FirstOrDefault(x => x.EmergencyReportId == item.EmergencyReportId && x.EmergencySort == item.EmergencySort);
                    if (one == null)
                    {
                        db.Insert(item);
                    }
                    else
                    {
                        one.EmergencyUser = item.EmergencyUser;
                        one.EmergencyUserid = item.EmergencyUserid;
                        db.Update(one);
                    }
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

        }
        public void updateEmergencyReportEvaluate(string EmergencyReportId, EmergencyReportEntity workEntity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            var query = db.FindEntity<EmergencyReportEntity>(EmergencyReportId);
            try
            {
                //var one = db.FindEntity<EmergencyReportEntity>(x => x.EmergencyReportId == EmergencyReportId);
                query.state = "1";
                //if (!string.IsNullOrEmpty(workEntity.evaluationscore.ToString()) && workEntity.evaluationscore != 10)
                //{
                //    query.evaluationscore = workEntity.evaluationscore;
                //}
                if (!string.IsNullOrEmpty(workEntity.score.ToString()) && workEntity.score != 10)
                {
                    query.score = workEntity.score;
                }
                query.evaluation = workEntity.evaluation;
                query.evaluationdate = DateTime.Now;
                query.evaluationuser = workEntity.evaluationuser;
                //query.score = query.score;

                //清理非实体的数据实例
                query.EmergencyPersons = null;
                query.EmergencyReportSteps = null;
                query.File = null;
                query.MODIFYDATE = DateTime.Now;
                db.Update(query);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }


        }

        #endregion


        #region 管理平台
        /// <summary>
        /// 导入应急预案保存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="entitys"></param>
        public void SaveImportList(List<EmergencyWorkEntity> entity, List<EmergencyStepsEntity> entitys)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(entity);
                db.Insert(entitys);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }

        /// <summary>
        /// 删除应急预案
        /// </summary>
        /// <param name="emergencyId"></param>
        public void deleteEmergency(string emergencyId)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var entity = db.IQueryable<EmergencyWorkEntity>().FirstOrDefault(row => row.EmergencyId == emergencyId);
                var entitys = db.IQueryable<EmergencyStepsEntity>().Where(row => row.EmergencyId == emergencyId);
                db.Delete(entity);
                db.Delete(entitys);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }


        }
        /// <summary>
        /// 获取应急演练步骤人员
        /// </summary>
        /// <returns></returns>
        public IList<EmergencyReportStepsEntity> GetEmergencyReportStepsList(string EmergencyId, string EmergencyReportId)
        {
            try
            {
                var list = new List<EmergencyReportStepsEntity>();
                var expression = LinqExtensions.True<EmergencyReportStepsEntity>();
                if (!string.IsNullOrEmpty(EmergencyReportId))
                {
                    expression = expression.And(t => t.EmergencyReportId == EmergencyReportId);
                }
                if (!string.IsNullOrEmpty(EmergencyId))
                {
                    expression = expression.And(t => t.EmergencyId == EmergencyId);
                }
                var db = new RepositoryFactory().BaseRepository();
                list = db.IQueryable<EmergencyReportStepsEntity>(expression).ToList();
                return list;
            }
            catch (Exception ex)
            {

                return new List<EmergencyReportStepsEntity>();
            }

        }
        /// <summary>
        /// 获取应急演练
        /// </summary>
        /// <returns></returns>
        public IList<EmergencyReportEntity> GetEmergencyReportList(string createUser, string EmergencyReportId)
        {
            try
            {
                var list = new List<EmergencyReportEntity>();
                var expression = LinqExtensions.True<EmergencyReportEntity>();
                if (!string.IsNullOrEmpty(createUser))
                {
                    expression = expression.And(t => t.CREATEUSERNAME == createUser);
                }
                if (!string.IsNullOrEmpty(EmergencyReportId))
                {
                    expression = expression.And(t => t.EmergencyReportId == EmergencyReportId);
                }
                var db = new RepositoryFactory().BaseRepository();
                list = db.IQueryable<EmergencyReportEntity>(expression).ToList();
                return list;
            }
            catch (Exception ex)
            {

                return new List<EmergencyReportEntity>();
            }

        }

        /// <summary>
        /// 获取应急预案
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="EmergencyType"></param>
        /// <returns></returns>
        public IList<EmergencyWorkEntity> GetEmergencyWorkList
            (string deptId, string EmergencyType, string EmergencyId, string createUser)
        {
            try
            {
                var list = new List<EmergencyWorkEntity>();
                var expression = LinqExtensions.True<EmergencyWorkEntity>();
                if (!string.IsNullOrEmpty(deptId))
                {
                    expression = expression.And(t => t.ToCompileDeptId == deptId);

                }
                if (!string.IsNullOrEmpty(EmergencyType))
                {
                    expression = expression.And(t => t.EmergencyTypeId == EmergencyType);
                }
                if (!string.IsNullOrEmpty(EmergencyId))
                {
                    expression = expression.And(t => t.EmergencyId == EmergencyId);
                }
                if (!string.IsNullOrEmpty(createUser))
                {
                    expression = expression.And(t => t.CREATEUSERNAME == createUser);
                }
                var db = new RepositoryFactory().BaseRepository();
                list = db.IQueryable<EmergencyWorkEntity>(expression).ToList();
                return list;
            }
            catch (Exception ex)
            {

                return new List<EmergencyWorkEntity>();
            }

        }

        /// <summary>
        /// 获取应急预案步骤
        /// </summary>
        public IList<EmergencyStepsEntity> GetEmergencyStepsList(string EmergencyId, string createUser)
        {
            try
            {
                var list = new List<EmergencyStepsEntity>();
                var expression = LinqExtensions.True<EmergencyStepsEntity>();
                if (!string.IsNullOrEmpty(EmergencyId))
                {
                    expression = expression.And(t => t.EmergencyId == EmergencyId);

                }
                if (!string.IsNullOrEmpty(createUser))
                {
                    expression = expression.And(t => t.CREATEUSERNAME == createUser);
                }
                var db = new RepositoryFactory().BaseRepository();
                list = db.IQueryable<EmergencyStepsEntity>(expression).ToList();
                return list;
            }
            catch (Exception ex)
            {
                return new List<EmergencyStepsEntity>();
            }

        }

        #endregion
    }
}
