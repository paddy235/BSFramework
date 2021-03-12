using BSFramework.Application.Entity.EducationManage;
using BSFramework.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.IService.EducationManage;
namespace BSFramework.Application.Service.EducationManage
{
    public class EduPlanInfoService : RepositoryFactory<EduPlanInfoEntity>, IEduPlanInfoService
    {
        public IEnumerable<EduPlanInfoEntity> GetPlanInfoList(string planid) 
        {
            var query = this.BaseRepository().IQueryable();//.Where(x => x.PlanId == planid);
            return query.ToList();
        }

        public IEnumerable<EduPlanInfoEntity> GetPlanInfoList(string edutype,string verifyhtml,string month,string year,string state,string txt_Keyword,string deptCode,int page,int pagesize,out int total) {
            var query = this.BaseRepository().IQueryable();

            if (!string.IsNullOrEmpty(edutype))
            {
                if (edutype != "0")
                    query = query.Where(x => x.TrainType == edutype);
            }
            if (!string.IsNullOrEmpty(verifyhtml))
            {
                query = query.Where(x => x.VerifyState == "待审核");

            }
            if (!string.IsNullOrEmpty(month))
            {
                if (month != "0")
                {
                    query = query.Where(x => x.TrainDateMonth == month);

                }
            }
            if (!string.IsNullOrEmpty(year))
            {
                query = query.Where(x => x.TrainDateYear == year);
            }
            if (!string.IsNullOrEmpty(state))
            {
                query = query.Where(x => x.workState == state);
            }
            if (!string.IsNullOrEmpty(txt_Keyword))
            {
                query = query.Where(x => x.TrainProject.Contains(txt_Keyword));
            }
            if (!string.IsNullOrEmpty(deptCode))
            {
                query = query.Where(x => !string.IsNullOrEmpty(x.GroupCode));
                query = query.Where(x => x.GroupCode.StartsWith(deptCode));
            }
            total = query.Count();
            query = query.OrderByDescending(x => x.CreateDate).Skip(pagesize * (page - 1)).Take(pagesize);
            return query.ToList();
        }

        public EduPlanInfoEntity GetPlanInfoEntity(string keyValue) 
        {
            return this.BaseRepository().FindEntity(keyValue);
        }

        public void RemoveEduPlanInfo(string keyValue) 
        {
            this.BaseRepository().Delete(keyValue);
        }


        public void SaveEduPlanInfo(string keyValue, EduPlanInfoEntity entity)
        {
            var entity1 = this.GetPlanInfoEntity(keyValue);
            if (entity1 == null)
            {

                this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.Files = null;
                entity.Files1 = null;
                entity.CreateDate = entity1.CreateDate;
                entity.createDeptid = entity1.createDeptid;
                entity.createDeptName = entity1.createDeptName;
                entity.CreateUser = entity1.CreateUser;
                entity.CreateUserId = entity1.CreateUserId;
                //entity1.Files = null;
                //entity.Files = null;
                //entity1.TrainDate = entity.TrainDate;
                //entity1.TrainContent = entity.TrainContent;
                //entity1.TrainProject = entity.TrainProject;
                //entity1.TrainTarget = entity.TrainTarget;
                //entity1.TrainType = entity.TrainType;
                //entity1.TrainUserId = entity.TrainUserId;
                //entity1.TrainUserName = entity.TrainUserName;
                //entity1.Remark = entity.Remark;
                this.BaseRepository().Update(entity);
            }
        }
    }
}
