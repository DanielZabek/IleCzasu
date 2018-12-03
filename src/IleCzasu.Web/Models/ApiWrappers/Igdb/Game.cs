using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Models.ApiWrappers.Igdb
{
    public class Game
    {
        public String name { get; set; }
        public String url { get; set; }
        public List<Release_dates> Release_dates { get; set; }
        public List<Screenshots> Screenshots { get; set; }
    }
}
