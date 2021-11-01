using Newtonsoft.Json;

namespace WeatherApiTechTask
{
    [JsonObject]
    public class CityInfoResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
}