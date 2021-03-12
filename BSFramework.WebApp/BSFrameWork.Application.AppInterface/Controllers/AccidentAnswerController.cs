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
    /// 事故预想
    /// </summary>
    [RoutePrefix("api/AccidentAnswer")]
    public class AccidentAnswerController : BaseApiController
    {
        private readonly EducationAnswerBLL educationAnswerBLL;

        /// <summary>
        /// ctor
        /// </summary>
        public AccidentAnswerController()
        {
            educationAnswerBLL = new EducationAnswerBLL();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns>答题内容</returns>
        [Route("")]
        [ResponseType(typeof(List<AccidentAnswerModel>))]
        public IHttpActionResult Get(int pageSize = 10, int pageIndex = 1)
        {
            var data = educationAnswerBLL.List(null);
            var list = data.Select(x => new AccidentAnswerModel
            {
                Id = x.ID,
                AnswerPeople = x.AnswerPeople,
                AnswerPeopleId = x.AnswerPeopleId,
                AnswerContent = x.AnswerContent,
                Description = x.Description,
                Question = x.Question
            }).ToList();
            return Ok(list);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>答题内容</returns>
        /// <response code="404">无效的资源</response>
        [Route("{id}", Name = "AccidentAnswer")]
        [ResponseType(typeof(AccidentAnswerModel))]
        public IHttpActionResult Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var entity = educationAnswerBLL.Get(id);
            if (entity == null)
                return NotFound();

            var model = new AccidentAnswerModel()
            {
                Id = entity.ID,
                AnswerPeople = entity.AnswerPeople,
                AnswerPeopleId = entity.AnswerPeopleId,
                AnswerContent = entity.AnswerContent,
                Description = entity.Description,
                Question = entity.Question
            };

            return Ok(model);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns>答题内容</returns>
        /// <response code="201">成功</response>
        /// <response code="400">验证错误</response>
        [Route("")]
        [ResponseType(typeof(AccidentAnswerModel))]
        public IHttpActionResult Post(AccidentAnswerModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.Id = Guid.NewGuid().ToString();
            var currentUser = OperatorProvider.Provider.Current();

            var entity = new EduAnswerEntity()
            {
                ID = model.Id,
                CreateDate = DateTime.Now,
                CreateUser = currentUser.UserId,
                AnswerPeople = model.AnswerPeople,
                AnswerPeopleId = model.AnswerPeopleId,
                AnswerContent = model.AnswerContent,
                Description = model.Description,
                Question = model.Question
            };

            educationAnswerBLL.Add(entity);

            return CreatedAtRoute("AccidentAnswer", new { id = model.Id }, model);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="model">答题内容</param>
        /// <returns>no content</returns>
        /// <response code="200">成功</response>
        /// <response code="400">验证错误</response>
        /// <response code="404">无效的资源</response>
        [Route("{id}")]
        public IHttpActionResult Put(string id, AccidentAnswerModel model)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = educationAnswerBLL.Get(id);
            if (entity == null)
                return NotFound();

            entity.AnswerPeople = model.AnswerPeople;
            entity.AnswerPeopleId = model.AnswerPeopleId;
            entity.AnswerContent = model.AnswerContent;
            entity.Description = model.Description;
            entity.Question = model.Question;

            educationAnswerBLL.Edit(entity);

            return Ok();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>答题内容</returns>
        /// <response code="404">无效的资源</response>
        [Route("{id}")]
        [ResponseType(typeof(AccidentAnswerModel))]
        public IHttpActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var entity = educationAnswerBLL.Get(id);
            if (entity == null)
                return Ok();

            educationAnswerBLL.Delete(entity);

            var model = new AccidentAnswerModel()
            {
                Id = entity.ID,
                AnswerPeople = entity.AnswerPeople,
                AnswerPeopleId = entity.AnswerPeopleId,
                AnswerContent = entity.AnswerContent,
                Description = entity.Description,
                Question = entity.Question
            };

            return Ok(model);
        }
    }
}