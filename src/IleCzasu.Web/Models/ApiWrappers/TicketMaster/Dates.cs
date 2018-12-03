using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Models.ApiWrappers.TicketMaster
{
    public class Dates
    {
       public Start start { get; set; }
        
    }

    public class Start
    {
        public String localDate { get; set; }
        public String localTime { get; set; }
        public DateTime dateTime { get; set; }
    }
}
