using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Data.EF;
using BSFramework.Data.Repository;
using BSFramework.Util;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Service.BaseManage
{
    /// <summary>
    /// 个人首页内容
    /// </summary>
    public class PersonDoshboardService : IPersonDoshboardService
    {
        private System.Data.Entity.DbContext _context;

        /// <summary>
        /// 构造器
        /// </summary>
        public PersonDoshboardService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        public List<PersonDoshboardEntity> GetEnabledList(string userId)
        {
            var data = _context.Set<PersonDoshboardEntity>().AsNoTracking().Where(x => x.UserId == userId && x.Enabled == true).OrderBy(x => x.Seq).ToList();
            return data;
        }

        public List<PersonDoshboardEntity> GetList(string userId)
        {
            var data = _context.Set<PersonDoshboardEntity>().AsNoTracking().Where(x => x.UserId == userId).ToList();
            return data;
        }

        public void Save(List<PersonDoshboardEntity> settings)
        {
            var keys = settings.Select(x => x.PersonDoshboardId).ToArray();
            var exists = _context.Set<PersonDoshboardEntity>().Where(x => keys.Contains(x.PersonDoshboardId)).ToList();

            foreach (var item in settings)
            {
                var exist = exists.Find(x => x.PersonDoshboardId == item.PersonDoshboardId);
                if (exist == null) _context.Set<PersonDoshboardEntity>().Add(item);
                else
                {
                    _context.Entry(exist).State = System.Data.Entity.EntityState.Modified;
                    exist.SettingId = item.SettingId;
                    exist.Enabled = item.Enabled;
                    exist.Seq = item.Seq;
                    exist.Url = item.Url;
                }
            }

            _context.SaveChanges();
        }
    }
}
