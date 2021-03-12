using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFrameWork.Application.AppInterface.Models;
using BSFrameWork.Application.AppInterface.Models.QueryModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class ActivityController : BaseApiController
    {
        private ActivitySubjectBLL _activitySubjectBLL;
        private ActivityBLL _activityBLL;
        /// <summary>
        /// 安全日活动  改进行动
        /// </summary>
        public ActivityActionBLL _acitonBLL;

        public ActivityController()
        {
            _activityBLL = new ActivityBLL();
            _activitySubjectBLL = new ActivitySubjectBLL();
            _acitonBLL = new ActivityActionBLL();
        }

        /// <summary>
        /// 获取可用的活动议题
        /// </summary>
        /// <param name="paramBucket"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<ActivitySubjectEntity> GetActiveSubjects(ParamBucket paramBucket)
        {
            var data = _activitySubjectBLL.GetActiveSubjects();
            return new ListBucket<ActivitySubjectEntity>() { Data = data, Success = true, Total = data.Count };
        }

        [HttpPost]
        public ResultBucket EndActivity(ParamBucket<string> paramBucket)
        {
            _activityBLL.EndActivity(paramBucket.Data);
            return new ResultBucket { Success = true };
        }


        [HttpPost]
        public ResultBucket EditActivity(ParamBucket<ActivityEntity> paramBucket)
        {
            _activityBLL.EditActivity(paramBucket.Data);
            return new ResultBucket { Success = true };
        }

        #region 安全日活动 改进行动
        /// <summary>
        /// 根据安全日活动的Id获取改进行动，包含上次落实情况
        /// </summary>
        /// <param name="parames"></param>
        /// <returns></returns>
        [HttpPost]
        public ListBucket<ActivityActionEntity> GetActionList(ParamBucket<string> parames)
        {
            if (string.IsNullOrWhiteSpace(parames.Data)) throw new ArgumentNullException("parames.Data", "安全日活动的ActivityId不能为空");
            var data = _acitonBLL.GetListByActionId(parames.Data, curUser.DeptId);
            return new ListBucket<ActivityActionEntity>(0, "查询成功", data, data.Count) { Success = true };
        }

        /// <summary>
        /// 新增/修改 改进行动
        /// </summary>
        /// <param name="parames"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket SaveAction()
        {
            var json = HttpContext.Current.Request.Form.Get("json");
            ParamBucket<ActivityActionQuery> parames = JsonConvert.DeserializeObject<ParamBucket<ActivityActionQuery>>(json);
            if (parames.Data == null) throw new ArgumentNullException("Data", "参数实体不能为空");
            if (parames.Data.ActionEntity == null) throw new ArgumentNullException("Data.ActionEntity", "参数实体不能为空");
            if (string.IsNullOrWhiteSpace(parames.Data.ActionEntity.Id))
            {
                //新增
                _acitonBLL.Insert(parames.Data.ActionEntity);
            }
            else
            {
                //修改
                List<FileInfoEntity> files = UpLoadFile(parames.Data.ActionEntity.Id, "ActivityAction");
                string[] delIds = parames.Data.DelKeys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                _acitonBLL.Update(parames.Data.ActionEntity, files, delIds);
            }
            return new ResultBucket(0, "操作成功");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="param">主键</param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket DelAction(ParamBucket<string> param)
        {
            if (string.IsNullOrWhiteSpace(param.Data)) throw new ArgumentNullException("Data", "要删除的主键不能空");
            _acitonBLL.Del(param.Data);
            return new ResultBucket(0, "操作成功");
        }
        #endregion

        /// <summary>
        /// 获取上传的附件
        /// </summary>
        /// <param name="recId">关联数据的Id</param>
        /// <param name="folderName">附件在服务器存放的文件夹的名称</param>
        /// <returns></returns>
        public List<FileInfoEntity> UpLoadFile(string recId, string folderName)
        {
            NameValueCollection npara = HttpContext.Current.Request.Form;
            Operator user = default;
            foreach (string keyName in npara.AllKeys)
            {
                if (keyName.ToUpper() == "USERID")
                {
                    user = GetOperator(npara[keyName]);
                }
            }
            List<FileInfoEntity> filesList = new List<FileInfoEntity>();
            if (HttpContext.Current.Request.Files != null && HttpContext.Current.Request.Files.Count > 0)
            {
                int sort = 0;
                for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[i];
                    string ext = System.IO.Path.GetExtension(file.FileName);//文件扩展名
                    string fileId = Guid.NewGuid().ToString();//上传后文件名
                    FileInfoEntity fileEntity = new FileInfoEntity
                    {
                        FileId = fileId,
                        FolderId = recId,
                        RecId = recId,
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        FilePath = Path.Combine("~/Resource/AppFile", folderName, fileId + ext),
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        //Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user?.UserId,
                        CreateUserName = user?.UserName,
                        SortCode = sort++

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + $"\\AppFile\\{folderName}"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + $"\\AppFile\\{folderName}");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + $"\\AppFile\\{folderName}\\" + fileId + ext);
                    //保存附件信息
                    //new FileInfoBLL().SaveForm(fileEntity
                    filesList.Add(fileEntity);
                }
            }
            return filesList;
        }

        private Operator GetOperator(string v)
        {
            throw new NotImplementedException();
        }
    }
}