using System.Numerics;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C01BoundaryTests
{
    [TestMethod]
    public void DefaultValueMustNotExposeAnInvalidCanonicalDenominator()
    {
        var value = default(ExactRational);
        Assert.IsTrue(value.Denominator > BigInteger.Zero,
            "M1-W01-C01-T2 RED: default ExactRational exposes zero denominator; default policy is unresolved.");
    }

    [TestMethod]
    public void LargeIntermediateArithmeticRemainsExactWithoutDouble()
    {
        var power = BigInteger.Pow(2, 128);
        var left = new ExactRational(power + 1, power);
        var right = new ExactRational(-power, power);
        var sum = left + right;
        Assert.AreEqual(BigInteger.One, sum.Numerator);
        Assert.AreEqual(power, sum.Denominator);
    }
}
