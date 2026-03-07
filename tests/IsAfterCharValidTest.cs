namespace seeker.literaturetime.tests;

public class IsAfterCharValidTest
{
    [Theory]
    [InlineData("7.46 am", "and a density of 7.46 amperes, is one")]
    [InlineData("7.46 pm", "and a density of 7.46 pmperes, is one")]
    [InlineData("about five Am", "chew up about five American MBAs for breakfast")]
    public void IsAfterCharValid_ShouldReturnFalse_WhenCharAfterIsInvalid(
        string phrase,
        string line
    )
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsAfterCharValid(line, phrase, startIndex);

        Assert.False(result);
    }

    [Theory]
    [InlineData("midnight", "midnight,it was")]
    [InlineData("midnight", "midnight.it was")]
    [InlineData("midnight", "midnight:it was")]
    [InlineData("midnight", "midnight;it was")]
    [InlineData("midnight", "midnight it was")]
    public void IsAfterCharValid_ShouldReturnTrue_WhenValidSeparators(string phrase, string line)
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsAfterCharValid(line, phrase, startIndex);

        Assert.True(result);
    }

    [Theory]
    [InlineData("midnight", "midnight-it was")]
    [InlineData("midnight", "midnight_it was")]
    public void IsAfterCharValid_ShouldReturnFalse_WhenInvalidSeparators(string phrase, string line)
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsAfterCharValid(line, phrase, startIndex);

        Assert.False(result);
    }
}
