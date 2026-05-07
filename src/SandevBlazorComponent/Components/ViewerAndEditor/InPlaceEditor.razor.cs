using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SandevBlazorComponent.Infrastructure.EnumClass;
using SandevBlazorComponent.Infrastructure.JsInterop;
using SandevBlazorComponent.Infrastructure.JsInterop.ViewerAndEditor;
using System.Globalization;

namespace SandevBlazorComponent.Components.ViewerAndEditor;

public partial class InPlaceEditor<T> : IAsyncDisposable
{
    [Inject] private IJSRuntime? JS { get; set; }

    [Parameter] public string? Label { get; set; }
    [Parameter] public bool FloatLabel { get; set; }
    [Parameter] public LabelPosition LabelPosition { get; set; } = LabelPosition.Top;
    [Parameter] public bool Required { get; set; }

    [Parameter] public T? Value { get; set; }
    [Parameter] public EventCallback<T> ValueChanged { get; set; }
    [Parameter] public string EmptyText { get; set; } = "Click to edit";

    [Parameter] public EditorMode Mode { get; set; } = EditorMode.Inline;
    [Parameter] public EditableOn EditableOn { get; set; } = EditableOn.Click;
    [Parameter] public EditorType EditorType { get; set; } = EditorType.Text;
    [Parameter] public bool ShowButtons { get; set; } = true;

    [Parameter] public IEnumerable<T>? Items { get; set; }

    // 🔥 Template support
    [Parameter] public RenderFragment<T>? EditorTemplate { get; set; }

    protected bool IsEditing;
    protected T? CurrentValue;

    private bool _shouldFocus;

    protected ElementReference InputRef;
    protected ElementReference PopupRef;    // untuk positioning (optional)
    protected ElementReference WrapperRef;  // untuk click outside
    protected ElementReference EditorRef;   // untuk focus

    private DotNetObjectReference<InPlaceEditor<T>>? dotnetRef;

    private IJSObjectReference? _module;

    private string DisplayValue => Value?.ToString() ?? "";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        CurrentValue = Value;
        dotnetRef = DotNetObjectReference.Create(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _module = await JS!.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/SandevBlazorComponent/js/viewer-and-editor/inplace-editor.js");
        }

        if (_shouldFocus && _module != null)
        {
            _shouldFocus = false;

            await Task.Yield();

            await _module.InvokeVoidAsync("focusFirstInput", EditorRef);

            await Task.Delay(100);

            await _module.InvokeVoidAsync("registerClickOutside", WrapperRef, dotnetRef);

            if (Mode == EditorMode.Popup)
            {
                await _module.InvokeVoidAsync("positionPopup", WrapperRef, EditorRef);
            }
        }
    }

    private async Task HandleClick()
    {
        if (EditableOn == EditableOn.Click)
            await StartEditingAsync();
    }

    private async Task HandleDoubleClick()
    {
        if (EditableOn == EditableOn.DoubleClick)
            await StartEditingAsync();
    }

    private async Task Save()
    {
        if (!IsValid())
            return;

        IsEditing = false;

        Value = CurrentValue;
        await ValueChanged.InvokeAsync(Value);

        if (_module != null)
            await _module.InvokeVoidAsync("unregisterClickOutside", WrapperRef);
    }

    private async Task Cancel()
    {
        IsEditing = false;

        CurrentValue = Value;

        if (_module != null)
            await _module.InvokeVoidAsync("unregisterClickOutside", WrapperRef);
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
        if (!IsEditing) return;

        await Cancel();
        await InvokeAsync(StateHasChanged);
    }

    protected RenderFragment RenderEditor() => builder =>
    {
        var seq = 0;

        switch (EditorType)
        {
            case EditorType.Text:
                BindInput<T>(builder, ref seq, "input", "ipe-input");
                break;

            case EditorType.TextArea:
                BindInput<T>(builder, ref seq, "textarea", "ipe-textarea");
                break;

            case EditorType.Numeric:
                BindInput<T>(builder, ref seq, "input", "ipe-input", "number");
                break;

            case EditorType.Date:
                BindInput<T>(builder, ref seq, "input", "ipe-input", "date");
                break;

            case EditorType.Dropdown:
                builder.OpenElement(seq++, "select");
                builder.AddAttribute(seq++, "class", "ipe-select");

                if (Items != null)
                {
                    foreach (var item in Items)
                    {
                        builder.OpenElement(seq++, "option");
                        builder.AddAttribute(seq++, "value", item?.ToString());
                        builder.AddContent(seq++, item?.ToString());
                        builder.CloseElement();
                    }
                }

                builder.AddAttribute(seq++, "onchange",
                    EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
                    {
                        if (BindConverter.TryConvertTo<T>(
                            e.Value,
                            CultureInfo.InvariantCulture,
                            out var result))
                        {
                            CurrentValue = result;
                        }
                    })
                );

                builder.CloseElement();
                break;

            default:
                builder.AddContent(seq++, "Unsupported editor type");
                break;
        }
    };

    private async Task StartEditingAsync()
    {
        if (IsEditing) return;

        CurrentValue = Value;
        IsEditing = true;
        _shouldFocus = true;

        await InvokeAsync(StateHasChanged);
    }

    private bool IsValid()
    {
        if (!Required) return true;

        if (CurrentValue == null) return false;

        if (typeof(T) == typeof(string))
        {
            return !string.IsNullOrWhiteSpace(CurrentValue.ToString());
        }

        return true;
    }

    private void BindInput<TValue>(RenderTreeBuilder builder, ref int seq, string element, string cssClass, string? type = null)
    {
        builder.OpenElement(seq++, element);

        if (!string.IsNullOrEmpty(type))
            builder.AddAttribute(seq++, "type", type);

        builder.AddAttribute(seq++, "class", cssClass);

        builder.AddAttribute(seq++, "value", BindConverter.FormatValue(CurrentValue));

        builder.AddAttribute(seq++, "placeholder", EmptyText);

        builder.AddAttribute(seq++, "onchange",
            EventCallback.Factory.Create<ChangeEventArgs>(this, e =>
            {
                try
                {
                    if (BindConverter.TryConvertTo<T>(
                        e.Value,
                        CultureInfo.InvariantCulture,
                        out var result))
                    {
                        CurrentValue = result;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            })
        );

        builder.AddAttribute(seq++, "onkeydown",
            EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDown));

        builder.CloseElement();
    }

    private string GetWrapperClass()
    {
        return LabelPosition switch
        {
            LabelPosition.Left => "left",
            LabelPosition.Right => "right",
            _ => "top"
        };
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_module != null)
            {
                await _module.InvokeVoidAsync("unregisterClickOutside", WrapperRef);
                await _module.InvokeVoidAsync("unregisterPopup", EditorRef); // 🔥 penting
                await _module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
            // aman
        }

        dotnetRef?.Dispose();
    }
}
