using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Entity.WorkMeeting;
using BSFramework.IService.WorkMeeting;
using BSFramework.Service.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WorkMeeting
{
    public class CultureBLL
    {
        private ICultureService service = new CultureService();


        public void SaveCulture(CultureTemplateEntity model)
        {
            if (string.IsNullOrEmpty(model.CultureTemplateId)) this.AddCulture(model);
            else this.EditCulture(model);
        }
        public void EditCulture(CultureTemplateEntity model)
        {
            if (model.Contents != null)
            {
                for (int i = 0; i < model.Contents.Count; i++)
                {
                    model.Contents[i].CultureTemplateId = model.CultureTemplateId;
                    model.Contents[i].CreateUserId = model.CreateUserId;
                }
            }
            service.EditCulture(model);
        }
        public void AddCulture(CultureTemplateEntity model)
        {
            model.CultureTemplateId = Guid.NewGuid().ToString();
            if (model.Contents != null)
            {
                for (int i = 0; i < model.Contents.Count; i++)
                {
                    model.Contents[i].CultureTemplateId = model.CultureTemplateId;
                    model.Contents[i].CreateTime = model.CreateTime.AddSeconds(i);
                    model.Contents[i].CreateUserId = model.CreateUserId;
                }
            }
            service.AddCulture(model);
        }

        public int GetTotal4(string deptid, DateTime now)
        {
            return service.GetTotal4(deptid, now);
        }

        public double GetTotal3(string deptid, DateTime now)
        {
            return service.GetTotal3(deptid, now);
        }

        public int GetTotal2(string deptid, DateTime now)
        {
            return service.GetTotal2(deptid, now);
        }

        public int GetTotal1(string deptid, DateTime now)
        {
            return service.GetTotal1(deptid, now);
        }

        public double GetAvgage(string deptId)
        {
            return service.GetAvgage(deptId);
        }

        public FileInfoEntity GetImage1(string deptid, DateTime now)
        {
            return service.GetImage1(deptid, now);
        }

        public List<FileInfoEntity> GetImage4(string deptid, DateTime now)
        {
            return service.GetImage4(deptid, now);
        }

        public List<FileInfoEntity> GetImage3(string deptid, DateTime now)
        {
            return service.GetImage3(deptid, now);
        }

        public List<NewsEntity> GetNotices(string deptid)
        {
            return service.GetNotices(deptid);
        }

        public List<FileInfoEntity> GetImage2(string deptid, DateTime now)
        {
            return service.GetImage2(deptid, now);
        }

        public int GetPersonTotal(string deptId)
        {
            return service.GetPersonTotal(deptId);
        }

        public CultureTemplateEntity GetCulture(string groupid)
        {
            return service.GetCulture(groupid);
        }

        public IList<CultureTemplateEntity> GetData(string name, int rows, int page, out int total)
        {
            return service.GetData(name, rows, page, out total);
        }

        public CultureTemplateEntity GetTemplate(string id)
        {
            return service.GetTemplate(id);
        }

        public CultureTemplateItemEntity GetTemplateContent(string id)
        {
            return service.GetTemplateContent(id);
        }
    }
}
