using LocalSearcher.Api.Common.Interfaces;
using System.Text;
using Porter2StemmerStandard;

namespace LocalSearcher.Api.Common.Tokenizers;

public class MarkdownTokenizer : ITokenizer
{
    private readonly EnglishPorter2Stemmer _stemmer = new();

    public IEnumerable<string> Tokenize(string content)
    {
        var sb = new StringBuilder();
        bool inCodeBlock = false;
        bool inFrontMatter = false;
        int i = 0;

        while (i < content.Length)
        {
            // Handle front matter: starts and ends with ---
            if (!inFrontMatter && StartsWith(content, i, "---") && (i == 0 || content[i - 1] == '\n'))
            {
                inFrontMatter = true;
                i += 3;
                continue;
            }
            if (inFrontMatter && StartsWith(content, i, "---") && content[i - 1] == '\n')
            {
                inFrontMatter = false;
                i += 3;
                continue;
            }
            if (inFrontMatter)
            {
                i++;
                continue;
            }

            // Code block: ```
            if (StartsWith(content, i, "```"))
            {
                inCodeBlock = !inCodeBlock;
                i += 3;
                continue;
            }
            if (inCodeBlock)
            {
                i++;
                continue;
            }

            // Token logic
            char c = content[i];
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

            i++;
        }

        // Last token
        if (sb.Length > 2)
        {
            yield return _stemmer.Stem(sb.ToString()).Value;
        }
    }

    private static bool StartsWith(string content, int index, string match)
    {
        if (index + match.Length > content.Length)
            return false;

        for (int j = 0; j < match.Length; j++)
        {
            if (content[index + j] != match[j])
                return false;
        }

        return true;
    }

    private static bool IsWordChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }
}
