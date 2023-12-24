namespace CitizenFX.Extensions.Blazor.WebAssembly;

/// <summary>
/// Any method with this attribute will be considered when handling Nui Messages.
/// <see cref="NuiMessageListener"/> uses this attribute to determine which method the Nui Message belongs to, using the <see cref="Identifier"/> property added in the Nui Message.
/// </summary>
/// <example>
/// <code>
/// private readonly bool _showUi { get; set; }
/// private readonly string _message { get; set; }
/// 
/// [NuiMessageHandler("showui:message")]
/// public async Task OnShowUi(string message)
/// {
///     _showUi = true;
///     _message = message;
/// 
///     StateHasChanged();
/// }
/// </code>
/// </example>
/// <param name="type">The type or identifier of a Nui Message, typically sent with SendNuiMessage, this is used to determine which method the Nui Message should go to. For example, type = "showui:message"</param>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class NuiMessageHandler(string type) : Attribute
{
    public readonly string Type = type;
    public const string Identifier = "type";
}
