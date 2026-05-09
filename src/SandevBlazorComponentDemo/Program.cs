using SandevBlazorComponent;
using SandevBlazorComponentDemo.Components;

namespace SandevBlazorComponentDemo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ✅ REGISTER SERVICES (HARUS SEBELUM BUILD)
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddCircuitOptions(options =>
            {
                options.DetailedErrors = true;
            });

        builder.Services.AddSandevBlazor(options =>
        {
            options.PrimaryColor = "#6366f1";
            options.DarkMode = false;
        });

        var app = builder.Build();

        // Middleware
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", null, true);
        app.UseHttpsRedirection();
        app.UseAntiforgery();

        // ✅ STATIC FILES
        app.UseStaticFiles(); // ✅ WAJIB
        //app.MapStaticAssets();

        // 🔥 INI YANG KAMU HAPUS TADI (WAJIB ADA)
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
