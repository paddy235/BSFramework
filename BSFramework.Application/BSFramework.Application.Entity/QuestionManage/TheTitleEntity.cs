using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.QuestionManage
{
    /// <summary>
    /// 答题目录
    /// </summary>
    public class TheTitleEntity : BaseEntity
    {

        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>	
        public string Id { get; set; }
        /// <summary>
        /// 时间
        /// </summary>	
        public DateTime startTime { get; set; }
        /// <summary>
        /// 时间
        /// </summary>	
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 类型
        /// </summary>	
        public string category { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>	
        public string activityid { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>	
        public bool iscomplete { get; set; }
        /// <summary>
        /// 答题人
        /// </summary>	
        public string username { get; set; }
        /// <summary>
        /// 答题人
        /// </summary>	
        public string userid { get; set; }
        /// <summary>
        /// 答题部门
        /// </summary>	
        public string deptid { get; set; }
        /// <summary>
        /// 答题部门
        /// </summary>	
        public string deptname { get; set; }
        /// <summary>
        /// 答题部门
        /// </summary>	
        public string deptcode { get; set; }
        /// <summary>
        /// 分数
        /// </summary>
        public string score { get; set; }

        /// <summary>
        ///回答列表
        /// </summary>
        [NotMapped]
        public List<HistoryUserAnswerEntity> useranswer { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {

        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
        }
        #endregion
    }
}
