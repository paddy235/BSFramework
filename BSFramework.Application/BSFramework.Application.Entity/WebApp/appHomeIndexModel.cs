using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WebApp
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseDataModel
    {
        public string Code { get; set; }
        public int Count { get; set; }
        public string Info { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDataModel<T> : BaseDataModel where T : class
    {
        public T data { get; set; }
    }

    // mode为1，data返回的数据：
    //名称 类型  是否必须 备注
    //s1 string 必须  一般未闭环
    //s2  string 必须  重大未闭环
    //code    string 必须  部门编码
    //name    string 必须  部门名称

    //mode为2，data返回的数据：
    //名称 类型  是否必须 备注
    //s1 string 必须  一般未闭环
    //s2  string 必须  重大未闭环
    //code    string 必须  区域编码
    //name    string 必须  区域名称

    //mode为3，data返回的数据：
    //名称 类型  是否必须 备注
    //s1 string 必须  一般未闭环
    //s2  string 必须  重大未闭环
    //code    string 必须  专业编码
    //name    string 必须  专业名称
    //id  string 必须  专业id

    // mode为5，data返回的数据：
    //名称 类型  是否必须 备注
    //s1 string 必须  一般违章(未闭环)
    //s2 string 必须  较严重违章(未闭环)
    //s3 string 必须  严重违章(未闭环)
    //code string 必须  部门编码
    //name    string 必须  部门名称
    public class modeone
    {
        public string s1 { get; set; }
        public string s2 { get; set; }
        public string s3 { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }
    //   mode为4，data返回的数据：

    //名称 类型  是否必须 备注
    //changedutydepartcode string 必须  部门编码
    //fullname    string 必须  部门名称
    //sortcode    string 必须  顺序号
    //ybzgl   string 必须  一般整改率
    //zdzgl   string 必须  重大整改率

    //    mode为6，data返回的数据：

    //名称 类型  是否必须 备注
    //reformdeptcode string 必须  部门编码
    //fullname    string 必须  部门名称
    //sortcode    string 必须  顺序号
    //ybzgl   string 必须  一般违章整改率
    //jyzzgl  string 必须  较严重违章整改率
    //yzzgl   string 必须  严重违章整改率
    public class modetwo
    {
        public string changedutydepartcode { get; set; }
        public string reformdeptcode { get; set; }
        public string fullname { get; set; }
        public string sortcode { get; set; }
        public string ybzgl { get; set; }
        public string zdzgl { get; set; }
        public string jyzzgl { get; set; }
        public string yzzgl { get; set; }
    }



}
