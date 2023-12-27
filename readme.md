# CitizenFX.Extensions.Blazor.WebAssembly
An unofficial set of extensions for developing Nui interfaces with Blazor WASM in .NET 8

[![GitHub License](https://img.shields.io/github/license/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly?style=flat-square)](https://github.com/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly/blob/main/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/CitizenFX.Extensions.Blazor.WebAssembly?style=flat-square)](https://www.nuget.org/packages/CitizenFX.Extensions.Blazor.WebAssembly)
[![GitHub release](https://img.shields.io/github/v/release/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly?style=flat-square)](https://github.com/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly/releases)
[![Nuget](https://img.shields.io/nuget/v/CitizenFX.Extensions.Blazor.WebAssembly?style=flat-square)](https://www.nuget.org/packages/CitizenFX.Extensions.Blazor.WebAssembly)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly/build-publish.yaml?style=flat-square)](https://github.com/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly/actions/workflows/build-publish.yaml)

## Features
- Attribute-based Nui Message handling
  - `[NuiMessageHandler("showui:true")]`
- Service-based Nui Callback triggering
  - `@inject INuiCallbackService`
  - `await NuiCallbackService.TriggerNuiCallbackAsync("getItemInfo", new { item = "phone" });`

## Getting started
- For Nui Message handling, the [NuiMessageListener](src/CitizenFX.Extensions.Blazor.WebAssembly/NuiMessageListener.cs) must be injected as a root-component in index.html, and your component(s) must inherit [NuiComponent](src/CitizenFX.Extensions.Blazor.WebAssembly/NuiComponent.cs)
  - Add `<template id="nui-message-listener"></template>` to your `index.html` in the `<body>`
  - Add `builder.RootComponents.Add<NuiMessageListener>("#nui-message-listener");` in your `Program.cs`
  - This adds some Javascript to your `<body>` that directs any Nui Messages for the resource to `NuiMessageListener`
  - Add `@inherits NuiComponent` in your component
  - Add `[NuiMessageHandler("<type-name>")]` to any static or instanced method in your component
- For triggering Nui Callbacks, [NuiCallbackService](src/CitizenFX.Extensions.Blazor.WebAssembly/Services/NuiCallbackService.cs) must be added to your service collection
  - Add `builder.Services.AddNuiServices();` in your `Program.cs`
  - Inject in your page with `@inject INuiCallbackService NuiCallbackService`

## Notes
- `NuiMessageListener` & `NuiMessageHandler` requires/uses a specific string field in the sending-message to determine which method to invoke
  - Currently that field is `type`, but this may change in the future to be configurable
  - When `NuiMessageListener` recieves an Nui message, `NuiMessageListener` will look for all `NuiMessageHandler` attributed methods constructed with the matching `type` name found in the recieved Nui message, and then attempt to deserialize any json fields specified in the `NuiMessageHandler` attributed method paramters
- Currently, you may only have one `NuiMessageListener` per type-string
- `NuiMessageListener` and `NuiCallbackService` uses System.Text.Json to serialize & deserialize json in the library
  - You can configure the JsonSerializerOptions the message handler and the callback service uses in `AddNuiServices();`
  - `builder.Services.AddNuiServices(options => { options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb); });`
 
## Example - `NuiMessageHandler`
`client.lua`
```lua
RegisterCommand('ui', function()
  SendNUIMessage({ type = "blazor:show-ui"})
end)

RegisterCommand('ui-increment', function()
  SendNUIMessage({ type = "blazor:increment" })
end)

-- Assumes args[1] is an integer
RegisterCommand('ui-set', function(source, args, user)
  SendNUIMessage({
    type = "blazor:set",
    count = tonumber(args[1])
  })
end)
```
`App.razor`
```csharp
@using CitizenFX.Extensions.Blazor.WebAssembly
@inherits NuiComponent

<div style="text-align: center; color: aquamarine">
@if (_showUi)
{
    <h1>I'm a Nui in Blazor!</h1>
    @if (_count >= 3)
    {
        <h2>Count: @_count</h2>
    }
}
</div>

@code {

    private bool _showUi = false;
    private int _count = 0;
    
    [NuiMessageHandler("blazor:increment")]
    public async Task OnIncrement()
    {
        Interlocked.Increment(ref _count);
        
        StateHasChanged(); // Tell Blazor to re-render the page
    }

    [NuiMessageHandler("blazor:set")]
    public async Task SetCount(int count)
    {
        Interlocked.Exchange(ref _count, count);
        
        StateHasChanged(); // Tell Blazor to re-render the page
    }

    [NuiMessageHandler("blazor:show-ui")]
    public void OnShowUi()
    {
        _showUi = true;
        
        StateHasChanged(); // Tell Blazor to re-render the page
    }
}
```

## Example - `NuiCallbackService`
`client.lua`
```lua
RegisterCommand('add-item', function() 
    SendNUIMessage({ 
        type = "blazor:add-item",
        itemId = 69, 
    })
end)

RegisterNUICallback('addItemCallback', function(data, cb) 
    SendNUIMessage({ 
        type = "blazor:callback"
    })
end)

```
`App.razor`
```csharp
@using CitizenFX.Extensions.Blazor.WebAssembly
@using CitizenFX.Extensions.Blazor.WebAssembly.Services
@inherits NuiComponent
@inject INuiCallbackService NuiCallbackService

<div style="text-align: center; color: aquamarine">
    <h1>I'm a Nui in Blazor!</h1>
    <h3>Last item added: @_lastItemAdded</h3>
    @if (_recievedFromCallback)
    {
        <h3>Hello from our Nui callback!</h3>
    }
</div>

@code {

    private bool _recievedFromCallback = false;
    private string _lastItemAdded = "None";
    
    [NuiMessageHandler("blazor:add-item")]
    public async Task AddItem(int itemId)
    {
        _lastItemAdded = itemId.ToString();
        
        StateHasChanged(); // Tell Blazor to re-render the page
        
        await NuiCallbackService.TriggerNuiCallbackAsync("addItemCallback", new { itemId = itemId });
    }
    
    [NuiMessageHandler("blazor:callback")]
    public void FromCallback()
    {
        _recievedFromCallback = true;
        
        StateHasChanged(); // Tell Blazor to re-render the page
    }
}
```
