using System.Collections.Immutable;
using System.Text.Encodings.Web;
using System.Text.Json;
using seeker.literaturetime.models;

namespace seeker.literaturetime;

internal static class Data
{
    public static readonly List<string> SubjectExlusions =
    [
        "Apocryphal",
        "Apologetics",
        "Baptism",
        "Bible",
        "Biblical",
        "Catechisms",
        "Catholic",
        "Christian",
        "Christianity",
        "Church",
        "Churches",
        "Clergy",
        "Cookbook",
        "Cooking",
        "Covenanters",
        "Genesis",
        "God",
        "Hymns",
        "Jesus Christ",
        "Judaism",
        "Lutheran",
        "Messiah",
        "Mission",
        "Missions",
        "Mormon",
        "New Testament",
        "Old Testament",
        "Orthodox",
        "Prayer",
        "Prayers",
        "Presbyterian",
        "Protestantism",
        "Psalms",
        "Reformation",
        "Religion",
        "Religious",
        "Revelation",
        "Salvation",
        "Sanctification",
        "Sermon",
        "Sermons",
        "Worship",
        "World politics -- Handbooks, manuals, etc.",
        "Geography -- Handbooks, manuals, etc.",
        "Political science -- Handbooks, manuals, etc.",
        "Political statistics -- Handbooks, manuals, etc.",
    ];

    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public static void PersistPhrases(
        ImmutableDictionary<string, List<string>> timePhrasesOneOf,
        ImmutableDictionary<string, List<string>> timePhrasesGenericOneOf,
        ImmutableDictionary<string, List<string>> timePhrasesSuperGenericOneOf,
        string outputDirectory
    )
    {
        var timePhrasesOneOfJson = JsonSerializer.Serialize(
            timePhrasesOneOf,
            JsonSerializerOptions
        );
        File.WriteAllText($"{outputDirectory}/timePhrasesOneOf.json", timePhrasesOneOfJson);

        var timePhrasesGenericOneOfJson = JsonSerializer.Serialize(
            timePhrasesGenericOneOf,
            JsonSerializerOptions
        );
        File.WriteAllText(
            $"{outputDirectory}/timePhrasesGenericOneOf.json",
            timePhrasesGenericOneOfJson
        );

        var timePhrasesSuperGenericOneOfJson = JsonSerializer.Serialize(
            timePhrasesSuperGenericOneOf,
            JsonSerializerOptions
        );
        File.WriteAllText(
            $"{outputDirectory}/timePhrasesSuperGenericOneOf.json",
            timePhrasesSuperGenericOneOfJson
        );
    }

    public static (
        List<string> fileDirectoryDone,
        Dictionary<string, SubjectHistogramEntry> subjectHistogram
    ) ReadState(string outputDirectory)
    {
        List<string> fileDirectoryDone = [];
        Dictionary<string, SubjectHistogramEntry> subjectHistogram = [];

        if (File.Exists($"{outputDirectory}/fileDirectoryDone.json"))
        {
            var content = File.ReadAllText($"{outputDirectory}/fileDirectoryDone.json");
            fileDirectoryDone = JsonSerializer.Deserialize<List<string>>(content) ?? [];
        }

        if (File.Exists($"{outputDirectory}/subjectHistogram.json"))
        {
            var content = File.ReadAllText($"{outputDirectory}/subjectHistogram.json");
            subjectHistogram =
                JsonSerializer.Deserialize<Dictionary<string, SubjectHistogramEntry>>(content)
                ?? [];
        }

        return (fileDirectoryDone, subjectHistogram);
    }

    public static void PersistState(
        List<string> fileDirectoryDone,
        Dictionary<string, SubjectHistogramEntry> subjectHistogram,
        string outputDirectory
    )
    {
        var fileDirectoryDoneJson = JsonSerializer.Serialize(
            fileDirectoryDone,
            JsonSerializerOptions
        );
        File.WriteAllText($"{outputDirectory}/fileDirectoryDone.json", fileDirectoryDoneJson);

        var subjectHistogramJson = JsonSerializer.Serialize(
            subjectHistogram,
            JsonSerializerOptions
        );
        File.WriteAllText($"{outputDirectory}/subjectHistogram.json", subjectHistogramJson);
    }
}
