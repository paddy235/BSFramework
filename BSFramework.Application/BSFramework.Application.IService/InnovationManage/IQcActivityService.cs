using BSFramework.Application.Entity.InnovationManage;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.InnovationManage
{
    public interface IQcActivityService
    {
        /// <summary>
        /// 获取qc数据
        /// </summary>
        /// <returns></returns>
        List<QcActivityEntity> getQcList(Dictionary<string, string> keyValue, Pagination pagination);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        QcActivityEntity getQcById(string keyvalue);

        /// <summary>
        /// 新增数据qc活动数据
        /// </summary>
        void addEntity(QcActivityEntity qc);
        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="qc"></param>
        void EditEntity(QcActivityEntity qc);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="qc"></param>
        void delEntity(QcActivityEntity qc);
        int GetQcTimes(string deptid);
    }
}
