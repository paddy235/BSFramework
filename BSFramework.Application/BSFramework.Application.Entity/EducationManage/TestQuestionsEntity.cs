using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
    /// <summary>
    /// 教育培训试题
    /// </summary>
    [Table("wg_testquestions")]
    public class TestQuestionsEntity
    {
        /// <summary>
        /// id
        /// </summary>
        [Column("questionsid")]
        public String Questionsid { get; set; }
        /// <summary>
        /// 试题内容
        /// </summary>
        [Column("Theme")]
        public String Theme { get; set; }
        /// <summary>
        /// 参考答案
        /// </summary>
        [Column("answer")]
        public String Answer { get; set; }
    }
}
