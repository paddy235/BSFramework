using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.BaseManage
{
    /// <summary>
    /// 首页内容设置
    /// </summary>
    public class PersonDoshboardEntity
    {
        [Column("PERSONDOSHBOARDID")]
        public string PersonDoshboardId { get; set; }
        [Column("USERID")]
        public string UserId { get; set; }
        [Column("SETTINGID")]
        public string SettingId { get; set; }
        [Column("ENABLED")]
        public bool Enabled { get; set; }
        [Column("SEQ")]
        public int Seq { get; set; }
        [Column("URL")]
        public string Url { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }
}
