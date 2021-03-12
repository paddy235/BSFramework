using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    /// <summary>
    /// 传入参数
    /// </summary>
    public class ParamBucket
    {
        /// <summary>
        /// 登录人
        /// </summary>
        [Required]
        public string UserId { get; set; }
        public string TokenId { get; set; }
        public bool AllowPaging { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 传入参数
    /// </summary>
    /// <typeparam name="TModel">实体类型</typeparam>
    public class ParamBucket<TModel> : ParamBucket
    {
        /// <summary>
        /// 业务参数
        /// </summary>
        public TModel Data { get; set; }
    }

    /// <summary>
    /// 传入参数
    /// </summary>
    /// <typeparam name="TModel">集合类型</typeparam>
    public class ListParam<TModel> : ParamBucket
    {
        /// <summary>
        /// 业务参数
        /// </summary>
        public List<TModel> Data { get; set; }
    }

    /// <summary>
    /// 返回值
    /// </summary>
    public class ResultBucket
    {
        public ResultBucket()
        {
            this.code = 0;
            this.info = "操作成功";
        }
        public ResultBucket(int code, string info)
        {
            this.code = code;
            this.info = info;
        }
        private bool success;
        private string message;
        public int code { get; set; }
        public string info { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Success { get { return success; } set { success = value; code = value ? 0 : 1; } }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get { return message; } set { message = value; info = value; } }
    }

    /// <summary>
    /// 返回值
    /// </summary>
    /// <typeparam name="TModel">集合类型</typeparam>
    public class ListBucket<TModel> : ResultBucket where TModel : class
    {
        public ListBucket()
        {

        }
        public ListBucket(int code, string info, List<TModel> data, int total)
        {
            this.code = code;
            this.info = info;
            this.Data = data;
            this.Total = total;
        }
        /// <summary>
        /// 总数据量
        /// </summary>
        public int Total { get; set; }
        public int count { get { return Total; } private set { } }
        /// <summary>
        /// 集合数据
        /// </summary>
        public List<TModel> Data { get; set; }
        public List<TModel> data { get { return this.Data; } private set { this.Data = value; } }
    }

    /// <summary>
    /// 返回值
    /// </summary>
    /// <typeparam name="TModel">实体类型</typeparam>
    public class ModelBucket<TModel> : ResultBucket
    {
        /// <summary>
        /// 空构造器
        /// </summary>
        public ModelBucket()
        {
        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="code"></param>
        /// <param name="info"></param>
        /// <param name="data"></param>
        public ModelBucket(int code, string info, TModel data)
        {
            this.code = code;
            this.info = info;
            this.Data = data;
        }
        /// <summary>
        /// 实体数据
        /// </summary>
        public TModel Data { get; set; }
        public TModel data { get { return this.Data; } private set { this.Data = value; } }
    }

    public class Evaluation
    {
        public string Plan { get; set; }
        public string Effect { get; set; }
        public string keyvalue { get; set; }
    }


    public class DepartmentData
    {
        public string DepartmentId { get; set; }
        public string FullName { get; set; }
        public string Nature { get; set; }
        public int NumberOfPeople { get; set; }
    }

    public class UserData
    {
        public string UserId { get; set; }
        public string RealName { get; set; }

        public string Reasonremark { get; set; }
        public string DepartmentId { get; set; }

        public string DeptName { get; set; }

        public string Mobile { get; set; }
        public string State { get; set; }
    }

    public class UserIEntity
    {
        public string UserId { get; set; }
        public string RealName { get; set; }
        public string Mobile { get; set; }
        public string FullName { get; set; }
        public string State { get; set; }
    }

    public class MeetAbstract
    {
        /// <summary>
        /// 班组成员
        /// </summary>
        public int Member { get; set; }
        /// <summary>
        /// 工程师
        /// </summary>
        public int Engineer { get; set; }
        /// <summary>
        /// 技师
        /// </summary>
        public int Technician { get; set; }
        /// <summary>
        /// 中共党员
        /// </summary>
        public int PartyMember { get; set; }
        /// <summary>
        /// 助理工程师
        /// </summary>
        public int AssistantEngineer { get; set; }
        /// <summary>
        /// 高级工
        /// </summary>
        public int Expert { get; set; }
        /// <summary>
        /// 大专及以上学历
        /// </summary>
        public int Education { get; set; }
        /// <summary>
        /// 高级技师
        /// </summary>
        public int ExpertTechnician { get; set; }
        /// <summary>
        /// 平均年龄
        /// </summary>
        public double AverageAage { get; set; }
        /// <summary>
        /// 班组
        /// </summary>
        public string BzName { get; internal set; }
    }

    public class CultureWallInfoModel
    {
        public string summary { get; set; }
        public DateTime? summarydate { get; set; }
        public string slogan { get; set; }
        public DateTime? slogandate { get; set; }
        public string vision { get; set; }
        public DateTime? visiondate { get; set; }
        public string concept { get; set; }
        public DateTime? conceptdate { get; set; }

        public List<CultureWallInfoFileModel> pics { get; set; }
    }

    /// <summary>
    /// 班组荣誉/风采剪影
    /// </summary>
    public class CultureWallInfoFileModel
    {
        public string fileid { get; set; }
        public string description { get; set; }
        public string createdate { get; set; }
        public string modifydate { get; set; }
        public string filepath { get; set; }
        public string filetype { get; set; }
        public int key { get; set; }
    }

    public class FileInfos
    {
        public string description { get; set; }
        public string filetype { get; set; }
        public DateTime? modifydate { get; set; }
        public string modifyuserid { get; set; }

        public string modifyusername { get; set; }

        public int key { get; set; }
    }

    public class DateLimit
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool? IsEvaluate { get; set; }
        public string UserId { get; set; }
        public string Key { get; set; }
        public string DeptId { get; set; }
    }

    public class IndexCountries
    {

        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public string DeptId { get; set; }

        public string switchValue { get; set; }

    }

}