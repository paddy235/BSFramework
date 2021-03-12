using BSFramework.Application.Entity.SetManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.SetManage
{
    public interface IMeasureSetService
    {
        /// <summary>
        /// 得到危险因素下的防范措施列表
        /// </summary>
        /// <param name="riskFactorId">危险因素ID</param>
        /// <returns></returns>
        IEnumerable<MeasureSetEntity> GetList(string riskFactorId);

        /// <summary>
        /// 删除防范措施
        /// </summary>
        /// <param name="keyValue">防范措施ID</param>
        void Delete(string keyValue);
        /// <summary>
        /// 新增一条防范措施
        /// </summary>
        /// <param name="entity"></param>
        void Insert(MeasureSetEntity entity);

    }
}
