
using BSFramework.Application.Entity.SafetyScore;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.IService.WorkMeeting;
using BSFramework.Application.Service.SafetyScore;
using BSFramework.Data.Repository;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.WorkMeeting
{
    /// <summary>
    /// 班前一课
    /// </summary>
    public class MeetSubjectService : RepositoryFactory<MeetSubjectEntity>, IMeetSubjectService
    {

        #region MyRegion
        /// <summary>
        /// 活动id获取数据
        /// </summary>
        /// <returns></returns>
        public MeetSubjectEntity getDataByMeetID(string MeetId)
        {
            var db = new RepositoryFactory().BaseRepository();
            var data = db.IQueryable<MeetSubjectEntity>(x => x.MeetId == MeetId).FirstOrDefault();
            return data;
        }
        /// <summary>
        /// 主键获取数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public MeetSubjectEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void delEntity(string keyValue)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var delEntity = db.IQueryable<MeetSubjectEntity>(x => keyValue.Contains(x.ID));
                db.Delete(delEntity);
                db.Commit();
            }
            catch (Exception ex)
            {

                db.Rollback();

            }

        }
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
        public void SaveForm(string keyValue, MeetSubjectEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                //添加安全积分
                SafetyScoreService scoreService = new SafetyScoreService();
                if (!string.IsNullOrEmpty(keyValue))
                {
                    var old = db.FindEntity<MeetSubjectEntity>(x => x.ID == keyValue);
                    if (!old.State && entity.State)
                    {
                        var ck = db.IQueryable<AccountRuleEntity>(p => p.Standard == "班前一课" && p.IsOpen == 1).FirstOrDefault();
                        if (ck != null)
                        {
                            scoreService.AddScore(entity.TeachUserId, 7);
                        }

                    }
                    db.Update(entity);

                }
                else
                {

                    if (entity.State)
                    {
                        scoreService.AddScore(entity.TeachUserId, 7);
                    }
                    db.Insert(entity);
                }
                db.Commit();
            }
            catch (Exception ex)
            {

                db.Rollback();

            }

        }
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public void Save(MeetSubjectEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var ck = db.IQueryable<AccountRuleEntity>(p => p.Standard == "班前一课" && p.IsOpen == 1).FirstOrDefault();
                if (ck != null)
                {
                    SafetyScoreService scoreService = new SafetyScoreService();
                    if (entity.State)
                    {
                        scoreService.AddScore(entity.TeachUserId, 7);
                    }
                }
                db.Insert(entity);
                db.Commit();
            }
            catch (Exception)
            {

                db.Rollback();
            }

        }
        #endregion

    }
}
