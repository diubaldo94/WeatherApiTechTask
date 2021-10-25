using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using WeatherApiTechTask;

namespace WeatherApiTechTask.Test
{
    public class WeatherAppTest
    {
        private IWeatherApp _sut;
        //private IWeatherLoader _loader;
        //private IPublisher _publisher;
        //private IRestClient _restClient;
        private readonly Mock<INotifier> _consoleMock = new();
        private readonly Mock<IRestClient> _restClientMockForCity = new();
        private readonly Mock<IRestClient> _restClientMockForWeather = new();

        private readonly CityLoadConfiguration _cityConfig = new CityLoadConfiguration();
        private readonly WeatherLoadConfiguration _weatherConfig = new WeatherLoadConfiguration();

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
            var citiesInfoResponse = new CitiesInfoResponse() { Cities = new CityInfoResponse[_testData.Length] };
            var cities = new List<CityInfoResponse>();
            foreach (var testData in _testData)
            {
                cities.Add(new CityInfoResponse 
                    { Name = testData.Name, Latitude = testData.Latitude, Longitude = testData.Longitude });
                _restClientMockForWeather.Setup(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url, new Dictionary<string, string> {
                        //todo vedi meglio gestione double 
                            { _weatherConfig.ParamNames.Latitude, testData.Latitude.ToString() }, 
                            { _weatherConfig.ParamNames.Longitude, testData.Longitude.ToString() },
                            { _weatherConfig.ParamNames.ApiKey, _weatherConfig.ApiKey }, { _weatherConfig.ParamNames.Days, _weatherConfig.Days },
                        }))
                    .Returns(new WeatherInfoResponse()
                    {
                        Forecast = new ForecastDay[] { 
                            new ForecastDay() { Day = new Day() { Condition = new Condition() { Text = testData.WeatherToday} } },
                            new ForecastDay() { Day = new Day() { Condition = new Condition() { Text = testData.WeatherTomorrow} } },
                        }
                    }).Verifiable();
            }
            _restClientMockForCity.Setup(c => c.Get<CitiesInfoResponse>(_cityConfig.Url, null))
                .Returns(citiesInfoResponse).Verifiable();
           
            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(_restClientMockForCity.Object, _cityConfig),
                    new WeatherLoader(_restClientMockForWeather.Object, _weatherConfig)
                    ),
                new Publisher(_consoleMock.Object)
                );
        }

        [Test] //Chek order of the send of messgaes
        public void LoadSomeCitiesWithWeatherAndNotifyThem()
        {
            _sut.Run();

            _restClientMockForCity.Verify();
            _restClientMockForWeather.Verify();
            foreach(var row in _testData)
                _consoleMock.Verify(c => c.Notify(new WeatherOutcome(row.ExpectedOutcome)), Times.Once);
        }

        [Test]
        public void ThrowExceptionIfErrorOnLoadingCities()
        {
            _restClientMockForCity.Setup(c => c.Get<CityInfoResponse>(_cityConfig.Url, null))
                .Throws<Exception>().Verifiable(); //which exception?

            Assert.Throws<Exception>(() => _sut.Run());

            _restClientMockForCity.Verify(); 
            _restClientMockForWeather.Verify(c => c.Get<CityInfoResponse>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
            _consoleMock.Verify(c => c.Notify(It.IsAny<WeatherOutcome>()), Times.Never);
        }

        [Test]
        public void ThrowExceptionIfErrorOnLoadingWeather()
        {
            _restClientMockForWeather.Setup(c => c.Get<WeatherInfoResponse>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Throws<Exception>().Verifiable(); //which exception?

            Assert.Throws<Exception>(() => _sut.Run());

            _restClientMockForCity.Verify();
            _restClientMockForWeather.Verify();
            _consoleMock.Verify(c => c.Notify(It.IsAny<WeatherOutcome>()), Times.Never);
        }

        [Test]
        public void ThrowExceptionIfErrorOnNotifying()
        {
            _consoleMock.Setup(c => c.Notify(It.IsAny<WeatherOutcome>())).Throws<Exception>().Verifiable();

            Assert.Throws<Exception>(() => _sut.Run());

            _restClientMockForCity.Verify();
            _restClientMockForWeather.Verify();
            _consoleMock.Verify();
        }



        //            1.GET ALL CITIES FROM https://api.musement.com/api/v3/cities


        //            2.foreach city get
        //http://api.weatherapi.com/v1/forecast.json?key=%5Byour-key%5D&q=%5Blat%5D,%5Blong%5D&days=2
        //            (days meglio renderli parametrici)
        //            API KEY DA FREE PLAN
        //            LAT Long potrebbero non essere precisi non fa niente

        //            3.Processare i dati ricevuti(farlo in maniera scalabile):
        //                Console.writeline

        internal class CityTestInputObj
        {
            internal string Name { get; }
            internal double Latitude { get; }
            internal double Longitude { get; }
            internal string WeatherToday { get; }
            internal string WeatherTomorrow { get; }
            internal string ExpectedOutcome { get; }

            public CityTestInputObj(string name, double latitude, double longitude, string weatherToday, string weatherTomorrow, string expectedOutcome)
            {
                Name = name;
                Latitude = latitude;
                Longitude = longitude;
                WeatherToday = weatherToday;
                WeatherTomorrow = weatherTomorrow;
                ExpectedOutcome = expectedOutcome;
            }

        }
    }
}