using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StreamingService.Models
{
    public class MovieData
    {
        public System.Guid Id { get; set; }
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        public string Cast { get; set; }
        public string Country { get; set; }
        public string ReleaseDate { get; set; }
        public string Dabing { get; set; }
        public string CoverPath { get; set; }
        public string Lenght { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> StreamSources { get; set; } = new List<string>();
    }
}