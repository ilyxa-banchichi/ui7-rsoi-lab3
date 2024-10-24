using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.CircuitBreaker;

public class CircuitBreaker<TService> : ICircuitBreaker<TService>
{
    private readonly ILogger<CircuitBreaker<TService>> _logger;
    private readonly ICircuitBreakerStateFactory _stateFactory;

    private IState _state;

    public CircuitBreaker(
        ILogger<CircuitBreaker<TService>> logger,
        IOptions<CircuitBreakerConfig> config)
    {
        _logger = logger;
        _stateFactory = new CircuitBreakerStateFactory(logger, config.Value);
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
    
    // public async Task ExecuteCommandAsync(Func<Task> command, Func<Task>? fallback = null)
    // {
    //     _logger.LogDebug($"CircuitBreaker in state {_state.State}");
    //     var newState = _state.TryDoTransition();
    //     if (newState != State.None)
    //     {
    //         _state = _stateFactory.Create(newState);
    //         _logger.LogDebug($"New state {newState}");
    //     }
    //     
    //     await _state.ExecuteCommandAsync(command, fallback);
    // }
}