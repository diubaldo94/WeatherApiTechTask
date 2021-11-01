namespace WeatherApiTechTask
{
    internal interface IEnricher<T, TD>
    {
        TD Load(T input);
    }
}