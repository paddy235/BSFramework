
using BSFramework.Application.Entity.OndutyManage;
using BSFramework.Application.IService.OndutyManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.OndutyManage
{
    /// <summary>
    /// 
    /// </summary>
    public class OndutyMeetOnoffService : RepositoryFactory<OndutyMeetOnoffEntity>, IOndutyMeetOnoffService
    {
        /// <summary>
        /// 修改开关
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <param name="onoff"></param>
        public void OnOff(string keyvalue, string onoff)
        {
            var db = new RepositoryFactory().BaseRepository().BeginTrans();

            try
            {
                var IsOn = onoff == "on";
                var ck = db.IQueryable<OndutyMeetOnoffEntity>().Where(x => x.meetid == keyvalue).ToList();
                if (ck.Count() >= 1)
                {
                    var one = ck[0];
                    one.onoff = IsOn;
                    db.Update(ck);
                }
                else
                {
                    var one = new OndutyMeetOnoffEntity() { id = Guid.NewGuid().ToString(), meetid = keyvalue, onoff = IsOn };
                    db.Insert(one);

                }
                db.Commit();

            }
            catch (Exception)
            {

                db.Rollback();
            }

        }
        /// <summary>
        /// 获取开关数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public OndutyMeetOnoffEntity getCk(string keyvalue)
        {
            return this.BaseRepository().IQueryable().FirstOrDefault(x => x.meetid == keyvalue);
        }
    }
}
