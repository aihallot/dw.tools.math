using System.Globalization;
using System.Numerics;
using dw.quantities;
using dw.quantities.standard;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Quantities.Tests;

[TestClass]
public sealed class M1W02C01BoundaryTests
{
    [TestMethod]
    public void BoundedCandidateDiscoveryShowsAllAmbiguousProfiles()
    {
        var cups = StandardExpressionUnitResolver.FindCandidates("cup");
        var pints = StandardExpressionUnitResolver.FindCandidates("pint");
        Assert.AreEqual(3, cups.Length);
        Assert.AreEqual(3, pints.Length);
        CollectionAssert.AreEquivalent(
            new[] { UnitSystem.UsCustomary, UnitSystem.BritishImperial, UnitSystem.AustralianCulinary },
            cups.Select(x => x.System).ToArray());
        Assert.AreEqual(1, StandardExpressionUnitResolver.FindCandidates("cup", UnitSystem.UsCustomary).Length);
        Assert.AreEqual(0, StandardExpressionUnitResolver.FindCandidates("cup", UnitSystem.Si).Length);
    }

    [TestMethod]
    public void CatalogIdsAreUniqueAndLookupsAreCanonical()
    {
        var ids = StandardUnitCatalog.All.Select(unit => unit.Id).ToArray();
        Assert.AreEqual(ids.Length, ids.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        foreach (var unit in StandardUnitCatalog.All)
        {
            Assert.IsTrue(StandardUnitCatalog.TryGetById(unit.Id.ToUpperInvariant(), out var result));
            Assert.AreEqual(unit, result);
        }
        Assert.IsFalse(StandardUnitCatalog.TryGetById("unknown-unit", out var absent));
        Assert.IsNull(absent);
    }

    [TestMethod]
    public void SymbolsAreCaseSensitiveWhereCaseChangesTheMeaning()
    {
        Assert.AreEqual("byte", StandardExpressionUnitResolver.Resolve("B").Id);
        Assert.ThrowsExactly<KeyNotFoundException>(() => StandardExpressionUnitResolver.Resolve("b"));
        Assert.AreEqual("kibibyte", StandardExpressionUnitResolver.Resolve("KiB").Id);
        Assert.ThrowsExactly<KeyNotFoundException>(() => StandardExpressionUnitResolver.Resolve("kiB"));
        Assert.AreEqual("kibibyte", StandardExpressionUnitResolver.Resolve("KIBIBYTE").Id);
    }

    [TestMethod]
    public void BoundedTokensRejectExcessivelyLongOrEmptyInput()
    {
        var longToken = new string('x', StandardExpressionUnitResolver.MaximumTokenLength + 1);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            StandardExpressionUnitResolver.Resolve(longToken));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            StandardExpressionUnitResolver.FindCandidates(longToken));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            StandardUnitCatalog.TryGetById(longToken, out _));
        Assert.ThrowsExactly<ArgumentException>(() =>
            StandardExpressionUnitResolver.Resolve(" "));
        Assert.ThrowsExactly<ArgumentException>(() =>
            StandardUnitCatalog.TryGetById(" ", out _));
    }

    [TestMethod]
    public void ExplicitAliasProfilesRejectUnresolvedAmbiguity()
    {
        foreach (var token in new[] { "cup", "cups", "pint", "pints" })
        {
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                StandardExpressionUnitResolver.Resolve(token));
            Assert.AreEqual(3, StandardExpressionUnitResolver.FindCandidates(token).Length);
        }
        Assert.AreEqual("us-liquid-cup",
            StandardExpressionUnitResolver.Resolve("cups", UnitSystem.UsCustomary).Id);
        Assert.AreEqual("uk-imperial-pint",
            StandardExpressionUnitResolver.Resolve("pints", UnitSystem.BritishImperial).Id);
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            StandardExpressionUnitResolver.Resolve("USD", UnitSystem.Si));
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            StandardExpressionUnitResolver.Resolve("dB"));
    }

    [TestMethod]
    public void EveryAdmittedUnitConvertsExactlyToAndFromItsOwnBaseScale()
    {
        var value = new ExactRational(BigInteger.Pow(2, 128) + 3, 7);
        foreach (var unit in StandardUnitCatalog.All)
        {
            Assert.AreEqual(value, unit.FromBase(unit.ToBase(value)),
                "The catalog unit should roundtrip exactly: " + unit.Id);
            Assert.AreEqual(value, PureUnitConverter.Convert(value, unit, unit));
            Assert.AreEqual(UnitTransformKind.Linear, unit.TransformKind);
        }
    }

    [TestMethod]
    public void CultureChangesCannotReorderOrChooseAmbiguousCandidates()
    {
        var culture = CultureInfo.CurrentCulture;
        var ui = CultureInfo.CurrentUICulture;
        try
        {
            var expected = StandardExpressionUnitResolver.FindCandidates("cup").Select(x => x.Id).ToArray();
            foreach (var name in new[] { "en-US", "fr-BE", "nl-BE", "tr-TR" })
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(name);
                CollectionAssert.AreEqual(expected,
                    StandardExpressionUnitResolver.FindCandidates("cup").Select(x => x.Id).ToArray());
                Assert.ThrowsExactly<InvalidOperationException>(() =>
                    StandardExpressionUnitResolver.Resolve("cup"));
            }
        }
        finally
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = ui;
        }
    }

    [TestMethod]
    public void ExplicitProfileDoesNotOverrideUniqueCanonicalIdWithDifferentProfile()
    {
        Assert.AreEqual("inch", StandardUnitCatalog.All.Single(x => x.Id == "inch").Id);
        Assert.AreEqual("inch", StandardExpressionUnitResolver.Resolve("inch", UnitSystem.BritishImperial).Id);
        Assert.ThrowsExactly<KeyNotFoundException>(() =>
            StandardExpressionUnitResolver.Resolve("inch", UnitSystem.UsCustomary));
        Assert.AreEqual("inch", StandardExpressionUnitResolver.Resolve("INCH").Id);
    }
}
