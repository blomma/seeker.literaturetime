using CsvHelper.Configuration.Attributes;

namespace seeker.literaturetime.models;

// Text#,Type,Issued,Title,Language,Authors,Subjects,LoCC,Bookshelves

internal sealed class CatalogEntry
{
    [Name("Text#")]
    public required string GutenbergReference { get; set; }

    [Name("Type")]
    public required string Type { get; set; }

    [Name("Issued")]
    public required string Issued { get; set; }

    [Name("Title")]
    public required string Title { get; set; }

    [Name("Language")]
    public required string Language { get; set; }

    [Name("Authors")]
    public required string Authors { get; set; }

    [Name("Subjects")]
    public required string Subjects { get; set; }

    [Name("LoCC")]
    public required string LoCC { get; set; }

    [Name("Bookshelves")]
    public required string Bookshelves { get; set; }
}
