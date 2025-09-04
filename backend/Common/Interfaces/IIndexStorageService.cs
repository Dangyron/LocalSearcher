using LocalSearcher.Api.Models;

namespace LocalSearcher.Api.Common.Interfaces;

public interface IIndexStorageService<TIndex>
{
    Task StoreAsync(string key, TIndex index, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task<TIndex?> LoadAsync(string key, CancellationToken cancellationToken = default);
}

public interface ITfIdfIndexStorageService : IIndexStorageService<TfIdfIndex>;