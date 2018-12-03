using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Models.ApiWrappers.TicketMaster
{
    public class TicketMasterWrapper
    {
        public _Embedded _embedded { get; set; }
        public Page page { get; set; }
    }

    public class Page
    {
        public int number { get; set; }
        public int size { get; set; }
        public int totalElements { get; set; }
        public int totalPages { get; set; }
    }
}
