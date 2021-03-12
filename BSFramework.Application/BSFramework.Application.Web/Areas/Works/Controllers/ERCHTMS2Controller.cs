using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.LllegalManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Util;
using System;
using System.Net;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class ERCHTMS2Controller : MvcControllerBase
    {
        //
        // GET: /Works/People/
        private LllegalBLL lbll = new LllegalBLL();
        //private PostCache postCache = new PostCache();
        private UserBLL userbll = new UserBLL();
        private DepartmentBLL deptbll = new DepartmentBLL();
        private PeopleBLL peoplebll = new PeopleBLL();
        private RoleBLL rbll = new RoleBLL();


        /// <summary>
        ///查询
        /// </summary>
        public ActionResult List(int page, int pagesize, string ctype, FormCollection fc)
        {
            if (page == 0) page = 1;
            if (pagesize == 0) page = 12;
            if (ctype == null)
            {
                ctype = fc.Get("texCtype");
            }
            var CheckDataRecordName = fc.Get("CheckDataRecordName");
            var StartTime = fc.Get("from");
            var EndTime = fc.Get("to");
            string useAccountr = BSFramework.Application.Code.OperatorProvider.Provider.Current().Account;
            string url;
            if (ctype == "1")
            {
                url = Config.GetValue("SyncIp") + "/SaftyCheck/SaftyCheckDataRecord/GetPageListJson?queryJson={ctype:\"1\",st:\"" + StartTime + "\",et:\"" + EndTime + "\",keyword:\"" + CheckDataRecordName + "\"}&rows=" + pagesize + "&page=" + page + "&sidx=createdate&sord=desc";
            }
            else
            {
                url = Config.GetValue("SyncIp") + "/SaftyCheck/SaftyCheckDataRecord/GetPageListJsonForType?queryJson={indexData:\"\",mode:\"\",ctype:\"" + ctype + "\",stm:\"" + StartTime + "\",etm:\"" + EndTime + "\",keyword:\"" + CheckDataRecordName + "\"}&rows=" + pagesize + "&page=" + page + "&sidx=createdate&sord=desc";
            }

            string args = BSFramework.Util.DESEncrypt.Encrypt(useAccountr + "^" + url + "^" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "^DLBZ");

            string aaa = Config.GetValue("SyncIp") + "/erchtms/login/signin?args=" + args;
            HttpWebRequest wrq = (HttpWebRequest)HttpWebRequest.Create(Config.GetValue("SyncIp") + "/login/signin?args=" + args);
            wrq.Method = "GET";
            System.Net.WebResponse wrp = wrq.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(wrp.GetResponseStream());
            string json = sr.ReadToEnd();

            BSFramework.Application.Web.Areas.Works.Models.SaftyCheckDataRecordModel entity = Newtonsoft.Json.JsonConvert.DeserializeObject<BSFramework.Application.Web.Areas.Works.Models.SaftyCheckDataRecordModel>(json);
            foreach (BSFramework.Application.Web.Areas.Works.Models.SaftyCheckDataRecordrows a in entity.rows)
            {
                if (Convert.ToDecimal(a.SolveCount) == 0)
                {
                    a.SolveCount = "未开始检查";
                }
                else
                {
                    a.SolveCount = "已完成" + a.SolveCount + "%";
                }
                a.CheckBeginTime = Convert.ToDateTime(a.CheckBeginTime).ToString("yyyy-MM-dd") + "至" + Convert.ToDateTime(a.CheckEndTime).ToString("yyyy-MM-dd");
                a.CheckDataType = Config.GetValue("SyncIpName") + "/SaftyCheck/SaftyCheckDataRecord/ZXDetails?recid=" + a.Id + "&zj=0&cname=专项安全检查";
                a.Id = Config.GetValue("SyncIpName") + "/HiddenTroubleManage/HTBaseInfo/Index?SAFETYCHECKOBJECTID=" + a.Id + "&actiontype=view";


            }
            //view-source:http://10.36.1.70/ERCHTMS/HiddenTroubleManage/HTBaseInfo/Index?SAFETYCHECKOBJECTID=0206064c-8a2b-45a8-a416-3ba0c946d6e0
            // ViewBag.path =ctype;
            // ViewBag.getList = "jQuery(function(){getList(path);});";
            ViewBag.ctype = ctype;
            var data = entity.rows;
            ViewBag.pages = Math.Ceiling((decimal)entity.records / pagesize); ;
            ViewBag.current = page;
            ViewBag.pagesize = pagesize;
            return View(data);
        }






    }
}
