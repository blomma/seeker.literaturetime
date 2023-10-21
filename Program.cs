using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using Seeker;

var jsonSerializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    // Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
};

Directory.CreateDirectory("/Users/blomma/Downloads/data/");

// string gutPath = "./test/";

// string gutPath = "/Users/blomma/Downloads/gutenberg";

var gutPath = "/Users/blomma/Downloads/test";
var files = Directory.EnumerateFiles(gutPath, "*.txt", SearchOption.AllDirectories);
var timePhrasesOneOf = Phrases.GeneratePhrases();

var timePhrasesOneOfJson = JsonSerializer.Serialize(timePhrasesOneOf, jsonSerializerOptions);
File.WriteAllText("/Users/blomma/Downloads/data/timePhrasesOneOf.json", timePhrasesOneOfJson);

var titlesExclusion = new List<string>
{
    "Deuterocanonical Books of the Bible",
    "The Book Of Mormon",
    "The Bible, Douay-Rheims, Old Testament--Part 2",
    "The Bible, Douay-Rheims, Old Testament--Part I",
    "The Bible, Douay-Rheims, New Testament",
    "The Declaration of Independence",
    "The Declaration of Independence"
};

List<LiteratureTime> literatureTimes = new();

var fileDirectoryDone = new List<string>();
if (File.Exists("/Users/blomma/Downloads/data/fileDirectoryDone.json"))
{
    var content = File.ReadAllText("/Users/blomma/Downloads/data/fileDirectoryDone.json");
    fileDirectoryDone = JsonSerializer.Deserialize<List<string>>(content);
}

var totalFiles = files.Count();
var processedFiles = 0;

var watch = Stopwatch.StartNew();
foreach (var file in files)
{
    processedFiles += 1;

    var filePath = Path.GetDirectoryName(file);

    var fileDirectory = filePath?.Split(Path.DirectorySeparatorChar).LastOrDefault();
    if (fileDirectory == null)
    {
        continue;
    }

    fileDirectory = fileDirectory.ToLowerInvariant();
    if (fileDirectory == "old")
    {
        continue;
    }

    if (fileDirectoryDone.Contains(fileDirectory))
    {
        Console.WriteLine($"Skipping (directory done) {file} - {processedFiles}:{totalFiles}");
        continue;
    }

    var fileToRead = Path.Combine(filePath!, $"{fileDirectory}.txt");

    // Prefer utf-8, files that end in -0
    // Otherwise prefer files that end in -8
    // else fallback to default
    var utf8File = Path.Combine(filePath!, $"{fileDirectory}-0.txt");
    var iso8859_1 = Path.Combine(filePath!, $"{fileDirectory}-8.txt");
    if (File.Exists(utf8File))
    {
        fileToRead = utf8File;
    }
    else if (File.Exists(iso8859_1))
    {
        fileToRead = iso8859_1;
    }

    if (!File.Exists(fileToRead))
    {
        Console.WriteLine($"Skipping (wrong format) {file} - {processedFiles}:{totalFiles}");
        continue;
    }

    Console.WriteLine($"{fileToRead} - {processedFiles}:{totalFiles}");

    var lines = File.ReadAllLines(fileToRead);
    var title = "";
    var author = "";
    var language = "";

    var startIndex = -1;
    for (var i = 0; i < lines.Length; i++)
    {
        var line = lines[i];
        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        if (line.StartsWith("Posting Date:", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        if (line.StartsWith("Release Date:", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        if (line.StartsWith("Language:", StringComparison.OrdinalIgnoreCase))
        {
            language = line.Replace("Language:", "").Trim();
            if (string.IsNullOrWhiteSpace(language))
            {
                break;
            }

            if (!language.Contains("English", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            continue;
        }

        if (line.StartsWith("[Most recently updated:", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        if (line.StartsWith("Title: ", StringComparison.OrdinalIgnoreCase))
        {
            title = line.Replace("Title:", "").Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                break;
            }

            if (titlesExclusion.Contains(title))
            {
                break;
            }

            continue;
        }

        if (line.StartsWith("Author: ", StringComparison.OrdinalIgnoreCase))
        {
            author = line.Replace("Author:", "").Trim();

            if (string.IsNullOrWhiteSpace(author))
            {
                break;
            }

            continue;
        }

        if (
            !string.IsNullOrWhiteSpace(title)
            && !string.IsNullOrWhiteSpace(author) & !string.IsNullOrWhiteSpace(language)
        )
        {
            startIndex = i;
            break;
        }
    }

    var matches = new ConcurrentDictionary<int, List<string>>();

    if (startIndex != -1)
    {
        Parallel.For(
            startIndex + 1,
            lines.Length,
            new ParallelOptions { MaxDegreeOfParallelism = 1 },
            index =>
            {
                var line = lines[index];
                var result = Matcher.FindMatches(timePhrasesOneOf, line);
                if (result.Count > 0)
                {
                    matches.TryAdd(index, result);
                }
            }
        );
    }

    var literatureTimesFromMatches = Matcher.GenerateQuotesFromMatches(
        matches,
        lines,
        title,
        author,
        fileDirectory
    );

    literatureTimes.AddRange(literatureTimesFromMatches);

    var lookup = literatureTimesFromMatches.ToLookup(t => t.Time);
    foreach (var literatureTimesIndexGroup in lookup)
    {
        var jsonString = JsonSerializer.Serialize(
            literatureTimesIndexGroup.ToList(),
            jsonSerializerOptions
        );

        var directory =
            $"/Users/blomma/Downloads/data/{literatureTimesIndexGroup.Key.Replace(":", "_")}";
        Directory.CreateDirectory(directory);

        File.WriteAllText($"{directory}/{fileDirectory}.json", jsonString);
    }

    fileDirectoryDone.Add(fileDirectory);
    var fileDirectoryDoneJson = JsonSerializer.Serialize(fileDirectoryDone, jsonSerializerOptions);
    File.WriteAllText("/Users/blomma/Downloads/data/fileDirectoryDone.json", fileDirectoryDoneJson);

    if (literatureTimes.Count > 200)
    {
        break;
    }
}

watch.Stop();

Console.WriteLine($"Time Taken : {watch.ElapsedMilliseconds} ms.");

public record LiteratureTime(
    string Time,
    string TimeQuote,
    string Quote,
    string Title,
    string Author,
    string GutenbergReference
);
