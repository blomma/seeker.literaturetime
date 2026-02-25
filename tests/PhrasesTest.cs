namespace seeker.literaturetime.tests;

public class PhrasesTest
{
    [Fact]
    public void GeneratePhrases_ShouldContainKeyPhrases()
    {
        // Act
        var (oneOf, generic, superGeneric) = Phrases.GeneratePhrases();

        // Assert
        Assert.NotEmpty(oneOf);
        Assert.NotEmpty(generic);
        Assert.NotEmpty(superGeneric);

        // Midnight
        Assert.Contains("At midnight", oneOf["00:00"]);
        Assert.Contains("It struck midnight", oneOf["00:00"]);

        // Noon
        Assert.Contains("At noon", oneOf["12:00"]);
        Assert.Contains("half past noon", oneOf["12:30"]);

        // O'clock
        Assert.Contains("one o'clock", oneOf["01:00"]);
        Assert.Contains("one o'clock", oneOf["13:00"]);

        // Morning/Afternoon/Evening/Night
        Assert.Contains("six in the morning", oneOf["06:00"]);
        Assert.Contains("three in the afternoon", oneOf["15:00"]);
        Assert.Contains("six in the evening", oneOf["18:00"]);
        Assert.Contains("nine at night", oneOf["21:00"]);

        // Quarter/Half
        Assert.Contains("quarter past one am", oneOf["01:15"]);
        Assert.Contains("quarter past one", generic["01:15"]);
        Assert.Contains("half past one", generic["01:30"]);
        Assert.Contains("quarter to two", generic["01:45"]);

        // Generic
        Assert.Contains("fifteen minutes past one", generic["01:15"]);
        Assert.Contains("15 minutes past 1", generic["01:15"]);

        // Super Generic
        Assert.Contains("1:15am", superGeneric["01:15"]);
        Assert.Contains("13:15h", superGeneric["13:15"]);
    }

    [Fact]
    public void GeneratePhrases_ShouldGenerateAllMinutes()
    {
        // Act
        var (oneOf, generic, superGeneric) = Phrases.GeneratePhrases();

        // Assert
        Assert.Equal(1440, oneOf.Count);
        Assert.Equal(1440, generic.Count);
        Assert.Equal(1440, superGeneric.Count);
    }
}
