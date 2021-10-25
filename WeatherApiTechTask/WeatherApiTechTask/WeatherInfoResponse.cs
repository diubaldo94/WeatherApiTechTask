namespace WeatherApiTechTask
{
    internal class WeatherInfoResponse
    {
        public ForecastDay[] Forecast { get; internal set; }
    }

    internal class ForecastDay
    {
        public Day Day { get; internal set; }
    }

    internal class Day
    {
        public Condition Condition { get; internal set; }
    }

    internal class Condition
    {
        public string Text { get; internal set; }
    }
}