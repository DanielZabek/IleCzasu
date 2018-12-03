using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

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
