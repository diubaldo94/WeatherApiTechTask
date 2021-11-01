using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherApiTechTask.Test
{
    internal class WeatherTestUtils
    {
        private static readonly CityLoadConfiguration _cityConfig = new() { Url = "cities.it" };
        private static readonly WeatherLoadConfiguration _weatherConfig = new()
        {
            Url = "weather.it",
            ApiKey = "apikey",
            Days = "2",
            ParamNames = new WeatherParams() { Days = "days", ApiKey = "apikey111", Position = "q" }
        };

        private static readonly CityTestInputObj[] _testData = new CityTestInputObj[]{
            new CityTestInputObj("Milan", 40.9, 50.3,
                "Sunny", "Cloudy and rainy",
                "Processed city Milan | Sunny - Cloudy and rainy"),
            new CityTestInputObj("Turin", 90, 80.977,
                "Cloudy and rainy", "Cloudy and rainy",
                "Processed city Turin | Cloudy and rainy - Cloudy and rainy"),
            new CityTestInputObj("Pescara", 266, 54.3,
                "Partially cloudy", "Sunny",
                "Processed city Pescara | Partially cloudy - Sunny"),
        };

        internal static WeatherLoadConfiguration WeatherConfig => _weatherConfig;
        internal static CityLoadConfiguration CityConfig => _cityConfig;

        internal static CityTestInputObj[] TestData => _testData;

        internal static void SetupRestClientCity(Mock<IRestClient> client, bool willThrow = false)
        {
            var cities = TestData.Select(i => new CityInfoResponse
            { Name = i.Name, Latitude = i.Latitude, Longitude = i.Longitude })
                .ToList();
            var setup = client.Setup(c => c.Get<CityInfoResponse[]>(CityConfig.Url, null, null));
            if (willThrow)
                setup.Throws<Exception>().Verifiable();
            else
                setup.Returns(cities.ToArray()).Verifiable();
        }

        internal static void SetupRestClientWeather(Mock<IRestClient> client, bool willThrow = false)
        {
            foreach (var testData in TestData)
            {
                var response = new WeatherInfoResponse()
                {
                    Forecast = new ForecastDto()
                    {
                        ForecastDays = new ForecastDayDto[] {
                            new ForecastDayDto() { Day = new DayDto() { Condition = new ConditionDto() { Text = testData.WeatherToday} } },
                            new ForecastDayDto() { Day = new DayDto() { Condition = new ConditionDto() { Text = testData.WeatherTomorrow} } },
                        }
                    }
                };
                var setup = client.Setup(c => c.Get<WeatherInfoResponse>(WeatherConfig.Url,
                   It.Is<Dictionary<string, string>>(
                       d => d[WeatherConfig.ParamNames.Position]
                       .Equals($"{testData.Latitude:r} {testData.Longitude:r}")), null));
                if (willThrow)
                    setup.Throws<Exception>().Verifiable();
                else
                    setup.Returns(response).Verifiable();
            }
        }
    }
}