using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WeatherApiTechTask;

namespace WeatherApiTechTask.Test
{
    public partial class WeatherAppTest
    {
        private IWeatherApp _sut;


        //private IPublisher _publisher;
        //private IRestClient _restClient;
        

        private readonly CityLoadConfiguration _cityConfig = new() { Url = "cities.it" };
        private readonly WeatherLoadConfiguration _weatherConfig = new() {
            Url = "weather.it",
            ApiKey = "apikey", 
            Days = "2",
            ParamNames = new WeatherParams() { Days = "days", ApiKey = "apikey111", Position = "q"}
        };

        private CityTestInputObj[] _testData = new CityTestInputObj[]{
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



        //PROVA NETMOCK
        [SetUp]
        public void Setup()
        {
        }

        private void SetupRestClientCity(Mock<IRestClient> client, bool willThrow = false)
        {
            var cities = _testData.Select(i => new CityInfoResponse
            { Name = i.Name, Latitude = i.Latitude, Longitude = i.Longitude })
                .ToList();
            var setup = client.Setup(c => c.Get<CityInfoResponse[]>(_cityConfig.Url, null, null));
            if(willThrow)
                setup.Throws<Exception>().Verifiable();
            else
                setup.Returns(cities.ToArray()).Verifiable();
        }

        private void SetupRestClientWeather(Mock<IRestClient> client, bool willThrow = false)
        {
            foreach (var testData in _testData)
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
                var setup = client.Setup(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url,
                   It.Is<Dictionary<string, string>>(
                       d => d[_weatherConfig.ParamNames.Position]
                       .Equals($"{testData.Latitude:r} {testData.Longitude:r}")), null));
                if (willThrow)
                    setup.Throws<Exception>().Verifiable();
                else
                    setup.Returns(response).Verifiable();
            }
        }

        //private void SetupNotifier(Mock<INotifier> console, bool willThrow = false)
        //{

        //}

        [Test] //Chek order of the send of messgaes
        public void LoadSomeCitiesWithWeatherAndNotifyThem()
        {
            Mock<INotifier> consoleMock = new();
            Mock<IRestClient> restClientMockForCity = new();
            Mock<IRestClient> restClientMockForWeather = new();

            SetupRestClientCity(restClientMockForCity);
            SetupRestClientWeather(restClientMockForWeather);

            //build sut
            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(restClientMockForCity.Object, _cityConfig),
                    new WeatherLoader(restClientMockForWeather.Object, _weatherConfig)
                    ),
                new Publisher(consoleMock.Object)
                );

            _sut.Run();

            restClientMockForCity.Verify();
            restClientMockForWeather.Verify();
            consoleMock.Verify(
                c => c.Notify(It.Is<WeatherOutcome>(e => _testData.Select(i => i.ExpectedOutcome).Contains(e.ExpectedOutcome))), 
                Times.Exactly(_testData.Length)); //problem: exatcly one for each elemtn??
        }

        [Test]
        public void ThrowExceptionIfErrorOnLoadingCities()
        {
            Mock<INotifier> consoleMock = new();
            Mock<IRestClient> restClientMockForCity = new();
            Mock<IRestClient> restClientMockForWeather = new();

            SetupRestClientCity(restClientMockForCity, true);
            SetupRestClientWeather(restClientMockForWeather);

            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(restClientMockForCity.Object, _cityConfig),
                    new WeatherLoader(restClientMockForWeather.Object, _weatherConfig)
                    ),
                new Publisher(consoleMock.Object)
                );

            Assert.Throws<Exception>(() => _sut.Run());

            restClientMockForCity.Verify(); 
            restClientMockForWeather.Verify(c => c.Get<CityInfoResponse>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), null), Times.Never);
            consoleMock.Verify(c => c.Notify(It.IsAny<WeatherOutcome>()), Times.Never);
        }

        [Test]
        public void ThrowExceptionIfErrorOnLoadingWeather()
        {
            Mock<INotifier> consoleMock = new();
            Mock<IRestClient> restClientMockForCity = new();
            Mock<IRestClient> restClientMockForWeather = new();
            SetupRestClientCity(restClientMockForCity);
            SetupRestClientWeather(restClientMockForWeather, true);

            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(restClientMockForCity.Object, _cityConfig),
                    new WeatherLoader(restClientMockForWeather.Object, _weatherConfig)
                    ),
                new Publisher(consoleMock.Object)
                );

            Assert.Throws<Exception>(() => _sut.Run());

            restClientMockForCity.Verify();
            restClientMockForWeather.Verify(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url, It.IsAny<Dictionary<string,string>>(), null), Times.Once);
            consoleMock.Verify(c => c.Notify(It.IsAny<WeatherOutcome>()), Times.Never);
        }

        [Test]
        public void ThrowExceptionIfErrorOnNotifying()
        {
            Mock<INotifier> consoleMock = new();
            Mock<IRestClient> restClientMockForCity = new();
            Mock<IRestClient> restClientMockForWeather = new();
            SetupRestClientCity(restClientMockForCity);
            SetupRestClientWeather(restClientMockForWeather);
            consoleMock.Setup(c => c.Notify(It.IsAny<WeatherOutcome>())).Throws<Exception>().Verifiable();

            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(restClientMockForCity.Object, _cityConfig),
                    new WeatherLoader(restClientMockForWeather.Object, _weatherConfig)
                    ),
                new Publisher(consoleMock.Object)
                );            

            Assert.Throws<Exception>(() => _sut.Run());

            restClientMockForCity.Verify();
            restClientMockForWeather.Verify();
            consoleMock.Verify();
        }
    }
}