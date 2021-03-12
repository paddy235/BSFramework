using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Application.IService.InnovationManage;
using BSFramework.Application.Service.InnovationManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.InnovationManage
{
    public class AdviceBLL
    {
        private IAdviceService service = new AdviceService();
        private FileInfoBLL fileBll = new FileInfoBLL();
        /// <summary>
        /// 获取Advice数据
        /// </summary>
        /// <returns></returns>
        public List<AdviceEntity> getAdviceList(Dictionary<string, string> keyValue, Pagination pagination, bool ispage = false)
        {
            if (!ispage)
            {
                if (pagination == null)
                {
                    pagination = new Pagination();
                    pagination.page = 1;
                    pagination.rows = 2000;
                }
            }

            var data = service.getAdviceList(keyValue, pagination);

            foreach (var item in data)
            {
                item.audit = getAuditByid(item.adviceid).OrderBy(x => x.sort).ToList();
                var fileList = fileBll.GetFilesByRecIdNew(item.adviceid);
                //foreach (var items in fileList)
                //{
                //    items.FilePath = items.FilePath.Replace("~/", url);
                //}
                item.Photos = fileList.Where(x => x.Description == "照片").ToList();
                item.Files = fileList.Where(x => x.Description == "文件").ToList();
            }

            return data;
        }

        /// <summary>
        /// 获取Advice数据
        /// </summary>
        /// <returns></returns>
        public List<AdviceEntity> getAdviceListIndex(Dictionary<string, string> keyValue, Pagination pagination,  bool ispage = false)
        {
            if (!ispage)
            {
                if (pagination == null)
                {
                    pagination = new Pagination();
                    pagination.page = 1;
                    pagination.rows = 2000;
                }
            }
            var list = new List<AdviceEntity>();
            var data = service.getAdviceList(keyValue, pagination);
            if (data.Count > 0)
            {
                var state = data.Where(x => x.aduitresult == "待审核").ToList();
                if (state.Count > 0)
                {
                    list.AddRange(state);
                }
                state = data.Where(x => x.aduitresult == "审核通过").ToList();
                if (state.Count > 0)
                {
                    list.AddRange(state);
                }
                state = data.Where(x => x.aduitresult == "审核不通过").ToList();
                if (state.Count > 0)
                {
                    list.AddRange(state);
                }
            }
            foreach (var item in list)
            {
                item.audit = getAuditByid(item.adviceid).OrderBy(x => x.sort).ToList();
                var fileList = fileBll.GetFilesByRecIdNew(item.adviceid);
                //foreach (var items in fileList)
                //{
                //    items.FilePath = items.FilePath.Replace("~/", url);
                //}
                item.Photos = fileList.Where(x => x.Description == "照片").ToList();
                item.Files = fileList.Where(x => x.Description == "文件").ToList();
            }

            return list;


        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<AdviceEntity> getAdvicebyuser(string userid)
        {
            var data = service.getAdvicebyuser(userid).OrderByDescending(x => x.reporttime).ToList();
            var list = new List<AdviceEntity>();
            if (data.Count > 0)
            {
                var state = data.Where(x => x.aduitresult == "待审核").ToList();
                if (state.Count > 0)
                {
                    list.AddRange(state);
                }
                state = data.Where(x => x.aduitresult == "审核通过").ToList();
                if (state.Count > 0)
                {
                    list.AddRange(state);
                }
                state = data.Where(x => x.aduitresult == "审核不通过").ToList();
                if (state.Count > 0)
                {
                    list.AddRange(state);
                }
            }
            foreach (var item in list)
            {
                item.audit = getAuditByid(item.adviceid).OrderBy(x => x.sort).ToList();
                var fileList = fileBll.GetFilesByRecIdNew(item.adviceid);
                //foreach (var items in fileList)
                //{
                //    items.FilePath = items.FilePath.Replace("~/", url);
                //}
                item.Photos = fileList.Where(x => x.Description == "照片").ToList();
                item.Files = fileList.Where(x => x.Description == "文件").ToList();
            }
            return list;
        }

        /// <summary>
        /// 审核人员获取数据
        /// </summary>
        /// <returns></returns>
        public List<AdviceEntity> getAdviceByidExtensions(string userid)
        {
            var audit = getAuditByuser(userid).Where(x => string.IsNullOrEmpty(x.state)).ToList();
            if (audit.Count > 0)
            {
                string Strid = string.Join(",", audit.Select(x => x.adviceid));
                var data = getAdvicebyid(Strid).Where(x => x.aduitstate != "待提交" && x.aduitresult == "待审核").OrderByDescending(x => x.reporttime).ToList();
                var list = new List<AdviceEntity>();
                if (data.Count > 0)
                {
                    var state = data.Where(x => x.aduitresult == "待审核").ToList();
                    if (state.Count > 0)
                    {
                        list.AddRange(state);
                    }
                    state = data.Where(x => x.aduitresult == "审核通过").ToList();
                    if (state.Count > 0)
                    {
                        list.AddRange(state);
                    }
                    state = data.Where(x => x.aduitresult == "审核不通过").ToList();
                    if (state.Count > 0)
                    {
                        list.AddRange(state);
                    }
                }
                foreach (var item in list)
                {
                    item.audit = getAuditByid(item.adviceid).OrderBy(x => x.sort).ToList();
                    var fileList = fileBll.GetFilesByRecIdNew(item.adviceid);
                    //foreach (var items in fileList)
                    //{
                    //    items.FilePath = items.FilePath.Replace("~/", url);
                    //}
                    item.Photos = fileList.Where(x => x.Description == "照片").ToList();
                    item.Files = fileList.Where(x => x.Description == "文件").ToList();
                }
                return list;
            }
            else
            {
                return new List<AdviceEntity>();
            }
        }

        public int GetSuggestions(string deptid)
        {
            return service.GetSuggestions(deptid);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public List<AdviceEntity> getAdviceByid(string Strid)
        {

            var data = getAdvicebyid(Strid).OrderByDescending(x => x.reporttime).ToList();
            var list = new List<AdviceEntity>();
            if (data.Count > 0)
            {
                var state = data.Where(x => x.aduitresult == "待审核").ToList();
                if (state.Count > 0)
                {
                    list.AddRange(state);
                }
                state = data.Where(x => x.aduitresult == "审核通过").ToList();
                if (state.Count > 0)
                {
                    list.AddRange(state);
                }
                state = data.Where(x => x.aduitresult == "审核不通过").ToList();
                if (state.Count > 0)
                {
                    list.AddRange(state);
                }
            }
            foreach (var item in data)
            {
                item.audit = getAuditByid(item.adviceid).OrderBy(x => x.sort).ToList();
                var fileList = fileBll.GetFilesByRecIdNew(item.adviceid);
                //foreach (var items in fileList)
                //{
                //    items.FilePath = items.FilePath.Replace("~/", url);
                //}
                item.Photos = fileList.Where(x => x.Description == "照片").ToList();
                item.Files = fileList.Where(x => x.Description == "文件").ToList();
            }
            return list;
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="Strid"></param>
        /// <returns></returns>
        public List<AdviceEntity> getAdvicebyid(string Strid)
        {
            return service.getAdvicebyid(Strid);
        }


        /// <summary>
        /// 数据操作
        /// </summary>
        /// <param name="add"></param>
        /// <param name="update"></param>
        /// <param name="del"></param>
        /// <param name="audit"></param>
        public void Operation(AdviceEntity add, AdviceEntity update, string del, AdviceAuditEntity audit, AdviceAuditEntity auditupdate)
        {
            try
            {
                if (audit != null)
                {
                    if (add != null)
                    {
                        if (string.IsNullOrEmpty(add.aduitstate))
                        {
                            add.aduitstate = "待审核";
                            add.aduitresult = "待审核";

                        }
                        audit.adviceid = add.adviceid;
                    }

                    audit.auditid = Guid.NewGuid().ToString();
                    var list = getAuditByid(audit.adviceid);
                    audit.sort = list.Count() + 1;
                }
                service.Operation(add, update, del, audit, auditupdate);
                //发消息
                MessageBLL msgBll = new MessageBLL();
                if (audit != null)
                {
                    //待审批
                    msgBll.SendMessage("合理化建议审批", audit.adviceid);
                }
                else if (auditupdate != null)
                {
                    if (auditupdate.state == "审核通过")
                    {
                        msgBll.SendMessage("合理化建议审批通过", auditupdate.adviceid);
                    }
                    else if (auditupdate.state == "审核不通过")
                    {
                        msgBll.SendMessage("合理化建议审批不通过", auditupdate.adviceid);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 获取审核记录
        /// </summary>
        /// <param name="officeid"></param>
        /// <returns></returns>
        public List<AdviceAuditEntity> getAuditByid(string officeid)
        {

            return service.getAuditByid(officeid);
        }


        /// <summary>
        /// 获取审核记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<AdviceAuditEntity> getAuditByuser(string userid)
        {
            return service.getAuditByuser(userid);

        }

        /// 获取审核记录
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<AdviceAuditEntity> getAuditId(string Id)
        {
            return service.getAuditId(Id);

        }

        public int GetTodoCount(string userId)
        {
            return service.GetTodoCount(userId);
        }
    }
}
