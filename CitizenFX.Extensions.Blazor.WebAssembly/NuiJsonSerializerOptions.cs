using System.Text.Json;

namespace CitizenFX.Extensions.Blazor.WebAssembly;

/// <summary>
/// The internal <see cref="JsonSerializerOptions"/>
/// </summary>
internal static class NuiJsonSerializerOptions
{
    /// <summary>
    /// Configured in `<see cref="ServiceCollectionExtensions.AddNuiServices"/>
    /// </summary>
    internal static JsonSerializerOptions Options { get; } = new();
}
