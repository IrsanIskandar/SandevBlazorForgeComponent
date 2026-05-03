using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SandevBlazorComponent.Infrastructure.JsInterop;

public class BaseJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    private const string JS_PATH = "./_content/SandevBlazorComponent/js/dom-utils.js";

    public BaseJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() =>
            jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", JS_PATH
            ).AsTask());
    }

    public async Task Focus(ElementReference element)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("focusElement", element);
    }

    public async Task RegisterClickOutside<T>(ElementReference element, DotNetObjectReference<T> dotnetRef)
        where T : class
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("registerClickOutside", element, dotnetRef);
    }

    public async Task UnregisterClickOutside(ElementReference element)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("unregisterClickOutside", element);
    }

    public async Task PositionPopup(ElementReference target, ElementReference popup)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("positionPopup", target, popup);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
