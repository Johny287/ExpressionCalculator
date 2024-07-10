namespace StringExpressionCalculator.Domain.Domain;

/// <summary>
/// Базовый контекст математической операции.
/// Результаты операций могут храниться здесь же.
/// </summary>
public abstract class OperationContext : IResult<double>
{
    private double? _result;

    public string Symbol { get; }

    protected OperationContext(string symbol)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);
        Symbol = symbol;
    }

    public void SetResult(double result) => _result = result;
    public double? GetResult() => _result;
}


public interface IResult<T> where T : struct
{
    void SetResult(T result);
    T? GetResult();
}