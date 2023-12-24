using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace CitizenFX.Extensions.Blazor.WebAssembly;

/// <summary>
/// A component that attaches a JavaScript event listener to the window, to listen and distribute Nui Messages to <see cref="NuiMessageHandler"/> attributed methods.
/// </summary>
/// <remarks>
/// Should only ever be included as a Root component in the <see cref="WebAssemblyHostBuilder"/>
/// </remarks>
/// <example>
/// <code>
/// builder.RootComponents.Add&lt;NuiMessageListener&gt;("#nui-message-listener");
/// </code>
/// </example>
public partial class NuiMessageListener : ComponentBase
{
    [Inject]
    private ILogger<NuiMessageListener> Logger { get; set; } = default!;

    private string? _assemblyName;
    private string? _nuiMessageMethod;

    private static NuiMessageListener? _instance;
    private static ILogger<NuiMessageListener>? _logger;

    protected override void OnInitialized()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException($"{nameof(NuiMessageListener)} should only ever be instantiated once as a Root component.");
        }

        _assemblyName = typeof(NuiMessageListener).Assembly.GetName().Name;
        _nuiMessageMethod = nameof(OnNuiMessage);

        _instance = this;
        _logger = Logger;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "script");
        builder.AddAttribute(1, "type", "text/javascript");
        
        builder.AddMarkupContent(2, $@"
        window.addEventListener('message', async (event) => {{
            await DotNet.invokeMethodAsync('{_assemblyName}', '{_nuiMessageMethod}', event.data);
        }});");
        
        builder.CloseElement();
    }

    [JSInvokable]
    public static async Task OnNuiMessage(JsonDocument eventData)
    {
        var methods = await NuiComponent.GetMessageHandlerMethods();

        if (!eventData.RootElement.TryGetProperty(NuiMessageHandler.Identifier, out var identifierValue))
        {
            // log debug here
            return;
        }
        
        var identifiedMethods = methods.Where(m => m.Type == identifierValue.GetString()).ToList();
        
        // Optionally, if it's > 1, then check and make sure all identified methods have the same number of params and all params have the same types and names
        // filter identified methods down further
        if (identifiedMethods.Count > 1)
        {
            throw new InvalidOperationException($"More than one method is attached to the {nameof(NuiMessageHandler)} attribute with the type identifier {identifierValue}");
        }

        var identifiedMethod = identifiedMethods.First();
        var methodParams = identifiedMethod.Info.GetParameters();
        var methodValues = new List<object>();

        foreach (var param in methodParams)
        {
            try
            {
                if (param.Name is null)
                {
                    throw new Exception();
                }
                
                if (eventData.RootElement.TryGetProperty(param.Name, out var element))
                {
                    var deserialized = element.Deserialize(param.ParameterType, NuiJsonSerializerOptions.Options);
                    if (deserialized is not null)
                    {
                        methodValues.Add(deserialized);
                    }
                    else
                    {
                        throw new InvalidOperationException("Deserialized object is null, this isn't expected");
                    }
                }
                else
                {
                    LogErrorPropertyNotFound(_logger!, param.Name, param.ParameterType);
                    return;
                }
            }
            catch (Exception e)
            {
                LogCriticalJsonBinding(_logger!, e, param.Name, param.ParameterType);
                return;
            }
        }
        
        // log debug here
        _logger!.LogDebug("Attempting to invoke method {MethodName} with {NumberOfParameters} parameters", identifiedMethod.Info.Name, methodValues.Count);

        await InvokeAsync(identifiedMethod.Info, identifiedMethod.Instance, methodValues.ToArray());
    }
    
    private static async ValueTask InvokeAsync(MethodInfo info, object? instance, object?[]? parameters)
    {
        if (info.ReturnType == typeof(Task))
        {
            await (info.Invoke(instance, parameters) as Task)!;
        }
        else if (info.ReturnType.IsGenericType &&
                 info.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            await (info.Invoke(instance, parameters) as Task)!;
        }
        else
        {
            info.Invoke(instance, parameters);
        }
    }
    
    
    [LoggerMessage(1, LogLevel.Critical, "Critical exception in {caller} when attempting to bind to a handler method with a parameter name {parameterName} and parameter type {parameterType}", EventName = "Handler method parameter JSON binding")]
    static partial void LogCriticalJsonBinding(ILogger logger, Exception ex, string? parameterName, Type parameterType, [CallerMemberName] string caller = nameof(NuiMessageListener));
    
    [LoggerMessage(2, LogLevel.Critical, "Parameter not found in the handler when attempting to bind with a parameter name {parameterName} and parameter type {parameterType}", EventName = "Handler method parameter discovery")]
    static partial void LogErrorPropertyNotFound(ILogger logger, string? parameterName, Type parameterType, [CallerMemberName] string caller = nameof(NuiMessageListener));
}
