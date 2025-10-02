using DustyPig.OMDb.Models;
using DustyPig.REST;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DustyPig.OMDb;

public class Client
{
    /// <summary>
    /// As far as I can tell
    /// </summary>
    public const string API_VERSION = "1.0";
    public const string API_AS_OF_DATE = "08/31/2025";

    private const string API_BASE_ADDRESS = "https://www.omdbapi.com/";
    private const string IMG_BASE_ADDRESS = "https://img.omdbapi.com/";

    private static readonly HttpClient _httpClient = new();
    private readonly REST.Client _restClient;


    public Client() : this(null, null) { }

    public Client(HttpClient httpClient) : this(httpClient, null) { }

    public Client(ILogger<Client> logger) : this(null, logger) { }
    
    public Client(HttpClient? httpClient, ILogger<Client>? logger)
    {
        _restClient = new(httpClient ?? new(), logger) { BaseAddress = new(API_BASE_ADDRESS) };
    }

    


    public string? ApiKey { get; set; }

    public bool IncludeRawContentInResponse
    {
        get => _restClient.IncludeRawContentInResponse;
        set => _restClient.IncludeRawContentInResponse = value;
    }

    public bool AutoThrowIfError
    {
        get => _restClient.AutoThrowIfError;
        set => _restClient.AutoThrowIfError = value;
    }

    /// <summary>
    /// When an error occurs, how many times to retry the api call.
    /// <br />
    /// Default = 0
    /// </summary>
    /// <remarks>
    /// <para>
    /// There are 2 events that can trigger a retry:
    /// </para>
    /// <para>
    /// 1. There is an error connecting to the server (such as a network layer error).
    /// </para>
    /// <para>
    /// 2. The connection succeeded, but the server sent HttpStatusCode.TooManyRequests
    ///    (429). In this case, the client will attempt to get the RetryAfter header, and
    ///    if found, the delay will use that value. If not found, exponential backoff with
    ///    jitter will be used.
    /// </para>
    /// </remarks>
    public ushort RetryCount
    {
        get => _restClient.RetryCount;
        set => _restClient.RetryCount = value;
    }




    private Task<Response<T>> GetByIdInternalAsync<T>(string imdbId, bool fullPlot, CancellationToken cancellationToken)
    {
        string plotOpt = fullPlot ? "&plot=full" : string.Empty;
        return _restClient.GetAsync<T>($"?apikey={ApiKey}&i={imdbId}{plotOpt}", cancellationToken: cancellationToken);
    }


    public Task<Response<MovieInfo>> GetMovieByIdAsync(string imdbId, bool fullPlot = false, CancellationToken cancellationToken = default) =>
        GetByIdInternalAsync<MovieInfo>(imdbId, fullPlot, cancellationToken);


    public Task<Response<SeriesInfo>> GetSeriesByIdAsync(string imdbId, bool fullPlot = false, CancellationToken cancellationToken = default) =>
        GetByIdInternalAsync<SeriesInfo>(imdbId, fullPlot, cancellationToken);


    public Task<Response<EpisodeInfo>> GetEpisodeByIdAsync(string imdbId, bool fullPlot = false, CancellationToken cancellationToken = default) =>
        GetByIdInternalAsync<EpisodeInfo>(imdbId, fullPlot, cancellationToken);


    public Task<Response<SeasonInfo>> GetSeasonAsync(string seriesImdbId, int season, CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<SeasonInfo>($"?apikey={ApiKey}&i={seriesImdbId}&season={season}", cancellationToken: cancellationToken);


    public Task<Response<EpisodeInfo>> GetEpisodeAsync(string seriesId, int season, int episode, bool fullPlot, CancellationToken cancellationToken)
    {
        string plotOpt = fullPlot ? "&plot=full" : string.Empty;
        return _restClient.GetAsync<EpisodeInfo>($"?apikey={ApiKey}&i={seriesId}&season={season}&episode={episode}{plotOpt}", cancellationToken: cancellationToken);
    } 


    private Task<Response<SearchResults>> SearchInternalAsync(string mediaType, string title, int? year, int? page, CancellationToken cancellationToken)
    {
        string search = "s=" + Uri.EscapeDataString(title) + "&type=" + mediaType;
        if (year != null)
            search += $"&y={year}";
        if (page != null)
            search += $"&page={page}";
        return _restClient.GetAsync<SearchResults>($"?apikey={ApiKey}&{search}", cancellationToken: cancellationToken);
    }


    public Task<Response<SearchResults>> SearchForMovieAsync(string title, int? year = null, int? page = null, CancellationToken cancellationToken = default) =>
        SearchInternalAsync("movie", title, year, page, cancellationToken);


    public Task<Response<SearchResults>> SearchForSeriesAsync(string title, int? year = null, int? page = null, CancellationToken cancellationToken = default) =>
        SearchInternalAsync("series", title, year, page, cancellationToken);


    public string GetPosterUrl(string imdbId) => IMG_BASE_ADDRESS + $"?apikey={ApiKey}&i={imdbId}";
}
