using System.Numerics;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C03BoundaryTests
{
    [TestMethod]
    public void EveryDimensionComponentRejectsAdditionAndSubtractionOverflow()
    {
        for (var component = 0; component < 8; component++)
        {
            var upper = Component(component, int.MaxValue);
            var lower = Component(component, int.MinValue);
            var one = Component(component, 1);
            Assert.ThrowsExactly<OverflowException>(() => _ = upper + one,
                "Dimension addition overflow must be checked at component " + component);
            Assert.ThrowsExactly<OverflowException>(() => _ = lower - one,
                "Dimension subtraction overflow must be checked at component " + component);
        }
    }

    [TestMethod]
    public void QuantityOperationsAlsoRejectOverflowRatherThanWrap()
    {
        var huge = new Quantity(ExactRational.One, Component(0, int.MaxValue));
        var length = new Quantity(ExactRational.One, DimensionVector.LengthDimension);
        Assert.ThrowsExactly<OverflowException>(() => _ = huge.Multiply(length));
        var minimum = new Quantity(ExactRational.One, Component(0, int.MinValue));
        Assert.ThrowsExactly<OverflowException>(() => _ = minimum.Divide(length));
        var mass = new Quantity(ExactRational.One, DimensionVector.MassDimension);
        var time = new Quantity(ExactRational.One, DimensionVector.TimeDimension);
        Assert.IsFalse(mass.TryAdd(time, out _));
    }

    [TestMethod]
    public void AbsoluteAndIntervalConversionsDoNotShareOffsets()
    {
        var fahrenheit = Fahrenheit();
        var absolute = fahrenheit.ToBase(TemperatureMeasurement.Absolute(new ExactRational(32, 1)));
        var interval = fahrenheit.ToBase(TemperatureMeasurement.Interval(new ExactRational(9, 1)));
        Assert.AreEqual(TemperatureKind.Absolute, absolute.Kind);
        Assert.AreEqual(new ExactRational(27315, 100), absolute.Value);
        Assert.AreEqual(TemperatureKind.Interval, interval.Kind);
        Assert.AreEqual(new ExactRational(5, 1), interval.Value);
        Assert.AreEqual(TemperatureMeasurement.Absolute(new ExactRational(32, 1)),
            fahrenheit.FromBase(absolute));
        Assert.AreEqual(TemperatureMeasurement.Interval(new ExactRational(9, 1)),
            fahrenheit.FromBase(interval));
        Assert.AreNotEqual(new ExactRational(5, 1), fahrenheit.ToBase(new ExactRational(9, 1)),
            "The legacy scalar path is an absolute conversion and does not represent a temperature interval.");
    }

    [TestMethod]
    public void TemperatureArithmeticRejectsInvalidAbsoluteCombinations()
    {
        var freezing = TemperatureMeasurement.Absolute(new ExactRational(27315, 100));
        var warmer = TemperatureMeasurement.Absolute(new ExactRational(28315, 100));
        var interval = TemperatureMeasurement.Interval(new ExactRational(10, 1));
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = freezing.Add(warmer));
        Assert.AreEqual(interval, warmer.Subtract(freezing));
        Assert.AreEqual(warmer, freezing.Add(interval));
        Assert.AreEqual(warmer, interval.Add(freezing));
        Assert.AreEqual(freezing, warmer.Subtract(interval));
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = interval.Subtract(warmer));
        Assert.AreEqual(TemperatureMeasurement.Interval(ExactRational.Zero),
            default(TemperatureMeasurement));
    }

    [TestMethod]
    public void InvalidTemperatureUnitAndKindAreRejectedAtTypedBoundary()
    {
        var metre = new UnitDefinition("m", "m", "metre", UnitSystem.Si,
            DimensionVector.LengthDimension, ExactRational.One, ExactRational.Zero,
            UnitTransformKind.Linear);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            metre.ToBase(TemperatureMeasurement.Interval(ExactRational.One)));

        var invalidAffine = new UnitDefinition("broken", "X", "bad-linear-temperature",
            UnitSystem.Si, DimensionVector.TemperatureDimension,
            ExactRational.One, ExactRational.One, UnitTransformKind.Linear);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            invalidAffine.ToBase(TemperatureMeasurement.Absolute(ExactRational.One)));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            Fahrenheit().ToBase(new TemperatureMeasurement(ExactRational.One, (TemperatureKind)42)));
    }

    [TestMethod]
    public void TemperatureScalingPreservesArbitrarilyLargeExactNumerators()
    {
        var numerator = BigInteger.Pow(2, 256) + 1;
        var hugeInterval = TemperatureMeasurement.Interval(new ExactRational(numerator, 1));
        var fahrenheit = Fahrenheit();
        var converted = fahrenheit.ToBase(hugeInterval);
        Assert.AreEqual(new ExactRational(numerator * 5, 9), converted.Value);
        Assert.AreEqual(hugeInterval, fahrenheit.FromBase(converted));
    }

    private static UnitDefinition Fahrenheit() =>
        new("degF", "degF", "Fahrenheit", UnitSystem.UsCustomary,
            DimensionVector.TemperatureDimension,
            new ExactRational(5, 9), new ExactRational(45967, 180),
            UnitTransformKind.AbsoluteTemperature);

    private static DimensionVector Component(int index, int exponent)
    {
        var exponents = new int[8];
        exponents[index] = exponent;
        return new DimensionVector(
            exponents[0], exponents[1], exponents[2], exponents[3],
            exponents[4], exponents[5], exponents[6], exponents[7]);
    }
}
