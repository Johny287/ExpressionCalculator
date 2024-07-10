using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;

namespace StringExpressionCalculator.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly IExpressionCalculator _calculator;

    public CalculatorController(IExpressionCalculator calculator) => _calculator = calculator;

    [HttpPost("CalculateExpression")]
    public string CalculateExpression([FromBody] string expression) 
        => _calculator.Calculate(expression).ToString(CultureInfo.CurrentCulture);
}