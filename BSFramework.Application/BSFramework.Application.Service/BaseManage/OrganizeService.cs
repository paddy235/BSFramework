using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Data.Repository;
using BSFramework.Util.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BSFramework.Application.Service.BaseManage
{
    /// <summary>
    /// 描 述：机构管理
    /// </summary>
    public class OrganizeService : RepositoryFactory<OrganizeEntity>, IOrganizeService
    {
        #region 获取数据
        /// <summary>
        /// 机构列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrganizeEntity> GetList()
        {
            return this.BaseRepository().IQueryable().OrderBy(x => x.EnCode).ToList();
        }
        /// <summary>
        /// 机构实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public OrganizeEntity GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity(keyValue);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 公司名称不能重复
        /// </summary>
        /// <param name="organizeName">公司名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            var expression = LinqExtensions.True<OrganizeEntity>();
            expression = expression.And(t => t.FullName == fullName);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.OrganizeId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        /// <summary>
        /// 外文名称不能重复
        /// </summary>
        /// <param name="enCode">外文名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            var expression = LinqExtensions.True<OrganizeEntity>();
            expression = expression.And(t => t.EnCode == enCode);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.OrganizeId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        /// <summary>
        /// 中文名称不能重复
        /// </summary>
        /// <param name="shortName">中文名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistShortName(string shortName, string keyValue)
        {
            var expression = LinqExtensions.True<OrganizeEntity>();
            expression = expression.And(t => t.ShortName == shortName);
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.OrganizeId != keyValue);
            }
            return this.BaseRepository().IQueryable(expression).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除机构
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            int count = this.BaseRepository().IQueryable(t => t.ParentId == keyValue).Count();
            if (count > 0)
            {
                throw new Exception("当前所选数据有子节点数据！");
            }
            this.BaseRepository().Delete(keyValue);
        }
        /// <summary>
        /// 保存机构表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="organizeEntity">机构实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, OrganizeEntity organizeEntity)
        {
            //更新
            if (!string.IsNullOrEmpty(keyValue))
            {
                OrganizeEntity curEntity = this.BaseRepository().FindEntity(keyValue);  //获取父对象
                if (curEntity.OrganizeId != organizeEntity.OrganizeId || string.IsNullOrEmpty(curEntity.EnCode))
                {
                    organizeEntity.EnCode = GetOrganizeCode(organizeEntity);
                }
                organizeEntity.Modify(keyValue);
                this.BaseRepository().Update(organizeEntity);
            }
            else //新增
            {
                organizeEntity.EnCode = GetOrganizeCode(organizeEntity);
                organizeEntity.Create();
                this.BaseRepository().Insert(organizeEntity);
            }
        }
        #endregion


        /// <summary>
        /// 根据当前机构获取对应的机构代码  机构代码 2-6-8-10  位
        /// </summary>
        /// <param name="organizeEntity"></param>
        /// <returns></returns>
        public string GetOrganizeCode(OrganizeEntity organizeEntity)
        {
            string maxCode = string.Empty;



            //查询是否存在平级机构
            var maxObj = BaseRepository().IQueryable(x =>  x.ParentId == organizeEntity.ParentId && !string.IsNullOrWhiteSpace(x.EnCode)).OrderByDescending(x => x.EnCode).ToList();
            //var maxObj = this.BaseRepository().FindList(string.Format("select t.*  from BASE_ORGANIZE t where t.parentid ='{0}' and  encode is not null order by encode desc", organizeEntity.ParentId));

            //确定是否存在上级机构,非根节点
            if (organizeEntity.ParentId != "0")
            {
                //存在，则取编码最大的那一个
                if (maxObj.Count() > 0)
                {
                    maxCode = maxObj.FirstOrDefault().EnCode;  //获取最大的Code 
                    if (!string.IsNullOrEmpty(maxCode))
                    {
                        maxCode = QueryOrganizeCodeByCondition(maxCode);
                    }
                }
                else
                {
                    OrganizeEntity parentEntity = this.BaseRepository().FindEntity(organizeEntity.ParentId);  //获取父对象

                    maxCode = parentEntity.EnCode + "001";  //固定值,非可变
                }
            }
            else  //根节点的操作
            {
                //do somethings 
                if (maxObj.Count() > 0)
                {
                    maxCode = maxObj.FirstOrDefault().EnCode;  //获取最大的Code 
                    if (!string.IsNullOrEmpty(maxCode))
                    {
                        maxCode = QueryOrganizeCodeByCondition(maxCode);
                    }
                    else
                    {
                        maxCode = "00";  //固定值,非可变
                    }
                }
                else
                {
                    maxCode = "00";  //固定值,非可变
                }
            }
            //先判断当前编码是否存在于数据库表中
            return maxCode;
        }


        public string QueryOrganizeCodeByCondition(string code)
        {
            string newCode = string.Empty;

            string maxValue = (Convert.ToDecimal(code) + 1).ToString();

            for (int i = 1; i <= 30; i++)
            {
                if (maxValue.ToString().Length == i)
                {
                    newCode = code.Substring(0, code.Length - i) + maxValue;
                    break;
                }
            }
            return newCode;
        }
    }
}
