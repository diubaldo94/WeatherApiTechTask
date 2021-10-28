namespace WeatherApiTechTask
{
    internal class CityModel : BaseCityModel
    {
        public CityModel(string name, double latitude, double longitude) : base(name, latitude, longitude)
        {
        }

        public Forecast Forecast { get; internal set; }

        public string Format() => $"Processed city {Name} | {Forecast.Today} - {Forecast.Tomorrow}";
    }

    internal class BaseCityModel
    {
        public BaseCityModel(string name, double latitude, double longitude)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Name { get; }
        public double Latitude { get; }
        public double Longitude { get; }
    }
}