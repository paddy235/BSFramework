using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.BudgetAbout;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.BudgetAbout;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Bst.Fx.MessageData;
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
    public class MessageController : ApiController
    {
        [HttpPost]
        public ListBucket<MessageEntity> GetMessage(ParamBucket arg)
        {
            var bll = new MessageBLL();
            var total = 0;
            var data = bll.GetMessage(arg.UserId, out total);
            return new ListBucket<MessageEntity>() { Data = data, Total = total, code = 0 };
        }

        [HttpPost]
        public ModelBucket<int> GetMessageTotal(ParamBucket arg)
        {
            var bll = new MessageBLL();
            var total = bll.GetMessageTotal(arg.UserId);
            return new ModelBucket<int>() { Data = total, code = 0 };
        }

        [HttpPost]
        public ListBucket<MessageEntity> GetTodo(ParamBucket arg)
        {
            var bll = new MessageBLL();
            var total = 0;
            var data = bll.GetTodo(arg.UserId, out total);
            return new ListBucket<MessageEntity>() { Data = data, Total = total, code = 0 };
        }

        [HttpPost]
        public ModelBucket<MessageEntity> GetMessageDetail(ParamBucket<string> arg)
        {
            var bll = new MessageBLL();
            var total = 0;
            var data = bll.GetMessageDetail(Guid.Parse(arg.Data));
            return new ModelBucket<MessageEntity>() { Data = data, code = 0 };
        }

        [HttpPost]
        public ResultBucket FinishTodo(string businessId)
        {
            var bll = new MessageBLL();
            var success = true;
            var msg = string.Empty;
            try
            {
                bll.FinishTodo(businessId, null);
            }
            catch (Exception e)
            {
                success = false;
                msg = e.Message;
            }
            return new ResultBucket() { code = success ? 0 : 1, info = msg };
        }

        [HttpPost]
        public ResultBucket SendMessage(ParamBucket<string> arg)
        {
            var userbll = new UserBLL();
            var user = userbll.GetEntity(arg.UserId);

            var bll = new MessageBLL();
            var success = true;
            var msg = string.Empty;
            try
            {
                bll.SendMessage("工作提示", "00264a7c-a55d-4e25-b979-0ae6289a1d51");
            }
            catch (Exception e)
            {
                success = false;
                msg = e.Message;
            }
            return new ResultBucket() { code = success ? 0 : 1, info = msg };
        }
    }
}
