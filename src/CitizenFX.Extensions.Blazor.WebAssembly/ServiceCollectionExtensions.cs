using CitizenFX.Extensions.Blazor.WebAssembly.Internal;
using CitizenFX.Extensions.Blazor.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CitizenFX.Extensions.Blazor.WebAssembly;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNuiServices(this IServiceCollection services, Action<INuiBuilder> configure)
    {
        configure(Nui.Options);
        
        services.TryAddSingleton<INuiCallbackService, NuiCallbackService>();
        
        return services;
    }
}
