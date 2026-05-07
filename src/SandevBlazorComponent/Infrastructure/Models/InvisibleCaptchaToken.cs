namespace SandevBlazorComponent.Infrastructure.Models;

public class InvisibleCaptchaToken
{
    public DateTime CreatedAt { get; set; }
    public string Fingerprint { get; set; } = default!;
}
