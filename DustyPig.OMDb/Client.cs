using DustyPig.OMDb.Models;
using DustyPig.REST;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DustyPig.OMDb;

public class Client : IDisposable
{
    /// <summary>
    /// As far as I can tell
    /// </summary>
    public const string API_VERSION = "1.0";
    public const string API_AS_OF_DATE = "08/31/2025";

    private const string API_BASE_ADDRESS = "https://www.omdbapi.com/";
    private const string IMG_BASE_ADDRESS = "https://img.omdbapi.com/";

    private readonly REST.Client _restClient;


    /// <summary>
    /// Creates a configuration that uses its own internal <see cref="HttpClient"/>. When using this constructor, <see cref="Dispose"/> should be called.
    /// </summary>
    public Client()
    {
        _restClient = new() { BaseAddress = new(API_BASE_ADDRESS) };
    }


    /// <summary
    /// Creates a configurtion that uses a shared <see cref="HttpClient"/>
    /// </summary
    /// <param name="httpClient">The shared <see cref="HttpClient"/> this REST configuration should use. Calling dispose will not call dispose on the HttpClient</param>
    public Client(HttpClient httpClient)
    {
        _restClient = new(httpClient) { BaseAddress = new(API_BASE_ADDRESS) };
    }

    public void Dispose()
    {
        _restClient.Dispose();
        GC.SuppressFinalize(this);
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
    /// 2. The connection succeeded, but the server sent HttpStatusCode.TooManyRequests (429). 
    ///    In this case, the client will attempt to get the RetryAfter header, and if found, 
    ///    the delay will be the maximum of the header and the <see cref="RetryDelay"/>. 
    ///    Otherwise, the retry delay will just be <see cref="RetryDelay"/>.
    /// </para>
    /// </remarks>
    public int RetryCount
    {
        get => _restClient.RetryCount;
        set => _restClient.RetryCount = value;
    }

    /// <summary>
    /// Number of milliseconds between retries.
    /// <br />
    /// Default = 0
    /// </summary>
    public int RetryDelay
    {
        get => _restClient.RetryDelay;
        set => _restClient.RetryDelay = value;
    }

    /// <summary>
    /// Minimum number of milliseconds between api calls.
    /// <br />
    /// Default = 0
    /// </summary>
    public int Throttle
    {
        get => _restClient.Throttle;
        set => _restClient.Throttle = value;
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
