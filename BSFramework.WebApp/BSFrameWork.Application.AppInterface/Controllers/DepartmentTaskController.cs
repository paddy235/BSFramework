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
    public class DepartmentTaskController : BaseApiController
    {
        [HttpPost]
        public ListBucket<DepartmentTaskEntity> List(ParamBucket<string> args)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new DepartmentTaskBLL();
            var total = 0;
            var data = bll.List(user.DeptId, args.Data, args.PageSize, args.PageIndex, out total);
            var date = DateTime.Now;
            foreach (var item in data)
            {
                if (item.Status == "未开始")
                {
                    if (item.StartDate.Value <= date && item.EndDate.Value >= date) item.Status = "进行中";
                    else if (item.EndDate <= date) item.Status = "未完成";
                }

                if (item.SubTasks != null)
                {
                    foreach (var item1 in item.SubTasks)
                    {
                        if (item1.Status == "未开始")
                        {
                            if (item1.StartDate.Value <= date && item1.EndDate.Value >= date) item1.Status = "进行中";
                            else if (item1.EndDate <= date) item1.Status = "未完成";
                        }
                    }
                }
            }

            return new ListBucket<DepartmentTaskEntity>() { Success = true, Data = data };
        }

        [HttpPost]
        public ModelBucket<DepartmentTaskEntity> Detail(ParamBucket<string> args)
        {
            var url = ConfigurationManager.AppSettings["AppUrl"].ToString();
            var bll = new DepartmentTaskBLL();
            var data = bll.Detail(args.Data);
            if (data.Files != null)
            {
                data.Files.ForEach(x => x.FilePath = url + x.FilePath.Replace("~/", string.Empty));
            }
            return new ModelBucket<DepartmentTaskEntity>() { Success = true, Data = data };
        }

        [HttpPost]
        public ListBucket<DepartmentTaskEntity> List1(ParamBucket<DeaprtmentTaskModel> args)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new DepartmentTaskBLL();
            var total = 0;
            var data = bll.List1(user.DeptId, args.Data.Begin, args.Data.End, args.Data.Status, args.PageSize, args.PageIndex, out total);
            var date = DateTime.Today;
            foreach (var item in data)
            {
                if (item.Status == "未开始")
                {
                    if (item.StartDate.Value <= date && item.EndDate.Value >= date) item.Status = "进行中";
                    else if (item.EndDate <= date) item.Status = "未完成";
                }
            }
            return new ListBucket<DepartmentTaskEntity>() { Success = true, Data = data, Total = total };
        }

        [HttpPost]
        public ResultBucket Complete(ParamBucket<DepartmentTaskEntity> args)
        {
            var user = OperatorProvider.Provider.Current();
            var bll = new DepartmentTaskBLL();
            bll.Complete(args.Data);
            return new ResultBucket() { Success = true };
        }
    }
}
