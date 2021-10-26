﻿using System;

namespace WeatherApiTechTask
{
    internal class WeatherLoader
    {
        private IRestClient _restClient;
        private WeatherLoadConfiguration _weatherConfig;

        public WeatherLoader(IRestClient restClient, WeatherLoadConfiguration weatherConfig)
        {
            _restClient = restClient;
            _weatherConfig = weatherConfig;
        }

        internal Forecast Load(BaseCityModel city)
        {
            var result = _restClient.Get<WeatherInfoResponse>(_weatherConfig.Url,
                new()
                {
                    { _weatherConfig.ParamNames.ApiKey, _weatherConfig.ApiKey },
                    { _weatherConfig.ParamNames.Days, _weatherConfig.Days },
                    { _weatherConfig.ParamNames.Latitude, city.Latitude.ToString() },
                    { _weatherConfig.ParamNames.Longitude, city.Longitude.ToString() }
                });
            return new Forecast(result.Forecast[0].Day.Condition.Text, result.Forecast[1].Day.Condition.Text);
        }
    }
}