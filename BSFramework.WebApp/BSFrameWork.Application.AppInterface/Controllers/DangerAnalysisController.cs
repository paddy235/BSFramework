using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Entity.WorkMeeting;
using BSFrameWork.Application.AppInterface.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class DangerAnalysisController : BaseApiController
    {
        private DangerAnalysisBLL _bll;
        private UserBLL _userBLL;

        public DangerAnalysisController()
        {
            _bll = new DangerAnalysisBLL();
            _userBLL = new UserBLL();
        }

        [HttpPost]
        public ModelBucket<DangerAnalysisEntity> GetLast(ParamBucket dataBucket)
        {
            var user = OperatorProvider.Provider.Current();
            var model = _bll.GetLast(user.DeptId);
            if (model == null)
            {
                model = new DangerAnalysisEntity()
                {
                    AnalysisId = Guid.NewGuid().ToString(),
                    DeptId = user.DeptId,
                    DeptName = user.DeptName,
                };
                _bll.Init(model);
            }
            return new ModelBucket<DangerAnalysisEntity>() { Data = model, Success = true };
        }

        [HttpPost]
        public ListBucket<RiskFactorSetEntity> FindDanger(ParamBucket<string> dataBucket)
        {
            var user = OperatorProvider.Provider.Current();
            var data = _bll.FindDanger(dataBucket.Data, dataBucket.PageSize, user.DeptId);
            return new ListBucket<RiskFactorSetEntity>() { Success = true, Data = data, Total = data.Count };
        }

        [HttpPost]
        public ModelBucket<JobDangerousEntity> GetDanger(ParamBucket<string> dataBucket)
        {
            var data = _bll.GetDanger(dataBucket.Data);
            return new ModelBucket<JobDangerousEntity>() { Data = data, Success = true };
        }

        [HttpPost]
        public ResultBucket EditDanger(ParamBucket<JobDangerousEntity> dataBucket)
        {
            var danger = new JobDangerousEntity()
            {
                JobDangerousId = dataBucket.Data.JobDangerousId,
                Content = dataBucket.Data.Content,
                CreateTime = DateTime.Now,
                JobId = dataBucket.Data.JobId,
                MeasureList = new List<JobMeasureEntity>()
            };
            if (string.IsNullOrEmpty(dataBucket.Data.JobDangerousId)) danger.JobDangerousId = Guid.NewGuid().ToString();
            for (int i = 0; i < dataBucket.Data.MeasureList.Count; i++)
            {
                danger.MeasureList.Add(new JobMeasureEntity
                {
                    Content = dataBucket.Data.MeasureList[i].Content,
                    CreateTime = danger.CreateTime.AddSeconds(i),
                    JobDangerousId = danger.JobDangerousId,
                    JobMeasureId = Guid.NewGuid().ToString()
                });
            }

            _bll.EditDanger(danger);

            return new ResultBucket() { Success = true };
        }

        [HttpPost]
        public ResultBucket DeleteDanger(ParamBucket<string> dataBucket)
        {
            _bll.DeleteDanger(dataBucket.Data);
            return new ResultBucket() { Success = true };
        }

        [HttpPost]
        public ModelBucket<DangerAnalysisEntity> Prev(ParamBucket<string> dataBucket)
        {
            var user = OperatorProvider.Provider.Current();
            var model = _bll.Prev(user.DeptId, dataBucket.Data);
            return new ModelBucket<DangerAnalysisEntity>() { Data = model, Success = true };
        }

        [HttpPost]
        public ModelBucket<DangerAnalysisEntity> GetByMeeting(ParamBucket<string> dataBucket)
        {
            var user = OperatorProvider.Provider.Current();
            var model = _bll.GetByMeeting(dataBucket.Data);
            return new ModelBucket<DangerAnalysisEntity>() { Data = model, Success = true };
        }

        public async Task<HttpResponseMessage> Sign()
        {
            if (!this.Request.Content.IsMimeMultipartContent())
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError) { ReasonPhrase = "No Files" };
            }

            var result = 0;
            var message = string.Empty;

            var filepath = ConfigurationManager.AppSettings["FilePath"].ToString();
            var fold = "Meeting";
            var root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files");
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            var provider = new MultipartFormDataStreamProvider(root);

            await this.Request.Content.ReadAsMultipartAsync(provider);
            var json = provider.FormData.Get("json");
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ParamBucket<string>>(json);

            var user = _userBLL.GetEntity(model.UserId);

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

                var fileentity = new FileInfoEntity() { RecId = model.Data, CreateDate = DateTime.Now, CreateUserId = model.UserId, CreateUserName = user.Account, DeleteMark = 0, FileId = fileid, FileName = filename, ModifyDate = DateTime.Now, ModifyUserId = user.UserId, ModifyUserName = user.Account, FileExtensions = Path.GetExtension(filename) };
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

            return this.Request.CreateResponse<object>(HttpStatusCode.OK, new { code = result, info = message });
        }
    }
}
