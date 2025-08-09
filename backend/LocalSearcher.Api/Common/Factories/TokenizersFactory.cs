using LocalSearcher.Api.Common.Interfaces;
using LocalSearcher.Api.Common.Tokenizers;

namespace LocalSearcher.Api.Common.Factories;

public class TokenizersFactory :  ITokenizerFactory
{
    private readonly Dictionary<string, ITokenizer> _tokenizers = 
        new (StringComparer.OrdinalIgnoreCase)
        {
            [".md"] = new MarkdownTokenizer(),
            [".txt"] = new BareTextTokenizer(),
        };

    public bool TryGetTokenizer(string? extension, out ITokenizer? tokenizer)
    {
        return _tokenizers.TryGetValue(extension ?? string.Empty, out tokenizer);
    }
}