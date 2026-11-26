using System.Numerics;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M11ExactInt64ScalingVerificationTests
{
    [TestMethod]
    public void NormalizedOperandsAndCancellationRespectExactBitBoundaries()
    {
        var denominator = BigInteger.One << 4095;
        var half = new ExactRational(denominator, denominator << 1);
        Check(half, 2, 2, ExactInt64ScaleStatus.Success, 1);
        var rational = new ExactRational(3, denominator);
        Check(rational, denominator, 4096, ExactInt64ScaleStatus.Success, 3);
        Check(rational, -denominator, 4096, ExactInt64ScaleStatus.Success, -3);
        Check(rational, denominator, 4095, ExactInt64ScaleStatus.ResourceLimit);
        Check(rational, denominator - 1, 4096, ExactInt64ScaleStatus.NonIntegral);
        Check(rational, denominator + 1, 4096, ExactInt64ScaleStatus.NonIntegral);
    }

    [TestMethod]
    public void AmbiguousProductBitBoundaryAndOverflowPrecedenceAreDistinguished()
    {
        // 3 and 2^(b-2) produce an exact b-bit result.
        // Doubling the factor produces a b+1-bit result.
        foreach (var bits in new[] { 2, 7, 31, 63, 64, 65, 128 })
        {
            var scale = BigInteger.One << (bits - 2);
            var product = 3 * scale;
            var expected = product > long.MaxValue
                ? ExactInt64ScaleStatus.Overflow : ExactInt64ScaleStatus.Success;
            Check(new ExactRational(3, 1), scale, bits, expected,
                expected == ExactInt64ScaleStatus.Success ? (long)product : null);
            Check(new ExactRational(3, 1), scale << 1, bits, ExactInt64ScaleStatus.ResourceLimit);
        }
        var beyond = BigInteger.One << 63;
        Check(new ExactRational(beyond, 1), -1, 64, ExactInt64ScaleStatus.Success, long.MinValue);
        Check(new ExactRational(beyond, 1), 1, 64, ExactInt64ScaleStatus.Overflow);
        Check(new ExactRational(beyond, 1), 2, 64, ExactInt64ScaleStatus.ResourceLimit);
        Check(new ExactRational(1, 3), beyond + 2, 64, ExactInt64ScaleStatus.NonIntegral);
    }

    [TestMethod]
    public void ReturnedResultsAreImmutableAndRefusalsCarryNoValue()
    {
        var result = ExactRationalBudget.TryScaleToInt64(ExactRational.Zero, 0, 2);
        Assert.AreEqual(ExactInt64ScaleStatus.Success, result.Status);
        Assert.AreEqual((long?)0, result.Value);
        Assert.IsTrue(typeof(ExactInt64ScaleResult).IsSealed);
        Assert.AreEqual(0, typeof(ExactInt64ScaleResult).GetConstructors().Length);
        Assert.IsNull(typeof(ExactInt64ScaleResult).GetProperty(nameof(ExactInt64ScaleResult.Status))!.GetSetMethod());
        Assert.IsNull(typeof(ExactInt64ScaleResult).GetProperty(nameof(ExactInt64ScaleResult.Value))!.GetSetMethod());
        Check(new ExactRational(1, 2), 1, 2, ExactInt64ScaleStatus.NonIntegral);
        Check(new ExactRational(BigInteger.One << 63, 1), 1, 64, ExactInt64ScaleStatus.Overflow);
        Check(new ExactRational(1, 4), 0, 2, ExactInt64ScaleStatus.ResourceLimit);
        Assert.AreEqual(ExactInt64ScaleStatus.Success, result.Status);
        Assert.AreEqual((long?)0, result.Value);
    }

    private static void Check(ExactRational input, BigInteger scale, int bits,
        ExactInt64ScaleStatus expected, long? expectedValue = null)
    {
        var actual = ExactRationalBudget.TryScaleToInt64(input, scale, bits);
        Assert.AreEqual(expected, actual.Status);
        Assert.AreEqual(expectedValue, actual.Value);
    }
}
