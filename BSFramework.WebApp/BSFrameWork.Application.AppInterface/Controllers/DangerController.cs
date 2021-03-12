using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    /// 安全预知训练
    /// </summary>
    public class DangerController : ApiController
    {
        /// <summary>
        /// 2.获取班组活动列表
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object GetList([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                long status = dy.data.status;  //查询状态
                string title = dy.data.title; //活动主题
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId); 
                ActivityBLL act = new ActivityBLL();
                int total = 0;
                IList list = act.GetList(int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total, int.Parse(status.ToString()), title,user.DepartmentId);
                return new { code = 0, info = "获取数据成功", count = total, data = new { activies = list } };
            }
            catch(Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
          
        }

        /// <summary>
        /// 15.获取班组活动详情
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object GetInfo([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string id = dy.data.DangerId;//记录Id
                DangerBLL actBll = new DangerBLL();
                DangerEntity act = actBll.GetEntity(id);//获取详情
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                IList list = fileBll.GetFilesByRecId(id, BSFramework.Util.Config.GetValue("AppUrl"));//获取相关附件集合
                return new { code = 0, info = "获取数据成功", data = new { Id = act.Id, JobName = act.JobName, JobUser = act.JobUser, JobAddress =act.JobAddress,JobTime=act.JobTime,TicketId=act.TicketId,Sno=act.Sno,Attachments = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 16.上传音频或图片
        /// </summary>
        /// <param name="json">参数对象</param>
        /// <returns></returns>
        [HttpPost]
        public object Upload()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string id = dy.data.DangerId;//业务记录Id
                ActivityBLL actBll = new ActivityBLL();
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                IList DelKeys = dy.data.DelKeys;
                string keys = string.Empty;
                for (int i = 0; i < DelKeys.Count; i++)
                {
                    if (i == DelKeys.Count - 1)
                    {
                        keys += DelKeys[i].ToString();
                    }
                    else
                        keys += DelKeys[i].ToString() + "','";
                }
                List<FileInfoEntity> fileList = fileBll.RemoveForm(keys, id, user);
                for (int i = 0; i < fileList.Count; i++)
                {
                    string path = fileBll.Delete(fileList[i].FileId);
                    string url = Config.GetValue("FilePath") + path.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                }
                FileInfoEntity fi = null;
                foreach(string key in HttpContext.Current.Request.Files.AllKeys)
                {
                    HttpPostedFile file=HttpContext.Current.Request.Files[key];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity 
                    {
                        FileId = fileId,
                        FolderId=id,
                        RecId=id,
                        FileName=System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/Danger/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        FileSize=file.ContentLength.ToString(),
                        Description=ext.ToLower()==".mp3"?"音频":"照片",
                        DeleteMark=0
                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Danger"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Danger");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Danger\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                return new { code = 0, info = "操作成功",};
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
 
    }
}
