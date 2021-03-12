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
    [RoutePrefix("api/AccidentPreview")]
    public class AccidentPreviewController : BaseApiController
    {
        private readonly EducationBLL educationBLL;
        private readonly EducationAnswerBLL educationAnswerBLL;

        /// <summary>
        /// ctor
        /// </summary>
        public AccidentPreviewController()
        {
            educationBLL = new EducationBLL();
            educationAnswerBLL = new EducationAnswerBLL();
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns>事故预想列表</returns>
        [Route("")]
        [ResponseType(typeof(List<AccidentPreviewModel>))]
        public IHttpActionResult Get(int pageSize = 10, int pageIndex = 1)
        {
            var data = educationBLL.List(10, 1);
            var list = data.Select(x => new AccidentPreviewModel
            {
                Id = x.ID,
                Theme = x.Theme,
                Teacher = x.Teacher,
                TeacherId = x.TeacherId,
                RegisterPeople = x.RegisterPeople,
                RegisterPeopleId = x.RegisterPeopleId,
            }).ToList();
            return Ok(list);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>事故预想</returns>
        /// <response code="404">无效的资源</response>
        [Route("{id}", Name = "AccidentPreview")]
        [ResponseType(typeof(AccidentPreviewModel))]
        public IHttpActionResult Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var entity = educationBLL.Get(id);
            if (entity == null)
                return NotFound();

            var model = new AccidentPreviewModel()
            {
                Id = entity.ID,
                Theme = entity.Theme,
                Teacher = entity.Teacher,
                TeacherId = entity.TeacherId,
                RegisterPeople = entity.RegisterPeople,
                RegisterPeopleId = entity.RegisterPeopleId,
            };

            return Ok(model);
        }

        /// <summary>
        /// 答题
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>答题内容列表</returns>
        [Route("{id}/Answers")]
        [ResponseType(typeof(List<AccidentAnswerModel>))]
        public IHttpActionResult GetAnswers(string id)
        {
            var data = educationAnswerBLL.List(id);
            var list = data.Select(x => new AccidentAnswerModel
            {
                Id = x.ID,
                AnswerPeople = x.AnswerPeople,
                AnswerPeopleId = x.AnswerPeopleId,
                AnswerContent = x.AnswerContent,
                Description = x.Description,
                Question = x.Question,
                Grade = x.Grade,
                AppraiseContent = x.AppraiseContent,
            }).ToList();
            return Ok(list);

        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">事故预想</param>
        /// <returns></returns>
        [Route("")]
        public IHttpActionResult Post(AccidentPreviewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUser = OperatorProvider.Provider.Current();

            model.Id = Guid.NewGuid().ToString();
            var entity = new EduBaseInfoEntity
            {
                ID = model.Id,
                CreateDate = DateTime.Now,
                CreateUser = currentUser.UserId,
                BZId = currentUser.DeptId,
                Theme = model.Theme,
                Teacher = model.Teacher,
                TeacherId = model.TeacherId,
                RegisterPeople = model.RegisterPeople,
                RegisterPeopleId = model.RegisterPeopleId,
                Answers = model.AnswerList.Select(x => new EduAnswerEntity { ID = x }).ToList(),
                Flow = "-1",
                EduType = "3",
                BZName = currentUser.DeptName,
                MeetingId = model.MeetingId
            };

            educationBLL.Add(entity);

            return CreatedAtRoute("AccidentPreview", new { id = model.Id }, model);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="model">事故预想</param>
        /// <returns></returns>
        [Route("{id}")]
        public IHttpActionResult Put(string id, AccidentPreviewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = new EduBaseInfoEntity
            {
                ID = model.Id,
                CreateDate = DateTime.Now,
                Theme = model.Theme,
                Teacher = model.Teacher,
                TeacherId = model.TeacherId,
                RegisterPeople = model.RegisterPeople,
                RegisterPeopleId = model.RegisterPeopleId,
                Answers = model.AnswerList.Select(x => new EduAnswerEntity { ID = x }).ToList(),
                Flow = "-1",
                EduType = "3",
                MeetingId = model.MeetingId
            };

            educationBLL.Modify(entity);

            return Ok("success");
        }

        ///// <summary>
        ///// 删除
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[Route("{id}")]
        //public IHttpActionResult Delete(string id)
        //{
        //    return Ok();
        //}
    }
}