using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 描 述：班组台
    /// </summary>
    public class OrderinfoEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string CreateUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DeptCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string CreateUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 关联安全日记录Id
        /// </summary>
        /// <returns></returns>
        public string SdId { get; set; }
        /// <summary>
        /// 班组名称
        /// </summary>
        /// <returns></returns>
        public string GroupName { get; set; }
        /// <summary>
        /// 班组Id
        /// </summary>
        /// <returns></returns>
        public string GroupId { get; set; }
        /// <summary>
        /// 预约人Id
        /// </summary>
        /// <returns></returns>
        public string OrderUserId { get; set; }
        /// <summary>
        /// 是否预约（0:未预约，1：已预约）
        /// </summary>
        /// <returns></returns>
        public int IsOrder { get; set; }
        /// <summary>
        /// 预约人姓名
        /// </summary>
        /// <returns></returns>
        public string OrderUserName { get; set; }
        /// <summary>
        /// 预约时间
        /// </summary>
        /// <returns></returns>
        public DateTime? OrderDate { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
