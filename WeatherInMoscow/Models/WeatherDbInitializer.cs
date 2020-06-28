using System.Data.Entity;

namespace WeatherInMoscow.Models
{
    public class WeatherDbInitializer : DropCreateDatabaseAlways<WeatherContext>
    {
        protected override void Seed(WeatherContext db)
        {
            base.Seed(db);
        }
    }
}