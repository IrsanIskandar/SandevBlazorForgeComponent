using SandevBlazorComponent.Infrastructure.Models;

namespace SandevBlazorComponent.Infrastructure.BusinessLogic;

public class ThemeService
{
    public SandevTheme Theme { get; }

    public ThemeService(SandevOptions options)
    {
        Theme = options.Theme;
    }
}
