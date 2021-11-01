namespace WeatherApiTechTask
{
    public class WeatherInfoResponse
    {
        public ForecastDto Forecast { get; set; }
    }

    public class ForecastDto
    {
        public ForecastDayDto[] ForecastDays { get; set; }
    }

    public class ForecastDayDto
    {
        public DayDto Day { get; set; }
    }

    public class DayDto
    {
        public ConditionDto Condition { get; set; }
    }

    public class ConditionDto
    {
        public string Text { get; set; }
    }
}