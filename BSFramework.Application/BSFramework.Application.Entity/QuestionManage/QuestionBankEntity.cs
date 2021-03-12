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
    /// 题库
    /// </summary>
   public class QuestionBankEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>	
        public string Id { get; set; }
        /// <summary>
        /// 类型
        /// </summary>	
        public string topictype { get; set; }
        /// <summary>
        /// 题目标题
        /// </summary>	
        public string topictitle { get; set; }
        /// <summary>
        /// 描述
        /// </summary>	
        public string description { get; set; }
        /// <summary>
        /// 是否正确
        /// </summary>	
        public bool istrue { get; set; }
        /// <summary>
        /// 关联主键
        /// </summary>	
        public string outkeyvalue { get; set; }
        /// <summary>
        /// 排序
        /// </summary>	
        public int sort { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>		
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>		
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>		
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>		
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>		
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>		
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 答案
        /// </summary>
        [NotMapped]
        public List<TheAnswerEntity> TheAnswer { get; set; }
        /// <summary>
        /// 关联文件id
        /// </summary>	
        public string fileids { get; set; }
        /// <summary>
        /// 文件
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }

        /// <summary>
        /// 关联材料
        /// </summary>	
       [NotMapped]
        public string safetydayid { get; set; }
        
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
