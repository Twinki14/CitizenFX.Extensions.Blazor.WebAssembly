using System.Text.Json;

namespace CitizenFX.Extensions.Blazor.WebAssembly;

public interface INuiBuilder
{
    /// <summary>
    /// The <see cref="System.Text.Json.JsonSerializerOptions"/> used by <see cref="NuiMessageHandler"/> and <see cref="Services.NuiCallbackService"/> for JSON serialization/deserialization
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; }
    
    /// <summary>
    /// The identifier field for <see cref="NuiMessageHandler"/> to determine which <see cref="NuiMessageHandler"/> to trigger by <see cref="NuiMessageListener"/>
    /// </summary>
    public string MessageHandlerIdentifierField { get; set;  }
}
