using IleCzasu.Data;
using IleCzasu.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IleCzasu.Data.Entities;
using IleCzasu.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IleCzasu.Services
{
    public class StatisticsUpdater : IStatisticsUpdater
    {
        private readonly ApplicationDbContext _context;

        public StatisticsUpdater(ApplicationDbContext context)
        {
            _context = context;
        }
        public Task UpdateStatistics()
        {
            HttpWebRequest apiRequest = WebRequest.Create("https://ileczasu.pl") as HttpWebRequest;
            apiRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            string apiResponse = "";
            using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                apiResponse = reader.ReadToEnd();
            }
            var categories = _context.Categories
                .Include(c => c.SubCategories)
                .ToList();

            foreach (var c in categories)
            {
                c.NumberOfEvents = _context.PublicEvents.Where(e => e.Date.Date >= DateTime.Now.Date && e.CategoryId == c.CategoryId).Count();
                _context.Update(c);
            }
            foreach (var c in categories)
            {
                if(c.SubCategories != null)
                {
                    foreach(var sub in c.SubCategories)
                    {
                        c.NumberOfEvents += sub.NumberOfEvents;
                    }
                    _context.Update(c);
                }
            }
            _context.SaveChanges();
        

            return Task.CompletedTask;
        }

}
}
