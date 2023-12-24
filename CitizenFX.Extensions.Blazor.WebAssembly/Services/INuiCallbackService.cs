namespace CitizenFX.Extensions.Blazor.WebAssembly.Services;

/// <summary>
/// Service for triggering registered client-script Nui callbacks
/// </summary>
public interface INuiCallbackService
{
    /// <summary>
    /// Triggers a Nui callback using a POST request with a HttpClient.
    /// </summary>
    /// <param name="callback">The name of the callback to trigger, typically this is registered with 'RegisterNuiCallback'.</param>
    /// <param name="value">The content to send in the body, this is serialized into JSON before sending using <see cref="NuiJsonSerializerOptions"/> configured in <see cref="ServiceCollectionExtensions.AddNuiServices"/>.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The <see cref="HttpResponseMessage"/> of the POST request made to the resource.</returns>
    ValueTask<HttpResponseMessage> TriggerNuiCallbackAsync<T>(string callback, T value, CancellationToken cancellationToken = default);
}
