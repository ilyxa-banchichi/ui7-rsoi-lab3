namespace Common.CircuitBreaker;

public class CircuitBreakerConfig
{
    public int CloseStateThreshold { get; set; }
    public int OpenStateTimeoutSeconds { get; set; }
    public int HalfOpenStateThreshold { get; set; }
}