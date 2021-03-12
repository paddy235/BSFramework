using System;
using System.ComponentModel.DataAnnotations;

namespace BSFrameWork.Application.AppInterface.Models
{
    /// <summary>
    /// 事故预想
    /// </summary>
    public class AccidentPreviewModel
    {
        public string Id { get; set; }
        /// <summary>
        /// 题目
        /// </summary>
        [Required(ErrorMessage = "题目是必需的")]
        public string Theme { get; set; }
        /// <summary>
        /// 主持人
        /// </summary>
        [Required(ErrorMessage = "主持人是必需的")]
        public string TeacherId { get; set; }
        /// <summary>
        /// 主持人
        /// </summary>
        [Required(ErrorMessage = "主持人是必需的")]
        public string Teacher { get; set; }
        /// <summary>
        /// 记录人
        /// </summary>
        [Required(ErrorMessage = "记录人是必需的")]
        public string RegisterPeople { get; set; }
        /// <summary>
        /// 记录人
        /// </summary>
        [Required(ErrorMessage = "记录人是必需的")]
        public string RegisterPeopleId { get; set; }
        /// <summary>
        /// 答题内容
        /// </summary>
        [Required(ErrorMessage = "答题内容是必需的")]
        public string[] AnswerList { get; set; }
        /// <summary>
        /// 班会
        /// </summary>
        [Required(ErrorMessage = "班会是必需的")]
        public string MeetingId { get; set; }
        /// <summary>
        /// 参与人员
        /// </summary>
        public string AttendPeople { get; set; }
        /// <summary>
        /// 参与人员
        /// </summary>
        public string AttendPeopleId { get; set; }
        /// <summary>
        /// 参与人数
        /// </summary>
        public int AttendNumber { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? ActivityDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? ActivityEndDate { get; set; }
    }
}