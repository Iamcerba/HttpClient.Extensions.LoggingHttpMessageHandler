# HttpClient.Extensions.LoggingHttpMessageHandler
Provides a message handler for HttpClient which allows to enable logging of request/response related info. In comparison with Microsoft.Extensions.Http.Logging.LoggingHttpMessageHandler it allows to log request/response content.

## Installation
You can build from the source here, or you can install the Nuget version:

Install-Package HttpClient.Extensions.LoggingHttpMessageHandler

## Usage

```csharp
var messageHandler = new LoggingHttpMessageHandler(new Log4NetAdapter(typeof(HttpClient).FullName))
{
	EnableContentLogging = true
};

using (var httpClient = new HttpClient(messageHandler))
{
    // omitted for brevity
}
```

## License

Licensed under MIT. License included.
