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
            var configuration = BuildConfiguration();
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

            var app = serviceCollection.BuildServiceProvider().GetService<IWeatherApp>();
            //var app = new WeatherApp(
            //    new Loader(
            //        new CityLoader(new ApiGateway(), cityConfig),
            //        new WeatherLoader(new ApiGateway(), weatherConfig)
            //        ),
            //    new Publisher(new ConsoleNotifier())
            //    );
            app.Run();
        }

        static IConfiguration BuildConfiguration() =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
    }
}
