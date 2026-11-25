using System.Numerics;
using Dw.Tools.Math.Statistics;

var domain = BigInteger.One << 64;
void Need(bool ok, string label)
{
    if (!ok) throw new InvalidDataException("M9-ORACLE: " + label);
}
foreach (ulong seed in new ulong[] { 0, 1, 17, 42, ulong.MaxValue })
foreach (ulong bound in new ulong[] { 1, 2, 3, 24, 1UL << 63, (1UL << 63) + 1, ulong.MaxValue })
{
    var actual = new SplitMix64Random(seed);
    var raw = new SplitMix64Random(seed);
    for (int i = 0; i < 32; i++)
    {
        ulong word;
        do { word = raw.NextUInt64(); } while ((BigInteger)word < domain % bound);
        Need(actual.NextBoundedUInt64(bound, 128) == (ulong)((BigInteger)word % bound), "rejection residue");
        Need(actual.Counter == raw.Counter, "rejection counter");
    }
}
var limited = new SplitMix64Random(0, 1);
bool refused = false;
try { _ = limited.NextBoundedUInt64((1UL << 63) + 1, 2); }
catch (InvalidOperationException) { refused = true; }
Need(refused && limited.Counter == 3, "budget");
foreach (var (low, high) in new[] { (long.MinValue, long.MaxValue), (-7L, 13L), (long.MinValue, -1L), (long.MaxValue, long.MaxValue) })
{
    var actual = new SplitMix64Random(0);
    var raw = new SplitMix64Random(0);
    var width = (BigInteger)high - low + 1;
    for (int i = 0; i < 32; i++)
    {
        ulong word;
        do { word = raw.NextUInt64(); } while ((BigInteger)word < domain % width);
        var expected = (long)((BigInteger)low + word % width);
        Need(actual.NextInt64Inclusive(low, high, 128) == expected, "signed interval");
        Need(actual.Counter == raw.Counter, "signed counter");
    }
}
var exhausted = new SplitMix64Random(17, ulong.MaxValue - 1);
_ = exhausted.NextBoundedUInt64(1, 1);
refused = false;
try { _ = exhausted.NextBoundedUInt64(1, 1); }
catch (InvalidOperationException) { refused = true; }
Need(refused && exhausted.Counter == ulong.MaxValue, "counter exhaustion");
var snapshot = new SplitMix64Random(0, 1).Snapshot();
Need(SplitMix64Random.FromSnapshot(snapshot).NextBoundedUInt64(3, 8) ==
    new SplitMix64Random(0, 1).NextBoundedUInt64(3, 8), "checkpoint");
Console.WriteLine("M9-ORACLE-PASS");
