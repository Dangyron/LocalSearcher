using System.Collections.Concurrent;
using System.Diagnostics;
using LocalSearcher.Api.Common.Configs;
using LocalSearcher.Api.Common.Interfaces;
using LocalSearcher.Api.Models;
using LocalSearcher.Api.Utils;
using Microsoft.Extensions.Options;
using static LocalSearcher.Api.Utils.TfIdfIndexCacheKeys;

namespace LocalSearcher.Api.Services;

public class TfIdfIndexService(
    ITfIdfIndexStorageService redisService,
    ILogger<TfIdfIndexService> logger,
    IOptions<SearchConfig> configOptions,
    ITokenizerFactory tokenizerFactory)
    : IIndexService
{
    private readonly SearchConfig _config = configOptions.Value;

    public async Task BuildIndexAsync(CancellationToken cancellationToken = default)
    {
        using var scope = logger.BeginScope("IndexBuild");
        var start = Stopwatch.GetTimestamp();
        logger.LogInformation("Starting index build at {Path}", _config.Directory);

        var files = Directory.GetFiles(_config.Directory, "*.*", SearchOption.AllDirectories);
        var stats = new IndexStats(files.Length);

        var df = new ConcurrentDictionary<string, int>();
        var tf = new ConcurrentDictionary<string, Document>();
        
        var tasks = files.Select(async file =>
        {
            try
            {
                var content = await File.ReadAllTextAsync(file, cancellationToken);
                var ext = Path.GetExtension(file);

                if (!tokenizerFactory.TryGetTokenizer(ext, out var tokenizer))
                {
                    stats.RegisterSkipped(ext);
                    return;
                }

                var terms = tokenizer!.Tokenize(content);
                var tfDoc = new Dictionary<string, int>();
                foreach (var term in terms)
                {
                    tfDoc.AddOrIncrement(term, 0);
                }

                foreach (var term in tf.Keys)
                    df.AddOrUpdate(term, 1, (_, v) => v + 1);

                var doc = new Document(tfDoc, tfDoc.Values.Sum());
                tf.TryAdd(file, doc);
                stats.RegisterIndexed(ext);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to index file: {File}", file);
            }
        });

        await Task.WhenAll(tasks);
        
        var index = new TfIdfIndex(
            tf.ToDictionary(k => k.Key, v => v.Value), 
            df.ToDictionary(k => k.Key, v => v.Value));
        
        await redisService.StoreAsync(SearchIndex, index, null, cancellationToken);
        
        logger.LogInformation("Index built in {Elapsed}ms", Stopwatch.GetElapsedTime(start).TotalMilliseconds);
        logger.LogInformation("Stats: Indexed files count: {Indexed}, Skipped files count: {Skipped}, Total files count: {Total}",
            stats.IndexedFilesCount,
            stats.SkippedFilesCount,
            stats.TotalFiles);
    }
}

