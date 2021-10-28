namespace WeatherApiTechTask.Test
{
    public partial class WeatherAppTest
    {
        //            1.GET ALL CITIES FROM https://api.musement.com/api/v3/cities


        //            2.foreach city get
        //http://api.weatherapi.com/v1/forecast.json?key=%5Byour-key%5D&q=%5Blat%5D,%5Blong%5D&days=2
        //            (days meglio renderli parametrici)
        //            API KEY DA FREE PLAN
        //            LAT Long potrebbero non essere precisi non fa niente

        //            3.Processare i dati ricevuti(farlo in maniera scalabile):
        //                Console.writeline

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
}