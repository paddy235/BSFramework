using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    /**
     * 违章模块 2018.12.21
     * 接口实体结构
     ***/

    /// <summary>
    /// 违章类型
    /// </summary>
    public class LllegaType
    {
        public string applianceclass { get; set; }
        public List<ItemData> itemdata { get; set; }

        public class ItemData
        {
            public string lllegaltypeid { get; set; }
            public string lllegaltypename { get; set; }
        }
    }
    /// <summary>
    /// 违章级别
    /// </summary>
    public class LllegaLevel
    {
        public string lllegallevelid { get; set; }
        public string lllegallevelname { get; set; }
    }
    /// <summary>
    /// 违章列表实体
    /// </summary>
    public class LllegaEntity
    {
        #region 目前用到的项
        public string id { get; set; }
        /// <summary>
        /// 违章编号
        /// </summary>
        public string lllegalnumber { get; set; }
        /// <summary>
        /// 违章类型id
        /// </summary>
        public string lllegaltype { get; set; }
        /// <summary>
        /// 违章类型名称
        /// </summary>
        public string lllegaltypename { get; set; }
        private string _lllegaltime;
        /// <summary>
        /// 违章时间
        /// </summary>
        public string lllegaltime
        {
            get
            {
                if (!string.IsNullOrEmpty(_lllegaltime))
                {
                    return _lllegaltime.Substring(0, 10);
                }
                return string.Empty;
            }
            set
            {
                _lllegaltime = value;
            }
        }
        /// <summary>
        /// 违章级别
        /// </summary>
        public string lllegallevel { get; set; }
        /// <summary>
        /// 违章级别
        /// </summary>
        public string lllegallevelname { get; set; }
        /// <summary>
        /// 违章人姓名
        /// </summary>
        public string lllegalperson { get; set; }
        /// <summary>
        /// 违章人id
        /// </summary>
        public string lllegalpersonid { get; set; }
        /// <summary>
        /// 违章状态
        /// </summary>
        public string flowstate { get; set; }

        private string _reformfinishdate;
        /// <summary>
        /// 整改结束时间
        /// </summary>
        public string reformfinishdate
        {
            get
            {
                if (!string.IsNullOrEmpty(_reformfinishdate))
                {
                    return _reformfinishdate.Substring(0, 10);
                }
                return string.Empty;
            }
            set
            {
                _reformfinishdate = value;
            }
        }
        /// <summary>
        /// 违章描述
        /// </summary>
        public string lllegaldescribe { get; set; }

        #endregion

        #region 未用到的项
        /// <summary>
        /// 创建用户部门编码
        /// </summary>
        public string createuserdeptcode { get; set; }
        /// <summary>
        /// 创建用户机构编码
        /// </summary>
        public string createuserorgcode { get; set; }
        /// <summary>
        /// 创建人id
        /// </summary>
        public string createuserid { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdate { get; set; }
        /// <summary>
        /// 违章单位名称
        /// </summary>
        public string lllegalteam { get; set; }
        /// <summary>
        /// 违章单位编码
        /// </summary>

        public string lllegalteamcode { get; set; }
        /// <summary>
        /// 违章责任单位名称
        /// </summary>
        public string lllegaldepart { get; set; }
        /// <summary>
        /// 违章责任单位编码
        /// </summary>
        public string lllegaldepartcode { get; set; }
        /// <summary>
        /// 违章地址
        /// </summary>

        public string lllegaladdress { get; set; }
        /// <summary>
        /// 违章图片id
        /// </summary>
        public string lllegalpic { get; set; }
        /// <summary>
        /// 整改要求
        /// </summary>

        public string reformrequire { get; set; }
        /// <summary>
        /// 创建人姓名
        /// </summary>

        public string createusername { get; set; }
        /// <summary>
        /// 新增方式(0:非已整改违章；1：已整改违章登记)
        /// </summary>
        public string addtype { get; set; }
        /// <summary>
        /// 是否曝光
        /// </summary>
        public string isexposure { get; set; }
        /// <summary>
        /// 整改人姓名
        /// </summary>
        public string reformpeople { get; set; }
        /// <summary>
        /// 整改人id
        /// </summary>
        public string reformpeopleid { get; set; }
        /// <summary>
        /// 整改人联系方式
        /// </summary>
        public string reformtel { get; set; }
        /// <summary>
        /// 整改部门编码
        /// </summary>
        public string reformdeptcode { get; set; }
        /// <summary>
        /// /整改部门名称
        /// </summary>

        public string reformdeptname { get; set; }
        /// <summary>
        /// 整改截止时间
        /// </summary>

        public string reformdeadline { get; set; }
        /// <summary>
        /// 整改情况
        /// </summary>
        public string reformstatus { get; set; }
        /// <summary>
        /// 整改措施
        /// </summary>
        public string reformmeasure { get; set; }
        /// <summary>
        /// 验收人id
        /// </summary>
        public string acceptpeopleid { get; set; }
        /// <summary>
        /// 验收人姓名
        /// </summary>
        public string acceptpeople { get; set; }
        /// <summary>
        /// 验收部门名称
        /// </summary>
        public string acceptdeptname { get; set; }
        /// <summary>
        /// 验收部门编码
        /// </summary>
        public string acceptdeptcode { get; set; }
        /// <summary>
        /// 验收结果
        /// </summary>
        public string acceptresult { get; set; }
        /// <summary>
        /// /验收意见
        /// </summary>
        public string acceptmind { get; set; }
        /// <summary>
        /// 验收时间
        /// </summary>
        public DateTime? accepttime { get; set; }
        /// <summary>
        /// 关联其他应用的id
        /// </summary>
        public string reseverid { get; set; }
        /// <summary>
        /// 关联其他应用的类型值
        /// </summary>
        public string resevertype { get; set; }
        /// <summary>
        /// 当前流程下可审批的审核人(值为人员的账号)
        /// </summary>
        public string participant { get; set; }

        /// <summary>
        /// 违章图片
        /// </summary>
        public string filepath { get; set; }


        #endregion
    }

    /// <summary>
    /// 违章详情实体
    /// </summary>
    public class LllegaDetailEntity
    {
        public Appentity appentity { get; set; }

        /// <summary>
        /// 违章主键
        /// </summary>
        public string lllegalid { get; set; }
        /// <summary>
        /// 创建人id
        /// </summary>
        public string createuserid { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createdate { get; set; }

        /// <summary>
        /// 违章编号
        /// </summary>
        public string lllegalnumber { get; set; }

        /// <summary>
        /// 违章类型id
        /// </summary>
        public string lllegaltype { get; set; }

        /// <summary>
        /// 违章类型名称
        /// </summary>
        public string lllegaltypename { get; set; }

        /// <summary>
        /// 违章时间
        /// </summary>
        public DateTime? lllegaltime { get; set; }

        /// <summary>
        /// 违章级别
        /// </summary>
        public string lllegallevel { get; set; }
        /// <summary>
        /// 违章级别名称
        /// </summary>

        public string lllegallevelname { get; set; }
        /// <summary>
        /// 违章人姓名
        /// </summary>
        public string lllegalperson { get; set; }
        /// <summary>
        /// 违章人id
        /// </summary>
        public string lllegalpersonid { get; set; }
        /// <summary>
        /// 违章单位名称
        /// </summary>
        public string lllegalteamcode { get; set; }

        /// <summary>
        /// 违章单位名称
        /// </summary>
        public string lllegalteam { get; set; }

        /// <summary>
        /// 违章责任单位编码
        /// </summary>
        public string lllegaldepartcode { get; set; }
        /// <summary>
        /// 违章责任单位名称
        /// </summary>
        public string lllegaldepart { get; set; }

        /// <summary>
        /// 违章描述
        /// </summary>
        public string lllegaldescribe { get; set; }
        /// <summary>
        /// 违章地址
        /// </summary>
        public string lllegaladdress { get; set; }

        /// <summary>
        /// 整改要求
        /// </summary>
        public string reformrequire { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public string flowstate { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string createusername { get; set; }

        /// <summary>
        /// 新增方式(0:非已整改违章；1：已整改违章登记)
        /// </summary>
        public string addtype { get; set; }

        /// <summary>
        /// 是否曝光
        /// </summary>
        public string isexposure { get; set; }

        #region 违章整改部分信息
        /// <summary>
        /// 整改人姓名
        /// </summary>
        public string reformpeople { get; set; }
        /// <summary>
        /// 整改人id
        /// </summary>
        public string reformpeopleid { get; set; }
        /// <summary>
        /// 整改人联系方式
        /// </summary>
        public string reformtel { get; set; }

        /// <summary>
        /// 整改部门编码
        /// </summary>
        public string reformdeptcode { get; set; }
        /// <summary>
        /// 整改部门名称
        /// </summary>
        public string reformdeptname { get; set; }

        /// <summary>
        /// 整改截止时间
        /// </summary>
        public DateTime? reformdeadline { get; set; }
        /// <summary>
        /// 整改结束时间
        /// </summary>
        public DateTime? reformfinishdate { get; set; }

        /// <summary>
        /// 整改情况
        /// </summary>
        public string reformstatus { get; set; }

        /// <summary>
        /// 整改措施
        /// </summary>
        public string reformmeasure { get; set; }
        #endregion

        #region 违章验收部分信息
        /// <summary>
        /// 验收人id
        /// </summary>
        public string acceptpeopleid { get; set; }
        /// <summary>
        /// 验收人姓名
        /// </summary>
        public string acceptpeople { get; set; }

        /// <summary>
        /// 验收部门名称
        /// </summary>
        public string acceptdeptname { get; set; }
        /// <summary>
        /// 验收部门编码
        /// </summary>
        public string acceptdeptcode { get; set; }
        /// <summary>
        /// 验收结果
        /// </summary>
        public string acceptresult { get; set; }
        /// <summary>
        /// 验收意见
        /// </summary>
        public string acceptmind { get; set; }
        /// <summary>
        /// 验收时间
        /// </summary>
        public DateTime? accepttime { get; set; }

        #endregion


        #region 违章基础部分信息
        /// <summary>
        /// 关联其他应用的id
        /// </summary>
        public string reseverid { get; set; }
        /// <summary>
        /// 关联其他应用的类型值
        /// </summary>
        public string resevertype { get; set; }
        /// <summary>
        /// 当前流程下可审批的审核人(值为人员的账号)
        /// </summary>
        public string participant { get; set; }
        /// <summary>
        /// 是否上报安全主管部门
        /// </summary>
        public string isupsafety { get; set; }
        #endregion

        #region 违章考核部门信息
        /// <summary>
        /// 违章责任人
        /// </summary>
        public string chargepersonone { get; set; }
        /// <summary>
        /// 违章责任人id
        /// </summary>
        public string chargepersonidone { get; set; }
        /// <summary>
        /// 经济处罚
        /// </summary>
        public string economicspunishone { get; set; }

        /// <summary>
        /// 违章扣分
        /// </summary>
        public string lllegalpointone { get; set; }

        /// <summary>
        /// 待岗
        /// </summary>
        public string awaitjobone { get; set; }

        /// <summary>
        /// 违章责任人(第一联)
        /// </summary>
        public string chargepersontwo { get; set; }

        /// <summary>
        /// 违章责任人(第一联)id
        /// </summary>
        public string chargepersonidtwo { get; set; }
        /// <summary>
        /// 经济处罚
        /// </summary>
        public string economicspunishtwo { get; set; }
        /// <summary>
        /// 违章扣分
        /// </summary>
        public string lllegalpointtwo { get; set; }

        /// <summary>
        /// 待岗
        /// </summary>
        public string awaitjobtwo { get; set; }
        /// <summary>
        /// 违章责任人(第二联)
        /// </summary>
        public string chargepersonthree { get; set; }

        /// <summary>
        /// 违章责任人(第二联) id
        /// </summary>
        public string chargepersonidthree { get; set; }

        /// <summary>
        /// 经济处罚
        /// </summary>
        public string economicspunishthree { get; set; }

        /// <summary>
        /// 违章扣分
        /// </summary>
        public string lllegalpointthree { get; set; }

        /// <summary>
        /// 待岗
        /// </summary>
        public string awaitjobthree { get; set; }


        public List<Photo> lllegalpic { get; set; }

        public List<Photo> reformpic { get; set; }

        public List<Photo> acceptpic { get; set; }


        #endregion

        public class Photo
        {
            public string id { get; set; }
            public string filename { get; set; }
            public string fileurl { get; set; }
            public string folderid { get; set; }
        }
        public class Appentity
        {
            public string id { get; set; }
            public string autoid { get; set; }
            public string createuserid { get; set; }
            public string createuserdeptcode { get; set; }
            public string createuserorgcode { get; set; }
            public DateTime? createdate { get; set; }
            public string createusername { get; set; }
            public DateTime? modifydate { get; set; }
            public string modifyuserid { get; set; }
            public string modifyusername { get; set; }
            public string lllegaid { get; set; }
            public string approvepersonid { get; set; }
            public string approveperson { get; set; }
            public string approvedeptcode { get; set; }
            public string approvedeptname { get; set; }
            public DateTime? approvedate { get; set; }
            public string approveresult { get; set; }
            public string approvereason { get; set; }
        }
    }
    /// <summary>
    /// 日历汇总实体
    /// </summary>
    public class DateNumEntity
    {
        public int dates { get; set; }
        /// <summary>
        /// 登记数量
        /// </summary>
        public int alls { get; set; }
        /// <summary>
        /// 未验收数量
        /// </summary>
        public int dcls { get; set; }
    }
    /// <summary>
    /// 返回结果实体
    /// </summary>
    public class RetDataModel
    {
        public string code { get; set; }
        public string info { get; set; }
        public int? count { get; set; }
        public dynamic data { get; set; }
    }
}