using BSFramework.Application.Busines.ExperienceManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.ExperienceManage;
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
using System.IO;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Service.ExperienceManage;
using System.Configuration;
using BSFramework.Util.Extension;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class ExperienceController : ApiController
    {
        //
        // GET: /Experience/

        QuestionBLL qbll = new QuestionBLL();
        CommentBLL cbll = new CommentBLL();
        UserBLL ubll = new UserBLL();
        //
        // GET: /Emergency/
        /// <summary>
        /// 查询列表，含搜索
        /// 参数 index   1-你问我答   2-经验分享   title-标题搜索
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetList([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                string remark = dy.data.remark;
                string title = dy.data.title;
                var url = BSFramework.Util.Config.GetValue("AppUrl");
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                List<QuestionEntity> list = qbll.GetPageList("", "", title, remark, user.DepartmentId, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    //IList fileList = new FileInfoBLL().GetFilesByRecId(list[i].ID, );
                    var fileList = new FileInfoBLL().GetFilesByRecIdNew(list[i].ID).ToList();
                    foreach (var item in fileList)
                    {
                        item.FilePath = url + item.FilePath.Replace("~/", string.Empty);
                    }
                    list[i].Files = fileList;
                }
                //排序字段
                string[] property = new string[] { "commentCount", "ClickCount", "CreateDate" };
                bool[] sort = new bool[] { false, false, false };

                //对 List 排序
                list = new IListSort<QuestionEntity>(list, property, sort).Sort().ToList();

                return new { code = 0, info = "获取数据成功", count = total, data = new { questionList = list } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }

        }
        public int getccount(string id)
        {
            return cbll.GetList(id).Count();
        }
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object QuestionDetail([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string id = dy.data.id;
                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);
                QuestionEntity que = qbll.GetEntity(id);
                var url = BSFramework.Util.Config.GetValue("AppUrl");

                var fileList = new FileInfoBLL().GetFilesByRecIdNew(id).ToList();
                foreach (var item in fileList)
                {
                    item.FilePath = url + item.FilePath.Replace("~/", string.Empty);
                }
                que.Files = fileList;
                //IList fileList = new FileInfoBLL().GetFilesByRecId(id, BSFramework.Util.Config.GetValue("AppUrl"));
                //que.Files = fileList;

                que.ClickCount = que.ClickCount + 1;
                qbll.SaveForm(id, que);
                return new { info = "成功", code = 0, data = new { questionEntity = que, attachments = fileList } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }

        /// <summary>
        /// 评论列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetCommentList([FromBody] JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                long pageIndex = dy.data.pageIndex;//当前索引页
                long pageSize = dy.data.pageSize;//每页记录数
                //string remark = dy.data.remark;
                //string title = dy.data.title;
                string id = dy.data.id;

                string userId = dy.userId;
                UserEntity user = new UserBLL().GetEntity(userId);

                int total = 0;
                List<BSFramework.Application.Entity.ExperienceManage.CommentEntity> list = cbll.GetPageList("", "", id, user.DepartmentId, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), out total).ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    IList fileList = new FileInfoBLL().GetFilesByRecId(list[i].ID, BSFramework.Util.Config.GetValue("AppUrl"));
                    list[i].Files = fileList;
                }

                string[] property = new string[] { "commentCount", "CreateDate" };
                bool[] sort = new bool[] { false, false, };

                list = new IListSort<BSFramework.Application.Entity.ExperienceManage.CommentEntity>(list, property, sort).Sort().ToList();
                var nlist = list.Select(t => new
                {
                    t.ID,
                    t.Files,
                    t.CreateDate,   //发布日期  3
                    t.CreateUserName,
                    t.CreateUserId,
                    t.ClickCount,   //点击次数  2
                    t.Content,
                    t.Oppose,
                    t.OpposeUserId,
                    t.OpposeUserName,
                    t.SupportUserId,
                    t.SupportUserName,
                    t.Support,
                    t.Anonymity,
                    t.commentCount,
                    isSupport = isSupport(userId, t.SupportUserId),
                    isOppose = isOppose(userId, t.OpposeUserId)

                }).ToList();

                return new { code = 0, info = "获取数据成功", count = total, data = new { commentList = nlist } };
            }
            catch (Exception ex)
            {
                return new { code = 1, info = ex.Message };
            }
        }
        public string isSupport(string userid, string supportuserid)
        {
            if (supportuserid == null)
            {
                return "n";
            }
            else
            {
                if (supportuserid.Contains(userid))
                {
                    return "y";
                }
                else
                {
                    return "n";
                }
            }
        }
        public string isOppose(string userid, string opposeuserid)
        {
            if (opposeuserid == null)
            {
                return "n";
            }
            else
            {
                if (opposeuserid.Contains(userid))
                {
                    return "y";
                }
                else
                {
                    return "n";
                }
            }
        }

        /// <summary>
        /// 发表主题
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object NewQuestion()
        {

            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userId;
                UserEntity user = ubll.GetEntity(userId);
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                QuestionEntity entity = new QuestionEntity();
                entity.ID = Guid.NewGuid().ToString();
                entity.CreateDate = DateTime.Now;
                entity.CreateUserId = userId;
                entity.CreateUserName = user.RealName;
                entity.BZId = user.DepartmentId;
                entity.ClickCount = 0;

                entity.Content = dy.data.content;
                entity.Anonymity = dy.data.anonymity;
                entity.Remark = dy.data.remark;
                entity.Title = dy.data.title;

                FileInfoBLL fileBll = new FileInfoBLL();
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/ExpFile/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\ExpFile"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\ExpFile");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\ExpFile\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                qbll.SaveForm(string.Empty, entity);
                return new { info = "成功", code = 0, count = 0, data = entity };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }


        /// <summary>
        /// 点赞
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object Support([FromBody] JObject json)
        {

            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userId;
                string id = dy.data.id;
                string type = dy.data.type;
                UserEntity user = ubll.GetEntity(userId);
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                BSFramework.Application.Entity.ExperienceManage.CommentEntity entity = cbll.GetEntity(id);
                if (type == "1")    //点赞
                {
                    entity.Support += 1;
                    entity.SupportUserName += user.RealName + ',';
                    entity.SupportUserId += userId + ',';
                }
                else    //取消点赞，从name  id  中移除该人员
                {
                    entity.Support -= 1;
                    IList<string> names = entity.SupportUserName.Split(',').ToList();
                    IList<string> ids = entity.SupportUserId.Split(',').ToList();
                    names.Remove(user.RealName);
                    ids.Remove(user.UserId);
                    string sname = "";
                    string sid = "";
                    for (int i = 0; i < names.Count; i++)
                    {
                        sname += names[i] + ',';
                        sid += ids[i] + ',';
                    }
                    entity.SupportUserName = sname;
                    entity.SupportUserId = sid;
                }
                if (entity.SupportUserId.Length > 0)
                {
                    entity.SupportUserId = entity.SupportUserId.Substring(0, entity.SupportUserId.Length - 1);
                    entity.SupportUserName = entity.SupportUserName.Substring(0, entity.SupportUserName.Length - 1);
                }
                cbll.SaveForm(id, entity);
                return new { info = "成功", code = 0, count = 0, data = entity };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }


        /// <summary>
        /// 点踩
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object Oppose([FromBody] JObject json)
        {

            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userId;
                string id = dy.data.id;
                string type = dy.data.type;
                UserEntity user = ubll.GetEntity(userId);
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                BSFramework.Application.Entity.ExperienceManage.CommentEntity entity = cbll.GetEntity(id);


                if (type == "1")
                {
                    entity.Oppose += 1;
                    entity.OpposeUserName += user.RealName + ',';
                    entity.OpposeUserId += userId + ',';
                }
                else
                {
                    entity.Support -= 1;
                    IList<string> names = entity.OpposeUserName.Split(',').ToList();
                    IList<string> ids = entity.OpposeUserId.Split(',').ToList();
                    names.Remove(user.RealName);
                    ids.Remove(user.UserId);
                    string sname = "";
                    string sid = "";
                    for (int i = 0; i < names.Count; i++)
                    {
                        sname += names[i] + ',';
                        sid += ids[i] + ',';
                    }
                    entity.OpposeUserName = sname;
                    entity.OpposeUserId = sid;
                }
                if (entity.SupportUserId.Length > 0)
                {
                    entity.OpposeUserName = entity.OpposeUserName.Substring(0, entity.OpposeUserName.Length - 1);
                    entity.OpposeUserId = entity.OpposeUserId.Substring(0, entity.OpposeUserId.Length - 1);
                }
                cbll.SaveForm(id, entity);
                return new { info = "成功", code = 0, count = 0, data = entity };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }
        /// <summary>
        /// 获取班组帮版本信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetVersion([FromBody] JObject json)
        {
            //string res = json.Value<string>("json");
            //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
            //string v = dy.data.PublishVersion;

            //string v1 = BSFramework.Util.Config.GetValue("AppVersion");  //system.config
            //string r = BSFramework.Util.Config.GetValue("ReleaseVersion");
            //if (v != v1)
            //{
            //    string url = string.Format("http://{0}{1}/Version/{2}{3}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.ApplicationPath, "bzapp", v1);
            //    url += ".apk";
            //    return new { code = 0, info = "获取成功", data = new { Url = url, PublishVersion = v1, ReleaseVersion = r } };
            //}
            //else
            //{
            //    return new { code = 1, info = "暂无更新" };
            //}
            PackageBLL packageBLL = new PackageBLL();
            FileInfoBLL fileinfoBLL = new FileInfoBLL();
            //var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            PackageEntity package = packageBLL.GetEntity(0);
            if (package != null)
            {
                IList<FileInfoEntity> fileList = fileinfoBLL.GetFilesByRecIdNew(package.ID);
                string downloadurl = string.Empty;
                if (fileList != null && fileList.Count > 0)
                {
                    Url.Link("Attachment", new { id = fileList[0].FileId });
                    //downloadurl = Path.Combine(url, fileList[0].FilePath.Replace("~/", ""));
                }
                return new { code = 0, info = string.Empty, data = new { AppName = package.AppName, PublishVersion = package.PublishVersion, ReleaseVersion = package.ReleaseVersion, ReleaseDate = package.ReleaseDate.Value.ToString("yyyy-MM-dd"), Url = downloadurl } };
            }
            return new { code = 1, info = "未找到APP包信息", data = new { } };


        }
        /// <summary>
        /// 获取终端版本信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetVersionNew([FromBody] JObject json)
        {
            var deviceType = 1;
            string flavor = string.Empty;
            string res = json.Value<string>("json");
            json = JObject.Parse(res);
            var jToken = json.SelectToken("data.DeviceType");
            if (jToken != null) deviceType = jToken.Value<int>();
            var jTFlavor = json.SelectToken("data.PublicFlavor");
            if (jTFlavor != null) flavor = jTFlavor.Value<string>();


            PackageBLL packageBLL = new PackageBLL();
            FileInfoBLL fileinfoBLL = new FileInfoBLL();
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            System.Linq.Expressions.Expression<Func<PackageEntity, bool>> expression = p => p.PackType == deviceType;
            if (!string.IsNullOrEmpty(flavor)) expression = expression.And(x => x.ApkType == flavor);


            PackageEntity package = packageBLL.GetEntity(expression);
            if (package == null) package = packageBLL.GetEntity(p => p.PackType == deviceType && p.ApkType == null);
            if (package != null)
            {
                IList<FileInfoEntity> fileList = fileinfoBLL.GetFilesByRecIdNew(package.ID);
                string downloadurl = string.Empty;
                if (fileList != null && fileList.Count > 0)
                {
                    downloadurl = Path.Combine(url, fileList[0].FilePath.Replace("~/", ""));
                }
                return new { code = 0, info = string.Empty, data = new { AppName = package.AppName, PublishVersion = package.PublishVersion, ReleaseVersion = package.ReleaseVersion, ReleaseDate = package.ReleaseDate.Value.ToString("yyyy-MM-dd"), Url = downloadurl } };
            }
            return new { code = 1, info = "未找到APP包信息", data = new { } };
            //string v1 = BSFramework.Util.Config.GetValue("AppVersionNew");  //system.config
            //string r = BSFramework.Util.Config.GetValue("ReleaseVersionNew");
            //if (v != v1)
            //{
            //    string url = string.Format("http://{0}{1}/VersionNew/{2}{3}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.ApplicationPath, "bzzdapp", v1);
            //    url += ".apk";
            //    return new { code = 0, info = "获取成功", data = new { Url = url, PublishVersion = v1, ReleaseVersion = r } };
            //}
            //else
            //{
            //    return new { code = 1, info = "暂无更新" };
            //}

        }
        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object Comment()
        {

            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                //string res = json.Value<string>("json");
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                string userId = dy.userId;
                string objectId = dy.data.objectId;
                string type = dy.data.type;
                UserEntity user = ubll.GetEntity(userId);
                //dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);

                BSFramework.Application.Entity.ExperienceManage.CommentEntity entity = new BSFramework.Application.Entity.ExperienceManage.CommentEntity();
                entity.ID = Guid.NewGuid().ToString();
                entity.CreateDate = DateTime.Now;
                entity.CreateUserId = userId;
                entity.CreateUserName = user.RealName;
                entity.BZId = user.DepartmentId;
                entity.ClickCount = 0;

                entity.Content = dy.data.content;
                entity.Anonymity = dy.data.anonymity;
                entity.ObjectId = objectId;
                //entity.Remark = dy.data.remark;
                // entity.Title = dy.data.title;

                FileInfoBLL fileBll = new FileInfoBLL();
                HttpFileCollection files = HttpContext.Current.Request.Files;
                FileInfoEntity fi = null;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile hf = files[i];
                    string ext = System.IO.Path.GetExtension(hf.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    fi = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = entity.ID,
                        RecId = entity.ID,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/ExpFile/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\ExpFile"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\ExpFile");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\ExpFile\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                cbll.SaveForm(string.Empty, entity);
                if (type == "q")
                {
                    QuestionEntity qentity = qbll.GetEntity(objectId);
                    qentity.commentCount = qentity.commentCount + 1;
                    qbll.SaveForm(objectId, qentity);
                }
                else
                {
                    BSFramework.Application.Entity.ExperienceManage.CommentEntity qentity = cbll.GetEntity(objectId);
                    qentity.commentCount = qentity.commentCount + 1;
                    cbll.SaveForm(objectId, qentity);
                }
                return new { info = "成功", code = 0, count = 0, data = entity };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }




    }
}
