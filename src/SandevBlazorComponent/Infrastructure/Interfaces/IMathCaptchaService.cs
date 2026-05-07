namespace SandevBlazorComponent.Infrastructure.Interfaces;

public interface IMathCaptchaService
{
    (string key, string question) Generate();
    bool Validate(string key, int input);
}
