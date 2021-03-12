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
    [Table("WG_WORKTIMESORT")]
    public class WorkTimeSortEntity : BaseEntity
    {

        [Column("WORKTIMESORTID")]
        public string worktimesortid { get; set; }
        [Column("YEAR")]
        public int year { get; set; }
        [Column("MONTH")]
        public int month { get; set; }
        [Column("TIMEDATA")]
        public string timedata { get; set; }
        [Column("DEPARTMENTID")]
        public string departmentid { get; set; }
        [Column("DEPTCODE")]
        public string deptcode { get; set; }
        [Column("FULLNAME")]
        public string fullname { get; set; }
        [Column("SETSORT")]
        public int setsort { get; set; }
        [Column("TIMEDATAID")]
        public string timedataid { get; set; }
        [NotMapped]
        public string datatype { get; set; }

    }


}
