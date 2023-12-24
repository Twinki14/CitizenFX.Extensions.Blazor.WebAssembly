using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace CitizenFX.Extensions.Blazor.WebAssembly.Services;

/// <inheritdoc/>
public class NuiCallbackService(IJSRuntime jsRuntime) : INuiCallbackService
{
    private string? ResourceName { get; set; }
    private static readonly SemaphoreSlim ResourceNameSemaphore = new(1, 1);
    
    /// <inheritdoc/>
    public async ValueTask<HttpResponseMessage> TriggerNuiCallbackAsync<T>(string callback, T value, CancellationToken cancellationToken = default)
    {
        var resourceName = await GetResourceNameAsync(cancellationToken);

        using var httpClient = new HttpClient();
        {
            httpClient.BaseAddress = new Uri($"https://{resourceName}/");

            return await httpClient.PostAsJsonAsync($"{callback}", value, NuiJsonSerializerOptions.Options, cancellationToken);   
        }
    }

    /// <summary>
    /// Invokes 'eval' using the JS runtime to get the value of 'window.location.host', which represents the name of the resource.
    /// Uses a semaphore slim, so that we only ever need to invoke 'eval' once.
    /// </summary>
    private async ValueTask<string> GetResourceNameAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(ResourceName))
        {
            return ResourceName;
        }

        try
        {
            await ResourceNameSemaphore.WaitAsync(cancellationToken);

            ResourceName = await jsRuntime.InvokeAsync<string>("eval", cancellationToken, "window.location.host");
        }
        finally
        {
            ResourceNameSemaphore.Release();
        }
        
        return ResourceName;
    }
}
