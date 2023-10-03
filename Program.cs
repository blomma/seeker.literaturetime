using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Seeker;

string gutPath = "/Users/blomma/Downloads/gutenberg";
var files = Directory.EnumerateFiles(gutPath, "*.txt", SearchOption.AllDirectories);
var (timePhrases, timePhrasesOneOf) = Phrases.GeneratePhrases();

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

var fileDirectoyDone = new List<string>();

foreach (var file in files)
{
    var filePath = Path.GetDirectoryName(file);
    if (filePath == null)
    {
        continue;
    }

    var fileDirectory = filePath.Split(Path.DirectorySeparatorChar).LastOrDefault();
    if (fileDirectory == null)
    {
        continue;
    }

    if (fileDirectory.ToLowerInvariant() == "old")
    {
        continue;
    }

    if (fileDirectoyDone.Contains(fileDirectory))
    {
        continue;
    }

    var fileToRead = Path.Combine(filePath!, $"{fileDirectory}.txt");

    // Prefer utf-8, files that end in -0
    // Otherwise prefer files that end in -8
    // else fallback to default
    var utf8File = Path.Combine(filePath, $"{fileDirectory}-0.txt");
    var iso8859_1 = Path.Combine(filePath, $"{fileDirectory}-8.txt");
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
        continue;
    }

    Console.WriteLine(fileToRead);

    var lines = File.ReadAllLines(fileToRead);
    var title = "";
    var author = "";

    var matches = new Dictionary<int, List<string>>();

    var taskList = new List<Task<(List<string>, int)>>();
    for (int i = 0; i < lines.Length; i++)
    {
        var line = lines[i];
        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        if (line.StartsWith("Posting Date:", StringComparison.InvariantCultureIgnoreCase))
        {
            continue;
        }

        if (line.StartsWith("Release Date:", StringComparison.InvariantCultureIgnoreCase))
        {
            continue;
        }

        if (
            line.StartsWith("Language:", StringComparison.InvariantCultureIgnoreCase)
            && !line.Contains("English", StringComparison.InvariantCultureIgnoreCase)
        )
        {
            continue;
        }

        if (line.StartsWith("[Most recently updated:", StringComparison.InvariantCultureIgnoreCase))
        {
            continue;
        }

        if (line.StartsWith("Title: ", StringComparison.InvariantCultureIgnoreCase))
        {
            title = line.Replace("Title:", "").Trim();
            if (titlesExclusion.Contains(title))
            {
                break;
            }

            continue;
        }

        if (line.StartsWith("Author: ", StringComparison.InvariantCultureIgnoreCase))
        {
            author = line.Replace("Author:", "").Trim();
            continue;
        }

        var index = i;
        taskList.Add(
            Task.Run(() =>
            {
                var result = Matcher.FindMatches(timePhrases, timePhrasesOneOf, line);
                return (result, index);
            })
        );
    }

    Task.WaitAll(taskList.ToArray());

    foreach (var t in taskList)
    {
        var (matchesInLine, index) = t.Result;
        if (matchesInLine.Count > 0)
        {
            matches.Add(index, matchesInLine);
        }
    }

    fileDirectoyDone.Add(fileDirectory);

    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
    {
        continue;
    }

    var literatureTimesFromMatches = Matcher.GenerateQuotesFromMatches(
        matches,
        lines,
        title,
        author,
        fileDirectory
    );

    literatureTimes.AddRange(literatureTimesFromMatches);

    if (literatureTimes.Count > 100)
    {
        break;
    }
}

var lookup = literatureTimes.ToLookup(t => t.Time);
var foundTimes = new List<string>();
foreach (IGrouping<string, LiteratureTime> literatureTimesIndexGroup in lookup)
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        // Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };
    string jsonString = JsonSerializer.Serialize(literatureTimesIndexGroup.ToList(), options);
    var file = $"data/{literatureTimesIndexGroup.Key.Replace(":", "_")}";
    File.WriteAllText(file, jsonString);
    foundTimes.Add(literatureTimesIndexGroup.Key);
}

var missing = timePhrases.Keys.Except(foundTimes).ToList();
File.WriteAllLines("missing", missing);

public record LiteratureTime(
    string Time,
    string TimeQuote,
    string Quote,
    string Title,
    string Author,
    string gutenbergReference
);
