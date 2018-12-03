using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace IleCzasu.Models.ApiWrappers.KupBilecik
{
    [XmlRoot(ElementName = "hall_full_name")]
    public class Hall_full_name
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "hall_city")]
    public class Hall_city
    {
        [XmlAttribute(AttributeName = "hall_city_id")]
        public string Hall_city_id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "hall_woj")]
    public class Hall_woj
    {
        [XmlAttribute(AttributeName = "country")]
        public string Country { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "cena_min")]
    public class Cena_min
    {
        [XmlAttribute(AttributeName = "waluta")]
        public string Waluta { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "event")]
    public class Event
    {
        [XmlElement(ElementName = "event_type")]
        public string Event_type { get; set; }
        [XmlElement(ElementName = "event_date")]
        public string Event_date { get; set; }
        [XmlElement(ElementName = "event_time")]
        public string Event_time { get; set; }
        [XmlElement(ElementName = "hall_full_name")]
        public Hall_full_name Hall_full_name { get; set; }
        [XmlElement(ElementName = "geo_lat")]
        public string Geo_lat { get; set; }
        [XmlElement(ElementName = "geo_long")]
        public string Geo_long { get; set; }
        [XmlElement(ElementName = "hall_city")]
        public Hall_city Hall_city { get; set; }
        [XmlElement(ElementName = "hall_woj")]
        public Hall_woj Hall_woj { get; set; }
        [XmlElement(ElementName = "hall_address")]
        public string Hall_address { get; set; }
        [XmlElement(ElementName = "hall_code")]
        public string Hall_code { get; set; }
        [XmlElement(ElementName = "event_title")]
        public string Event_title { get; set; }
        [XmlElement(ElementName = "event_text")]
        public string Event_text { get; set; }
        [XmlElement(ElementName = "artist_name")]
        public string Artist_name { get; set; }
        [XmlElement(ElementName = "artist_persons")]
        public string Artist_persons { get; set; }
        [XmlElement(ElementName = "artist_text")]
        public string Artist_text { get; set; }
        [XmlElement(ElementName = "artist_link")]
        public string Artist_link { get; set; }
        [XmlElement(ElementName = "event_link")]
        public string Event_link { get; set; }
        [XmlElement(ElementName = "jpg_link")]
        public string Jpg_link { get; set; }
        [XmlElement(ElementName = "jpg_min_link")]
        public string Jpg_min_link { get; set; }
        [XmlElement(ElementName = "last_update")]
        public string Last_update { get; set; }
        [XmlElement(ElementName = "cena_min")]
        public Cena_min Cena_min { get; set; }
        [XmlAttribute(AttributeName = "event_id")]
        public string Event_id { get; set; }
    }

    [XmlRoot(ElementName = "events")]
    public class Events
    {
        [XmlElement(ElementName = "event")]
        public List<Event> Event { get; set; }
    }

}


