using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.QuestionManage
{
    /// <summary>
    /// 答案
    /// </summary>
    public class TheAnswerEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>	
        public string Id { get; set; }
        /// <summary>
        /// 问题
        /// </summary>	
        public string answer { get; set; }

        /// <summary>
        /// 是否正确
        /// </summary>	
        public bool istrue { get; set; }
        /// <summary>
        /// 描述
        /// </summary>	
        public string description { get; set; }
        /// <summary>
        /// 问题主键
        /// </summary>	
        public string questionid { get; set; }

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
