using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoticiasTiempo.Models
{
    public class tiempoModel
    {
        public string name { get; set; }
        public List<Weather> weather { get; set; }

        public Main main { get; set; }

        public Sys sys { get; set; }
    }
    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }

    }
    public class Sys
    {
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class Main
    {
        public string temp { get; set; }
        public string humidity { get; set; }
        public string pressure { get; set; }
    }

    public class Wind
    {
        public decimal speed { get; set; }

    }
}