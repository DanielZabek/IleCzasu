using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IleCzasu.Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        public string Name { get; set; }
        public int NumberOfEvents { get; set; }
        public string IconClass { get; set; }

        [ForeignKey("ParentCategoryId")]
        public List<Category> SubCategories { get; set; }
        [ForeignKey("ParentCategoryId")]
        public Category ParentCategory  { get; set; }

        public List<PublicEvent> Events { get; set; }
        public List<TagType> TagTypes { get; set; }
    }
}
