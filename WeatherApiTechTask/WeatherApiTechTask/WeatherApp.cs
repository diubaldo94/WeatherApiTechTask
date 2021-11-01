using System.Collections.Generic;

namespace WeatherApiTechTask
{
    internal class WeatherApp : IWeatherApp
    {
        private readonly ILoader<IEnumerable<CityModel>> _loader;
        private readonly IPublisher _publisher;

        public WeatherApp(ILoader<IEnumerable<CityModel>> loader, IPublisher publisher)
        {
            _loader = loader;
            _publisher = publisher;
        }

        public void Run()
        {
            var data = _loader.Load();
            foreach (var unit in data)
                _publisher.Publish(unit);
        }
    }
}
