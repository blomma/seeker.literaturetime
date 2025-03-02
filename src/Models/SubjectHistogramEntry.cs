namespace seeker.literaturetime.models;

internal sealed class SubjectHistogramEntry
{
    public string Subject { get; set; } = string.Empty;

    public int Matches { get; set; }

    public int Count { get; set; }
}
