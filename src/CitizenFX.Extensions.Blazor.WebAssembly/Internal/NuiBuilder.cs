using System.Text.Json;

namespace CitizenFX.Extensions.Blazor.WebAssembly.Internal;

internal sealed class NuiBuilder : INuiBuilder
{
    /// <inheritdoc />
    public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    
    /// <inheritdoc />
    public string MessageHandlerIdentifierField { get; set; } = "type";
}
