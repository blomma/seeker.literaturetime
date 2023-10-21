using System.Collections.Concurrent;
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
                var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
                if (startIndex == -1)
                    continue;

                if (startIndex == 0)
                {
                    matches.Add($"{timePhraseOneOf.Key}|{phrase}");
                    break;
                }

                var beforeChar = line[startIndex - 1];
                if (beforeChar != ' ')
                    continue;

                matches.Add($"{timePhraseOneOf.Key}|{phrase}");
                break;
            }
        }

        return matches;
    }

    public static List<LiteratureTime> GenerateQuotesFromMatches(
        ConcurrentDictionary<int, List<string>> matches,
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

                // var noOfDotsTobeFound = new Random().Next(1, 3);
                var noOfDotsTobeFound = 2;

                // Search backwards... sort of for Uppercase
                var emptyLines = 0;
                var loopCount = 0;
                while (loopCount < 8 && currentIndex > 0)
                {
                    currentIndex -= 1;
                    loopCount += 1;

                    var currentLine = lines[currentIndex].Trim();
                    if (string.IsNullOrWhiteSpace(currentLine))
                    {
                        emptyLines += 1;
                        if (emptyLines > 0)
                        {
                            break;
                        }

                        continue;
                    }

                    var dotIndex = currentLine.LastIndexOf(".", StringComparison.Ordinal);

                    // Check for am/pm pattern
                    var patternFound = false;
                    if (dotIndex - 2 > 0)
                    {
                        var p = currentLine[dotIndex - 1].ToString().ToLowerInvariant();
                        var pp = currentLine[dotIndex - 2].ToString().ToLowerInvariant();

                        switch (p)
                        {
                            case "m" when pp == ".":
                            case "d" when pp == "n":
                                patternFound = true;
                                break;
                        }
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
                            if (string.IsNullOrWhiteSpace(currentLine))
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
                || !string.IsNullOrWhiteSpace(endQuote)
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
                    if (string.IsNullOrWhiteSpace(currentLine))
                    {
                        emptyLines += 1;
                    }
                    else
                    {
                        if (loopCount >= 5)
                        {
                            endQuote = string.Empty;
                        }

                        var endQuoteIndex = !string.IsNullOrWhiteSpace(endQuote)
                            ? currentLine.IndexOf(endQuote, StringComparison.OrdinalIgnoreCase)
                            : -1;

                        if (endQuoteIndex != -1)
                        {
                            endQuote = string.Empty;
                        }

                        var dotIndex = currentLine.LastIndexOf(".", StringComparison.Ordinal);

                        // Check for am/pm pattern
                        var patternFound = false;
                        if (dotIndex != -1 && dotIndex + 2 <= currentLine.Length - 1)
                        {
                            var p = currentLine[dotIndex + 1];
                            var pp = currentLine[dotIndex + 2];

                            if (p.ToString().ToLowerInvariant() == "m" && pp.ToString() == ".")
                            {
                                patternFound = true;
                            }
                        }

                        if (dotIndex != -1 && dotIndex - 3 >= 0)
                        {
                            var p = currentLine[dotIndex - 1].ToString().ToLowerInvariant();
                            var pp = currentLine[dotIndex - 2].ToString().ToLowerInvariant();
                            var ppp = currentLine[dotIndex - 3].ToString().ToLowerInvariant();

                            if (
                                !string.IsNullOrEmpty(p)
                                && !string.IsNullOrEmpty(pp)
                                && string.IsNullOrWhiteSpace(ppp)
                            )
                            {
                                patternFound = true;
                            }
                        }

                        // ." || ".
                        if (dotIndex != -1 && endQuoteIndex != -1 && !patternFound)
                        {
                            noOfDotsTobeFound -= 1;
                            if (
                                noOfDotsTobeFound == 0 || (noOfDotsTobeFound == 1 && loopCount >= 4)
                            )
                            {
                                var endIndex = dotIndex < endQuoteIndex ? endQuoteIndex : dotIndex;
                                currentLine = currentLine[..(endIndex + 1)].Trim();
                                quoteString.Append(currentLine);
                                quoteString.Append('\n');
                                break;
                            }
                        }
                        else if (
                            dotIndex != -1 && string.IsNullOrWhiteSpace(endQuote) && !patternFound
                        )
                        {
                            noOfDotsTobeFound -= 1;
                            if (
                                noOfDotsTobeFound == 0 || (noOfDotsTobeFound == 1 && loopCount >= 4)
                            )
                            {
                                currentLine = currentLine[..(dotIndex + 1)].Trim();
                                quoteString.Append(currentLine);
                                quoteString.Append('\n');
                                break;
                            }
                        }
                    }

                    quoteString.Append(currentLine);
                    quoteString.Append('\n');

                    if (emptyLines > 1)
                    {
                        break;
                    }
                }
            }

            var result = quoteString.ToString();
            result = result.Trim();

            foreach (var m in value)
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

    private static bool IsUpperCase(char a)
    {
        const char upper = new();
        char.ToUpperInvariant(upper);

        return a == upper;
    }
}
