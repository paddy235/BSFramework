using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.CustomParameterManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.CustomParameterManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    /// 自定义台账
    /// </summary>
    public class CustomParameterController : ApiController
    {

        private CustomParameterBLL cpbll = new CustomParameterBLL();
        private CustomTemplateBLL ctbll = new CustomTemplateBLL();
        private FileInfoBLL fileBLL = new FileInfoBLL();



        /// <summary>
        ///部门根据id获取数据
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket getCustomParameter(ParamBucket<getCustomParameterModel> dy)
        {
            if (!dy.AllowPaging)
            {
                dy.PageSize = 10000;
                dy.PageIndex = 1;
            }
            Pagination pagination = new Pagination();
            pagination.rows = dy.PageSize;
            pagination.page = dy.PageIndex;
            var queryModel = new { DeptId = dy.Data.deptid, StartTIme = dy.Data.from, EndTime = dy.Data.to, CTId = dy.Data.ctid };
            var queryStr = JsonConvert.SerializeObject(queryModel);
            //dy.data 可为空 空为全部
            var data = cpbll.GetPageList(pagination, queryStr, dy.UserId);
            data.ForEach(X => X.Files = fileBLL.GetFilesByRecIdNew(X.CPId).ToList());
            return new ListBucket<CustomParameterEntity> { code = 0, info = "操作成功", Data = data, Total = pagination.records, Success = true };
        }




        /// <summary>
        ///获取用户可使用模板数据
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket getTemplateType(ParamBucket<string> dy)
        {
            //dy.data 可为空 空为全部
            var data = ctbll.setSelect(dy.Data);
            return new ListBucket<CustomTemplateEntity> { code = 0, info = "操作成功", Data = data, Total = data.Count, Success = true };
        }
        /// <summary>
        ///获取内置使用模板数据
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket getAllTemplateType(ParamBucket dy)
        {
            if (!dy.AllowPaging)
            {
                dy.PageSize = 10000;
                dy.PageIndex = 1;
            }
            Pagination pagination = new Pagination();
            pagination.rows = dy.PageSize;
            pagination.page = dy.PageIndex;
            var queryModel = new { istemplate = "超级管理员" };
            var queryStr = JsonConvert.SerializeObject(queryModel);
            var data = ctbll.GetPageList(pagination, queryStr, dy.UserId);
            return new ListBucket<CustomTemplateEntity> { code = 0, info = "操作成功", Data = data, Total = pagination.records, Success = true };
        }

        /// <summary>
        ///自定义模板数据操作
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket OperationTemplate(ParamBucket<CustomTemplateEntity> dy)
        {
            var result = 0;
            try
            {

                var userbll = new UserBLL();
                var deptbll = new DepartmentBLL();
                var user = userbll.GetEntity(dy.UserId);
                var dept = deptbll.GetEntity(user.DepartmentId);
                dy.Data.UserDpet = dept.FullName;
                dy.Data.UserDpetId = dept.DepartmentId;
                dy.Data.UserDpetCode = dept.EnCode;
                ctbll.SaveForm(dy.Data, dy.UserId);

                return new ResultBucket { info = "操作成功", code = result, Success = true };
            }
            catch (Exception ex)
            {
                return new ResultBucket { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// 自定义台账数据操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket OperationParameter()
        {
            var result = 0;
            var json = HttpContext.Current.Request.Form.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<CustomParameterModel>>(json);
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                UserEntity user = new UserBLL().GetEntity(dy.userId);
                string getDel = string.IsNullOrEmpty(dy.data.DelKeys) ? "" : dy.data.DelKeys;
                #region 删除数据
                //if (!string.IsNullOrEmpty(dy.data.deldata))
                //{
                //    var fileList = fileBll.GetFilesByRecIdNew(dy.data.deldata).ToList();
                //    getDel = string.Join(",", fileList.Select(x => x.FileId));
                //}
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
                    string url = Config.GetValue("FilePath") + fileList.FilePath.Replace("~/Resource/AppFile/CustomParameter", "").Replace("/", "\\");
                    if (!string.IsNullOrEmpty(fileList.FilePath) && System.IO.File.Exists(url))
                    {
                        System.IO.File.Delete(url);
                    }
                    fileBll.Delete(DelKeys[i]);
                }
                #endregion
                string id = string.Empty;
                if (dy.data.entity != null)
                {
                    if (string.IsNullOrEmpty(dy.data.entity.CPId))
                    {
                        id = Guid.NewGuid().ToString();
                        dy.data.entity.CPId = id;
                    }
                    else
                    {
                        id = dy.data.entity.CPId;
                    }
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
                        FilePath = "~/Resource/AppFile/CustomParameter/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = file.FileName.Contains("tp_") ? "照片" : "文件",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\CustomParameter"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\CustomParameter");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\CustomParameter\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                #endregion
                #region 数据操作
                cpbll.SaveForm(dy.data.entity, dy.userId);
                #endregion
                return new ResultBucket { info = "操作成功", code = result, Success = true };

            }
            catch (Exception ex)
            {

                return new ResultBucket { info = "操作失败：" + ex.Message, code = 1 };
            }


        }




    }
}
