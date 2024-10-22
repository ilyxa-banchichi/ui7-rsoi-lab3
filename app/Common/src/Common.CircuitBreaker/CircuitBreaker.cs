using Microsoft.Extensions.Logging;

namespace Common.CircuitBreaker;

public class CircuitBreaker<TService> : ICircuitBreaker<TService>
{
    private readonly ILogger<CircuitBreaker<TService>> _logger;
    private readonly ICircuitBreakerStateFactory _stateFactory;

    private IState _state;

    public CircuitBreaker(ILogger<CircuitBreaker<TService>> logger)
    {
        _logger = logger;
        _stateFactory = new CircuitBreakerStateFactory(logger);
        _state = _stateFactory.Create(State.Close);
    }

    public async Task<T?> ExecuteCommandAsync<T>(Func<Task<T>> command, Func<Task<T>>? fallback = null)
    {
        _logger.LogDebug($"CircuitBreaker in state {_state.State}");
        var newState = _state.TryDoTransition();
        if (newState != State.None)
        {
            _state = _stateFactory.Create(newState);
            _logger.LogDebug($"New state {newState}");
        }
        
        var result =  await _state.ExecuteCommandAsync(command, fallback);
        
        return result;
    }

    // private bool TryDoTransition()
    // {
    //     switch (_state.State)
    //     {
    //         case State.Close:
    //             var close = _state as CloseState;
    //             
    //             break;
    //         case State.Open:
    //             var open = _state as OpenState;
    //             
    //             break;
    //         case State.HalfOpen:
    //             var halfOpen = _state as HalfOpenState; ;
    //             if (halfOpen.)
    //                 _state = new HalfOpenState();
    //             
    //             break;
    //     }
    // }
}