using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace BSFramework.Application.IService.PublicInfoManage
{
    public interface INewFileInfoService
    {
        NewFileInfoEntity GetEntity(string id);
        IEnumerable<NewFileInfoEntity> GetList(string recid);
        void SaveForm(string keyValue, NewFileInfoEntity entity);
    }
}
