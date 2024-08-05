using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using CsvHelper;
using seeker.literaturetime;
using seeker.literaturetime.models;

var jsonSerializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

const string outputDirectory = "../quotes.literaturetime.temp";

const string gutPath = "/Users/blomma/Downloads/gutenberg";

Directory.CreateDirectory(outputDirectory);

var files = Directory.EnumerateFiles(gutPath, "*.txt", SearchOption.AllDirectories).ToList();
files = [.. files.OrderByDescending(s => s.Length)];

var (timePhrasesOneOf, timePhrasesGenericOneOf, timePhrasesSuperGenericOneOf) =
    Phrases.GeneratePhrases();

var timePhrasesOneOfJson = JsonSerializer.Serialize(timePhrasesOneOf, jsonSerializerOptions);
File.WriteAllText($"{outputDirectory}/timePhrasesOneOf.json", timePhrasesOneOfJson);

var timePhrasesGenericOneOfJson = JsonSerializer.Serialize(
    timePhrasesGenericOneOf,
    jsonSerializerOptions
);
File.WriteAllText($"{outputDirectory}/timePhrasesGenericOneOf.json", timePhrasesGenericOneOfJson);

var timePhrasesSuperGenericOneOfJson = JsonSerializer.Serialize(
    timePhrasesSuperGenericOneOf,
    jsonSerializerOptions
);
File.WriteAllText(
    $"{outputDirectory}/timePhrasesSuperGenericOneOf.json",
    timePhrasesSuperGenericOneOfJson
);

var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 3 };

var pgCatalogPath = $"{gutPath}/cache/epub/feeds/pg_catalog.csv";
List<CatalogEntry> catalogEntries = [];
using (var reader = new StreamReader(pgCatalogPath))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    catalogEntries = csv.GetRecords<CatalogEntry>().ToList();
}

List<string> subjectExlusions =
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
    "Political statistics -- Handbooks, manuals, etc."
];

List<string> fileDirectoryDone = [];
List<string> fileDirectoryExcluded = [];

if (File.Exists($"{outputDirectory}/fileDirectoryDone.json"))
{
    var content = File.ReadAllText($"{outputDirectory}/fileDirectoryDone.json");
    fileDirectoryDone = JsonSerializer.Deserialize<List<string>>(content) ?? [];
}

Console.CancelKeyPress += (s, e) =>
{
    var fileDirectoryDoneJson = JsonSerializer.Serialize(fileDirectoryDone, jsonSerializerOptions);
    File.WriteAllText($"{outputDirectory}/fileDirectoryDone.json", fileDirectoryDoneJson);

    var fileDirectoryExcludedJson = JsonSerializer.Serialize(
        fileDirectoryExcluded,
        jsonSerializerOptions
    );
    File.WriteAllText($"{outputDirectory}/fileDirectoryExcluded.json", fileDirectoryExcludedJson);
};

try
{
    var totalFiles = files.Count;
    var processedFiles = fileDirectoryDone.Count;
    var excludedFiles = fileDirectoryExcluded.Count;

    foreach (var file in files)
    {
        processedFiles += 1;

        var filePath = Path.GetDirectoryName(file);

        var fileDirectory = filePath?.Split(Path.DirectorySeparatorChar).LastOrDefault();
        if (fileDirectory == null)
        {
            excludedFiles += 1;

            continue;
        }

        fileDirectory = fileDirectory.ToLowerInvariant();
        if (fileDirectory == "old")
        {
            fileDirectoryExcluded.Add(fileDirectory);
            excludedFiles += 1;

            continue;
        }

        if (fileDirectoryDone.Contains(fileDirectory))
        {
            continue;
        }

        var fileToRead = Path.Combine(filePath!, $"{fileDirectory}.txt");

        // Prefer utf-8, files that end in -0
        // Otherwise prefer files that end in -8
        // else fallback to default
        var utf8File = Path.Combine(filePath!, $"{fileDirectory}-0.txt");
        var iso8859_1 = Path.Combine(filePath!, $"{fileDirectory}-8.txt");
        Encoding encoding = Encoding.ASCII;
        if (File.Exists(utf8File))
        {
            fileToRead = utf8File;
            encoding = Encoding.UTF8;
        }
        else if (File.Exists(iso8859_1))
        {
            fileToRead = iso8859_1;
            encoding = Encoding.Latin1;
        }

        if (!File.Exists(fileToRead))
        {
            fileDirectoryExcluded.Add(fileDirectory);
            excludedFiles += 1;

            continue;
        }

        var match = catalogEntries.Find(c =>
            c.GutenbergReference.Equals(fileDirectory, StringComparison.OrdinalIgnoreCase)
        );

        if (match == null)
        {
            fileDirectoryExcluded.Add(fileDirectory);
            excludedFiles += 1;

            continue;
        }

        bool subjectExclusionFound = subjectExlusions.Any(s =>
            match.Subjects.Contains(s, StringComparison.OrdinalIgnoreCase)
        );

        if (subjectExclusionFound)
        {
            fileDirectoryExcluded.Add(fileDirectory);
            excludedFiles += 1;

            continue;
        }

        if (!match.Language.Equals("en", StringComparison.OrdinalIgnoreCase))
        {
            fileDirectoryExcluded.Add(fileDirectory);
            excludedFiles += 1;

            continue;
        }

        Console.WriteLine(
            $"{fileToRead} - {excludedFiles}:{processedFiles}:{totalFiles}:{match.Subjects}"
        );

        var lines = File.ReadAllLines(fileToRead, encoding);

        var matches = new ConcurrentDictionary<long, Match>();
        Parallel.ForEach(
            lines,
            parallelOptions,
            (line, state, index) =>
            {
                var result = Matcher.FindMatches(
                    timePhrasesOneOf,
                    timePhrasesGenericOneOf,
                    timePhrasesSuperGenericOneOf,
                    line
                );

                if (result.Matches.Count > 0)
                {
                    matches.TryAdd(index, result);
                }
            }
        );

        var literatureTimesFromMatches = Matcher.GenerateQuotesFromMatches(
            matches,
            lines,
            match.Title,
            match.Authors,
            fileDirectory
        );

        var lookup = literatureTimesFromMatches.ToLookup(t => t.Time);
        foreach (var literatureTimesIndexGroup in lookup)
        {
            var jsonString = JsonSerializer.Serialize(
                literatureTimesIndexGroup.ToList(),
                jsonSerializerOptions
            );

            var directory =
                $"{outputDirectory}/{literatureTimesIndexGroup.Key.Replace(":", "_", StringComparison.InvariantCultureIgnoreCase)}";
            Directory.CreateDirectory(directory);

            File.WriteAllText($"{directory}/{fileDirectory}.json", jsonString);
        }

        fileDirectoryDone.Add(fileDirectory);
    }
}
finally
{
    var fileDirectoryDoneJson = JsonSerializer.Serialize(fileDirectoryDone, jsonSerializerOptions);
    File.WriteAllText($"{outputDirectory}/fileDirectoryDone.json", fileDirectoryDoneJson);

    var fileDirectoryExcludedJson = JsonSerializer.Serialize(
        fileDirectoryExcluded,
        jsonSerializerOptions
    );
    File.WriteAllText($"{outputDirectory}/fileDirectoryExcluded.json", fileDirectoryExcludedJson);
}
