using HttpClient.Extensions.LoggingHttpMessageHandler;

namespace NETFramework.Log4Net.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var messageHandler = new LoggingHttpMessageHandler(new Log4NetAdapter(typeof(System.Net.Http.HttpClient).FullName))
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
