using System;
using System.Collections.Generic;
using IleCzasu.Data.Entities;

namespace IleCzasu.Models.ViewModels
{
    public class UserCalWidgetViewModel
    {
        public ApplicationUser User { get; set; }
        public DateTime Today { get; set; }  
       
        public UserCalWidgetViewModel()
        {
            Today = DateTime.Now;
        }
    }
}
