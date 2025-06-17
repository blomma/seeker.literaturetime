namespace seeker.literaturetime.models;

internal sealed record LiteratureTimeEntry(
    string Time,
    string TimeQuote,
    string Quote,
    string Title,
    string Author,
    string GutenbergReference
);
