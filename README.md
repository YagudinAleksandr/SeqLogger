# SeqLogger

## Описание
Сервис логирования для микросервисов. Поддерживает работу с SEQ и ведет журнал логов самого микросервиса

## Использование
### Регистрация в DI
``` csharp
using Microsoft.Extensions.DependencyInjection;
using SeqLoggerLibrary;

// Создаем коллекцию сервисов
var services = new ServiceCollection();

// Регистрируем логгер
services.AddSeqLogger(
    seqUrl: "http://localhost:5341",         // URL SEQ сервера
    microserviceName: "OrderService",        // Имя микросервиса
    minimumLevel: LogEventLevel.Debug        // Уровень логирования
);

// Регистрируем другие сервисы
services.AddScoped<OrderProcessor>();
services.AddScoped<PaymentService>();

var serviceProvider = services.BuildServiceProvider();
```

### Пример сервиса с логированием
```csharp
/// <summary>
/// Сервис обработки заказов
/// </summary>
public class OrderProcessor
{
    private readonly ILoggerService _logger;
    private readonly PaymentService _paymentService;

    public OrderProcessor(ILoggerService logger, PaymentService paymentService)
    {
        _logger = logger;
        _paymentService = paymentService;
    }

    /// <summary>
    /// Обработать заказ
    /// </summary>
    public void ProcessOrder(Order order)
    {
        try
        {
            _logger.LogInformation<OrderProcessor>("Начало обработки заказа {OrderId}", order.Id);
            
            // Проверка заказа
            if (order.Items.Count == 0)
            {
                _logger.LogWarning<OrderProcessor>("Пустой заказ {OrderId}", order.Id);
                return;
            }

            // Обработка платежа
            _paymentService.ProcessPayment(order);

            _logger.LogInformation<OrderProcessor>("Заказ {OrderId} успешно обработан", order.Id);
        }
        catch (PaymentException ex)
        {
            _logger.LogError<OrderProcessor>(ex, "Ошибка оплаты заказа {OrderId}", order.Id);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError<OrderProcessor>(ex, "Критическая ошибка при обработке заказа {OrderId}", order.Id);
            throw;
        }
    }
}

/// <summary>
/// Сервис обработки платежей
/// </summary>
public class PaymentService
{
    private readonly ILoggerService _logger;

    public PaymentService(ILoggerService logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Обработать платеж
    /// </summary>
    public void ProcessPayment(Order order)
    {
        _logger.LogDebug<PaymentService>("Проверка баланса для заказа {OrderId}", order.Id);
        
        // Симуляция ошибки
        if (order.TotalAmount > 1000)
        {
            throw new PaymentException("Недостаточно средств на счете");
        }

        _logger.LogInformation<PaymentService>("Платеж для заказа {OrderId} успешно обработан", order.Id);
    }
}

/// <summary>
/// Пользовательское исключение для ошибок оплаты
/// </summary>
public class PaymentException : Exception
{
    public PaymentException(string message) : base(message) { }
}
```

## Особенности
Интеграция с SEQ:

Все логи отправляются в указанный сервер SEQ

Каждое сообщение содержит свойства:

`Microservice` - имя микросервиса

`Service` - имя класса сервиса


Запись ошибок в файл:

Ошибки (LogError) дополнительно записываются в файл

Формат имени файла: [MicroserviceName]_log-error.txt
```txt
[2023-10-15 14:30:25] [ERROR] [Microservice: OrderService] [Service: OrderProcessor]
Message: Ошибка оплаты заказа ORD-12345
Exception: PaymentException
Message: Недостаточно средств на счете
Stack Trace:
   at PaymentService.ProcessPayment(Order order) in ...
   at OrderProcessor.ProcessOrder(Order order) in ...
```

