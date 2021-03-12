using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.Activity;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.Activity
{
    /// <summary>
    /// 安全日活动-改进行动
    /// </summary>
    public class ActivityActionService : RepositoryFactory<ActivityActionEntity>, IActivityActionService
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void Del(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository();
            db.Delete<ActivityActionEntity>(keyValue);
            Task.Run(() =>
            {
                var delFileList = db.IQueryable<FileInfoEntity>().Where(x => x.RecId == keyValue).ToList();
                if (delFileList != null && delFileList.Count > 0)
                {
                    try
                    {
                        db.Delete(delFileList);
                        delFileList.ForEach(file =>
                        {
                            string url = Config.GetValue("FilePath") + file.FilePath.Replace("~/Resource", "").Replace("/", "\\");
                            if (!string.IsNullOrEmpty(file.FilePath) && System.IO.File.Exists(url))
                            {
                                System.IO.File.Delete(url);
                            }
                        });
                    }
                    catch (Exception)
                    {

                    }
                }
            });
        }

        /// <summary>
        /// 根据安全日活动的Id获取改进行动，包含上次落实情况
        /// </summary>
        /// <param name="activityId">安全日活动的Id</param>
        /// <param name="deptId">班组ID</param>
        /// <returns></returns>
        public List<ActivityActionEntity> GetListByActionId(string activityId, string deptId)
        {
            var db = new RepositoryFactory().BaseRepository();
            //本次改进行动 + 所有的未完成的行动（当前安全日活动之前的安全日活动下的未完成的改进行动） + 前一次的安全日活动的 所有的改进行动
            //System.Linq.Expressions.Expression<Func<ActivityActionEntity, bool>> expression = x => x.ActivityId == activityId;
            

            //所有的未完成的行动（当前安全日活动之前的安全日活动下的未完成的改进行动）
            var thisCreateDate = db.IQueryable<ActivityEntity>(x => x.ActivityId == activityId).FirstOrDefault()?.CreateDate;
            //上一次的安全活动日的Id
            var lastActivityId = db.IQueryable<ActivityEntity>(x => x.CreateDate < thisCreateDate && x.GroupId == deptId).OrderByDescending(x => x.CreateDate).FirstOrDefault()?.ActivityId;
            IQueryable<ActivityActionEntity> actionQuery = default;
            if (thisCreateDate.HasValue)
            {
                var lastActivityIds = db.IQueryable<ActivityEntity>(x => x.CreateDate <= thisCreateDate.Value && x.GroupId == deptId).Select(x => x.ActivityId);
                actionQuery = db.IQueryable<ActivityActionEntity>(x => x.ActivityId == activityId || (lastActivityIds.Contains(x.ActivityId) && x.Status != "已完成") || x.ActivityId == lastActivityId);
            }
            else
            {
                actionQuery = db.IQueryable<ActivityActionEntity>(x => x.ActivityId == activityId || (x.Status != "已完成" && x.DeptId == deptId) || x.ActivityId == lastActivityId);
            }


            var query = from action in actionQuery
                        join file in db.IQueryable<FileInfoEntity>() on action.Id equals file.RecId into t
                        //where action.ActivityId == activityId || (action.Status != "已完成" && action.DeptId==deptId)
                        select new { action, t };
            var data = query.ToList();
            var url = BSFramework.Util.Config.GetValue("AppUrl");
            var result = data.Select(x =>
            {
                if (x.t != null && x.t.Count() > 0)
                {
                    foreach (var item in x.t)
                    {
                        item.FilePath = item.FilePath.Replace("~/", url);
                    }
                }
                return new ActivityActionEntity()
                {
                    ActivityId = x.action.ActivityId,
                    Content = x.action.Content,
                    CreateDate = x.action.CreateDate,
                    CreateUserId = x.action.CreateUserId,
                    CreateUserName = x.action.CreateUserName,
                    DeptCode = x.action.DeptCode,
                    DeptId = x.action.DeptId,
                    DeptName = x.action.DeptName,
                    FileList = x.t,
                    FinishDate = x.action.FinishDate,
                    Id = x.action.Id,
                    IsLast = x.action.ActivityId != activityId,
                    ModifyDate = x.action.ModifyDate,
                    ModifyUserId = x.action.ModifyUserId,
                    ModifyUserName = x.action.ModifyUserName,
                    Status = x.action.Status,
                    UserIds = x.action.UserIds,
                    UserNames = x.action.UserNames,
                };
            }).ToList();
            return result;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="data"></param>
        public void Insert(ActivityActionEntity data)
        {
            data.Create();
            BaseRepository().Insert(data);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="actionEntity">数据实体</param>
        /// <param name="files">要新增的文件的实体集合</param>
        /// <param name="delIds">要删除的文件的主键ID</param>
        public void Update(ActivityActionEntity actionEntity, List<FileInfoEntity> files, string[] delIds)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
        
            try
            {
                var entity = db.FindEntity<ActivityActionEntity>(actionEntity.Id);
                if (entity == null) throw new Exception("未找到要修改的数据");
                entity.Modify();
                if (actionEntity.IsLast==true)
                {
                    //如果是上次未落实的， 只该对应的字段即可
                    entity.Status = actionEntity.Status;
                    entity.FinishDate = actionEntity.FinishDate;
                }
                else
                {
                    entity.UserIds = actionEntity.UserIds;
                    entity.UserNames = actionEntity.UserNames;
                    entity.Content = actionEntity.Content;
                }
                var delFileList = db.IQueryable<FileInfoEntity>().Where(x => delIds.Contains(x.FileId)).ToList();
                db.Update(entity);
                db.Insert(files);
                db.Delete<FileInfoEntity>(delIds);
                db.Commit();
                if (delFileList !=null && delFileList.Count>0)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            delFileList.ForEach(file =>
                            {
                                string url = Config.GetValue("FilePath") + file.FilePath.Replace("~/Resource", "").Replace("/", "\\");
                                if (!string.IsNullOrEmpty(file.FilePath) && System.IO.File.Exists(url))
                                {
                                    System.IO.File.Delete(url);
                                }
                            });
                        }
                        catch (Exception)
                        {

                        }
                    });
                }

            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }

        }
    }
}
