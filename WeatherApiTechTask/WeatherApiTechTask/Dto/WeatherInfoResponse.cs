using Newtonsoft.Json;

namespace WeatherApiTechTask
{
    [JsonObject]
    internal class WeatherInfoResponse
    {
        [JsonProperty("forecast")]
        public ForecastDay[] Forecast { get; internal set; }
    }

    [JsonObject]
    internal class ForecastDay
    {
        [JsonProperty("day")]
        public Day Day { get; internal set; }
    }

    [JsonObject]
    internal class Day
    {
        [JsonProperty("condition")]
        public Condition Condition { get; internal set; }
    }

    [JsonObject]
    internal class Condition
    {
        [JsonProperty("text")]
        public string Text { get; internal set; }
    }
}