
namespace BSFramework.Application.Entity
{
    /// <summary>
    /// 实体类基类
    /// </summary>
    public class BaseEntity
    {
        ///// <summary>
        ///// 创建人所在的部门编码
        ///// </summary>
        //public virtual string CreateUserDeptCode { get; set; } 
        
        /// <summary>
        /// 新增调用
        /// </summary>
        public virtual void Create()
        {
        }
        /// <summary>
        /// 新增调用
        /// </summary>
        public virtual void CreateApp()
        {
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue">主键值</param>
        public virtual void Modify(string keyValue)
        {
        }
        /// <summary>
        /// 删除调用
        /// </summary>
        /// <param name="keyValue">主键值</param>
        public virtual void Remove(string keyValue)
        {
        }
    }
}
