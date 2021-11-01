using System;

namespace WeatherApiTechTask
{
    internal class WeatherLoader : IEnricher<BaseCityModel, Forecast>
    {
        private IRestClient _restClient;
        private WeatherLoadConfiguration _weatherConfig;

        public WeatherLoader(IRestClient restClient, WeatherLoadConfiguration weatherConfig)
        {
            _restClient = restClient;
            _weatherConfig = weatherConfig;
        }

        public Forecast Load(BaseCityModel city)
        {
            var result = _restClient.Get<WeatherInfoResponse>(_weatherConfig.Url,
                new()
                {
                    { _weatherConfig.ParamNames.ApiKey, _weatherConfig.ApiKey },
                    { _weatherConfig.ParamNames.Days, _weatherConfig.Days },
                    { _weatherConfig.ParamNames.Position, $"{city.Latitude:r} {city.Longitude:r}" }
                });
            return new Forecast(result.Forecast.ForecastDays[0].Day.Condition.Text,
                result.Forecast.ForecastDays[1].Day.Condition.Text);
        }
    }
}