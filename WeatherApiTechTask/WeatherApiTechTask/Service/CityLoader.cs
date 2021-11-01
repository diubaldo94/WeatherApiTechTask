using System;
using System.Collections.Generic;

namespace WeatherApiTechTask
{
    internal class CityLoader : ILoader<IEnumerable<BaseCityModel>>
    {
        private IRestClient _restClient;
        private CityLoadConfiguration _cityConfig;

        public CityLoader(IRestClient restClient, CityLoadConfiguration cityConfig)
        {
            _restClient = restClient;
            _cityConfig = cityConfig;
        }

        public IEnumerable<BaseCityModel> Load()
        {
            var response = _restClient.Get<CityInfoResponse[]>(_cityConfig.Url, null);
            var cityList = new List<BaseCityModel>();
            foreach(var city in response)
            {
                cityList.Add(new BaseCityModel(city.Name, city.Latitude, city.Longitude));
            }
            return cityList;
        }
    }
}