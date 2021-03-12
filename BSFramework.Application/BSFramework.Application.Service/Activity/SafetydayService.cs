using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Util;
using BSFramework.Util.Extension;
using System.IO;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Application.Service.PublicInfoManage;
using BSFramework.Data.EF;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;

namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class SafetydayService : RepositoryFactory<SafetydayEntity>, SafetydayIService
    {
        private DepartmentService deptService = new DepartmentService();
        private FileInfoService fileinfo = new PublicInfoManage.FileInfoService();

        private System.Data.Entity.DbContext _context;
        public SafetydayService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<SafetydayEntity> GetList(string queryJson)
        {
            return this.BaseRepository().IQueryable().ToList();
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public DataTable GetPageList(Pagination pagination, string queryJson)
        {
            var queryParam = queryJson.ToJObject();

            if (!queryParam["keyword"].IsEmpty())
            {
                string subject = queryParam["keyword"].ToString().Trim();
                pagination.conditionJson += string.Format(" and subject like '%{0}%'", subject);
            }
            if (!queryParam["activitytype"].IsEmpty())
            {
                string activitytype = queryParam["activitytype"].ToString().Trim();
                pagination.conditionJson += string.Format(" and activitytype ='{0}'", activitytype);
            }
            else
            {
                pagination.conditionJson += string.Format(" and activitytype !='{0}'", "安全学习日");

            }
            DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, DbHelper.DbType);
            return dt;
        }
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public List<SafetydayEntity> GetPagesList(Pagination pagination, string queryJson)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<SafetydayEntity>();
            var queryParam = queryJson.ToJObject();

            if (!queryParam["keyword"].IsEmpty())
            {
                string subject = queryParam["keyword"].ToString().Trim();
                expression = expression.And(x => x.Subject.Contains(subject));

            }

            if (!queryParam["keyvalue"].IsEmpty())
            {
                string subject = queryParam["keyvalue"].ToString().Trim();
                expression = expression.And(x => x.Subject.Contains(subject));

            }


            if (!queryParam["activitytype"].IsEmpty())
            {
                string activitytype = queryParam["activitytype"].ToString().Trim();
                expression = expression.And(x => x.ActivityType == activitytype);
            }
            //else
            //{
            //    expression = expression.And(x => x.ActivityType == "安全学习日");

            //}
            var data = db.IQueryable(expression).OrderByDescending(x => x.CreateDate);
            pagination.records = data.Count();
            return data.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
        }

        public List<SafetydayMaterialEntity> getMaterial(string deptid, string keyvalue)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            try
            {
                return db.FindList<SafetydayMaterialEntity>(x => x.deptid == deptid && x.SafetydayId == keyvalue).ToList();
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public SafetydayEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        public List<SafetydayEntity> GetIdEntityList(string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue) && keyValue.Length > 0)
            {
                IRepository db = new RepositoryFactory().BaseRepository();
                var expression = LinqExtensions.True<SafetydayEntity>();
                expression = expression.And(x => keyValue.Contains(x.Id));
                return db.IQueryable(expression).OrderByDescending(x => x.CreateDate).ToList();
                //keyValue.TrimEnd(',');
                //string sql = " SELECT Id,CreateDate,CreateUserId,CreateUserName,ModifyDate,ModifyUserId,ModifyUserName,DeptCode,`Subject`,`Explain`,DeptId,DeptName,Remark,ActIds,activitytype from wg_safetyday where Id in ('" + keyValue.Replace(",", "','") + "');";
                //return this.BaseRepository().FindList(sql).ToList();
            }
            return new List<SafetydayEntity>();
        }

        /// <summary>
        /// 得到学习园地
        /// </summary>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public DataTable GetLearningGardens(int? rowcount = null)
        {
            //string sql = "select t1.filename,t1.filepath from wg_safetyday t,base_fileinfo t1 where t.Id = t1.RECID  order by t1.CREATEDATE DESC LIMIT {0} ";
            //if (rowcount.HasValue)
            //{
            //    sql = string.Format(sql, rowcount);
            //}
            //else
            //{
            //    sql = string.Format(sql, 20);
            //}
            //return this.BaseRepository().FindTable(sql);
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = from a in db.IQueryable<SafetydayEntity>()
                        join b in db.IQueryable<FileInfoEntity>()
                        on a.Id equals b.RecId
                        orderby b.CreateDate descending
                        select new { filename = b.FileName, filepath = b.FilePath };
            if (rowcount.HasValue)
            {
                //sql = string.Format(sql, rowcount);
                query = query.Take(rowcount.Value);
            }
            else
            {
                query = query.Take(20);
                //sql = string.Format(sql, 20);
            }
            var queryTalbe = DataHelper.ConvertToTable(query);
            return queryTalbe;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            SafetydayEntity entity = this.BaseRepository().FindEntity(keyValue);
            //if (!string.IsNullOrEmpty(entity.ActIds))
            //{
            if (this.BaseRepository().Delete(keyValue) > 0)
            {
                var resAct = new Repository<ActivityEntity>(DbFactory.Base());
                if (!string.IsNullOrEmpty(entity.ActIds))
                {
                    resAct.ExecuteBySql(string.Format("delete from wg_activity where activityid in('{0}')", entity.ActIds.Replace(",", "','")));

                    resAct.ExecuteBySql(string.Format("delete from wg_orderinfo where SdId='{0}'", entity.Id));
                }
                var resRead = new Repository<SafetydayReadEntity>(DbFactory.Base());
                resRead.ExecuteBySql(string.Format("delete from wg_SafetydayRead where safetydayid='{0}'", entity.Id));
                var material = new Repository<SafetydayMaterialEntity>(DbFactory.Base());
                resRead.ExecuteBySql(string.Format("delete from wg_safetydaymaterial where safetydayid='{0}'", entity.Id));
            }
            //}

        }
        /// <summary>
        /// 新增安全日活动并同步新增到班组活动及相关附件
        /// </summary>
        /// <param name="entity">安全日活动实体</param>
        /// <param name="mode">安全日活动实体</param>
        /// <param name="path">文件目录</param>
        private void SaveEntity(SafetydayEntity entity, int mode, string path)
        {
            StringBuilder sb = new StringBuilder();
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (mode == 0)
                {
                    entity.Create();

                    //获取相关所有附件信息
                    var resFile = new Repository<FileInfoEntity>(DbFactory.Base());
                    List<FileInfoEntity> listFiles = resFile.FindList(string.Format("select * from base_fileinfo where recid='" + entity.Id + "'")).ToList();
                    //List<ActivityEntity> list = new List<ActivityEntity>();
                    List<FileInfoEntity> newFiles = new List<FileInfoEntity>();

                    //同步记录到班组活动
                    //var resGroup = new Repository<OrderinfoEntity>(DbFactory.Base());
                    //List<OrderinfoEntity> listOrders = new List<OrderinfoEntity>();
                    int j = 0;
                    foreach (string str in entity.DeptId.Split(','))
                    {

                        //同步记录到班组预约台
                        //OrderinfoEntity order = new OrderinfoEntity
                        //{
                        //    DeptCode = entity.DeptCode,
                        //    SdId = entity.Id,
                        //    GroupId = str,
                        //    IsOrder = 0,
                        //    GroupName = entity.DeptName.Split(',')[j]
                        //};
                        //order.Create();
                        //listOrders.Add(order);
                        //j++;
                        ////设置业务记录Id
                        //string id = Guid.NewGuid().ToString();
                        ////设置班组活动基本信息
                        //ActivityEntity act = new ActivityEntity
                        //{
                        //    ActivityId = id,
                        //    ActivityType = "安全日活动",
                        //    Subject = entity.Subject,
                        //    Remark = entity.Explain,
                        //    State = "Prepare",
                        //    GroupId = str,
                        //    CreateDate = DateTime.Now
                        //};
                        //list.Add(act);
                        //sb.Append(id + ",");
                        //设置班组活动记录相关的附件信息
                        //foreach (FileInfoEntity fi in listFiles)
                        //{
                        //    var filepath = fi.FilePath.Replace("~", path).Replace('/', '\\');
                        //    FileInfoEntity fin = new FileInfoEntity();
                        //    fin.FileName = fi.FileName;
                        //    fin.FilePath = fi.FilePath;
                        //    fin.FileSize = fi.FileSize;
                        //    fin.FileType = fi.FileType;
                        //    fin.FileExtensions = fi.FileExtensions;
                        //    //fin.RecId = act.ActivityId;
                        //    fin.Description = "材料";
                        //    fin.Create();
                        //    var newfile = filepath.Substring(0, filepath.LastIndexOf("\\") + 1) + fin.FileId + fin.FileExtensions;
                        //    File.Copy(filepath, newfile, true);
                        //    fin.FilePath = fi.FilePath.Substring(0, fi.FilePath.LastIndexOf("/") + 1) + fin.FileId + fin.FileExtensions;
                        //    newFiles.Add(fin);
                        //}
                    }
                    //entity.ActIds = sb.ToString().TrimEnd(',');
                    if (this.BaseRepository().Insert(entity) > 0)
                    {
                        //resGroup.Insert(listOrders);//批量插入班组预约信息
                        //var resAct = new Repository<ActivityEntity>(DbFactory.Base());
                        //resAct.Insert(list);//批量插入班组活动
                        resFile.Insert(newFiles);//批量插入相关附件信息   
                    }


                }
                else
                {
                    SafetydayEntity oldEntity = this.BaseRepository().FindEntity(entity.Id);
                    entity.DeptCode = oldEntity.DeptCode;
                    entity.CreateDate = oldEntity.CreateDate;
                    entity.CreateUserId = oldEntity.CreateUserId;
                    entity.CreateUserName = oldEntity.CreateUserName;
                    entity.ActIds = string.IsNullOrEmpty(entity.ActIds) ? oldEntity.ActIds : entity.ActIds;
                    //获取相关所有附件信息
                    var resFile = new Repository<FileInfoEntity>(DbFactory.Base());
                    List<FileInfoEntity> listFiles = resFile.FindList(string.Format("select * from base_fileinfo where recid='" + entity.Id + "'")).ToList();

                    //List<ActivityEntity> list = new List<ActivityEntity>();
                    List<FileInfoEntity> newFiles = new List<FileInfoEntity>();
                    //同步记录到班组活动
                    //var resGroup = new Repository<OrderinfoEntity>(DbFactory.Base());
                    //List<OrderinfoEntity> listOrders = new List<OrderinfoEntity>();

                    foreach (string str in entity.DeptId.Split(','))
                    {

                        if (!oldEntity.DeptId.Contains(str))
                        {
                            //设置业务记录Id
                            //string id = Guid.NewGuid().ToString();
                            //sb.Append(id + ",");
                            ////设置班组活动基本信息
                            //ActivityEntity act = new ActivityEntity
                            //{
                            //    ActivityId = id,
                            //    ActivityType = "安全日活动",
                            //    Subject = entity.Subject,
                            //    Remark = entity.Explain,
                            //    GroupId = str,
                            //    CreateDate = DateTime.Now
                            //};

                            //list.Add(act);

                            //设置班组活动记录相关的附件信息
                            //foreach (FileInfoEntity fi in listFiles)
                            //{
                            //    FileInfoEntity fin = new FileInfoEntity();
                            //    fin.FileName = fi.FileName;
                            //    fin.FilePath = fi.FilePath;
                            //    fin.FileSize = fi.FileSize;
                            //    fin.FileType = fi.FileType;
                            //    fin.FileExtensions = fi.FileExtensions;
                            //    fin.Description = fi.Description;
                            //    //fin.RecId = act.ActivityId;
                            //    fin.Description = "材料";
                            //    fin.Create();
                            //    newFiles.Add(fin);
                            //}
                        }

                    }
                    //if (sb.Length > 0)
                    //{
                    //    entity.ActIds += "," + sb.ToString().TrimEnd(',');

                    //}
                    //foreach (string id in entity.ActIds.Split(','))
                    //{
                    //    foreach (FileInfoEntity fi in listFiles)
                    //    {
                    //        FileInfoEntity fin = new FileInfoEntity();
                    //        fin.FileName = fi.FileName;
                    //        fin.FilePath = fi.FilePath;
                    //        fin.FileSize = fi.FileSize;
                    //        fin.FileType = fi.FileType;
                    //        fin.FileExtensions = fi.FileExtensions;
                    //        fin.Description = fi.Description;
                    //        fin.RecId = id;
                    //        fin.Create();
                    //        newFiles.Add(fin);
                    //    }
                    //}
                    if (this.BaseRepository().Update(entity) > 0)
                    {
                        //var resAct = new Repository<ActivityEntity>(DbFactory.Base());
                        //resAct.ExecuteBySql(string.Format("update wg_activity set subject='{0}',Remark='{1}' where activityid in('{2}') and state='Prepare'", entity.Subject, entity.Explain, entity.ActIds.Replace(",", "','")));
                        //resFile.ExecuteBySql(string.Format("delete from base_fileinfo where recid in('{0}')", entity.ActIds.Replace(",", "','")));
                        //resAct.ExecuteBySql(string.Format("delete from wg_activity where activityid in('{0}') and groupid not in('{1}')", entity.ActIds.Replace(",", "','"), entity.DeptId.Replace(",", "','")));
                        //resAct.Insert(list);
                        resFile.Insert(newFiles);//批量插入相关附件信息   

                    }
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
        /// 
        /// </summary>
        /// <returns></returns>
        public void RemoveFile(string keyValue)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Delete<SafetydayMaterialEntity>(x => x.fileid == keyValue);
                db.Commit();
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <param name="path">目录文件</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, SafetydayEntity entity, string path)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    SafetydayEntity sd = this.BaseRepository().FindEntity(keyValue);
                    if (sd == null)
                    {
                        SaveEntity(entity, 0, path);
                        List<SafetydayReadEntity> readList = new List<SafetydayReadEntity>();
                        List<SafetydayMaterialEntity> MaterialList = new List<SafetydayMaterialEntity>();
                        var filelist = fileinfo.GetFileList(entity.Id);
                        foreach (string str in entity.DeptId.Split(','))
                        {
                            foreach (var item in filelist)
                            {
                                SafetydayMaterialEntity material = new SafetydayMaterialEntity();
                                material.Id = Guid.NewGuid().ToString();
                                var dept = deptService.GetEntity(str);
                                material.deptid = dept.DepartmentId;
                                material.deptname = dept.FullName;
                                material.isread = false;
                                material.SafetydayId = entity.Id;
                                material.fileid = item.FileId;
                                material.filename = item.FileName;
                                MaterialList.Add(material);
                            }
                            var user = db.IQueryable<UserEntity>().Where(x => x.DepartmentId == str).ToList();
                            if (user != null)
                            {
                                foreach (UserEntity u in user)
                                {
                                    SafetydayReadEntity read = new SafetydayReadEntity();
                                    read.SafetydayReadId = Guid.NewGuid().ToString();
                                    read.SafetydayId = entity.Id;
                                    read.Userid = u.UserId;
                                    read.Deptid = u.DepartmentId;
                                    read.IsRead = 0;
                                    read.activitytype = entity.ActivityType;
                                    read.Create();
                                    readList.Add(read);
                                }
                            }
                        }
                        db.Insert(MaterialList);
                        db.Insert<SafetydayReadEntity>(readList);
                        //db.Commit();
                    }
                    else
                    {
                        var deptIdsFront = db.FindEntity<SafetydayEntity>(entity.Id).DeptId.Split(',').ToArray();//前
                        var deptIdsBack = entity.DeptId.Split(',').ToArray();//后
                        var deptId = deptIdsFront.Intersect(deptIdsBack).ToArray();//修改前和修改后都有的数据
                                                                                   //需要删除的通知信息


                        var deptIdDelete = deptIdsFront.Except(deptId).ToArray();
                        foreach (string str in deptIdDelete)
                        {
                            db.Delete<SafetydayReadEntity>(x => x.Deptid == str && x.SafetydayId == entity.Id);
                            db.Delete<SafetydayMaterialEntity>(x => x.deptid == str && x.SafetydayId == entity.Id);
                        }
                        //需要添加的通知信息
                        var deptIdInsert = deptIdsBack.Except(deptId).ToArray();
                        List<SafetydayReadEntity> readList = new List<SafetydayReadEntity>();
                        List<SafetydayMaterialEntity> MaterialList = new List<SafetydayMaterialEntity>();
                        var filelist = fileinfo.GetFileList(entity.Id);
                        //删除已经清理的file 添加新增的
                        if (deptId.Length > 0)
                        {
                            var deptidStr = deptId[0];
                            var oldfilelist = db.FindList<SafetydayMaterialEntity>(x => x.deptid == deptidStr && x.SafetydayId == entity.Id).Select(x => x.fileid).ToArray();
                            var newfilelist = filelist.Select(x => x.FileId).ToArray();
                            var file = oldfilelist.Intersect(newfilelist).ToArray();//还存有的
                            var fileDelete = oldfilelist.Except(file).ToArray();//要删除的
                            var fileadd = newfilelist.Except(file).ToArray();
                            for (int i = 0; i < fileDelete.Length; i++)
                            {
                                db.Delete<SafetydayMaterialEntity>(x => x.fileid == fileDelete[i] && x.SafetydayId == entity.Id);
                            }
                            //添加新增的file
                            foreach (var item in deptIdsBack)
                            {
                                for (int i = 0; i < fileadd.Length; i++)
                                {
                                    var addfile = fileinfo.GetEntity(fileadd[i]);
                                    SafetydayMaterialEntity material = new SafetydayMaterialEntity();
                                    material.Id = Guid.NewGuid().ToString();
                                    var dept = deptService.GetEntity(item);
                                    material.deptid = dept.DepartmentId;
                                    material.deptname = dept.FullName;
                                    material.isread = false;
                                    material.SafetydayId = entity.Id;
                                    material.fileid = addfile.FileId;
                                    material.filename = addfile.FileName;
                                    MaterialList.Add(material);
                                }
                            }

                        }

                        foreach (string str in deptIdInsert)
                        {
                            foreach (var item in filelist)
                            {
                                SafetydayMaterialEntity material = new SafetydayMaterialEntity();
                                material.Id = Guid.NewGuid().ToString();
                                var dept = deptService.GetEntity(str);
                                material.deptid = dept.DepartmentId;
                                material.deptname = dept.FullName;
                                material.isread = false;
                                material.SafetydayId = entity.Id;
                                material.fileid = item.FileId;
                                material.filename = item.FileName;
                                MaterialList.Add(material);
                            }
                            var user = db.IQueryable<UserEntity>().Where(x => x.DepartmentId == str).ToList();
                            if (user != null)
                            {
                                foreach (UserEntity u in user)
                                {
                                    SafetydayReadEntity read = new SafetydayReadEntity();
                                    read.SafetydayReadId = Guid.NewGuid().ToString();
                                    read.SafetydayId = entity.Id;
                                    read.Userid = u.UserId;
                                    read.Deptid = u.DepartmentId;
                                    read.IsRead = 0;
                                    read.activitytype = entity.ActivityType;
                                    read.Create();
                                    readList.Add(read);
                                }
                            }
                        }
                        db.Insert(MaterialList);
                        db.Insert<SafetydayReadEntity>(readList);
                        //db.Commit();
                        entity.Modify(keyValue);
                        SaveEntity(entity, 1, path);
                    }
                }
                else
                {

                    SaveEntity(entity, 0, path);
                    List<SafetydayReadEntity> readList = new List<SafetydayReadEntity>();
                    List<SafetydayMaterialEntity> MaterialList = new List<SafetydayMaterialEntity>();
                    var filelist = fileinfo.GetFileList(entity.Id);
                    foreach (string str in entity.DeptId.Split(','))
                    {
                        foreach (var item in filelist)
                        {
                            SafetydayMaterialEntity material = new SafetydayMaterialEntity();
                            material.Id = Guid.NewGuid().ToString();
                            var dept = deptService.GetEntity(str);
                            material.deptid = dept.DepartmentId;
                            material.deptname = dept.FullName;
                            material.isread = false;
                            material.SafetydayId = entity.Id;
                            material.fileid = item.FileId;
                            material.filename = item.FileName;
                            MaterialList.Add(material);
                        }
                        var user = db.IQueryable<UserEntity>().Where(x => x.DepartmentId == str).ToList();
                        if (user != null)
                        {
                            foreach (UserEntity u in user)
                            {
                                SafetydayReadEntity read = new SafetydayReadEntity();
                                read.SafetydayReadId = Guid.NewGuid().ToString();
                                read.SafetydayId = entity.Id;
                                read.Userid = u.UserId;
                                read.Deptid = u.DepartmentId;
                                read.IsRead = 0;
                                read.activitytype = entity.ActivityType;
                                read.Create();
                                readList.Add(read);
                            }
                        }
                    }
                    db.Insert(MaterialList);
                    db.Insert<SafetydayReadEntity>(readList);
                }
                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public IEnumerable<SafetydayEntity> GetSafetydayList(string userid, int page, int pagesize, string category, out int total)
        {

            var userservice = new UserService();
            var user = userservice.GetEntity(userid);

            IRepository db = new RepositoryFactory().BaseRepository();

            try
            {
                string deptid = user.DepartmentId;
                DateTime time = DateTime.Now.AddDays(-365);
                var query = db.IQueryable<SafetydayEntity>().OrderByDescending(x => x.CreateDate).Where(x => x.DeptId.Contains(deptid) && x.ActivityType == category && x.CreateDate > time).ToList();
                if (query.ToList().Count > 50)
                {
                    query = query.Take(50).ToList();
                }
                total = query.Count();
                var model = query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                var userId = userid;
                foreach (SafetydayEntity s in model)
                {
                    var read = db.FindEntity<SafetydayReadEntity>(x => x.SafetydayId == s.Id && x.Userid == userId);
                    if (read != null)
                    {
                        s.state = read.IsRead;
                    }
                }


                return model;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveRead(string keyValue, string userId)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var read = db.FindEntity<SafetydayReadEntity>(x => x.SafetydayId == keyValue && x.Userid == userId);
                if (read != null)
                {
                    read.IsRead = 1;
                    db.Update<SafetydayReadEntity>(read);
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        public List<SafetydayEntity> GetSafetydayList(string deptId, string category)
        {
            IRepository db = new RepositoryFactory().BaseRepository();

            var query = db.IQueryable<SafetydayEntity>().Where(x => x.DeptId.Contains(deptId)).ToList();
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.ActivityType == category).ToList();
            }
            return query;
        }

        public SafetydayReadEntity GetSafetydayReadEntity(string deptId, string userid, string safetydayid)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var query = db.FindEntity<SafetydayReadEntity>(x => x.Deptid == deptId && x.Userid == userid && x.SafetydayId == safetydayid);
            return query;
        }
        /// <summary>
        /// 获取通知数量
        /// </summary>
        /// <returns></returns>
        public IList<SafetydayReadEntity> GetIndex(string deptid, string userid, string category)
        {
            IRepository db = new RepositoryFactory().BaseRepository();
            var safety = db.IQueryable<SafetydayReadEntity>().Where(x => x.Deptid == deptid && x.IsRead == 0 && x.Userid == userid && x.activitytype == category);
            return safety.ToList();
        }

        public List<SafetydayEntity> GetList(string name, int pagesize, int pageindex, out int total)
        {
            var query = from q1 in _context.Set<SafetydayEntity>()
                        join q2 in _context.Set<SafetydayMaterialEntity>() on q1.Id equals q2.SafetydayId into into2
                        select new { q1.Id, q1.Subject, q1.CreateDate, q1.CreateUserName, q1.ActivityType, Total = into2.Count(), Readed = into2.Count(x => x.isread == true) };
            if (!string.IsNullOrEmpty(name)) query = query.Where(x => x.Subject.Contains(name));

            total = query.Count();
            var data = query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (pageindex - 1)).Take(pagesize).ToList();
            return data.Select(x => new SafetydayEntity { Id = x.Id, Subject = x.Subject, CreateUserName = x.CreateUserName, CreateDate = x.CreateDate, ActivityType = x.ActivityType, Material = $"{x.Readed}/{x.Total}" }).ToList();
        }



        /// <summary>
        /// 获取单位某时间段内的活动开展次数统计
        /// </summary>
        /// <param name="enumerable">单位的主键的集合</param>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <param name="catagory">活动类型</param>
        public List<KeyValue> GetStatistics(IEnumerable<string> enumerable, DateTime starttime , DateTime endtime, string catagory)
        {
            var db = new RepositoryFactory().BaseRepository();
            endtime = (endtime.AddDays(1).Date);

           var query = from a in db.IQueryable<DepartmentEntity>()
                        join activity in db.IQueryable<ActivityEntity>(x=> x.State == "Finish" && x.StartTime >= starttime && x.StartTime < endtime && x.ActivityType == catagory) on a.DepartmentId equals activity.GroupId  into t
                       where  enumerable.Contains(a.DepartmentId)
                        //group t by   a.FullName  into g
                        select new { Count = t.Count(), a.FullName };
                        



            //var query = from activity in db.IQueryable<ActivityEntity>()
            //            join dept in db.IQueryable<DepartmentEntity>() on activity.GroupId equals dept.DepartmentId
            //            where enumerable.Contains(activity.GroupId) && activity.State == "Finish" && activity.StartTime >= starttime && activity.StartTime < endtime && activity.ActivityType == catagory
            //            group activity.ActivityId by new { activity.GroupId,dept.FullName } into g
            //            select new { Count = g.Count(), g.Key.FullName };
            var data = query.ToList();
            var kvs = data.Select(x => new KeyValue() { key = x.FullName, value = x.Count.ToString()}).OrderByDescending(x=>x.value).ToList();
            return kvs;
        }
        #endregion
    }
}
