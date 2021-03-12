using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.WebApp;
using BSFramework.Application.IService.WebApp;
using BSFramework.Application.Service.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WebApp
{
 public    class MedicalBLL
    {
        IMedicalService service = new MedicalService();

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public IList<MedicalEntity> getMedicalInfo(string userid)
        {
            var  entity=service.getMedicalInfo(userid);
            //FileInfoBLL file = new PublicInfoManage.FileInfoBLL();
            //foreach (var item in entity)
            //{
            //    item.Files = file.GetFilesByRecIdNew(item.MedicalId).Where(x => x.Description == "照片").ToList();
            //}
            return entity;
             
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        public MedicalEntity getMedicalDetail(string userid, string id)
        {
            var entity = service.getMedicalDetail(userid, id);
            if (entity == null)
            {
                return entity;
            }
            FileInfoBLL file = new PublicInfoManage.FileInfoBLL();

            entity.Files = file.GetFilesByRecIdNew(entity.MedicalId).ToList();
            return entity;
             
        }
        /// <summary>
        /// 添加信息
        /// </summary>
        /// <param name="entity"></param>
        public void addMedical(MedicalEntity entity)
        {
            FileInfoBLL fb = new FileInfoBLL();
            var flist = fb.GetFilebyDescription(entity.createuserid, "职业健康二维码");
            service.addMedical(entity, flist);
        }


        /// <summary>
        /// 修改信息
        /// </summary>
        public void modifyMedical(MedicalEntity entity)
        {
            service.modifyMedical(entity );

        }
        /// <summary>
        ///删除体检信息
        /// </summary>
        /// <param name="id"></param>
        public void delMedical(string id) {
            service.delMedical(id);
        }
    }
}
