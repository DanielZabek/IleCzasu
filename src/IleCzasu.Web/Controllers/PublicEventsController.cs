using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IleCzasu.Data.Entities;
using IleCzasu.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using MHBlog.Extensions;
using IleCzasu.Models.ViewModels;
using IleCzasu.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

using SixLabors.ImageSharp;
using MediatR;
using IleCzasu.Application.Events.Queries;
using IleCzasu.Application.Queries;
using IleCzasu.Application.Interfaces;
using System.Security.Claims;

namespace IleCzasu.Controllers
{
    public class PublicEventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration Configuration;
        private IPublicEventService _publicEventService;

        public PublicEventsController(ApplicationDbContext context, IPublicEventService publicEventService)
        {
            _context = context;
            _publicEventService = publicEventService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel();
            model.MostPopularEvents = await _publicEventService.GetPublicEvents(sortBy: "follows", pageSize: 10);
            model.Categories = await _publicEventService.GetCategories();
            return View(model);
        }

        public async Task<IActionResult> Event(int id, string name)
        {                  
            string friendlyTitle = FriendlyUrlHelper.GetFriendlyTitle(name);
            if (!string.Equals(friendlyTitle, name, StringComparison.Ordinal))
            {             
                return RedirectToRoute("event", new { id = id, name = friendlyTitle });
            }
            
            var model = new EventViewModel();
            model.SimilarEvents = await _publicEventService.GetSimilarEvents(id);
            if (User.Identity.IsAuthenticated)
            {
                model.PublicEvent = await _publicEventService.GetPublicEvent(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
            }
            else
            {
                model.PublicEvent = await _publicEventService.GetPublicEvent(id);
            }  

            return View(model);
        }

        public async Task<IActionResult> ShowEvents(int categoryId = 0, string sortBy = "date", string date = "", string city = "", int pageNumber = 1, int pageSize = 25)
        {
            if (User.Identity.IsAuthenticated)
            {
                return ViewComponent("ShowEvents", await _publicEventService.GetPublicEvents(categoryId, sortBy, date, city, pageNumber, pageSize, User.FindFirstValue(ClaimTypes.NameIdentifier)));
            }
            else
            {
                return ViewComponent("ShowEvents", await _publicEventService.GetPublicEvents(categoryId, sortBy, date, city, pageNumber, pageSize, String.Empty));
            }
        }

        public async Task<IActionResult> ShowMostPopularTags(int categoryId)
        {        
            return ViewComponent("MostPopularTags", await _publicEventService.GetPopularTags(categoryId));
        }

        public async Task<IActionResult> ShowSubCategories(int categoryId)
        {           
            return ViewComponent("ShowSubcategories", await _publicEventService.GetCategories(categoryId));
        }
      
        public IActionResult GetSearchResults(string input)
        {
            List<PublicEvent> model = new List<PublicEvent>();
            var events = _context.PublicEvents.ToList();
            foreach (var e in events)
            {
                var words = e.Name.Split(" ");

                foreach (var w in words)
                {
                    if (w.ToLower().StartsWith(input.ToLower()))
                    {
                        model.Add(e);
                    }
                }

            }
            return ViewComponent("SearchResults", model);
        }

        public async Task<IActionResult> CategoryView(int categoryId, string categoryName)
        {           
            return View(await _publicEventService.GetCategory(categoryId));
        }

        public async Task<IEnumerable<string>> GetCities()
        {
            return await _publicEventService.GetCities();         
        }

        public IActionResult Stoper()
        {
            return View();
        }

        public IActionResult IntervalTimer()
        {
            return View();
        }

        public IActionResult TimeZones()
        {
            var model = TimeZoneInfo.GetSystemTimeZones();
            return View(model);
        }     

        public IActionResult ShowCitiesRanking()
        {
            var model = _context.Cities.OrderByDescending(c => c.NumberOfEvents).Take(6).ToList();
            return ViewComponent("CitiesRanking", model);
        }

        public string GetWeather(double lat, double lon)
        {
            HttpWebRequest apiRequest = WebRequest.Create("http://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lon + "&lang=pl&appid=d10f1539ba2f3d05b8e5a5b085e59baa&units=metric") as HttpWebRequest;

            string apiResponse = "";
            using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                apiResponse = reader.ReadToEnd();
            }
            return apiResponse;
        }    

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult PolitykaPrywatnosci()
        {
            return View();
        }

        public bool NameValidation(string name)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName.ToUpper() == name.ToUpper().Trim());
            if (user != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool EmailValidation(string email)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email.ToUpper() == email.ToUpper().Trim());
            if (user != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
   
    }
   
}
