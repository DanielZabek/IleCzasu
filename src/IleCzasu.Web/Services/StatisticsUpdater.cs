using IleCzasu.Data;
using IleCzasu.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IleCzasu.Domain.Entities;
using IleCzasu.Infrastructure;

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
            var categories = _context.Categories.ToList();



            foreach (var c in categories)
            {
                var categoriesId = from x in _context.Categories
                          .Where(x => x.CategoryId == c.CategoryId || x.ParentCategoryId == c.CategoryId)
                                   select x.CategoryId;

                if (_context.Statistics.Where(s => s.Name == c.Name).Count() == 0)
                {
                    var stat = new Statistic { Name = c.Name, Value = _context.PublicEvents.Where(e => categoriesId.Contains(e.CategoryId) && e.Date.Date >= DateTime.Now.Date).Count() };
                    _context.Statistics.Add(stat);
                    _context.SaveChanges();
                    if (c.CategoryId == 10)
                    {
                        stat = new Statistic { Name = c.Name, Value = _context.PublicEvents.Where(e => e.Date.Date >= DateTime.Now.Date).Count() };
                        _context.Statistics.Add(stat);
                    }

                } else if(_context.Statistics.Where(s => s.Name == c.Name).Count() == 1)
                {
                    var stat = _context.Statistics.SingleOrDefault(s => s.Name == c.Name);
                    stat.Value = _context.PublicEvents.Where(e => e.Date.Date >= DateTime.Now.Date && categoriesId.Contains(e.CategoryId)).Count();
                    _context.Update(stat);
                    if (c.CategoryId == 10)
                    {
                        stat.Value = _context.PublicEvents.Where(e => e.Date.Date >= DateTime.Now.Date).Count();
                        _context.Update(stat);
                    }
                }
                _context.SaveChanges();
            }

            return Task.CompletedTask;
        }
  
    }
}
