using CitizenFX.Extensions.Blazor.WebAssembly.Internal;
using CitizenFX.Extensions.Blazor.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CitizenFX.Extensions.Blazor.WebAssembly;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Nui related services to the collection
    /// </summary>
    /// <param name="services">The extended service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddNuiServices(this IServiceCollection services)
    {
        services.TryAddSingleton<INuiCallbackService, NuiCallbackService>();
        
        return services;
    }
    
    /// <summary>
    /// Adds Nui related services to the collection
    /// </summary>
    /// <param name="services">The extended service collection</param>
    /// <param name="configure">Action to configure Nui-specific options <see cref="INuiBuilder"/></param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddNuiServices(this IServiceCollection services, Action<INuiBuilder> configure)
    {
        configure(Nui.Options);
        
        services.TryAddSingleton<INuiCallbackService, NuiCallbackService>();
        
        return services;
    }
}
