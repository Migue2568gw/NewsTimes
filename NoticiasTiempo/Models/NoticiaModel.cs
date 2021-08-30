using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoticiasTiempo.Models
{
    public class NoticiaModel
    {
        public List<Articles> articles { get; set; }
    }

    public class Articles
    {
        public string author { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public DateTime publishedAt { get; set; }
        public string content { get; set; }
    }
}