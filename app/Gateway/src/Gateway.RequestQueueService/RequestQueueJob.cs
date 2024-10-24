using System.Text.Json;
using Gateway.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Gateway.RequestQueueService;

public class RequestQueueJob : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RequestQueueJob> _logger;
    private readonly IEnumerable<IRequestQueueUser> _services;

    public RequestQueueJob(
        IConnectionMultiplexer redis, 
        ILogger<RequestQueueJob> logger, 
        IEnumerable<IRequestQueueUser> services)
    {
        _redis = redis;
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var db = _redis.GetDatabase();
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"Start dequeue");

            foreach (var service in _services)
                await SendToService(db, service);
            
            _logger.LogInformation("End dequeue");
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task SendToService(IDatabase db, IRequestQueueUser service)
    {
        _logger.LogInformation($"Service {service.Name}. Count {db.ListLength(service.Name)}");
        
        var requestData = await db.ListLeftPopAsync(service.Name);
        if (!requestData.IsNullOrEmpty)
        {
            var requestDto = JsonSerializer.Deserialize<HttpRequestDto>(requestData);
            var request = HttpRequestDto.FromDto(requestDto);

            await service.SendRequestAsync(request);
        }
    }
}