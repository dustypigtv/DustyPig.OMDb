using System.Collections.Generic;

namespace DustyPig.OMDb.Models;

public class SearchResults
{
    public List<SearchResultItem> Search { get; set; } = [];
    public string? TotalResults { get; set; }
    public string? Response { get; set; }
}
