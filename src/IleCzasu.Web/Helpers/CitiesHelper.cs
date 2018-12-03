using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IleCzasu.Extensions
{
    public class CitiesHelper
    {
        public string GetCityName(string place)
        {
            if (!String.IsNullOrEmpty(place))
            {
                string city;
                String[] words = place.Split(',');
                if(words.Length >= 2)
                {
                    city = words[words.Length - 2].Trim();
                    if (city.ToLower().Equals("warsaw"))
                    {
                        city = "Warszawa";
                    }
                    else if (city.ToLower().Equals("krakow"))
                    {
                        city = "Kraków";
                    }
                    else if (city.ToLower().Equals("lodz"))
                    {
                        city = "Łódź";
                    }
                    else if (city.ToLower().Equals("gdansk"))
                    {
                        city = "Gdańsk";
                    }
                    else if (city.ToLower().Equals("poznan"))
                    {
                        city = "Poznań";
                    }
                    else if (city.ToLower().Equals("wroclaw"))
                    {
                        city = "Wrocław";
                    }
                    return city;
                }
                else return null;
            }

            else return null;
        
        }
    }
}
