using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SandevBlazorComponent.Infrastructure.JsInterop.ViewerAndEditor;

public class InPlaceEditorJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    private const string JS_PATH = "./_content/SandevBlazorComponent/js/viewer-and-editor/sandev-blazor.js";

    public InPlaceEditorJsInterop(IJSRuntime jsRuntime)
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

    public async Task FocusFirstInput(ElementReference container)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("focusFirstInput", container);
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

    public async Task PositionPopup(ElementReference target, ElementReference popup)
    {
        var module = await moduleTask.Value;
        await module.InvokeVoidAsync("positionPopup", target, popup);
    }

    public async ValueTask DisposeAsync()
    {
        if (!moduleTask.IsValueCreated)
            return;

        try
        {
            var module = await moduleTask.Value;

            if (module != null)
            {
                await module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
            // ignore
        }
        catch (ObjectDisposedException)
        {
            // optional safety
        }
    }
}
