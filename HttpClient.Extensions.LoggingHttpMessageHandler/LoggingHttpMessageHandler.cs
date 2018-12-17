using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HttpClient.Extensions.LoggingHttpMessageHandler
{
    public class LoggingHttpMessageHandler : DelegatingHandler
    {
        private ILogger logger;

        public LoggingHttpMessageHandler(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.logger = logger;
        }

        public bool EnableContentLogging { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var stopwatch = new Stopwatch();

            Guid requestId = Guid.NewGuid();

            await LogHttpRequest(requestId, request);

            stopwatch.Start();

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            stopwatch.Stop();

            await LogHttpResponse(requestId, response, stopwatch.Elapsed);

            return response;
        }

        private async Task LogHttpRequest(Guid requestId, HttpRequestMessage request)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine($"Sending HTTP request (Id: {requestId}) {request.Method} {request.RequestUri}");

            if (logger.IsEnabled(LogLevel.Trace))
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
                {
                    foreach (string value in header.Value)
                    {
                        message.AppendLine($"{header.Key}: {value}");
                    }
                }

                if (request.Content != null)
                {
                    foreach (KeyValuePair<string, IEnumerable<string>> header in request.Content.Headers)
                    {
                        foreach (string value in header.Value)
                        {
                            message.AppendLine($"{header.Key}: {value}");
                        }
                    }

                    if (this.EnableContentLogging)
                    {
                        string content = await request.Content.ReadAsStringAsync();

                        message.AppendLine(content);
                    }
                }
            }

            logger.Log(
                LogLevel.Information,
                new EventId(100, "RequestStart"),
                message,
                null,
                (state, ex) => state.ToString()
            );
        }

        private async Task LogHttpResponse(Guid requestId, HttpResponseMessage response, TimeSpan duration)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine($"Received HTTP response (Id: {requestId}) after {duration.TotalMilliseconds}ms - {response.StatusCode} ({(int)response.StatusCode})");

            if (logger.IsEnabled(LogLevel.Trace))
            {
                foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
                {
                    foreach (string value in header.Value)
                    {
                        message.AppendLine($"{header.Key}: {value}");
                    }
                }

                if (response.Content != null)
                {
                    foreach (KeyValuePair<string, IEnumerable<string>> header in response.Content.Headers)
                    {
                        foreach (string value in header.Value)
                        {
                            message.AppendLine($"{header.Key}: {value}");
                        }
                    }

                    if (this.EnableContentLogging)
                    {
                        string content = await response.Content.ReadAsStringAsync();

                        message.AppendLine(content);
                    }
                }
            }

            logger.Log(
                LogLevel.Information,
                new EventId(101, "RequestEnd"),
                message,
                null,
                (state, ex) => state.ToString()
            );
        }
    }
}
