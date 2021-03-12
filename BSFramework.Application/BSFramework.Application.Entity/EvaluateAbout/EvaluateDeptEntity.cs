using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Entity.EvaluateAbout
{
    /// <summary>
    /// Ãè Êö£º¿¼ÆÀ
    /// </summary>
    [Table("wg_evaluatedept")]
    public class EvaluateDeptEntity : BaseEntity
    {
        public string EvaluateDeptId { get; set; }
        public string EvaluateId { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public bool IsSubmitted { get; set; }
    }
}