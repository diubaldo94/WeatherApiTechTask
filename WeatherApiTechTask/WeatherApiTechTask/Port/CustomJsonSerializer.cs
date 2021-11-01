using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;
using System;

namespace WeatherApiTechTask
{
    internal class CustomJsonSerializer : IRestSerializer
    {
        public string Serialize(object obj) => JsonConvert.SerializeObject(obj);

        public string Serialize(Parameter bodyParameter) => JsonConvert.SerializeObject(bodyParameter.Value);

        public T Deserialize<T>(IRestResponse response) =>
            response.IsSuccessful ? JsonConvert.DeserializeObject<T>(response.Content) : throw new Exception($"Failed to get data from api");

        public string[] SupportedContentTypes { get; } = { "application/json", "text/json", "text/x-json", "text/javascript", "*+json" };

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;
    }
}