using BSFramework.Application.Entity.MisManage;
using BSFramework.Application.IService.MisManage;
using BSFramework.Application.Service.MisManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.MisManage
{
    public class TicketBLL
    {
        public List<string> GetUnits(string ticketStr)
        {
            ITicketService service = new TicketService();
            return service.GetUnits(ticketStr);
        }

        public List<string> GetSpecialties()
        {
            IFaultService service = new FaultService();
            return service.GetSpecialties();
        }

        public List<string> GetCategories(string ticketStr)
        {
            ITicketService service = new TicketService();
            return service.GetCategories(ticketStr);
        }

        public List<string> GetStatus(string ticketStr)
        {
            ITicketService service = new TicketService();
            return service.GetStatus(ticketStr);
        }


        public List<TicketEntity> GetList(string deptname, string[] units, string specialty, string category, string status, bool includecode,string keyword ,int pagesize, int pageindex, out int total, string ticketStr)
        {
            ITicketService service = new TicketService();
            return service.GetList(deptname, units, specialty, category, status, includecode,keyword, pagesize, pageindex, out total, ticketStr);
        }

        public List<TicketEntity> GetStatistical(string deptname, string[] units,DateTime Start,DateTime End,string TeamType, string ticketStr)
        {
            ITicketService service = new TicketService();
            return service.GetStatistical(deptname, units, Start, End,TeamType, ticketStr);
        }

        public TicketEntity GetDetail(string ticketid, string ticketStr)
        {
            ITicketService service = new TicketService();
            return service.GetDetail(ticketid, ticketStr);
        }
    }
}
