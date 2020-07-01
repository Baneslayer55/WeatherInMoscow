using System;

namespace WeatherInMoscow.Models
{
    public class Weather
    {
        public int WeatherID { get; set; }

        public DateTime WeatherDateTime { get; set; }

        public double? Temperature { get; set; }

        public double? Humidity { get; set; } //Влажность

        public double? Dewpoint { get; set; } //точка росы

        public int? Pressure { get; set; }

        public string WindDirection { get; set; }

        public int? WindSpeed { get; set; }

        public int? Cloudiness { get; set; }

        public int? LowCloudCover { get; set; } //нижняя граница облачности

        public int? HorizontalVisibility { get; set; }

        public string WeatherConditions { get; set; }
    }
}