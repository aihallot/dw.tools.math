using System.Globalization;
using System.Numerics;
using dw.quantities;
using dw.quantities.standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C01SourceParityTests
{
    [TestMethod]
    public void InheritedCatalogueRetainsOriginalVersionAndAllFiftyEightUnitDefinitions()
    {
        var field = typeof(InheritedStandardUnitCatalog).GetField(
            nameof(InheritedStandardUnitCatalog.Version),
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        Assert.IsNotNull(field);
        Assert.AreEqual("0.2.0", field.GetRawConstantValue() as string);
        Assert.AreEqual(58, InheritedStandardUnitCatalog.All.Length);
        Assert.AreEqual(58, InheritedStandardUnitCatalog.All.Select(x => x.Id)
            .Distinct(StringComparer.Ordinal).Count());
        Assert.IsFalse(InheritedStandardUnitCatalog.All.Any(x => x.Dimension == DimensionVector.Scalar));
    }

    [TestMethod]
    public void AllOriginalCanonicalIdsAreAccessibleWithoutCultureInference()
    {
        foreach (var unit in InheritedStandardUnitCatalog.All)
        {
            Assert.IsTrue(InheritedStandardUnitCatalog.TryGetById(unit.Id, out var found), unit.Id);
            Assert.AreEqual(unit, found, unit.Id);
            Assert.AreEqual(unit, StandardExpressionUnitResolver.ResolveInherited(unit.Id), unit.Id);
        }
        Assert.IsFalse(InheritedStandardUnitCatalog.TryGetById("not.a.unit", out _));
    }

    [TestMethod]
    public void ExactMetricAndImperialConstantsRemainSourceFaithful()
    {
        AssertScale("imperial.inch", new ExactRational(127, 5000));
        AssertScale("imperial.foot", ExactRational.ParseInvariantDecimal("0.3048"));
        AssertScale("imperial.pound", ExactRational.ParseInvariantDecimal("0.45359237"));
        AssertScale("metric.litre", new ExactRational(1, 1000));
        AssertScale("metric.millilitre", new ExactRational(1, 1_000_000));
        AssertScale("metric.kilometre-per-hour", new ExactRational(5, 18));
        AssertScale("us.tablespoon", ExactRational.ParseInvariantDecimal("0.00001478676478125"));
        AssertScale("au.tablespoon", new ExactRational(1, 50000));
    }

    [TestMethod]
    public void CompleteDecimalAndBinaryInformationPrefixesRetainExactPowers()
    {
        Assert.AreEqual(DimensionVector.InformationDimension,
            StandardExpressionUnitResolver.ResolveInherited("information.byte").Dimension);
        var decimalIds = new[] { "si.kilobyte", "si.megabyte", "si.gigabyte", "si.terabyte",
            "si.petabyte", "si.exabyte", "si.zettabyte", "si.yottabyte" };
        var binaryIds = new[] { "iec.kibibyte", "iec.mebibyte", "iec.gibibyte", "iec.tebibyte",
            "iec.pebibyte", "iec.exbibyte", "iec.zebibyte", "iec.yobibyte" };
        for (var i = 0; i < 8; i++)
        {
            AssertScale(decimalIds[i], new ExactRational(BigInteger.Pow(1000, i + 1), BigInteger.One));
            AssertScale(binaryIds[i], new ExactRational(BigInteger.Pow(1024, i + 1), BigInteger.One));
        }
    }

    [TestMethod]
    public void AurasThreeCupProfilesAndThreePintProfilesRequireExplicitSelection()
    {
        var cups = StandardExpressionUnitResolver.FindInheritedCandidates("cup");
        var pints = StandardExpressionUnitResolver.FindInheritedCandidates("pint");
        Assert.AreEqual(3, cups.Length);
        Assert.AreEqual(3, pints.Length);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            StandardExpressionUnitResolver.ResolveInherited("cup"));
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            StandardExpressionUnitResolver.ResolveInherited("pint"));
        Assert.AreEqual("us.cup", StandardExpressionUnitResolver.ResolveInherited("cup", UnitSystem.UsCustomary).Id);
        Assert.AreEqual("metric.cup", StandardExpressionUnitResolver.ResolveInherited("cup", UnitSystem.MetricInternational).Id);
        Assert.AreEqual("au.cup", StandardExpressionUnitResolver.ResolveInherited("cup", UnitSystem.AustralianCulinary).Id);
        Assert.AreEqual("us.pint", StandardExpressionUnitResolver.ResolveInherited("pint", UnitSystem.UsCustomary).Id);
        Assert.AreEqual("imperial.pint", StandardExpressionUnitResolver.ResolveInherited("pint", UnitSystem.BritishImperial).Id);
        Assert.AreEqual("metric.pint", StandardExpressionUnitResolver.ResolveInherited("pint", UnitSystem.MetricInternational).Id);
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            StandardExpressionUnitResolver.ResolveInherited("pint", UnitSystem.AustralianCulinary));
    }

    [TestMethod]
    public void CustomInitialCatalogueAndInheritedAurasPintPolicyRemainDistinct()
    {
        Assert.AreEqual("au-beer-pint-570ml",
            StandardExpressionUnitResolver.Resolve("pint", UnitSystem.AustralianCulinary).Id);
        Assert.AreEqual("metric.pint",
            StandardExpressionUnitResolver.ResolveInherited("metric.pint").Id);
        Assert.AreEqual(new ExactRational(1, 2000),
            StandardExpressionUnitResolver.ResolveInherited("metric.pint").ScaleToBase);
    }

    [TestMethod]
    public void InheritedSourceKeepsExactAffineTemperatures()
    {
        var celsius = StandardExpressionUnitResolver.ResolveInherited("si.celsius");
        var fahrenheit = StandardExpressionUnitResolver.ResolveInherited("us.fahrenheit");
        Assert.AreEqual(UnitTransformKind.AbsoluteTemperature, celsius.TransformKind);
        Assert.AreEqual(UnitTransformKind.AbsoluteTemperature, fahrenheit.TransformKind);
        Assert.AreEqual(new ExactRational(27315, 100), celsius.OffsetToBase);
        Assert.AreEqual(new ExactRational(45967, 180), fahrenheit.OffsetToBase);
        Assert.AreEqual(celsius.ToBase(TemperatureMeasurement.Absolute(ExactRational.Zero)),
            fahrenheit.ToBase(TemperatureMeasurement.Absolute(new ExactRational(32,1))));
    }

    [TestMethod]
    public void FrenchAliasDoesNotMeanFrenchCultureChoosesImperialUnits()
    {
        Assert.AreEqual("imperial.inch",
            StandardExpressionUnitResolver.ResolveInherited("pouce").Id);
        Assert.AreEqual("imperial.pound",
            StandardExpressionUnitResolver.ResolveInherited("livre").Id);
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            StandardExpressionUnitResolver.ResolveInherited("EUR"));
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            StandardExpressionUnitResolver.ResolveInherited("dB"));
    }

    [TestMethod]
    public void HostCultureNeverSelectsAmbiguousOriginalAliases()
    {
        var before = CultureInfo.CurrentCulture;
        try
        {
            foreach (var culture in new[] { "en-US", "en-GB", "fr-BE", "tr-TR" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
                Assert.ThrowsExactly<InvalidOperationException>(() =>
                    StandardExpressionUnitResolver.ResolveInherited("pint"));
                Assert.AreEqual("us.pint",
                    StandardExpressionUnitResolver.ResolveInherited("pint", UnitSystem.UsCustomary).Id);
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = before;
        }
    }

    [TestMethod]
    public void AdaptedCatalogueDependsOnNoHostLocalizationAssembly()
    {
        var names = typeof(InheritedStandardUnitCatalog).Assembly.GetReferencedAssemblies()
            .Select(x => x.Name).ToArray();
        Assert.IsFalse(names.Contains("dw.localization", StringComparer.OrdinalIgnoreCase));
        Assert.IsFalse(names.Any(name => name is not null && name.StartsWith("aura.", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void EveryInheritedUnitPerformsAnExactBaseRoundTrip()
    {
        var numerator = BigInteger.Pow(2, 127) + 1;
        var value = new ExactRational(numerator, 7);
        foreach (var unit in InheritedStandardUnitCatalog.All)
        {
            Assert.AreEqual(value, unit.FromBase(unit.ToBase(value)), unit.Id);
            Assert.AreEqual(value, PureUnitConverter.Convert(value, unit, unit), unit.Id);
        }
    }

    private static void AssertScale(string id, ExactRational scale)
    {
        var unit = StandardExpressionUnitResolver.ResolveInherited(id);
        Assert.AreEqual(scale, unit.ScaleToBase, id);
    }
}
