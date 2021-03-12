using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.EvaluateAbout;
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


namespace BSFramework.Application.Service.EvaluateAbout
{
    /// <summary>
    /// 班组分类管理
    /// </summary>
    public class EvaluateSetService : RepositoryFactory<EvaluateSetEntity>, IEvaluateSetService
    {
        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<EvaluateSetEntity> GetList(string queryJson)
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
                string ClassName = queryParam["keyword"].ToString().Trim();
                pagination.conditionJson += string.Format(" and ClassName like '%{0}%'", ClassName);
            }

            DataTable dt = this.BaseRepository().FindTableByProcPager(pagination, DbHelper.DbType);
            return dt;
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public List<EvaluateSetEntity> GetPagesList(Pagination pagination, string queryJson)
        {
            var db = new RepositoryFactory().BaseRepository();
            var expression = LinqExtensions.True<EvaluateSetEntity>();
            var queryParam = queryJson.ToJObject();
            //查询条件
            if (!queryParam["keyword"].IsEmpty())
            {
                string keyword = queryParam["keyword"].ToString();
                expression = expression.And(x => x.ClassName.Contains(keyword));
            }
            var data = db.IQueryable(expression).OrderByDescending(x => x.CreateDate);
            pagination.records = data.Count();
            return data.Skip(pagination.rows * (pagination.page - 1)).Take(pagination.rows).ToList();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public EvaluateSetEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        //错误方法 未使用
        public List<EvaluateSetEntity> GetIdEntityList(string keyValue)
        {
            //if (!string.IsNullOrEmpty(keyValue) && keyValue.Length > 0)
            //{
            //    keyValue.TrimEnd(',');
            //    string sql = " SELECT Id,CreateDate,CreateUserId,CreateUserName,ModifyDate,ModifyUserId,ModifyUserName,DeptCode,`Subject`,`Explain`,DeptId,DeptName,Remark,ActIds,activitytype from wg_safetyday where Id in ('" + keyValue.Replace(",", "','") + "');";
            //    return this.BaseRepository().FindList(sql).ToList();
            //}
            return new List<EvaluateSetEntity>();
        }

     
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            EvaluateSetEntity entity = this.BaseRepository().FindEntity(keyValue);
            try
            {
                if (entity != null)
                {
                    this.BaseRepository().Delete(keyValue);
                }
            }
            catch (Exception er)
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
        public void SaveForm(string keyValue, EvaluateSetEntity entity)
        {
            IRepository db = new RepositoryFactory().BaseRepository().BeginTrans();
            try
            {
                entity = IsCheckClass(keyValue,entity);
                if (string.IsNullOrEmpty(keyValue))
                {
                    entity.Create();
                    db.Insert(entity);
                }
                else
                {
                    entity.Modify(keyValue);
                    db.Update(entity);
                }
                db.Commit();

            }
            catch (Exception er)
            {
                db.Rollback();
                throw;
            }
        }



        /// <summary>
        /// 获取已分配所以班组信息
        /// </summary>
        /// <param name="DeptId"></param>
        /// <returns></returns>
        public List<string> GetDeptIdAll(string keyValue)
        {
            try
            {
                List<string> DeptList = new List<string>();
                List<EvaluateSetEntity> list = this.BaseRepository().IQueryable().ToList();
                foreach (var Rows in list)
                {
                    if (!string.IsNullOrEmpty(keyValue) && !Rows.Id.Contains(keyValue))
                    {
                        DeptList.AddRange(Rows.DeptId.Split(','));
                    }
                }
                return DeptList;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// 筛选掉已分配的班组
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public EvaluateSetEntity IsCheckClass(string keyvalue,EvaluateSetEntity entity)
        {
            string Id = string.Empty;
            string name = string.Empty;
            List<string> DeptList = this.GetDeptIdAll(keyvalue);
            if (entity.DeptId != null)
            {
                string[] list = entity.DeptId.Split(',');
                string[] NameList = entity.DeptName.Split(',');
                for (int i = 0; i < list.Length; i++)
                {
                    var Rows = list[i];
                    if (string.IsNullOrEmpty(Rows)) continue;
                    if (!DeptList.Contains(Rows))
                    {
                        if (string.IsNullOrEmpty(Id))
                        {
                            Id = Rows;
                            name = NameList[i];
                        }
                        else
                        {
                            Id += "," + Rows;
                            name += "," + NameList[i];
                        }
                    }
                }

            }
            entity.DeptName = name;
            entity.DeptId = Id;
            return entity;
        }


      
       
        #endregion
    }
}
