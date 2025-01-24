namespace seeker.literaturetime.tests;

public class IsAfterCharValidTest
{
    [Theory]
    [InlineData("7.46 am", "and a density of 7.46 amperes, is one")]
    [InlineData("7.46 pm", "and a density of 7.46 pmperes, is one")]
    public void IsBeforeCharValidTestFalse(string phrase, string line)
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsAfterCharValid(line, phrase, startIndex);

        Assert.False(result);
    }

    [Theory]
    [InlineData("7.46 am", "and a density of 7.46 am,peres, is one")]
    [InlineData("7.46 pm", "and a density of 7.46 pm,peres, is one")]
    [InlineData("7.46 am", "and a density of 7.46 am peres, is one")]
    [InlineData("7.46 pm", "and a density of 7.46 pm peres, is one")]
    [InlineData("7.46 am", "and a density of 7.46 am.peres, is one")]
    [InlineData("7.46 pm", "and a density of 7.46 pm.peres, is one")]
    [InlineData("7.46 am", "and a density of 7.46 am:peres, is one")]
    [InlineData("7.46 pm", "and a density of 7.46 pm:peres, is one")]
    [InlineData("7.46 am", "and a density of 7.46 am;peres, is one")]
    [InlineData("7.46 pm", "and a density of 7.46 pm;peres, is one")]
    [InlineData("7.46 am", "and a density of 7.46 am")]
    [InlineData("7.46 pm", "and a density of 7.46 pm")]
    public void IsBeforeCharValidTestTrue(string phrase, string line)
    {
        var startIndex = line.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
        var result = Matcher.IsAfterCharValid(line, phrase, startIndex);

        Assert.True(result);
    }
}
