﻿namespace WeatherApiTechTask
{ 
    internal class WeatherLoadConfiguration
    {
        public string Url { get; internal set; }
        public WeatherParams ParamNames { get; set; }
        public string ApiKey { get; internal set; }
        public string Days { get; internal set; }
    }

    public class WeatherParams
    {
        public string Position { get; internal set; }
        public string ApiKey { get; internal set; }
        public string Days { get; internal set; }
    }
}