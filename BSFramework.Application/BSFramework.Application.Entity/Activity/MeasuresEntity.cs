using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 描 述：危险预知训练-危险控制措施
    /// </summary>
    public class MeasuresEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Id { get; set; }
        /// <summary>
        /// 责任人Id
        /// </summary>
        /// <returns></returns>
        public string UserId { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        /// <returns></returns>
        public string DutyMan { get; set; }
        /// <summary>
        /// 采取的安全防范措施
        /// </summary>
        /// <returns></returns>
        public string Measure { get; set; }
        /// <summary>
        /// 潜在的危险因素及其后果
        /// </summary>
        /// <returns></returns>
        public string DangerSource { get; set; }
        /// <summary>
        /// 落实情况（0：未完成，1：已完成）
        /// </summary>
        /// <returns></returns>
        public string IsOver { get; set; }
        /// <summary>
        /// 关联危险训练记录Id
        /// </summary>
        /// <returns></returns>
        public string DangerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? CreateDate { get; set; }
        public string CreateUserId { get; set; }
        [NotMapped]
        public int? State { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToString() : Id;
            this.CreateDate = DateTime.Now;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
 
        }
        #endregion
    }
}
