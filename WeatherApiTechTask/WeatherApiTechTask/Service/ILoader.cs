using System.Collections.Generic;

namespace WeatherApiTechTask
{
    internal interface ILoader<T>
    {
        T Load();
    }
}