using IleCzasu.Data;
using IleCzasu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IleCzasu.Infrastructure;
using IleCzasu.Domain.Entities;
namespace IleCzasu.Services
{
    public class InfoService : IInfoService
    {
        private readonly ApplicationDbContext _context;

        public InfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddApplicationInfo(string info)
        {
            var infoForModerators = new InfoForModerators();
            infoForModerators.Info = info;
            infoForModerators.InfoCategoryId = 1;
            _context.InfoForModerators.Add(infoForModerators);
            _context.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
