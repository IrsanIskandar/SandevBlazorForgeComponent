using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SandevBlazorComponent.Infrastructure.Interfaces;
using SandevBlazorComponent.Infrastructure.Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

namespace SandevBlazorComponent.Infrastructure.BusinessLogic;

public class CaptchaService : ICaptchaService
{
    private readonly IMemoryCache _cache;
    private readonly Random _rand = new();

    private const int ExpireMinutes = 5;
    private const int MaxAttempts = 5;

    public CaptchaService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public ImageCaptchaResult Generate(CaptchaRequest req)
    {
        var text = GenerateRandomText(req);
        var key = Guid.NewGuid().ToString();

        var entry = new CaptchaEntry
        {
            Value = text,
            ExpiredAt = DateTime.UtcNow.AddMinutes(ExpireMinutes),
            Attempts = 0
        };

        _cache.Set(key, entry, TimeSpan.FromMinutes(ExpireMinutes));

        var imageBytes = GenerateImage(text, req);

        return new ImageCaptchaResult
        {
            Key = key,
            Base64Image = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}"
        };
    }

    public bool ValidateCaptcha(string key, string input)
    {
        // 🔒 guard clause
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(input))
            return false;

        input = input.Trim();

        // 🔍 ambil dari cache
        if (!_cache.TryGetValue(key, out CaptchaEntry entry))
            return false;

        // ⏱️ cek expired
        if (DateTime.UtcNow > entry.ExpiredAt)
        {
            _cache.Remove(key);
            return false;
        }

        // 🚫 increment attempt
        entry.Attempts++;

        if (entry.Attempts > MaxAttempts)
        {
            _cache.Remove(key);
            return false;
        }

        // 🔐 secure compare (anti timing attack)
        bool isValid = SecureEquals(entry.Value, input);

        if (isValid)
        {
            // 🔥 anti replay (sekali pakai langsung hapus)
            _cache.Remove(key);
            return true;
        }

        // 🔄 update cache dengan sisa waktu yang valid
        var remaining = entry.ExpiredAt - DateTime.UtcNow;

        if (remaining <= TimeSpan.Zero)
        {
            _cache.Remove(key);
            return false;
        }

        _cache.Set(key, entry, remaining);

        return false;
    }

    private bool SecureEquals(string a, string b)
    {
        if (a.Length != b.Length)
            return false;

        int result = 0;

        for (int i = 0; i < a.Length; i++)
        {
            result |= char.ToLowerInvariant(a[i]) ^ char.ToLowerInvariant(b[i]);
        }

        return result == 0;
    }

    private string GenerateRandomText(CaptchaRequest req)
    {
        int length = Math.Min(req.Length, req.MaxLength);

        const string normalChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        const string specialChars = "@#$*&";

        var result = new List<char>();

        int specialCount = _rand.Next(1, Math.Min(req.MaxSpecialChars, length) + 1);

        for (int i = 0; i < specialCount; i++)
        {
            result.Add(specialChars[_rand.Next(specialChars.Length)]);
        }

        for (int i = result.Count; i < length; i++)
        {
            result.Add(normalChars[_rand.Next(normalChars.Length)]);
        }

        return new string(result.OrderBy(_ => _rand.Next()).ToArray());
    }

    private byte[] GenerateImage(string text, CaptchaRequest options)
    {
        int width = options.WidthPerChar * text.Length;
        int height = options.Height;

        using var image = new Image<Rgba32>(width, height);

        image.Mutate(ctx =>
        {
            ctx.Fill(Color.White);

            var font = SystemFonts.CreateFont("Arial", 28, FontStyle.Bold);

            int x = 10;

            foreach (char c in text)
            {
                float y = _rand.Next(5, 20);

                var color = Color.FromRgb(
                    (byte)_rand.Next(50, 200),
                    (byte)_rand.Next(50, 200),
                    (byte)_rand.Next(50, 200));

                float angle = _rand.Next(-30, 30);

                var optionsDraw = new DrawingOptions
                {
                    Transform = Matrix3x2.CreateRotation(
                        DegreesToRadians(angle),
                        new PointF(x, y))
                };

                ctx.DrawText(optionsDraw, c.ToString(), font, color, new PointF(x, y));

                x += options.WidthPerChar;
            }

            // noise lines
            for (int i = 0; i < options.NoiseLines; i++)
            {
                var p1 = new PointF(_rand.Next(width), _rand.Next(height));
                var p2 = new PointF(_rand.Next(width), _rand.Next(height));

                ctx.DrawLine(Color.Gray, 1, p1, p2);
            }

            // noise dots
            for (int i = 0; i < options.NoiseDots; i++)
            {
                ctx.Fill(Color.LightGray,
                    new EllipsePolygon(_rand.Next(width), _rand.Next(height), 1));
            }

            if (options.BlurRadius > 0)
            {
                ctx.GaussianBlur(options.BlurRadius);
            }
        });

        ApplyWaveDistortion(image, options);

        using var ms = new MemoryStream();
        image.SaveAsPng(ms);

        return ms.ToArray();
    }

    // ============================
    // 🔥 WAVE DISTORTION
    // ============================
    private void ApplyWaveDistortion(Image<Rgba32> image, CaptchaRequest options)
    {
        int width = image.Width;
        int height = image.Height;

        using var clone = image.Clone();

        for (int y = 0; y < height; y++)
        {
            int offsetX = (int)(Math.Sin((y + _rand.Next(5)) * options.WaveFrequency) * options.WaveAmplitude);

            for (int x = 0; x < width; x++)
            {
                int srcX = x + offsetX;

                if (srcX >= 0 && srcX < width)
                {
                    image[x, y] = clone[srcX, y];
                }
            }
        }
    }

    private float DegreesToRadians(float deg)
        => (float)(Math.PI / 180) * deg;
}
