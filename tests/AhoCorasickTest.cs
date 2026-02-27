using System.Collections.Immutable;

namespace seeker.literaturetime.tests;

public class AhoCorasickTest
{
    [Fact]
    public void Search_ShouldOnlyFindTheLongestMatchingPattern()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            {
                "12:00",
                new List<string> { "midnight", "was almost midnight" }
            },
            {
                "06:00",
                new List<string> { "morning", "In the morning" }
            },
        }.ToImmutableDictionary();
        var ac = AhoCorasick.CreateAutomaton(phrases);
        var matches = new Dictionary<string, string>();
        var text = "In the morning, it was almost midnight.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Equal(2, matches.Count);
        Assert.Equal("In the morning", matches["06:00"]);
        Assert.Equal("was almost midnight", matches["12:00"]);
    }

    [Fact]
    public void Search_ShouldFindMultiplePatterns()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            {
                "12:00",
                new List<string> { "midnight" }
            },
            {
                "06:00",
                new List<string> { "morning" }
            },
        }.ToImmutableDictionary();
        var ac = AhoCorasick.CreateAutomaton(phrases);
        var matches = new Dictionary<string, string>();
        var text = "In the morning, it was almost midnight.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Equal(2, matches.Count);
        Assert.Equal("morning", matches["06:00"]);
        Assert.Equal("midnight", matches["12:00"]);
    }

    [Fact]
    public void Search_ShouldBeCaseInsensitive()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            {
                "12:00",
                new List<string> { "MIDNIGHT" }
            },
        }.ToImmutableDictionary();
        var ac = AhoCorasick.CreateAutomaton(phrases);
        var matches = new Dictionary<string, string>();
        var text = "It was midnight.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Single(matches);
        Assert.Equal("MIDNIGHT", matches["12:00"]);
    }

    [Fact]
    public void Search_ShouldRespectValidationLogic()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            {
                "05:00",
                new List<string> { "five" }
            },
        }.ToImmutableDictionary();
        var ac = AhoCorasick.CreateAutomaton(phrases);
        var matches = new Dictionary<string, string>();

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
            {
                "05:00",
                new List<string> { "five" }
            },
        }.ToImmutableDictionary();
        var ac = AhoCorasick.CreateAutomaton(phrases);
        var matches = new Dictionary<string, string>();

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
            {
                "05:00",
                new List<string> { "five" }
            },
        }.ToImmutableDictionary();
        var ac = AhoCorasick.CreateAutomaton(phrases);
        var matches = new Dictionary<string, string>();

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Contains("05:00", matches.Keys);
    }

    [Fact]
    public void Search_ShouldWorkWithSharedPrefixes()
    {
        // Arrange
        var phrases = new Dictionary<string, List<string>>
        {
            {
                "12:00",
                new List<string> { "midnight" }
            },
            {
                "12:15",
                new List<string> { "midnight snack" }
            },
        }.ToImmutableDictionary();
        var ac = AhoCorasick.CreateAutomaton(phrases);
        var matches = new Dictionary<string, string>();
        var text = "I had a midnight snack.";

        // Act
        ac.Search(text, matches);

        // Assert
        Assert.Contains("12:00", matches.Keys);
        Assert.Contains("12:15", matches.Keys);
    }

    [Fact]
    public void CreateAutomaton_ShouldHandleEmptyInput()
    {
        // Arrange
        var phrases = ImmutableDictionary<string, List<string>>.Empty;

        // Act
        var ac = AhoCorasick.CreateAutomaton(phrases);
        var matches = new Dictionary<string, string>();
        ac.Search("any text", matches);

        // Assert
        Assert.Empty(matches);
    }
}
