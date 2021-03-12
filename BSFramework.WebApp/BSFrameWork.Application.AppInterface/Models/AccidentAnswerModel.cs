using System;
using System.ComponentModel.DataAnnotations;

namespace BSFrameWork.Application.AppInterface.Models
{
    /// <summary>
    /// 事故预想答题
    /// </summary>
    public class AccidentAnswerModel
    {
        public string Id { get; set; }
        /// <summary>
        /// 答题人
        /// </summary>
        [Required(ErrorMessage = "答题人是必需的")]
        public string AnswerPeople { get; set; }
        /// <summary>
        /// 答题人
        /// </summary>
        [Required(ErrorMessage = "答题人是必需的")]
        public string AnswerPeopleId { get; set; }
        /// <summary>
        /// 题目
        /// </summary>
        [Required(ErrorMessage = "题目是必需的"),
            MinLength(1, ErrorMessage = "题目至少需要1字符"),
            MaxLength(200, ErrorMessage = "题目至多需要200字符")]
        public string Question { get; set; }
        /// <summary>
        /// 事故现象
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 采取措施
        /// </summary>
        public string AnswerContent { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public string Grade { get; set; }
        /// <summary>
        /// 评价
        /// </summary>
        public string AppraiseContent { get; set; }
    }
}