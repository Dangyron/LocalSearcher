using LocalSearcher.Api.Common.Configs;
using LocalSearcher.Api.Models;

namespace LocalSearcher.Api.Common.Interfaces;

public interface ISearchService
{
    Task<List<SearchResult>> SearchAsync(SearchOptions searchOptions, CancellationToken cancellationToken = default);
}