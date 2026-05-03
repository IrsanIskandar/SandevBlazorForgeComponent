using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SandevBlazorComponent.Infrastructure.EnumClass;
using SandevBlazorComponent.Infrastructure.JsInterop;

namespace SandevBlazorComponent.ViewerAndEditor;

public partial class InPlaceEditor<T> : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime? JS { get; set; }
    [Inject] private BaseJsInterop? JSCore { get; set; }

    [Parameter] public string? Label { get; set; }
    [Parameter] public bool FloatLabel { get; set; }
    [Parameter] public LabelPosition LabelPosition { get; set; } = LabelPosition.Top;
    [Parameter] public string? LabelCssClass { get; set; }
    [Parameter] public bool Required { get; set; }

    [Parameter] public T? Value { get; set; }
    [Parameter] public EventCallback<T> ValueChanged { get; set; }

    [Parameter] public EditorMode Mode { get; set; } = EditorMode.Inline;
    [Parameter] public EditableOn EditableOn { get; set; } = EditableOn.Click;
    [Parameter] public bool ShowButtons { get; set; } = true;

    // 🔥 Template support
    [Parameter] public RenderFragment<T>? EditorTemplate { get; set; }

    protected bool IsEditing;
    protected T? CurrentValue;

    private bool _isJsRegistered;

    protected ElementReference InputRef;
    protected ElementReference PopupRef;
    protected ElementReference ContainerRef;

    private DotNetObjectReference<InPlaceEditor<T>>? dotnetRef;

    protected override void OnInitialized()
    {
        CurrentValue = Value;
        JSCore = new(JS);
        dotnetRef = DotNetObjectReference.Create(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsEditing && !_isJsRegistered)
        {
            _isJsRegistered = true;

            await JSCore.Focus(InputRef);
            await JSCore.RegisterClickOutside(ContainerRef, dotnetRef);

            if (Mode == EditorMode.Popup)
            {
                await JSCore.PositionPopup(ContainerRef, PopupRef);
            }
        }
    }

    private string DisplayValue => Value?.ToString() ?? "";

    private void HandleClick()
    {
        if (EditableOn == EditableOn.Click)
            EnableEdit();
    }

    private void HandleDoubleClick()
    {
        if (EditableOn == EditableOn.DoubleClick)
            EnableEdit();
    }

    private void EnableEdit()
    {
        CurrentValue = Value;
        IsEditing = true;
    }

    private async Task Save()
    {
        IsEditing = false;
        _isJsRegistered = false;

        Value = CurrentValue;
        await ValueChanged.InvokeAsync(Value);
        await JSCore.UnregisterClickOutside(ContainerRef);
    }

    private async Task Cancel()
    {
        IsEditing = false;
        _isJsRegistered = false;

        CurrentValue = Value;
        await JSCore.UnregisterClickOutside(ContainerRef);
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            await Save();
        else if (e.Key == "Escape")
            await Cancel();
    }

    [JSInvokable]
    public async Task OnClickOutside()
    {
        await Cancel();
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        if (JSCore != null)
            await JSCore.DisposeAsync();

        dotnetRef?.Dispose();
    }
}
