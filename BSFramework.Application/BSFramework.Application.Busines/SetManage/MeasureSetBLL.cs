using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.IService.SetManage;
using BSFramework.Application.Service.SetManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.SetManage
{
    public class MeasureSetBLL
    {
        IMeasureSetService service = new MeasureSetService();

        /// <summary>
        /// 得到危险因素下的防范措施列表
        /// </summary>
        /// <param name="riskFactorId">危险因素ID</param>
        /// <returns></returns>
        public IEnumerable<MeasureSetEntity> GetList(string riskFactorId)
        {
            return service.GetList(riskFactorId);
        }
        /// <summary>
        /// 删除防范措施
        /// </summary>
        /// <param name="keyValue">防范措施ID</param>
        public void Delete(string keyValue)
        {
            service.Delete(keyValue);
        }
        /// <summary>
        /// 新增一条防范措施
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(MeasureSetEntity entity)
        {
            service.Insert(entity);
        }
    }
}
