using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PerformanceManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.PerformanceManage
{
    public interface IPerformancesetupSecondService
    {
        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <returns></returns>
        List<PerformancesetupSecondEntity> AllTitle(string departmentid);
        /// <summary>
        /// 操作配置  对应修改当前月标题和数据
        /// </summary>
        void operation(List<PerformancesetupSecondEntity> add, List<PerformancesetupSecondEntity> del, List<PerformancesetupSecondEntity> Listupdate, PerformancetitleSecondEntity title, List<PerformanceSecondEntity> Score, PerformancePersonSecondEntity person);


        /// <summary>
        /// 清理数据
        /// </summary>
        /// <param name="del"></param>
         void remove(List<PerformanceSecondEntity> del);
        /// <summary>
        /// 获取所有人员配置
        /// </summary>
        /// <returns></returns>
        PerformancePersonSecondEntity getDeptPerson(string departmentid);

        /// <summary>
        /// 修改新增人员信息
        /// </summary>
        /// <param name="entity"></param>
        void SetDeptPerson(PerformancePersonSecondEntity entity, UserEntity user);

        /// <summary>
        ///获取特殊配置列表
        /// </summary>
        /// <returns></returns>
        List<PerformanceMethodSecondEntity> getisspecial(string performancetypeid);

        /// <summary>
        /// 新增特殊配置
        /// </summary>
        /// <param name="entity"></param>
        void Setisspecial(List<PerformanceMethodSecondEntity> entity);
      
    }
}
