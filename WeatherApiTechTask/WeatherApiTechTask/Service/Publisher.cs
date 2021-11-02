using System;
using System.Collections.Generic;

namespace WeatherApiTechTask
{
    internal class Publisher : IPublisher
    {
        private readonly INotifier _consoleNotifier;

        public Publisher(INotifier consoleNotifier)
        {
            _consoleNotifier = consoleNotifier;
        }

        private Dictionary<Func<CityModel, WeatherOutcome>, INotifier> NotifierList() => new() {
            { m => new WeatherOutcome($"Processed city {m.Name} | {m.Forecast.Today} - {m.Forecast.Tomorrow}"),
            _consoleNotifier }
        };

        public void Publish(CityModel unit)
        {
            foreach(var notifier in NotifierList())
            {
                notifier.Value.Notify(notifier.Key(unit));
            }
        }
    }

    internal interface IPublisher
    {
        void Publish(CityModel unit);
    }
}