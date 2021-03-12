using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Entity.WorkMeeting
{
    public class DepartmentTaskEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 任务
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 状态 未开始/已取消/已完成
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        public string DutyUserId { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        public string DutyUser { get; set; }
        /// <summary>
        /// 责任部门
        /// </summary>
        public string DutyDepartmentId { get; set; }
        /// <summary>
        /// 责任部门
        /// </summary>
        public string DutyDepartment { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建部门
        /// </summary>
        public string CreateDeptId { get; set; }
        /// <summary>
        /// 创建部门
        /// </summary>
        public string CreateDept { get; set; }
        /// <summary>
        /// 任务说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 父任务
        /// </summary>
        public string ParentTaskId { get; set; }
        /// <summary>
        /// 变更记录
        /// </summary>
        public string UpdateRecords { get; set; }
        /// <summary>
        /// 变更人
        /// </summary>
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 变更人
        /// </summary>
        public string ModifyUser { get; set; }
        /// <summary>
        /// 授权人
        /// </summary>
        public string TodoUserId { get; set; }
        /// <summary>
        /// 授权人
        /// </summary>
        public string TodoUser { get; set; }
        /// <summary>
        /// 变更时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// 父任务责任部门
        /// </summary>
        [NotMapped]
        public string ParentDutyDepartmentId { get; set; }
        /// <summary>
        /// 父任务责任人
        /// </summary>
        [NotMapped]
        public string ParentDutyUserId { get; set; }
        /// <summary>
        /// 父任务创建人
        /// </summary>
        [NotMapped]
        public string ParentCreateUserId { get; set; }
        public bool IsPublish { get; set; }
        /// <summary>
        /// 子任务
        /// </summary>
        [NotMapped]
        public List<DepartmentTaskEntity> SubTasks { get; set; }
        /// <summary>
        /// 子任务数量
        /// </summary>
        [NotMapped]
        public int? SubTaskTotal { get; set; }
        [NotMapped]
        public int? State { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }
    }
}