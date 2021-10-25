using System.Collections.Generic;

namespace WeatherApiTechTask
{
    internal interface IRestClient
    {
        T Get<T>(string url, Dictionary<string, string> dictionary);

    }
}