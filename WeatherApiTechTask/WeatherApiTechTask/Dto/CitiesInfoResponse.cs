using System.Text.Json.Serialization;

namespace WeatherApiTechTask
{
    internal class CitiesInfoResponse
    {
        public CitiesInfoResponse()
        {
        }

        public CityInfoResponse[] Cities { get; set; }
    }
}