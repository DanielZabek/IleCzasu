using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Data.Entities
{
    public class TagType
    {
        public int TagTypeId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public List<Tag> Tags { get; set; }
    }
}
