namespace LocalSearcher.Api.Models;

public class SearchResult
{
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public double Score { get; set; }
}