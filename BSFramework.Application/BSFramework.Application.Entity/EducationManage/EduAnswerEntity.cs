using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
    [Table("WG_EDUANSWER")]
    public class EduAnswerEntity
    {
        public EduAnswerEntity()
        {
            this.Files = new List<FileInfoEntity>();
        }
        [Column("ID")]
        public String ID { get; set; }

        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("CREATEUSER")]
        public String CreateUser { get; set; }

        /// <summary>
        /// 题目
        /// </summary>
        [Column("QUESTION")]
        public String Question { get; set; }
        /// <summary>
        /// 教育培训id
        /// </summary>
        [Column("EDUID")]
        public String EduId { get; set; }
        /// <summary>
        /// 答题人
        /// </summary>
        [Column("ANSWERPEOPLE")]
        public String AnswerPeople { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ANSWERPEOPLEID")]
        public String AnswerPeopleId { get; set; }
        /// <summary>
        /// 答题内容（采取措施描述）
        /// </summary>
        [Column("ANSWERCONTENT")]
        public String AnswerContent { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [Column("GRADE")]
        public String Grade { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Column("APPRAISECONTENT")]
        public String AppraiseContent { get; set; }

        /// <summary>
        /// 事故现象描述
        /// </summary>
        [Column("DESCRIPTION")]
        public String Description { get; set; }
        [Column("REASON")]
        public String Reason { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files1 { get; set; }
    }

    public class MainCount
    {
        public int BZ_BQBHH { get; set; }
        public int BZ_WCRW { get; set; }

        public int BZ_SYRW { get; set; }
        public int BZ_RWWCL { get; set; }
        public int BZ_AQRHD { get; set; }
        public int BZ_JSJK { get; set; }
        public int BZ_JSWD { get; set; }
        public int BZ_SGYX { get; set; }
        public int BZ_FSGXX { get; set; }
        public int BZ_ZZXX { get; set; }

        public int BZ_LDBHJC { get; set; }
        public int BZ_BWH { get; set; }
        public int BZ_MZGLH { get; set; }


        public int BZ_JNJL { get; set; }
        public int BZ_SBFX { get; set; }
        public int BZ_YXFX { get; set; }

        public int BZ_LSZC { get; set; }
        public int BZ_DDZP { get; set; }
        public int BZ_JXSBBZ { get; set; }


        public int BZ_QCCG { get; set; }
        public int BZ_GLCXCG { get; set; }
        public int BZ_7SCXTA { get; set; }

        public int BZ_WXYZXL { get; set; }
        public int BZ_ZDXX { get; set; }

        public int BZ_RSFXYK { get; set; }
        public int BZ_AQXXR { get; set; }
    }
}
