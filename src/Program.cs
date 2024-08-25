using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.Json;
using CsvHelper;
using seeker.literaturetime;
using seeker.literaturetime.models;

const string outputDirectory = "../quotes.literaturetime.temp";

const string gutPath = "/Users/blomma/Downloads/gutenberg";

Directory.CreateDirectory(outputDirectory);

var files = Directory.EnumerateFiles(gutPath, "*.txt", SearchOption.AllDirectories).ToList();
files = [.. files.OrderByDescending(s => s.Length)];

var (timePhrasesOneOf, timePhrasesGenericOneOf, timePhrasesSuperGenericOneOf) =
    Phrases.GeneratePhrases();

Data.PersistPhrases(
    timePhrasesOneOf,
    timePhrasesGenericOneOf,
    timePhrasesSuperGenericOneOf,
    outputDirectory
);

var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 3 };

var pgCatalogPath = $"{gutPath}/cache/epub/feeds/pg_catalog.csv";
List<CatalogEntry> catalogEntries = [];
using (var reader = new StreamReader(pgCatalogPath))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    catalogEntries = csv.GetRecords<CatalogEntry>().ToList();
}

List<string> fileDirectoryExcluded = [];

var (fileDirectoryDone, subjectHistogram) = Data.ReadState(outputDirectory);

Console.CancelKeyPress += (s, e) =>
{
    Data.PersistState(fileDirectoryDone, subjectHistogram, outputDirectory);
};

try
{
    var totalFiles = files.Count;

    var matchCount = 0;
    foreach (var file in files)
    {
        var filePath = Path.GetDirectoryName(file);

        var fileDirectory = filePath?.Split(Path.DirectorySeparatorChar).LastOrDefault();
        if (fileDirectory == null)
        {
            continue;
        }

        fileDirectory = fileDirectory.ToLowerInvariant();
        if (fileDirectory == "old")
        {
            fileDirectoryExcluded.Add(fileDirectory);

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

            continue;
        }

        var match = catalogEntries.Find(c =>
            c.GutenbergReference.Equals(fileDirectory, StringComparison.OrdinalIgnoreCase)
        );

        if (match == null)
        {
            fileDirectoryExcluded.Add(fileDirectory);

            continue;
        }

        bool subjectExclusionFound = Data.SubjectExlusions.Any(s =>
            match.Subjects.Contains(s, StringComparison.OrdinalIgnoreCase)
        );

        if (subjectExclusionFound)
        {
            fileDirectoryExcluded.Add(fileDirectory);

            continue;
        }

        if (!match.Language.Equals("en", StringComparison.OrdinalIgnoreCase))
        {
            fileDirectoryExcluded.Add(fileDirectory);

            continue;
        }

        Console.WriteLine(
            $"{fileToRead} - {fileDirectoryExcluded.Count}:{fileDirectoryDone.Count}:{totalFiles}:{match.Subjects}"
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

        var subjects = match.Subjects.Split(";").Select(s => s.Trim());
        foreach (var subject in subjects)
        {
            if (subjectHistogram.TryGetValue(subject, out SubjectHistogramEntry? value))
            {
                value.Count++;
                value.Matches += matches.Count;
            }
            else
            {
                subjectHistogram[subject] = new SubjectHistogramEntry
                {
                    Subject = subject,
                    Count = 1,
                    Matches = matches.Count,
                };
            }
        }

        var literatureTimesFromMatches = Matcher
            .GenerateQuotesFromMatches(matches, lines, match.Title, match.Authors, fileDirectory)
            .ToList();

        var lookup = literatureTimesFromMatches.ToLookup(t => t.Time);
        foreach (var literatureTimesIndexGroup in lookup)
        {
            var jsonString = JsonSerializer.Serialize(
                literatureTimesIndexGroup.ToList(),
                Data.JsonSerializerOptions
            );

            var directory =
                $"{outputDirectory}/{literatureTimesIndexGroup.Key.Replace(":", "_", StringComparison.InvariantCultureIgnoreCase)}";
            Directory.CreateDirectory(directory);

            File.WriteAllText($"{directory}/{fileDirectory}.json", jsonString);
        }

        fileDirectoryDone.Add(fileDirectory);

        matchCount += literatureTimesFromMatches.Count > 0 ? 1 : 0;
        if (matchCount > 9)
        {
            Data.PersistState(fileDirectoryDone, subjectHistogram, outputDirectory);
            matchCount = 0;
        }
    }
}
finally
{
    Data.PersistState(fileDirectoryDone, subjectHistogram, outputDirectory);
}
