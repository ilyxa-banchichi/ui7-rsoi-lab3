using Common.CircuitBreaker.States;
using Microsoft.Extensions.Logging;

namespace Common.CircuitBreaker;

public class CircuitBreakerStateFactory : ICircuitBreakerStateFactory
{
    private readonly ILogger _logger;
    private readonly CircuitBreakerConfig _config;

    public CircuitBreakerStateFactory(ILogger logger, CircuitBreakerConfig config)
    {
        _logger = logger;
        _config = config;
    }

    public IState Create(State state)
    {
        return state switch
        {
            State.None => throw new ArgumentOutOfRangeException(nameof(state), state, null),
            State.Close => new CloseState(_logger, _config.CloseStateThreshold),
            State.Open => new OpenState(_logger, _config.OpenStateTimeoutSeconds),
            State.HalfOpen => new HalfOpenState(_logger, _config.HalfOpenStateThreshold),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}