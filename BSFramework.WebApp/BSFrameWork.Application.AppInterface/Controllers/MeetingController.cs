using BSFramework.Application.Busines.EducationManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.EducationManage;
using BSFramework.Busines.WorkMeeting;
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
    [RoutePrefix("api/Meeting")]
    public class MeetingController : BaseApiController
    {
        private readonly WorkmeetingBLL workmeetingBLL;
        private readonly EducationBLL educationBLL;

        /// <summary>
        /// ctor
        /// </summary>
        public MeetingController()
        {
            workmeetingBLL = new WorkmeetingBLL();
            educationBLL = new EducationBLL();
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>班会</returns>
        /// <response code="200">成功</response>
        /// <response code="404">无效的资源</response>
        [Route("{id}", Name = "Meeting")]
        [ResponseType(typeof(MeetingModel))]
        public IHttpActionResult Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var data = workmeetingBLL.Get(id);
            if (data == null)
                return NotFound();

            return Ok(new MeetingModel { MeetingId = data.MeetingId });
        }

        /// <summary>
        /// 班会事故预想
        /// </summary>
        /// <param name="id">班会</param>
        /// <returns>事故预想</returns>
        /// <response code="200">成功</response>
        /// <response code="404">无效的资源</response>
        [Route("{id}/AccidentPreview")]
        [ResponseType(typeof(List<AccidentPreviewModel>))]
        public IHttpActionResult GetAccidentPreview(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var data = educationBLL.FilterByMeeting(id);
            var list = data.Select(x => new AccidentPreviewModel
            {
                Id = x.ID,
                Theme = x.Theme,
                Teacher = x.Teacher,
                TeacherId = x.TeacherId,
                RegisterPeople = x.RegisterPeople,
                RegisterPeopleId = x.RegisterPeopleId,
                AttendPeople = x.AttendPeople,
                AttendPeopleId = x.AttendPeopleId,
                AttendNumber = x.AttendNumber,
                ActivityDate = x.ActivityDate,
                ActivityEndDate = x.ActivityEndDate,

            }).ToList();
            return Ok(list);
        }
    }
}