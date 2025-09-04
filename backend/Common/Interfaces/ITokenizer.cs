namespace LocalSearcher.Api.Common.Interfaces;

public interface ITokenizer
{
    IEnumerable<string> Tokenize(string content);
}