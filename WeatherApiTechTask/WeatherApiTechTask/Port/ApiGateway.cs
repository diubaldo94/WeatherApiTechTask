using System.Collections.Generic;
using RestSharp;
using System;

namespace WeatherApiTechTask
{
    internal class ApiGateway : IRestClient
    {
        private const string BodyName = @"application/json; charset=utf-8";

        public ApiGateway()
        {
        }

        public T Get<T>(string url, Dictionary<string, string> parameterDictionary = null, Dictionary<string, string> headerDictionary = null)
        {
            var client = new RestClient(url);
            client.UseJson();
            client.UseSerializer<CustomJsonSerializer>();

            var request = GetRequest(url, parameterDictionary, headerDictionary);

            var response = client.Execute<T>(request);

            if (response.IsSuccessful || response.Data != null)
                return response.Data;

            throw response.ErrorException ?? new Exception($"Failed to get data from api {url}");
        }

        private static RestRequest GetRequest(string url,
                                                Dictionary<string, string> parameterDictionary = null,
                                                Dictionary<string, string> headerDictionary = null)
        {
            var request = new RestRequest(url, Method.GET) { RequestFormat = DataFormat.Json };

            if (parameterDictionary != null)
                foreach (var parTuple in parameterDictionary)
                    request.AddParameter(parTuple.Key, parTuple.Value, ParameterType.QueryString);

            if (headerDictionary != null)
                foreach (var headerTuple in headerDictionary)
                    request.AddHeader(headerTuple.Key, headerTuple.Value);

            return request;
        }
    }
}