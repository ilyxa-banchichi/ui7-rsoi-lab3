using System.Net;
using Microsoft.Extensions.Logging;

namespace Common.CircuitBreaker.States;

public class HalfOpenState : IState
{
    public State State => State.HalfOpen;

    private readonly ILogger _logger;
    private readonly int _threshold;

    private bool _hasFailedRequest = false;
    private int _requestsCount = 0;
    
    public HalfOpenState(ILogger logger, int threshold)
    {
        _logger = logger;
        _threshold = threshold;
    }

    public async Task<T?> ExecuteCommandAsync<T>(Func<Task<T>> command, Func<Task<T>>? fallback = null)
    {
        try
        {
            _requestsCount++;
            return await command();
        }
        catch (HttpRequestException ex)
        {
            _hasFailedRequest = ex.StatusCode == null || ex.StatusCode >= (HttpStatusCode)500;

            if (_hasFailedRequest && fallback != null)
                return await fallback();
            
            throw;
        }
    }

    public State TryDoTransition()
    {
        if (_hasFailedRequest)
            return State.Open;

        if (_requestsCount > _threshold)
            return State.Close;

        return State.None;
    }
}