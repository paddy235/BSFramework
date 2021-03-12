using BSFramework.Application.Entity.EducationManage;
using BSFramework.Application.IService.EducationManage;
using BSFramework.Application.Service.EducationManage;
using System;
using System.Collections.Generic;

namespace BSFramework.Application.Busines.EducationManage
{
    public class EducationAnswerBLL
    {
        private readonly IEduAnswerService eduAnswerService;
        public EducationAnswerBLL()
        {
            eduAnswerService = new EduAnswerService();
        }

        public void Add(EduAnswerEntity entity)
        {
            eduAnswerService.Add(entity);
        }

        public EduAnswerEntity Get(string id)
        {
            return eduAnswerService.Get(id);
        }

        public void Edit(EduAnswerEntity entity)
        {
            eduAnswerService.Edit(entity);
        }

        public void Delete(EduAnswerEntity entity)
        {
            eduAnswerService.Delete(entity);
        }

        public List<EduAnswerEntity> List(string baseId)
        {
            return eduAnswerService.List(baseId);
        }
    }
}
