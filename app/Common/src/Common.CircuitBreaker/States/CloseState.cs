using System.Net;
using Microsoft.Extensions.Logging;

namespace Common.CircuitBreaker.States;

public class CloseState : IState
{
    public State State => State.Close;
    
    private readonly ILogger _logger;
    private readonly int _threshold;
    
    private int _failureRate = 0;

    public CloseState(ILogger logger, int threshold)
    {
        _logger = logger;
        _threshold = threshold;
    }

    public async Task<T?> ExecuteCommandAsync<T>(Func<Task<T>> command, Func<Task<T>>? fallback = null)
    {
        try
        {
            return await command();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == null || ex.StatusCode >= (HttpStatusCode)500)
            {
                _failureRate++;
                
                if (fallback != null)
                    return await fallback();
            }
            
            throw;
        }
    }

    public State TryDoTransition()
    {
        _logger.LogDebug($"Failure rate {_failureRate}");
        if (_failureRate >= _threshold)
            return State.Open;

        return State.None;
    }
}