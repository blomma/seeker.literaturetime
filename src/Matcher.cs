using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.ObjectPool;
using seeker.literaturetime.models;

[assembly: InternalsVisibleTo("seeker.literaturetime.tests")]

namespace seeker.literaturetime;

internal static class Matcher
{
    private static readonly ObjectPool<StringBuilder> StringBuilderPool =
        new DefaultObjectPoolProvider().CreateStringBuilderPool(
            initialCapacity: 1,
            maximumRetainedCapacity: 10
        );

    private static readonly SearchValues<char> Digits = SearchValues.Create("0123456789");
    private static readonly SearchValues<char> Separators = SearchValues.Create(" ,.:;");

    private static ReadOnlySpan<char> Colon => ":";

    public static bool IsBeforeCharValid(
        ReadOnlySpan<char> line,
        ReadOnlySpan<char> phrase,
        int startIndex
    )
    {
        switch (startIndex)
        {
            case -1:
                return false;
            case 0:
                return true;
        }

        var beforeChar = line.Slice(startIndex - 1, 1);

        var phraseFirstChar = phrase[..1];
        if (phraseFirstChar.ContainsAny(Digits))
        {
            // Phrase is 12:12
            // Sequence is matched on is 12:12:12
            if (beforeChar.Equals(Colon, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Phrase is 2:12
            // Sequence is matched on is 12:12
            if (beforeChar.ContainsAny(Digits))
            {
                return false;
            }
        }

        return beforeChar.ContainsAny(Separators);
    }

    public static bool IsAfterCharValid(
        ReadOnlySpan<char> line,
        ReadOnlySpan<char> phrase,
        int startIndex
    )
    {
        var lastIndex = startIndex + phrase.Length - 1;

        // The match is the last thing on the line
        if (lastIndex >= line.Length - 1)
        {
            return true;
        }

        var afterChar = line.Slice(lastIndex + 1, 1);
        return afterChar.ContainsAny(Separators);
    }

    public static Dictionary<string, string> FindMatches(
        AhoCorasick combinedAutomaton,
        ReadOnlySpan<char> line
    )
    {
        var foundMatches = new List<(string TimeKey, string Phrase, int Priority)>();
        combinedAutomaton.Search(line, foundMatches);

        if (foundMatches.Count == 0)
        {
            return [];
        }

        // Apply priority logic (oneOf > generic > superGeneric)
        int minPriority = foundMatches.Min(m => m.Priority);

        var results = new Dictionary<string, string>();
        foreach (var match in foundMatches)
        {
            if (match.Priority == minPriority)
            {
                if (results.TryGetValue(match.TimeKey, out var existingPhrase))
                {
                    if (match.Phrase.Length > existingPhrase.Length)
                    {
                        results[match.TimeKey] = match.Phrase;
                    }
                }
                else
                {
                    results.Add(match.TimeKey, match.Phrase);
                }
            }
        }

        return results;
    }

    public static IEnumerable<LiteratureTimeEntry> GenerateQuotesFromMatches(
        ConcurrentDictionary<long, Dictionary<string, string>> matches,
        string[] lines,
        string title,
        string author,
        string gutenbergReference
    )
    {
        var literatureTimes = new List<LiteratureTimeEntry>();

        foreach (var (index, value) in matches)
        {
            var quoteString = StringBuilderPool.Get();

            var matchLine = lines[index].Trim();
            var matchLineFirst = matchLine[0];

            var endQuote = matchLineFirst switch
            {
                '“' => "”",
                '"' => "\"",
                _ => "",
            };

            if (!char.IsUpper(matchLineFirst) && string.IsNullOrEmpty(endQuote))
            {
                var beforeLines = new List<string>();
                var currentIndex = index;

                var noOfDotsTobeFound = 2;

                // Search backwards...
                var loopCount = 0;
                while (loopCount < 8 && currentIndex > 0)
                {
                    currentIndex -= 1;
                    loopCount += 1;

                    var currentLine = lines[currentIndex].Trim();
                    if (string.IsNullOrEmpty(currentLine))
                    {
                        break;
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
                            _ => patternFound,
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
                        if (noOfDotsTobeFound == 0 || noOfDotsTobeFound == 1 && loopCount >= 4)
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
                                _ => endQuote,
                            };

                            beforeLines.Add(currentLine);

                            break;
                        }
                    }

                    beforeLines.Add(currentLine);
                }

                beforeLines.Reverse();
                foreach (var beforeLine in beforeLines)
                {
                    _ = quoteString.Append(beforeLine);
                    _ = quoteString.Append('\n');
                }
            }

            _ = quoteString.Append(matchLine);
            _ = quoteString.Append('\n');

            if (!matchLine.EndsWith('.') || !string.IsNullOrEmpty(endQuote))
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
                            _ => patternFound,
                        };
                    }

                    if (patternFound)
                    {
                        _ = quoteString.Append(currentLine);
                        _ = quoteString.Append('\n');

                        continue;
                    }

                    // ." || ".
                    if (dotIndex != -1 && endQuoteIndex != -1)
                    {
                        noOfDotsTobeFound -= 1;
                        if (noOfDotsTobeFound == 0 || noOfDotsTobeFound == 1 && loopCount >= 4)
                        {
                            var endIndex = dotIndex < endQuoteIndex ? endQuoteIndex : dotIndex;
                            currentLine = currentLine[..(endIndex + 1)].Trim();
                            _ = quoteString.Append(currentLine);
                            _ = quoteString.Append('\n');
                            break;
                        }
                    }
                    else if (dotIndex != -1 && string.IsNullOrEmpty(endQuote))
                    {
                        noOfDotsTobeFound -= 1;
                        if (noOfDotsTobeFound == 0 || noOfDotsTobeFound == 1 && loopCount >= 4)
                        {
                            currentLine = currentLine[..(dotIndex + 1)].Trim();
                            _ = quoteString.Append(currentLine);
                            _ = quoteString.Append('\n');
                            break;
                        }
                    }

                    _ = quoteString.Append(currentLine);
                    _ = quoteString.Append('\n');
                }
            }

            var result = quoteString.ToString();
            result = result.Trim();

            StringBuilderPool.Return(quoteString);

            foreach (var m in value)
            {
                var literatureTime = new LiteratureTimeEntry(
                    m.Key,
                    m.Value,
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
}
