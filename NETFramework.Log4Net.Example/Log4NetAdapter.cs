using System;
using System.Reflection;
using log4net;
using log4net.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NETFramework.Log4Net.Example
{
    /// <summary>
    /// Adapter for NLog logging library.
    /// </summary>
    public class Log4NetAdapter : ILogger
    {
        private readonly ILog logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetAdapter"/> class.
        /// </summary>
        /// <param name="loggerName">Name of the logger.</param>
        public Log4NetAdapter(string loggerName)
        {
            this.logger = LogManager.GetLogger(loggerName);
        }

        #region Implementation of ILogger

        /// <summary>
        /// Determines whether the logging level is enabled.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <returns>The <see cref="bool"/> value indicating whether the logging level is enabled.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Throws when <paramref name="logLevel"/> is outside allowed range</exception>
        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    {
                        return this.logger.IsFatalEnabled;
                    }

                case LogLevel.Debug:
                case LogLevel.Trace:
                    {
                        return this.logger.IsDebugEnabled;
                    }

                case LogLevel.Error:
                    {
                        return this.logger.IsErrorEnabled;
                    }

                case LogLevel.Information:
                    {
                        return this.logger.IsInfoEnabled;
                    }
                case LogLevel.Warning:
                    {
                        return this.logger.IsWarnEnabled;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(logLevel));
                    }
            }
        }

        /// <summary>
        /// Logs an exception into the log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="eventId">The event Id.</param>
        /// <param name="state">The state.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="formatter">The formatter.</param>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <exception cref="ArgumentNullException">Throws when the <paramref name="formatter"/> is null.</exception>
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter
        )
        {
            if (!this.IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = formatter(state, exception);

            if (
                !string.IsNullOrWhiteSpace(message)
                || exception != null
            )
            {
                switch (logLevel)
                {
                    case LogLevel.Critical:
                        {
                            this.logger.Fatal(message, exception);

                            break;
                        }

                    case LogLevel.Debug:
                        {
                            this.logger.Debug(message, exception);

                            break;
                        }

                    case LogLevel.Error:
                        {
                            this.logger.Error(message, exception);

                            break;
                        }

                    case LogLevel.Information:
                        {
                            this.logger.Info(message, exception);

                            break;
                        }

                    case LogLevel.Warning:
                        {
                            this.logger.Warn(message, exception);

                            break;
                        }

                    case LogLevel.Trace:
                        {
                            this.logger.Logger
                                .Log(
                                    MethodBase.GetCurrentMethod().DeclaringType,
                                    Level.Trace,
                                    message,
                                    exception
                                );

                            break;
                        }

                    default:
                        {
                            this.logger.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                            this.logger.Info(message, exception);

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>
        /// An IDisposable that ends the logical operation scope on dispose.
        /// </returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        #endregion
    }
}
