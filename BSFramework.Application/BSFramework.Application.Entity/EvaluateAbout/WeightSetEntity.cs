using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EvaluateAbout
{
    /// <summary>
    /// 班组权重设置
    /// </summary>
    public class WeightSetEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 类别Id（考评标准类别主键）
        /// </summary>
        /// <returns></returns>
        public string Id { get; set; }
        /// <summary>
        /// 记录创建时间
        /// </summary>
        /// <returns></returns>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 记录创建人编码
        /// </summary>
        /// <returns></returns>
        public string CreateUserId { get; set; }
        /// <summary>
        /// 记录创建人姓名
        /// </summary>
        /// <returns></returns>
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        /// <returns></returns>
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 最近一次修改人编码
        /// </summary>
        /// <returns></returns>
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 最近一次修改人姓名
        /// </summary>
        /// <returns></returns>
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 记录创建人所在部门编码
        /// </summary>
        /// <returns></returns>
        public string DeptCode { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        /// <returns></returns>
        public string ClassName { get; set; }
        /// <summary>
        /// 是否启动
        /// </summary>
        /// <returns></returns>
        public int? IsFiring { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        /// <returns></returns>
        public int SortCode { get; set; }

        /// <summary>
        /// 权重（0-1范围）
        /// </summary>
        /// <returns></returns>
        public decimal? Weight { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        public string Remark { get; set; }


        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            //this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.DeptCode = string.IsNullOrEmpty(OperatorProvider.Provider.Current().DeptCode) ? OperatorProvider.Provider.Current().OrganizeCode : OperatorProvider.Provider.Current().DeptCode;
        }

        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
