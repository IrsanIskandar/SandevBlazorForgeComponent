using Microsoft.Extensions.Caching.Memory;
using SandevBlazorComponent.Infrastructure.Interfaces;
using SandevBlazorComponent.Infrastructure.Models;

namespace SandevBlazorComponent.Infrastructure.BusinessLogic;

public class InvisibleCaptchaService : IInvisibleCaptchaService
{
    private readonly IMemoryCache _cache;

    private const int ExpireSeconds = 60;
    private const int MinTimeSeconds = 2;
    private const int MinInteractions = 5;

    public InvisibleCaptchaService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string GenerateToken(string fingerprint)
    {
        var token = Guid.NewGuid().ToString();

        _cache.Set(token, new InvisibleCaptchaToken
        {
            CreatedAt = DateTime.UtcNow,
            Fingerprint = fingerprint
        }, TimeSpan.FromSeconds(ExpireSeconds));

        return token;
    }

    public bool ValidateToken(InvisibleCaptchaRequest req)
    {
        if (!_cache.TryGetValue(req.Token, out InvisibleCaptchaToken data))
            return false;

        // ⏱️ TIME CHECK
        var age = DateTime.UtcNow - data.CreatedAt;
        if (age.TotalSeconds < MinTimeSeconds)
        {
            _cache.Remove(req.Token);
            return false;
        }

        // 🧠 FINGERPRINT CHECK
        if (data.Fingerprint != req.Fingerprint)
        {
            _cache.Remove(req.Token);
            return false;
        }

        // 🖱️ INTERACTION CHECK
        if (req.Interactions.Count < MinInteractions)
        {
            _cache.Remove(req.Token);
            return false;
        }

        // 📈 MOUSE ANALYSIS
        if (!IsHumanMovement(req.Interactions))
        {
            _cache.Remove(req.Token);
            return false;
        }

        // ⏳ TIMING ANALYSIS
        if (!IsHumanTiming(req.Interactions))
        {
            _cache.Remove(req.Token);
            return false;
        }

        // 🍯 HONEYPOT
        if (!string.IsNullOrEmpty(req.Honeypot))
        {
            _cache.Remove(req.Token);
            return false;
        }

        _cache.Remove(req.Token);
        return true;
    }

    private bool IsHumanMovement(List<MousePoint> points)
    {
        int directionChanges = 0;

        for (int i = 2; i < points.Count; i++)
        {
            var dx1 = points[i - 1].X - points[i - 2].X;
            var dx2 = points[i].X - points[i - 1].X;

            if (Math.Sign(dx1) != Math.Sign(dx2))
                directionChanges++;
        }

        return directionChanges > 2; // human biasanya zig-zag
    }

    private bool IsHumanTiming(List<MousePoint> points)
    {
        var intervals = new List<double>();

        for (int i = 1; i < points.Count; i++)
        {
            intervals.Add((points[i].Time - points[i - 1].Time).TotalMilliseconds);
        }

        var variance = intervals.Max() - intervals.Min();

        return variance > 50; // bot biasanya stabil
    }
}
