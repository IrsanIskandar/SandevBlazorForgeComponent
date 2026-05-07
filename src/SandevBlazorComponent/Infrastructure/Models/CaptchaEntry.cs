namespace SandevBlazorComponent.Infrastructure.Models;

public class CaptchaEntry
{
    public string Value { get; set; } = default!;
    public DateTime ExpiredAt { get; set; }
    public int Attempts { get; set; } = 0;
}
