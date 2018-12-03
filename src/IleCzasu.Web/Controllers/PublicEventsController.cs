using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IleCzasu.Domain.Entities;
using IleCzasu.Infrastructure;
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
using System.Security.Claims;

namespace IleCzasu.Controllers
{
    public class PublicEventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration Configuration;
        private IHostingEnvironment _enviroment;
        private IMediator _mediator;

        public PublicEventsController(ApplicationDbContext context, IConfiguration configuration, IHostingEnvironment enviroment, IMediator mediator)
        {
            _context = context;
            Configuration = configuration;
            _enviroment = enviroment;
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel();
            model.Cities = _context.Cities.OrderByDescending(c => c.NumberOfEvents).Take(6).ToList();
            model.MostPopularEvents = await _mediator.Send(new GetPublicEventsQuery { SortBy = "follows", PageSize = 10});
            model.Categories = await _mediator.Send(new GetCategoriesQuery());
            return View(model);
        }
        // GET: 5/event-name
        [HttpGet]
        public async Task<IActionResult> Event(int id, string name)
        {                  
            string friendlyTitle = FriendlyUrlHelper.GetFriendlyTitle(name);
            if (!string.Equals(friendlyTitle, name, StringComparison.Ordinal))
            {             
                return RedirectToRoute("event", new { id = id, name = friendlyTitle });
            }

            var model = new EventViewModel();
            if (User.Identity.IsAuthenticated)
            {
                model.PublicEvent = await _mediator.Send(new GetPublicEventByIdQuery { PublicEventId = id, UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
            }
            else
            {
                model.PublicEvent = await _mediator.Send(new GetPublicEventByIdQuery { PublicEventId = id });
            }  

            return View(model);
        }
        
  
        public async Task<IActionResult> ShowEvents(int categoryId = 0, string sortBy = "date", string date = "", string city = "", int pageNumber = 1, int pageSize = 25)
        {
            return ViewComponent("ShowEvents", await _mediator.Send(new GetPublicEventsQuery { SortBy = sortBy, CategoryId = categoryId, Date = date, City = city, PageNumber = pageNumber, PageSize = pageSize }));
        }

        public IActionResult ShowMostPopularTags(int categoryId)
        {
            var category = _context.Categories.SingleOrDefault(c => c.CategoryId == categoryId);
            var tagTypes = _context.TagTypes.Where(t => t.CategoryId == categoryId || t.CategoryId == category.ParentCategoryId).Select(t => t.TagTypeId);

            var model = _context.Tags.Where(t => tagTypes.Contains(t.TagTypeId)).OrderByDescending(t => t.Popularity).Take(20).ToList();
            return ViewComponent("MostPopularTags", model);
        }
        public IActionResult ShowSubCategories(int categoryId)
        {
            var model = _context.Categories.Where(c => c.ParentCategoryId == categoryId).ToList();
            return ViewComponent("ShowSubcategories", model);
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

        public IActionResult CategoryView(string categoryName)
        {
            if (categoryName.Equals("swiat"))
            {
                categoryName = "świat";
            }
            else if (categoryName.Equals("rozwoj"))
            {
                categoryName = "rozwój";
            }
            List<PublicEvent> events = new List<PublicEvent>();
            var category = _context.Categories.SingleOrDefault(c => c.Name == categoryName);

            if (categoryName == "wszystkie")
            {
                events = _context.PublicEvents.ToList();
            }
            else
            {
                events = _context.PublicEvents.Where(e => e.CategoryId == category.CategoryId).ToList();
            }

            ViewData["CategoryName"] = categoryName;
            ViewData["CategoryId"] = category.CategoryId;
            return View(events);
        }

        public JsonResult GetCities()
        {
            var cities = _context.Cities.Select(c => c.Name);
            return Json(cities);
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

        //public string GetIgdbEvents()
        //{
        //    HttpWebRequest apiRequest = WebRequest.Create("https://api-endpoint.igdb.com//release_dates/?fields=*&order=date:asc&filter[date][gt]=1531796461283&expand=game") as HttpWebRequest;
        //    apiRequest.Headers["user-key"] = "ef9779137b8258b2cfb498d40f35b472";
        //    apiRequest.Headers["Accept"] = "application/json";

        //    var t = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;


        //    string apiResponse = "";
        //    using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
        //    {
        //        StreamReader reader = new StreamReader(response.GetResponseStream());
        //        apiResponse = reader.ReadToEnd();
        //    }
        //    List<IgdbWrapper> igdb = JsonConvert.DeserializeObject<List<IgdbWrapper>>(apiResponse);

        //    foreach(var g in igdb)
        //    {
        //        DateTime dt = new DateTime(1970, 1, 1);
        //        Event e = new Event();
        //        e.Name = g.Game.name;
        //        e.Date = dt.AddMilliseconds(g.Game.Release_dates[0].date);

        //        string imageName = g.id + "_igdb.jpg";             
        //        var imagePath = Path.Combine(_enviroment.WebRootPath, "images", "temp", imageName);
        //        var imageSavePath = Path.Combine(_enviroment.WebRootPath, "images", imageName);
        //        var rectangle = new Rectangle(71, 0, 400, 320);
        //        using (WebClient webClient = new WebClient())
        //        {
        //            webClient.DownloadFile("https://images.igdb.com/igdb/image/upload/t_screenshot_med_2x/"+g.Game.Screenshots[0].cloudinary_id + ".jpg", imagePath);
        //        }
        //        using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(imagePath))
        //        {
        //            image.Mutate(i => i
        //                 .Crop(rectangle)
        //                 .Resize(400, 300));
        //            image.Save(imagePath);
        //            using (var stream = new FileStream(imageSavePath, FileMode.Create))
        //            {
        //                image.SaveAsJpeg(stream);
        //            }
        //        }
        //        e.ImagePath = "https://ileczasu.pl/images/" + imageName;
        //        e.CategoryId = 8;
        //        e.Follows = 0;
        //        e.IsPublic = true;
        //        e.UserId = "0ffa0570-92b3-43a6-b028-00eeb2740c6a";
        //        e.Url = g.Game.url;
        //        _context.Events.Add(e);
        //        _context.SaveChanges();
        //    }        
        //    return apiResponse;
        //}
        public bool AddTicketMasterEvents()
        {

            return false;

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
