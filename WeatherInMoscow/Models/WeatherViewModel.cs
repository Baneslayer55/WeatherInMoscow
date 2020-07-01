using PagedList;
using System.Web.Mvc;

namespace WeatherInMoscow.Models
{
    public class WeatherViewModel
    {
        public IPagedList<Weather> Weathers { get; set; }

        public int? YearFilter { get; set; }

        public int? MonthFilter { get; set; }

        public SelectList Year { get; set; }

        public SelectList Month { get; set; }
    }
}