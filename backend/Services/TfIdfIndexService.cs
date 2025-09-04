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
    IOptions<ApplicationGLobalConfig> configOptions,
    ITokenizerFactory tokenizerFactory)
    : IIndexService, IHostedService, IDisposable
{
    private readonly ApplicationGLobalConfig _config = configOptions.Value;

    private FileSystemWatcher? _watcher;

    public async Task BuildIndexAsync(CancellationToken cancellationToken = default)
    {
        using var scope = logger.BeginScope("IndexBuild");
        var start = Stopwatch.GetTimestamp();
        logger.LogInformation("Starting index build at {Path}", _config.SearchBaseDirectoryPath);
        var stats = new IndexStats();
        var previousIndex = await redisService.LoadAsync(SearchIndex, cancellationToken);
        var files = Directory.EnumerateFiles(_config.SearchBaseDirectoryPath, "*.*", SearchOption.AllDirectories)
            .Where(file =>
            {
                stats.TotalFiles++;
                var ext = Path.GetExtension(file);
                if (!tokenizerFactory.SupportedFileExtensions.Contains(ext))
                {
                    stats.RegisterSkipped(ext);
                    return false;
                }

                if (previousIndex == null || !previousIndex.Documents.TryGetValue(file, out var document))
                    return true;
                
                if (File.GetLastWriteTime(file) > document.LastModified)
                    return true;

                stats.RegisterSkipped(ext);
                return false;

            }).ToArray();
        

        var df = new ConcurrentDictionary<string, int>();
        var tf = new ConcurrentDictionary<string, Document>();

        var tasks = files.Select(async file =>
        {
            try
            {
                var ext = Path.GetExtension(file);
                if (!tokenizerFactory.TryGetTokenizer(ext, out var tokenizer))
                    return;
                
                var content = await File.ReadAllTextAsync(file, cancellationToken);
                var terms = tokenizer.Tokenize(content);
                var tfDoc = new Dictionary<string, int>();
                foreach (var term in terms)
                {
                    tfDoc.AddOrIncrement(term, 0);
                }

                foreach (var term in tf.Keys)
                    df.AddOrUpdate(term, 1, (_, v) => v + 1);

                var doc = new Document(tfDoc, tfDoc.Values.Sum(), File.GetLastWriteTime(file));
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
        logger.LogInformation(
            "Stats: Indexed files count: {Indexed}, Skipped files count: {Skipped}, Total files count: {Total}",
            stats.IndexedFilesCount,
            stats.SkippedFilesCount,
            stats.TotalFiles);
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await BuildIndexAsync(cancellationToken);
        
        StartFileWatching();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _watcher?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _watcher?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void StartFileWatching()
    {
        if (!_config.FileWatcherEnabled)
            return;
        
        logger.LogInformation("Index service started with file watching");

        _watcher = new(configOptions.Value.SearchBaseDirectoryPath)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
            EnableRaisingEvents = true
        };

        // ISSUE: Deleted event ignored, TODO: will be implemented later.
        _watcher.Created += OnFileChanged;
        _watcher.Renamed += OnFileChanged;
        _watcher.Changed += OnFileChanged;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        logger.LogInformation("File change detected: {ChangeType} - {FilePath}", e.ChangeType, e.FullPath);
        _ = BuildIndexAsync(CancellationToken.None);
    }
}