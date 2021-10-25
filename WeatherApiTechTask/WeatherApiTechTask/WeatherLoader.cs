namespace WeatherApiTechTask
{
    internal class WeatherLoader
    {
        private IRestClient @object;
        private WeatherLoadConfiguration weatherConfig;

        public WeatherLoader(IRestClient @object, WeatherLoadConfiguration weatherConfig)
        {
            this.@object = @object;
            this.weatherConfig = weatherConfig;
        }
    }
}