using LoggingHandler = HttpClient.Extensions.LoggingHttpMessageHandler.LoggingHttpMessageHandler;

namespace LoggingHttpMessageHandler.Example.NET9
{
    class Program
    {
        static void Main(string[] args)
        {
            var messageHandler = new LoggingHandler(new Log4NetAdapter(typeof(System.Net.Http.HttpClient).FullName))
            {
                EnableContentLogging = true
            };

            using (var httpClient = new System.Net.Http.HttpClient(messageHandler))
            {
                // omitted for brevity
            }
        }
    }
}
