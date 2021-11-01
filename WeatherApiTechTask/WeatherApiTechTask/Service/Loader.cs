using System;
using System.Collections.Generic;

namespace WeatherApiTechTask
{
    internal class Loader : ILoader<IEnumerable<CityModel>>
    {
        private ILoader<IEnumerable<BaseCityModel>> _cityLoader;
        private IEnricher<BaseCityModel, Forecast> _weatherLoader;

        public Loader(ILoader<IEnumerable<BaseCityModel>> cityLoader, IEnricher<BaseCityModel, Forecast> weatherLoader)
        {
            _cityLoader = cityLoader;
            _weatherLoader = weatherLoader;
        }

        public IEnumerable<CityModel> Load()
        {
            var cities = _cityLoader.Load();
            var enrichedCities = new List<CityModel>();
            foreach (var city in cities)
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