using System;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace SeqLogger.Services
{
    internal class LoggerService : ILoggerService
    {
        private readonly ILogger<LoggerService> _logger;
        private readonly string _microserviceName;
        private readonly string _errorLogPath;

        /// <summary>
        /// Конструктор сервиса логирования
        /// </summary>
        /// <param name="logger">Базовый логгер</param>
        /// <param name="microserviceName">Имя микросервиса</param>
        public LoggerService(ILogger<LoggerService> logger, string microserviceName)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _microserviceName = microserviceName ?? throw new ArgumentNullException(nameof(microserviceName));

            // Создаем путь к файлу ошибок
            _errorLogPath = $"{_microserviceName}_log-error.txt";
        }

        /// <summary>
        /// Запись сообщения в лог с указанным уровнем
        /// </summary>
        private void Log<TService>(LogLevel logLevel, string message, object[] args, Exception exception = null)
        {
            var serviceName = typeof(TService).Name;

            using (LogContext.PushProperty("Microservice", _microserviceName))
            using (LogContext.PushProperty("Service", serviceName))
            {
                if (exception != null)
                {
                    _logger.Log(logLevel, exception, message, args);
                }
                else
                {
                    _logger.Log(logLevel, message, args);
                }
            }

            // Дополнительная запись ошибок в файл
            if (logLevel == LogLevel.Error)
            {
                WriteErrorToFile(serviceName, message, exception);
            }
        }

        /// <summary>
        /// Запись ошибки в текстовый файл
        /// </summary>
        private void WriteErrorToFile(string serviceName, string message, Exception exception)
        {
            try
            {
                var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [ERROR] [Microservice: {_microserviceName}] [Service: {serviceName}]\n";
                logEntry += $"Message: {message}\n";

                if (exception != null)
                {
                    logEntry += $"Exception: {exception.GetType().Name}\n";
                    logEntry += $"Message: {exception.Message}\n";
                    logEntry += $"Stack Trace:\n{exception.StackTrace}\n";
                }

                logEntry += new string('-', 80) + "\n\n";

                System.IO.File.AppendAllText(_errorLogPath, logEntry);
            }
            catch (Exception ex)
            {
                // Если не удалось записать в файл, логируем в основной логгер
                _logger.LogError(ex, "Ошибка при записи в файл логов");
            }
        }

        public void LogDebug<TService>(string message, params object[] args)
            => Log<TService>(LogLevel.Debug, message, args);

        public void LogInformation<TService>(string message, params object[] args)
            => Log<TService>(LogLevel.Information, message, args);

        public void LogWarning<TService>(string message, params object[] args)
            => Log<TService>(LogLevel.Warning, message, args);

        public void LogError<TService>(string message, params object[] args)
            => Log<TService>(LogLevel.Error, message, args);

        public void LogError<TService>(Exception exception, string message, params object[] args)
            => Log<TService>(LogLevel.Error, message, args, exception);
    }
}
