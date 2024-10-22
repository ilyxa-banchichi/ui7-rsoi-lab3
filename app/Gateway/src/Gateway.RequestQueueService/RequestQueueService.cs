using System.Text.Json;
using StackExchange.Redis;

namespace Gateway.RequestQueueService;

public interface IRequestQueueService
{
    Task EnqueueRequestAsync(string serviceName, HttpRequestMessage request);
}

public class RequestQueueService : IRequestQueueService
{
    private readonly IConnectionMultiplexer _redis;
    
    public RequestQueueService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task EnqueueRequestAsync(string serviceName, HttpRequestMessage request)
    {
        var requestDto = HttpRequestDto.FromHttpRequestMessage(request);
        var db = _redis.GetDatabase();
        await db.ListRightPushAsync(serviceName, JsonSerializer.Serialize(requestDto));
    }
}