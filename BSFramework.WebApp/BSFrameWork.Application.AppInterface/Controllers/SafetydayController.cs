using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class SafetydayController : ApiController
    {
        private SafetydayBLL safetydaybll = new SafetydayBLL();
        private string url = BSFramework.Util.Config.GetValue("AppUrl");
        private FileInfoBLL fileBll = new FileInfoBLL();

        public class queryJson
        {
            public string keyvalue { get; set; }
            public string activitytype { get; set; }
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetListJson(BaseDataModel<queryJson> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                Pagination pagination = new Pagination();
                pagination.p_kid = "Id";
                pagination.p_fields = "CreateUserName,CreateDate,Subject,`Explain`,DeptName,activitytype,deptid,CDeptId,CDeptName";
                pagination.p_tablename = "wg_safetyday";
                pagination.conditionJson = "1=1";
                pagination.sidx = "CreateDate";
                pagination.sord = "desc";
                //var watch = CommonHelper.TimerStart();
               // string queryJson = "{}";
                if (dy.allowPaging)
                {
                    pagination.page = dy.pageIndex;
                    pagination.rows = dy.pageSize;
                }
                else
                {
                    pagination.page = 1;
                    pagination.rows = 2000;
                }
                //if (!string.IsNullOrEmpty(dy.data.keyvalue))
                //{
                //    pagination.conditionJson += string.Format(" and subject like '%{0}%'", dy.data.keyvalue);
                //}
                //if (!string.IsNullOrEmpty(dy.data.activitytype))
                //{
                //    pagination.conditionJson += string.Format(" and activitytype='{0}'", dy.data.activitytype);
                //}
               // var data = safetydaybll.GetPageList(pagination, queryJson);
                var data = safetydaybll.GetPagesList(pagination, Newtonsoft.Json.JsonConvert.SerializeObject(dy.data));
                //var dataJson = data.ToJson();
                //var dataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SafetydayEntity>>(dataJson);
                #region
                //data.Columns.Add("readnum", Type.GetType("System.String"));
                //for (int i = 0; i < data.Rows.Count; i++)
                //{
                //    var readnum = string.Empty;
                //    var deptStr = data.Rows[i]["deptid"].ToString();
                //    var keyvalue = data.Rows[i]["Id"].ToString();
                //    var deptList = deptStr.Split(',');
                //    var isreadnum = 0;
                //    var deptnum = deptList.Length;
                //    for (int j = 0; j < deptList.Length; j++)
                //    {
                //        var read = safetydaybll.getMaterial(deptList[j], keyvalue);
                //        if (read.Count > 0)
                //        {
                //            var readGroup = read.GroupBy(x => x.deptname);
                //            foreach (var item in readGroup)
                //            {
                //                // var deptreadnum = item.Count();
                //                var deptisread = item.Where(x => x.isread).Count() > 0;
                //                if (deptisread)
                //                {
                //                    isreadnum++;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            deptnum = 0;
                //            break;
                //        }
                //    }
                //    if (deptnum == 0)
                //    {
                //        readnum = "*";
                //    }
                //    else
                //    {
                //        readnum = isreadnum + "/" + deptnum;
                //    }

                //    data.Rows[i]["readnum"] = readnum;
                //}
                #endregion
                foreach (var item in data)
                {
                    var fileList = fileBll.GetFilesByRecIdNew(item.Id).ToList();
                    if (fileList.Count == 0)
                    {
                        item.Files = new List<FileInfoEntity>();
                    }
                    else
                    {
                        foreach (var items in fileList)
                        {
                            items.FilePath = items.FilePath.Replace("~/", url);
                        }
                        item.Files = fileList;
                    }


                }
                return new { code = result, info = "操作成功", data = data };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }

        }


        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public object GetActivitype(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var detail = new List<Object>();
                detail.Insert(0, new { Value = "安全日活动", Text = "安全日活动" });
                detail.Insert(0, new { Value = "政治学习", Text = "政治学习" });

                detail.Insert(0, new { Value = "上级精神宣贯", Text = "上级精神宣贯" });

                return new { code = result, info = "操作成功", data = detail };
            }
            catch (Exception ex)
            {

                return new { code = 1, info = ex.Message };
            }


        }
    }
}