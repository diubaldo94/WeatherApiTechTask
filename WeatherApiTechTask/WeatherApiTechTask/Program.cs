using Microsoft.Extensions.Configuration;
using System;
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
            var app = new WeatherApp(
                new Loader(
                    new CityLoader(new ApiGateway(), cityConfig),
                    new WeatherLoader(new ApiGateway(), weatherConfig)
                    ),
                new Publisher(new ConsoleNotifier())
                );
            app.Run();
        }

        static IConfiguration BuildConfiguration() =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
    }
}
