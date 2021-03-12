using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Bst.Fx.Uploading;
using Bst.ServiceContract.MessageQueue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class DepartmentPublishController : BaseApiController
    {
        [HttpPost]
        public ListBucket<DepartmentPublishEntity> List(ParamBucket args)
        {
            var user = OperatorProvider.Provider.Current();
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            var bll = new DepartmentPublishBLL();
            var data = bll.List(user.DeptId);
            foreach (var item in data)
            {
                if (item.Files != null)
                {
                    foreach (var item1 in item.Files)
                    {
                        item1.FilePath = url + item1.FilePath.Replace("~/", string.Empty);
                    }
                }
            }
            return new ListBucket<DepartmentPublishEntity>() { Success = true, Data = data };
        }

        //[HttpPost]
        //public ModelBucket<DepartmentPublishEntity> Add(DataBucket<DepartmentPublishEntity> args)
        //{
        //    var user = OperatorProvider.Provider.Current();
        //    var bll = new DepartmentPublishBLL();
        //    args.Data.CreateUserId = args.UserId;
        //    args.Data.CreateDate = DateTime.Now;
        //    args.Data.PublishId = Guid.NewGuid().ToString();
        //    args.Data.DeptId = user.DeptId;
        //    args.Data.DeptName = user.DeptName;
        //    var data = bll.Add(args.Data);
        //    return new ModelBucket<DepartmentPublishEntity>() { Success = true, Data = data };
        //}

        //[HttpPost]
        //public ModelBucket<DepartmentPublishEntity> Edit(DataBucket<DepartmentPublishEntity> args)
        //{
        //    var bll = new DepartmentPublishBLL();
        //    args.Data.CreateUserId = args.UserId;
        //    args.Data.CreateDate = DateTime.Now;
        //    var data = bll.Edit(args.Data);
        //    return new ModelBucket<DepartmentPublishEntity>() { Success = true, Data = data };
        //}

        [HttpPost]
        public ResultBucket Delete(ParamBucket<string> args)
        {
            var bll = new DepartmentPublishBLL();
            bll.Delete(args.Data);
            return new ResultBucket() { Success = true };
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Add()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "No Files" };
            }

            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "publish";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DepartmentPublishEntity>>(json);
                var user = new UserBLL().GetEntity(model.UserId);
                model.Data.CreateUserId = model.UserId;
                model.Data.CreateDate = DateTime.Now;
                model.Data.PublishId = Guid.NewGuid().ToString();
                model.Data.DeptId = user.DepartmentId;
                model.Data.DeptName = user.DepartmentName;
                var bll = new DepartmentPublishBLL();
                var files = new List<FileInfoEntity>();
                var filebll = new FileInfoBLL();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    //if (filebll.Exist(filename))
                    //    continue;

                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);

                    switch (file.Headers.ContentType.MediaType)
                    {
                        case "image/jpeg":
                            fileentity.Description = "照片";
                            break;
                        case "image/jpg":
                            fileentity.Description = "照片";
                            break;
                        case "image/png":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mpeg":
                            fileentity.Description = "音频";
                            break;
                        case "audio/mp3":
                            fileentity.Description = "音频";
                            break;
                        case "video/mp4":
                            fileentity.Description = "视频";
                            break;
                        default:
                            break;
                    }
                    files.Add(fileentity);

                }
                model.Data.Files = files;

                var data = bll.Add(model.Data);

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message, data = files });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

        }

        [HttpPost]
        public async Task<HttpResponseMessage> Edit()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "No Files" };
            }

            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "publish";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<DepartmentPublishEntity>>(json);
                var user = new UserBLL().GetEntity(model.UserId);
                model.Data.CreateUserId = model.UserId;
                model.Data.CreateDate = DateTime.Now;
                model.Data.DeptId = user.DepartmentId;
                model.Data.DeptName = user.DepartmentName;
                var bll = new DepartmentPublishBLL();
                var filebll = new FileInfoBLL();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    //if (filebll.Exist(filename))
                    //    continue;

                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);

                    switch (file.Headers.ContentType.MediaType)
                    {
                        case "image/jpeg":
                            fileentity.Description = "照片";
                            break;
                        case "image/jpg":
                            fileentity.Description = "照片";
                            break;
                        case "image/png":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mpeg":
                            fileentity.Description = "音频";
                            break;
                        case "audio/mp3":
                            fileentity.Description = "音频";
                            break;
                        case "video/mp4":
                            fileentity.Description = "视频";
                            break;
                        default:
                            break;
                    }
                    model.Data.Files.Add(fileentity);

                }

                var data = bll.Edit(model.Data);

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message, data = data });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }

        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostFiles()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "No Files" };
            }

            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "publish";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);

                var userbll = new UserBLL();
                var user = userbll.GetEntity(model.UserId);

                var files = new List<FileInfoEntity>();
                var filebll = new FileInfoBLL();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    if (filebll.Exist(filename))
                        continue;

                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
                    fileentity.FilePath = "~/Resource/" + fold + "/" + fileid + Path.GetExtension(filename);

                    switch (file.Headers.ContentType.MediaType)
                    {
                        case "image/jpeg":
                            fileentity.Description = "照片";
                            break;
                        case "image/jpg":
                            fileentity.Description = "照片";
                            break;
                        case "image/png":
                            fileentity.Description = "照片";
                            break;
                        case "audio/mpeg":
                            fileentity.Description = "音频";
                            break;
                        case "audio/mp3":
                            fileentity.Description = "音频";
                            break;
                        case "video/mp4":
                            fileentity.Description = "视频";
                            break;
                        default:
                            break;
                    }
                    files.Add(fileentity);

                }
                filebll.SaveFiles(files);

                return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message, data = files });
            }
            catch (Exception e)
            {
                return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
