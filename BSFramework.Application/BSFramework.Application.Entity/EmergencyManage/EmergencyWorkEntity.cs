using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EmergencyManage
{
    [Table("wg_emergencywork")]
    public class EmergencyWorkEntity : BaseEntity
    {
        [Column("emergencyid")]
        public String EmergencyId { get; set; }
        [Column("name")]
        public String Name { get; set; }
        [Column("emergencytype")]
        public String EmergencyType { get; set; }
        [Column("emergencytypeid")]
        public String EmergencyTypeId { get; set; }
        [Column("tocompileuser")]
        public String ToCompileUser { get; set; }
        [Column("tocompileuserid")]
        public String ToCompileUserid { get; set; }
        [Column("tocompiledept")]
        public String ToCompileDept { get; set; }
        [Column("tocompiledeptid")]
        public String ToCompileDeptId { get; set; }
        
        [Column("tocompiledate")]
        public DateTime ToCompileDate { get; set; }
        [Column("attachment")]
        public String Attachment { get; set; }
        [Column("attachmentid")]
        public String AttachmentId { get; set; }
        
        [Column("purpose")]
        public String Purpose { get; set; }    
        [Column("emergencyplan")]
        public String EmergencyPlan { get; set; }
        [Column("rehearsedate")]
        public DateTime RehearseDate { get; set; }
        [Column("rehearseplace")]
        public String RehearsePlace { get; set; }
        [Column("rehearsetype")]
        public String RehearseType { get; set; }
        [Column("rehearsetypeid")]
        public String RehearseTypeId { get; set; }
        [Column("rehearsescenario")]
        public String RehearseScenario { get; set; }
        [Column("rehearsename")]
        public string RehearseName { get; set; }
        [Column("mainpoints")]
        public String MainPoints { get; set; }
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
            if (string.IsNullOrEmpty(this.EmergencyId))
            {
                this.EmergencyId = Guid.NewGuid().ToString();
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
            this.EmergencyId = keyValue;
            this.MODIFYDATE = DateTime.Now;
            this.MODIFYUSERID = OperatorProvider.Provider.Current().UserId;
            this.MODIFYUSERNAME = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
