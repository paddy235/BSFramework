using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// 班会任务评分表
    /// </summary>
    [Table("wg_jobevaluate")]
    public class JobEvaluateEntity
    {
        private List<EvaluateItem> _evaluateItems = new List<EvaluateItem>();
        private string _content;

        /// <summary>
        /// 主键
        /// </summary>
        [Column("Id")]
        [Description("主键")]
        public string Id { get; set; }

        /// <summary>
        /// 任务的ID
        /// </summary>
        [Column("JobId")]
        [Description("任务的ID")]
        public string JobId { get; set; }

        /// <summary>
        /// 状态0未评分  1已评分
        /// </summary>
        [Column("State")]
        [Description("状态0未评分  1已评分")]
        public bool State { get; set; }

        /// <summary>
        /// 评分项详情 json格式
        /// </summary>
        [Column("Content")]
        [Description("评分项详情 json格式")]
        //[System.Web.Script.Serialization.ScriptIgnore]
        public string Content { get { return _content; } set { _content = value; } }

        /// <summary>
        /// 总得分
        /// </summary>
        [Column("TotalScore")]
        [Description("总得分")]
        public int TotalScore { get; set; }

        /// <summary>
        /// 评分人Id
        /// </summary>
        [Column("CreateUserId")]
        [Description("评分人Id")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 评分人
        /// </summary>
        [Column("CreateUserName")]
        [Description("评分人")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 评分时间
        /// </summary>
        [Column("CreateDate")]
        [Description("评分时间")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 各考评项的得分情况
        /// </summary>
        [NotMapped]
        [XmlElement("EvaluateItems")]
        public List<EvaluateItem> EvaluateItems
        {
            get
            {
                if (_evaluateItems == null)
                {
                    _evaluateItems = new List<EvaluateItem>();
                }
                if (!string.IsNullOrWhiteSpace(Content))
                {
                    try
                    {
                        _evaluateItems = JsonConvert.DeserializeObject<List<EvaluateItem>>(Content);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return _evaluateItems;
            }
            set {
                _evaluateItems = value;
                Content = JsonConvert.SerializeObject(_evaluateItems);
            }
        }
    }

    /// <summary>
    /// 得分内容
    /// </summary>
    public class EvaluateItem
    {
        /// <summary>
        /// 排序标号
        /// </summary>
        public int SortCode { get; set; }
        /// <summary>
        /// 评分项的名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 评分内容
        /// </summary>
        public string EvaluateContent { get; set; }
        /// <summary>
        /// 得分
        /// </summary>
        public int Score { get; set; }
        /// <summary>
        /// 是否特殊（班长负责人得分项）项
        /// </summary>
        public string Type { get; set; }
    }
}
