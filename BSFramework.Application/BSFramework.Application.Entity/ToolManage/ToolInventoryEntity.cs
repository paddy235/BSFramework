using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.ToolManage
{
    [Table("wg_toolinventory")]
    public class ToolInventoryEntity : BaseEntity
    {
        /// <summary>
        /// 所属部门code  区分电厂数据
        /// </summary>
        [Column("DeptCode")]
        public String DeptCode { get; set; }
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Column("NAME")]
        public String Name { get; set; }

        [Column("TYPE")]
        public String Type { get; set; }
        [Column("SPEC")]
        public String Spec { get; set; }
        [Column("INFO")]
        public String Info { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        [Column("PATH")]
        public String Path { get; set; }


        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("DEPTID")]
        public String DeptId { get; set; }

        [Column("BZID")]
        public String BZId { get; set; }

        [Column("File")]
        public String File { get; set; }

        [Column("Video")]
        public String Video { get; set; }
    }
}
