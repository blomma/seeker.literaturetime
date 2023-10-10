using System.Text;

namespace Seeker;

public static class Matcher
{
    public static List<string> FindMatches(
        Dictionary<string, List<string>> timePhrasesOneOf,
        string line
    )
    {
        var matches = new List<string>();
        foreach (var timePhraseOneOf in timePhrasesOneOf)
        {
            foreach (var phrase in timePhraseOneOf.Value)
            {
                var startIndex = line.IndexOf(phrase, StringComparison.InvariantCultureIgnoreCase);
                if (startIndex != -1)
                {
                    if (startIndex == 0)
                    {
                        matches.Add($"{timePhraseOneOf.Key}|{phrase}");
                        break;
                    }
                    else
                    {
                        var beforeChar = line[startIndex - 1];
                        if (beforeChar == ' ')
                        {
                            matches.Add($"{timePhraseOneOf.Key}|{phrase}");
                            break;
                        }
                    }
                }
            }
        }

        return matches;
    }

    public static List<LiteratureTime> GenerateQuotesFromMatches(
        Dictionary<int, List<string>> matches,
        string[] lines,
        string title,
        string author,
        string gutenbergReference
    )
    {
        var literatureTimes = new List<LiteratureTime>();

        foreach (var match in matches)
        {
            var index = match.Key;
            var startLine = (index - 5) >= 0 ? (index - 5) : 0;
            var endLine = (index + 5) <= lines.Length - 1 ? (index + 5) : lines.Length - 1;

            var range = new Range(startLine, endLine);
            var quote = lines.AsSpan(range);

            var quoteString = new StringBuilder();
            foreach (var quoteLine in quote)
            {
                if (!string.IsNullOrEmpty(quoteLine))
                {
                    quoteString.Append(quoteLine);
                    quoteString.Append(' ');
                }
            }

            var result = quoteString.ToString();
            result = result.Trim();

            // if (!IsUpperCase(result.AsSpan(0, 1)))
            // {
            //     var firstDotIndex = result.IndexOf(".");
            //     if (firstDotIndex != -1)
            //     {
            //         result = result[(firstDotIndex + 1)..];
            //     }
            // }

            // var lastDotIndex = result.LastIndexOf(".");
            // if (lastDotIndex != -1)
            // {
            //     result = result[..(lastDotIndex + 1)];
            // }

            // result = result.Trim();

            foreach (var m in match.Value)
            {
                var matchValues = m.Split("|");
                var literatureTime = new LiteratureTime(
                    matchValues.First(),
                    matchValues.Last(),
                    result,
                    title,
                    author,
                    gutenbergReference
                );

                literatureTimes.Add(literatureTime);
            }
        }

        return literatureTimes;
    }

    public static bool IsUpperCase(ReadOnlySpan<char> a)
    {
        Span<char> upper = new();
        a.ToUpperInvariant(upper);

        return a == upper;
    }
}
