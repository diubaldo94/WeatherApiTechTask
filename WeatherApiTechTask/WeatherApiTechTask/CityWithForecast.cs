namespace WeatherApiTechTask
{
    internal class Forecast
    {
        public Forecast(string today, string tomorrow)
        {
            Today = today;
            Tomorrow = tomorrow;
        }

        public string Today { get; }
        public string Tomorrow { get; }
    }
}