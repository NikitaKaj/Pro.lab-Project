using ProLab.Data;
using ProLab.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace ProLab.Api.Logging;

public class DatabaseLoggingBackgroundService : BackgroundService
{
  private readonly ILogger<DatabaseLoggingBackgroundService> _logger;
  private readonly ApplicationDbContext _dbContext;
  private readonly ConcurrentQueue<LogEntry> _logQueue;

  public DatabaseLoggingBackgroundService(ILogger<DatabaseLoggingBackgroundService> logger, IDbContextFactory<ApplicationDbContext> dbContextFactory, ConcurrentQueue<LogEntry> logQueue)
  {
    _logger = logger;
    _dbContext = dbContextFactory.CreateDbContext();
    _logQueue = logQueue;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      if (_logQueue.TryDequeue(out LogEntry logEntry))
      {
        _dbContext.LogEntries.Add(logEntry);
        await _dbContext.SaveChangesAsync();
      }
      else
      {
        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
      }
    }
  }
}
