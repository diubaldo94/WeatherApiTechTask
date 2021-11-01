using Newtonsoft.Json;

namespace WeatherApiTechTask
{
    [JsonObject]
    public class WeatherInfoResponse
    {
        [JsonProperty("forecast")]
        public ForecastDto Forecast { get; set; }
    }

    [JsonObject]
    public class ForecastDto
    {
        [JsonProperty("forecastday")]
        public ForecastDayDto[] ForecastDays { get; set; }
    }

    [JsonObject]
    public class ForecastDayDto
    {
        [JsonProperty("day")]
        public DayDto Day { get; set; }
    }

    [JsonObject]
    public class DayDto
    {
        [JsonProperty("condition")]
        public ConditionDto Condition { get; set; }
    }

    [JsonObject]
    public class ConditionDto
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}