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
            throw new System.NotImplementedException();
        }
    }
}
