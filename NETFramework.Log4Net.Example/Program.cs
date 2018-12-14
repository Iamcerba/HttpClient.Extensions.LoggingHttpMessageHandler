using System.Net.Http;
using System.Net.Http.LoggingHttpMessageHandler;

namespace NETFramework.Log4Net.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var messageHandler = new LoggingHttpMessageHandler(new Log4NetAdapter(typeof(HttpClient).FullName))
            {
                EnableContentLogging = true
            };

            using (var httpClient = new HttpClient(messageHandler))
            {
                // omitted for brevity
            }
        }
    }
}
