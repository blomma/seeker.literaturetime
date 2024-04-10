namespace Seeker.Tests;

public class MatcherTest
{
    [Fact]
    public void IsBeforeCharValidTest1_ShouldBeFalse()
    {
        var phrase = "12:12";
        var line = "13:12:12";

        var result = Matcher.IsBeforeCharValid(line, phrase);

        Assert.False(result);
    }

    [Fact]
    public void IsAfterCharValidTest1_ShouldBeFalse()
    {
        var phrase = "12:12";
        var line = "12:12:12";

        var result = Matcher.IsAfterCharValid(line, phrase);

        Assert.False(result);
    }

    [Fact]
    public void IsBeforeCharValidTest1_ShouldBeTrue()
    {
        var phrase = "12:12";
        var line = "Squint 12:12";

        var result = Matcher.IsBeforeCharValid(line, phrase);

        Assert.True(result);
    }

    [Fact]
    public void IsAfterCharValidTest1_ShouldBeTrue()
    {
        var phrase = "12:12";
        var line = "12:12 squint";

        var result = Matcher.IsAfterCharValid(line, phrase);

        Assert.True(result);
    }

    [Fact]
    public void IsBeforeCharValidTest2_ShouldBeFalse()
    {
        var phrase = "2:12";
        var line = "12:12:12";

        var result = Matcher.IsBeforeCharValid(line, phrase);

        Assert.False(result);
    }

    [Fact]
    public void IsAfterCharValidTest2_ShouldBeFalse()
    {
        var phrase = "12:1";
        var line = "12:12:12";

        var result = Matcher.IsAfterCharValid(line, phrase);

        Assert.False(result);
    }

    [Fact]
    public void IsBeforeCharValidTest2_ShouldBeTrue()
    {
        var phrase = "2:12";
        var line = "as 2:12";

        var result = Matcher.IsBeforeCharValid(line, phrase);

        Assert.True(result);
    }

    [Fact]
    public void IsAfterCharValidTest2_ShouldBeTrue()
    {
        var phrase = "2:12";
        var line = "2:12 as";

        var result = Matcher.IsAfterCharValid(line, phrase);

        Assert.True(result);
    }

    [Fact]
    public void IsBeforeCharValidTest3_ShouldBeFalse()
    {
        var phrase = "five minutes past three";
        var line = "twenty-five minutes past three";

        var result = Matcher.IsBeforeCharValid(line, phrase);

        Assert.False(result);
    }

    [Theory]
    [InlineData("twenty-five minutes past three", "abacus twenty-five minutes past three")]
    [InlineData("ten minutes past three", "abacu ten minutes past three")]
    public void IsBeforeCharValidTest3_ShouldBeTrue(string phrase, string line)
    {
        var result = Matcher.IsBeforeCharValid(line, phrase);

        Assert.True(result);
    }
}
