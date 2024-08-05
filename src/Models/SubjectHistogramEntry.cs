namespace seeker.literaturetime.models;

public class SubjectHistogramEntry
{
    public string Subject { get; set; } = string.Empty;

    public int Matches { get; set; }

    public int Count { get; set; }
}
