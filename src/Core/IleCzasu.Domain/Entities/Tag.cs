using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Domain.Entities
{
    public class Tag
    {
        public int TagId { get; set; }
        public int TagTypeId { get; set; }
        public string Value { get; set; }
        public int Popularity { get; set; }
        public TagType TagType { get; set; }
      
    }
}
