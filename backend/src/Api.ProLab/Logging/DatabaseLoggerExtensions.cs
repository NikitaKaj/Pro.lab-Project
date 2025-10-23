using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

namespace ProLab.Api.Logging;

public static class DatabaseLoggerExtensions
{
  public static ILoggingBuilder AddDatabase(this ILoggingBuilder builder)
  {
    builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DatabaseLoggerProvider>());

    return builder;
  }
}
