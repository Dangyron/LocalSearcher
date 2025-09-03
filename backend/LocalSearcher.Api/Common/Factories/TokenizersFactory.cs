using System.Diagnostics.CodeAnalysis;
using LocalSearcher.Api.Common.Interfaces;
using LocalSearcher.Api.Common.Tokenizers;

namespace LocalSearcher.Api.Common.Factories;

public class TokenizersFactory : ITokenizerFactory
{
    private readonly ITokenizer[] _tokenizers =
    [
        new BareTextTokenizer(),
        new MarkdownTokenizer()
    ];

    public string[] SupportedFileExtensions { get; } = [".md", ".txt"];

    public bool TryGetTokenizer(string? extension, [MaybeNullWhen(false)] out ITokenizer tokenizer)
    {
        tokenizer = extension switch
        {
            ".txt" => _tokenizers[0],
            ".md" => _tokenizers[1],
            _ => null,
        };

        return tokenizer != null;
    }
}