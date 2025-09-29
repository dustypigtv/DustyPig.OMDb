using System;
using System.Net.Http;

namespace DustyPig.OMDb.Tests;

internal static class ClientFactory
{
    const string ENV_KEY_VARIABLE = "OMDB_API_KEY";

    private static readonly HttpClient _client = new();

    static string GetEnvVar(string name)
    {
        string ret = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

        if (string.IsNullOrWhiteSpace(ret))
            ret = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);

        if (string.IsNullOrWhiteSpace(ret))
            ret = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);

        return ret;
    }

    /// <summary>
    /// Don't dispose
    /// </summary>
    public static Client SharedClient { get; } = new(_client)
    {
        Throttle = 1000, 
        ApiKey = GetEnvVar(ENV_KEY_VARIABLE),
        AutoThrowIfError = true,
        IncludeRawContentInResponse = true
    };
}
