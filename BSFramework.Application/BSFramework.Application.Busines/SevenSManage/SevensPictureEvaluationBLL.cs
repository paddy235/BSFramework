using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.SevenSManage;
using BSFramework.Application.IService.SevenSManage;
using BSFramework.Application.Service.SevenSManage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SevenSManage
{
  public  class SevensPictureEvaluationBLL
    {
        private ISevensPictureEvaluationService service;
        public SevensPictureEvaluationBLL()
        {
            service = new SevensPictureEvaluationService();
        }

        public void Insert(SevensPictureEvaluationEntity evaluationEntity)
        {
            service.Insert(evaluationEntity);
        }

        /// <summary>
        /// 获取某条定点拍照数据的所有评论
        /// </summary>
        /// <param name="dataid"></param>
        /// <returns></returns>
        public List<SevensPictureEvaluationEntity> GetList(string dataid)
        {
            return service.GetList(dataid);
        }

        /// <summary>
        /// 查询用户的历史记录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="deptid">部门Id</param>
        /// <returns></returns>
        public List<SevensPictureEvaluationEntity> GetHistory(string userId, string deptid)
        {
            return service.GetHistory(userId,deptid);
        }

        /// <summary>
        /// 查询定点拍照的评论
        /// </summary>
        /// <param name="dataid"></param>
        /// <returns></returns>
        public IList GetActivityEvaluateList(string dataid)
        {
            return service.GetActivityEvaluateList(dataid);
        }

        public IList GetHistory(string userId)
        {
            return service.GetHistory(userId);
        }
    }
}
