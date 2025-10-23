using ProLab.Data.Entities;
using System.Collections.Concurrent;

namespace ProLab.Api.Logging;

[ProviderAlias("Database")]
public class DatabaseLoggerProvider : ILoggerProvider//, ISupportExternalScope
{
  private readonly ConcurrentQueue<LogEntry> _logQueue;
  //private IExternalScopeProvider _scopeProvider = null;//NullExternalScopeProvider.Instance;

  public DatabaseLoggerProvider(ConcurrentQueue<LogEntry> logQueue)
  {
    _logQueue = logQueue;
  }

  public ILogger CreateLogger(string categoryName)
  {
    return new DatabaseLogger(categoryName, _logQueue);
  }

  public void Dispose() { }

  //public void SetScopeProvider(IExternalScopeProvider scopeProvider)
  //{
  //  _scopeProvider = scopeProvider;
  //}
}