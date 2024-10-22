using Common.CircuitBreaker;
using Common.Models.DTO;
using Gateway.RequestQueueService;
using Microsoft.Extensions.Logging;

namespace Gateway.Services;

public class RatingService : BaseHttpService, IRatingService, IRequestQueueUser
{
    public string Name => "rating";

    private readonly IRequestQueueService _queueService;
    
    public RatingService(
        IHttpClientFactory httpClientFactory, string baseUrl,
        ICircuitBreaker circuitBreaker, ILogger<RatingService> logger,
        IRequestQueueService queueService)
        : base(httpClientFactory, baseUrl, circuitBreaker, logger)
    {
        _queueService = queueService;
    }

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
            });
    }

    public async Task<UserRatingResponse?> IncreaseRating(string xUserName)
    {
        var method = $"/api/v1/rating/increase";
        var request = new HttpRequestMessage(HttpMethod.Patch, method);
        request.Headers.Add("X-User-Name", xUserName);

        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<UserRatingResponse>(request),
            fallback: async () =>
            {
                await _queueService.EnqueueRequestAsync(this, request);
                return null;
            });
    }

    public async Task<UserRatingResponse?> DecreaseRating(string xUserName)
    {
        var method = $"/api/v1/rating/decrease";
        var request = new HttpRequestMessage(HttpMethod.Patch, method);
        request.Headers.Add("X-User-Name", xUserName);

        return await circuitBreaker.ExecuteCommandAsync(
            async () => await SendAsync<UserRatingResponse>(request),
            fallback: async () =>
            {
                await _queueService.EnqueueRequestAsync(this, request);
                return null;
            });
    }
    
    public async Task SendRequestAsync(HttpRequestMessage request)
    {
        await circuitBreaker.ExecuteCommandAsync<object>(
            async () =>
            {
                await base.SendAsync(request);
                return null;
            },
            fallback: async () =>
            {
                await _queueService.EnqueueRequestAsync(this, request);
                return null;
            });
    }
}