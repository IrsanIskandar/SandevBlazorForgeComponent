namespace SandevBlazorComponent.Infrastructure.Models;

public class CaptchaRequest
{
    public int Length { get; set; }
    public int MaxLength { get; set; }
    public int MaxSpecialChars { get; set; }

    public int WidthPerChar { get; set; }
    public int Height { get; set; }

    public float WaveAmplitude { get; set; }
    public float WaveFrequency { get; set; }

    public int NoiseLines { get; set; }
    public int NoiseDots { get; set; }

    public float BlurRadius { get; set; }
}
