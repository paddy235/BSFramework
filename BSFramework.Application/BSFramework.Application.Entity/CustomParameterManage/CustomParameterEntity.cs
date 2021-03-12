using BSFramework.Application.Entity.PublicInfoManage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.CustomParameterManage
{
    /// <summary>
    /// 自定义台账数据
    /// </summary>
    [Table("WG_CUSTOMPARAMETER")]
    public class CustomParameterEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("CPID")]
        public string CPId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>		
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>		
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户id
        /// </summary>		
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>		
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>		
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>		
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 模板类型
        /// </summary>
        [Column("TEMPLATENAME")]
        public string TemplateName { get; set; }

        private string _FormContent;
        /// <summary>
        /// 表单内容
        /// </summary>		
        [Column("FORMCONTENT")]
        public string FormContent
        {
            get { return _FormContent; }
            set
            {

                if (!string.IsNullOrEmpty(value))
                {
                    _FormContentList = JsonConvert.DeserializeObject<List<CustomColsEntity>>(value);
                }
                _FormContent = value;
            }
        }

        private string _FormContentText;
        /// <summary>
        /// 表单值
        /// </summary>		
        [Column("FORMCONTENTTEXT")]
        public string FormContentText
        {
            get { return _FormContentText; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _FormText = JsonConvert.DeserializeObject<List<FormText>>(value);
                }
                _FormContentText = value;
            }
        }

        private string _TitleContent;
        /// <summary>
        /// 标题内容
        /// </summary>		
        [Column("TITLECONTENT")]
        public string TitleContent
        {
            get { return _TitleContent; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _TitleContentList = JsonConvert.DeserializeObject<List<CustomColsEntity>>(value);
                }
                _TitleContent = value;
            }
        }
        /// <summary>
        /// 所属部门Id
        /// </summary>
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        ///所属部门
        /// </summary>
        [Column("DEPT")]
        public string Dept { get; set; }
        /// <summary>
        /// 所属部门code
        /// </summary>
        [Column("DEPTCODE")]
        public string DeptCode { get; set; }
        /// <summary>
        /// 模板id
        /// </summary>
        [Column("CTID")]
        public string CTId { get; set; }
        /// <summary>
        /// 文件
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }

        private List<CustomColsEntity> _FormContentList;
        /// <summary>
        /// 表单内容转实体
        /// </summary>
        [NotMapped]
        public List<CustomColsEntity> FormContentList
        {
            get { return _FormContentList; }
            set
            {
                if (value != null)
                {
                    if (value.Count > 0)
                    {
                        _FormContent = JsonConvert.SerializeObject(value);
                    }
                }
                _FormContentList = value;
            }
        }

        private List<FormText> _FormText;
        /// <summary>
        /// 表单内容值转实体 
        /// </summary>
        [NotMapped]
        public List<FormText> FormText
        {
            get { return _FormText; }
            set
            {

                if (value != null)
                {
                    if (value.Count > 0)
                    {
                        _FormContentText = JsonConvert.SerializeObject(value);
                    }
                }
                _FormText = value;
            }
        }

        private List<CustomColsEntity> _TitleContentList;
        /// <summary>
        /// 标题内容转实体
        /// </summary>
        [NotMapped]
        public List<CustomColsEntity> TitleContentList
        {
            get { return _TitleContentList; }
            set
            {
                if (value != null)
                {
                    if (value.Count > 0)
                    {
                        _TitleContent = JsonConvert.SerializeObject(value);
                    }
                }

                _TitleContentList = value;
            }
        }


    }

}
