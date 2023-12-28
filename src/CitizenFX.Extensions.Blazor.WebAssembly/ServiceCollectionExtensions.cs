using System.Text.Json;
using CitizenFX.Extensions.Blazor.WebAssembly.Internal;
using CitizenFX.Extensions.Blazor.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CitizenFX.Extensions.Blazor.WebAssembly;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Nui related services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureNuiJsonSerializerOptions">Action to configure the <see cref="NuiJsonSerializerOptions"/> used in <see cref="NuiCallbackService"/> and <see cref="NuiMessageListener"/> for serializing & deserializing JSON</param>
    public static IServiceCollection AddNuiServices(
        this IServiceCollection services, 
        Action<JsonSerializerOptions>? configureNuiJsonSerializerOptions = null)
    {
        configureNuiJsonSerializerOptions?.Invoke(NuiJsonSerializerOptions.Options);
        
        services.TryAddSingleton<INuiCallbackService, NuiCallbackService>();
        
        services.AddNuiServices(options =>
        {
            options.JsonSerializerOptions.MaxDepth = 0;
            options.MessageHandlerIdentifierField = "type";
        });
        
        return services;
    }
    
    public static IServiceCollection AddNuiServices(this IServiceCollection services, Action<INuiBuilder> configure)
    {
        configure(Nui.Builder);
        
        services.TryAddSingleton<INuiCallbackService, NuiCallbackService>();
        
        return services;
    }
}
