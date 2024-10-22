using System.Text.Json;
using Gateway.Services;
using StackExchange.Redis;

namespace Gateway.RequestQueueService;

public class RequestQueueService : IRequestQueueService
{
    private readonly IConnectionMultiplexer _redis;
    
    public RequestQueueService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task EnqueueRequestAsync(IRequestQueueUser service, HttpRequestMessage request)
    {
        var requestDto = HttpRequestDto.FromHttpRequestMessage(request);
        var db = _redis.GetDatabase();
        await db.ListRightPushAsync(service.Name, JsonSerializer.Serialize(requestDto));
    }
}