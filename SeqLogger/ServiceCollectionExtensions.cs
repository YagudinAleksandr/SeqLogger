using Microsoft.Extensions.DependencyInjection;
using SeqLogger.Services;
using Serilog.Events;
using Serilog;
using Microsoft.Extensions.Logging;

namespace SeqLogger
{
    /// <summary>
    /// Методы расширения для регистрации логгера
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавляет и настраивает логгер с поддержкой SEQ и записи ошибок в файл
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <param name="seqUrl">URL сервера SEQ</param>
        /// <param name="microserviceName">Имя микросервиса</param>
        /// <param name="minimumLevel">Минимальный уровень логирования (по умолчанию Information)</param>
        /// <returns>Коллекция сервисов</returns>
        public static IServiceCollection AddSeqLogger(
            this IServiceCollection services,
            string seqUrl,
            string microserviceName,
            LogEventLevel minimumLevel = LogEventLevel.Information)
        {
            // Конфигурация Serilog
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .Enrich.FromLogContext()
                .WriteTo.Seq(seqUrl);

            // Создаем и сохраняем логгер
            Log.Logger = loggerConfiguration.CreateLogger();

            // Регистрируем сервисы логирования
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            // Регистрируем наш сервис логирования
            services.AddSingleton<ILoggerService>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<LoggerService>();
                return new LoggerService(logger, microserviceName);
            });

            return services;
        }
    }
}
