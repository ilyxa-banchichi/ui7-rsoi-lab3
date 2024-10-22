using Gateway.Services;

namespace Gateway.RequestQueueService;

public interface IRequestQueueService
{
    Task EnqueueRequestAsync(IRequestQueueUser service, HttpRequestMessage request);
}