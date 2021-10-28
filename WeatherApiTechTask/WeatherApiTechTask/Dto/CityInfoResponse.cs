using Newtonsoft.Json;

namespace WeatherApiTechTask
{
    [JsonObject("city")]
    internal class CityInfoResponse
    {
        [JsonProperty("name")]
        public string Name { get; internal set; }
        [JsonProperty("latitude")]
        public double Latitude { get; internal set; }
        [JsonProperty("longitude")]
        public double Longitude { get; internal set; }
    }
}