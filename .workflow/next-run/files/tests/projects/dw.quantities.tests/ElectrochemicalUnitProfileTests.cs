using dw.quantities;
using dw.quantities.standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

/// <summary>
/// Twelve independently selectable Math-owned source tests; the later
/// external NuGet consumer must load exact package references instead.
/// </summary>
[TestClass]
public sealed class ElectrochemicalUnitProfileTests
{
    [TestMethod]
    [DataRow("si.ampere", "A", "ampere", 0, 0, 0, 1, 0, 0, 0, 0)]
    [DataRow("si.volt", "V", "volt", 2, 1, -3, -1, 0, 0, 0, 0)]
    [DataRow("si.ohm", "Ω", "ohm", 2, 1, -3, -2, 0, 0, 0, 0)]
    [DataRow("si.watt", "W", "watt", 2, 1, -3, 0, 0, 0, 0, 0)]
    [DataRow("si.mole", "mol", "mole", 0, 0, 0, 0, 0, 1, 0, 0)]
    [DataRow("si.mole-per-cubic-metre", "mol/m³", "mole per cubic metre",
        -3, 0, 0, 0, 0, 1, 0, 0)]
    public void DefinitionHasExactIdentityAndEightDimensions(
        string id, string symbol, string name,
        int length, int mass, int time, int current,
        int temperature, int substance, int luminous, int information)
    {
        Assert.IsTrue(ElectrochemicalUnitProfile.TryGetById(id, out var unit));
        Assert.IsNotNull(unit);
        Assert.AreEqual(id, unit.Id);
        Assert.AreEqual(symbol, unit.Symbol);
        Assert.AreEqual(name, unit.Name);
        Assert.AreEqual(UnitSystem.Si, unit.System);
        Assert.AreEqual(new DimensionVector(length, mass, time, current,
            temperature, substance, luminous, information), unit.Dimension);
        Assert.AreEqual(0, unit.Dimension.Information);
        Assert.AreEqual(ExactRational.One, unit.ScaleToBase);
        Assert.AreEqual(ExactRational.Zero, unit.OffsetToBase);
        Assert.AreEqual(UnitTransformKind.Linear, unit.TransformKind);
        Assert.AreEqual(new ExactRational(7, 2),
            unit.FromBase(unit.ToBase(new ExactRational(7, 2))));
    }

    [TestMethod]
    public void OptInProfilePreservesBothHistoricalCatalogues()
    {
        Assert.AreEqual("math.si-electrochemical",
            typeof(ElectrochemicalUnitProfile)
                .GetField(nameof(ElectrochemicalUnitProfile.ProfileId))?.GetRawConstantValue());
        Assert.AreEqual("2026-11-electrochemical-exact-v1",
            typeof(ElectrochemicalUnitProfile)
                .GetField(nameof(ElectrochemicalUnitProfile.ProfileVersion))?.GetRawConstantValue());
        Assert.AreEqual(6, ElectrochemicalUnitProfile.All.Length);
        Assert.AreEqual(6, ElectrochemicalUnitProfile.All
            .Select(x => x.Id).Distinct(StringComparer.Ordinal).Count());
        Assert.AreEqual("0.2.0",
            typeof(InheritedStandardUnitCatalog)
                .GetField(nameof(InheritedStandardUnitCatalog.Version))?.GetRawConstantValue());
        Assert.AreEqual(58, InheritedStandardUnitCatalog.All.Length);
        Assert.AreEqual("2026-10-exact-v1",
            typeof(StandardUnitCatalog)
                .GetField(nameof(StandardUnitCatalog.CatalogVersion))?.GetRawConstantValue());
        Assert.AreEqual(14, StandardUnitCatalog.All.Length);
        foreach (var unit in ElectrochemicalUnitProfile.All)
        {
            Assert.IsFalse(InheritedStandardUnitCatalog.TryGetById(unit.Id, out _));
            Assert.IsFalse(StandardUnitCatalog.TryGetById(unit.Id, out _));
        }
        Assert.IsFalse(ElectrochemicalUnitProfile.TryGetById("si.kilogram", out _));
        Assert.ThrowsExactly<ArgumentException>(() =>
            ElectrochemicalUnitProfile.TryGetById(" ", out _));
    }

    [TestMethod]
    public void TwoAmperesTimesThreeOhmsProduceSixVolts()
    {
        var current = BaseQuantity(new ExactRational(2, 1), ElectrochemicalUnitProfile.Ampere);
        var resistance = BaseQuantity(new ExactRational(3, 1), ElectrochemicalUnitProfile.Ohm);
        var potential = current.Multiply(resistance);
        Assert.AreEqual(ElectrochemicalUnitProfile.Volt.Dimension, potential.Dimension);
        Assert.AreEqual(new ExactRational(6, 1),
            ElectrochemicalUnitProfile.Volt.FromBase(potential.Value));
        Assert.AreEqual(new DimensionVector(2, 1, -3, -1, 0, 0, 0, 0),
            potential.Dimension);
    }

    [TestMethod]
    public void SixVoltsTimesTwoAmperesProduceTwelveWatts()
    {
        var potential = BaseQuantity(new ExactRational(6, 1), ElectrochemicalUnitProfile.Volt);
        var current = BaseQuantity(new ExactRational(2, 1), ElectrochemicalUnitProfile.Ampere);
        var power = potential.Multiply(current);
        Assert.AreEqual(ElectrochemicalUnitProfile.Watt.Dimension, power.Dimension);
        Assert.AreEqual(new ExactRational(12, 1),
            ElectrochemicalUnitProfile.Watt.FromBase(power.Value));
        Assert.AreEqual(new DimensionVector(2, 1, -3, 0, 0, 0, 0), power.Dimension);
    }

    [TestMethod]
    public void HalfMolePerLitreEqualsFiveHundredMolesPerCubicMetreAndInverts()
    {
        Assert.IsTrue(InheritedStandardUnitCatalog.TryGetById("metric.litre", out var litre));
        Assert.IsNotNull(litre);
        var halfMole = BaseQuantity(new ExactRational(1, 2), ElectrochemicalUnitProfile.Mole);
        var oneLitre = BaseQuantity(ExactRational.One, litre);
        var concentration = halfMole.Divide(oneLitre);
        Assert.AreEqual(new DimensionVector(-3, 0, 0, 0, 0, 1, 0, 0),
            concentration.Dimension);
        Assert.AreEqual(new ExactRational(500, 1),
            ElectrochemicalUnitProfile.MolePerCubicMetre.FromBase(concentration.Value));
        Assert.AreEqual(halfMole, concentration.Multiply(oneLitre));
        Assert.AreEqual(new ExactRational(1, 2),
            ElectrochemicalUnitProfile.Mole.FromBase(
                concentration.Multiply(oneLitre).Value));
    }

    [TestMethod]
    public void KilogramToMoleRefusesBecauseActualPresentDimensionsDiffer()
    {
        Assert.IsTrue(InheritedStandardUnitCatalog.TryGetById("si.kilogram", out var kilogram));
        Assert.IsNotNull(kilogram);
        Assert.IsTrue(ElectrochemicalUnitProfile.TryGetById("si.mole", out var mole));
        Assert.IsNotNull(mole);
        Assert.AreNotEqual(kilogram.Dimension, mole.Dimension);
        var failure = Assert.ThrowsExactly<ArgumentException>(() =>
            PureUnitConverter.Convert(ExactRational.One, kilogram, mole));
        StringAssert.Contains(failure.Message, "dimensions are incompatible");
    }

    [TestMethod]
    public void IncompatibleElectricalDimensionsAndTransformationAreRejected()
    {
        var ampere = ElectrochemicalUnitProfile.Ampere;
        var volt = ElectrochemicalUnitProfile.Volt;
        Assert.AreNotEqual(ampere.Dimension, volt.Dimension);
        Assert.ThrowsExactly<ArgumentException>(() =>
            PureUnitConverter.Convert(ExactRational.One, ampere, volt));

        var incompatibleTransform = new UnitDefinition(
            "test.non-linear-volt", "V-test", "incompatible voltage transformation",
            UnitSystem.Si, volt.Dimension, ExactRational.One,
            ExactRational.Zero, UnitTransformKind.AbsoluteTemperature);
        Assert.AreEqual(volt.Dimension, incompatibleTransform.Dimension);
        Assert.ThrowsExactly<ArgumentException>(() =>
            PureUnitConverter.Convert(ExactRational.One, volt, incompatibleTransform));
    }

    private static Quantity BaseQuantity(ExactRational value, UnitDefinition unit) =>
        new(unit.ToBase(value), unit.Dimension);
}
