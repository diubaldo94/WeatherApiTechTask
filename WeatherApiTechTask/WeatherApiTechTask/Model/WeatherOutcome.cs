namespace WeatherApiTechTask
{
    internal class WeatherOutcome
    {
        public string ExpectedOutcome { get; }

        public WeatherOutcome(string expectedOutcome)
        {
            ExpectedOutcome = expectedOutcome;
        }
    }
}