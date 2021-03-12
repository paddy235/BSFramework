using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BSFramework.Application.Entity.MessageManage
{
    /// <summary>
    /// 消息内容
    /// </summary>
    public class IMReadModel
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public string ReadId { get; set; }
        /// <summary>
        /// 信息内容ID
        /// </summary>
        public string ContentId { get; set; }
        /// <summary>
        /// 消息接收者Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 消息发送者Id
        /// </summary>
        public string SendId { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 消息时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
