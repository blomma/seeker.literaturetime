# seeker.literaturetime

`seeker.literaturetime` is a high-performance .NET 10 console application designed to autonomously scan Project Gutenberg's vast library for literary quotes that mention specific times of day. 

This tool generates the core dataset used by:
- [Literature Time](https://literature.artsoftheinsane.com) - A website displaying literary quotes for the current time.
- [Timely Quote](https://apps.apple.com/us/app/timely-quote/id6466886308) - An iOS application.

## How it Works

1.  **Time Phrase Generation:** It creates thousands of permutations of how time is described in literature (e.g., "quarter past noon", "At 12:00 am", "five minutes to midnight").
2.  **Gutenberg Scanning:** It iterates through a Project Gutenberg mirror, scanning millions of lines of text using a high-performance Aho-Corasick matching engine.
3.  **Validation:** It filters out false positives (e.g., ensuring "five" isn't matched within "forty-five") and validates surrounding characters.
4.  **Context Extraction:** It intelligently identifies surrounding sentences to ensure the extracted quote is complete and makes sense.
5.  **Data Export:** Findings are saved as JSON files organized by the specific time of day they reference.

## Tech Stack

- **Runtime:** .NET 10.0
- **Performance:** `ReadOnlySpan<char>`, `SearchValues<char>`, `Microsoft.Extensions.ObjectPool`, and `Parallel.ForEach`.
- **Matching:** Custom flattened Aho-Corasick automaton for concurrent multi-pattern searching.
- **Testing:** xUnit with exhaustive validation of matching rules and context extraction.

## Getting Started

### Prerequisites
- .NET 10 SDK
- A Project Gutenberg mirror (text files) located at `../gutenberg`.
- Gutenberg catalog CSV at `../gutenberg/cache/epub/feeds/pg_catalog.csv`.

### Commands
- **Build:** `dotnet build`
- **Run:** `dotnet run --project src/seeker.literaturetime.csproj`
- **Test:** `dotnet test`

## Project Structure

- `src/`: Core logic and application entry point.
- `tests/`: Comprehensive test suite for matching, phrase generation, and context extraction.
- `Models/`: Data structures for catalog entries and literary quotes.
