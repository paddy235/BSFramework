using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.InnovationManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.WebApp;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.CertificateManage;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class UserController : BaseApiController
    {
        [HttpPost]
        public ListBucket<UserEntity> List(ParamBucket<string> args)
        {
            var total = 0;
            var data = new UserBLL().GetList(args.Data, args.PageSize, args.PageIndex, out total);
            return new ListBucket<UserEntity>() { Success = true, Data = data, Total = total };
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Edit()
        {
            var result = true;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "UserCertificate";
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                await this.Request.Content.ReadAsMultipartAsync(provider);
                var json = provider.FormData.Get("json");
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<UserCertificateEntity>>(json);

                var userbll = new UserBLL();
                var user = userbll.GetEntity(model.UserId);

                model.Data.createtime = model.Data.modifytime = DateTime.Now;
                model.Data.createuser = model.Data.modifyuser = user.RealName;
                model.Data.createuserid = model.Data.modifyuserid = user.UserId;

                var files = new List<FileInfoEntity>();
                var bis = new UserBLL();

                foreach (MultipartFileData file in provider.FileData)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                    var fileid = Guid.NewGuid().ToString();
                    var path = Path.Combine(filepath, fold);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                    var fileentity = new FileInfoEntity() { RecId = model.Data.Id, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
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
                bis.Edit(model.Data);

                return this.Request.CreateResponse<ResultBucket>(HttpStatusCode.OK, new ResultBucket { Success = result, Message = message });
            }
            catch (Exception e)
            {
                result = false;
                message = e.Message;
                return this.Request.CreateResponse<ResultBucket>(HttpStatusCode.InternalServerError, new ResultBucket { Success = result, Message = message });
            }
        }
    }
}
