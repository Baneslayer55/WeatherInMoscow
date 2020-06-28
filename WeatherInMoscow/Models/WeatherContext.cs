using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WeatherInMoscow.Models
{
    public class WeatherContext : DbContext
    {
        public DbSet<Weather> Weathers { get; set; }
    }
}