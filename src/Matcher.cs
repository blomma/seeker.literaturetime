using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Immutable;
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

    private static ReadOnlySpan<char> Twenty => "twenty-";
    private static ReadOnlySpan<char> Thirty => "thirty-";
    private static ReadOnlySpan<char> Forty => "forty-";
    private static ReadOnlySpan<char> Fifty => "fifty-";
    private static ReadOnlySpan<char> Colon => ":";
    private static ReadOnlySpan<char> AM => "am";
    private static ReadOnlySpan<char> PM => "pm";

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

        var phraseFirst = phrase[..1];
        if (phraseFirst.ContainsAny(Digits))
        {
            var beforeChar = line.Slice(startIndex - 1, 1);

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

        switch (startIndex)
        {
            // Phrase is five minutes past three
            // Sequence is matched on: forty-five minutes past three
            case >= 7:
            {
                var beforeSpan = line.Slice(startIndex - 7, 7);
                if (
                    beforeSpan.Equals(Twenty, StringComparison.OrdinalIgnoreCase)
                    || beforeSpan.Equals(Thirty, StringComparison.OrdinalIgnoreCase)
                )
                {
                    return false;
                }

                beforeSpan = line.Slice(startIndex - 6, 6);
                if (
                    beforeSpan.Equals(Forty, StringComparison.OrdinalIgnoreCase)
                    || beforeSpan.Equals(Fifty, StringComparison.OrdinalIgnoreCase)
                )
                {
                    return false;
                }
                break;
            }
            case >= 6:
            {
                var beforeSpan = line.Slice(startIndex - 6, 6);
                if (
                    beforeSpan.Equals(Forty, StringComparison.OrdinalIgnoreCase)
                    || beforeSpan.Equals(Fifty, StringComparison.OrdinalIgnoreCase)
                )
                {
                    return false;
                }
                break;
            }
        }

        return true;
    }

    public static bool IsAfterCharValid(
        ReadOnlySpan<char> line,
        ReadOnlySpan<char> phrase,
        int startIndex
    )
    {
        var lastIndex = startIndex + phrase.Length - 1;

        // The match is not the last thing on the line, so we check that
        if (lastIndex >= line.Length - 1)
        {
            return true;
        }

        var phraseLast = phrase.Slice(phrase.Length - 2, 2);
        if (
            !phraseLast.Equals(AM, StringComparison.OrdinalIgnoreCase)
            && !phraseLast.Equals(PM, StringComparison.OrdinalIgnoreCase)
        )
        {
            return true;
        }

        var afterChar = line.Slice(lastIndex + 1, 1);

        return afterChar.ContainsAny(Separators);
    }

    public static Dictionary<string, string> FindMatches(
        ImmutableDictionary<string, List<string>> timePhrasesOneOf,
        ImmutableDictionary<string, List<string>> timePhrasesGenericOneOf,
        ImmutableDictionary<string, List<string>> timePhrasesSuperGenericOneOf,
        string line
    )
    {
        var lineSpan = line.AsSpan();

        var matches = new Dictionary<string, string>();

        foreach (var phrases in timePhrasesOneOf)
        {
            foreach (var phrase in phrases.Value)
            {
                var startIndex = lineSpan.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);

                if (!IsBeforeCharValid(lineSpan, phrase, startIndex))
                {
                    continue;
                }

                if (!IsAfterCharValid(lineSpan, phrase, startIndex))
                {
                    continue;
                }

                matches.Add(phrases.Key, phrase);

                break;
            }
        }

        if (matches.Count > 0)
        {
            return matches;
        }

        foreach (var phrases in timePhrasesGenericOneOf)
        {
            foreach (var phrase in phrases.Value)
            {
                var startIndex = lineSpan.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
                if (!IsBeforeCharValid(lineSpan, phrase, startIndex))
                {
                    continue;
                }

                if (!IsAfterCharValid(lineSpan, phrase, startIndex))
                {
                    continue;
                }

                matches.Add(phrases.Key, phrase);

                break;
            }
        }

        if (matches.Count > 0)
        {
            return matches;
        }

        foreach (var phrases in timePhrasesSuperGenericOneOf)
        {
            foreach (var phrase in phrases.Value)
            {
                var startIndex = lineSpan.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
                if (!IsBeforeCharValid(lineSpan, phrase, startIndex))
                {
                    continue;
                }

                if (!IsAfterCharValid(lineSpan, phrase, startIndex))
                {
                    continue;
                }

                matches.Add(phrases.Key, phrase);

                break;
            }
        }

        return matches;
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
