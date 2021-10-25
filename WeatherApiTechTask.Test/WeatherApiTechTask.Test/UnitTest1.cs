using Moq;
using NUnit.Framework;
using System;

namespace WeatherApiTechTask.Test
{
    public class Tests
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


        //PROVA NETMOCK
        [SetUp]
        public void Setup()
        {
            _restClientMockForCity.Setup(c => c.Get<CityInfoResponse>(_cityConfig.Url, new { }))
                .Returns(new CityInfoResponse()).Verifiable();
            _restClientMockForWeather.Setup(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url, new { }))
                .Returns(new WeatherInfoResponse()).Verifiable();


            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(_restClientMockForCity.Object, cityConfig),
                    new WeatherLoader(_restClientMockForWeather.Object, weatherConfig)
                    ),
                new Publisher(_consoleMock.Object)
                );
        }

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
    }
}