using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.WebApp
{
   public  interface IMedicalService
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        IList<MedicalEntity> getMedicalInfo(string userid);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        MedicalEntity getMedicalDetail(string userid, string id);
        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="entity"></param>
        void addMedical(MedicalEntity entity, FileInfoEntity file);
        /// <summary>
        /// 修改信息
        /// </summary>
        void modifyMedical(MedicalEntity entity);
        /// <summary>
        ///删除体检信息
        /// </summary>
        /// <param name="id"></param>
        void delMedical(string id);
    }


}
