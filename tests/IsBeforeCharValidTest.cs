namespace seeker.literaturetime.tests;

public class IsBeforeCharValidTest
{
    [Theory]
    [InlineData("12:12", "13:12:12")]
    [InlineData("2:12", "12:12:12")]
    [InlineData("2:12", "12:12")]
    [InlineData(
        "0 o'clock",
        "10 o'clock to-morrow, everybody,' and then I would lay in bed all morning"
    )]
    public void IsBeforeCharValidTest1(string phrase, string line)
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsBeforeCharValid(line, phrase, startIndex);

        Assert.False(result);
    }

    [Theory]
    [InlineData("12:12", "Squint 12:12")]
    [InlineData("2:12", "Squint 2:12")]
    public void IsBeforeCharValidTest2(string phrase, string line)
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsBeforeCharValid(line, phrase, startIndex);

        Assert.True(result);
    }

    [Theory]
    [InlineData("five minutes past three", "twenty-five minutes past three")]
    [InlineData("five minutes past three", "thirty-five minutes past three")]
    [InlineData("five minutes past three", "forty-five minutes past three")]
    [InlineData("five minutes past three", "fifty-five minutes past three")]
    [InlineData("five minutes past three", "a single time twenty-five minutes past three")]
    [InlineData("five minutes past three", "a single time thirty-five minutes past three")]
    [InlineData("five minutes past three", "a single time forty-five minutes past three")]
    [InlineData("five minutes past three", "a single time fifty-five minutes past three")]
    public void IsBeforeCharValidTest5(string phrase, string line)
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsBeforeCharValid(line, phrase, startIndex);

        Assert.False(result);
    }

    [Theory]
    [InlineData("twenty-five minutes past three", "abacus twenty-five minutes past three")]
    [InlineData("ten minutes past three", "abacu ten minutes past three")]
    public void IsBeforeCharValidTest6(string phrase, string line)
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsBeforeCharValid(line, phrase, startIndex);

        Assert.True(result);
    }
}
