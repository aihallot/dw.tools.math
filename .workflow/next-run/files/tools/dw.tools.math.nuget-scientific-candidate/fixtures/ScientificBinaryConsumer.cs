using System.Numerics;
using System.Security.Cryptography;
using Dw.Tools.Math.Scientific;

if (args.Length != 1 || args[0].Length != 64)
    throw new InvalidOperationException("Expected exact original package DLL SHA-256.");
var checks = 0;
void Probe(string name, Action check)
{
    try
    {
        check();
        checks++;
        Console.WriteLine("MATH-SCIENTIFIC-CHECK|" + name + "|PASS");
    }
    catch (Exception error)
    {
        throw new InvalidOperationException("Scientific package binary check " + name, error);
    }
}
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
static void Near(double? actual, double expected, double tolerance = 1e-10)
{
    Need(actual.HasValue && double.IsFinite(actual.Value) &&
         System.Math.Abs(actual.Value - expected) <= tolerance,
         "Numerical result outside independent expectation.");
}

Probe("loaded-package-sha256", () =>
{
    var library = typeof(BoundedExactScientific).Assembly;
    Need(library.GetName().Name == "dw.tools.math.scientific",
         "Expected published Scientific assembly identity.");
    Need(Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(library.Location))) == args[0],
         "Loaded scientific binary differs from package archive.");
});
Probe("integer-gcd", () =>
{
    var result = BoundedExactScientific.GreatestCommonDivisor(54, 24);
    Need(result.Succeeded && result.Value == new BigInteger(6), "gcd(54,24) must be 6.");
});
Probe("factorial-six", () =>
{
    var result = BoundedExactScientific.Factorial(6);
    Need(result.Succeeded && result.Value == new BigInteger(720), "6! must be 720.");
});
Probe("binomial-valid", () =>
{
    var result = BoundedExactScientific.Binomial(5, 2);
    Need(result.Succeeded && result.Value == new BigInteger(10), "binomial(5,2)=10.");
});
Probe("binomial-rejection", () =>
{
    var result = BoundedExactScientific.Binomial(5, 6);
    Need(result.Status == ExactScientificStatus.InvalidInput && result.Value is null,
         "Out-of-range binomial choice must refuse without a value.");
});
Probe("euclidean-norm", () =>
{
    var result = FiniteGeometry64.Norm([3d, 4d]);
    Need(result.Succeeded && !result.ErrorBoundGuaranteed,
         "Finite Euclidean norm is an approximate result, not a guarantee.");
    Near(result.Scalar, 5d);
});
Probe("zero-axis-rejection", () =>
{
    var result = FiniteGeometry64.Project([3d, 4d], [0d, 0d]);
    Need(!result.Succeeded && result.Status == FiniteGeometryStatus64.ZeroAxis,
         "Projection onto a zero axis must refuse.");
});
Probe("uniform-first-differences", () =>
{
    var result = FiniteUniformSeries64.FirstDifferences([1d, 4d, 10d], 0.5d);
    Need(result.Succeeded && result.Samples is { Length: 2 } &&
         result.Samples[0] == 3d && result.Samples[1] == 6d,
         "Adjacent differences must be 3 and 6, not interval-normalized slopes.");
});
Probe("invalid-moving-average-window", () =>
{
    var result = FiniteUniformSeries64.MovingAverage([1d, 2d], 1d, 3);
    Need(!result.Succeeded && result.Status == FiniteUniformSeriesStatus64.InvalidWindow,
         "Too-wide moving average must refuse.");
});
Probe("finite-linear-convolution", () =>
{
    var result = FiniteSignal64.LinearConvolution([1d, 2d], [3d, 4d]);
    Need(result.Succeeded && result.Samples is { Length: 3 } &&
         result.Samples[0] == 3d && result.Samples[1] == 10d &&
         result.Samples[2] == 8d,
         "Linear convolution expected [3,10,8].");
});
Probe("special-sine-radians", () =>
{
    var result = BoundedSpecialFunctions64.EvaluateReal(
        SpecialFunctionKind64.Sine, System.Math.PI / 2d);
    Need(result.Succeeded && !result.ErrorBoundGuaranteed &&
         result.AngleConvention == SpecialAngleConvention64.Radians,
         "Sine uses radians with noncertified approximate result.");
    Near(result.Value, 1d);
});
Probe("special-log-domain", () =>
{
    var result = BoundedSpecialFunctions64.EvaluateReal(
        SpecialFunctionKind64.NaturalLogarithm, -1d);
    Need(!result.Succeeded && result.Status == SpecialFunctionStatus64.UnsupportedDomain &&
         result.Value is null,
         "Negative-real logarithm must refuse without a value.");
});
Probe("one-period-discount", () =>
{
    var result = M6W02C04Contract.Discount(110d, 0.1d, 1);
    Need(result.Succeeded && !result.ErrorBoundGuaranteed,
         "Discount result must not claim a numerical error theorem.");
    Near(result.Value, 100d);
});
Probe("invalid-discount-rate", () =>
{
    var result = M6W02C04Contract.Discount(10d, -1d, 1);
    Need(!result.Succeeded && result.Status == FinancialMathStatus64.InvalidRate &&
         result.Value is null,
         "Effective rate -1 is outside admitted financial policy.");
});
Probe("cancellation-is-refusal", () =>
{
    using var cancellation = new CancellationTokenSource();
    cancellation.Cancel();
    var result = BoundedExactScientific.Factorial(6, cancellationToken: cancellation.Token);
    Need(result.Status == ExactScientificStatus.Cancelled && result.Value is null,
         "Cancelled exact operation must not return a partial integer.");
});
Need(checks == 15, "Exactly fifteen independent source-free scientific checks are required.");
Console.WriteLine("MATH-SCIENTIFIC-BINARY|PASS|15");
