using System.Runtime.InteropServices;
using LocalSearcher.Api.Utils;

namespace LocalSearcher.Api.Models;

public class IndexStats(int totalFiles = 0)
{
    private readonly object _indexedLock = new();
    private readonly object _skippedLock = new();
    
    public Dictionary<string, int> IndexedFilesByExtension { get; init; } = new();
    public Dictionary<string, int> SkippedFilesByExtension { get; init; } = new();
    public int TotalFiles { get; set; } = totalFiles;

    public int IndexedFilesCount => IndexedFilesByExtension.Values.Sum();
    public int SkippedFilesCount => SkippedFilesByExtension.Values.Sum();
    
    public void RegisterIndexed(string fileExtension)
    {
        lock (_indexedLock)
        {
            IndexedFilesByExtension.AddOrIncrement(fileExtension, 1);
        }
    }
    
    public void RegisterSkipped(string fileExtension)
    {
        lock (_skippedLock)
        {
            SkippedFilesByExtension.AddOrIncrement(fileExtension, 1);
        }
    }
}