namespace WeatherApiTechTask
{
    internal class Publisher
    {
        private INotifier @object;

        public Publisher(INotifier @object)
        {
            this.@object = @object;
        }
    }
}