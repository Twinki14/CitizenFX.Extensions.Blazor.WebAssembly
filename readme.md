# CitizenFX.Extensions.Blazor.WebAssembly
An unofficial set of extensions for developing Nui interfaces with Blazor WASM in .NET 8

[![GitHub License](https://img.shields.io/github/license/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly?style=flat-square)](https://github.com/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly/blob/main/LICENSE)
[![Downloads](https://img.shields.io/nuget/dt/CitizenFX.Extensions.Blazor.WebAssembly?style=flat-square)](https://www.nuget.org/packages/CitizenFX.Extensions.Blazor.WebAssembly)
[![GitHub release](https://img.shields.io/github/v/release/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly?style=flat-square)](https://github.com/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly/releases)
[![Nuget](https://img.shields.io/nuget/v/CitizenFX.Extensions.Blazor.WebAssembly?style=flat-square)](https://www.nuget.org/packages/CitizenFX.Extensions.Blazor.WebAssembly)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly/build-publish.yaml?style=flat-square)](https://github.com/Twinki14/CitizenFX.Extensions.Blazor.WebAssembly/actions/workflows/build-publish.yaml)

## Features
- Attribute-based Nui Message handling - `[NuiMessageHandler("showui:true")]`
- Service-based Nui Callback triggering - `await NuiCallbackService.TriggerNuiCallbackAsync("getItemInfo", new { item = "phone" });`

## Getting started
- For Nui Message handling, the [NuiMessageListener](src/CitizenFX.Extensions.Blazor.WebAssembly/NuiMessageListener.cs) must be injected as a root-component in index.html, and your component(s) must inherit [NuiComponent](src/CitizenFX.Extensions.Blazor.WebAssembly/NuiComponent.cs)
  - Add `<template id="nui-message-listener"></template>` to your `index.html` in the `<body>`
  - Add `builder.RootComponents.Add<NuiMessageListener>("#nui-message-listener");` in your `Program.cs`
  - This adds some Javascript to your `<body>` that directs any Nui Messages for the resource to [NuiMessageListener](src/CitizenFX.Extensions.Blazor.WebAssembly/NuiMessageListener.cs)
  - Add `@inherits NuiComponent` in your component
- For triggering Nui Callbacks, [NuiCallbackService](src/CitizenFX.Extensions.Blazor.WebAssembly/Services/NuiCallbackService.cs) must be added to your service collection
  - Add `builder.Services.AddNuiServices();` in your `Program.cs`
  - Inject in your page with `@inject INuiCallbackService NuiCallbackService`
