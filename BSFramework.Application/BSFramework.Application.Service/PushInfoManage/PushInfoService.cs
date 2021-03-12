
using BSFramework.Application.Entity.PushInfoManage;
using BSFramework.Application.IService.PushInfoManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.PushInfoManage
{
    public class PushInfoService : RepositoryFactory<PushInfoEntity>, IPushInfoService
    {
        /// <summary>
        /// 根据用户获取推送的内容
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<PushInfoEntity> GetPushInfoList(string userId, string pushid)
        {

            var db = new RepositoryFactory().BaseRepository();
            var List = new List<PushInfoEntity>();
            try
            {
                if (!string.IsNullOrEmpty(pushid))
                {
                    List = db.IQueryable<PushInfoEntity>().Where(x => x.pushid == pushid).ToList();

                }
                else
                {
                    var person = db.IQueryable<PushPersonEntity>().Where(x => x.personid == userId);
                    var pushList = person.GroupBy(x => x.pushid);
                    var newdb = new RepositoryFactory().BaseRepository();
                    foreach (var item in pushList)
                    {

                        var one = newdb.FindEntity<PushInfoEntity>(item.Key);
                        List.Add(one);
                    }
                }
                return List;
            }
            catch (Exception)
            {
                return List;
            }

        }
        /// <summary>
        /// 根据推送表id获取阅读人数
        /// </summary>
        /// <param name="pushid"></param>
        /// <returns></returns>
        public IEnumerable<PushPersonEntity> GetPushPerson(string pushid)
        {

            var db = new RepositoryFactory().BaseRepository();
            var List = new List<PushPersonEntity>();
            try
            {
                List = db.IQueryable<PushPersonEntity>().Where(x => x.pushid == pushid).ToList();
                return List;
            }
            catch (Exception)
            {
                return List;
            }
        }
        /// <summary>
        /// 推送保存推送数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelList"></param>
        public void SavePushInfo(PushInfoEntity model, List<PushPersonEntity> modelList)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                db.Insert(model);


                db.Insert(modelList);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 阅读
        /// </summary>
        /// <param name="model"></param>
        public void PushRead(PushPersonEntity model)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                var one = db.IQueryable<PushPersonEntity>().FirstOrDefault(x => x.personid == model.personid & x.pushid == model.pushid);
                one.isread = true;
                db.Update(one);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

    }
}
