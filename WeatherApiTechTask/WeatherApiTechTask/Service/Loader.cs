using System;
using System.Collections.Generic;

namespace WeatherApiTechTask
{
    internal class Loader
    {
        private CityLoader _cityLoader;
        private WeatherLoader _weatherLoader;

        public Loader(CityLoader cityLoader, WeatherLoader weatherLoader)
        {
            _cityLoader = cityLoader;
            _weatherLoader = weatherLoader;
        }

        internal IEnumerable<CityModel> Load()
        {
            var cities = _cityLoader.Load();
            var enrichedCities = new List<CityModel>();
            foreach(var city in cities)
            {
                var dataToEnrich = new CityModel(city.Name, city.Latitude, city.Longitude);
                var forecast = _weatherLoader.Load(city);
                dataToEnrich.Forecast = forecast;
                enrichedCities.Add(dataToEnrich);
            }
            return enrichedCities;
        }
    }
}