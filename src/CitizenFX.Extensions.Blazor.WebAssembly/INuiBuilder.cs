using System.Text.Json;

namespace CitizenFX.Extensions.Blazor.WebAssembly;

public interface INuiBuilder
{
    public JsonSerializerOptions JsonSerializerOptions { get; }
    
    public string MessageHandlerIdentifierField { get; set;  }
}
