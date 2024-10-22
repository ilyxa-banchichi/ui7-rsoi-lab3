namespace Common.CircuitBreaker;

public enum State
{
    None = 0,
    Close = 1,
    Open = 2,
    HalfOpen = 3,
}

public interface IState
{
    State State { get; }
    Task<T?> ExecuteCommandAsync<T>(Func<Task<T>> command, Func<Task<T>>? fallback = null);
    State TryDoTransition();
}