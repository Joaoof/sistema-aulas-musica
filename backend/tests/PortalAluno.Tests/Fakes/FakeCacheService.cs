using System.Collections.Concurrent;
using PortalAluno.Application.Common.Interfaces;

namespace PortalAluno.Tests.Fakes;

/// <summary>
/// Cache em memória que replica fielmente a semântica de cache-aside do Redis,
/// permitindo asserções de hit/miss e invalidação sem infraestrutura.
/// </summary>
public class FakeCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, object?> _store = new();

    public int FactoryCalls { get; private set; }

    public bool Contains(string key) => _store.ContainsKey(key);

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        => Task.FromResult(_store.TryGetValue(key, out var v) ? (T?)v : default);

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        _store[key] = value;
        return Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? ttl = null,
        CancellationToken ct = default)
    {
        if (_store.TryGetValue(key, out var cached) && cached is not null)
            return (T)cached;

        FactoryCalls++;
        var fresh = await factory(ct);
        if (fresh is not null)
            _store[key] = fresh;
        return fresh;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public void Seed(string key, object value) => _store[key] = value;
}
