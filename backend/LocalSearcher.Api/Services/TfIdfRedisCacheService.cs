using System.IO.Compression;
using System.Text.Json;
using LocalSearcher.Api.Common.Interfaces;
using LocalSearcher.Api.Models;
using LocalSearcher.Api.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace LocalSearcher.Api.Services;

public class TfIdfRedisCacheService(IDistributedCache cache, ILogger<TfIdfRedisCacheService> logger) : ITfIdfIndexStorageService
{
    public async Task StoreAsync(string key, TfIdfIndex index, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var buffer = JsonBufferCache.GetBuffer();

            await using (var writer = new Utf8JsonWriter(buffer))
                JsonSerializer.Serialize(writer, index);

            var compressed = Compress(buffer.WrittenSpan);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromHours(24)
            };

            await cache.SetAsync(key, compressed, options, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Redis set operation failed for key: {Key}", key);
            throw;
        }
    }

    public async Task<TfIdfIndex?> LoadAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var compressed = await cache.GetAsync(key, cancellationToken);
            if (compressed is null)
                return null;

            var decompressedBytes = DecompressToBytes(compressed);

            return JsonSerializer.Deserialize<TfIdfIndex>(decompressedBytes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Redis load operation failed for key: {Key}", key);
            throw;
        }
    }
    
    private static byte[] Compress(ReadOnlySpan<byte> input)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
            gzip.Write(input);
        return output.ToArray();
    }

    private static byte[] DecompressToBytes(byte[] input)
    {
        using var inputStream = new MemoryStream(input);
        using var gzip = new GZipStream(inputStream, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }
}