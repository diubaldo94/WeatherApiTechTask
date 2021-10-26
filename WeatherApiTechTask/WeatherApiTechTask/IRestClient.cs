using System.Collections.Generic;

namespace WeatherApiTechTask
{
    internal interface IRestClient
    {
        T Get<T>(string url, Dictionary<string, string> dictionary);

    }

    internal class RestClient : IRestClient
    {
        public T Get<T>(string url, Dictionary<string, string> dictionary)
        {
            throw new System.NotImplementedException();
        }
    }
}