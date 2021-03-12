using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 安全交底
    /// </summary>
    public class DangerAnalysisController : MvcControllerBase
    {
        public DangerAnalysisBLL _bll;

        public DangerAnalysisController()
        {
            _bll = new DangerAnalysisBLL();
        }


        // GET: Works/DangerAnalists
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var model = _bll.GetLast(user.DeptId);
            if (model == null)
            {
                model = new DangerAnalysisEntity()
                {
                    AnalysisId = Guid.NewGuid().ToString(),
                    DeptId = user.DeptId,
                    DeptName = user.DeptName,
                };
                _bll.Init(model);
            }
            ViewBag.id = model.AnalysisId;
            return View(model);
        }

        [HttpGet]
        public ViewResult Edit(string id, string jobid)
        {
            var data = default(JobDangerousEntity);
            if (string.IsNullOrEmpty(id))
                data = new JobDangerousEntity() { JobId = jobid };
            else
                data = _bll.GetDanger(id);
            return View(data);
        }

        [HttpPost]
        public JsonResult Edit(string id, DangerAnalysisModel model)
        {
            var danger = new JobDangerousEntity()
            {
                JobDangerousId = id,
                Content = model.Danger,
                CreateTime = DateTime.Now,
                JobId = model.JobId,
                MeasureList = new List<JobMeasureEntity>()
            };
            if (string.IsNullOrEmpty(id)) danger.JobDangerousId = Guid.NewGuid().ToString();
            for (int i = 0; i < model.Measures.Count; i++)
            {
                danger.MeasureList.Add(new JobMeasureEntity
                {
                    Content = model.Measures[i],
                    CreateTime = danger.CreateTime.AddSeconds(i),
                    JobDangerousId = danger.JobDangerousId,
                    JobMeasureId = Guid.NewGuid().ToString()
                });
            }

            _bll.EditDanger(danger);

            return Json(model);
        }

        public JsonResult DeleteDanger(string id)
        {
            _bll.DeleteDanger(id);
            return Json("ok");
        }

        public JsonResult FindDanger(string query, int limit)
        {
            var user = OperatorProvider.Provider.Current();
            var data = _bll.FindDanger(query, limit, user.DeptId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Prev(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var data = _bll.Prev(user.DeptId, id);
            ViewBag.id = id;
            return View(data);
        }

        public JsonResult Copy(string id, JobDangerousEntity model)
        {
            _bll.Copy(model);
            return Success();
        }
    }
}
