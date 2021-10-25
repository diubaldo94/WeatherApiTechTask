namespace WeatherApiTechTask
{
    internal interface INotifier
    {
        void Notify(WeatherOutcome weatherOutcome);
    }
}