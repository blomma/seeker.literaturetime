namespace seeker.literaturetime.models;

internal sealed class SubjectHistogramEntry
{
    public required string Subject { get; set; }

    public required int Matches { get; set; }

    public required int Count { get; set; }
}
