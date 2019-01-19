using System.ComponentModel.DataAnnotations;

namespace IleCzasu.Models.ViewModels
{
    public class CropImageViewModel
    {
        [FileExtensions(Extensions = "jpg,jpeg,png")]
        public string imagePath { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

}
