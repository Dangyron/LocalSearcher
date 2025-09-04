namespace LocalSearcher.Api.Common.Configs;

public sealed class SearchOptions
{
    public string Query { get; set; } = null!;
    public int OutputNumber { get; set; }
}