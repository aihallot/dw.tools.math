using System.Numerics;
using dw.quantities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W01C04Tests
{
    [TestMethod]
    public void ExactRationalEqualityIsRepresentationIndependent()
    {
        Assert.AreEqual(new ExactRational(1, 2), new ExactRational(2, 4));
        Assert.AreEqual(ExactRational.Zero, default(ExactRational));
        Assert.IsTrue(ExactRational.One == new ExactRational(9, 9));
    }

    [TestMethod]
    public void OneMetreAndOneHundredOneCentimetresAreEquivalentAtInclusiveOneCentimetreBoundary()
    {
        var metre = new Quantity(ExactRational.One, DimensionVector.LengthDimension);
        var oneHundredOneCentimetres = new Quantity(
            new ExactRational(101, 100), DimensionVector.LengthDimension);
        var oneCentimetre = new Quantity(new ExactRational(1, 100), DimensionVector.LengthDimension);
        var boundary = new QuantityTolerance(oneCentimetre, ExactRational.Zero);
        Assert.IsTrue(ExactQuantityComparison.AreEquivalent(metre, oneHundredOneCentimetres, boundary));
        Assert.IsTrue(ExactQuantityComparison.AreEquivalent(oneHundredOneCentimetres, metre, boundary));
        Assert.IsFalse(ExactQuantityComparison.AreEquivalent(metre, oneHundredOneCentimetres,
            new QuantityTolerance(new Quantity(ExactRational.Zero, DimensionVector.LengthDimension),
                ExactRational.Zero)));
    }

    [TestMethod]
    public void RelativeComparisonIsSymmetricAndMaxMagnitudeBased()
    {
        var metre = new Quantity(ExactRational.One, DimensionVector.LengthDimension);
        var oneHundredOneCentimetres = new Quantity(
            new ExactRational(101, 100), DimensionVector.LengthDimension);
        var onePercent = new QuantityTolerance(
            new Quantity(ExactRational.Zero, DimensionVector.Scalar),
            new ExactRational(1, 100));
        Assert.IsTrue(ExactQuantityComparison.AreEquivalent(metre, oneHundredOneCentimetres, onePercent));
        Assert.IsTrue(ExactQuantityComparison.AreEquivalent(oneHundredOneCentimetres, metre, onePercent));
        Assert.IsFalse(ExactQuantityComparison.AreEquivalent(metre, oneHundredOneCentimetres,
            new QuantityTolerance(new Quantity(ExactRational.Zero, DimensionVector.Scalar),
                new ExactRational(9, 1000))));
    }

    [TestMethod]
    public void MismatchedDimensionsAndInvalidTolerancesAreRejected()
    {
        var mass = new Quantity(ExactRational.One, DimensionVector.MassDimension);
        var time = new Quantity(ExactRational.One, DimensionVector.TimeDimension);
        var zeroMass = new Quantity(ExactRational.Zero, DimensionVector.MassDimension);
        Assert.ThrowsExactly<ArgumentException>(() =>
            ExactQuantityComparison.AreEquivalent(mass, time,
                new QuantityTolerance(zeroMass, ExactRational.Zero)));
        Assert.ThrowsExactly<ArgumentException>(() =>
            ExactQuantityComparison.AreEquivalent(mass, mass,
                new QuantityTolerance(new Quantity(ExactRational.One, DimensionVector.LengthDimension),
                    ExactRational.Zero)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactQuantityComparison.AreEquivalent(mass, mass,
                new QuantityTolerance(new Quantity(new ExactRational(-1, 10), DimensionVector.MassDimension),
                    ExactRational.Zero)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactQuantityComparison.AreEquivalent(mass, mass,
                new QuantityTolerance(zeroMass, new ExactRational(-1, 10))));
        Assert.ThrowsExactly<ArgumentException>(() =>
            QuantityTolerance.FromQuantities(zeroMass,
                new Quantity(ExactRational.One, DimensionVector.TimeDimension)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            QuantityTolerance.FromQuantities(zeroMass,
                new Quantity(new ExactRational(-1, 10), DimensionVector.Scalar)));
    }

    [TestMethod]
    public void BothZeroQuantitiesHaveZeroRelativeDifference()
    {
        var zero = new Quantity(ExactRational.Zero, DimensionVector.MassDimension);
        var tolerance = QuantityTolerance.FromQuantities(
            new Quantity(ExactRational.Zero, DimensionVector.Scalar),
            new Quantity(ExactRational.Zero, DimensionVector.Scalar));
        Assert.IsTrue(ExactQuantityComparison.AreEquivalent(zero, zero, tolerance));
        Assert.IsFalse(ExactQuantityComparison.AreEquivalent(zero,
            new Quantity(new ExactRational(1, 1000), DimensionVector.MassDimension), tolerance));
    }

    [TestMethod]
    public void TaggedAbsoluteTemperaturesCompareOnCanonicalKelvin()
    {
        var celsius = new UnitDefinition("degC", "C", "Celsius", UnitSystem.Si,
            DimensionVector.TemperatureDimension, ExactRational.One,
            new ExactRational(27315, 100), UnitTransformKind.AbsoluteTemperature);
        var fahrenheit = new UnitDefinition("degF", "F", "Fahrenheit", UnitSystem.UsCustomary,
            DimensionVector.TemperatureDimension, new ExactRational(5, 9),
            new ExactRational(45967, 180), UnitTransformKind.AbsoluteTemperature);
        var zeroC = celsius.ToBase(TemperatureMeasurement.Absolute(ExactRational.Zero));
        var thirtyTwoF = fahrenheit.ToBase(TemperatureMeasurement.Absolute(new ExactRational(32, 1)));
        Assert.AreEqual(zeroC, thirtyTwoF);
        var noTolerance = TemperatureMeasurement.Interval(ExactRational.Zero);
        Assert.IsTrue(ExactTemperatureComparison.AreEquivalent(
            zeroC, thirtyTwoF, noTolerance, ExactRational.Zero));
        Assert.IsFalse(ExactTemperatureComparison.AreEquivalent(
            zeroC,
            TemperatureMeasurement.Absolute(zeroC.Value + new ExactRational(1, 100)),
            noTolerance, ExactRational.Zero));
        Assert.IsTrue(ExactTemperatureComparison.AreEquivalent(
            zeroC,
            TemperatureMeasurement.Absolute(zeroC.Value + new ExactRational(1, 100)),
            TemperatureMeasurement.Interval(new ExactRational(1, 100)), ExactRational.Zero));
    }

    [TestMethod]
    public void TemperatureToleranceMustBeAnIntervalAndNonNegative()
    {
        var value = TemperatureMeasurement.Absolute(new ExactRational(27315, 100));
        var zero = TemperatureMeasurement.Interval(ExactRational.Zero);
        Assert.ThrowsExactly<ArgumentException>(() =>
            ExactTemperatureComparison.AreEquivalent(value, value, value, ExactRational.Zero));
        Assert.ThrowsExactly<ArgumentException>(() =>
            ExactTemperatureComparison.AreEquivalent(value, zero, zero, ExactRational.Zero));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactTemperatureComparison.AreEquivalent(value, value,
                TemperatureMeasurement.Interval(new ExactRational(-1, 10)), ExactRational.Zero));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactTemperatureComparison.AreEquivalent(value, value, zero, new ExactRational(-1, 100)));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactTemperatureComparison.AreEquivalent(
                new TemperatureMeasurement(ExactRational.One, (TemperatureKind)77),
                value, zero, ExactRational.Zero));
    }

    [TestMethod]
    public void VeryLargeExactValuesDoNotConvertToFloatingPoint()
    {
        var n = BigInteger.Pow(2, 256);
        var left = new Quantity(new ExactRational(n, 1), DimensionVector.InformationDimension);
        var right = new Quantity(new ExactRational(n + 1, 1), DimensionVector.InformationDimension);
        var one = new QuantityTolerance(
            new Quantity(ExactRational.One, DimensionVector.InformationDimension),
            ExactRational.Zero);
        Assert.IsTrue(ExactQuantityComparison.AreEquivalent(left, right, one));
        Assert.IsFalse(ExactQuantityComparison.AreEquivalent(left, right,
            new QuantityTolerance(new Quantity(ExactRational.Zero, DimensionVector.InformationDimension),
                ExactRational.Zero)));
    }
}
