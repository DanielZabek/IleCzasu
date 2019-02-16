using System;
using System.Collections.Generic;
using IleCzasu.Data.Entities;

namespace IleCzasu.Models.ViewModels
{
    public class UserPanelViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Category> Categories { get; set; }      

    }
}
