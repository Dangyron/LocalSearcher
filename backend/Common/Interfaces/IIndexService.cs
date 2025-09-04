using LocalSearcher.Api.Models;

namespace LocalSearcher.Api.Common.Interfaces;

public interface IIndexService
{
    Task BuildIndexAsync(CancellationToken cancellationToken = default);
}