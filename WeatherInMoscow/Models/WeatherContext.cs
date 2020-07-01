using System.Data.Entity;

namespace WeatherInMoscow.Models
{
    public class WeatherContext : DbContext
    {
        public DbSet<Weather> Weathers { get; set; }
    }
}