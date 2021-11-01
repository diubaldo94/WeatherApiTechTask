namespace WeatherApiTechTask
{ 
    public class WeatherLoadConfiguration
    {
        public string Url { get; set; }
        public WeatherParams ParamNames { get; set; }
        public string ApiKey { get; set; }
        public string Days { get; set; }
    }

    public class WeatherParams
    {
        public string Position { get; set; }
        public string ApiKey { get; set; }
        public string Days { get; set; }
    }
}