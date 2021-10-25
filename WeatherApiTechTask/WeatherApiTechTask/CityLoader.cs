namespace WeatherApiTechTask
{
    internal class CityLoader
    {
        private IRestClient @object;
        private CityLoadConfiguration cityConfig;

        public CityLoader(IRestClient @object, CityLoadConfiguration cityConfig)
        {
            this.@object = @object;
            this.cityConfig = cityConfig;
        }
    }
}