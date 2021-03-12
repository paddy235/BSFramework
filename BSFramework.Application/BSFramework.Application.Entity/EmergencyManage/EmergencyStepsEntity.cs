using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EmergencyManage
{
    [Table("wg_emergencysteps")]
    public class EmergencyStepsEntity : BaseEntity
    {
        [Column("emergencystepsid")]
        public String EmergencyStepsId { get; set; }
        [Column("emergencyid")]
        public String EmergencyId { get; set; }
        [Column("emergencycontext")]
        public String EmergencyContext { get; set; }
        [Column("emergencysort")]
        public int EmergencySort { get; set; }
        [Column("emergencyuser")]
        public String EmergencyUser { get; set; }
        [Column("emergencyuserid")]
        public String EmergencyUserid { get; set; }
        [Column("CREATEDATE")]
        public DateTime CREATEDATE { get; set; }
        [Column("CREATEUSERID")]
        public String CREATEUSERID { get; set; }
        [Column("CREATEUSERNAME")]
        public String CREATEUSERNAME { get; set; }
        [Column("MODIFYDATE")]
        public DateTime MODIFYDATE { get; set; }
        [Column("MODIFYUSERID")]
        public String MODIFYUSERID { get; set; }
        [Column("MODIFYUSERNAME")]
        public String MODIFYUSERNAME { get; set; }
        [NotMapped]
        public string ImplementStep { get; set; }
        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
          
            if (string.IsNullOrEmpty(this.EmergencyStepsId))
            {
                this.EmergencyStepsId = Guid.NewGuid().ToString();
            }
            this.CREATEDATE = DateTime.Now;
            this.CREATEUSERID = OperatorProvider.Provider.Current().UserId;
            this.CREATEUSERNAME = OperatorProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.EmergencyStepsId = keyValue;
            this.MODIFYDATE = DateTime.Now;
            this.MODIFYUSERID = OperatorProvider.Provider.Current().UserId;
            this.MODIFYUSERNAME = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
