using Moq;
using NUnit.Framework;

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

        private readonly CityLoader.Configuration _cityConfig = new CityLoader.Configuration();
        private readonly WeatherLoader.Configuration _weatherConfig = new WeatherLoader.Configuration();


        //PROVA NETMOCK
        [SetUp]
        public void Setup()
        {          
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
            _restClientMockForCity.Setup(c => c.Get<CityInfoResponse>(_cityConfig.Url, new { }))
                .Returns(new CityInfoResponse()).Verifiable();
            _restClientMockForWeather.Setup(c => c.Get<WeatherInfoResponse>(_weatherConfig.Url, new { }))
                .Returns(new WeatherInfoResponse()).Verifiable();

            _consoleMock.Verify(c => c.Notify(new WeatherOutcome(new { "", "" })), Times.Once);

            _sut.Run();

            _restClientMockForCity.Verify();
            _restClientMockForWeather.Verify();
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