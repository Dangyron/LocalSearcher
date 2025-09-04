using System.Diagnostics.CodeAnalysis;

namespace LocalSearcher.Api.Common.Interfaces;

public interface ITokenizerFactory
{
    string[] SupportedFileExtensions { get; }
    bool TryGetTokenizer(string extension, [MaybeNullWhen(false)] out ITokenizer tokenizer);
}