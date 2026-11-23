namespace EstatePulse.Common.Caching;

public interface ICacheService
{
    bool TryGet<T>(string key, out T? value);
    void Set<T>(string key, T value, TimeSpan expiration, string? prefix = null);
    void Remove(string key);
    void RemoveByPrefix(string prefix);
    void Clear();
    CacheStatsDto GetStats();
}

public class CacheStatsDto
{
    public long Hits { get; set; }
    public long Misses { get; set; }
    public int TrackedKeyCount { get; set; }
    public IReadOnlyList<string> Keys { get; set; } = Array.Empty<string>();
}
