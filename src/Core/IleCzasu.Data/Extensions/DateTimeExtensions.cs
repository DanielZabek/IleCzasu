using System;
using System.Collections.Generic;
using System.Text;

namespace IleCzasu.Application.Extensions
{
    public static class DateTimeExtensions
    {
        public static string TimeAgo(this DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} sekund temu", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("{0} minut temu", timeSpan.Minutes) :
                    "minutę temu";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("{0} godzin temu", timeSpan.Hours) :
                    "godzinę temu";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("{0} dni temu", timeSpan.Days) :
                    "wczoraj";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("{0} miesięcy temu", timeSpan.Days / 30) :
                    "miesiąc temu";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("{0} lat temu", timeSpan.Days / 365) :
                    "rok temu";
            }

            return result;
        }
    }
}
