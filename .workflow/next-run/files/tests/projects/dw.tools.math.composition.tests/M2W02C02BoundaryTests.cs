using System.Numerics;
using dw.quantities;
using Dw.Tools.Math.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

[TestClass]
public sealed class M2W02C02BoundaryTests
{
    private static UnitDefinition Linear(string id, DimensionVector dimension, ExactRational scale) =>
        new(id, id, id, UnitSystem.Si, dimension, scale,
            ExactRational.Zero, UnitTransformKind.Linear);

    private static readonly UnitDefinition Km = Linear("km", DimensionVector.LengthDimension,
        new ExactRational(1000, 1));
    private static readonly UnitDefinition Metre = Linear("m", DimensionVector.LengthDimension,
        ExactRational.One);
    private static readonly UnitDefinition Minute = Linear("min", DimensionVector.TimeDimension,
        new ExactRational(60, 1));
    private static readonly UnitDefinition KmPerHour = Linear("km/h",
        DimensionVector.LengthDimension - DimensionVector.TimeDimension,
        new ExactRational(5, 18));

    [TestMethod]
    public void DirectAndFluentCancellationOverloadsPreserveExactResult()
    {
        var steps = new[] {
            ExactPipelineStep.Start(new ExactRational(5, 1), Km),
            ExactPipelineStep.Convert(Metre),
            ExactPipelineStep.Divide(new ExactRational(2, 1), Minute),
            ExactPipelineStep.Convert(KmPerHour)
        };
        var direct = ExactQuantityPipeline.Evaluate(steps, CancellationToken.None);
        var fluent = ExactQuantityPipeline.From(new ExactRational(5, 1), Km)
            .ConvertTo(Metre).DivideBy(new ExactRational(2, 1), Minute)
            .ConvertTo(KmPerHour).Evaluate(CancellationToken.None);
        Assert.AreEqual(new ExactRational(150, 1), direct.DisplayValue);
        Assert.AreEqual(direct.BaseQuantity, fluent.BaseQuantity);
        CollectionAssert.AreEqual(direct.DetailedSteps.ToArray(), fluent.DetailedSteps.ToArray());
    }

    [TestMethod]
    public void CancellationBeforeValidationReturnsNoPartialResult()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var steps = new[] { ExactPipelineStep.Start(ExactRational.One, Metre) };
        Assert.ThrowsExactly<OperationCanceledException>(() =>
            ExactQuantityPipeline.Evaluate(steps, source.Token));
        Assert.ThrowsExactly<OperationCanceledException>(() =>
            ExactQuantityPipeline.From(ExactRational.One, Metre).Evaluate(source.Token));
    }

    [TestMethod]
    public void CancellationDuringFiniteInputMaterializationIsObserved()
    {
        using var source = new CancellationTokenSource();
        IEnumerable<ExactPipelineStep> Steps()
        {
            yield return ExactPipelineStep.Start(ExactRational.One, Metre);
            source.Cancel();
            yield return ExactPipelineStep.Convert(Metre);
        }
        Assert.ThrowsExactly<OperationCanceledException>(() =>
            ExactQuantityPipeline.Evaluate(Steps(), source.Token));
    }

    [TestMethod]
    public void EveryStepRecordsExactBaseQuantityDimensionAndPresentationIdentity()
    {
        var result = ExactQuantityPipeline.From(new ExactRational(5, 1), Km)
            .ConvertTo(Metre).DivideBy(new ExactRational(2, 1), Minute)
            .ConvertTo(KmPerHour).Evaluate();
        Assert.AreEqual(4, result.DetailedSteps.Length);
        CollectionAssert.AreEqual(result.Steps.ToArray(),
            result.DetailedSteps.Select(x => x.Kind).ToArray());
        for (var i = 0; i < result.DetailedSteps.Length; i++)
            Assert.AreEqual(i, result.DetailedSteps[i].Index);
        Assert.AreEqual(new ExactRational(5000, 1), result.DetailedSteps[0].BaseQuantity.Value);
        Assert.AreEqual("km", result.DetailedSteps[0].PresentationUnitId);
        Assert.AreEqual(new ExactRational(5000, 1), result.DetailedSteps[1].BaseQuantity.Value);
        Assert.AreEqual("m", result.DetailedSteps[1].PresentationUnitId);
        Assert.AreEqual(new ExactRational(125, 3), result.DetailedSteps[2].BaseQuantity.Value);
        Assert.AreEqual(DimensionVector.LengthDimension - DimensionVector.TimeDimension,
            result.DetailedSteps[2].BaseQuantity.Dimension);
        Assert.IsNull(result.DetailedSteps[2].PresentationUnitId);
        Assert.AreEqual("km/h", result.DetailedSteps[3].PresentationUnitId);
    }

    [TestMethod]
    public void AllSixteenStagesHaveBoundedImmutableObservations()
    {
        var pipeline = ExactQuantityPipeline.From(ExactRational.One, Metre);
        for (var i = 0; i < 15; i++) pipeline = pipeline.ConvertTo(Metre);
        var result = pipeline.Evaluate();
        Assert.AreEqual(16, result.Steps.Length);
        Assert.AreEqual(16, result.DetailedSteps.Length);
        Assert.AreEqual(15, result.DetailedSteps[^1].Index);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => pipeline.ConvertTo(Metre));
    }

    [TestMethod]
    public void ExactNumeralMagnitudeAtBoundaryIsAcceptedAndAboveIsRejected()
    {
        var admitted = new ExactRational(BigInteger.Pow(10, 255), BigInteger.One);
        var refused = new ExactRational(BigInteger.Pow(10, 256), BigInteger.One);
        Assert.AreEqual(admitted, ExactQuantityPipeline.From(admitted, Metre)
            .Evaluate().BaseQuantity.Value);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactQuantityPipeline.From(refused, Metre).Evaluate());
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactQuantityPipeline.From(new ExactRational(-refused.Numerator, BigInteger.One), Metre).Evaluate());
    }

    [TestMethod]
    public void InvalidLaterStepFailsBeforeAnyPublicResultIsReturned()
    {
        var mass = Linear("kg", DimensionVector.MassDimension, ExactRational.One);
        var incompatible = new[] {
            ExactPipelineStep.Start(new ExactRational(2, 1), Metre),
            ExactPipelineStep.Convert(Metre),
            ExactPipelineStep.Add(new ExactRational(1, 1), mass)
        };
        Assert.ThrowsExactly<ArgumentException>(() =>
            ExactQuantityPipeline.Evaluate(incompatible, CancellationToken.None));
    }

    [TestMethod]
    public void ZeroDivisorAndExponentOverflowAreDetectedDuringPreflight()
    {
        var oversizedDimension = new DimensionVector(int.MinValue, 0, 0, 0, 0, 0, 0);
        var edge = Linear("edge", oversizedDimension, ExactRational.One);
        Assert.ThrowsExactly<DivideByZeroException>(() => ExactQuantityPipeline.Evaluate(new[] {
            ExactPipelineStep.Start(ExactRational.One, Metre),
            ExactPipelineStep.Divide(ExactRational.Zero, Minute)
        }, CancellationToken.None));
        Assert.ThrowsExactly<OverflowException>(() => ExactQuantityPipeline.Evaluate(new[] {
            ExactPipelineStep.Start(ExactRational.One, Metre),
            ExactPipelineStep.Divide(ExactRational.One, edge)
        }, CancellationToken.None));
    }

    [TestMethod]
    public void ResultsExposeNoPartialMutationOrExternalExecutionIdentity()
    {
        var result = ExactQuantityPipeline.From(new ExactRational(5, 1), Km).Evaluate();
        Assert.AreEqual(1, result.DetailedSteps.Length);
        Assert.AreEqual(result.BaseQuantity, result.DetailedSteps[0].BaseQuantity);
        Assert.IsNull(typeof(ExactPipelineResult).GetProperty("Provider"));
        Assert.IsNull(typeof(ExactPipelineResult).GetProperty("Authorization"));
        Assert.IsNull(typeof(ExactPipelineResult).GetProperty("ExecutionTrace"));
        Assert.AreEqual(0, typeof(ExactPipelineStepSnapshot).GetConstructors().Length);
    }

    [TestMethod]
    public void NegativeMagnitudeAndOversizedScaleAreRefused()
    {
        var negative = new ExactRational(-BigInteger.Pow(10, 256), BigInteger.One);
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactQuantityPipeline.From(negative, Metre).Evaluate());
        var largeScale = Linear("large", DimensionVector.LengthDimension,
            new ExactRational(BigInteger.Pow(10, 256), BigInteger.One));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            ExactQuantityPipeline.From(ExactRational.One, largeScale).Evaluate());
    }
}
