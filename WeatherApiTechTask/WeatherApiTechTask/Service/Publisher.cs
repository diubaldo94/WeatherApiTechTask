using System;

namespace WeatherApiTechTask
{
    internal class Publisher : IPublisher
    {
        private INotifier _notifier;

        public Publisher(INotifier notifier)
        {
            _notifier = notifier;
        }

        public void Publish(CityModel unit)
        {
            string expectedOutcome = unit.Format();
            _notifier.Notify(new WeatherOutcome(expectedOutcome));
        }
    }

    internal interface IPublisher
    {
        void Publish(CityModel unit);
    }
}