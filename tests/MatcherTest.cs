using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace seeker.literaturetime.tests;

public class MatcherTest
{
    [Fact]
    public void FindMatches_ShouldReturnCorrectMatches()
    {
        // Arrange
        var oneOf = new Dictionary<string, List<string>>
        {
            {
                "12:00",
                new List<string> { "midnight", "twelve o'clock" }
            },
            {
                "13:00",
                new List<string> { "one o'clock" }
            },
        }.ToImmutableDictionary();

        var generic = ImmutableDictionary<string, List<string>>.Empty;
        var superGeneric = ImmutableDictionary<string, List<string>>.Empty;

        var oneOfAutomaton = AhoCorasick.CreateAutomaton(oneOf);
        var genericAutomaton = AhoCorasick.CreateAutomaton(generic);
        var superGenericAutomaton = AhoCorasick.CreateAutomaton(superGeneric);

        var line = "It was twelve o'clock at night.";

        // Act
        var result = Matcher.FindMatches(
            oneOfAutomaton,
            genericAutomaton,
            superGenericAutomaton,
            line
        );

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

        string[] lines = { "First sentence.", "It was midnight.", "Third sentence." };

        // Act
        var results = Matcher
            .GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref")
            .ToList();

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
            {
                "12:00",
                new List<string> { "past noon" }
            },
        }.ToImmutableDictionary();
        var superGeneric = ImmutableDictionary<string, List<string>>.Empty;

        var oneOfAutomaton = AhoCorasick.CreateAutomaton(oneOf);
        var genericAutomaton = AhoCorasick.CreateAutomaton(generic);
        var superGenericAutomaton = AhoCorasick.CreateAutomaton(superGeneric);

        var line = "It was five minutes past noon.";

        // Act
        var result = Matcher.FindMatches(
            oneOfAutomaton,
            genericAutomaton,
            superGenericAutomaton,
            line
        );

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

        string[] lines =
        {
            "Sentence one. Start of context",
            "Sentence before",
            "and it was midnight.",
        };

        // Act
        var results = Matcher
            .GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref")
            .ToList();

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

        string[] lines = { "It was midnight", "but the story continued", "and then it ended." };

        // Act
        var results = Matcher
            .GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref")
            .ToList();

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

        string[] lines = { "“It was midnight", "and I was alone.”", "Said the narrator." };

        // Act
        var results = Matcher
            .GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref")
            .ToList();

        // Assert
        Assert.Single(results);
        var quote = results[0].Quote;
        Assert.Contains("“It was midnight", quote);
        Assert.Contains("and I was alone.”", quote);
    }

    [Fact]
    public void GenerateQuotesFromMatches_ShouldHandleAmPmInContext()
    {
        // Arrange
        var matches = new ConcurrentDictionary<long, Dictionary<string, string>>();
        matches.TryAdd(1, new Dictionary<string, string> { { "12:00", "midnight" } });

        string[] lines = { "It was 11:59 p.m.", "suddenly it was midnight.", "The end." };

        // Act
        var results = Matcher
            .GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref")
            .ToList();

        // Assert
        Assert.Single(results);
        var quote = results[0].Quote;
        // p.m. should not be treated as end of sentence
        Assert.Contains("It was 11:59 p.m.", quote);
        Assert.Contains("suddenly it was midnight.", quote);
    }

    [Fact]
    public void GenerateQuotesFromMatches_ShouldBreakOnEmptyLineBackward()
    {
        // Arrange
        var matches = new ConcurrentDictionary<long, Dictionary<string, string>>();
        matches.TryAdd(2, new Dictionary<string, string> { { "12:00", "midnight" } });

        string[] lines = { "First sentence.", "", "it was midnight." };

        // Act
        var results = Matcher
            .GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref")
            .ToList();

        // Assert
        Assert.Single(results);
        var quote = results[0].Quote;
        Assert.DoesNotContain("First sentence.", quote);
        Assert.Equal("it was midnight.", quote);
    }

    [Fact]
    public void GenerateQuotesFromMatches_ShouldBreakOnEmptyLineForward()
    {
        // Arrange
        var matches = new ConcurrentDictionary<long, Dictionary<string, string>>();
        matches.TryAdd(0, new Dictionary<string, string> { { "12:00", "midnight" } });

        string[] lines = { "It was midnight.", "", "Last sentence." };

        // Act
        var results = Matcher
            .GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref")
            .ToList();

        // Assert
        Assert.Single(results);
        var quote = results[0].Quote;
        Assert.DoesNotContain("Last sentence.", quote);
        Assert.Equal("It was midnight.", quote);
    }

    [Fact]
    public void GenerateQuotesFromMatches_ShouldHandleNoOfDotsBackward()
    {
        // Arrange
        var matches = new ConcurrentDictionary<long, Dictionary<string, string>>();
        matches.TryAdd(2, new Dictionary<string, string> { { "12:00", "midnight" } });

        string[] lines = { "Sentence 1. Sentence 2", "Sentence 3.", "it was midnight." };

        // Act
        var results = Matcher
            .GenerateQuotesFromMatches(matches, lines, "Title", "Author", "ref")
            .ToList();

        // Assert
        Assert.Single(results);
        var quote = results[0].Quote;
        // Should find 2 dots backwards (one in line 2, one in line 1) and take part of line 1
        Assert.Contains("Sentence 2", quote);
        Assert.DoesNotContain("Sentence 1.", quote);
    }
}
