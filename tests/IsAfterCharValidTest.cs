namespace seeker.literaturetime.tests;

public class IsAfterCharValidTest
{
    [Theory]
    [InlineData("12:12", "12:12:12")]
    public void IsAfterCharValidTest1(string phrase, string line)
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsAfterCharValid(line, phrase, startIndex);

        Assert.False(result);
    }

    [Theory]
    [InlineData("12:12", "12:12 squint")]
    [InlineData("2:12", "2:12 squint")]
    public void IsAfterCharValidTest2(string phrase, string line)
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsAfterCharValid(line, phrase, startIndex);

        Assert.True(result);
    }
}
