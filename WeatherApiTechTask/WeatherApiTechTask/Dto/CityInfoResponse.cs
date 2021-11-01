using Newtonsoft.Json;

namespace WeatherApiTechTask
{
    [JsonObject]
    public class CitiesInfoResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }

    [JsonArray]
    public class CityInfoResponse
    {        
        public CitiesInfoResponse[] Cities { get; set; }
    }
}