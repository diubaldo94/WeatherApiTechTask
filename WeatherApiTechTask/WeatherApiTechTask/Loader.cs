namespace WeatherApiTechTask
{
    internal class Loader
    {
        private CityLoader cityLoader;
        private WeatherLoader weatherLoader;

        public Loader(CityLoader cityLoader, WeatherLoader weatherLoader)
        {
            this.cityLoader = cityLoader;
            this.weatherLoader = weatherLoader;
        }
    }
}