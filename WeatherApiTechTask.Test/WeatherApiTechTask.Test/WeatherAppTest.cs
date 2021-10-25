using Moq;
using NUnit.Framework;
using System;

namespace WeatherApiTechTask.Test
{
    public class WeatherAppTest
    {
        private readonly IWeatherApp _sut;
        //private IWeatherLoader _loader;
        //private IPublisher _publisher;
        //private IRestClient _restClient;
        private readonly Mock<INotifier> _consoleMock = new Mock<INotifier>();
        private readonly Mock<IRestClient> _restClientMockForCity = new Mock<IRestClient>();
        private readonly Mock<IRestClient> _restClientMockForWeather = new Mock<IRestClient>();

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
            foreach (var testData in _testData)
            {
                citiesInfoResponse.Cities.Add(new CityInfoResponse 
                    { Name = testData.Name, Latitude = testData.Latitude, Longitude = testData.Longitude });
                _restClientMockForWeather.Setup(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url, new { }))
                    .Returns(new WeatherInfoResponse()).Verifiable();
            }
            _restClientMockForCity.Setup(c => c.Get<CityInfoResponse>(_cityConfig.Url, new { }))
                .Returns(citiesInfoResponse).Verifiable();
           
            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(_restClientMockForCity.Object, cityConfig),
                    new WeatherLoader(_restClientMockForWeather.Object, weatherConfig)
                    ),
                new Publisher(_consoleMock.Object)
                );
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LoadSomeCitiesWithWeatherAndNotifyThem()
        {
            _sut.Run();

            _restClientMockForCity.Verify();
            _restClientMockForWeather.Verify();
            _consoleMock.Verify(c => c.Notify(new WeatherOutcome(new { "", "" })), Times.Once);
        }

        [Test]
        public void ThrowExceptionIfErrorOnLoadingCities()
        {
            _restClientMockForCity.Setup(c => c.Get<CityInfoResponse>(_cityConfig.Url, new { }))
                .Throws().Verifiable(); //which exception?

            Assert.Throws(_ => _sut.Run());

            _restClientMockForCity.Verify(); 
            _restClientMockForWeather.Verify(c => c.Get<CityInfoResponse>(It.IsAny, It.IsAny), Times.Never);
            _consoleMock.Verify(c => c.Notify(It.IsAny), Times.Never);
        }

        [Test]
        public void ThrowExceptionIfErrorOnLoadingWeather()
        {
            _restClientMockForWeather.Setup(c => c.Get<WeatherInfoResponse>(It.IsAny, It.IsAny))
                .Throws().Verifiable(); //which exception?

            Assert.Throws(_ => _sut.Run());

            _restClientMockForCity.Verify();
            _restClientMockForWeather.Verify();
            _consoleMock.Verify(c => c.Notify(It.IsAny), Times.Never);
        }

        [Test]
        public void ThrowExceptionIfErrorOnNotifying()
        {
            _consoleMock.Setup(c => c.Notify(It.IsAny)).Throws().Verifiable();

            Assert.Throws(_ => _sut.Run());

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