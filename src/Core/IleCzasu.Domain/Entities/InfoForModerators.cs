using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Domain.Entities
{
    public class InfoForModerators
    {
        public int InfoForModeratorsId { get; set; }
        public int InfoCategoryId { get; set; }
        public string Info { get; set; }
        public DateTime Time { get; set; }

        public InfoForModerators()
        {
            Time = DateTime.Now;
        }
    }
}
