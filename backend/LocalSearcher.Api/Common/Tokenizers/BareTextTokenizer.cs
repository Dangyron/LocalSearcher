using LocalSearcher.Api.Common.Interfaces;
using System.Text;
using Porter2StemmerStandard;

namespace LocalSearcher.Api.Common.Tokenizers;

public class BareTextTokenizer : ITokenizer
{
    private readonly EnglishPorter2Stemmer _stemmer = new();
    
    public IEnumerable<string> Tokenize(string content)
    {
        var sb = new StringBuilder();

        foreach (var c in content)
        {
            if (IsWordChar(c))
            {
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                if (sb.Length > 2)
                {
                    yield return _stemmer.Stem(sb.ToString()).Value;
                }
                sb.Clear();
            }
        }

        if (sb.Length > 2)
        {
            yield return _stemmer.Stem(sb.ToString()).Value;
        }
    }

    private static bool IsWordChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }
}