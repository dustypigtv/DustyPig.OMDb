﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DustyPig.OMDb.Models;

public class EpisodeInfo
{
    public string? ImdbId { get; set; }  
    public string? Title { get; set; }
    public string? Year { get; set; }
    public string? Rated { get; set; }
    public string? Released { get; set; }
    public string? Season { get; set; }
    public string? Episode { get; set; }
    public string? Runtime { get; set; }
    public string? Genre { get; set; }
    public string? Director { get; set; }
    public string? Writer { get; set; }
    public string? Actors { get; set; }
    public string? Plot { get; set; }
    public string? Language { get; set; }
    public string? Country { get; set; }
    public string? Awards { get; set; }
    public string? Poster { get; set; }
    public List<Rating> Ratings { get; } = [];
    public string? Metascore { get; set; }
    public string? ImdbRating { get; set; }
    public string? ImdbVotes { get; set; }
    public string? SeriesId { get; set; }
    public string? Type { get; set; }
    public string? Response { get; set; }
}
