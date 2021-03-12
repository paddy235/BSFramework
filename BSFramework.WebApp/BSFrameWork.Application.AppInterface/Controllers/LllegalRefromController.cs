using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Http;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.LllegalManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.LllegalManage;
using BSFramework.Application.Entity.PublicInfoManage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class LllegalRefromController : ApiController
    {
        PeopleBLL pbll = new PeopleBLL();
        UserBLL ubll = new UserBLL();
        LllegalBLL lbll = new LllegalBLL();
        LllegalRefromBLL refrombll = new LllegalRefromBLL();
        /// <summary>
        /// 获取违章已核准列表
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetLllegalRefromList([FromBody]JObject json)
        {
            try
            {
                string res = json.Value<string>("json");
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(res);
                //根据登陆人员Id找到所属班组Id
                string userId = dy.userId;
                string ApproveResult = dy.data.ApproveResult;
                UserEntity user = ubll.GetEntity(userId);

                long pageSize = 0, pageIndex = 0;
                string from = dy.data.startTime;
                string to = dy.data.endTime;
                pageSize = dy.data.pageSize;
                pageIndex = dy.data.pageIndex;
                int total = 0;
                List<LllegalEntity> lList = lbll.GetLllegalList(from, to, int.Parse(pageIndex.ToString()), int.Parse(pageSize.ToString()), user.DepartmentId, ApproveResult, out total).ToList();
                for (int i = 0; i < lList.Count; i++)
                {
                    IList fileList = new FileInfoBLL().GetFilesByRecId(lList[i].ID, BSFramework.Util.Config.GetValue("AppUrl"));
                    lList[i].fileList = fileList;
                }
                lList.Select(t => new
                {
                    t.ID,
                    t.fileList,
                    t.LllegalAddress,
                    t.LllegalDepart,
                    t.LllegalDepartCode,
                    t.LllegalDescribe,
                    t.LllegalLevel,
                    t.LllegalNumber,
                    t.LllegalPerson,
                    t.LllegalPersonId,
                    t.LllegalTeam,
                    t.LllegalTeamId,
                    t.LllegalType,
                    t.RegisterPerson,
                    t.RegisterPersonId,
                    t.Remark,
                    LllegalTime = t.LllegalTime.ToString("yyyy-MM-dd")
                }).ToList();
                return new { info = "成功", code = 0, count = total, data = lList };
            }
            catch (Exception ex)
            {

                return new { info = "失败：" + ex.Message, code = 1, data = new { } };
            }
        }

        /// <summary>
        /// 违章整改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object AddLllegalRefrom()
        {
            try
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(HttpContext.Current.Request["json"]);
                string RefromEntity = JsonConvert.SerializeObject(dy.data.RefromEntity);
                LllegalRefromEntity entity = JsonConvert.DeserializeObject<LllegalRefromEntity>(RefromEntity);
                //entity.Id = Guid.NewGuid().ToString();
                refrombll.SaveFrom(entity);

                LllegalEntity l = lbll.GetEntity(entity.LllegalId);
                l.FlowState = "待验收";
                lbll.SaveForm(entity.LllegalId, l);

                FileInfoBLL fileBll = new FileInfoBLL();
                LllegalRefromEntity lr = refrombll.GetEntityByLllegalId(entity.LllegalId);
                if (lr != null)
                {
                    var flist = fileBll.GetFilesByRecIdNew(lr.Id);
                    foreach (FileInfoEntity f in flist)
                    {
                        fileBll.Delete(f.FileId);
                    }
                }

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
                        FolderId = lr.Id,
                        RecId = lr.Id,
                        FileName = System.IO.Path.GetFileName(hf.FileName),
                        FilePath = "~/Resource/AppFile/LllegalRefrom/" + fileId + ext,
                        FileType = System.IO.Path.GetExtension(hf.FileName),
                        FileExtensions = ext,
                        FileSize = hf.ContentLength.ToString(),
                        DeleteMark = 0
                    };

                    //上传附件到服务器
                    if (!System.IO.Directory.Exists(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\LllegalRefrom"))
                    {
                        System.IO.Directory.CreateDirectory(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\LllegalRefrom");
                    }
                    hf.SaveAs(BSFramework.Util.Config.GetValue("FilePath") + "\\AppFile\\LllegalRefrom\\" + fileId + ext);
                    //保存附件信息
                    fileBll.SaveForm(fi);
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
