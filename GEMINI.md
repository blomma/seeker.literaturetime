# seeker.literaturetime

`seeker.literaturetime` is a .NET 10 console application designed to autonomously scan Project Gutenberg's vast library for literary quotes that mention specific times of day. This tool generates the core dataset used by the [Literature Time](https://literature.artsoftheinsane.com) website and the [Timely Quote](https://apps.apple.com/us/app/timely-quote/id6466886308) iOS application.

## Project Overview

The project operates as a high-performance scanner that:
1.  **Generates Time Phrases:** Exhaustively creates thousands of permutations of how time is described in literature (e.g., "quarter past noon", "At 12:00 am", "five minutes to midnight").
2.  **Scans Gutenberg Texts:** Iterates through a Project Gutenberg mirror, scanning millions of lines of text.
3.  **Matches and Validates:** Employs sophisticated matching logic to identify time mentions while filtering out false positives (like "forty-five" being partially matched as "five").
4.  **Extracts Context:** Intelligently identifies the surrounding sentences to ensure the extracted quote is complete and makes sense.
5.  **Groups by Time:** Saves findings as JSON files organized by the specific time of day they reference.

### Core Technologies
- **Runtime:** .NET 10.0
- **Parsing:** `CsvHelper` for Gutenberg catalog processing.
- **Performance:** `Microsoft.Extensions.ObjectPool` for efficient `StringBuilder` reuse and `Parallel.ForEach` for multi-threaded scanning.
- **Testing:** xUnit with `Theory` and `InlineData` for exhaustive validation of matching rules.

## Project Structure

- `src/Program.cs`: The orchestration layer and entry point.
- `src/AhoCorasick.cs`: High-performance string search using the Aho-Corasick algorithm.
- `src/Phrases.cs`: Logic for generating the vast dictionary of time-related search phrases.
- `src/Matcher.cs`: High-performance matching engine with character validation and context extraction.
- `src/Data.cs`: State persistence, subject exclusion lists, and JSON serialization configurations.
- `src/Models/`: Data structures for `CatalogEntry`, `LiteratureTimeEntry`, and `SubjectHistogramEntry`.
- `tests/`: Comprehensive unit test suite (xUnit) covering:
    - `AhoCorasickTest.cs`: Automaton validation.
    - `PhrasesTest.cs`: Time phrase generation.
    - `MatcherTest.cs`: Matching and context extraction logic.
    - `DataTest.cs`: State persistence and serialization.
    - `IsBeforeCharValidTest.cs` & `IsAfterCharValidTest.cs`: Fine-grained character validation rules.

## Building and Running

### Prerequisites
- .NET 10 SDK
- A Project Gutenberg mirror located at `../gutenberg` (relative to the project root).
- Gutenberg catalog CSV at `../gutenberg/cache/epub/feeds/pg_catalog.csv`.

### Commands
- **Build:** `dotnet build`
- **Run:** `dotnet run --project src/seeker.literaturetime.csproj`
- **Test:** `dotnet test`

## Development Conventions

- **Performance First:** Use `ReadOnlySpan<char>` and `SearchValues` for high-performance string operations. Use `ObjectPool<StringBuilder>` to minimize GC pressure during large-scale scans.
- **Testing Logic:** New matching rules or validation logic MUST be accompanied by unit tests in the `tests/` directory.
- **Internal Visibility:** The `Matcher` class uses `InternalsVisibleTo` for unit testing, allowing core logic to remain internal to the assembly while being fully testable.
- **Data Integrity:** The application maintains state in `fileDirectoryDone.json` and `subjectHistogram.json` to allow resuming interrupted scans. Always ensure `Data.PersistState` is called during long-running operations.
- **Subject Filtering:** The `Data.SubjectExlusions` list is used to skip non-literary or irrelevant genres (e.g., Bibles, cookbooks, technical manuals).
