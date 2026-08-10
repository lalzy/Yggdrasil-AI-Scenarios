// ServiceExtensions.cs
using Yggdrasil.Services;
using System.Reflection;

namespace Yggdrasil.Extensions;

/// <summary>Register the services automatically that exist in the Yggdrasil.Services namespace</summary>
public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        var serviceTypes = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract
            && t.Namespace == "Yggdrasil.Services"
            && !t.IsSubclassOf(typeof(DbContext))
            && !t.IsNested);

        foreach (var type in serviceTypes)
        {
            if (type == typeof(LLMSendService)) services.AddHttpClient<LLMSendService>();
            else services.AddScoped(type);
        }
    }
}
