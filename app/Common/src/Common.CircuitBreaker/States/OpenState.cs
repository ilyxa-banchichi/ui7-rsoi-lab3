using System.Net;
using Microsoft.Extensions.Logging;

namespace Common.CircuitBreaker.States;

public class OpenState : IState
{
    public State State => State.Open;

    private readonly ILogger _logger;
    private readonly int _timeoutSeconds;
    private readonly long _enterTimeSeconds;
    
    public OpenState(ILogger logger, int timeoutSeconds)
    {
        _logger = logger;
        _timeoutSeconds = timeoutSeconds;
        _enterTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public async Task<T?> ExecuteCommandAsync<T>(Func<Task<T>> command, Func<Task<T>>? fallback = null)
    {
        if (fallback != null)
            return await fallback();

        throw new HttpRequestException(HttpRequestError.ConnectionError, "Blocked from CircuitBreaker", statusCode: HttpStatusCode.ServiceUnavailable);
    }

    public State TryDoTransition()
    {
        _logger.LogDebug($"Seconds in open state {DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _enterTimeSeconds}");
        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _enterTimeSeconds > _timeoutSeconds)
            return State.HalfOpen;

        return State.None;
    }
}