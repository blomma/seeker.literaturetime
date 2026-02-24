using System.Collections.Concurrent;
using System.Collections.Immutable;
using seeker.literaturetime.models;
using Xunit;

namespace seeker.literaturetime.tests;

public class MatcherTest
{
    [Fact]
    public void FindMatches_ShouldReturnCorrectMatches()
    {
        // Arrange
        var oneOf = new Dictionary<string, List<string>>
        {
            { "12:00", new List<string> { "midnight", "twelve o'clock" } },
            { "13:00", new List<string> { "one o'clock" } }
        }.ToImmutableDictionary();

        var generic = ImmutableDictionary<string, List<string>>.Empty;
        var superGeneric = ImmutableDictionary<string, List<string>>.Empty;

        var oneOfAutomaton = AhoCorasick.CreateAutomaton(oneOf);
        var genericAutomaton = AhoCorasick.CreateAutomaton(generic);
        var superGenericAutomaton = AhoCorasick.CreateAutomaton(superGeneric);

        var line = "It was twelve o'clock at night.";

        // Act
        var result = Matcher.FindMatches(oneOfAutomaton, genericAutomaton, superGenericAutomaton, line);

        // Assert
        Assert.Single(result);
        Assert.Equal("twelve o'clock", result["12:00"]);
    }

    [Fact]
    public void GenerateQuotesFromMatches_ShouldExtractCorrectContext()
    {
        // Arrange
        var matches = new ConcurrentDictionary<long, Dictionary<string, string>>();
        matches.TryAdd(1, new Dictionary<string, string> { { "12:00", "midnight" } });

        string[] lines = {
            "First sentence.",
            "It was midnight.",
            "Third sentence."
        };

        // Act
        var results = Matcher.GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref").ToList();

        // Assert
        Assert.Single(results);
        var quote = results[0];
        Assert.Equal("It was midnight.", quote.Quote);
        Assert.Equal("12:00", quote.Time);
    }

    [Fact]
    public void FindMatches_ShouldReturnGenericMatchesIfNoOneOf()
    {
        // Arrange
        var oneOf = ImmutableDictionary<string, List<string>>.Empty;
        var generic = new Dictionary<string, List<string>>
        {
            { "12:00", new List<string> { "past noon" } }
        }.ToImmutableDictionary();
        var superGeneric = ImmutableDictionary<string, List<string>>.Empty;

        var oneOfAutomaton = AhoCorasick.CreateAutomaton(oneOf);
        var genericAutomaton = AhoCorasick.CreateAutomaton(generic);
        var superGenericAutomaton = AhoCorasick.CreateAutomaton(superGeneric);

        var line = "It was five minutes past noon.";

        // Act
        var result = Matcher.FindMatches(oneOfAutomaton, genericAutomaton, superGenericAutomaton, line);

        // Assert
        Assert.Single(result);
        Assert.Equal("past noon", result["12:00"]);
    }

    [Fact]
    public void GenerateQuotesFromMatches_ShouldSearchBackwardsForContext()
    {
        // Arrange
        var matches = new ConcurrentDictionary<long, Dictionary<string, string>>();
        matches.TryAdd(2, new Dictionary<string, string> { { "12:00", "midnight" } });

        string[] lines = {
            "Sentence one. Start of context",
            "Sentence before",
            "and it was midnight."
        };

        // Act
        var results = Matcher.GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref").ToList();

        // Assert
        Assert.Single(results);
        var quote = results[0].Quote;
        Assert.Contains("Start of context", quote);
        Assert.Contains("Sentence before", quote);
        Assert.Contains("and it was midnight.", quote);
    }

    [Fact]
    public void GenerateQuotesFromMatches_ShouldSearchForwardsForContext()
    {
        // Arrange
        var matches = new ConcurrentDictionary<long, Dictionary<string, string>>();
        matches.TryAdd(0, new Dictionary<string, string> { { "12:00", "midnight" } });

        string[] lines = {
            "It was midnight",
            "but the story continued",
            "and then it ended."
        };

        // Act
        var results = Matcher.GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref").ToList();

        // Assert
        Assert.Single(results);
        var quote = results[0].Quote;
        Assert.Contains("It was midnight", quote);
        Assert.Contains("but the story continued", quote);
        Assert.Contains("and then it ended.", quote);
    }

    [Fact]
    public void GenerateQuotesFromMatches_ShouldHandleQuotes()
    {
        // Arrange
        var matches = new ConcurrentDictionary<long, Dictionary<string, string>>();
        matches.TryAdd(0, new Dictionary<string, string> { { "12:00", "midnight" } });

        string[] lines = {
            "“It was midnight",
            "and I was alone.”",
            "Said the narrator."
        };

        // Act
        var results = Matcher.GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref").ToList();

        // Assert
        Assert.Single(results);
        var quote = results[0].Quote;
        Assert.Contains("“It was midnight", quote);
        Assert.Contains("and I was alone.”", quote);
    }
}
