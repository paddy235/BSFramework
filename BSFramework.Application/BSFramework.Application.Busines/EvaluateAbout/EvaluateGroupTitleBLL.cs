using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.IService.EvaluateAbout;
using BSFramework.Application.Service.EvaluateAbout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.EvaluateAbout
{
   public  class EvaluateGroupTitleBLL 
    {
        private IEvaluateGroupTitleService service = new EvaluateGroupTitleService();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId">班组Id</param>
        /// <param name="bK1">考评的Id</param>
        public void Remove(string groupId, string bK1)
        {
            service.Remove(groupId,bK1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId">班组的ID</param>
        /// <param name="evaluateId">考评的Id</param>
        /// <returns></returns>
        public EvaluateGroupTitleEntity GetEntity(string titleId)
        {
            return service.GetEntity(titleId);
        }

        public void Update(EvaluateGroupTitleEntity oldEntity)
        {
            service.Update(oldEntity);
        }

        public void Insert(EvaluateGroupTitleEntity entity)
        {
            service.Insert(entity);
        }

        public string GetTitleNameByGroupId(string deptId,string evaluateId)
        {
            return service.GetTitleNameByGroupId(deptId,evaluateId);
        }

        public void Remove(string titleId)
        {
            service.Remove(titleId);
        }
    }
}
