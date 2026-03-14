using System.Collections.Immutable;

namespace seeker.literaturetime.tests;

public class AhoCorasickTest
{
    private static AhoCorasick CreateAutomaton(ImmutableDictionary<string, List<string>> phrases)
    {
        return AhoCorasick.CreateCombinedAutomaton(phrases, [], []);
    }

    [Fact]
    public void Search_ShouldOnlyFindTheLongestMatchingPattern()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            { "12:00", ["midnight", "was almost midnight"] },
            { "06:00", ["morning", "In the morning"] },
        }.ToImmutableDictionary();
        var ac = CreateAutomaton(phrases);
        var text = "In the morning, it was almost midnight.";

        // Act
        var results = Matcher.FindMatches(ac, text);

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Equal("In the morning", results["06:00"]);
        Assert.Equal("was almost midnight", results["12:00"]);
    }

    [Fact]
    public void Search_ShouldFindMultiplePatterns()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            { "12:00", ["midnight"] },
            { "06:00", ["morning"] },
        }.ToImmutableDictionary();
        var ac = CreateAutomaton(phrases);
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();
        var text = "In the morning, it was almost midnight.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Equal(2, matches.Count);
        Assert.Contains(matches, m => m is { TimeKey: "06:00", Phrase: "morning" });
        Assert.Contains(matches, m => m is { TimeKey: "12:00", Phrase: "midnight" });
    }

    [Fact]
    public void Search_ShouldBeCaseInsensitive()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            { "12:00", ["MIDNIGHT"] },
        }.ToImmutableDictionary();
        var ac = CreateAutomaton(phrases);
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();
        var text = "It was midnight.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Single(matches);
        Assert.Equal("MIDNIGHT", matches[0].Phrase);
        Assert.Equal("12:00", matches[0].TimeKey);
    }

    [Fact]
    public void Search_ShouldRespectValidationLogic()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            { "05:00", ["five"] },
        }.ToImmutableDictionary();
        var ac = CreateAutomaton(phrases);
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();

        // "twenty-five" contains "five", but Matcher.IsBeforeCharValid should return false for "five" here.
        var text = "It was twenty-five minutes past.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Empty(matches);
    }

    [Theory]
    [InlineData("It was twentyfive minutes past.")]
    public void Search_ShouldNotMatchPartOfWord(string text)
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            { "05:00", ["five"] },
        }.ToImmutableDictionary();
        var ac = CreateAutomaton(phrases);
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Empty(matches);
    }

    [Theory]
    [InlineData("It was five minutes past.")]
    [InlineData("It was;five minutes past.")]
    public void Search_ShouldAllowBehindWordBoundary(string text)
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            { "05:00", ["five"] },
        }.ToImmutableDictionary();
        var ac = CreateAutomaton(phrases);
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Contains(matches, m => m.TimeKey == "05:00");
    }

    [Fact]
    public void Search_ShouldWorkWithSharedPrefixes()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            { "12:00", ["midnight"] },
            { "12:15", ["midnight snack"] },
        }.ToImmutableDictionary();
        var ac = CreateAutomaton(phrases);
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();
        var text = "I had a midnight snack.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Contains(matches, m => m.TimeKey == "12:00");
        Assert.Contains(matches, m => m.TimeKey == "12:15");
    }

    [Fact]
    public void CreateAutomaton_ShouldHandleEmptyInput()
    {
        // Arrange
        var phrases = ImmutableDictionary<string, List<string>>.Empty;

        // Act
        var ac = CreateAutomaton(phrases);
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();
        ac.Search("any text", matches);

        // Assert
        Assert.Empty(matches);
    }

    [Fact]
    public void Search_ShouldWorkWithUnicodeCharacters()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            { "12:00", ["mínùtë", "hóùr"] },
        }.ToImmutableDictionary();
        var ac = CreateAutomaton(phrases);
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();
        var text = "It was a mínùtë past the hóùr.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Equal(2, matches.Count);
        Assert.Contains(matches, m => m.Phrase == "mínùtë");
        Assert.Contains(matches, m => m.Phrase == "hóùr");
    }

    [Fact]
    public void Search_ShouldReturnCorrectPriorities()
    {
        // Arrange
        var oneOf = new Dictionary<string, List<string>>
        {
            { "12:00", ["midnight"] },
        }.ToImmutableDictionary();
        var generic = new Dictionary<string, List<string>>
        {
            { "12:01", ["one minute past"] },
        }.ToImmutableDictionary();
        var superGeneric = new Dictionary<string, List<string>>
        {
            { "12:05", ["12:05"] },
        }.ToImmutableDictionary();

        var ac = AhoCorasick.CreateCombinedAutomaton(oneOf, generic, superGeneric);
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();
        var text = "It was midnight, then one minute past, then 12:05.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Equal(3, matches.Count);
        Assert.Contains(matches, m => m is { TimeKey: "12:00", Priority: 1 });
        Assert.Contains(matches, m => m is { TimeKey: "12:01", Priority: 2 });
        Assert.Contains(matches, m => m is { TimeKey: "12:05", Priority: 3 });
    }

    [Fact]
    public void Search_ShouldHandleLargeNumberOfPatterns()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>();
        for (int i = 0; i < 1000; i++)
        {
            phrases.Add(i.ToString(), ["pattern" + i]);
        }
        var ac = CreateAutomaton(phrases.ToImmutableDictionary());
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();
        var text = "Searching for pattern123 and pattern789 and pattern999.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Equal(3, matches.Count);
        Assert.Contains(matches, m => m.TimeKey == "123");
        Assert.Contains(matches, m => m.TimeKey == "789");
        Assert.Contains(matches, m => m.TimeKey == "999");
    }

    [Fact]
    public void Search_ShouldHandleOverlappingPatternsOfDifferentPriorities()
    {
        // Arrange
        var oneOf = new Dictionary<string, List<string>>
        {
            { "12:00", ["midnight snack"] },
        }.ToImmutableDictionary();
        var generic = new Dictionary<string, List<string>>
        {
            { "12:00", ["midnight"] },
        }.ToImmutableDictionary();

        var ac = AhoCorasick.CreateCombinedAutomaton(
            oneOf,
            generic,
            ImmutableDictionary<string, List<string>>.Empty
        );
        var matches = new List<(string TimeKey, string Phrase, int Priority)>();
        var text = "I had a midnight snack.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Equal(2, matches.Count);
        Assert.Contains(matches, m => m is { Phrase: "midnight snack", Priority: 1 });
        Assert.Contains(matches, m => m is { Phrase: "midnight", Priority: 2 });
    }
}
