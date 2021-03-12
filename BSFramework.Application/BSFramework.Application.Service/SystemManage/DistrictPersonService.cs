using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PeopleManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.IService.SystemManage;
using BSFramework.Data.EF;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Service.SystemManage
{
    /// <summary>
    /// 区域责任人
    /// </summary>
    public class DistrictPersonService : IDistrictPersonService
    {
        private System.Data.Entity.DbContext _context;

        public DistrictPersonService()
        {
            _context = (DbFactory.Base() as Database).dbcontext;
        }

        public List<UserEntity> GetDistrictPerson(string districtId)
        {
            var query = from q1 in _context.Set<DistrictPersonEntity>()
                        join q2 in _context.Set<UserEntity>() on q1.DutyUserId equals q2.UserId into into2
                        from q2 in into2.DefaultIfEmpty()
                        where q1.DistrictId == districtId
                        select new { q1.DutyUserId, q1.DutyUser, q2.HeadIcon, q1.DutyDepartmentName, q2.DutyName, q1.Phone, q1.CategoryName };

            return query.ToList().Select(x => new UserEntity
            {
                UserId = x.DutyUserId,
                RealName = x.DutyUser,
                HeadIcon = x.HeadIcon,
                DepartmentName = x.DutyDepartmentName,
                DutyName = x.DutyName,
                Telephone = x.Phone,
                PostName = x.CategoryName
            }).ToList();
        }

        public List<DistrictPersonEntity> GetDistricts()
        {
            var query = from q in _context.Set<DistrictPersonEntity>()
                        select new { q.DistrictId, q.DistrictCode, q.DistrictName, q.CompanyName };

            return query.Distinct().ToList().Select(x => new DistrictPersonEntity { DistrictId = x.DistrictId, DistrictCode = x.DistrictCode, DistrictName = x.DistrictName, CompanyName = x.CompanyName }).ToList();
        }

        public List<DistrictPersonEntity> GetList(string districtCode, string districtName, string category, string person, int rows, int page, out int total)
        {
            var query = _context.Set<DistrictPersonEntity>().AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(districtCode)) query = query.Where(x => x.DistrictCode.Contains(districtCode));
            if (!string.IsNullOrEmpty(districtName)) query = query.Where(x => x.DistrictName.Contains(districtName));
            if (!string.IsNullOrEmpty(category)) query = query.Where(x => x.CategoryName.Contains(category));
            if (!string.IsNullOrEmpty(person)) query = query.Where(x => x.DutyUser.Contains(person));

            total = query.Count();
            return query.OrderBy(x => x.DistrictCode).ThenBy(x => x.CategoryName).ThenBy(x => x.CreateDate).Skip(rows * (page - 1)).Take(rows).ToList();
        }

        public List<DistrictPersonEntity> GetList(string id)
        {
            var query = from q in _context.Set<DistrictPersonEntity>()
                        where q.DistrictId == id
                        select q;

            return query.OrderBy(x => x.DistrictCode).ThenBy(x => x.CategoryName).ThenBy(x => x.CreateDate).ToList();
        }

        public void Remove(string id)
        {
            var entity = _context.Set<DistrictPersonEntity>().Find(id);
            if (entity != null)
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Deleted;
                _context.SaveChanges();
            }
        }

        public void Save(List<DistrictPersonEntity> entities)
        {
            foreach (var item in entities)
            {
                var entity = _context.Set<DistrictPersonEntity>().Find(item.DistrictPersonId);
                if (entity == null)
                    _context.Set<DistrictPersonEntity>().Add(item);
                else
                {
                    _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    entity.CategoryId = item.CategoryId;
                    entity.CategoryName = item.CategoryName;
                    entity.DistrictId = item.DistrictId;
                    entity.DistrictCode = item.DistrictCode;
                    entity.DistrictName = item.DistrictName;
                    entity.DutyDepartmentId = item.DutyDepartmentId;
                    entity.DutyDepartmentName = item.DutyDepartmentName;
                    entity.DutyUserId = item.DutyUserId;
                    entity.DutyUser = item.DutyUser;
                    entity.Phone = item.Phone;
                    entity.Cycle = item.Cycle;
                }
            }
            _context.SaveChanges();
        }
    }
}
