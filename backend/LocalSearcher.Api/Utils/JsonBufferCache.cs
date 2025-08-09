using System.Buffers;

namespace LocalSearcher.Api.Utils;

public static class JsonBufferCache
{
    [ThreadStatic]
    private static ArrayBufferWriter<byte>? _writer;

    public static ArrayBufferWriter<byte> GetBuffer()
    {
        _writer ??= new ArrayBufferWriter<byte>(64 * 1024);
        _writer.Clear();
        return _writer;
    }
}