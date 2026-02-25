using System.Collections.Immutable;
using seeker.literaturetime.models;

namespace seeker.literaturetime.tests;

public class DataTest : IDisposable
{
    private readonly string _tempDir;

    public DataTest()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }
    }

    [Fact]
    public void PersistPhrases_ShouldWriteFiles()
    {
        // Arrange
        var oneOf = new Dictionary<string, List<string>>
        {
            { "00:00", ["midnight"] },
        }.ToImmutableDictionary();
        var generic = new Dictionary<string, List<string>>
        {
            { "00:00", ["past midnight"] },
        }.ToImmutableDictionary();
        var superGeneric = new Dictionary<string, List<string>>
        {
            { "00:00", ["12:00am"] },
        }.ToImmutableDictionary();

        // Act
        Data.PersistPhrases(oneOf, generic, superGeneric, _tempDir);

        // Assert
        Assert.True(File.Exists(Path.Combine(_tempDir, "timePhrasesOneOf.json")));
        Assert.True(File.Exists(Path.Combine(_tempDir, "timePhrasesGenericOneOf.json")));
        Assert.True(File.Exists(Path.Combine(_tempDir, "timePhrasesSuperGenericOneOf.json")));
    }

    [Fact]
    public void ReadAndPersistState_ShouldWork()
    {
        // Arrange
        var done = new List<string> { "dir1", "dir2" };
        var histogram = new Dictionary<string, SubjectHistogramEntry>
        {
            {
                "Subject1",
                new SubjectHistogramEntry
                {
                    Subject = "Subject1",
                    Count = 10,
                    Matches = 5,
                }
            },
        };

        // Act
        Data.PersistState(done, histogram, _tempDir);
        var (readDone, readHistogram) = Data.ReadState(_tempDir);

        // Assert
        Assert.Equal(done, readDone);
        Assert.Single(readHistogram);
        Assert.Equal(10, readHistogram["Subject1"].Count);
    }

    [Fact]
    public void ReadState_ShouldReturnEmpty_WhenFilesDoNotExist()
    {
        // Act
        var (readDone, readHistogram) = Data.ReadState(_tempDir);

        // Assert
        Assert.Empty(readDone);
        Assert.Empty(readHistogram);
    }
}
