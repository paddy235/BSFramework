using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.IService.PublicInfoManage;
using BSFramework.Application.Service.PublicInfoManage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace BSFramework.Application.Busines.PublicInfoManage
{
    public class NewFileInfoBLL
    {
        private INewFileInfoService service = new NewFileInfoService();

        public NewFileInfoEntity GetEntity(string id) 
        {
            return service.GetEntity(id);
        }
        public IEnumerable<NewFileInfoEntity> GetList(string recid) 
        {
            return service.GetList(recid);
        }

        public void SaveForm(string keyValue, NewFileInfoEntity entity) 
        {
            service.SaveForm(keyValue, entity);
        }
    }
}
