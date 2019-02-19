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
using IleCzasu.Application.Interfaces;
using IleCzasu.Models;
using IleCzasu.Web.Models.ViewModels;

namespace IleCzasu.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _enviroment;
        private IMediator _mediator;
        private readonly IUserService _userService;
        private readonly IPublicEventService _publicEventService;
        private string _userId { get { return User.FindFirstValue(ClaimTypes.NameIdentifier); } }

        public UserController(IUserService userService, IPublicEventService publicEventService, ApplicationDbContext context, IHostingEnvironment env, IMediator mediator)
        {
            _publicEventService = publicEventService;
            _userService = userService;
            _context = context;
            _enviroment = env;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var model = new UserPanelViewModel()
            {
                User = await _userService.GetUserWithItems(_userId),
                Categories = await _publicEventService.GetCategories()
            };
            return View(model);
        }

        public async Task<IActionResult> ModeratorIndex(int id)
        {          
            return View();
        }

        public async Task<IActionResult> ShowUserNavWidget()
        {       
            return ViewComponent("UserNavWidget", await _userService.GetUserById(_userId));
        }
        public async Task<IActionResult>  ShowUserCalWidget()
        {
            return ViewComponent("UserCalWidget", await _userService.GetUserWithItems(_userId));
        }

        public IActionResult Calendar(string date = "")
        {
            ViewData["calDate"] = date;
            return View();
        }

        public async Task<IActionResult> ShowCalendar(int monthsToAdd)
        {
            CalendarViewModel model = new CalendarViewModel();
            model.User = await _userService.GetUserWithItems(_userId);
            model.FirstDayOfWeek = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            model.FirstDayOfWeek = model.FirstDayOfWeek.AddMonths(monthsToAdd);
            model.Current = model.FirstDayOfWeek;
            int diff = (7 + (model.FirstDayOfWeek.DayOfWeek - DayOfWeek.Monday)) % 7;
            model.FirstDayOfWeek = model.FirstDayOfWeek.AddDays(-1 * diff);

            return ViewComponent("Calendar", model);
        }

        public async Task<IActionResult> ShowCalendarDay(string date = "")
        {
            return ViewComponent("CalendarDay", await _userService.GetUserEventsByDate(_userId, date));
        }
        public async Task<IActionResult> ShowUserItems(string date = "")
        {
            var model = new UserItemsViewModel();
            //model.UserEvents = await _userService.GetUserEvents(_userId, date);
            //model.UserFollows = await _userService.GetUserFollows(_userId, date);
            //model.UserNotes = await _userService.GetUserNotes(_userId, date);
            return ViewComponent("ShowUserItems", model);
        }


        public async Task<int> FollowEvent(int eventId)
        { 
            return await _userService.FollowEvent(_userId, eventId);
        }

        public async Task<int> UnfollowEvent(int eventId)
        {
            return await _userService.UnfollowEvent(_userId, eventId);
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