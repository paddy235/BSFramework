using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.MisManage;

namespace BSFramework.Application.IService.MisManage
{
    public interface ITicketService
    {
        List<string> GetUnits(string ticketStr);
        List<string> GetStatus(string ticketStr);
        List<string> GetCategories(string ticketStr);
        List<TicketEntity> GetList(string deptname, string[] units, string dutyperson, string category, string status, bool includecode,string keyword, int pagesize, int pageindex, out int total, string ticketStr);
        List<TicketEntity> GetStatistical(string deptname, string[] units, DateTime Start, DateTime End,string TeamType, string ticketStr);

        TicketEntity GetDetail(string ticketid, string ticketStr);
    }
}
