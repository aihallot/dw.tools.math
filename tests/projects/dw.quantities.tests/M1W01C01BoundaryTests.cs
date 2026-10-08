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
    [TestMethod]
    public void DefaultIsCanonicalZeroAndAgreesWithConstructedZero()
    {
        var value = default(ExactRational);
        Assert.AreEqual(BigInteger.Zero, value.Numerator);
        Assert.AreEqual(BigInteger.One, value.Denominator);
        Assert.AreEqual(ExactRational.Zero, value);
        Assert.AreEqual("0", value.ToFractionString());
        Assert.AreEqual("0", value.ToMixedNumberString());
        Assert.AreEqual(new ExactRational(5, 7), value + new ExactRational(5, 7));
        Assert.AreEqual(0, value.CompareTo(ExactRational.Zero));
        Assert.ThrowsExactly<DivideByZeroException>(() => _ = ExactRational.One / value);
    }

    [TestMethod]
    public void LargeExactInputsAndIntermediateRemainCanonical()
    {
        var large = BigInteger.Pow(2, 256) + 1;
        var fraction = new ExactRational(large, 3);
        var restored = (fraction * new ExactRational(3, 1)) / new ExactRational(large, 1);
        Assert.AreEqual(BigInteger.One, restored.Numerator);
        Assert.AreEqual(BigInteger.One, restored.Denominator);
        Assert.AreEqual("1", restored.ToFractionString());
    }

}
