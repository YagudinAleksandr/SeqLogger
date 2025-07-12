using System;

namespace SeqLogger.Services
{
    /// <summary>
    /// Сервис логирования с поддержкой SEQ и записи ошибок в файл
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Запись отладочного сообщения
        /// </summary>
        /// <typeparam name="TService">Тип сервиса, где происходит логирование</typeparam>
        /// <param name="message">Текст сообщения</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogDebug<TService>(string message, params object[] args);

        /// <summary>
        /// Запись информационного сообщения
        /// </summary>
        /// <typeparam name="TService">Тип сервиса, где происходит логирование</typeparam>
        /// <param name="message">Текст сообщения</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogInformation<TService>(string message, params object[] args);

        /// <summary>
        /// Запись предупреждения
        /// </summary>
        /// <typeparam name="TService">Тип сервиса, где происходит логирование</typeparam>
        /// <param name="message">Текст сообщения</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogWarning<TService>(string message, params object[] args);

        /// <summary>
        /// Запись ошибки
        /// </summary>
        /// <typeparam name="TService">Тип сервиса, где происходит логирование</typeparam>
        /// <param name="message">Текст сообщения</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogError<TService>(string message, params object[] args);

        /// <summary>
        /// Запись ошибки с исключением
        /// </summary>
        /// <typeparam name="TService">Тип сервиса, где происходит логирование</typeparam>
        /// <param name="exception">Исключение</param>
        /// <param name="message">Текст сообщения</param>
        /// <param name="args">Аргументы для форматирования</param>
        void LogError<TService>(Exception exception, string message, params object[] args);
    }
}
