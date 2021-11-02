using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WeatherApiTechTask.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace WeatherApiTechTask
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var configuration = BuildConfiguration();
                var serviceCollection = GetServices(configuration);

                var app = serviceCollection.BuildServiceProvider().GetService<IWeatherApp>();
                app.Run();
            }
            catch(Exception e)
            {
                ManageException(e);
            }
        }

        private static void ManageException(Exception e)
        {
            Console.Error.WriteLine($"Error : {e.Message}");
        }

        private static ServiceCollection GetServices(IConfiguration configuration)
        {
            var cityConfig = configuration.GetSection(typeof(CityLoadConfiguration).Name).Get<CityLoadConfiguration>();
            var weatherConfig = configuration.GetSection(typeof(WeatherLoadConfiguration).Name).Get<WeatherLoadConfiguration>();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IRestClient, ApiGateway>();
            serviceCollection.AddSingleton<INotifier, ConsoleNotifier>();
            serviceCollection.AddSingleton<IPublisher, Publisher>();
            serviceCollection.AddSingleton<ILoader<IEnumerable<BaseCityModel>>, CityLoader>();
            serviceCollection.AddSingleton<ILoader<IEnumerable<CityModel>>, Loader>();
            serviceCollection.AddSingleton<IEnricher<BaseCityModel, Forecast>, WeatherLoader>();
            serviceCollection.AddSingleton(cityConfig);
            serviceCollection.AddSingleton(weatherConfig);
            serviceCollection.AddSingleton<IWeatherApp, WeatherApp>();
            return serviceCollection;
        }

        static IConfiguration BuildConfiguration() =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
    }
}
