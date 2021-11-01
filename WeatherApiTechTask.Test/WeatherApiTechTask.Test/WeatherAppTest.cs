using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WeatherApiTechTask;

namespace WeatherApiTechTask.Test
{
    public class WeatherAppTest
    {
        private IWeatherApp _sut;
              
        [Test] //Chek order of the send of messgaes
        public void LoadSomeCitiesWithWeatherAndNotifyThem()
        {
            Mock<INotifier> consoleMock = new();
            Mock<IRestClient> restClientMockForCity = new();
            Mock<IRestClient> restClientMockForWeather = new();

            WeatherTestUtils.SetupRestClientCity(restClientMockForCity);
            WeatherTestUtils.SetupRestClientWeather(restClientMockForWeather);

            //build sut
            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(restClientMockForCity.Object, WeatherTestUtils.CityConfig),
                    new WeatherLoader(restClientMockForWeather.Object, WeatherTestUtils.WeatherConfig)
                    ),
                new Publisher(consoleMock.Object)
                );

            _sut.Run();

            restClientMockForCity.Verify();
            restClientMockForWeather.Verify();
            consoleMock.Verify(
                c => c.Notify(It.Is<WeatherOutcome>(e => WeatherTestUtils.TestData.Select(i => i.ExpectedOutcome).Contains(e.ExpectedOutcome))), 
                Times.Exactly(WeatherTestUtils.TestData.Length)); //problem: exatcly one for each elemtn??
        }

        [Test]
        public void ThrowExceptionIfErrorOnLoadingCities()
        {
            Mock<INotifier> consoleMock = new();
            Mock<IRestClient> restClientMockForCity = new();
            Mock<IRestClient> restClientMockForWeather = new();

            WeatherTestUtils.SetupRestClientCity(restClientMockForCity, true);
            WeatherTestUtils.SetupRestClientWeather(restClientMockForWeather);

            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(restClientMockForCity.Object, WeatherTestUtils.CityConfig),
                    new WeatherLoader(restClientMockForWeather.Object, WeatherTestUtils.WeatherConfig)
                    ),
                new Publisher(consoleMock.Object)
                );

            Assert.Throws<Exception>(() => _sut.Run());

            restClientMockForCity.Verify(); 
            restClientMockForWeather.Verify(c => c.Get<CityInfoResponse>(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), null), Times.Never);
            consoleMock.Verify(c => c.Notify(It.IsAny<WeatherOutcome>()), Times.Never);
        }

        [Test]
        public void ThrowExceptionIfErrorOnLoadingWeather()
        {
            Mock<INotifier> consoleMock = new();
            Mock<IRestClient> restClientMockForCity = new();
            Mock<IRestClient> restClientMockForWeather = new();
            WeatherTestUtils.SetupRestClientCity(restClientMockForCity);
            WeatherTestUtils.SetupRestClientWeather(restClientMockForWeather, true);

            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(restClientMockForCity.Object, WeatherTestUtils.CityConfig),
                    new WeatherLoader(restClientMockForWeather.Object, WeatherTestUtils.WeatherConfig)
                    ),
                new Publisher(consoleMock.Object)
                );

            Assert.Throws<Exception>(() => _sut.Run());

            restClientMockForCity.Verify();
            restClientMockForWeather.Verify(c => c.Get<WeatherInfoResponse>(WeatherTestUtils.WeatherConfig.Url, It.IsAny<Dictionary<string,string>>(), null), Times.Once);
            consoleMock.Verify(c => c.Notify(It.IsAny<WeatherOutcome>()), Times.Never);
        }

        [Test]
        public void ThrowExceptionIfErrorOnNotifying()
        {
            Mock<INotifier> consoleMock = new();
            Mock<IRestClient> restClientMockForCity = new();
            Mock<IRestClient> restClientMockForWeather = new();
            WeatherTestUtils.SetupRestClientCity(restClientMockForCity);
            WeatherTestUtils.SetupRestClientWeather(restClientMockForWeather);
            consoleMock.Setup(c => c.Notify(It.IsAny<WeatherOutcome>())).Throws<Exception>().Verifiable();

            _sut = new WeatherApp(
                new Loader(
                    new CityLoader(restClientMockForCity.Object, WeatherTestUtils.CityConfig),
                    new WeatherLoader(restClientMockForWeather.Object, WeatherTestUtils.WeatherConfig)
                    ),
                new Publisher(consoleMock.Object)
                );            

            Assert.Throws<Exception>(() => _sut.Run());

            restClientMockForCity.Verify();
            restClientMockForWeather.Verify();
            consoleMock.Verify();
        }
    }
}