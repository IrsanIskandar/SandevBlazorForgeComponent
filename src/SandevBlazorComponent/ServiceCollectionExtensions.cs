using Microsoft.Extensions.DependencyInjection;
using SandevBlazorComponent.Infrastructure.BusinessLogic;
using SandevBlazorComponent.Infrastructure.Interfaces;

namespace SandevBlazorComponent;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSandevBlazor(this IServiceCollection services)
    {
        // 🔥 register semua service di sini
        services.AddScoped<Infrastructure.JsInterop.BaseJsInterop>();

        services.AddMemoryCache();
        services.AddScoped<ICaptchaService, CaptchaService>();
        services.AddScoped<IMathCaptchaService, MathCaptchaService>();
        services.AddScoped<IInvisibleCaptchaService, InvisibleCaptchaService>();

        // kalau nanti ada:
        // services.AddScoped<DialogService>();
        // services.AddScoped<NotificationService>();

        return services;
    }
}
