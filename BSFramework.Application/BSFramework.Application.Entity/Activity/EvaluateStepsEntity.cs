using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 评价流程设置
    /// </summary>
    [Table("wg_evaluatesteps")]
    public class EvaluateStepsEntity : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("Id")]
        public string Id { get; set; }
        /// <summary>
        /// 模块
        /// </summary>
        [Column("module")]
        public string module { get; set; }
        /// <summary>
        /// 模块
        /// </summary>
        [Column("modulename")]
        public string modulename { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [Column("deptname")]
        public string deptname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("deptid")]
        public string deptid { get; set; }
        /// <summary>
        /// 是否部门
        /// </summary>
        [Column("isdept")]
        public bool isdept { get; set; }
        /// <summary>
        /// 是否班组
        /// </summary>
        [Column("isgroup")]
        public bool isgroup { get; set; }
        /// <summary>
        /// 用户角色
        /// </summary>
        [Column("userrole")]
        public string userrole { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        [Column("userroleid")]
        public string userroleid { get; set; }
        /// <summary>
        /// 用户岗位    
        /// </summary>
        [Column("userjobs")]
        public string userjobs { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Column("sort")]
        public int? sort { get; set; }
        /// <summary>
        /// 是否专业工种
        /// </summary>
        [Column("isprofessional")]
        public bool isprofessional { get; set; }
        /// <summary>
        /// 评价排序
        /// </summary>
        [Column("evaluatesort")]
        public int evaluatesort { get; set; }
        [Column("CREATEDATE")]
        public DateTime createdate { get; set; }
        [Column("CREATEUSERID")]
        public String createuserid { get; set; }
        [Column("CREATEUSERNAME")]
        public String createusername { get; set; }
        [Column("MODIFYDATE")]
        public DateTime modifydate { get; set; }
        [Column("MODIFYUSERID")]
        public String modifyuserid { get; set; }
        [Column("MODIFYUSERNAME")]
        public String modifyusername { get; set; }
        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = Guid.NewGuid().ToString();
            this.createdate = DateTime.Now;
            this.createuserid = OperatorProvider.Provider.Current().UserId;
            this.createusername = OperatorProvider.Provider.Current().UserName;
           
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
            this.modifydate = DateTime.Now;
            this.modifyuserid = OperatorProvider.Provider.Current().UserId;
            this.modifyusername = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
