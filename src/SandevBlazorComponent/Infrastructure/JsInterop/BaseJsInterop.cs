using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SandevBlazorComponent.Infrastructure.JsInterop;

public class BaseJsInterop
{
    private readonly IJSRuntime _js;

    public BaseJsInterop(IJSRuntime js)
    {
        _js = js;
    }

    public async Task FocusElement(ElementReference el)
    {
        await _js.InvokeVoidAsync("sandev.focusElement", el);
    }

    public async Task ScrollToTop()
    {
        await _js.InvokeVoidAsync("window.scrollTo", 0, 0);
    }
}
