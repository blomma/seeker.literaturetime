using CsvHelper.Configuration.Attributes;

namespace seeker.literaturetime.models;

// Text#,Type,Issued,Title,Language,Authors,Subjects,LoCC,Bookshelves

public class CatalogEntry
{
    [Name("Text#")]
    public string GutenbergReference { get; set; } = string.Empty;

    [Name("Type")]
    public string Type { get; set; } = string.Empty;

    [Name("Issued")]
    public string Issued { get; set; } = string.Empty;

    [Name("Title")]
    public string Title { get; set; } = string.Empty;

    [Name("Language")]
    public string Language { get; set; } = string.Empty;

    [Name("Authors")]
    public string Authors { get; set; } = string.Empty;

    [Name("Subjects")]
    public string Subjects { get; set; } = string.Empty;

    [Name("LoCC")]
    public string LoCC { get; set; } = string.Empty;

    [Name("Bookshelves")]
    public string Bookshelves { get; set; } = string.Empty;
}
