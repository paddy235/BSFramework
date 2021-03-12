using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.CarcOrCardManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.CarcOrCardManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFrameWork.Application.AppInterface.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    /// carc card
    /// </summary>
    public class CarcOrCardController : ApiController
    {
        private CarcOrCardBLL cbll = new CarcOrCardBLL();

        /// <summary>
        ///carc card 台账
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket getCarcOrCard(ParamBucket<getCarcOrCardModel> dy)
        {
            if (!dy.AllowPaging)
            {
                dy.PageSize = 10000;
                dy.PageIndex = 1;
            }
            var filebll = new FileInfoBLL();
            Pagination pagination = new Pagination();
            pagination.rows = dy.PageSize;
            pagination.page = dy.PageIndex;
            var queryStr = JsonConvert.SerializeObject(dy.Data);
            //dy.data 可为空 空为全部
            var data = cbll.GetPageList(pagination, queryStr, dy.UserId);
            var url = BSFramework.Util.Config.GetValue("AppUrl");
            foreach (var item in data)
            {
                item.Files = filebll.GetFilesByRecIdNew(item.Id).Where(x => x.Description == "照片").OrderBy(x => x.CreateDate).ToList();
                foreach (var model in item.Files)
                {
                    model.FilePath = model.FilePath.Replace("~", url);
                }
            }
            return new ListBucket<CarcEntity> { code = 0, info = "操作成功", Data = data, Total = pagination.records, Success = true };
        }

        /// <summary>
        ///检索 card库
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket getVagueCard(ParamBucket<string> dy)
        {

            var data = cbll.GetVagueList(dy.Data, dy.UserId);
            foreach (var item in data)
            {
                foreach (var danger in item.CDangerousList)
                {
                    danger.IsSafe = 1;
                }
            }
            return new ListBucket<CCardEntity> { code = 0, info = "操作成功", Data = data, Total = data.Count, Success = true };
        }
        /// <summary>
        ///检索  所有风险因素
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket getDanger(ParamBucket<string> dy)
        {
            var total = 0;
            if (!dy.AllowPaging)
            {
                dy.PageSize = 10000;
                dy.PageIndex = 1;
            }
            var data = cbll.getDanger(dy.Data, dy.PageSize, dy.PageIndex, out total);

            return new ListBucket<CDangerousEntity> { code = 0, info = "操作成功", Data = data, Total = total, Success = true };
        }
        /// <summary>
        ///检索  所有措施
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket getMeasure(ParamBucket<string> dy)
        {
            var total = 0;
            if (!dy.AllowPaging)
            {
                dy.PageSize = 10000;
                dy.PageIndex = 1;
            }
            var data = cbll.getMeasure(dy.Data, dy.PageSize, dy.PageIndex, out total);
            return new ListBucket<CMeasureEntity> { code = 0, info = "操作成功", Data = data, Total = total, Success = true };
        }

        /// <summary>
        ///carc card 详情
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket getCarcOrCardDetail(ParamBucket<string> dy)
        {
            if (!dy.AllowPaging)
            {
                dy.PageSize = 10000;
                dy.PageIndex = 1;
            }
            Pagination pagination = new Pagination();
            pagination.rows = dy.PageSize;
            pagination.page = dy.PageIndex;
            //dy.data 可为空 空为全部
            var data = cbll.GetDetail(dy.Data);
            var url = BSFramework.Util.Config.GetValue("AppUrl");
            foreach (var item in data.Files)
            {
                item.FilePath = item.FilePath.Replace("~", url);
            }
            return new ModelBucket<CarcEntity> { code = 0, info = "操作成功", Data = data, Success = true };
        }

        /// <summary>
        ///card库
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket getCard(ParamBucket<getCarcOrCardModel> dy)
        {
            if (!dy.AllowPaging)
            {
                dy.PageSize = 10000;
                dy.PageIndex = 1;
            }
            Pagination pagination = new Pagination();
            pagination.rows = dy.PageSize;
            pagination.page = dy.PageIndex;
            var queryStr = JsonConvert.SerializeObject(dy.Data);
            //dy.data 可为空 空为全部
            var data = cbll.GetCPageList(pagination, queryStr, dy.UserId);
            return new ListBucket<CCardEntity> { code = 0, info = "操作成功", Data = data, Total = pagination.records, Success = true };
        }


        /// <summary>
        ///CarcOrCard
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket OperationCarc(ParamBucket<List<CarcEntity>> dy)
        {
            var result = 0;
            try
            {

                var userbll = new UserBLL();
                var user = userbll.GetEntity(dy.UserId);
                cbll.SaveForm(dy.Data, dy.UserId);
                return new ResultBucket { info = "操作成功", code = result, Success = true };
            }
            catch (Exception ex)
            {
                return new ResultBucket { code = 1, info = ex.Message };
            }

        }
        /// <summary>
        ///Card
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket OperationCard(ParamBucket<List<CCardEntity>> dy)
        {
            var result = 0;
            try
            {

                var userbll = new UserBLL();
                var user = userbll.GetEntity(dy.UserId);
                cbll.CSaveForm(dy.Data, dy.UserId);
                return new ResultBucket { info = "操作成功", code = result, Success = true };
            }
            catch (Exception ex)
            {
                return new ResultBucket { code = 1, info = ex.Message };
            }

        }

        /// <summary>
        /// Carc数据操作
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultBucket OperationCarcAndFile()
        {
            var result = 0;
            var json = HttpContext.Current.Request.Form.Get("json");
            try
            {
                var dy = JsonConvert.DeserializeObject<BaseDataModel<CarcOrCardModel>>(json);
                BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
                var user = new UserBLL().GetEntity(dy.userId);
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
                    var fileList = fileBll.GetEntity(DelKeys[i]);
                    string url = Config.GetValue("FilePath") + fileList.FilePath.Replace("~/Resource/AppFile/Carc", "").Replace("/", "\\");
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
                    if (string.IsNullOrEmpty(dy.data.entity.Id))
                    {
                        id = Guid.NewGuid().ToString();
                        dy.data.entity.Id = id;
                    }
                    else
                    {
                        id = dy.data.entity.Id;
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
                        FilePath = "~/Resource/AppFile/Carc/" + fileId + ext,
                        FileType = ext.TrimStart('.'),
                        FileExtensions = ext,
                        Description = ext.ToLower() == ".mp3" ? "音频" : "照片",
                        FileSize = file.ContentLength.ToString(),
                        DeleteMark = 0,
                        CreateDate = DateTime.Now,
                        CreateUserId = user.UserId,
                        CreateUserName = user.RealName

                    };
                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Carc"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Carc");
                    }
                    file.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\Carc\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
                }
                #endregion
                #region 数据操作
                List<CarcEntity> list = new List<CarcEntity>();
                list.Add(dy.data.entity);
                cbll.SaveForm(list, dy.userId);
                #endregion
                return new ResultBucket { info = "操作成功", code = result, Success = true };

            }
            catch (Exception ex)
            {

                return new ResultBucket { info = "操作失败：" + ex.Message, code = 1 };
            }

        }


        /// <summary>
        ///删除 carc
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public ResultBucket delCarc(ParamBucket<string> dy)
        {
            BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL fileBll = new BSFramework.Application.Busines.PublicInfoManage.FileInfoBLL();
            var files = fileBll.GetFilesByRecIdNew(dy.Data);
            cbll.deleteEntity(dy.Data);
            foreach (var item in files)
            {
                string url = Config.GetValue("FilePath") + item.FilePath.Replace("~/Resource/AppFile/Carc", "").Replace("/", "\\");
                if (!string.IsNullOrEmpty(item.FilePath) && System.IO.File.Exists(url))
                {
                    System.IO.File.Delete(url);
                }
            }
            return new ResultBucket { code = 0, info = "操作成功", Success = true };
        }

    }
}