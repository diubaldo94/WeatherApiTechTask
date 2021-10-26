using System;
using System.Collections.Generic;

namespace WeatherApiTechTask
{
    internal class CityLoader
    {
        private IRestClient _restClient;
        private CityLoadConfiguration _cityConfig;

        public CityLoader(IRestClient restClient, CityLoadConfiguration cityConfig)
        {
            _restClient = restClient;
            _cityConfig = cityConfig;
        }

        internal IEnumerable<BaseCityModel> Load()
        {
            var response = _restClient.Get<CitiesInfoResponse>(_cityConfig.Url, null);
            var cityList = new List<BaseCityModel>();
            foreach(var city in response.Cities)
            {
                cityList.Add(new BaseCityModel(city.Name, city.Latitude, city.Longitude));
            }
            return cityList;
        }
    }
}