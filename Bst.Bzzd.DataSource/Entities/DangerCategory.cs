using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("WG_DANGERCATEGORY")]
    public class DangerCategory
    {
        [Key]
        [Column("CATEGORYID")]
        [StringLength(36)]
        public string CategoryId { get; set; }
        [StringLength(200)]
        [Column("CATEGORYNAME")]
        public string CategoryName { get; set; }
        [Column("PARENTCATEGORYID")]
        [StringLength(36)]
        public string ParentCategoryId { get; set; }
        [Column("SORT")]
        public int? Sort { get; set; }
        [ForeignKey("ParentCategoryId")]
        public DangerCategory ParentCategory { get; set; }
        [InverseProperty("ParentCategory")]
        public List<DangerCategory> ChildCategories { get; set; }
    }
}
