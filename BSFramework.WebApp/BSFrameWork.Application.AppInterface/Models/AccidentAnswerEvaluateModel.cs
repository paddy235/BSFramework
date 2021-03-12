using System;
using System.ComponentModel.DataAnnotations;

namespace BSFrameWork.Application.AppInterface.Models
{
    /// <summary>
    /// 事故预想答题点评
    /// </summary>
    public class AccidentAnswerEvaluateModel
    {
        /// <summary>
        /// 答题记录
        /// </summary>
        [Required(ErrorMessage = "答题记录是必需的")]
        public string Id { get; set; }
        /// <summary>
        /// 点评
        /// </summary>
        [Display(Name = "点评"),
            MaxLength(200, ErrorMessage = "点评需要至多 200 字符")]
        public string AppraiseContent { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        [Display(Name = "评分")]
        public string Grade { get; set; }
    }
}