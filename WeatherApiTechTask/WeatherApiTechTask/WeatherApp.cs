namespace WeatherApiTechTask
{
    internal class WeatherApp : IWeatherApp
    {
        private readonly Loader _loader;
        private readonly Publisher _publisher;

        public WeatherApp(Loader loader, Publisher publisher)
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
