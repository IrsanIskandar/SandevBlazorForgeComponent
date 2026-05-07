using Microsoft.Extensions.Caching.Memory;
using SandevBlazorComponent.Infrastructure.Interfaces;
using SandevBlazorComponent.Infrastructure.Models;

namespace SandevBlazorComponent.Infrastructure.BusinessLogic;

public class MathCaptchaService : IMathCaptchaService
{
    private readonly IMemoryCache _cache;
    private readonly Random _rand = new();

    private const int ExpireMinutes = 5;
    private const int MaxAttempts = 5;

    public MathCaptchaService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public (string key, string question) Generate()
    {
        int a = _rand.Next(1, 10);
        int b = _rand.Next(1, 10);

        int op = _rand.Next(3); // 0:+ 1:- 2:×

        int answer;
        string symbol;

        switch (op)
        {
            case 0:
                answer = a + b;
                symbol = "+";
                break;

            case 1:
                // 🔥 pastikan tidak negatif
                if (a < b) (a, b) = (b, a);
                answer = a - b;
                symbol = "-";
                break;

            default:
                answer = a * b;
                symbol = "×";
                break;
        }

        var key = Guid.NewGuid().ToString();

        var entry = new MathCaptchaEntry
        {
            Answer = answer,
            ExpiredAt = DateTime.UtcNow.AddMinutes(ExpireMinutes),
            Attempts = 0
        };

        _cache.Set(key, entry, TimeSpan.FromMinutes(ExpireMinutes));

        string question = $"{a} {symbol} {b} = ?";

        return (key, question);
    }

    public bool Validate(string key, int input)
    {
        if (!_cache.TryGetValue(key, out MathCaptchaEntry entry))
            return false;

        if (DateTime.UtcNow > entry.ExpiredAt)
        {
            _cache.Remove(key);
            return false;
        }

        entry.Attempts++;

        if (entry.Attempts > MaxAttempts)
        {
            _cache.Remove(key);
            return false;
        }

        bool isValid = entry.Answer == input;

        if (isValid)
        {
            _cache.Remove(key); // 🔥 anti replay
            return true;
        }

        _cache.Set(key, entry, entry.ExpiredAt - DateTime.UtcNow);

        return false;
    }
}
