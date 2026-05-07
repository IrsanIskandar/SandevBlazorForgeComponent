using SandevBlazorComponent.Infrastructure.Models;

namespace SandevBlazorComponent.Infrastructure.Interfaces;

public interface IInvisibleCaptchaService
{
    string GenerateToken(string fingerprint);
    bool ValidateToken(InvisibleCaptchaRequest req);
}
