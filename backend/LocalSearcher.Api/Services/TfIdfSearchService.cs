using LocalSearcher.Api.Common.Configs;
using LocalSearcher.Api.Common.Interfaces;
using LocalSearcher.Api.Common.Tokenizers;
using LocalSearcher.Api.Models;
using static LocalSearcher.Api.Utils.TfIdfIndexCacheKeys;

namespace LocalSearcher.Api.Services;

public class TfIdfSearchService(ITfIdfIndexStorageService indexStorage) : ISearchService
{
    public async Task<List<SearchResult>> SearchAsync(SearchOptions searchOptions, CancellationToken cancellationToken = default)
    {
        var index = (await indexStorage.LoadAsync(SearchIndex, cancellationToken))!;
        
        var queryTerms = new BareTextTokenizer().Tokenize(searchOptions.Query).ToList();
        
        var scores = new Dictionary<string, double>();
        foreach (var (file, tf) in index.Documents)
        {
            var score = queryTerms.Sum(term => ComputeTf(term, tf) * ComputeIdf(term, index));

            scores[file] = score;
        }

        return scores.OrderByDescending(kv => kv.Value)
            .Take(10)
            .Select(kv => new SearchResult
            {
                FileName = Path.GetFileName(kv.Key),
                FilePath = kv.Key,
                Score = kv.Value
            })
            .ToList();
    }
    
    private static double ComputeTf(string term, Document document)
    {
        var n = document.TermsCountWithinDocument;
        var m = (double)document.TermFrequency.GetValueOrDefault(term, 0);

        return m / n;
    }
    
    private static double ComputeIdf(string term, TfIdfIndex data)
    {
        var n = data.Documents.Count;
        var m = (double)data.DocumentFrequency.GetValueOrDefault(term, 1);

        return Math.Log10(n / m);
    }
}