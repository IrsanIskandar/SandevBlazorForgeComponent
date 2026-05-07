namespace SandevBlazorComponent.Infrastructure.Models;

public class InvisibleCaptchaRequest
{
    public string Token { get; set; } = default!;
    public string Fingerprint { get; set; } = default!;
    public List<MousePoint> Interactions { get; set; } = new();
    public string? Honeypot { get; set; }
}
