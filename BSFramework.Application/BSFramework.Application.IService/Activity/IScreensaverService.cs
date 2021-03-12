using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.Activity
{
    public interface IScreensaverService
    {
        void DeleteFile(string fileId, string filePath);
        ScreensaverEntity GetEntity(string fileid);
        void SaveForm(ScreensaverEntity fileentity);
        List<ScreensaverEntity> GetList(string bzid);
    }
}
