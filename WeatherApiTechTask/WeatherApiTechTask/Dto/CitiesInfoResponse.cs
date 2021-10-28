using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WeatherApiTechTask
{
    [JsonObject]
    internal class CitiesInfoResponse
    {
        public CitiesInfoResponse()
        {
        }

        [JsonProperty("cities")]
        public CityInfoResponse[] Cities { get; set; }
    }
}