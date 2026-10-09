using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

[TestClass]
public sealed class M2W02C03Tests
{
    private static UnitDefinition Unit(string id, DimensionVector dimension, ExactRational scale) =>
        new(id, id, id, UnitSystem.Si, dimension, scale, ExactRational.Zero,
            UnitTransformKind.Linear);
    private static readonly UnitDefinition Km = Unit("km", DimensionVector.LengthDimension,
        new ExactRational(1000, 1));
    private static readonly UnitDefinition Metre = Unit("m", DimensionVector.LengthDimension,
        ExactRational.One);
    private static readonly UnitDefinition Minute = Unit("min", DimensionVector.TimeDimension,
        new ExactRational(60, 1));
    private static readonly UnitDefinition KmPerHour = Unit("km/h",
        DimensionVector.LengthDimension - DimensionVector.TimeDimension, new ExactRational(5, 18));

    private static ExactReplayContext Context(string assumptions = "assumptions/a",
        string policy = "policy/1", string catalog = "catalog/1",
        string tolerance = "exact", string provider = "none/1") =>
        ExactReplayContext.Create(assumptions, policy, catalog, tolerance, provider);

    private static ExactPipelineStep[] Steps(int amount = 5) =>
    [
        ExactPipelineStep.Start(new ExactRational(amount, 1), Km),
        ExactPipelineStep.Convert(Metre),
        ExactPipelineStep.Divide(new ExactRational(2, 1), Minute),
        ExactPipelineStep.Convert(KmPerHour)
    ];

    private static string Key(ExactPipelineStep[] steps, ExactReplayContext context) =>
        ExactReplayRunner.Run(steps, context).Key.Digest;

    [TestMethod]
    public void IdenticalExactComputationReplaysWithStableDigestAndValue()
    {
        var first = ExactReplayRunner.Run(Steps(), Context());
        var second = ExactReplayRunner.Run(Steps(), Context());
        Assert.AreEqual(first.Key.Digest, second.Key.Digest);
        Assert.AreEqual(64, first.Key.Digest.Length);
        Assert.AreEqual(new ExactRational(150, 1), second.Result.DisplayValue);
        Assert.AreEqual(first.Result.BaseQuantity, second.Result.BaseQuantity);
        Assert.IsFalse(first.CacheHit);
        Assert.IsFalse(second.CacheHit);
    }

    [TestMethod]
    public void ChangedCanonicalInputInvalidatesTheKey()
    {
        Assert.AreNotEqual(Key(Steps(5), Context()), Key(Steps(6), Context()));
    }

    [TestMethod]
    public void ChangedAssumptionsInvalidateTheKey()
    {
        Assert.AreNotEqual(Key(Steps(), Context()), Key(Steps(), Context(assumptions: "assumptions/b")));
    }

    [TestMethod]
    public void ChangedCalculationPolicyInvalidatesTheKey()
    {
        Assert.AreNotEqual(Key(Steps(), Context()), Key(Steps(), Context(policy: "policy/2")));
    }

    [TestMethod]
    public void ChangedCatalogVersionInvalidatesTheKey()
    {
        Assert.AreNotEqual(Key(Steps(), Context()), Key(Steps(), Context(catalog: "catalog/2")));
    }

    [TestMethod]
    public void ChangedTolerancePolicyInvalidatesTheKey()
    {
        Assert.AreNotEqual(Key(Steps(), Context()), Key(Steps(), Context(tolerance: "epsilon/1")));
    }

    [TestMethod]
    public void ChangedProviderVersionInvalidatesWithoutExecutingAProvider()
    {
        Assert.AreNotEqual(Key(Steps(), Context()), Key(Steps(), Context(provider: "provider/2")));
    }

    [TestMethod]
    public void StepOrderOrExactUnitScaleChangesIdentity()
    {
        var changedOrder = new[] {
            ExactPipelineStep.Start(new ExactRational(5, 1), Km),
            ExactPipelineStep.Divide(new ExactRational(2, 1), Minute),
            ExactPipelineStep.Convert(KmPerHour),
            ExactPipelineStep.Convert(KmPerHour)
        };
        var key = Key(Steps(), Context());
        Assert.AreNotEqual(key, Key(changedOrder, Context()));
        var alternativeUnit = Unit("km", DimensionVector.LengthDimension, new ExactRational(999, 1));
        var scaled = Steps();
        scaled[0] = ExactPipelineStep.Start(new ExactRational(5, 1), alternativeUnit);
        Assert.AreNotEqual(key, Key(scaled, Context()));
    }

    [TestMethod]
    public void OptionalCacheReportsHitsButDoesNotChangeExactValues()
    {
        var cache = new ExactReplayCache();
        var first = ExactReplayRunner.Run(Steps(), Context(), cache);
        var second = ExactReplayRunner.Run(Steps(), Context(), cache);
        Assert.IsFalse(first.CacheHit);
        Assert.IsTrue(second.CacheHit);
        Assert.AreEqual(1, cache.Count);
        Assert.AreSame(first.Result, second.Result);
        Assert.AreEqual(new ExactRational(150, 1), second.Result.DisplayValue);
        var without = ExactReplayRunner.Run(Steps(), Context());
        Assert.IsFalse(without.CacheHit);
        Assert.AreEqual(first.Key.Digest, without.Key.Digest);
    }

    [TestMethod]
    public void OptionalCacheIsFiniteAndEvictsOldestEntry()
    {
        var cache = new ExactReplayCache(2);
        ExactReplayRunner.Run(Steps(5), Context(), cache);
        ExactReplayRunner.Run(Steps(6), Context(), cache);
        ExactReplayRunner.Run(Steps(7), Context(), cache);
        Assert.AreEqual(2, cache.Count);
        var original = ExactReplayRunner.Run(Steps(5), Context(), cache);
        Assert.IsFalse(original.CacheHit);
        Assert.AreEqual(2, cache.Count);
        cache.Clear();
        Assert.AreEqual(0, cache.Count);
    }

    [TestMethod]
    public void InvalidResultsAreNeverCachedAndCancellationPrecedesCacheHits()
    {
        var cache = new ExactReplayCache();
        var bad = new[] {
            ExactPipelineStep.Start(ExactRational.One, Km),
            ExactPipelineStep.Divide(ExactRational.Zero, Minute)
        };
        Assert.ThrowsExactly<DivideByZeroException>(() =>
            ExactReplayRunner.Run(bad, Context(), cache));
        Assert.AreEqual(0, cache.Count);
        ExactReplayRunner.Run(Steps(), Context(), cache);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        Assert.ThrowsExactly<OperationCanceledException>(() =>
            ExactReplayRunner.Run(Steps(), Context(), cache, cancellation.Token));
        Assert.AreEqual(1, cache.Count);
    }

    [TestMethod]
    public void ReplayInputsAndContextsHaveExplicitBounds()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ExactReplayCache(33));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ExactReplayCache(0));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            Context(catalog: new string('c', 129)));
        Assert.ThrowsExactly<ArgumentException>(() =>
            Context(provider: " "));
        var steps = new List<ExactPipelineStep> {
            ExactPipelineStep.Start(ExactRational.One, Metre) };
        for (var i = 0; i < 16; i++) steps.Add(ExactPipelineStep.Convert(Metre));
        Assert.ThrowsExactly<ArgumentException>(() => ExactReplayRunner.Run(steps, Context()));
    }

    [TestMethod]
    public void HugeExactNumeralIsRefusedBeforeHashOrCacheAdmission()
    {
        var huge = new ExactRational(BigInteger.Pow(10, 256), BigInteger.One);
        var cache = new ExactReplayCache();
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactReplayRunner.Run([ExactPipelineStep.Start(huge, Metre)], Context(), cache));
        Assert.AreEqual(0, cache.Count);
    }

    [TestMethod]
    public void DisplayLabelsDoNotImplyProviderAccessOrExternalReplay()
    {
        var receipt = ExactReplayRunner.Run(Steps(), Context(provider: "offline/42"));
        Assert.AreEqual(new ExactRational(150, 1), receipt.Result.DisplayValue);
        Assert.IsFalse(receipt.CacheHit);
        Assert.IsNull(typeof(ExactReplayReceipt).GetProperty("Authorization"));
        Assert.IsNull(typeof(ExactReplayReceipt).GetProperty("ProviderExecution"));
        Assert.IsNull(typeof(ExactReplayCache).GetMethod("Execute"));
    }
}
