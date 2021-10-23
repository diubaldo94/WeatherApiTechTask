using Moq;
using NUnit.Framework;

namespace WeatherApiTechTask.Test
{
    public class Tests
    {
        private IWeatherApp _app;
        //private IWeatherLoader _loader;
        //private IPublisher _publisher;
        //private IRestClient _restClient;

        [SetUp]
        public void Setup()
        {
            var consoleMock = new Mock<INotifier>();
            var restClientMockForCity = new Mock<IRestClient>();
            var restClientMockForWeather = new Mock<IRestClient>();

            _app = new WeatherApp(
                new Loader(
                    new CityLoader(restClientMockForCity.Object),
                    new WeatherLoader(restClientMockForWeather.Object)
                    ),
                new Publisher(consoleMock.Object)
                );
        }

        [Test]
        public void Test1()
        {
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
}