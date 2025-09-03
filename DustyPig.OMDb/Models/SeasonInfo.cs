using System.Collections.Generic;

namespace DustyPig.OMDb.Models;

public class SeasonInfo
{
    public string? Title { get; set; }
    public string? Season { get; set; }
    public string? TotalSeasons { get; set; }
    public List<SeasonEpisodeItem>? Episodes { get; set; } = [];
    public string? Response { get; set; }
}
