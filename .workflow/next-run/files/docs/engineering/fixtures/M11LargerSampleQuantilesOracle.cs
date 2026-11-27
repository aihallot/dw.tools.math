using System.Numerics;
using System.Reflection;
using dw.quantities;
using Dw.Tools.Math.Statistics;

if (args.Length != 1 || args[0] is not ("--red" or "--green"))
    throw new ArgumentException("Expected --red or --green.");
bool red = args[0] == "--red";
var finite = typeof(QuantileStatistics64).GetMethod("Evaluate",
    [typeof(IEnumerable<double>), typeof(double), typeof(QuantileConvention64), typeof(int)]);
var exact = typeof(QuantileStatistics64).GetMethod("EvaluateExact",
    [typeof(IEnumerable<ExactRational>), typeof(ExactRational), typeof(QuantileConvention64), typeof(int)]);
Need(red ? finite is null && exact is null : finite is not null && exact is not null,
    "Explicit budget overload presence differs from requested phase.");
foreach (int n in new[] { 1, 4096, 4097, 1000000 })
foreach (int k in new[] { 0, 1, 2, 3, 4 })
foreach (var convention in Enum.GetValues<QuantileConvention64>())
{
    // Sorted sample is exactly 0,1,...,n-1. Independent integer ranks.
    long numerator = (long)(n - 1) * k;
    double expected = convention == QuantileConvention64.Type7
        ? numerator / 4d : Math.Max(0L, ((long)n * k + 3) / 4 - 1);
    if (n <= 4096)
        Need(QuantileStatistics64.Evaluate(Descending(n), k / 4d, convention) == expected, "Legacy finite result differs.");
    else if (k == 2)
        Refused(() => QuantileStatistics64.Evaluate(Descending(n), k / 4d, convention));
    if (!red)
        Need((double)finite!.Invoke(null, [Descending(n), k / 4d, convention, n])! == expected, "Explicit finite result differs.");
    if (n > 4097) continue;
    var probability = new ExactRational(k, 4);
    var expectedExact = convention == QuantileConvention64.Type7
        ? new ExactRational(numerator, 4)
        : new ExactRational(Math.Max(0L, ((long)n * k + 3) / 4 - 1), BigInteger.One);
    if (n <= 4096)
        Need(QuantileStatistics64.EvaluateExact(DescendingExact(n), probability, convention).Equals(expectedExact), "Legacy exact result differs.");
    else if (k == 2)
        Refused(() => QuantileStatistics64.EvaluateExact(DescendingExact(n), probability, convention));
    if (!red)
        Need(((ExactRational)exact!.Invoke(null, [DescendingExact(n), probability, convention, n])!).Equals(expectedExact), "Explicit exact result differs.");
}
Console.WriteLine(red ? "M11-QUANTILES-RED|missingOverloads=true|legacy=PASS|reference=PASS|PASS" :
    "M11-QUANTILES-GREEN|million=PASS|exact=PASS|legacy=PASS|PASS");

static IEnumerable<double> Descending(int n) => Enumerable.Range(0, n).Reverse().Select(x => (double)x);
static IEnumerable<ExactRational> DescendingExact(int n) =>
    Enumerable.Range(0, n).Reverse().Select(x => new ExactRational(x, BigInteger.One));
static void Need(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
static void Refused(Action action) {
    try { action(); } catch (ArgumentOutOfRangeException) { return; }
    throw new InvalidDataException("Expected legacy observation-budget refusal.");
}
