using System.Collections.Generic;
using System.Web.Mvc;

namespace WeatherInMoscow.Models
{
    public class WeatherViewModel
    {        
        public IEnumerable<Weather> Weathers { get; set; }

        public SelectList Year { get; set; }

        public SelectList Month { get; set; }        
    }
}