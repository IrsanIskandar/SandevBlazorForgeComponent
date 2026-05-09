using Microsoft.Extensions.DependencyInjection;
using SandevBlazorComponent.Infrastructure.BusinessLogic;
using SandevBlazorComponent.Infrastructure.Interfaces;
using SandevBlazorComponent.Infrastructure.Models;

namespace SandevBlazorComponent;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSandevBlazor(this IServiceCollection services, Action<SandevOptions>? configure = null)
    {
        // 🔹 Options
        var options = new SandevOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);

        services.AddSingleton<ThemeService>();

        // 🔹 Core Services
        services.AddScoped<Infrastructure.JsInterop.BaseJsInterop>();

        services.AddMemoryCache();

        // 🔹 Captcha
        services.AddScoped<ICaptchaService, CaptchaService>();
        services.AddScoped<IMathCaptchaService, MathCaptchaService>();
        services.AddScoped<IInvisibleCaptchaService, InvisibleCaptchaService>();

        // 🔹 Future services
        // services.AddScoped<DialogService>();
        // services.AddScoped<NotificationService>();

        return services;
    }
}
