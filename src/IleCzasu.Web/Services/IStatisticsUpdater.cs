using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Services
{
    public interface IStatisticsUpdater
    {
        Task UpdateStatistics();
    }
}
