namespace WeatherApiTechTask
{ 
    internal class WeatherLoadConfiguration
    {
        public string Url { get; set; }
        public WeatherParams ParamNames { get; set; }
        public string ApiKey { get; set; }
        public string Days { get; set; }
    }

    public class WeatherParams
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ApiKey { get; set; }
        public string Days { get; set; }
    }
}