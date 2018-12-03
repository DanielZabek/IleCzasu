using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Services
{
    public interface IEventsUpdater
    {
        Task GetTicketMasterEvents();
        Task GetFilmwebEventsFilms();
        Task GetFilmwebEventsSerials();
        Task GetKupBilecikEvents();
        Task GetEmpikEvents();
    }
}
