using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.IService.EvaluateAbout;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Service.EvaluateAbout
{
    public class EvaluateReviseService : RepositoryFactory<EvaluateReviseEntity>, IEvaluateReviseService
    {


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public List<EvaluateReviseEntity> GetPageList(Pagination pagination, string queryJson)
        {
            //var expression = LinqExtensions.True<EvaluateReviseEntity>();
            var queryParam = queryJson.ToJObject();
            var query = BaseRepository().IQueryable();
            //查询条件
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                //pagination.conditionJson += string.Format(" and EvaluateId like '%{0}%'", keyword.Trim());
                query = query.Where(x => x.EvaluateId.Contains(keyword.Trim()));
            }
            //DatabaseType dataType = DbHelper.DbType;
            //return this.BaseRepository().FindTableByProcPager(pagination, dataType);
            int count = 0;
            var data = DataHelper.DataPaging(pagination.rows, pagination.page, query.OrderByDescending(x => x.CreateDate), out count);
            pagination.records = count;
            return data;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public List<EvaluateReviseEntity> GetPagesList(Pagination pagination, string queryJson)
        {
            var db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<EvaluateReviseEntity>();
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                expression = expression.And(x => x.EvaluateId.Contains(keyword));
            }
            var data = db.IQueryable(expression).OrderByDescending(x => x.CreateDate);
            pagination.records = data.Count();
            return data.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
        }

        /// <summary>
        /// 根据前后打分记录，添加修正记录
        /// </summary>
        /// <param name="oldMarks">修正前</param>
        /// <param name="newMarks">修正后</param>
        public void Insert(EvaluateMarksRecordsEntity oldMarks, EvaluateMarksRecordsEntity newMarks)
        {
            var db = new RepositoryFactory().BaseRepository();
            //Operator user = OperatorProvider.Provider.Current();
            EvaluateItemEntity evaluateItemEntity = null;
            evaluateItemEntity = db.FindEntity<EvaluateItemEntity>(oldMarks.EvaluateItemId);
            EvaluateCategoryItemEntity categoryItemEntity = null;
            categoryItemEntity = db.FindEntity<EvaluateCategoryItemEntity>(evaluateItemEntity.EvaluateContentId);
            EvaluateCategoryEntity categoryEntity = null;
            categoryEntity = db.FindEntity<EvaluateCategoryEntity>(categoryItemEntity.CategoryId);
            EvaluateGroupEntity evaluateGroup = null;
            evaluateGroup = db.FindEntity<EvaluateGroupEntity>(evaluateItemEntity.EvaluateGroupId);
            EvaluateEntity evaluate = null;
            evaluate = db.FindEntity<EvaluateEntity>(evaluateGroup.EvaluateId);

            EvaluateReviseEntity reviseEntity = new EvaluateReviseEntity()
            {
                Id = Guid.NewGuid().ToString(),
                Category = categoryEntity?.Category,
                CategoryId = categoryEntity?.CategoryId,
                CreateDate = DateTime.Now,
                CreateUser = newMarks.CreateUserName,
                CreateUserId = newMarks.CreateUserId,
                DepartmentId = evaluateGroup?.DeptId,
                DepartmentName = evaluateGroup?.DeptName,
                DeptCause = oldMarks.Cause,
                DeptEvaluateUserId = oldMarks.CreateUserId,
                DeptEvaluteUser = oldMarks.CreateUserName,
                DeptScore = oldMarks.Score,
                EvaluteContent = evaluateItemEntity?.EvaluateContent,
                EvaluteContentId = evaluateItemEntity?.EvaluateContentId,
                GroupId = evaluateGroup?.GroupId,
                GroupName = evaluateGroup?.GroupName,
                ReviseCause = newMarks.Cause,
                ReviseScore = newMarks.Score,
                ReviseUser = newMarks.CreateUserName,
                ReviseUserId = newMarks.CreateUserId,
                StandardScore = evaluateItemEntity.Score,
                EvaluateId = evaluate.EvaluateId,
                EvaluateSeason = evaluate.EvaluateSeason
            };
            this.BaseRepository().Insert(reviseEntity);
        }
        /// <summary>
        /// 如果是公司级删除的部门级的数据，则会新增一条，否则的话就不会新增
        /// </summary>
        /// <param name="entity">所删除的部门级的数据</param>
        public void Insert(string deptid, EvaluateMarksRecordsEntity entity)
        {
            var db = new RepositoryFactory().BaseRepository();
            //Operator user = OperatorProvider.Provider.Current();
            DepartmentEntity userDept = db.FindEntity<DepartmentEntity>(deptid);
            if (entity.IsOrg == 0 && userDept.IsSpecial)//如果数据时部门级的，但是删除的用户的是部门级的，则添加一条记录
            {
                EvaluateItemEntity evaluateItemEntity = null;
                evaluateItemEntity = db.FindEntity<EvaluateItemEntity>(entity.EvaluateItemId);
                EvaluateCategoryItemEntity categoryItemEntity = null;
                categoryItemEntity = db.FindEntity<EvaluateCategoryItemEntity>(evaluateItemEntity.EvaluateContentId);
                EvaluateCategoryEntity categoryEntity = null;
                categoryEntity = db.FindEntity<EvaluateCategoryEntity>(categoryItemEntity.CategoryId);
                EvaluateGroupEntity evaluateGroup = null;
                evaluateGroup = db.FindEntity<EvaluateGroupEntity>(evaluateItemEntity.EvaluateGroupId);
                EvaluateEntity evaluate = null;
                evaluate = db.FindEntity<EvaluateEntity>(evaluateGroup.EvaluateId);

                EvaluateReviseEntity reviseEntity = new EvaluateReviseEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    Category = categoryEntity?.Category,
                    CategoryId = categoryEntity?.CategoryId,
                    CreateDate = DateTime.Now,
                    CreateUser = entity.CreateUserName,
                    CreateUserId = entity.CreateUserId,
                    DepartmentId = evaluateGroup?.DeptId,
                    DepartmentName = evaluateGroup?.DeptName,
                    DeptCause = entity.Cause,
                    DeptEvaluateUserId = entity.CreateUserId,
                    DeptEvaluteUser = entity.CreateUserName,
                    DeptScore = entity.Score,
                    EvaluteContent = evaluateItemEntity?.EvaluateContent,
                    EvaluteContentId = evaluateItemEntity?.EvaluateContentId,
                    GroupId = evaluateGroup?.GroupId,
                    GroupName = evaluateGroup?.GroupName,
                    ReviseCause = "该打分项在 " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " 被 " + entity.CreateUserName + " 删除",
                    ReviseUser = entity.CreateUserName,
                    ReviseUserId = entity.CreateUserId,
                    StandardScore = evaluateItemEntity.Score,
                    IsDeleteType = 1,//删除数据
                    EvaluateId = evaluate.EvaluateId,
                    EvaluateSeason = evaluate.EvaluateSeason

                };
                this.BaseRepository().Insert(reviseEntity);
            }
        }

        /// <summary>
        /// 获取所有的考评列表，绑定搜索 下拉框使用
        /// </summary>
        /// <returns></returns>
        public List<EvaluateEntity> BindCombobox()
        {
            var data = new RepositoryFactory().BaseRepository().IQueryable<EvaluateEntity>().ToList();
            return data;
        }
    }
}
