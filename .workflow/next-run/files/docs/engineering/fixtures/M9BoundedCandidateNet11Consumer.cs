using System.Security.Cryptography;
using Dw.Tools.Math.Statistics;

if (args.Length != 1 || args[0].Length != 64)
    throw new ArgumentException("Expected original candidate DLL SHA-256.");
void Need(bool yes, string message)
{
    if (!yes) throw new InvalidOperationException(message);
}
var asm = typeof(SplitMix64Random).Assembly;
Need(asm.GetName().Name == "dw.tools.math.statistics", "Wrong assembly identity.");
Need(Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(asm.Location))) == args[0],
    "Loaded binary is not the candidate archive DLL.");
Console.WriteLine("MATH-RS270-CHECK|candidate-loaded-dll|PASS");

Need(SplitMix64Random.BoundedTransformId == "splitmix64-bounded-rejection/v1",
    "Expected exact bounded transformation version.");
Console.WriteLine("MATH-RS270-CHECK|transform-version|PASS");
var g = new SplitMix64Random(0UL);
Need(g.NextBoundedUInt64(3UL, 1) == 1UL &&
    g.NextBoundedUInt64(3UL, 1) == 0UL && g.Counter == 2UL,
    "Seed zero independent raw golden vector or counter differs.");
Console.WriteLine("MATH-RS270-CHECK|golden-residues|PASS");
Need(new SplitMix64Random(11UL).NextBoundedUInt64(1UL, 1) == 0UL,
    "Unit upper bound must return zero.");
Console.WriteLine("MATH-RS270-CHECK|unit-bound|PASS");
var d = new SplitMix64Random(0UL);
_ = d.NextUInt64();
try { _ = d.NextBoundedUInt64(0x8000000000000001UL, 1); throw new Exception("Missing refusal."); }
catch (InvalidOperationException) { }
Need(d.Counter == 2UL, "Rejected word did not consume checkpoint counter.");
Console.WriteLine("MATH-RS270-CHECK|forced-rejection-budget|PASS");
Need(d.NextBoundedUInt64(0x8000000000000001UL, 2) ==
    0xf88bb8a8724c81ecUL % 0x8000000000000001UL && d.Counter == 4UL,
    "Rejected then accepted raw words differ.");
Console.WriteLine("MATH-RS270-CHECK|forced-rejection-recovery|PASS");
var inclusive = new SplitMix64Random(42UL);
for (var i = 0; i < 128; i++)
    Need(inclusive.NextInt64Inclusive(2L, 24L, 64) is >= 2L and <= 24L,
        "Inclusive integer bound violated.");
Console.WriteLine("MATH-RS270-CHECK|inclusive-range|PASS");
Need(new SplitMix64Random(0UL).NextInt64Inclusive(long.MinValue, long.MaxValue, 1) ==
    unchecked((long)(0xe220a8397b1dcdafUL ^ (1UL << 63))),
    "Full signed domain uses incorrect offset.");
Console.WriteLine("MATH-RS270-CHECK|signed-extremes|PASS");
var before = new SplitMix64Random(0UL);
before.NextBoundedUInt64(24UL, 1);
var restored = SplitMix64Random.FromSnapshot(
    SplitMix64PositionCodec.Parse(SplitMix64PositionCodec.Format(before.Snapshot())));
Need(before.NextBoundedUInt64(0x8000000000000001UL, 4) ==
     restored.NextBoundedUInt64(0x8000000000000001UL, 4) &&
     before.Snapshot() == restored.Snapshot(), "Checkpoint replay differs.");
Console.WriteLine("MATH-RS270-CHECK|checkpoint-replay|PASS");
var invalid = new SplitMix64Random(42UL);
try { invalid.NextBoundedUInt64(0UL, 1); throw new Exception("Missing invalid bound."); }
catch (ArgumentOutOfRangeException) { }
Need(invalid.Counter == 0UL, "Invalid bound consumed raw word.");
Console.WriteLine("MATH-RS270-CHECK|invalid-bound-refusal|PASS");
try { invalid.NextBoundedUInt64(3UL, 0); throw new Exception("Missing invalid budget."); }
catch (ArgumentOutOfRangeException) { }
Need(invalid.Counter == 0UL, "Invalid budget consumed raw word.");
Console.WriteLine("MATH-RS270-CHECK|invalid-budget-refusal|PASS");
var exhausted = new SplitMix64Random(0UL, ulong.MaxValue);
try { exhausted.NextBoundedUInt64(1UL, 1); throw new Exception("Missing counter refusal."); }
catch (InvalidOperationException) { }
Need(exhausted.Counter == ulong.MaxValue, "Counter overflowed.");
Console.WriteLine("MATH-RS270-CHECK|counter-exhaustion-refusal|PASS");
Console.WriteLine("MATH-RS270-BOUNDED-PACKAGE|PASS|12");
