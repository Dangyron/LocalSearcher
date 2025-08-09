namespace LocalSearcher.Api.Common.Interfaces;

public interface ITokenizerFactory
{
    bool TryGetTokenizer(string extension, out ITokenizer? tokenizer);
}