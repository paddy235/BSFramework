using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WorkMeeting
{
    /// <summary>
    /// 描 述：排班关联部门
    /// </summary>
    [Table("wg_workorder")]
    public class WorkGroupSetEntity : BaseEntity
    {
        [Column("workgroupid")]
        public string workgroupid { get; set; }
        [Column("departmentid")]
        public string departmentid { get; set; }
        [Column("fullname")]
        public string fullname { get; set; }

        [Column("groupsort")]
        public int groupsort { get; set; }
        [Column("workorderid")]
        public string workorderid { get; set; }
        [Column("createtime")]
        public DateTime CreateTime { get; set; }
        [Column("createuserid")]
        public string createuserid { get; set; }

        [Column("bookmarks")]
        public string bookmarks { get; set; }
        
        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.workgroupid = Guid.NewGuid().ToString();
       
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.workgroupid = keyValue;

        }
        #endregion
    }
}
