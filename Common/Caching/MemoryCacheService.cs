using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace EstatePulse.Common.Caching;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ConcurrentDictionary<string, string?> _trackedKeys = new();
    private long _hits;
    private long _misses;

    public MemoryCacheService(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
    {
        _memoryCache = memoryCache;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool TryGet<T>(string key, out T? value)
    {
        if (_memoryCache.TryGetValue(key, out value) && value != null)
        {
            Interlocked.Increment(ref _hits);
            SetHeader("HIT");
            return true;
        }

        Interlocked.Increment(ref _misses);
        SetHeader("MISS");
        value = default;
        return false;
    }

    public void Set<T>(string key, T value, TimeSpan expiration, string? prefix = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        options.RegisterPostEvictionCallback((k, _, _, _) =>
        {
            if (k is string strKey)
            {
                _trackedKeys.TryRemove(strKey, out _);
            }
        });

        _memoryCache.Set(key, value, options);
        _trackedKeys[key] = prefix;
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
        _trackedKeys.TryRemove(key, out _);
    }

    public void RemoveByPrefix(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix)) return;

        var normalizedPrefix = prefix.Trim().ToLowerInvariant();
        var keysToRemove = _trackedKeys
            .Where(k => k.Key.ToLowerInvariant().StartsWith(normalizedPrefix) ||
                        (k.Value != null && k.Value.ToLowerInvariant().StartsWith(normalizedPrefix)))
            .Select(k => k.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            Remove(key);
        }
    }

    public void Clear()
    {
        foreach (var key in _trackedKeys.Keys.ToList())
        {
            Remove(key);
        }
    }

    public CacheStatsDto GetStats()
    {
        return new CacheStatsDto
        {
            Hits = Volatile.Read(ref _hits),
            Misses = Volatile.Read(ref _misses),
            TrackedKeyCount = _trackedKeys.Count,
            Keys = _trackedKeys.Keys.OrderBy(k => k).ToList()
        };
    }

    private void SetHeader(string value)
    {
        try
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext != null && !httpContext.Response.HasStarted)
            {
                httpContext.Response.Headers["X-Cache"] = value;
            }
        }
        catch
        {
            // Suppress mutation error if response pipeline has already begun streaming
        }
    }
}
