namespace WeatherApiTechTask
{
    internal class CityModel : BaseCityModel
    {
        public CityModel(string name, double latitude, double longitude, Forecast forecast) : base(name, latitude, longitude)
        {
            Forecast = forecast;
        }

        public Forecast Forecast { get; }

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