using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class MeetSubjectController : ApiController
    {
        private MeetSubjectBLL bll;
        public MeetSubjectController()
        {
            bll = new MeetSubjectBLL();
        }
        /// <summary>
        ///获取详情
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object getDetail(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var AppUrl = Config.GetValue("AppUrl");
                var data = bll.getDataByMeetID(dy.data);
                if (data == null)
                {
                    return new { info = "未查询到数据", code = result };
                }
                FileInfoBLL fileBll = new FileInfoBLL();
                var file = fileBll.GetFilesByRecIdNew(data.ID);
                foreach (var item in file)
                {
                    item.FilePath = AppUrl + item.FilePath.Replace("~/", string.Empty);
                }
                data.Files = file.ToList();
                return new { info = "操作成功", code = result, data = data };
            }
            catch (Exception ex)
            {
                return new { info = "操作失败：" + ex.Message, code = 1 };
            }

        }

        /// <summary>
        ///获取状态
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object getState(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var data = bll.getDataByMeetID(dy.data);


                return new { info = "操作成功", code = result, data = data.State };
            }
            catch (Exception ex)
            {
                return new { info = "操作失败：" + ex.Message, code = 1 };
            }

        }
        /// <summary>
        ///提交完成状态
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object SetState(BaseDataModel<string> dy)
        {
            var result = 0;
            var message = string.Empty;
            try
            {
                var data = bll.getDataByMeetID(dy.data);
                data.State = true;
                bll.SaveForm(dy.data, data);
                return new { info = "操作成功", code = result };
            }
            catch (Exception ex)
            {
                return new { info = "操作失败：" + ex.Message, code = 1 };
            }

        }




        /// <summary>
        ///数据操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Operation()
        {
            var result = 0;
            var message = string.Empty;
            var json = HttpContext.Current.Request.Form.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<MeetSubjectModel>>(json);
                FileInfoBLL fileBll = new FileInfoBLL();
                UserEntity user = new UserBLL().GetEntity(dy.userId);
                string getDel = string.IsNullOrEmpty(dy.data.DelKeys) ? "" : dy.data.DelKeys;
                #region 删除数据
                if (!string.IsNullOrEmpty(dy.data.deldata))
                {
                    var fileList = fileBll.GetFilesByRecIdNew(dy.data.deldata).ToList();
                    getDel = string.Join(",", fileList.Select(x => x.FileId));
                }
                #endregion
                #region 修改删除图片
                var DelKeys = getDel.Split(',');
                string keys = string.Empty;
                for (int i = 0; i < DelKeys.Length; i++)
                {
                    if (string.IsNullOrEmpty(DelKeys[i]))
                    {
                        continue;
                    }
                    FileInfoEntity fileList = fileBll.GetEntity(DelKeys[i]);
                    string url = Config.GetValue("FilePath") + fileList.FilePath.Replace("~/Resource", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(fileList.FilePath) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                    fileBll.Delete(DelKeys[i]);
                }
                #endregion
                string id = string.Empty;
                bool EntityType = false;
                if (dy.data.entity != null)
                {
                    if (string.IsNullOrEmpty(dy.data.entity.ID))
                    {
                        id = Guid.NewGuid().ToString(); EntityType = true;
                    }
                    else
                    {
                        id = dy.data.entity.ID;
                    }
                }
                //课件

                foreach (FileInfoEntity f in dy.data.entity.Files)
                {
                    f.ShareLink = f.FileId;
                    f.FileId = Guid.NewGuid().ToString();
                    //f.RecId = dy.data.entity.ID;
                    f.RecId = id;
                    var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
                    if (f.Description == "课件")
                    {
                        f.FilePath = f.FilePath;
                    }
                    else
                    {
                        f.FilePath = f.FilePath.Replace(url, "~/");
                    }
                    if (string.IsNullOrEmpty(f.Description))
                        f.Description = "材料";
                    f.FolderId = f.FolderId;
                    f.CreateDate = DateTime.Now;
                    f.CreateUserId = user.UserId;
                    f.CreateUserName = user.RealName;
                    if (f.Description == "课件")
                    {
                        f.FileExtensions = "";
                    }
                    else
                    {
                        f.FileExtensions = System.IO.Path.GetExtension(f.FileName);
                    }
                    fileBll.SaveForm(f);
                }
                #region 存储图片
                var Description = string.Empty;
                FileInfoEntity fi = null;
                foreach (string key in HttpContext.Current.Request.Files.AllKeys)
                {

                    HttpPostedFile file = HttpContext.Current.Request.Files[key];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = id,
                        RecId = id,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = "~/Resource/AppFile/MeetSubject/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = "",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\MeetSubject"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\MeetSubject");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\MeetSubject\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                #endregion
                #region 数据操作
                if (string.IsNullOrEmpty(dy.data.deldata))
                {
                    if (EntityType)
                    {
                        dy.data.entity.ID = id;
                        dy.data.entity.CreateDate = DateTime.Now;
                        dy.data.entity.CreateUserId = user.UserId;
                        dy.data.entity.CreateUserName = user.RealName;
                        dy.data.entity.ModifyDate = DateTime.Now;
                        dy.data.entity.ModifyUserId = user.UserId;
                        dy.data.entity.ModifyUserName = user.RealName;
                        bll.Save(dy.data.entity);
                    }
                    else
                    {
                        dy.data.entity.ModifyDate = DateTime.Now;
                        dy.data.entity.ModifyUserId = user.UserId;
                        dy.data.entity.ModifyUserName = user.RealName;
                        bll.SaveForm(id, dy.data.entity);
                    }
                }
                else
                {
                    bll.delEntity(dy.data.deldata);
                }
                #endregion
                return new { info = "操作成功", code = result };

            }
            catch (Exception ex)
            {

                return new { info = "操作失败：" + ex.Message, code = 1 };
            }


        }
    }
}
