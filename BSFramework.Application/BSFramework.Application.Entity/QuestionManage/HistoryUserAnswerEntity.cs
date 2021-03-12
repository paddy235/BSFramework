using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.QuestionManage
{
    /// <summary>
    /// 用户答题答案
    /// </summary>
   public class HistoryUserAnswerEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>	
        public string id { get; set; }
        /// <summary>
        /// 答案顺序
        /// </summary>	
        public int sort { get; set; }
        /// <summary>
        /// 答案
        /// </summary>	
        public string answer { get; set; }

        /// <summary>
        /// 分数表
        /// </summary>	
        public string titleid { get; set; }

        /// <summary>
        /// 是否正确
        /// </summary>	
        public bool istrue { get; set; }

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
