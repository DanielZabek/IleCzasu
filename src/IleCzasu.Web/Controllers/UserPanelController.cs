using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IleCzasu.Data.Entities;
using IleCzasu.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using MediatR;
using IleCzasu.Application.Events.Queries;
using IleCzasu.Data;

namespace IleCzasu.Controllers
{
    [Authorize]
    public class UserPanelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _enviroment;
        private IMediator _mediator;

        public UserPanelController(ApplicationDbContext context, IHostingEnvironment env, IMediator mediator)
        {
            _context = context;
            _enviroment = env;
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            var model = new UserPanelViewModel();
            model.UserEvents = _context.PrivateEvents.Where(e => e.UserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
            model.UserNotes = _context.Notes.Where(e => e.UserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
            model.Categories = _context.Categories.ToList();
            model.User = _context.Users.FirstOrDefault(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            model.Settings = _context.ReminderSettings.Where(s => s.UserId == model.User.Id).ToList();
            int lvl = 1;
            int points = model.User.Points;

            while (points > 40 + (10 * lvl))
            {
                points = points - (40 + (10 * lvl));
                lvl += 1;
            }
            
            var users = from u in _context.Users
                        select u;       
            List<ApplicationUser> us = users.ToList();       

            return View(model);
        }

        public async Task<IActionResult> ModeratorIndex(int id)

        {
            //var date = DateTime.Now;
            //var outputString = "";
            //for (int i = 1; i < 5; i++)
            //{
            //    Console.WriteLine(outputString);
            //    Console.WriteLine(date.ToString("yyyy-MM-dd"));
            //    try
            //    {
            //        date = date.AddDays(1);

            //        HttpWebRequest apiRequest = WebRequest.Create("http://www.sport.pl/sport/0,65027,,,,,d,p," + date.ToString("yyyy-MM-dd") + ".html#wyniki") as HttpWebRequest;

            //        string apiResponse = "";
            //        using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
            //        {
            //            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("ISO-8859-2"));
            //            apiResponse = reader.ReadToEnd();
            //        }
            //        HtmlDocument pageDocument = new HtmlDocument();
            //        pageDocument.LoadHtml(apiResponse);

            //        var divs = pageDocument.DocumentNode.SelectNodes("//div[starts-with(@class,'entry')]").ToList();

            //        foreach (var d in divs)
            //        {
            //            try
            //            {
            //                var pathLi = d.SelectNodes(".//ul[contains(@class,'path')]/li").ToList();
            //                foreach (var li in pathLi)
            //                {
            //                    outputString += li.SelectSingleNode(".//a | .//span").InnerText + " / ";
            //                }
            //                var eventsA = d.SelectNodes(".//ul[contains(@class,'events')]/li").ToList();
            //                foreach (var a in eventsA)
            //                {

            //                    outputString += a.SelectSingleNode(".//span[1]/span").InnerText + " / ";

            //                    var eventTeams = a.SelectNodes(".//span[2]/span").ToList();
            //                    foreach(var team in eventTeams)
            //                    {
            //                        if (team.SelectSingleNode(".//span") != null){
            //                            continue;
            //                        }
            //                        {
            //                            outputString += team.InnerText + " / ";
            //                        }
            //                        outputString += team.InnerText + " / ";
            //                    }

            //                }
            //                outputString += "</br>";
            //            }
            //            catch (NullReferenceException)
            //            {
            //                continue;
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        continue;
            //    }
            //}


            //ViewData["res"] = outputString;

            
              var publicEvent = await _mediator.Send(new GetPublicEventByIdQuery { PublicEventId = id });

            return View(publicEvent);
        }

        public IActionResult ShowUserNavWidget()
        {
            var model = new UserNavWidgetViewModel();           
            model.User = _context.Users.SingleOrDefault(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            return ViewComponent("UserNavWidget", model);
        }
        public IActionResult ShowUserCalWidget()
        {
            var model = new UserCalWidgetViewModel();
            model.User = _context.Users
                .Include(f => f.UserFollows)
                .ThenInclude(e => e.Event)
                .SingleOrDefault(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));


            return ViewComponent("UserCalWidget", model);
        }

        public IActionResult Calendar(string date = "")
        {
            ViewData["calDate"] = date;
            return View();
        }

        public IActionResult ShowCalendar(int monthsToAdd)
        {
            return ViewComponent("Calendar", monthsToAdd);
        }
        public IActionResult ShowUserPrivateEvents(string date = "")
        {
            List<PrivateEvent> privateEvents = new List<PrivateEvent>();
            try
            {
                privateEvents = _context.PrivateEvents
               .Where(e => e.UserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier) && (e.StartDate.ToString("dd'.'MM'.'yyyy") == date || e.StartDate.ToString("yyyy-MM-dd") == date)).ToList();
            }
            catch (NullReferenceException)
            {
            }

            return ViewComponent("ShowPrivateEvents", privateEvents);
        }
        public IActionResult ShowUserEvents(string date = "")
        {
            var model = new ShowEventsViewModel();
            var user = _context.Users
               .Include(f => f.UserFollows).ThenInclude(f => f.Event).ThenInclude(c => c.Category).ThenInclude(tt => tt.TagTypes)
                .Include(f => f.UserFollows).ThenInclude(f => f.Event).ThenInclude(c => c.Category).ThenInclude(pc => pc.ParentCategory).ThenInclude(tt => tt.TagTypes)
               .Include(f => f.UserFollows).ThenInclude(f => f.Event).ThenInclude(e => e.TagEvents).ThenInclude(t => t.Tag)
                .Include(f => f.UserFollows).ThenInclude(f => f.Event)
               .Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).Single();

            model.Follows = _context.Follows.Where(f => f.UserId == user.Id).ToList();
            var events = user.UserFollows.Select(e => e.Event);

            if (!String.IsNullOrEmpty(date))
            {
                model.Events = events.Where(e => e.Date.ToString("dd'.'MM'.'yyyy") == date || e.Date.ToString("yyyy-MM-dd") == date).ToList();
            }

            return ViewComponent("ShowEvents", model);
        }


        public int FollowEvent(int eventId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var e = _context.PublicEvents.FirstOrDefault(a => a.PublicEventId == eventId);
            var followModel = new Follow { PublicEventId = eventId, UserId = user.Id };
           

            try
            {
                e.Follows += 1;
                _context.SaveChanges();
                _context.Follows.Add(followModel);
                _context.SaveChanges();

            }
            catch (DbUpdateException /* ex */)
            {

            }

            return e.Follows;
        }

        public int UnfollowEvent(int eventId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var e = _context.PublicEvents.FirstOrDefault(a => a.PublicEventId == eventId);
            var followModel = _context.Follows.FirstOrDefault(f => f.PublicEventId == eventId && f.UserId == user.Id);
      
            try
            {
                e.Follows -= 1;
                _context.Follows.Remove(followModel);
                _context.SaveChanges();
            }
            catch (DbUpdateException /* ex */)
            {

            }

            return e.Follows;
        }

        // GET: /Dodaj
        public IActionResult Create()
        {
            var model = new CreateViewModel();
         

            return View(model);
        }

        // POST: /Dodaj
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    model.Event.ImagePath = "";
            //    model.Event.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //    _context.PrivateEvents.Add(model.Event);
            //    var user = _context.Users.SingleOrDefault(u => u.Id == model.Event.UserId);
            //    user.Points += 10;
            //    _context.Update(user);
            //    _context.SaveChanges();

            //    if (!String.IsNullOrEmpty(model.Event.Place))
            //    {
            //        CitiesHelper ce = new CitiesHelper();
            //        var cities = _context.Cities.Select(c => c.Name).ToList();
            //        var cityName = ce.GetCityName(model.Event.Place);
            //        if (!String.IsNullOrEmpty(cityName))
            //        {
            //            if (!cities.Contains(cityName))
            //            {
            //                var city = new City { Name = cityName, NumberOfEvents = 1 };
            //                _context.Cities.Add(city);
            //                _context.SaveChanges();
            //            }
            //            else
            //            {
            //                var city = _context.Cities.SingleOrDefault(c => c.Name == cityName);
            //                city.NumberOfEvents += 1;
            //                _context.SaveChanges();
            //            }
            //        }
            //    }

            //    var evnt = _context.PrivateEvents.SingleOrDefault(e => e.EventId == model.Event.EventId);
            //    evnt.ImagePath = "https://ileczasu.pl/images/" + CroppImage(model.Event.EventId, model.imagePath, model.x, model.y, model.width, model.height);
            //    _context.SaveChanges();

            //    return RedirectToAction("Index");
            //}

         
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNote(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                model.Note.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Notes.Add(model.Note);              
                _context.SaveChanges();

                return RedirectToAction("Index");
            }


            return View(model);
        }
        public JsonResult GetSubCategories(int categoryId)
        {
            var subCategories = _context.Categories.Where(s => s.ParentCategoryId == categoryId).ToList();
            return Json(subCategories);
        }
        public JsonResult GetTagTypes(int categoryId)
        {
            var category = _context.Categories.SingleOrDefault(c => c.CategoryId == categoryId);

            var tagTypes = _context.TagTypes.Where(t => t.CategoryId == categoryId || t.CategoryId == category.ParentCategoryId).ToList();

            return Json(tagTypes);
        }
        public JsonResult GetTags(int tagTypeId, string input)
        {
            var tags = _context.Tags.Where(t => t.TagTypeId == tagTypeId && t.Value.ToLower().Contains(input)).Select(t => t.Value).ToList();

            return Json(tags);
        }
        public IActionResult DeleteConfirmation(int eventId)
        {
            var evnt = _context.PublicEvents.SingleOrDefault(e => e.PublicEventId == eventId);

            return ViewComponent("DeleteConfirmation", evnt);
        }

        public bool Delete(int eventId)
        {
            try
            {
                var evnt = _context.PublicEvents.SingleOrDefault(e => e.PublicEventId == eventId);
           
                _context.Remove(evnt);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException /* ex */)
            {
                return false;
            }
        }

        public IActionResult Edit(int eventId)
        {
            var model = new CreateViewModel();
            model.Event = _context.PrivateEvents.SingleOrDefault(e => e.PrivateEventId == eventId);    
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(CreateViewModel model)
        {
              _context.PrivateEvents.Update(model.Event);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SaveImage(IFormFile image)
        {
            if (image.FileName.EndsWith(".jpg") || image.FileName.EndsWith(".jpeg") || image.FileName.EndsWith(".png"))
            {
                Random rnd = new Random();

                var uploadPath = Path.Combine(_enviroment.WebRootPath, "images", "temp");
                var pathTab = image.FileName.Split('\\');

                var fileName = (rnd.Next(100, 200) + "_" + pathTab[pathTab.Length - 1]);
                var imagePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                    stream.Close();
                }
                return ViewComponent("CroppImage", fileName);
            }
            return ViewComponent("CroppImage", "");
        }

        public async Task<IActionResult> SaveAvatar(IFormFile image)
        {
            if (image.FileName.EndsWith(".jpg") || image.FileName.EndsWith(".jpeg") || image.FileName.EndsWith(".png"))
            {
                var uploadPath = Path.Combine(_enviroment.WebRootPath, "images", "temp");
                var pathTab = image.FileName.Split('\\');

                var fileName = User.Identity.Name + "_" + pathTab[pathTab.Length - 1];
                var imagePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                    stream.Close();
                }
                return ViewComponent("CroppAvatar", fileName);
            }
            return ViewComponent("CroppAvatar", "");
        }

        public string CroppImage(int eventId, string imageName, int x, int y, int width, int height)
        {
            var newImageName = eventId + imageName.Remove(0, 3);
            var imagePath = Path.Combine(_enviroment.WebRootPath, "images", "temp", imageName);
            var imageSavePath = Path.Combine(_enviroment.WebRootPath, "images", newImageName);
            var rectangle = new Rectangle(x, y, width, height);

            using (Image<Rgba32> image = Image.Load(imagePath))
            {
                image.Mutate(i => i
                     .Crop(rectangle)
                     .Resize(400, 300));
                image.Save(imagePath);
                using (var stream = new FileStream(imageSavePath, FileMode.Create))
                {
                    image.SaveAsJpeg(stream);
                }
            }

            return newImageName;
        }

        public string CroppAvatar(string imageName, int x, int y, int width, int height)
        {
            var imagePath = Path.Combine(_enviroment.WebRootPath, "images", "temp", imageName);
            var imageSavePath = Path.Combine(_enviroment.WebRootPath, "images", "avatars", imageName);
            var rectangle = new Rectangle(x, y, width, height);

            using (Image<Rgba32> image = Image.Load(imagePath))
            {
                image.Mutate(i => i
                     .Crop(rectangle)
                     .Resize(100, 100));
                image.Save(imagePath);
                using (var stream = new FileStream(imageSavePath, FileMode.Create))
                {
                    image.SaveAsJpeg(stream);
                }
            }

            var user = _context.Users.SingleOrDefault(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            user.Avatar = imageName;
            _context.SaveChanges();

            return imageName;
        }

        public bool TurnOnReminder()
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var settings = _context.ReminderSettings.SingleOrDefault(s => s.UserId == user.Id);

            if (settings == null)
            {
                settings = new ReminderSetting { UserId = user.Id, PrivateOnly = false, Active = true, CategoryId = 10, DaysBefore = 1 };
                _context.ReminderSettings.Add(settings);
            }
            else
            {
                settings.Active = true;
                _context.Update(settings);
            }

            _context.SaveChanges();

            return true;
        }

        public bool TurnOffReminder()
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var settings = _context.ReminderSettings.SingleOrDefault(s => s.UserId == user.Id);

            settings.Active = false;
            _context.Update(settings);
            _context.SaveChanges();

            return true;
        }

        public bool SaveReminderSettings(int catId, bool privateOnly, int daysBefore)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var settings = _context.ReminderSettings.SingleOrDefault(s => s.UserId == user.Id);

            settings.CategoryId = catId;
            settings.PrivateOnly = privateOnly;
            settings.DaysBefore = daysBefore;

            _context.Update(settings);
            _context.SaveChanges();

            return true;
        }

        public Comment AddComment(int eventId, string content, int? replyToId)
        {
            Comment comment = new Comment();
            var user = _context.Users.SingleOrDefault(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            comment.PublicEventId = eventId;
            comment.Content = content;
            comment.UserId = user.Id;
            comment.Points = 0;
            comment.CreationDate = DateTime.Now;
            comment.ReplyToId = replyToId;
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return comment;
        }

        public bool DeleteComment(int commentId)
        {
            var comment = _context.Comments.SingleOrDefault(c => c.CommentId == commentId);
            var replies = _context.Comments.Where(c => c.ReplyToId == commentId);
            if (comment.UserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                try
                {
                    foreach (var r in replies)
                    {
                        _context.Remove(r);
                    }
                    _context.Remove(comment);
                    _context.SaveChanges();

                    return true;
                }
                catch (DbUpdateException /* ex */)
                {
                    return false;
                }
            }

            return false;
        }

        public bool EditComment(int commentId, string commentContent)
        {
            var comment = _context.Comments.SingleOrDefault(c => c.CommentId == commentId);
            if (comment.UserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                try
                {
                    comment.Content = commentContent;
                    _context.SaveChanges();

                    return true;
                }
                catch (DbUpdateException /* ex */)
                {
                    return false;
                }
            }

            return false;
        }

        public bool LikeComment(int commentId)
        {
            var comment = _context.Comments.SingleOrDefault(c => c.CommentId == commentId);
            if (comment.UserId == this.User.FindFirstValue(ClaimTypes.NameIdentifier))
            {

            }

            return false;
        }      

        private static int levenshtein(String s, String t)
        {
            int i, j, m, n, cost;
            int[,] d;

            m = s.Length;
            n = t.Length;

            d = new int[m + 1, n + 1];

            for (i = 0; i <= m; i++)
                d[i, 0] = i;
            for (j = 1; j <= n; j++)
                d[0, j] = j;

            for (i = 1; i <= m; i++)
            {
                for (j = 1; j <= n; j++)
                {
                    if (s[i - 1] == t[j - 1])
                        cost = 0;
                    else
                        cost = 1;

                    d[i, j] = Math.Min(d[i - 1, j] + 1,   /* remove */
                    Math.Min(d[i, j - 1] + 1,         /* insert */
                    d[i - 1, j - 1] + cost));        /* change */
                }
            }

            return d[m, n];
        }


    }
}