using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    /// <summary>
    /// 基础返回类
    /// </summary>
    public class BaseResultEmergency
    {
        public int Code { get; set; }
        public int Count { get; set; }
        public string Info { get; set; }
    }
    #region 应急演练
    /// <summary>
    /// 应急演练类
    /// </summary>
    public class GetListDrillRecord
    {
        public string id { get; set; }
        public string status { get; set; }
        public string drilltime { get; set; }
    }

    public class GetDrillRecordBaseList : BaseResultEmergency
    {
        public List<GetDrillRecordBaseListDetail> data { get; set; }
    }
    public class GetDrillRecordBaseListDetail {

        public string id { get; set; }
        public string name { get; set; }
        public string drilltime { get; set; }
        public string filepath { get; set; }
    }
    /// <summary>
    /// 应急演练类
    /// </summary>
    public class GetListDrillRecordList : BaseResultEmergency
    {
        public List<GetListDrillRecord> data { get; set; }
    }
    /// <summary>
    /// 应急演练台账
    /// </summary>
    public class GetDrillRecordBase : BaseResultEmergency
    {
        public GetDrillRecordBaseDetail data { get; set; }
    }
    public class GetDrillRecordBaseDetail
    {

        public string id { get; set; }
        public drillplanrecordentity drillplanrecordentity { get; set; }
        public List<drillsteplistDrillRecord> drillsteplist { get; set; }
        public List<picturelist> picturelist { get; set; }

        public List<vediolist> vediolist { get; set; }
        public drillassessentity drillassessentity { get; set; }

    }

    public class drillplanrecordentity
    {

        public string maincontent { get; set; }
        public string name { get; set; }
        public string drilltype { get; set; }
        public string drilltypename { get; set; }
        public string drillmode { get; set; }
        public string drillmodename { get; set; }
        public string drilltime { get; set; }
        public string compere { get; set; }
        public string comperename { get; set; }
        public string drillplace { get; set; }
        public string drillpeople { get; set; }
        public string drillpeoplename { get; set; }
        public string drillpeoplenum { get; set; }
        public string drillplanid { get; set; }
        public string drillplanname { get; set; }
        public string drillpurpose { get; set; }
        public string scenesimulation { get; set; }
        public string drillkeypoint { get; set; }
        public string selfscore { get; set; }
        public string topscore { get; set; }
        public string drillidea { get; set; }
        public string assessperson { get; set; }
        public string assesstime { get; set; }
        public string assesspersonname { get; set; }
        public string isconnectplan { get; set; }




    }
    public class drillsteplistDrillRecord
    {

        public string stepid { get; set; }
        public string sortid { get; set; }
        public string content { get; set; }

        public string dutypersonname { get; set; }
    }

    public class picturelist
    {
        public string filename { get; set; }
        public string filepath { get; set; }
    }
    public class vediolist
    {
        public string filename { get; set; }
        public string filepath { get; set; }
    }

    public class drillassessentity
    {
        public string drillname { get; set; }
        public string drillplace { get; set; }
        public string organizedept { get; set; }

        public string topperson { get; set; }
        public string drilltime { get; set; }
        public string drilltype { get; set; }

        public string drillcontent { get; set; }
        public string suitable { get; set; }
        public string fullable { get; set; }
        public string personstandby { get; set; }

        public string personstandbyduty { get; set; }
        public string sitesupplies { get; set; }
        public string sitesuppliesduty { get; set; }
        public string wholeorganize { get; set; }
        public string dividework { get; set; }
        public string effectevaluate { get; set; }
        public string reportsuperior { get; set; }
        public string rescue { get; set; }
        public string evacuate { get; set; }

        public string valuateperson { get; set; }
        public string valuatepersonname { get; set; }
        public string score { get; set; }
        public string problemlist { get; set; }


        public string problem { get; set; }
        public string measure { get; set; }
    }
    #endregion
    #region 应急预案类型 
    /// <summary>
    /// 应急预案类型
    /// </summary>
    public class EmergencyList : BaseResultEmergency
    {
        public List<EmergencyItme> data { get; set; }

    }
    /// <summary>
    /// 应急预案类型
    /// </summary>
    public class EmergencyItme
    {
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
    }
    #endregion
    #region 应急预案
    public class EmergencyDataList : BaseResultEmergency
    {

        public List<EmergencyData> data { get; set; }
    }

    public class EmergencyData
    {
        public string id { get; set; }
        public string name { get; set; }

        public string plantypename { get; set; }
        public string username_bz { get; set; }
        public string departname_bz { get; set; }
        public string datetime_bz { get; set; }
        public string filepath { get; set; }
        public string filename { get; set; }
        public decimal r { get; set; }

    }
    public class EmergencyDataDetailList : BaseResultEmergency
    {

        public EmergencyDataDetail data { get; set; }
    }
    public class EmergencyDataDetail
    {

        public string id { get; set; }
        public string name { get; set; }
        public string drillpurpose { get; set; }

        public string scenesimulation { get; set; }
        public string drillkeypoint { get; set; }

        public List<drillsteplist> drillsteplist { get; set; }
        public string selfscore { get; set; }
        public string suitable { get; set; }
        public string fullable { get; set; }
        public string personstandby { get; set; }
        public string personstandbyduty { get; set; }
        public string sitesupplies { get; set; }
        public string sitesuppliesduty { get; set; }
        public string wholeorganize { get; set; }
        public string dividework { get; set; }
        public string effectevaluate { get; set; }
        public string reportsuperior { get; set; }
        public string rescue { get; set; }
        public string score { get; set; }
        public string evacuate { get; set; }
        public string valuateperson { get; set; }
        public string valuatepersonname { get; set; }
        public string problem { get; set; }
        public string measure { get; set; }

    }
    public class drillsteplist
    {

        public string stepid { get; set; }
        public string sortid { get; set; }
        public string content { get; set; }
        public string dutypersonname { get; set; }
        public string dutyperson { get; set; }
    }
    #endregion
    #region  应急演练保存
    public class SaveDrillAssess
    {
        public string userid { get; set; }
        public SaveDrillAssessDetail data { get; set; }

    }

    public class SaveDrillAssessDetail
    {
        public string keyvalue { get; set; }
        public string drillpurpose { get; set; }
        public string scenesimulation { get; set; }
        public string drillkeypoint { get; set; }
        public string selfscore { get; set; }
        public string suitable { get; set; }
        public string fullable { get; set; }
        public string personstandby { get; set; }
        public string personstandbyduty { get; set; }
        public string sitesupplies { get; set; }
        public string sitesuppliesduty { get; set; }
        public string wholeorganize { get; set; }
        public string dividework { get; set; }
        public string effectevaluate { get; set; }
        public string reportsuperior { get; set; }
        public string rescue { get; set; }
        public string evacuate { get; set; }
        public string valuateperson { get; set; }
        public string valuatepersonname { get; set; }
        public string problem { get; set; }
        public string measure { get; set; }
        public List<steplist> steplist { get; set; }
    }


    public class steplist
    {
        public string id { get; set; }
        public string dutyperson { get; set; }
        public string dutypersonname { get; set; }
        public string content { get; set; }
        public int sortid { get; set; }

    }
    #endregion
}