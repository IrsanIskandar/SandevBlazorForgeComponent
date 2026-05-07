using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SandevBlazorComponent.Infrastructure;

public abstract class SandevComponentBase : ComponentBase, IAsyncDisposable
{
    [Inject] protected IJSRuntime JS { get; set; } = default!;
    protected IJSObjectReference? Module;

    protected async Task LoadModuleAsync(string path)
    {
        Module = await JS.InvokeAsync<IJSObjectReference>(
            "import",
            $"./_content/SandevBlazorComponent/{path}");
    }

    protected async Task InvokeVoidAsync(string identifier, params object[] args)
    {
        if (Module == null)
            throw new InvalidOperationException("JS Module not loaded.");

        await Module.InvokeVoidAsync(identifier, args);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (Module != null)
                await Module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // aman untuk Blazor Server
        }
    }
}
