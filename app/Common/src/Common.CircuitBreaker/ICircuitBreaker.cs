namespace Common.CircuitBreaker;

public interface ICircuitBreaker
{
    Task<T?> ExecuteCommandAsync<T>(Func<Task<T>> command, Func<Task<T>>? fallback = null);
}

public interface ICircuitBreaker<TService> : ICircuitBreaker{ }