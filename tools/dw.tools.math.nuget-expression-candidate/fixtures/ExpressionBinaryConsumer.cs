using System.Security.Cryptography;
using dw.quantities;
using dw.quantities.expression;

if (args.Length != 2 || args.Any(hash => hash.Length != 64 ||
    hash.Any(character => character is not (>= '0' and <= '9' or >= 'a' and <= 'f'))))
    throw new ArgumentException("Two lower-case SHA-256 identities are required: expression and base.");

static void Check(bool condition, string reason)
{
    if (!condition)
        throw new InvalidOperationException("Math expression binary-only consumer: " + reason);
}

var expressionAssembly = typeof(ExpressionParser).Assembly;
Check(expressionAssembly.GetName().Name == "dw.quantities.expression",
    "loaded expression assembly identity differs");
Console.WriteLine("MATH-EXPRESSION-CHECK|expression-assembly|PASS");

var baseAssembly = typeof(ExactRational).Assembly;
Check(baseAssembly.GetName().Name == "dw.quantities",
    "loaded base quantities assembly differs");
Console.WriteLine("MATH-EXPRESSION-CHECK|base-assembly|PASS");

static string Sha(string path) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));

Check(Sha(expressionAssembly.Location) == args[0] &&
      Sha(baseAssembly.Location) == args[1] &&
      !string.Equals(expressionAssembly.Location, baseAssembly.Location,
          StringComparison.OrdinalIgnoreCase),
    "loaded DLL bytes must match both exact NuGet package binaries");
Console.WriteLine("MATH-EXPRESSION-CHECK|loaded-binaries|PASS");

var sum = ExpressionParser.Evaluate("1/3 + 1/6");
Check(sum is ExpressionEvaluationOutcome.Success success &&
      success.Quantity.Value == new ExactRational(1, 2) &&
      success.Quantity.Dimension == DimensionVector.Scalar,
    "1/3 plus 1/6 must be an exact dimensionless one half");
Console.WriteLine("MATH-EXPRESSION-CHECK|exact-evaluation|PASS");

var dividedByZero = ExpressionParser.Evaluate("1/0");
Check(dividedByZero is ExpressionEvaluationOutcome.Failure
      { Kind: ExpressionFailureKind.DivideByZero },
    "division by zero must return a bounded typed failure");
Console.WriteLine("MATH-EXPRESSION-CHECK|divide-by-zero-refusal|PASS");

var tooLong = ExpressionParser.Evaluate(new string('9',
    MathExpressionLimits.MaximumCharacters + 1));
Check(tooLong is ExpressionEvaluationOutcome.Failure
      { Kind: ExpressionFailureKind.ComplexityLimit },
    "bounded expression maximum length must refuse");
Console.WriteLine("MATH-EXPRESSION-CHECK|complexity-refusal|PASS");

Console.WriteLine("MATH-EXPRESSION-BINARY|PASS|6");
