using ProLab.Data.Entities;
using System.Collections.Concurrent;

namespace ProLab.Api.Logging;

public class DatabaseLogger : ILogger
{
  private readonly string _categoryName;
  private readonly ConcurrentQueue<LogEntry> _logQueue;

  public DatabaseLogger(string categoryName, ConcurrentQueue<LogEntry> logQueue)
  {
    _categoryName = categoryName;
    _logQueue = logQueue;
  }

  public IDisposable BeginScope<TState>(TState state) => default;

  public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Error;

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
  {
    if (!IsEnabled(logLevel))
    {
      return;
    }

    var logEntry = new LogEntry
    {
      Category = _categoryName,
      LogLevel = logLevel.ToString(),
      Message = formatter(state, exception),
      Timestamp = DateTimeOffset.UtcNow,
      StackTrace = exception?.StackTrace
    };

    _logQueue.Enqueue(logEntry);
  }
}
