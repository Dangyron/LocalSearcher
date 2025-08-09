namespace LocalSearcher.Api.Common.Configs;

public class ApplicationGLobalConfig
{
    public string RedisConnectionString { get; set; } = null!;
    public string SeqConnectionUrl { get; set; } = null!;
    public string LogsDirectoryPath { get; set; } = null!;
    public string SearchBaseDirectoryPath { get; set; } = null!;
}