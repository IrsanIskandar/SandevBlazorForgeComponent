namespace SandevBlazorComponent.Infrastructure.Models;

public class MathCaptchaEntry
{
    public int Answer { get; set; }
    public DateTime ExpiredAt { get; set; }
    public int Attempts { get; set; }
}
