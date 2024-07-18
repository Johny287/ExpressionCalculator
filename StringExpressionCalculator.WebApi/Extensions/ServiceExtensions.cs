using StringExpressionCalculator.Domain.Application.Operations;
using StringExpressionCalculator.Domain.Application.Operations.Abstractions;
using StringExpressionCalculator.Domain.Application.Services;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;

namespace StringExpressionCalculator.WebApi.Extensions;

public static class ServiceExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        AddOperations(services);
        services.AddScoped<IOperationsProvider, OperationsProvider>();

        services.AddScoped<IStringParser, StringParser>();
        services.AddScoped<IExpressionParser, ExpressionParser>();

        services.AddScoped<ICalculator, Calculator>();
        services.AddScoped<IExpressionCalculator, ExpressionCalculator>();
    }

    private static void AddOperations(this IServiceCollection services)
    {
        services.AddScoped<IMathOperation, SumOperation>();
        services.AddScoped<IMathOperation, SubtractionOperation>();
        services.AddScoped<IMathOperation, MultiplicationOperation>();
        services.AddScoped<IMathOperation, DivisionOperation>();
        services.AddScoped<IMathOperation, FactorialOperation>();
        services.AddScoped<IMathOperation, ExponentiationOperation>();
    }
}