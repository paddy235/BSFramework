
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WebApp
{
    /// <summary>
    /// 人员管理教育档案
    /// </summary>
    public class UserEduncationFileModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 培训时间
        /// </summary>
        public string trainingTime { get; set; }

        /// <summary>
        /// 培训名称
        /// </summary>
        public string trainingName { get; set; }

        /// <summary>
        /// 培训类型
        /// </summary>
        public string trainingType { get; set; }

        /// <summary>
        /// 培训时长
        /// </summary>
        public long trainingTimeLength { get; set; }
    }

    public class UserEduncationModel
    {
        public long totalTime { get; set; }

        public List<UserEduncationFileModel> EduncationFile { get; set; }
    }

    //一种年   一种条件
    public class Eduncationdata
    {
        public long allTime { get; set; }
        public UserEduncationModel aqpxEduncation { get; set; }
        public UserEduncationModel jspxEduncation { get; set; }
        public UserEduncationModel qtpxEduncation { get; set; }


    }

    public class jzpxModel
    {
        public string Code { get; set; }
        public string Info { get; set; }
        public jzpxresult Data { get; set; }
    }
    public class jzpxresult
    {
        public DateTime serverTime { get; set; }
        public List<jzpxlist> Projects { get; set; }
    }
    public class jzpxlist
    {
        public string projId { get; set; }
        public string examid { get; set; }
        public string proname { get; set; }

        public string pic { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public string parentId { get; set; }
        public string isExam { get; set; }
        public string state { get; set; }
        public string trainstart { get; set; }
    }

    public class jzpxDetail
    {
        public string Code { get; set; }
        public string Info { get; set; }
        public resutlData Data { get; set; }
    }

    public class resutlData {

        public List<jzpxUser> UserList { get; set; }
        public TrainRecord TrainRecord { get; set; }
    }
    public class TrainRecord {
        public string pageTime { get; set; }
        public string trainName { get; set; }
        public string trainTime { get; set; }

    }
    public class jzpxUser
    {
        public string USERNAME { get; set; }
    }
}
