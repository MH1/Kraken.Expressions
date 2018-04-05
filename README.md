# Kraken.Expressions
[![Nuget](https://img.shields.io/nuget/v/Kraken.Expressions.svg)](https://www.nuget.org/packages/Kraken.Expressions/)

The expression compiler and evaluation library.

### Supported platforms
- **.NET Standard 1.6** or later
- **.NET Framework 3.5** or later

### Download and install
Nuget package [Kraken.Expressions](https://www.nuget.org/packages/Kraken.Expressions/)

```
Install-Package Kraken.Expressions
```

### Usage

```csharp
string expression = "...";
EvaluationContext context = new EvaluationContext();
// generic method
int result1 = context.EvaluateExpression<int>(expression);

// non-generic variant
object result2 = context.EvaluateExpression(expression);

int result3 = context.EvaluateExpression<int>("1 + 2");
// result3 is 3

int result4 = context.EvaluateExpression<int>("x * 3", new { x = 2 });
// result4 is 6

bool result5 = context.EvaluateExpression<bool>("DateTime.Now.DayOfWeek == DayOfWeek.Sunday");
// if the current day of week is sunday, the result5 is true
```

### Precompile
Re-use the precompiled lambda expression to speed-up the evaluation time.
```csharp
EvaluationContext context = new EvaluationContext();
context.AddParameters(new[] { "x", "y" }, new[] { typeof(int), typeof(int) });
var expression = context.Precompile<int, int>("(x + 2) * y");
var expression2 = context.Precompile<int, int>("(x + 4) * y");
//	and invoke with multiple parameters
var result1 = expression.Invoke(1, 2);
var result2 = expression.Invoke(2, 3);
var result3 = expression.Invoke(3, 4);
var result4 = expression2.Invoke(1, 5);
var result5 = expression2.Invoke(2, 6);
```

### Supported operators
- Mathematical operators +, -, *, /, %, MOD
- Bit shifting <<, >>, SHL, SHR
- Comparison <, >, <=, >=, ==, !=, <>
- Logical &, |, ^, &&, ||, AND, OR, XOR
- ??
- ? :
- Negate !, NOT
- String concatenation +
- Property evaluation
- Static method call
- ...etc. See also examples in the [test file](https://github.com/MH1/Kraken.Expressions/blob/master/Source/Kraken.Expressions.UnitTests/Batch/Default.txt)
 in format: return type;return value;expression to test
