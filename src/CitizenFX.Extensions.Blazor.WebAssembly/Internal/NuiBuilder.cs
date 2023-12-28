using System.Text.Json;

namespace CitizenFX.Extensions.Blazor.WebAssembly.Internal;

internal sealed class NuiBuilder : INuiBuilder
{
    public JsonSerializerOptions JsonSerializerOptions { get; } = new();
        
    public string MessageHandlerIdentifierField { get; set; } = "type";
}
