
namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 
    /// </summary>
    public class StatisticsNumModel
    {
        /// <summary>
        /// 班组Id
        /// </summary>
        public string GroupId { set; get; }
        /// <summary>
        /// 班组名称
        /// </summary>
        public string GroupName { set; get; }
        /// <summary>
        /// 安全日活动
        /// </summary>
        public int Safety { set; get; }
        /// <summary>
        /// 民主生活会
        /// </summary>
        public int Democratic { set; get; }
        /// <summary>
        /// 政治学习
        /// </summary>
        public int Politics { set; get; }
        /// <summary>
        /// 班务会
        /// </summary>
        public int team { set; get; }
        /// <summary>
        /// 其他活动
        /// </summary>
        public int Elseactivity { set; get; }
        /// <summary>
        /// 时间
        /// </summary>
        public string Endtime { set; get; }
    }
}
