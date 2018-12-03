using IleCzasu.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IleCzasu.Infrastructure;
namespace IleCzasu.Services
{
 

    public class Reminder : IReminder
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _enviroment;

        public Reminder(ApplicationDbContext context, IHostingEnvironment enviroment)
        {
            _enviroment = enviroment;
            _context = context;
        }
        public Task SendEmail()
        {
            SmtpClient client = new SmtpClient("smtp.webio.pl");
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("powiadomienia@ileczasu.pl", "VeV<j2Q");
            var settingsList = _context.ReminderSettings.Where(s => s.Active == true).ToList();
            var users = _context.Users
               .Include(f => f.UserFollows)
               .ThenInclude(f => f.Event)
               .ToList();
            var path = Path.Combine(_enviroment.WebRootPath, "EmailTemplates", "ReminderTemplate.html");
           
            foreach (var s in settingsList)
            {
                var user = users.Where(u => u.Id == s.UserId).Single();
                var userEvents = user.UserFollows.Select(e => e.Event).ToList();

                foreach (var e in userEvents)
                {
                    TimeSpan daysLeft = e.Date.Date - DateTime.Now.Date;
                    if (s.DaysBefore == daysLeft.TotalDays)
                    {

                        string body = string.Empty;
                        using (StreamReader reader = new StreamReader(path))

                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("{name}", e.Name); //replacing the required things  
                        body = body.Replace("{description}", e.Description);
                        body = body.Replace("{date}", e.Date.ToString("dd MMMM, yyyy"));
                        body = body.Replace("{imagePath}", e.ImagePath);
                        body = body.Replace("{place}", e.Place);
                        if (daysLeft.TotalDays == 1)
                        {
                            body = body.Replace("{daysLeft}","JUTRO!");
                        } else
                        {
                            body = body.Replace("{daysLeft}","ZA " + daysLeft.TotalDays + " DNI!");
                        }
                            


                        MailMessage mailMessage = new MailMessage();
                        mailMessage.IsBodyHtml = true;
                        mailMessage.From = new MailAddress("powiadomienia@ileczasu.pl");
                        mailMessage.To.Add(user.Email);
                        mailMessage.Body = body;
                        mailMessage.Subject = WebUtility.HtmlDecode(Regex.Replace(e.Name, "<[^>]*(>|$)", string.Empty));
                        client.Send(mailMessage);

                    }
                 
                }
            }
            return Task.CompletedTask;
        }
       
    }
}
