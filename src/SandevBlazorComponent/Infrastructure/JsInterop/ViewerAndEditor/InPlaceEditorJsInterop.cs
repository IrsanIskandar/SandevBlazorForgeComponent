using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SandevBlazorComponent.Infrastructure.JsInterop.ViewerAndEditor;

public class InPlaceEditorJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public InPlaceEditorJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() =>
            jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/SandevBlazorComponent/js/viewer-and-editor/inplace-editor.js"
            ).AsTask());
    }

    public async Task RegisterClickOutside<T>(ElementReference element, DotNetObjectReference<T> dotnetRef) where T : class
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("registerClickOutside", element, dotnetRef);
    }

    public async Task UnregisterClickOutside(ElementReference element)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("unregisterClickOutside", element);
    }

    public async Task Focus(ElementReference element)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("focusElement", element);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    public async Task PositionPopup(ElementReference target, ElementReference popup)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("positionPopup", target, popup);
    }
}
