using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.WebApp
{
    public interface IUserWorkAllocationService
    {
        /// <summary>
        /// 获取转移到部门的成员
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserWorkAllocationEntity> GetSendUser(string deptId);
        /// <summary>
        /// 增改实体
        /// </summary>
        /// <param name="entity"></param>
        string OperationEntity(UserWorkAllocationEntity entity,string TID);
        /// <summary>
        /// 获取未推送完成的成员
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        IEnumerable<UserWorkAllocationEntity> GetIsSendUser(string deptId);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        UserWorkAllocationEntity GetDetailByUser(string id);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        UserWorkAllocationEntity GetDetail(string id);


        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        List<DepartmentEntity> GetSubDepartments(string id, string category);

        /// <summary>
        /// 离厂数据操作
        /// </summary>
        void Operationleave(UserWorkAllocationEntity entity);


        /// <summary>
        /// 转岗数据操作
        /// </summary>
        void OperationJobs(UserWorkAllocationEntity entity);
    }
}
