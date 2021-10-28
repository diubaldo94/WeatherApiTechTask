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
        private Mock<INotifier> _consoleMock = new();
        private Mock<IRestClient> _restClientMockForCity = new();
        private Mock<IRestClient> _restClientMockForWeather = new();

        private readonly CityLoadConfiguration _cityConfig = new() { Url = "cities.it" };
        private readonly WeatherLoadConfiguration _weatherConfig = new() {
            Url = "weather.it",
            ApiKey = "apikey", 
            Days = "2",
            ParamNames = new WeatherParams() { Days = "days", ApiKey = "apikey111", Latitude = "latitude", Longitude = "longitude"}
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

        [Test] //Chek order of the send of messgaes
        public void LoadSomeCitiesWithWeatherAndNotifyThem()
        {
            var citiesInfoResponse = new CitiesInfoResponse() { Cities = new CityInfoResponse[_testData.Length] };
            var cities = new List<CityInfoResponse>();
            foreach (var testData in _testData)
            {
                cities.Add(new CityInfoResponse
                { Name = testData.Name, Latitude = testData.Latitude, Longitude = testData.Longitude });
                var @params = new Dictionary<string, string> {
                        //todo vedi meglio gestione double 
                            { _weatherConfig.ParamNames.Latitude, testData.Latitude.ToString() },
                            { _weatherConfig.ParamNames.Longitude, testData.Longitude.ToString() },
                            { _weatherConfig.ParamNames.ApiKey, _weatherConfig.ApiKey },
                            { _weatherConfig.ParamNames.Days, _weatherConfig.Days },
                        };
                var response = new WeatherInfoResponse()
                {
                    Forecast = new ForecastDay[] {
                            new ForecastDay() { Day = new Day() { Condition = new Condition() { Text = testData.WeatherToday} } },
                            new ForecastDay() { Day = new Day() { Condition = new Condition() { Text = testData.WeatherTomorrow} } },
                        }
                };
                _restClientMockForWeather.Setup(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url,
                    It.Is<Dictionary<string, string>>(d => d[_weatherConfig.ParamNames.Latitude].Equals(testData.Latitude.ToString())
                        && d[_weatherConfig.ParamNames.Longitude].Equals(testData.Longitude.ToString()))))
                    .Returns(response).Verifiable();
            }
            citiesInfoResponse.Cities = cities.ToArray();
            _restClientMockForCity.Setup(c => c.Get<CitiesInfoResponse>(_cityConfig.Url, null))
                .Returns(citiesInfoResponse).Verifiable();

            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(_restClientMockForCity.Object, _cityConfig),
                    new WeatherLoader(_restClientMockForWeather.Object, _weatherConfig)
                    ),
                new Publisher(_consoleMock.Object)
                );

            _sut.Run();

            _restClientMockForCity.Verify();
            _restClientMockForWeather.Verify();
            _consoleMock.Verify(
                c => c.Notify(It.Is<WeatherOutcome>(e => _testData.Select(i => i.ExpectedOutcome).Contains(e.ExpectedOutcome))), 
                Times.Exactly(_testData.Length)); //problem: exatcly one for each elemtn??
        }

        [Test]
        public void ThrowExceptionIfErrorOnLoadingCities()
        {
            var citiesInfoResponse = new CitiesInfoResponse() { Cities = new CityInfoResponse[_testData.Length] };
            var cities = new List<CityInfoResponse>();
            foreach (var testData in _testData)
            {
                cities.Add(new CityInfoResponse
                { Name = testData.Name, Latitude = testData.Latitude, Longitude = testData.Longitude });
                var @params = new Dictionary<string, string> {
                        //todo vedi meglio gestione double 
                            { _weatherConfig.ParamNames.Latitude, testData.Latitude.ToString() },
                            { _weatherConfig.ParamNames.Longitude, testData.Longitude.ToString() },
                            { _weatherConfig.ParamNames.ApiKey, _weatherConfig.ApiKey },
                            { _weatherConfig.ParamNames.Days, _weatherConfig.Days },
                        };
                var response = new WeatherInfoResponse()
                {
                    Forecast = new ForecastDay[] {
                            new ForecastDay() { Day = new Day() { Condition = new Condition() { Text = testData.WeatherToday} } },
                            new ForecastDay() { Day = new Day() { Condition = new Condition() { Text = testData.WeatherTomorrow} } },
                        }
                };
                _restClientMockForWeather.Setup(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url,
                    It.Is<Dictionary<string, string>>(d => d[_weatherConfig.ParamNames.Latitude].Equals(testData.Latitude.ToString())
                        && d[_weatherConfig.ParamNames.Longitude].Equals(testData.Longitude.ToString()))))
                    .Returns(response).Verifiable();
            }
            citiesInfoResponse.Cities = cities.ToArray();
            _restClientMockForCity.Setup(c => c.Get<CitiesInfoResponse>(_cityConfig.Url, null))
                .Throws<Exception>().Verifiable(); //which exception?

            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(_restClientMockForCity.Object, _cityConfig),
                    new WeatherLoader(_restClientMockForWeather.Object, _weatherConfig)
                    ),
                new Publisher(_consoleMock.Object)
                );

            Assert.Throws<Exception>(() => _sut.Run());

            _restClientMockForCity.Verify(); 
            _restClientMockForWeather.Verify(c => c.Get<CityInfoResponse>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            _consoleMock.Verify(c => c.Notify(It.IsAny<WeatherOutcome>()), Times.Never);
        }

        [Test]
        public void ThrowExceptionIfErrorOnLoadingWeather()
        {
            var citiesInfoResponse = new CitiesInfoResponse() { Cities = new CityInfoResponse[_testData.Length] };
            var cities = new List<CityInfoResponse>();
            foreach (var testData in _testData)
            {
                cities.Add(new CityInfoResponse
                { Name = testData.Name, Latitude = testData.Latitude, Longitude = testData.Longitude });
                var @params = new Dictionary<string, string> {
                        //todo vedi meglio gestione double 
                            { _weatherConfig.ParamNames.Latitude, testData.Latitude.ToString() },
                            { _weatherConfig.ParamNames.Longitude, testData.Longitude.ToString() },
                            { _weatherConfig.ParamNames.ApiKey, _weatherConfig.ApiKey },
                            { _weatherConfig.ParamNames.Days, _weatherConfig.Days },
                        };
                var response = new WeatherInfoResponse()
                {
                    Forecast = new ForecastDay[] {
                            new ForecastDay() { Day = new Day() { Condition = new Condition() { Text = testData.WeatherToday} } },
                            new ForecastDay() { Day = new Day() { Condition = new Condition() { Text = testData.WeatherTomorrow} } },
                        }
                };
                _restClientMockForWeather.Setup(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url,
                    It.Is<Dictionary<string, string>>(d => d[_weatherConfig.ParamNames.Latitude].Equals(testData.Latitude.ToString())
                        && d[_weatherConfig.ParamNames.Longitude].Equals(testData.Longitude.ToString()))))
                    .Throws<Exception>();
            }
            citiesInfoResponse.Cities = cities.ToArray();
            _restClientMockForCity.Setup(c => c.Get<CitiesInfoResponse>(_cityConfig.Url, null))
                .Returns(citiesInfoResponse).Verifiable();

            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(_restClientMockForCity.Object, _cityConfig),
                    new WeatherLoader(_restClientMockForWeather.Object, _weatherConfig)
                    ),
                new Publisher(_consoleMock.Object)
                );

            Assert.Throws<Exception>(() => _sut.Run());

            _restClientMockForCity.Verify();
            _restClientMockForWeather.Verify(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url, It.IsAny<Dictionary<string,string>>()), Times.Once);
            _consoleMock.Verify(c => c.Notify(It.IsAny<WeatherOutcome>()), Times.Never);
        }

        [Test]
        public void ThrowExceptionIfErrorOnNotifying()
        {
            var citiesInfoResponse = new CitiesInfoResponse() { Cities = new CityInfoResponse[_testData.Length] };
            var cities = new List<CityInfoResponse>();
            foreach (var testData in _testData)
            {
                cities.Add(new CityInfoResponse
                { Name = testData.Name, Latitude = testData.Latitude, Longitude = testData.Longitude });
                var @params = new Dictionary<string, string> {
                        //todo vedi meglio gestione double 
                            { _weatherConfig.ParamNames.Latitude, testData.Latitude.ToString() },
                            { _weatherConfig.ParamNames.Longitude, testData.Longitude.ToString() },
                            { _weatherConfig.ParamNames.ApiKey, _weatherConfig.ApiKey },
                            { _weatherConfig.ParamNames.Days, _weatherConfig.Days },
                        };
                var response = new WeatherInfoResponse()
                {
                    Forecast = new ForecastDay[] {
                            new ForecastDay() { Day = new Day() { Condition = new Condition() { Text = testData.WeatherToday} } },
                            new ForecastDay() { Day = new Day() { Condition = new Condition() { Text = testData.WeatherTomorrow} } },
                        }
                };
                _restClientMockForWeather.Setup(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url,
                    It.Is<Dictionary<string, string>>(d => d[_weatherConfig.ParamNames.Latitude].Equals(testData.Latitude.ToString())
                        && d[_weatherConfig.ParamNames.Longitude].Equals(testData.Longitude.ToString()))))
                    .Returns(response).Verifiable();
            }
            citiesInfoResponse.Cities = cities.ToArray();
            _restClientMockForCity.Setup(c => c.Get<CitiesInfoResponse>(_cityConfig.Url, null))
                .Returns(citiesInfoResponse).Verifiable();
            _consoleMock.Setup(c => c.Notify(It.IsAny<WeatherOutcome>())).Throws<Exception>().Verifiable();

            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(_restClientMockForCity.Object, _cityConfig),
                    new WeatherLoader(_restClientMockForWeather.Object, _weatherConfig)
                    ),
                new Publisher(_consoleMock.Object)
                );
            

            Assert.Throws<Exception>(() => _sut.Run());

            _restClientMockForCity.Verify();
            _restClientMockForWeather.Verify();
            _consoleMock.Verify();
        }
    }
}