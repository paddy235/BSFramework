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
        /// ����
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// ״̬ δ��ʼ/��ȡ��/�����
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public string DutyUserId { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public string DutyUser { get; set; }
        /// <summary>
        /// ���β���
        /// </summary>
        public string DutyDepartmentId { get; set; }
        /// <summary>
        /// ���β���
        /// </summary>
        public string DutyDepartment { get; set; }
        /// <summary>
        /// ��ʼʱ��
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public string CreateUserId { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string CreateDeptId { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string CreateDept { get; set; }
        /// <summary>
        /// ����˵��
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        public string ParentTaskId { get; set; }
        /// <summary>
        /// �����¼
        /// </summary>
        public string UpdateRecords { get; set; }
        /// <summary>
        /// �����
        /// </summary>
        public string ModifyUserId { get; set; }
        /// <summary>
        /// �����
        /// </summary>
        public string ModifyUser { get; set; }
        /// <summary>
        /// ��Ȩ��
        /// </summary>
        public string TodoUserId { get; set; }
        /// <summary>
        /// ��Ȩ��
        /// </summary>
        public string TodoUser { get; set; }
        /// <summary>
        /// ���ʱ��
        /// </summary>
        public DateTime ModifyTime { get; set; }
        /// <summary>
        /// ���������β���
        /// </summary>
        [NotMapped]
        public string ParentDutyDepartmentId { get; set; }
        /// <summary>
        /// ������������
        /// </summary>
        [NotMapped]
        public string ParentDutyUserId { get; set; }
        /// <summary>
        /// �����񴴽���
        /// </summary>
        [NotMapped]
        public string ParentCreateUserId { get; set; }
        public bool IsPublish { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        [NotMapped]
        public List<DepartmentTaskEntity> SubTasks { get; set; }
        /// <summary>
        /// ����������
        /// </summary>
        [NotMapped]
        public int? SubTaskTotal { get; set; }
        [NotMapped]
        public int? State { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }
    }
}