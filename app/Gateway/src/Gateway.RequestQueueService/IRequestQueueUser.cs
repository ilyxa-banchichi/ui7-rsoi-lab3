namespace Gateway.Services;

public interface IRequestQueueUser
{
    string Name { get; }
    Task SendRequestAsync(HttpRequestMessage request);
}