using System;

namespace WeatherApiTechTask
{
    internal class Publisher
    {
        private INotifier _notifier;

        public Publisher(INotifier notifier)
        {
            _notifier = notifier;
        }

        internal void Publish(CityModel unit)
        {
            string expectedOutcome = unit.Format();
            _notifier.Notify(new WeatherOutcome(expectedOutcome));
        }
    }
}