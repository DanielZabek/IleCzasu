using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Models.ApiWrappers.TicketMaster
{
    public class Venue
    {
        public Address address { get; set; }
        public Country country { get; set; }
        public City city { get; set; }
    }
    public class Country
    {
        public String name { get; set; }
    }
    public class City
    {
        public String name { get; set; }
    }
    public class Address
    {
        public String line1 { get; set; }
    }
}
