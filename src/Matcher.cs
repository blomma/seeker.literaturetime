using System.Collections.Concurrent;
using System.Text;

namespace Seeker;

public record Match(int MatchType, Dictionary<string, string> Matches);

public static class Matcher
{
    private static readonly char[] Digits = ['1', '2', '3', '4', '5', '6', '7', '8', '9'];

    private const string Twenty = "twenty-";
    private const string Thirty = "thirty-";
    private const string Forty = "forty-";
    private const string Fifty = "fifty-";

    public static bool IsBeforeCharValid(string line, string phrase)
    {
        ReadOnlySpan<char> phraseSpan = phrase.AsSpan();
        ReadOnlySpan<char> lineSpan = line.AsSpan();
        ReadOnlySpan<char> digitsSpan = Digits.AsSpan();

        var startIndex = lineSpan.IndexOf(phraseSpan, StringComparison.OrdinalIgnoreCase);
        if (startIndex == -1)
        {
            return false;
        }

        // The match is not at the start of the line, so we check that
        if (startIndex > 0)
        {
            var phraseFirst = phraseSpan[..1];

            if (phraseFirst.ContainsAny(digitsSpan))
            {
                var beforeChar = lineSpan.Slice(startIndex - 1, 1);

                // Phrase is 12:12
                // Sequence is matched on is 12:12:12
                if (beforeChar.Equals(":".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                // Phrase is 2:12
                // Sequence is matched on is 12:12
                if (beforeChar.ContainsAny(digitsSpan))
                {
                    return false;
                }
            }

            // Phrase is five minutes past three
            // Sequence is matched on: forty-five minutes past three
            if (startIndex >= 7)
            {
                var beforeSpan = lineSpan.Slice(startIndex - 7, 7);
                if (
                    beforeSpan.Equals(Twenty.AsSpan(), StringComparison.OrdinalIgnoreCase)
                    || beforeSpan.Equals(Thirty.AsSpan(), StringComparison.OrdinalIgnoreCase)
                )
                {
                    return false;
                }
            }
            else if (startIndex >= 6)
            {
                var beforeSpan = lineSpan.Slice(startIndex - 6, 6);
                if (
                    beforeSpan.Equals(Forty.AsSpan(), StringComparison.OrdinalIgnoreCase)
                    || beforeSpan.Equals(Fifty.AsSpan(), StringComparison.OrdinalIgnoreCase)
                )
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool IsAfterCharValid(string line, string phrase)
    {
        ReadOnlySpan<char> phraseSpan = phrase.AsSpan();
        ReadOnlySpan<char> lineSpan = line.AsSpan();
        ReadOnlySpan<char> digitsSpan = Digits.AsSpan();

        var startIndex = lineSpan.IndexOf(phraseSpan, StringComparison.OrdinalIgnoreCase);
        var lastIndex = startIndex + phrase.Length - 1;

        // The match is not the last thing on the line, so we check that
        if (lastIndex < (line.Length - 1))
        {
            var phraseLast = phraseSpan.Slice(phraseSpan.Length - 1, 1);
            if (phraseLast.ContainsAny(digitsSpan))
            {
                var afterChar = lineSpan.Slice(lastIndex + 1, 1);

                // Phrase is 12:12
                // Sequence is matched on is 12:12:12
                if (afterChar.Equals(":".AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                // Phrase is 12:1
                // Sequence is matched on is 12:12
                if (afterChar.ContainsAny(digitsSpan))
                {
                    return false;
                }
            }
        }

        return true;
    }

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
                if (!IsBeforeCharValid(line, phrase))
                {
                    continue;
                }

                if (!IsAfterCharValid(line, phrase))
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
                if (!IsBeforeCharValid(line, phrase))
                {
                    continue;
                }

                if (!IsAfterCharValid(line, phrase))
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
                if (!IsBeforeCharValid(line, phrase))
                {
                    continue;
                }

                if (!IsAfterCharValid(line, phrase))
                {
                    continue;
                }

                matches.Add(timePhraseOneOf.Key, phrase);

                break;
            }
        }

        return new Match(2, matches);
    }

    public static IEnumerable<LiteratureTime> GenerateQuotesFromMatches(
        ConcurrentDictionary<long, Match> matches,
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
