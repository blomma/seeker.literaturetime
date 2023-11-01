﻿using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using Seeker;

var jsonSerializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};

Directory.CreateDirectory("/Users/blomma/Downloads/data/");

// string gutPath = "./test/";

var gutPath = "/Users/blomma/Downloads/gutenberg";

// var gutPath = "/Users/blomma/Downloads/test";
var files = Directory.EnumerateFiles(gutPath, "*.txt", SearchOption.AllDirectories);
var (timePhrasesOneOf, timePhrasesGenericOneOf, timePhrasesSuperGenericOneOf) =
    Phrases.GeneratePhrases();

var timePhrasesOneOfJson = JsonSerializer.Serialize(timePhrasesOneOf, jsonSerializerOptions);
File.WriteAllText("/Users/blomma/Downloads/data/timePhrasesOneOf.json", timePhrasesOneOfJson);

var timePhrasesGenericOneOfJson = JsonSerializer.Serialize(
    timePhrasesGenericOneOf,
    jsonSerializerOptions
);
File.WriteAllText(
    "/Users/blomma/Downloads/data/timePhrasesGenericOneOf.json",
    timePhrasesGenericOneOfJson
);

var timePhrasesSuperGenericOneOfJson = JsonSerializer.Serialize(
    timePhrasesSuperGenericOneOf,
    jsonSerializerOptions
);
File.WriteAllText(
    "/Users/blomma/Downloads/data/timePhrasesSuperGenericOneOf.json",
    timePhrasesSuperGenericOneOfJson
);

var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 1 };

var titlesExclusion = new List<string>
{
    "Deuterocanonical Books of the Bible",
    "The Book Of Mormon",
    "The Bible, Douay-Rheims, Old Testament--Part 2",
    "The Bible, Douay-Rheims, Old Testament--Part I",
    "The Bible, Douay-Rheims, New Testament",
    "The Declaration of Independence",
    "The Declaration of Independence",
    "The Antiquities of the Jews",
    "Science and Health With Key to The Scriptures",
    "The Gospels in Four Part Harmony",
    "They Call Me Carpenter",
    "Introduction to Robert Browning",
    "Commentary on the Epistle to the Galatians",
    "The Bible, King James Version",
    "Weymouth New Testament in Modern Speech, Preface and Introductions",
    "An Explanation of Luther's Small Catechism",
    "The Confutatio Pontificia",
    "The Great Doctrines of the Bible",
    "A Treatise on Good Works"
};

var authorExclusion = new List<string>
{
    "Flavius Josephus",
    "Mary Baker Eddy",
    "J. Clontz",
    "John Bunyan",
    "Joseph Stump",
    "Rev. William Evans",
    "Henry F. Lutz",
    "E. B. Stewart",
    "Henry T. Sell",
    "Benedict of Spinoza"
};

List<LiteratureTime> literatureTimes = new();

List<string> fileDirectoryDone = new();
if (File.Exists("/Users/blomma/Downloads/data/fileDirectoryDone.json"))
{
    var content = File.ReadAllText("/Users/blomma/Downloads/data/fileDirectoryDone.json");
    fileDirectoryDone = JsonSerializer.Deserialize<List<string>>(content) ?? new();
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
            if (string.IsNullOrEmpty(language))
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
            if (string.IsNullOrEmpty(title))
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
            if (string.IsNullOrEmpty(author))
            {
                break;
            }

            if (authorExclusion.Contains(author))
            {
                break;
            }

            continue;
        }

        if (
            !string.IsNullOrEmpty(title)
            && !string.IsNullOrEmpty(author) & !string.IsNullOrEmpty(language)
        )
        {
            startIndex = i;
            break;
        }
    }

    var matches = new ConcurrentDictionary<int, Dictionary<string, string>>();

    if (startIndex != -1)
    {
        Parallel.For(
            startIndex + 1,
            lines.Length,
            parallelOptions,
            index =>
            {
                var line = lines[index];
                var result = Matcher.FindMatches(
                    timePhrasesOneOf,
                    timePhrasesGenericOneOf,
                    timePhrasesSuperGenericOneOf,
                    line
                );
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

    // if (literatureTimes.Count > 200)
    // {
    //     break;
    // }
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
