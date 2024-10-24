using Common.CircuitBreaker.States;
using Microsoft.Extensions.Logging;

namespace Common.CircuitBreaker;

public class CircuitBreakerStateFactory : ICircuitBreakerStateFactory
{
    private readonly ILogger _logger;

    public CircuitBreakerStateFactory(ILogger logger)
    {
        _logger = logger;
    }

    public IState Create(State state)
    {
        return state switch
        {
            State.None => throw new ArgumentOutOfRangeException(nameof(state), state, null),
            State.Close => new CloseState(_logger, 1),
            State.Open => new OpenState(_logger, 1),
            State.HalfOpen => new HalfOpenState(_logger, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}