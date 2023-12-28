using System.Collections.Concurrent;
using System.Text;

namespace Seeker;

public record Match(int MatchType, Dictionary<string, string> Matches);

public static class Matcher
{
    public static Match FindMatches(
        Dictionary<string, List<string>> timePhrasesOneOf,
        Dictionary<string, List<string>> timePhrasesGenericOneOf,
        Dictionary<string, List<string>> timePhrasesSuperGenericOneOf,
        string line
    )
    {
        var matches = new Dictionary<string, string>();
        foreach (var timePhraseOneOf in timePhrasesOneOf)
        {
            foreach (var phrase in timePhraseOneOf.Value)
            {
                if (!line.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                matches.Add(timePhraseOneOf.Key, phrase);
                break;
            }
        }

        if (matches.Count > 0)
        {
            return new Match(0, matches);
        }

        foreach (var timePhraseOneOf in timePhrasesGenericOneOf)
        {
            foreach (var phrase in timePhraseOneOf.Value)
            {
                if (!line.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                matches.Add(timePhraseOneOf.Key, phrase);
                break;
            }
        }

        if (matches.Count > 0)
        {
            return new Match(1, matches);
        }

        foreach (var timePhraseOneOf in timePhrasesSuperGenericOneOf)
        {
            foreach (var phrase in timePhraseOneOf.Value)
            {
                var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
                if (startIndex == -1)
                {
                    continue;
                }

                // The match is not at the start of the line, so we check that
                if (startIndex > 0)
                {
                    var beforeChar = line[startIndex - 1];

                    // Phrase is 12:12
                    // Sequence is matched on is 12:12:12
                    if (beforeChar == ':')
                    {
                        continue;
                    }

                    // Phrase is 2:12
                    // Sequence is matched on is 12:12
                    if (char.IsNumber(beforeChar))
                    {
                        continue;
                    }
                }

                // The match is not the last thing on the line, so we check that
                var lastIndex = startIndex + phrase.Length - 1;
                if (lastIndex < (line.Length - 1))
                {
                    var afterChar = line[lastIndex + 1];

                    // Phrase is 12:12
                    // Sequence is matched on is 12:12:12
                    if (afterChar == ':')
                    {
                        continue;
                    }

                    // Phrase is 2:12
                    // Sequence is matched on is 12:12
                    if (char.IsNumber(afterChar))
                    {
                        continue;
                    }
                }

                matches.Add(timePhraseOneOf.Key, phrase);
                break;
            }
        }

        return new Match(2, matches);
    }

    public static IEnumerable<LiteratureTime> GenerateQuotesFromMatches(
        ConcurrentDictionary<int, Match> matches,
        string[] lines,
        string title,
        string author,
        string gutenbergReference
    )
    {
        var literatureTimes = new List<LiteratureTime>();

        foreach (var (index, value) in matches)
        {
            var quoteString = new StringBuilder();

            var matchLine = lines[index].Trim();
            var endQuote = "";
            if (matchLine[0] == '“')
            {
                endQuote = "”";
            }
            if (matchLine[0] == '"')
            {
                endQuote = "\"";
            }

            if (!IsUpperCase(matchLine[0]) && string.IsNullOrEmpty(endQuote))
            {
                var beforeLines = new List<string>();
                var currentIndex = index;

                var noOfDotsTobeFound = 2;

                // Search backwards... sort of for Uppercase
                var emptyLines = 0;
                var loopCount = 0;
                while (loopCount < 8 && currentIndex > 0)
                {
                    currentIndex -= 1;
                    loopCount += 1;

                    var currentLine = lines[currentIndex].Trim();
                    if (string.IsNullOrEmpty(currentLine))
                    {
                        emptyLines += 1;
                        if (emptyLines > 0)
                        {
                            break;
                        }

                        continue;
                    }

                    var dotIndex = currentLine.LastIndexOf('.');

                    // Check for am/pm pattern
                    var patternFound = false;
                    if (dotIndex - 2 > 0)
                    {
                        var p = currentLine[dotIndex - 1].ToString().ToLowerInvariant();
                        var pp = currentLine[dotIndex - 2].ToString().ToLowerInvariant();

                        patternFound = p switch
                        {
                            "m" when pp == "." => true,
                            "d" when pp == "n" => true,
                            _ => patternFound
                        };
                    }

                    if (patternFound)
                    {
                        beforeLines.Add(currentLine);
                        continue;
                    }

                    if (dotIndex != -1)
                    {
                        noOfDotsTobeFound -= 1;
                        if (noOfDotsTobeFound == 0 || (noOfDotsTobeFound == 1 && loopCount >= 4))
                        {
                            currentLine = currentLine[(dotIndex + 1)..].Trim();
                            if (string.IsNullOrEmpty(currentLine))
                            {
                                break;
                            }

                            // Check for a quote directly after the . and if that is all that is on the line break
                            var currentLineFirstChar = currentLine[0];
                            if (currentLineFirstChar is '”' or '"')
                            {
                                currentLine = currentLine[1..].Trim();
                                if (string.IsNullOrEmpty(currentLine))
                                {
                                    break;
                                }
                            }

                            endQuote = currentLineFirstChar switch
                            {
                                '“' => "”",
                                '"' => "\"",
                                _ => endQuote
                            };

                            beforeLines.Add(currentLine);

                            break;
                        }
                    }

                    beforeLines.Add(currentLine);
                }

                beforeLines.Reverse();
                beforeLines.ForEach(l =>
                {
                    quoteString.Append(l);
                    quoteString.Append('\n');
                });
            }

            quoteString.Append(matchLine);
            quoteString.Append('\n');

            if (
                !matchLine.AsSpan().EndsWith(".", StringComparison.InvariantCulture)
                || !string.IsNullOrEmpty(endQuote)
            )
            {
                var currentIndex = index;

                var noOfDotsTobeFound = 2;

                // Search forwards... sort of for a .
                var length = lines.Length - 1;
                var emptyLines = 0;
                var loopCount = 0;
                while (loopCount < 8 && currentIndex < length)
                {
                    currentIndex += 1;
                    loopCount += 1;

                    var currentLine = lines[currentIndex].Trim();
                    if (string.IsNullOrEmpty(currentLine))
                    {
                        emptyLines += 1;
                        if (emptyLines > 0)
                        {
                            break;
                        }
                        continue;
                    }

                    if (loopCount >= 5)
                    {
                        endQuote = string.Empty;
                    }

                    var endQuoteIndex = !string.IsNullOrEmpty(endQuote)
                        ? currentLine.IndexOf(endQuote, StringComparison.OrdinalIgnoreCase)
                        : -1;

                    if (endQuoteIndex != -1)
                    {
                        endQuote = string.Empty;
                    }

                    var dotIndex = currentLine.LastIndexOf('.');

                    // Check for a.m./p.m. pattern
                    var patternFound = false;
                    if (dotIndex - 2 > 0)
                    {
                        var p = currentLine[dotIndex - 1].ToString().ToLowerInvariant();
                        var pp = currentLine[dotIndex - 2].ToString().ToLowerInvariant();

                        patternFound = p switch
                        {
                            "m" when pp == "." => true,
                            _ => patternFound
                        };
                    }

                    if (patternFound)
                    {
                        quoteString.Append(currentLine);
                        quoteString.Append('\n');

                        continue;
                    }

                    // ." || ".
                    if (dotIndex != -1 && endQuoteIndex != -1)
                    {
                        noOfDotsTobeFound -= 1;
                        if (noOfDotsTobeFound == 0 || (noOfDotsTobeFound == 1 && loopCount >= 4))
                        {
                            var endIndex = dotIndex < endQuoteIndex ? endQuoteIndex : dotIndex;
                            currentLine = currentLine[..(endIndex + 1)].Trim();
                            quoteString.Append(currentLine);
                            quoteString.Append('\n');
                            break;
                        }
                    }
                    else if (dotIndex != -1 && string.IsNullOrEmpty(endQuote))
                    {
                        noOfDotsTobeFound -= 1;
                        if (noOfDotsTobeFound == 0 || (noOfDotsTobeFound == 1 && loopCount >= 4))
                        {
                            currentLine = currentLine[..(dotIndex + 1)].Trim();
                            quoteString.Append(currentLine);
                            quoteString.Append('\n');
                            break;
                        }
                    }

                    quoteString.Append(currentLine);
                    quoteString.Append('\n');
                }
            }

            var result = quoteString.ToString();
            result = result.Trim();

            foreach (var m in value.Matches)
            {
                var literatureTime = new LiteratureTime(
                    m.Key,
                    m.Value,
                    result,
                    title,
                    author,
                    gutenbergReference,
                    value.MatchType
                );

                literatureTimes.Add(literatureTime);
            }
        }

        return literatureTimes;
    }

    private static bool IsUpperCase(char a)
    {
        const char upper = new();
        char.ToUpperInvariant(upper);

        return a == upper;
    }
}
