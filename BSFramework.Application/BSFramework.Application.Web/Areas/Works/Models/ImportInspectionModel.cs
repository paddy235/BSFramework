using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    /// <summary>
    /// 设备巡回检查表 导入用
    /// </summary>
    public class ImportInspectionModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 检查表名称 唯一
        /// </summary>
        public string InspectionName { get; set; }

        /// <summary>
        /// 设备系统
        /// </summary>
        public string DeviceSystem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeptId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 记录创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 记录创建人编码
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 记录创建人姓名
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 最近一次修改人编码
        /// </summary>
        public string ModifyUserId { get; set; }

        /// <summary>
        /// 最近一次修改人姓名
        /// </summary>
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 检查项目
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 标准
        /// </summary>
        public string Standard { get; set; }

        public new void Create()
        {
            Operator user = OperatorProvider.Provider.Current();
            Id = Guid.NewGuid().ToString();
            CreateDate = DateTime.Now;
            CreateUserId = user.UserId;
            CreateUserName = user.UserName;
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
        }
    }
    
    /// <summary>
    /// 导入时分组用
    /// </summary>
    public class InspectionGroupingModel
    {
        /// <summary>
        /// 检查表名称
        /// </summary>
        public string InspectionName { get; set; }
        /// <summary>
        /// 设备系统
        /// </summary>
        public string DeivceSystem { get; set; }
        /// <summary>
        /// 班组名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 三项相同的实体
        /// </summary>
        public List<ImportInspectionModel> ItemList { get; set; }
    }
}