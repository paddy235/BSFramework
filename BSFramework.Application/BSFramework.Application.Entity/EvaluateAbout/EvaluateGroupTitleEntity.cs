using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EvaluateAbout
{
    [Table("WG_EvaluateGroupTitle")]
   public class EvaluateGroupTitleEntity :BaseEntity
    {
        /// <summary>
        /// 班组ID
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string TitleId { get; set; }
        public string CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        /// <summary>
        ///EvaluateId
        /// </summary>
        [Column("BK1")]
        public string EvaluateId { get; set; }
        /// <summary>
        /// 称号ID ，称号表 wg_Designation
        /// </summary>
        [Column("BK2")]
        public string TId { get; set; }
        public DateTime ModifyDate { get; set; }
        public DateTime CreateDate { get; set; }
        public override void Create()
        {
            if (string.IsNullOrEmpty(this.GroupId))
            {
                throw new Exception("班组ID是主键不能为空");
            }
            else
            {
                this.TitleId = Guid.NewGuid().ToString();
                this.CreateDate = DateTime.Now;
                this.CreateUserId = OperatorProvider.Provider.Current().UserId;
                this.CreateUserName = OperatorProvider.Provider.Current().UserName;
                this.ModifyDate = DateTime.Now;
            }
        }

        public override void Modify(string keyValue)
        {
            this.GroupId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
        }
    }
}
