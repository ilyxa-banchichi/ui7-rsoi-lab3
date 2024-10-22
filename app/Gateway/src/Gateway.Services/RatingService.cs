using System.Net;
using Common.CircuitBreaker;
using Common.Models.DTO;
using Microsoft.Extensions.Logging;

namespace Gateway.Services;

public class RatingService(
    IHttpClientFactory httpClientFactory, string baseUrl,
    ICircuitBreaker circuitBreaker, ILogger<RatingService> logger)
    : BaseHttpService(httpClientFactory, baseUrl, circuitBreaker, logger), IRatingService
{
    public async Task<UserRatingResponse?> GetUserRating(string xUserName)
    {
        return await circuitBreaker.ExecuteCommandAsync(async () =>
            {
                var method = $"/api/v1/rating";
                return await GetAsync<UserRatingResponse>(method,
                    new Dictionary<string, string>()
                    {
                        { "X-User-Name", xUserName }
                    });
            },
            fallback: async () => new UserRatingResponse() { Stars = 10001 });
    }

    public async Task<UserRatingResponse?> IncreaseRating(string xUserName)
    {
        var method = $"/api/v1/rating/increase";
        return await PatchAsync<UserRatingResponse>(method,
            headers: new Dictionary<string, string>()
            {
                { "X-User-Name", xUserName }
            });
    }

    public async Task<UserRatingResponse?> DecreaseRating(string xUserName)
    {
        var method = $"/api/v1/rating/decrease";
        return await PatchAsync<UserRatingResponse>(method,
            headers: new Dictionary<string, string>()
            {
                { "X-User-Name", xUserName }
            });
    }
}