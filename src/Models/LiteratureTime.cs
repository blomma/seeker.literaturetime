namespace seeker.literaturetime.models;

public record LiteratureTimeEntry(
    string Time,
    string TimeQuote,
    string Quote,
    string Title,
    string Author,
    string GutenbergReference,
    int MatchType
);
