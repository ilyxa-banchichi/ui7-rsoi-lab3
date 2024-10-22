using System.Text.Json;
using Common.CircuitBreaker;
using Common.Models.DTO;
using Gateway.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Gateway.RequestQueueService;

public class RequestQueueJob : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RequestQueueJob> _logger;
    private readonly IRatingService _ratingService;

    public RequestQueueJob(
        IConnectionMultiplexer redis, 
        ILogger<RequestQueueJob> logger, 
        IRatingService ratingService)
    {
        _redis = redis;
        _logger = logger;
        _ratingService = ratingService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var db = _redis.GetDatabase();
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"Start dequeue");

            await SendToRatingService(db);
            
            _logger.LogInformation("End dequeue");
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task SendToRatingService(IDatabase db)
    {
        string serviceName = "rating";
        
        _logger.LogInformation($"Count {db.ListLength(serviceName)}");
        
        var requestData = await db.ListLeftPopAsync(serviceName);
        if (!requestData.IsNullOrEmpty)
        {
            var requestDto = JsonSerializer.Deserialize<HttpRequestDto>(requestData);
            var request = HttpRequestDto.FromDto(requestDto);

            await _ratingService.SendAsync(request);
        }
    }
}