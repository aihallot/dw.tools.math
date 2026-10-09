using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

[TestClass]
public sealed class M2W02C03BoundaryTests
{
    private static UnitDefinition Unit(string id, DimensionVector dimension,
        ExactRational scale) =>
        new(id, id, id, UnitSystem.Si, dimension, scale,
            ExactRational.Zero, UnitTransformKind.Linear);

    private static readonly UnitDefinition Metre = Unit("m",
        DimensionVector.LengthDimension, ExactRational.One);
    private static readonly UnitDefinition Minute = Unit("min",
        DimensionVector.TimeDimension, new ExactRational(60, 1));
    private static readonly UnitDefinition Mass = Unit("kg",
        DimensionVector.MassDimension, ExactRational.One);
    private static ExactReplayContext Context(string provider = "none/1", string catalog = "catalog/1") =>
        ExactReplayContext.Create("assumptions/1", "policy/1", catalog, "exact", provider);
    private static ExactPipelineStep[] Valid(int value = 5) =>
        [ExactPipelineStep.Start(new ExactRational(value, 1), Metre)];

    [TestMethod]
    public void SuccessfulAttemptHasExactlyOneFinalReceipt()
    {
        var outcome = ExactReplayRunner.TryRun(Valid(), Context());
        Assert.AreEqual(ExactReplayStatus.Exact, outcome.Status);
        Assert.IsTrue(outcome.HasFinalValue);
        Assert.IsNotNull(outcome.Receipt);
        Assert.IsNull(outcome.Reason);
        Assert.AreEqual(new ExactRational(5, 1), outcome.Receipt.Result.DisplayValue);
        Assert.IsFalse(outcome.Receipt.CacheHit);
    }

    [TestMethod]
    public void InvalidDimensionsProduceUnsupportedWithoutAPartialFinalValue()
    {
        var cache = new ExactReplayCache();
        var input = new[] {
            ExactPipelineStep.Start(ExactRational.One, Metre),
            ExactPipelineStep.Add(ExactRational.One, Mass) };
        var outcome = ExactReplayRunner.TryRun(input, Context(), cache);
        Assert.AreEqual(ExactReplayStatus.Unsupported, outcome.Status);
        Assert.IsFalse(outcome.HasFinalValue);
        Assert.IsNull(outcome.Receipt);
        Assert.IsNotNull(outcome.Reason);
        Assert.IsTrue(outcome.Reason.Length <= 256);
        Assert.AreEqual(0, cache.Count);
    }

    [TestMethod]
    public void ZeroDenominatorOutcomeIsUnsupportedNotAnExactResult()
    {
        var outcome = ExactReplayRunner.TryRun(
            [ExactPipelineStep.Start(ExactRational.One, Metre),
             ExactPipelineStep.Divide(ExactRational.Zero, Minute)], Context());
        Assert.AreEqual(ExactReplayStatus.Unsupported, outcome.Status);
        Assert.IsNull(outcome.Receipt);
        Assert.IsFalse(outcome.HasFinalValue);
    }

    [TestMethod]
    public void OversizeNumeralIsExplicitBudgetExceededAndNotCached()
    {
        var cache = new ExactReplayCache();
        var huge = new ExactRational(BigInteger.Pow(10, 256), BigInteger.One);
        var outcome = ExactReplayRunner.TryRun(
            [ExactPipelineStep.Start(huge, Metre)], Context(), cache);
        Assert.AreEqual(ExactReplayStatus.BudgetExceeded, outcome.Status);
        Assert.IsNull(outcome.Receipt);
        Assert.IsFalse(outcome.HasFinalValue);
        Assert.AreEqual(0, cache.Count);
    }

    [TestMethod]
    public void ExcessiveStepCountHasNoPartialResultAndNoCacheEntry()
    {
        var input = new List<ExactPipelineStep> {
            ExactPipelineStep.Start(ExactRational.One, Metre) };
        for (var i = 0; i < 16; i++) input.Add(ExactPipelineStep.Convert(Metre));
        var cache = new ExactReplayCache(1);
        var outcome = ExactReplayRunner.TryRun(input, Context(), cache);
        Assert.AreEqual(ExactReplayStatus.Unsupported, outcome.Status);
        Assert.IsFalse(outcome.HasFinalValue);
        Assert.IsNull(outcome.Receipt);
        Assert.AreEqual(0, cache.Count);
    }

    [TestMethod]
    public void DimensionExponentOverflowIsReportedAsBudgetExceeded()
    {
        var extreme = Unit("extreme", new DimensionVector(-1, 0, 0, 0, 0, 0, 0),
            ExactRational.One);
        var start = Unit("start", new DimensionVector(int.MaxValue, 0, 0, 0, 0, 0, 0),
            ExactRational.One);
        var outcome = ExactReplayRunner.TryRun(
            [ExactPipelineStep.Start(ExactRational.One, start),
             ExactPipelineStep.Divide(ExactRational.One, extreme)], Context());
        Assert.AreEqual(ExactReplayStatus.BudgetExceeded, outcome.Status);
        Assert.IsNull(outcome.Receipt);
    }

    [TestMethod]
    public void CancellationRetainsIncompleteStateAndCannotEvictCache()
    {
        var cache = new ExactReplayCache(1);
        var initial = ExactReplayRunner.TryRun(Valid(5), Context(), cache);
        Assert.AreEqual(ExactReplayStatus.Exact, initial.Status);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var aborted = ExactReplayRunner.TryRun(Valid(6), Context(), cache, cancellation.Token);
        Assert.AreEqual(ExactReplayStatus.Cancelled, aborted.Status);
        Assert.IsNull(aborted.Receipt);
        Assert.IsFalse(aborted.HasFinalValue);
        Assert.AreEqual(1, cache.Count);
        var hit = ExactReplayRunner.TryRun(Valid(5), Context(), cache);
        Assert.IsNotNull(hit.Receipt);
        Assert.IsTrue(hit.Receipt.CacheHit);
    }

    [TestMethod]
    public void CancellationDuringEnumerationDoesNotStoreIntermediateValue()
    {
        using var cts = new CancellationTokenSource();
        IEnumerable<ExactPipelineStep> Enumerate()
        {
            yield return ExactPipelineStep.Start(ExactRational.One, Metre);
            cts.Cancel();
            yield return ExactPipelineStep.Convert(Metre);
        }
        var cache = new ExactReplayCache();
        var outcome = ExactReplayRunner.TryRun(Enumerate(), Context(), cache, cts.Token);
        Assert.AreEqual(ExactReplayStatus.Cancelled, outcome.Status);
        Assert.IsNull(outcome.Receipt);
        Assert.AreEqual(0, cache.Count);
    }

    [TestMethod]
    public void CacheCapacity32IsStrictAndOldestEntryIsEvicted()
    {
        var cache = new ExactReplayCache(ExactReplayCache.MaximumEntries);
        for (var i = 1; i <= 33; i++)
        {
            var result = ExactReplayRunner.TryRun(Valid(i), Context(), cache);
            Assert.AreEqual(ExactReplayStatus.Exact, result.Status);
        }
        Assert.AreEqual(32, cache.Count);
        Assert.IsFalse(ExactReplayRunner.TryRun(Valid(1), Context(), cache).Receipt!.CacheHit);
        Assert.AreEqual(32, cache.Count);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ExactReplayCache(33));
    }

    [TestMethod]
    public void InvalidAttemptDoesNotDisplaceAnExistingCacheHit()
    {
        var cache = new ExactReplayCache(1);
        var success = ExactReplayRunner.TryRun(Valid(), Context(), cache);
        var invalid = ExactReplayRunner.TryRun(
            [ExactPipelineStep.Start(ExactRational.One, Metre),
             ExactPipelineStep.Add(ExactRational.One, Mass)], Context(), cache);
        Assert.AreEqual(ExactReplayStatus.Unsupported, invalid.Status);
        Assert.AreEqual(1, cache.Count);
        var replay = ExactReplayRunner.TryRun(Valid(), Context(), cache);
        Assert.IsNotNull(replay.Receipt);
        Assert.IsTrue(replay.Receipt.CacheHit);
        Assert.AreSame(success.Receipt!.Result, replay.Receipt.Result);
    }

    [TestMethod]
    public void ChangedProviderAndCatalogVersionsNeverReuseCachedResults()
    {
        var cache = new ExactReplayCache(3);
        var first = ExactReplayRunner.TryRun(Valid(), Context(), cache);
        var providerChange = ExactReplayRunner.TryRun(Valid(), Context(provider: "none/2"), cache);
        var catalogChange = ExactReplayRunner.TryRun(Valid(), Context(catalog: "catalog/2"), cache);
        Assert.AreEqual(3, cache.Count);
        Assert.IsFalse(first.Receipt!.CacheHit);
        Assert.IsFalse(providerChange.Receipt!.CacheHit);
        Assert.IsFalse(catalogChange.Receipt!.CacheHit);
        Assert.AreNotEqual(first.Receipt.Key.Digest, providerChange.Receipt.Key.Digest);
        Assert.AreNotEqual(first.Receipt.Key.Digest, catalogChange.Receipt.Key.Digest);
    }

    [TestMethod]
    public void UnknownProviderLabelDoesNotGrantAuthorization()
    {
        var outcome = ExactReplayRunner.TryRun(Valid(), Context(provider: "uninstalled/42"));
        Assert.AreEqual(ExactReplayStatus.Exact, outcome.Status);
        Assert.IsNull(typeof(ExactReplayAttempt).GetProperty("Permission"));
        Assert.IsNull(typeof(ExactReplayAttempt).GetProperty("ProviderExecution"));
        Assert.IsNull(typeof(ExactReplayAttempt).GetProperty("Binary64Guarantee"));
    }

    [TestMethod]
    public void EmptyOrNullArgumentsAreNotMisreportedAsCompletedWork()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            ExactReplayRunner.TryRun(null!, Context()));
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            ExactReplayRunner.TryRun(Valid(), null!));
    }
}
