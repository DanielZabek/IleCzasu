using IleCzasu.Data.Entities;
namespace IleCzasu.Models.ViewModels
{
    public class CreateViewModel
    {
        public PrivateEvent Event { get; set; }    
        public Note Note { get; set; }
        public string imagePath { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
      
      
    }
}
