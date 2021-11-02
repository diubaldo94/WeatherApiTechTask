namespace WeatherApiTechTask
{
    internal class WeatherOutcome
    {
        public string Message { get; }

        public WeatherOutcome(string message)
        {
            Message = message;
        }
    }
}