namespace PortalAluno.Application.Common.Interfaces;

/// <summary>
/// Abstração de cache distribuído (implementada via Redis na Infrastructure).
/// A Application não conhece StackExchange.Redis nem IDistributedCache.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);

    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default);

    /// <summary>
    /// Retorna o valor do cache ou executa <paramref name="factory"/>, cacheando o resultado.
    /// </summary>
    Task<T> GetOrSetAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? ttl = null,
        CancellationToken ct = default);

    Task RemoveAsync(string key, CancellationToken ct = default);
}
