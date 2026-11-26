using System.Numerics;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M11ExactInt64ScalingTests
{
    [TestMethod]
    public void SignedAndZeroFactorsPreserveExactValues()
    {
        Check(new ExactRational(3, 4), 100, 64, ExactInt64ScaleStatus.Success, 75);
        Check(new ExactRational(-3, 4), 100, 64, ExactInt64ScaleStatus.Success, -75);
        Check(new ExactRational(3, 4), -100, 64, ExactInt64ScaleStatus.Success, -75);
        Check(new ExactRational(-3, 4), -100, 64, ExactInt64ScaleStatus.Success, 75);
        Check(new ExactRational(1, 3), 0, 64, ExactInt64ScaleStatus.Success, 0);
        Check(default, -7, 64, ExactInt64ScaleStatus.Success, 0);
    }

    [TestMethod]
    public void RefusalPrecedenceAndSignedInt64EndpointsAreExplicit()
    {
        Check(new ExactRational(1, 3), 100, 8, ExactInt64ScaleStatus.NonIntegral);
        Check(new ExactRational(1, 3), 256, 8, ExactInt64ScaleStatus.ResourceLimit);
        Check(new ExactRational(long.MinValue, 1), 1, 64, ExactInt64ScaleStatus.Success, long.MinValue);
        Check(new ExactRational(long.MaxValue, 1), 1, 64, ExactInt64ScaleStatus.Success, long.MaxValue);
        Check(new ExactRational(long.MinValue, 1), -1, 64, ExactInt64ScaleStatus.Overflow);
        Check(new ExactRational(long.MaxValue, 1), 2, 64, ExactInt64ScaleStatus.Overflow);
        Check(new ExactRational(long.MinValue, 1), 1, 63, ExactInt64ScaleStatus.ResourceLimit);
        Check(new ExactRational(128, 1), 2, 8, ExactInt64ScaleStatus.ResourceLimit);
        Check(new ExactRational(127, 1), 2, 8, ExactInt64ScaleStatus.Success, 254);
        Check(new ExactRational(256, 1), 0, 8, ExactInt64ScaleStatus.ResourceLimit);
    }

    [TestMethod]
    public void CancellationAndInvalidBudgetRespectExistingResourcePolicy()
    {
        var power = BigInteger.One << 2000;
        Check(new ExactRational(1, power), power, 4096, ExactInt64ScaleStatus.Success, 1);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactRationalBudget.TryScaleToInt64(ExactRational.One, 1, 1));
        Assert.AreEqual(new ExactRational(3, 4),
            ExactRationalBudget.Add(new ExactRational(1, 4), new ExactRational(1, 2), 64));
        Assert.AreEqual(new ExactRational(-1, 1),
            ExactRationalBudget.Multiply(new ExactRational(3, 4), new ExactRational(-4, 3), 64));
    }

    private static void Check(ExactRational input, BigInteger scale, int maxBits,
        ExactInt64ScaleStatus expectedStatus, long? expectedValue = null)
    {
        var actual = ExactRationalBudget.TryScaleToInt64(input, scale, maxBits);
        Assert.AreEqual(expectedStatus, actual.Status);
        Assert.AreEqual(expectedValue, actual.Value);
    }
}
