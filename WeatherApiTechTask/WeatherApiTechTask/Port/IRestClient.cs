using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace WeatherApiTechTask
{
    internal interface IRestClient
    {
        T Get<T>(string url, Dictionary<string, string> dictionary);

    }
}