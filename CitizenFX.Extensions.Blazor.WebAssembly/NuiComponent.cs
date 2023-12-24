using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace CitizenFX.Extensions.Blazor.WebAssembly;

/// <summary>
/// A FiveM Nui helper-component. Required for <see cref="NuiMessageHandler"/> methods to be invoked.
/// </summary>
/// <remarks>
/// Inherits <see cref="ComponentBase"/>.
/// </remarks>
public class NuiComponent : ComponentBase
{
    private static int _lastInstanceCount;
    private static readonly ConcurrentBag<NuiComponent> Instances = [];

    private static List<MessageHandlerMethod> _messageHandlerMethods = [];
    private static readonly SemaphoreSlim InstanceSemaphore = new(1, 1);
        
    public NuiComponent()
    {
        Instances.Add(this);
    }

    internal static async ValueTask<IEnumerable<MessageHandlerMethod>> GetMessageHandlerMethods()
    {
       var instancesCount = Instances.Count;

       if (instancesCount == _lastInstanceCount)
       {
           return _messageHandlerMethods;
       }

       await InstanceSemaphore.WaitAsync();
       
       try
       {
           _messageHandlerMethods = FindMessageHandlerMethods();
           _lastInstanceCount = instancesCount;
           
           return _messageHandlerMethods;
       }
       finally
       {
           InstanceSemaphore.Release();
       }
    }
    
    internal readonly struct MessageHandlerMethod(MethodInfo info, object? instance, string type)
    {
        public MethodInfo Info { get; } = info;
        public object? Instance { get; } = instance;
        public string Type { get; } = type;
    }

    private static List<MessageHandlerMethod> FindMessageHandlerMethods()
    {
        var methods = new List<MessageHandlerMethod>();

        foreach (var instance in Instances)
        {
            var methodsWithAttribute = instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => Attribute.IsDefined(m, typeof(NuiMessageHandler)))
                .Select(m =>
                {
                    var attribute = (NuiMessageHandler) Attribute.GetCustomAttribute(m, typeof(NuiMessageHandler))!;
                    return new MessageHandlerMethod(m, instance, attribute.Type);
                })
                .ToList();

            methods.AddRange(methodsWithAttribute);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var staticMethodsInAssembly = assembly.GetTypes()
                .Where(type => typeof(NuiComponent).IsAssignableFrom(type) && type != typeof(NuiComponent))
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(method => Attribute.IsDefined(method, typeof(NuiMessageHandler)))
                    .Select(method =>
                    {
                        var attribute = (NuiMessageHandler) Attribute.GetCustomAttribute(method, typeof(NuiMessageHandler))!;
                        return new MessageHandlerMethod(method, null, attribute.Type);
                    }))
                .ToList();

            methods.AddRange(staticMethodsInAssembly);
        }

        return methods;
    }
}
