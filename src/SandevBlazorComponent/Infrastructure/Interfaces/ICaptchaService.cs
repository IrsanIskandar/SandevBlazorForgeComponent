using SandevBlazorComponent.Infrastructure.Models;

namespace SandevBlazorComponent.Infrastructure.Interfaces;

public interface ICaptchaService
{
    //(string key, string base64Image) GenerateCaptcha();
    ImageCaptchaResult Generate(CaptchaRequest options);
    bool ValidateCaptcha(string key, string input);
}
