using HtmlAgilityPack;
using IleCzasu.Data;
using IleCzasu.Extensions;
using IleCzasu.Models;
using IleCzasu.Models.ApiWrappers.KupBilecik;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using IleCzasu.Infrastructure;
using IleCzasu.Data.Entities;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace IleCzasu.Services
{
    public class EventsUpdater : IEventsUpdater
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _enviroment;
        private IInfoService _infoService;

        public EventsUpdater(ApplicationDbContext context, IHostingEnvironment enviroment, IInfoService infoService)
        {
            _infoService = infoService;
            _context = context;
            _enviroment = enviroment;
        }

        public Task GetKupBilecikEvents()
        {
            int numberOfNewEvents = 0;
            var simEvents = _context.PublicEvents;
            HttpWebRequest apiRequest = WebRequest.Create("http://www.kupbilecik.pl/xml.php?pp=263") as HttpWebRequest;

            string apiResponse = "";
            using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                apiResponse = reader.ReadToEnd();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Events));
            Events events = (Events)serializer.Deserialize(new StringReader(apiResponse));

            foreach (var e in events.Event)
            {
                if (e.Artist_name != null && e.Event_date != null && e.Event_time != null && e.Event_title != null && e.Event_text != null && e.Event_type != null && e.Hall_city != null && e.Hall_address != null && e.Jpg_link != null)
                {
                    try
                    {                       
                        var kpe = new PublicEvent();
                        kpe.Name = e.Artist_name;
                        kpe.Date = DateTime.Parse(e.Event_date + " " + e.Event_time);

                        if (e.Event_type.Equals("teatr"))
                        {
                            kpe.CategoryId = 2;
                        }
                        else if (e.Event_type.Equals("muzyka"))
                        {
                            kpe.CategoryId = 15;
                        }
                        else if (e.Event_type.Equals("kabaret"))
                        {
                            kpe.CategoryId = 17;
                        }
                        else if (e.Event_type.Equals("standup"))
                        {
                            kpe.CategoryId = 18;
                        }
                        else if (e.Event_type.Equals("inne"))
                        {
                            kpe.CategoryId = 9;
                        }
                        else if (e.Event_type.Equals("sport"))
                        {
                            kpe.CategoryId = 6;
                        }
                        else
                        {
                            continue;
                        }

                        int count = simEvents.Where(ev => ev.CategoryId == kpe.CategoryId && ev.Date.Date == kpe.Date.Date && ev.Name.ToLower() == kpe.Name.ToLower()).Count();
                        if (count > 0)
                        {
                            continue;
                        }

     
                        kpe.Follows = 0;
                        kpe.Place = e.Hall_full_name.Text + ", " + e.Hall_address + ", " + e.Hall_city.Text + ", Polska";
                        kpe.Description = e.Event_title.Replace("&amp;", "&") + " | " + e.Event_text.Replace("&amp;", "&");
                        kpe.Url = e.Event_link;
                        kpe.Promotor = "KupBilecik";
                        int x = 0;
                        if (Int32.TryParse(e.Cena_min.Text, out x))
                        {
                            kpe.Price = x;
                        }
                        string imageName = e.Event_id + "_kupbilecik.jpg";

                        var imageSavePath = Path.Combine(_enviroment.WebRootPath, "images", imageName);
                        int width;
                        int height;
                        using (WebClient webClient = new WebClient())
                        {
                            webClient.DownloadFile(e.Jpg_link, imageSavePath);
                        }
                        using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(imageSavePath))
                        {
                            float ratio = (float)image.Width / image.Height;
                            if (ratio < 1.3333)
                            {
                                width = 400;
                                height = (int)(400 / ratio);
                            }
                            else
                            {
                                width = (int)(300 * ratio);
                                height = 300;
                            }

                            image.Mutate(i => i
                                 .Resize(width, height));
                            image.Save(imageSavePath);
                            using (var stream = new FileStream(imageSavePath, FileMode.Create))
                            {
                                image.SaveAsJpeg(stream);
                            }
                        }
                        kpe.ImagePath = "https://ileczasu.pl/images/" + imageName;
                        _context.PublicEvents.Add(kpe);
                        _context.SaveChanges();
                        numberOfNewEvents++;

                    }
                    catch (NullReferenceException)
                    {
                        continue;
                    }

                }
                else
                {
                    continue;
                }
            }
            _infoService.AddApplicationInfo("Dodano: " + numberOfNewEvents + " nowych wydarzeń z KupBilecik API");
            return Task.CompletedTask;
        }
        public Task GetEmpikEvents()
        {
            int numberOfNewEventsF = 0;
            int numberOfNewEventsM = 0;
            int numberOfNewEventsG = 0;
            int numberOfNewEventsB = 0;
            var lastId = _context.InfoForModerators.SingleOrDefault(i => i.InfoForModeratorsId == 1);
            var events = _context.PublicEvents.Where(e => e.ImagePath.Contains("https://ileczasu.pl/images/p"));
            for (int i = 0; i < 40; i++)
            {
                try
                {
                    HttpWebRequest apiRequest = WebRequest.Create("https://www.empik.com/zapowiedzi?hideUnavailable=true&start=" + ((i * 30) + 1) + "&availabilitySeparable=przedsprzedaz") as HttpWebRequest;
                    apiRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
                    string apiResponse = "";
                    using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        apiResponse = reader.ReadToEnd();
                    }
                    HtmlDocument pageDocument = new HtmlDocument();
                    pageDocument.LoadHtml(apiResponse);

                    var divs = pageDocument.DocumentNode.SelectNodes("//div[contains(@class,'search-list-item-hover')]").ToList();

                    foreach (var div in divs)
                    {
                        var tempImagePath = "https://ileczasu.pl/images/" + div.SelectSingleNode(".//a[contains(@class,'img seoImage')]").Attributes["data-product-id"].Value + ".jpg";

                        if (events.Where(e => e.ImagePath.Equals(tempImagePath)).Any())
                        {
                            continue;
                        }
                        else
                        {
                            try
                            {
                                var evnt = new PublicEvent();
                                evnt.Promotor = "Empik";
                                HttpWebRequest eventRequest = WebRequest.Create("https://www.empik.com" + div.SelectSingleNode(".//a[contains(@class,'img seoImage')]").Attributes["href"].Value) as HttpWebRequest;
                                eventRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
                                string eventResponse = "";
                                using (HttpWebResponse response = eventRequest.GetResponse() as HttpWebResponse)
                                {
                                    StreamReader reader = new StreamReader(response.GetResponseStream());
                                    eventResponse = reader.ReadToEnd();
                                }
                                HtmlDocument eventDocument = new HtmlDocument();
                                eventDocument.LoadHtml(eventResponse);
                                evnt.Name = eventDocument.DocumentNode.SelectSingleNode(".//h1[contains(@class,'productBaseInfo__title')]").InnerText.Replace("&nbsp;", " ");
                                evnt.Url = "https://www.empik.com" + div.SelectSingleNode(".//a[contains(@class,'img seoImage')]").Attributes["href"].Value;
                                var imgLink = eventDocument.DocumentNode.SelectSingleNode(".//div[contains(@class,'productGallery__mainImage')]").SelectSingleNode(".//img").Attributes["src"].Value;
                                string imageName = div.SelectSingleNode(".//a[contains(@class,'img seoImage')]").Attributes["data-product-id"].Value + ".jpg";
                                var imageSavePath = Path.Combine(_enviroment.WebRootPath, "images", imageName);
                                if (!String.IsNullOrEmpty(imgLink))
                                {
                                    using (WebClient webClient = new WebClient())
                                    {
                                        try
                                        {

                                        webClient.DownloadFile(imgLink, imageSavePath);

                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                    evnt.ImagePath = "https://ileczasu.pl/images/" + imageName;
                                    evnt.Description = eventDocument.DocumentNode.SelectSingleNode(".//div[contains(@class,'productComments productDescription')]").InnerText;

                                    evnt.CategoryId = 9;
                                    _context.PublicEvents.Add(evnt);
                                    _context.SaveChanges();
                                    
                                }
                                else
                                {
                                    continue;
                                }


                                var tags = eventDocument.DocumentNode.SelectNodes(".//div[contains(@class,'empikBreadcrumb')]/ul/li/a").ToList();
                                var tgs = _context.Tags.ToList();


                                var dates = eventDocument.DocumentNode.SelectNodes(".//span[contains(@class,'attributeDetailsValue')]").ToList();
                                var details = eventDocument.DocumentNode.SelectNodes(".//tr[contains(@class,'row--text row--text hidden-xs hidden-sm attributeName')]").ToList();


                                foreach (var d in details)
                                {
                                    var detailsTd = d.SelectNodes(".//td").ToList();
                                    var detailName = detailsTd[0].InnerText.Replace("\n", "");
                                   
                                    if (detailName.Equals("Data premiery:"))
                                    {
                                        DateTime dDate;
                                        if (DateTime.TryParse(detailsTd[1].SelectSingleNode(".//span").InnerText, out dDate))
                                        {
                                            evnt.Date = dDate;
                                        }
                                    }
                                    if (tags[1].InnerText.Equals("Książki"))
                                    {
                                        if (detailName.Equals("Liczba stron:"))
                                        {
                                            AddTag(evnt.PublicEventId, 15, detailsTd[1].SelectSingleNode(".//span").InnerText);
                                        }
                                        if (detailName.Equals("Wydawnictwo:"))
                                        {
                                            AddTag(evnt.PublicEventId, 17, detailsTd[1].SelectSingleNode(".//span/a").InnerText.Replace("&nbsp;", " "));
                                        }
                                    }
                                    if (tags[1].InnerText.Equals("Filmy"))
                                    {
                                        if (detailName.Equals("Czas trwania (min.):"))
                                        {
                                            AddTag(evnt.PublicEventId, 5, detailsTd[1].SelectSingleNode(".//span").InnerText);
                                        }
                                        if (detailName.Equals("Obsada:"))
                                        {
                                            var actors = detailsTd[1].SelectSingleNode(".//span").SelectNodes(".//a");
                                            foreach (var a in actors)
                                            {
                                                var names = a.InnerText.Replace("&nbsp;", " ").Split(" ");
                                                if (names.Length == 2)
                                                {
                                                    AddTag(evnt.PublicEventId, 2, names[1] + " " + names[0]);
                                                }
                                                if (names.Length == 3)
                                                {
                                                    AddTag(evnt.PublicEventId, 2, names[2] + " " + names[0] + " " + names[1]);
                                                }
                                                if (names.Length == 4)
                                                {
                                                    AddTag(evnt.PublicEventId, 2, names[3] + " " + names[0] + " " + names[1] + " " + names[2]);
                                                }

                                            }
                                        }

                                  

                                   
                                    if (tags[2].InnerText.Equals("Filmy Blu-ray.") || tags[2].InnerText.Equals("Filmy DVD"))
                                    {
                                        var tagName = tags[2].InnerText.Replace("Filmy ", "").Replace(".", "");
                                        AddTag(evnt.PublicEventId, 6, tagName);
                                        tagName = tags.Last().InnerText.Replace(".", "");
                                        if (tagName.Equals("Animowane"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Animacja");
                                        }
                                        else if (tagName.Equals("Familijne"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Familijny");
                                        }
                                        else if (tagName.Equals("Historyczne"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Historyczny");
                                        }
                                        else if (tagName.Equals("Podróżnicze"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Podróżniczy");
                                        }
                                        else if (tagName.Equals("Religijne"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Religijny");
                                        }
                                        else if (tagName.Equals("Sport/Rekreacja"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Sportowy");
                                        }
                                        else if (tagName.Equals("Fantasy/SF"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Fantasy");
                                            AddTag(evnt.PublicEventId, 1, "Sci-Fi");
                                        }
                                        else if (tagName.Equals("Horrory/Thrillery"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Horror");
                                            AddTag(evnt.PublicEventId, 1, "Thriller");
                                        }
                                        else if (tagName.Equals("Katastroficzne"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Katastroficzny");
                                        }
                                        else if (tagName.Equals("Przygodowe"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Przygodowy");
                                        }
                                        else if (tagName.Equals("Sensacyjne"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Sensacyjny");
                                        }
                                        else if (tagName.Equals("Westerny"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Western");
                                        }
                                        else if (tagName.Equals("Wojenne"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Wojenny");
                                        }
                                        else if (tagName.Equals("Kino polskie"))
                                        {
                                            AddTag(evnt.PublicEventId, 3, "Polska");
                                        }
                                        else if (tagName.Equals("Komedie"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Komedia");
                                        }
                                        else if (tagName.Equals("Muzyczne/Musicale"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Muzyczny");
                                            AddTag(evnt.PublicEventId, 1, "Musical");
                                        }
                                        else if (tagName.Equals("Biograficzne"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Biograficzny");
                                        }
                                        else if (tagName.Equals("Erotyczne"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Erotyczny");
                                        }
                                        else if (tagName.Equals("Muzyczne, Musicale"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Muzyczny");
                                            AddTag(evnt.PublicEventId, 1, "Musical");
                                        }
                                        else if (tagName.Equals("Obyczajowe"))
                                        {
                                            AddTag(evnt.PublicEventId, 1, "Obyczajowy");
                                        }
                                        else if (tagName.Equals("Polskie"))
                                        {
                                            AddTag(evnt.PublicEventId, 3, "Polska");
                                        }
                                    }
                                    }
                                }
                                if (tags[1].InnerText.Equals("Filmy"))
                                {
                                    evnt.CategoryId = 14;
                                    numberOfNewEventsF++;
                                    evnt.Price = ConvertPrice(eventDocument.DocumentNode.SelectSingleNode(".//div[contains(@class,'productPriceInfo__content ta-price')]").InnerText.Replace("&nbsp;", " "));
                                    var director = eventDocument.DocumentNode.SelectSingleNode(".//span[contains(@class,'pDAuthorList')]/a").InnerHtml.Replace("&nbsp;", " ").Split(" ");
                                    AddTag(evnt.PublicEventId, 4, director[1] + " " + director[0]);
                                }
                                else if (tags[1].InnerText.Equals("Muzyka"))
                                {
                                    numberOfNewEventsM++;
                                    evnt.CategoryId = 16;
                                    var tagName = tags[2].InnerText.ToLower().Replace("amp;", "");
                                    AddTag(evnt.PublicEventId, 10, tagName);
                                    if (tags.Count() > 3)
                                    {
                                        tagName = tags[3].InnerText.ToLower().Replace("amp;", "");
                                        AddTag(evnt.PublicEventId, 10, tagName);
                                    }
                                    if (tags.Count() == 5)
                                    {
                                        tagName = tags[4].InnerText.ToLower().Replace("amp;", "");
                                        AddTag(evnt.PublicEventId, 10, tagName);
                                    }
                                    evnt.Price = ConvertPrice(eventDocument.DocumentNode.SelectSingleNode(".//div[contains(@class,'productPriceInfo__content ta-price')]").InnerText.Replace("&nbsp;", " "));
                                    var authors = eventDocument.DocumentNode.SelectNodes(".//span[contains(@class,'pDAuthorList')]/a").ToList();
                                    foreach (var a in authors)
                                    {
                                        AddTag(evnt.PublicEventId, 8, a.InnerHtml.Replace("&nbsp;", " "));
                                    }

                                }
                                else if (tags[1].InnerText.Equals("Gry i programy"))
                                {
                                    numberOfNewEventsG++;
                                    if (tags[2].InnerText.Equals("Playstation 4"))
                                    {
                                        evnt.CategoryId = 21;
                                    }
                                    else if (tags[2].InnerText.Equals("Xbox One"))
                                    {
                                        evnt.CategoryId = 20;
                                    }
                                    else if (tags[2].InnerText.Equals("PC"))
                                    {
                                        evnt.CategoryId = 19;
                                    }
                                    else if (tags[2].InnerText.Equals("Nintendo Switch"))
                                    {
                                        evnt.CategoryId = 22;
                                    }
                                    if (tags.Count() == 5)
                                    {
                                        var tagName = tags[4].InnerText.ToLower().Replace("amp;", "").Replace("gry ", "").Replace("- ", "");
                                        AddTag(evnt.PublicEventId, 11, tagName);
                                    }


                                    evnt.Price = ConvertPrice(eventDocument.DocumentNode.SelectSingleNode(".//div[contains(@class,'productPriceInfo__content ta-price')]").InnerText.Replace("&nbsp;", " "));
                                    var authors = eventDocument.DocumentNode.SelectNodes(".//span[contains(@class,'pDAuthorList')]/a").ToList();
                                    foreach (var a in authors)
                                    {
                                        AddTag(evnt.PublicEventId, 13, a.InnerHtml.Replace("&nbsp;", " "));
                                    }

                                }
                                else if (tags[1].InnerText.Equals("Książki"))
                                {
                                    numberOfNewEventsB++;
                                    evnt.CategoryId = 5;
                                    evnt.Price = ConvertPrice(eventDocument.DocumentNode.SelectSingleNode(".//div[contains(@class,'productPriceInfo__content ta-price')]").InnerText.Replace("&nbsp;", " "));
                                    if (tags.Count() > 2)
                                    {
                                        var tagName = tags[2].InnerText.ToLower().Replace("amp;", "").Replace(".", "");
                                        AddTag(evnt.PublicEventId, 14, tagName);
                                    }
                                    if (tags.Count() > 3)
                                    {
                                        var tagName = tags[3].InnerText.ToLower().Replace("amp;", "").Replace(".", "");
                                        AddTag(evnt.PublicEventId, 14, tagName);
                                    }
                                    var authors = eventDocument.DocumentNode.SelectNodes(".//span[contains(@class,'pDAuthorList')]/a").ToList();
                                    foreach (var a in authors)
                                    {
                                        var names = a.InnerText.Replace("&nbsp;", " ").Split(" ");
                                        if (names.Length == 2)
                                        {
                                            AddTag(evnt.PublicEventId, 16, names[1] + " " + names[0]);
                                        }
                                        if (names.Length == 3)
                                        {
                                            AddTag(evnt.PublicEventId, 16, names[2] + " " + names[0] + " " + names[1]);
                                        }
                                        if (names.Length == 4)
                                        {
                                            AddTag(evnt.PublicEventId, 16, names[3] + " " + names[0] + " " + names[1] + " " + names[2]);
                                        }

                                    }

                                }

                            }

                            catch (NullReferenceException)
                            {
                                continue;
                            }
                            catch (ArgumentNullException)
                            {
                                continue;
                            }
                        }
                    }
                }
                catch (NullReferenceException)
                {
                    continue;
                }
                catch (ArgumentNullException)
                {
                    continue;
                }
            }
            _infoService.AddApplicationInfo("Dodano: " + numberOfNewEventsF + " nowych wydarzeń z EMPIK(FILMY)");
            _infoService.AddApplicationInfo("Dodano: " + numberOfNewEventsM + " nowych wydarzeń z EMPIK(MUZYKA)");
            _infoService.AddApplicationInfo("Dodano: " + numberOfNewEventsG + " nowych wydarzeń z EMPIK(GRY)");
            _infoService.AddApplicationInfo("Dodano: " + numberOfNewEventsB + " nowych wydarzeń z EMPIK(KSIĄŻKI)");
            return Task.CompletedTask;
        }
        public Task GetFilmwebEventsFilms()
        {
            int numberOfNewEvents = 0;
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            var events = _context.PublicEvents.Where(e => e.CategoryId == 12);
            var categoryId = 12;

            while (year <= 2020)
            {
                while (month <= 12)
                {
                    try
                    {
                        HttpWebRequest apiRequest = WebRequest.Create("https://www.filmweb.pl/premiere/" + year + "/" + month) as HttpWebRequest;

                        string apiResponse = "";
                        using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                        {
                            StreamReader reader = new StreamReader(response.GetResponseStream());
                            apiResponse = reader.ReadToEnd();
                        }
                        HtmlDocument pageDocument = new HtmlDocument();
                        pageDocument.LoadHtml(apiResponse);
                        var divs = pageDocument.DocumentNode.SelectNodes("//div[contains(@class,'filmPreview filmPreview--FILM Film')]").ToList();

                        foreach (var d in divs)
                        {
                            var fwEvent = new PublicEvent();
                            fwEvent.Name = d.SelectSingleNode(".//h3[contains(@class,'filmPreview__title')]").InnerText;
                            if (d.Attributes["data-release-country-public"] != null)
                            {
                                fwEvent.Date = DateTime.Parse(d.Attributes["data-release-country-public"].Value);
                            }
                            else
                            {
                                fwEvent.Date = DateTime.Parse(d.Attributes["data-release"].Value);
                            }

                            fwEvent.CategoryId = categoryId;
                            if (events.Where(e => e.Name.Equals(fwEvent.Name) && e.Date.Date == fwEvent.Date.Date).Any())
                            {
                                continue;
                            }
                            else
                            {
                                try
                                {
                                    fwEvent.Url = "https://www.filmweb.pl" + d.SelectSingleNode(".//a[contains(@class,'filmPreview__link')]").Attributes["href"].Value;
                                    fwEvent.Promotor = "Filmweb";
                                    string imageName = d.Attributes["data-id"].Value + "_film.jpg";
                                    var imageSavePath = Path.Combine(_enviroment.WebRootPath, "images", imageName);
                                    using (WebClient webClient = new WebClient())
                                    {
                                        webClient.DownloadFile(d.SelectSingleNode(".//img[contains(@class,'filmPoster__image FilmPoster')]").Attributes["data-src"].Value, imageSavePath);
                                    }
                                    fwEvent.ImagePath = "https://ileczasu.pl/images/" + imageName;
                                    fwEvent.Description = "";
                                    _context.PublicEvents.Add(fwEvent);
                                    _context.SaveChanges();
                                    numberOfNewEvents++;

                                    if (d.SelectSingleNode(".//div[contains(@class, 'filmPreview__filmTime')]") != null)
                                    {
                                        var time = d.SelectSingleNode(".//div[contains(@class, 'filmPreview__filmTime')]").Attributes["data-duration"].Value;
                                        if (_context.Tags.Where(t => t.Value == time && t.TagTypeId == 5).Any())
                                        {
                                            var tag = _context.Tags.SingleOrDefault(t => t.Value == time && t.TagTypeId == 5);
                                            _context.SaveChanges();
                                            _context.TagEvents.Add(new TagEvent { TagId = tag.TagId, PublicEventId = fwEvent.PublicEventId });
                                        }
                                        else
                                        {
                                            var tag = new Tag { TagTypeId = 5, Value = time };
                                            _context.Tags.Add(tag);
                                            _context.SaveChanges();
                                            _context.TagEvents.Add(new TagEvent { TagId = tag.TagId, PublicEventId = fwEvent.PublicEventId });
                                        }
                                    }


                                    if (d.SelectSingleNode(".//div[contains(@class,'filmPreview__info')]") != null)
                                    {
                                        var infoNodes = d.SelectNodes(".//div[contains(@class,'filmPreview__info')]").ToList();
                                        if (d.SelectSingleNode(".//div[contains(@class,'filmPreview__description')]/p") != null)
                                        {
                                            fwEvent.Description = d.SelectSingleNode(".//div[contains(@class,'filmPreview__description')]/p").InnerText + "</br></br>";
                                        }
                                        else
                                        {
                                            fwEvent.Description = "";
                                        }
                                        foreach (var dn in infoNodes)
                                        {
                                            var tagTypeName = dn.SelectSingleNode(".//span").InnerHtml.Replace(":", "");
                                            var tagType = _context.TagTypes.SingleOrDefault(tt => tt.Name.Equals(tagTypeName) && tt.CategoryId == 1);
                                            //fwEvent.Description += "<b>" + dn.SelectSingleNode(".//span").InnerHtml + "</b> ";
                                            var liList = dn.SelectNodes(".//ul/li/a").ToList();

                                            foreach (var li in liList)
                                            {
                                                //if (liList.IndexOf(li) < liList.Count - 1)
                                                //{
                                                //    fwEvent.Description += li.InnerText + ", ";
                                                //}
                                                //else
                                                //{
                                                //    fwEvent.Description += li.InnerText + "</br>";
                                                //}

                                                var tagName = li.InnerText;


                                                if (!String.IsNullOrEmpty(tagName) && tagType != null)
                                                {
                                                    if (_context.Tags.Where(t => t.Value == tagName && t.TagTypeId == tagType.TagTypeId).Any())
                                                    {
                                                        var tag = _context.Tags.SingleOrDefault(t => t.Value == tagName && t.TagTypeId == tagType.TagTypeId);
                                                        if (_context.TagEvents.Where(te => te.PublicEventId == fwEvent.PublicEventId && te.TagId == tag.TagId).Count() == 0)
                                                        {
                                                            _context.TagEvents.Add(new TagEvent { PublicEventId = fwEvent.PublicEventId, TagId = tag.TagId });
                                                            _context.SaveChanges();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var tag = new Tag { TagTypeId = tagType.TagTypeId, Value = tagName };
                                                        _context.Tags.Add(tag);
                                                        _context.SaveChanges();
                                                        if (_context.TagEvents.Where(te => te.PublicEventId == fwEvent.PublicEventId && te.TagId == tag.TagId).Count() == 0)
                                                        {
                                                            _context.TagEvents.Add(new TagEvent { PublicEventId = fwEvent.PublicEventId, TagId = tag.TagId });
                                                            _context.SaveChanges();
                                                        }

                                                    }

                                                }


                                            }
                                        }
                                    }
                                    else
                                    {
                                        fwEvent.Description = "";
                                    }
                                    _context.PublicEvents.Update(fwEvent);
                                    _context.SaveChanges();

                                }
                                catch (NullReferenceException)
                                {
                                    continue;
                                }
                                catch (ArgumentNullException)
                                {

                                    continue;
                                }

                            }
                        }
                    }
                    catch (ArgumentNullException)
                    {
                        month++;
                        continue;
                    }
                    catch (NullReferenceException)
                    {
                        month++;
                        continue;
                    }
                    Console.WriteLine(month + "-" + year);
                    month++;
                }
                month = 1;
                year++;
            }
            _infoService.AddApplicationInfo("Dodano: " + numberOfNewEvents + " nowych wydarzeń z Filmweb(FILMY)");
            return Task.CompletedTask;
        }
        public Task GetFilmwebEventsSerials()
        {
            int numberOfNewEvents = 0;
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            var events = _context.PublicEvents.Where(e => e.CategoryId == 13);
            var categoryId = 13;

            while (year <= 2020)
            {

                while (month <= 12)
                {
                    try
                    {
                        HttpWebRequest apiRequest = WebRequest.Create("https://www.filmweb.pl/serials/premiere/" + year + "/" + month + "?showUnpopular=true") as HttpWebRequest;

                        string apiResponse = "";
                        using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                        {
                            StreamReader reader = new StreamReader(response.GetResponseStream());
                            apiResponse = reader.ReadToEnd();
                        }
                        HtmlDocument pageDocument = new HtmlDocument();
                        pageDocument.LoadHtml(apiResponse);
                        var dayRanges = pageDocument.DocumentNode.SelectNodes("//div[contains(@class,'premieresList__dayRange')]").ToList();
                        foreach (var dr in dayRanges)
                        {
                            var eventDate = year + "-" + month + "-" + dr.Attributes["data-day"].Value;
                            var divs = dr.SelectNodes(".//li[contains(@class,'premieresList__box infoBox')]").ToList();

                            foreach (var d in divs)
                            {
                                var fwEvent = new PublicEvent();
                                fwEvent.Name = d.SelectSingleNode(".//h3[contains(@class,'filmPreview__title')]").InnerText + " (" + d.SelectSingleNode(".//ul[contains(@class,'boxBadge')]/li/a").Attributes["title"].Value + ")";
                                fwEvent.Date = DateTime.Parse(eventDate);

                                fwEvent.CategoryId = categoryId;
                                if (events.Where(e => e.Name.Equals(fwEvent.Name) && e.Date.Date == fwEvent.Date.Date).Any())
                                {
                                    continue;
                                }
                                else
                                {
                                    try
                                    {

                                        fwEvent.Url = "https://www.filmweb.pl" + d.SelectSingleNode(".//a[contains(@class,'filmPreview__link')]").Attributes["href"].Value;
                                        fwEvent.Promotor = "Filmweb";
                                        string imageName = d.SelectSingleNode(".//div").Attributes["data-id"].Value + "_film.jpg";
                                        var imageSavePath = Path.Combine(_enviroment.WebRootPath, "images", imageName);
                                        using (WebClient webClient = new WebClient())
                                        {
                                            webClient.DownloadFile(d.SelectSingleNode(".//img[contains(@class,'filmPoster__image FilmPoster')]").Attributes["data-src"].Value, imageSavePath);
                                        }
                                        fwEvent.ImagePath = "https://ileczasu.pl/images/" + imageName;
                                        fwEvent.Description = "";
                                        _context.PublicEvents.Add(fwEvent);
                                        _context.SaveChanges();
                                        numberOfNewEvents++;

                                        if (d.SelectSingleNode(".//div[contains(@class, 'filmPreview__filmTime')]") != null)
                                        {
                                            var time = d.SelectSingleNode(".//div[contains(@class, 'filmPreview__filmTime')]").Attributes["data-duration"].Value;
                                            if (_context.Tags.Where(t => t.Value == time && t.TagTypeId == 5).Any())
                                            {
                                                var tag = _context.Tags.SingleOrDefault(t => t.Value == time && t.TagTypeId == 5);
                                                _context.SaveChanges();
                                                _context.TagEvents.Add(new TagEvent { TagId = tag.TagId, PublicEventId = fwEvent.PublicEventId });
                                            }
                                            else
                                            {
                                                var tag = new Tag { TagTypeId = 5, Value = time };
                                                _context.Tags.Add(tag);
                                                _context.SaveChanges();
                                                _context.TagEvents.Add(new TagEvent { TagId = tag.TagId, PublicEventId = fwEvent.PublicEventId });
                                            }
                                        }

                                        if (d.SelectSingleNode(".//div[contains(@class,'filmPreview__info')]") != null)
                                        {
                                            var infoNodes = d.SelectNodes(".//div[contains(@class,'filmPreview__info')]").ToList();
                                            if (d.SelectSingleNode(".//div[contains(@class,'filmPreview__description')]/p") != null)
                                            {
                                                fwEvent.Description = d.SelectSingleNode(".//div[contains(@class,'filmPreview__description')]/p").InnerText + "</br></br>";
                                            }
                                            else
                                            {
                                                fwEvent.Description = "";
                                            }

                                            foreach (var dn in infoNodes)
                                            {
                                                var tagTypeName = dn.SelectSingleNode(".//span").InnerHtml.Replace(":", "");
                                                if (tagTypeName.Equals("twórca") || tagTypeName.Equals("twórcy"))
                                                {
                                                    tagTypeName = "reżyser";
                                                }
                                                var tagType = _context.TagTypes.SingleOrDefault(tt => tt.Name.Equals(tagTypeName) && tt.CategoryId == 1);
                                                var liList = dn.SelectNodes(".//ul/li/a").ToList();

                                                foreach (var li in liList)
                                                {

                                                    var tagName = li.InnerText;


                                                    if (!String.IsNullOrEmpty(tagName) && tagType != null)
                                                    {
                                                        if (_context.Tags.Where(t => t.Value == tagName && t.TagTypeId == tagType.TagTypeId).Any())
                                                        {
                                                            var tag = _context.Tags.SingleOrDefault(t => t.Value == tagName && t.TagTypeId == tagType.TagTypeId);
                                                            if (_context.TagEvents.Where(te => te.PublicEventId == fwEvent.PublicEventId && te.TagId == tag.TagId).Count() == 0)
                                                            {
                                                                _context.TagEvents.Add(new TagEvent { PublicEventId = fwEvent.PublicEventId, TagId = tag.TagId });
                                                                _context.SaveChanges();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var tag = new Tag { TagTypeId = tagType.TagTypeId, Value = tagName };
                                                            _context.Tags.Add(tag);
                                                            _context.SaveChanges();
                                                            if (_context.TagEvents.Where(te => te.PublicEventId == fwEvent.PublicEventId && te.TagId == tag.TagId).Count() == 0)
                                                            {
                                                                _context.TagEvents.Add(new TagEvent { PublicEventId = fwEvent.PublicEventId, TagId = tag.TagId });
                                                                _context.SaveChanges();
                                                            }

                                                        }

                                                    }

                                                }
                                            }
                                        }
                                        else
                                        {
                                            fwEvent.Description = "";
                                        }
                                        _context.PublicEvents.Update(fwEvent);
                                        _context.SaveChanges();

                                    }
                                    catch (NullReferenceException)
                                    {
                                        continue;
                                    }
                                    catch (ArgumentNullException)
                                    {

                                        continue;
                                    }

                                }
                            }

                        }

                    }
                    catch (ArgumentNullException)
                    {
                        month++;
                        continue;
                    }
                    catch (NullReferenceException)
                    {
                        month++;
                        continue;
                    }
                    Console.WriteLine(month + "-" + year);
                    month++;
                }
                month = 1;
                year++;
            }
            _infoService.AddApplicationInfo("Dodano: " + numberOfNewEvents + " nowych wydarzeń z Filmweb(SERIALE)");
            return Task.CompletedTask;
        }
        public Task GetTicketMasterEvents()
        {
            //    int numberOfNewEvents = 0;
            //    int totalPages = 1;
            //    for (int i = 0; i < totalPages; i++)
            //    {
            //        HttpWebRequest apiRequest = WebRequest.Create("https://app.ticketmaster.com/discovery/v2/events.json?apikey=Hb22xKO3kdbR8dAOEUBp7lfFvR6D26km&countryCode=PL&page=" + i) as HttpWebRequest;

            //        string apiResponse = "";
            //        using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
            //        {
            //            StreamReader reader = new StreamReader(response.GetResponseStream());
            //            apiResponse = reader.ReadToEnd();
            //        }
            //        Models.ApiWrappers.TicketMaster.TicketMasterWrapper tm = JsonConvert.DeserializeObject<Models.ApiWrappers.TicketMaster.TicketMasterWrapper>(apiResponse);
            //        totalPages = tm.page.totalPages;

            //        foreach (var e in tm._embedded.events)
            //        {
            //            try
            //            {

            //                if (e.dates.start.dateTime != null && e._embedded.attractions != null && e.name != null)
            //                {
            //                    if (e.dates.start.dateTime > DateTime.Now)
            //                    {
            //                        bool isDuplicate = false;
            //                        var events = _context.Events.Where(ev => ev.Name.Contains(e._embedded.attractions[0].name) || ev.Name.Contains(e.name));
            //                        if (events != null)
            //                        {
            //                            foreach (var evn in events)
            //                            {
            //                                if (evn.Date.Date == e.dates.start.dateTime.Date)
            //                                {
            //                                    isDuplicate = true;
            //                                }
            //                            }
            //                        }

            //                        if (!isDuplicate)
            //                        {
            //                            Models.Event tmEvent = new Models.Event();
            //                            tmEvent.User = _context.Users.SingleOrDefault(u => u.UserName == "TicketMasterAPI");

            //                            foreach (var img in e.images)
            //                            {
            //                                if (img.ratio.Equals("4_3"))
            //                                {
            //                                    tmEvent.ImagePath = img.url;
            //                                }
            //                            }

            //                            tmEvent.Name = e.name;
            //                            tmEvent.Date = e.dates.start.dateTime.ToLocalTime();
            //                            tmEvent.Place = e._embedded.venues[0].address.line1 + ", " + e._embedded.venues[0].city.name + ", " + e._embedded.venues[0].country.name;

            //                            if (e.classifications[0].segment.name.ToLower().Equals("music"))
            //                            {
            //                                tmEvent.CategoryId = 3;
            //                            }
            //                            else
            //                            {
            //                                continue;
            //                            }

            //                            tmEvent.IsPublic = true;
            //                            tmEvent.Follows = 0;
            //                            tmEvent.Url = e.url;
            //                            tmEvent.TicketPrice = 0;
            //                            if (!String.IsNullOrEmpty(tmEvent.Place))
            //                            {
            //                                CitiesHelper ce = new CitiesHelper();
            //                                var cities = _context.Cities.Select(c => c.Name).ToList();
            //                                var cityName = ce.GetCityName(tmEvent.Place);
            //                                if (!String.IsNullOrEmpty(cityName))
            //                                {
            //                                    if (!cities.Contains(cityName))
            //                                    {
            //                                        var city = new City { Name = cityName, NumberOfEvents = 1 };
            //                                        _context.Cities.Add(city);
            //                                        _context.SaveChanges();
            //                                    }
            //                                    else
            //                                    {
            //                                        var city = _context.Cities.SingleOrDefault(c => c.Name == cityName);
            //                                        city.NumberOfEvents += 1;
            //                                        _context.SaveChanges();
            //                                    }
            //                                }
            //                            }
            //                            try
            //                            {
            //                                _context.Events.Add(tmEvent);
            //                                _context.SaveChanges();
            //                                numberOfNewEvents++;
            //                            }
            //                            catch (DbUpdateException)
            //                            {

            //                            }
            //                            Console.Write("Added" + e.name);
            //                        }
            //                    }
            //                }
            //            }
            //            catch (NullReferenceException)
            //            {
            //                continue;
            //            }
            //        }
            //    }
            //    _infoService.AddApplicationInfo("Dodano: " + numberOfNewEvents + " nowych wydarzeń z TicketMaster API");
            return Task.CompletedTask;
        }
        private double ConvertPrice(string price)
        {
            if (!String.IsNullOrEmpty(price))
            {
                return Convert.ToDouble(price.Replace("zł", "").Trim());
            }
            else return 0;
            
        }
        private void AddTag(int eventId, int tagTypeId, string tagValue)
        {
            try
            {
                if (_context.Tags.Where(t => t.Value == tagValue && t.TagTypeId == tagTypeId).Count() == 0)
                {
                    var tag = new Tag { Value = tagValue, TagTypeId = tagTypeId };
                    if (_context.TagEvents.Where(te => te.PublicEventId == eventId && te.TagId == tag.TagId).Count() == 0)
                    {
                        _context.Tags.Add(tag);
                        _context.SaveChanges();
                        _context.TagEvents.Add(new TagEvent { PublicEventId = eventId, TagId = tag.TagId });
                    }
                }
                else
                {
                    var tag = _context.Tags.SingleOrDefault(t => t.Value == tagValue && t.TagTypeId == tagTypeId);
                    if (_context.TagEvents.Where(te => te.PublicEventId == eventId && te.TagId == tag.TagId).Count() == 0)
                    {
                        _context.TagEvents.Add(new TagEvent { PublicEventId = eventId, TagId = tag.TagId });
                    }
                }

                _context.SaveChanges();

            }
            catch (DbUpdateException)
            {

            }
        }

    }
}
