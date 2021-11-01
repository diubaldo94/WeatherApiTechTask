namespace WeatherApiTechTask.Test
{
    internal class CityTestInputObj
    {
        internal string Name { get; }
        internal double Latitude { get; }
        internal double Longitude { get; }
        internal string WeatherToday { get; }
        internal string WeatherTomorrow { get; }
        internal string ExpectedOutcome { get; }

        public CityTestInputObj(string name, double latitude, double longitude, string weatherToday, string weatherTomorrow, string expectedOutcome)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
            WeatherToday = weatherToday;
            WeatherTomorrow = weatherTomorrow;
            ExpectedOutcome = expectedOutcome;
        }
    }    
}