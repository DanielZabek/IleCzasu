using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IleCzasu.Domain.Entities;

namespace IleCzasu.Models.ViewModels
{
    public class UserNavWidgetViewModel
    {
        public ApplicationUser User { get; set; }
        public int Notifications { get; set; }
    }
}
