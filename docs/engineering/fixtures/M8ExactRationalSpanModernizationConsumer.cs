using System.Globalization;
using System.Numerics;
using dw.quantities;

if (args.Length != 1 || !int.TryParse(args[0], out var major) || Environment.Version.Major != major)
    throw new InvalidOperationException("Unexpected runtime.");
static void Need(bool ok, string label)
{
    if (!ok) throw new InvalidOperationException("RS342 oracle: " + label);
}
static void Exact(string input, BigInteger numerator, BigInteger denominator)
{
    var result = ExactRational.ParseInvariantDecimal(input);
    Need(result.Numerator == numerator && result.Denominator == denominator, input);
}
Exact("0", 0, 1);
Exact("-0.000", 0, 1);
Exact(".5", 1, 2);
Exact("-.5", -1, 2);
Exact("+12.", 12, 1);
Exact("00012.3400", 617, 50);
Exact("-0.000125", -1, 8000);
Exact("123456789012345678901234567890.25",
    BigInteger.Parse("493827156049382715604938271561", CultureInfo.InvariantCulture), 4);
var huge = BigInteger.Pow(10, 180);
Exact("1" + new string('0', 180) + ".125", huge * 8 + 1, 8);
var oldCulture = CultureInfo.CurrentCulture;
try
{
    CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-BE");
    Exact("12.50", 25, 2);
    foreach (var invalid in new[] { "+", "-", ".", "1.2.3", "1,25", "1e2", " 1", "1 ", "١" })
    {
        bool refused = false;
        try { _ = ExactRational.ParseInvariantDecimal(invalid); }
        catch (FormatException) { refused = true; }
        Need(refused, "format refusal " + invalid);
    }
}
finally { CultureInfo.CurrentCulture = oldCulture; }
Need(default(ExactRational) == ExactRational.Zero, "default zero");
var sample = "12345678901234567890123456789012345678901234567890." +
    "12345678901234567890123456789012345678901234567891";
for (int i = 0; i < 512; i++) _ = ExactRational.ParseInvariantDecimal(sample);
const int iterations = 4096;
var before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < iterations; i++) _ = ExactRational.ParseInvariantDecimal(sample);
var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
Need(allocated > 0, "allocation counter");
Console.WriteLine("MATH-RS342-ORACLE|PASS");
Console.WriteLine("MATH-RS342-ALLOC|" + allocated.ToString(CultureInfo.InvariantCulture));
