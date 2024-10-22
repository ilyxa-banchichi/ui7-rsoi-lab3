using Microsoft.Extensions.Logging;

namespace Common.CircuitBreaker;

public interface ICircuitBreakerStateFactory
{
    IState Create(State state);
}