using ProLab.Api.Infrastructure.Interfaces;

namespace ProLab.Api.Infrastructure;

public static class IServiceScopeExtensions
{
    public static T GetService<T>(this IServiceScope scope) where T : class => scope.ServiceProvider.GetRequiredService<T>();
}