using System.Globalization;
using dw.quantities;
using dw.quantities.standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C01Tests
{
    [TestMethod]
    public void CatalogHasStableExplicitVersionAndStandaloneNamespace()
    {
        Assert.AreEqual("2026-10-exact-v1", StandardUnitCatalog.CatalogVersion);
        Assert.IsTrue(StandardUnitCatalog.Units.Length >= 14);
        Assert.AreEqual("dw.quantities.standard", typeof(StandardUnitCatalog).Namespace);
        Assert.IsTrue(StandardUnitCatalog.PassiveNames.ContainsKey("fr"));
    }

    [TestMethod]
    public void InchAndIecInformationUnitsUseExactHandComputableConstants()
    {
        var inch = StandardExpressionUnitResolver.Resolve("inch");
        var metre = StandardExpressionUnitResolver.Resolve("m");
        Assert.AreEqual(new ExactRational(127, 5000),
            PureUnitConverter.Convert(ExactRational.One, inch, metre));

        var kibibyte = StandardExpressionUnitResolver.Resolve("KiB");
        var bytes = StandardExpressionUnitResolver.Resolve("B");
        Assert.AreEqual(new ExactRational(1024, 1),
            PureUnitConverter.Convert(ExactRational.One, kibibyte, bytes));
        Assert.AreEqual(DimensionVector.InformationDimension, kibibyte.Dimension);
    }

    [TestMethod]
    public void CupAndPintRequireExplicitProfileForAmbiguousTokens()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            StandardExpressionUnitResolver.Resolve("cup"));
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            StandardExpressionUnitResolver.Resolve("pint"));
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            StandardExpressionUnitResolver.Resolve("cup", UnitSystem.Si));
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            StandardExpressionUnitResolver.Resolve("GBP"));
    }

    [TestMethod]
    public void UsUkAustralianVolumeProfilesHaveExplicitAndDifferentExactConstants()
    {
        var usCup = StandardExpressionUnitResolver.Resolve("cup", UnitSystem.UsCustomary);
        var ukCup = StandardExpressionUnitResolver.Resolve("cup", UnitSystem.BritishImperial);
        var auCup = StandardExpressionUnitResolver.Resolve("cup", UnitSystem.AustralianCulinary);
        var usPint = StandardExpressionUnitResolver.Resolve("pint", UnitSystem.UsCustomary);
        var ukPint = StandardExpressionUnitResolver.Resolve("pint", UnitSystem.BritishImperial);
        var auPint = StandardExpressionUnitResolver.Resolve("pint", UnitSystem.AustralianCulinary);

        Assert.AreEqual(new ExactRational(2365882365, 10000000000000L), usCup.ScaleToBase);
        Assert.AreEqual(new ExactRational(284130625, 1000000000000L), ukCup.ScaleToBase);
        Assert.AreEqual(new ExactRational(1, 4000), auCup.ScaleToBase);
        Assert.AreEqual(new ExactRational(473176473, 1000000000000L), usPint.ScaleToBase);
        Assert.AreEqual(new ExactRational(56826125, 100000000000L), ukPint.ScaleToBase);
        Assert.AreEqual(new ExactRational(57, 100000), auPint.ScaleToBase);
        Assert.AreEqual("au-beer-pint-570ml", auPint.Id);
        foreach (var entry in new[] { usCup, ukCup, auCup, usPint, ukPint, auPint })
            Assert.AreEqual(StandardUnitCatalog.Volume, entry.Dimension);
        Assert.AreEqual(ExactRational.One,
            PureUnitConverter.Convert(ExactRational.One, usCup, usCup));
    }

    [TestMethod]
    public void ProcessCultureCannotChangeResolutionOrConversion()
    {
        var priorCulture = CultureInfo.CurrentCulture;
        var priorUi = CultureInfo.CurrentUICulture;
        try
        {
            foreach (var cultureName in new[] { "en-US", "fr-BE", "nl-BE" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
                var inch = StandardExpressionUnitResolver.Resolve("inch");
                var metre = StandardExpressionUnitResolver.Resolve("m");
                Assert.AreEqual(new ExactRational(127, 5000),
                    PureUnitConverter.Convert(ExactRational.One, inch, metre));
                Assert.ThrowsExactly<InvalidOperationException>(() =>
                    StandardExpressionUnitResolver.Resolve("cup"));
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = priorCulture;
            CultureInfo.CurrentUICulture = priorUi;
        }
    }

    [TestMethod]
    public void MismatchedDimensionsAreRejectedAndNoUnsafeUnitKindsAreAdmitted()
    {
        var inch = StandardExpressionUnitResolver.Resolve("inch");
        var seconds = StandardExpressionUnitResolver.Resolve("s");
        Assert.ThrowsExactly<ArgumentException>(() =>
            PureUnitConverter.Convert(ExactRational.One, inch, seconds));
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            StandardExpressionUnitResolver.Resolve("dB"));
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            StandardExpressionUnitResolver.Resolve("USD"));
    }

    [TestMethod]
    public void DecimalOrLocalizedAliasesNeverSelectAPintProfileImplicitly()
    {
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            StandardExpressionUnitResolver.Resolve("pints"));
        Assert.AreEqual("us-liquid-pint",
            StandardExpressionUnitResolver.Resolve("pints", UnitSystem.UsCustomary).Id);
        Assert.AreEqual("uk-imperial-pint",
            StandardExpressionUnitResolver.Resolve("pints", UnitSystem.BritishImperial).Id);
    }
}
