using System.Numerics;
using System.Reflection;
using dw.quantities;

if (args.Length != 1 || args[0] is not ("--red" or "--green"))
    throw new ArgumentException("Expected --red or --green.");
void Need(bool ok, string label) {
    if (!ok) throw new InvalidDataException("M11-INT64-ORACLE: " + label);
}
(string Status, long? Value) Oracle(ExactRational value, BigInteger scale, int bits) {
    if (bits < 2) throw new ArgumentOutOfRangeException(nameof(bits));
    long BitLength(BigInteger n) => BigInteger.Abs(n).GetBitLength();
    if (BitLength(value.Numerator) > bits || BitLength(value.Denominator) > bits || BitLength(scale) > bits)
        return ("ResourceLimit", null);
    var q = BigInteger.DivRem(value.Numerator * scale, value.Denominator, out var rem);
    if (!rem.IsZero) return ("NonIntegral", null);
    if (BitLength(q) > bits) return ("ResourceLimit", null);
    if (q < long.MinValue || q > long.MaxValue) return ("Overflow", null);
    return ("Success", (long)q);
}
var cases = new List<(ExactRational Input, BigInteger Scale, int Bits, string Status, long? Value)>();
void Case(BigInteger n, BigInteger d, BigInteger s, int b, string status, long? value = null)
    => cases.Add((new ExactRational(n,d),s,b,status,value));
Case(3,4,100,4096,"Success",75);
Case(-3,4,100,4096,"Success",-75);
Case(3,4,-100,4096,"Success",-75);
Case(-3,4,-100,4096,"Success",75);
Case(1,3,100,4096,"NonIntegral");
Case(1,1000,100,4096,"NonIntegral");
Case(1,3,0,4096,"Success",0);
cases.Add((default,-7,4096,"Success",0));
Case(long.MinValue,1,1,4096,"Success",long.MinValue);
Case(long.MaxValue,1,1,4096,"Success",long.MaxValue);
Case((BigInteger)long.MinValue-1,1,1,4096,"Overflow");
Case((BigInteger)long.MaxValue+1,1,1,4096,"Overflow");
Case(long.MinValue,1,-1,4096,"Overflow");
var large = BigInteger.One << 2000;
Case(1,large,large,4096,"Success",1);
Case(1,3,256,8,"ResourceLimit");
Case(128,1,2,8,"ResourceLimit");
Case(127,1,2,8,"Success",254);
Case(1,3,100,8,"NonIntegral");
Case(long.MaxValue,1,2,64,"Overflow");
Case(long.MinValue,1,-1,64,"Overflow");
Case(long.MinValue,1,1,63,"ResourceLimit");
Case(256,1,0,8,"ResourceLimit");
Case(1,256,0,8,"ResourceLimit");
Case(0,1,256,8,"ResourceLimit");
Case(1,-4,100,8,"Success",-25);
foreach (var c in cases)
    Need(Oracle(c.Input,c.Scale,c.Bits) == (c.Status,c.Value), "fixed independent expectation");
var method = typeof(ExactRationalBudget).GetMethod("TryScaleToInt64", BindingFlags.Public | BindingFlags.Static,
    null, new[] { typeof(ExactRational), typeof(BigInteger), typeof(int) }, null);
Need(ExactRationalBudget.Add(new ExactRational(1,4),new ExactRational(1,2),64) == new ExactRational(3,4), "historical addition");
Need(ExactRationalBudget.Multiply(new ExactRational(3,4),new ExactRational(-4,3),64) == new ExactRational(-1,1), "historical multiplication");
if (args[0] == "--red") {
    Need(method is null, "expected missing exact public signature; review existing implementation instead");
    Console.WriteLine($"M11-INT64-RED|missingMember=true|fixedOracleCases={cases.Count}|historicalArithmetic=PASS|PASS");
    return;
}
if (method is null) throw new InvalidDataException("M11-INT64-ORACLE: public signature missing");
Need(method.ReturnType.FullName == "dw.quantities.ExactInt64ScaleResult", "public return type");
var statusProperty = method.ReturnType.GetProperty("Status") ?? throw new InvalidDataException("Status missing");
var valueProperty = method.ReturnType.GetProperty("Value") ?? throw new InvalidDataException("Value missing");
Need(statusProperty.PropertyType.FullName == "dw.quantities.ExactInt64ScaleStatus" &&
    statusProperty.PropertyType.IsEnum && valueProperty.PropertyType == typeof(long?), "public result shape");
(string Status,long? Value) Actual(ExactRational value, BigInteger scale, int bits) {
    var result = method.Invoke(null, new object[] { value, scale, bits }) ?? throw new InvalidDataException("Null result");
    return (statusProperty.GetValue(result)?.ToString() ?? "", (long?)valueProperty.GetValue(result));
}
foreach (var c in cases)
    Need(Actual(c.Input,c.Scale,c.Bits) == (c.Status,c.Value), "fixed actual result");
int generated = 0;
foreach (var bits in new[] { 2, 8, 16, 64 })
for (int n = -17; n <= 17; n += 3)
for (int d = 1; d <= 13; d += 3)
for (int s = -31; s <= 31; s += 7) {
    var input = new ExactRational(n,d);
    Need(Actual(input,s,bits) == Oracle(input,s,bits), "generated actual result");
    generated++;
}
bool invalid = false;
try { _ = Actual(ExactRational.One,1,1); }
catch (TargetInvocationException ex) when (ex.InnerException is ArgumentOutOfRangeException) { invalid = true; }
Need(invalid, "invalid bit budget");
Console.WriteLine($"M11-INT64-GREEN|fixed={cases.Count}|generated={generated}|invalidBudget=PASS|PASS");
