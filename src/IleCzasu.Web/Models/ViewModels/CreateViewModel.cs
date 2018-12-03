using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IleCzasu.Domain.Entities;

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
