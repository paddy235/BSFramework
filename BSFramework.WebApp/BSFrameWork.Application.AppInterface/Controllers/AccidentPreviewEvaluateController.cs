using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.EducationManage;
using BSFrameWork.Application.AppInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    /// 事故预想点评
    /// </summary>
    [RoutePrefix("api/AccidentPreviewEvaluate")]
    public class AccidentPreviewEvaluateController : BaseApiController
    {
        private readonly EducationAnswerBLL educationAnswerBLL;

        /// <summary>
        /// ctor
        /// </summary>
        public AccidentPreviewEvaluateController()
        {
            educationAnswerBLL = new EducationAnswerBLL();
        }

        /// <summary>
        /// 点评
        /// </summary>
        /// <param name="model">点评</param>
        /// <returns>点评</returns>
        /// <response code="200">ok</response>
        /// <response code="400">验证错误</response>
        [Route("")]
        public IHttpActionResult Post(AccidentAnswerEvaluateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = educationAnswerBLL.Get(model.Id);
            if (entity == null)
            {
                ModelState.AddModelError("model.Id", "无效的答题记录");
                return BadRequest(ModelState);
            }

            entity.AppraiseContent = model.AppraiseContent;
            entity.Grade = model.Grade.ToString("F1");

            educationAnswerBLL.Edit(entity);

            return Ok(model);
        }
    }
}