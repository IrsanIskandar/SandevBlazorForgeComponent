namespace SandevBlazorComponent.Infrastructure.Models;

public class SandevOptions
{
    public bool LoadSandevCss { get; set; } = true;
    public bool LoadSandevJs { get; set; } = true;
    public bool LoadFontAwesome { get; set; } = true;
    public string PrimaryColor { get; set; } = "#3b82f6";

    public bool DarkMode { get; set; } = false;

    public SandevTheme Theme { get; set; } = new();
}
