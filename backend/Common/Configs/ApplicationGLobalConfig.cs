namespace LocalSearcher.Api.Common.Configs;

public class ApplicationGLobalConfig
{
    public required string RedisConnectionString { get; init; }
    public string? SeqConnectionUrl { get; init; }
    public required string LogsDirectoryPath { get; init; }
    public required string SearchBaseDirectoryPath { get; init; }
    public required bool FileWatcherEnabled { get; init; }
}