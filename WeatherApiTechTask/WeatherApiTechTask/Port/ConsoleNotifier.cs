using System;

namespace WeatherApiTechTask
{
    internal class ConsoleNotifier : INotifier
    {
        public void Notify(WeatherOutcome notifiable)
        {
            Console.WriteLine(notifiable.ExpectedOutcome);
        }
    }
}