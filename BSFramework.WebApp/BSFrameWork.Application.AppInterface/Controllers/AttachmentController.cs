using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    /// 附件
    /// </summary>
    public class AttachmentController : BaseApiController
    {
        private FileInfoBLL _fileBLL;

        /// <summary>
        /// ctor
        /// </summary>
        public AttachmentController()
        {
            _fileBLL = new FileInfoBLL();
        }

        /// <summary>
        /// 即将删除的接口
        /// </summary>
        /// <param name="paramBucket"></param>
        /// <returns></returns>
        public ListBucket<FileInfoEntity> List(ParamBucket<string> paramBucket)
        {
            var data = _fileBLL.GetFileList(paramBucket.Data);
            var baseurl = Config.GetValue("AppUrl");
            foreach (var item in data)
            {
                item.FilePath = baseurl + item.FilePath.Replace("~", string.Empty);
            }
            return new ListBucket<FileInfoEntity>() { Data = data, Success = true, Total = data.Count };
        }

        /// <summary>
        /// 即将删除的接口
        /// </summary>
        /// <param name="paramBucket"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultBucket> PostAttachment()
        {
            var user = OperatorProvider.Provider.Current();

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            await this.Request.Content.ReadAsMultipartAsync(provider);
            var data = provider.FormData.Get("Data");
            var description = provider.FormData.Get("Description");
            var folder = provider.FormData.Get("Category");

            var files = new List<FileInfoEntity>();

            foreach (MultipartFileData file in provider.FileData)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                var fileid = Guid.NewGuid().ToString();
                var path = Path.Combine(filepath, folder);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                var fileentity = new FileInfoEntity() { RecId = data, CreateDate = DateTime.Now, CreateUserId = user.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename), Description = description, };
                fileentity.FilePath = "~/Resource/" + folder + "/" + fileid + Path.GetExtension(filename);

                files.Add(fileentity);
            }
            var filebll = new FileInfoBLL();
            filebll.SaveFiles(files);

            return new ResultBucket { Success = true };
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <returns>on content</returns>
        /// <response code="200">ok</response>
        /// <response code="400">验证错误</response>
        [Route("api/Attachment")]
        public async Task<IHttpActionResult> Post()
        {
            var user = OperatorProvider.Provider.Current();

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var root = HttpContext.Current.Server.MapPath("~/files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            await this.Request.Content.ReadAsMultipartAsync(provider);
            var data = provider.FormData.Get("Data");
            if (string.IsNullOrEmpty(data))
            {
                ModelState.AddModelError("Data", new ArgumentException("没有参数 Data"));
                return BadRequest(ModelState);
            }
            var folder = provider.FormData.Get("Category");
            if (string.IsNullOrEmpty(folder))
            {
                ModelState.AddModelError("Category", new ArgumentException("没有参数 Category"));
                return BadRequest(ModelState);
            }
            if (provider.FileData.Count == 0)
            {
                ModelState.AddModelError("Files", new ArgumentException("没有附件"));
                return BadRequest(ModelState);
            }

            var files = new List<FileInfoEntity>();
            foreach (MultipartFileData file in provider.FileData)
            {
                var description = file.Headers.ContentDisposition.Name.Trim('"');
                var filename = file.Headers.ContentDisposition.FileName.Trim('"');
                var fileid = Guid.NewGuid().ToString();
                var path = Path.Combine(filepath, folder);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.Move(file.LocalFileName, Path.Combine(path, fileid + Path.GetExtension(filename)));

                var fileentity = new FileInfoEntity() { RecId = data, CreateDate = DateTime.Now, CreateUserId = user.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename), Description = description, };
                fileentity.FilePath = "~/Resource/" + folder + "/" + fileid + Path.GetExtension(filename);

                files.Add(fileentity);
            }
            var filebll = new FileInfoBLL();
            filebll.SaveFiles(files);

            return Ok(files);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="recid">业务数据</param>
        /// <param name="description">附件说明 上传时的 Description</param>
        /// <returns>附件列表</returns>
        [Route("api/Attachment")]
        [ResponseType(typeof(List<FileInfoEntity>))]
        public IHttpActionResult Get([FromUri] string recid, [FromUri] string[] description = null)
        {
            var data = _fileBLL.List(recid, description);
            return Ok(data);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>附件流</returns>
        /// <response code="200">ok</response>
        /// <response code="404">资源不存在</response>
        [Route("api/Attachment/{id}", Name = "Attachment")]
        public HttpResponseMessage Get(string id)
        {
            var response = new HttpResponseMessage();
            if (string.IsNullOrEmpty(id))
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            var entity = _fileBLL.Get(id);
            if (entity == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            var rootpath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var filepath = entity.FilePath.Replace("~/Resource", rootpath).Replace('/', '\\');
            if (!File.Exists(filepath))
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            var stream = new FileStream(filepath, FileMode.Open);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = HttpUtility.UrlEncode(entity.FileName, Encoding.UTF8);
            response.Content.Headers.ContentDisposition.FileNameStar = entity.FileName;
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentLength = stream.Length;
            return response;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>ok</returns>
        /// <response code="200">ok</response>
        /// <response code="404">无效的资源</response>
        [Route("api/Attachment/{id}")]
        [ResponseType(typeof(FileInfoEntity))]
        public IHttpActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var entity = _fileBLL.Get(id);
            if (entity == null)
                return Ok("ok");

            _fileBLL.Delete(entity);
            var rootpath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var filepath = entity.FilePath.Replace("~/Resource", rootpath).Replace('/', '\\');
            if (File.Exists(filepath))
                File.Delete(filepath);

            return Ok(entity);
        }
    }
}
