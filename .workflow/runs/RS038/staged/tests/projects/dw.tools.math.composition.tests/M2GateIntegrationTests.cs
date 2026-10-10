using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dw.Tools.Math.Composition.Tests;

/// <summary>Independent cross-layer M2 gate oracles, not algebraic-equivalence proofs.</summary>
[TestClass]
public sealed class M2GateIntegrationTests
{
    private static UnitDefinition Unit(string id, DimensionVector dimension, ExactRational scale) =>
        new(id, id, id, UnitSystem.Si, dimension, scale, ExactRational.Zero, UnitTransformKind.Linear);

    [TestMethod]
    public void ExactQuantityToIrRoundTripRetainsNumeratorDenominatorAndDimension()
    {
        var exact = new ExactRational(1, 3);
        var expression = new IrQuantityLiteral(new IrExactScalar(exact), "si.m",
            UnitSystem.Si, DimensionVector.LengthDimension);
        var decoded = (IrQuantityLiteral)IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(expression));
        Assert.AreEqual(exact, ((IrExactScalar)decoded.Value).Value);
        Assert.AreEqual(DimensionVector.LengthDimension, decoded.Dimension);
        Assert.AreEqual("si.m", decoded.CanonicalUnitId);
    }

    [TestMethod]
    public void IrExactAndApproximateAreDistinctAcrossTheCompositionBoundary()
    {
        var origin = MathResultProvenance.Create("matrix.inspect", "math-composition/1");
        var exact = MathOperationResult.Exact(new IrExactScalar(new ExactRational(1, 3)), origin);
        var approx = MathOperationResult.Approximate(IrFiniteBinary64Scalar.FromDouble(0.25), origin);
        Assert.AreEqual(ComputationStatus.Exact, exact.Status);
        Assert.AreEqual(ComputationStatus.Approximate, approx.Status);
        Assert.ThrowsExactly<ArgumentException>(() =>
            MathOperationResult.Exact(IrFiniteBinary64Scalar.FromDouble(0.25), origin));
    }

    [TestMethod]
    public void RestrictedIrStructuralIdentityKeepsItsDomainAndExclusion()
    {
        var x = IrSymbol.Free("x", "x", IrScalarDomain.Real);
        var expr = IrApply.Create(IrOperation.Add, [x, new IrExactScalar(ExactRational.One)]);
        var restricted = new IrRestrictedExpression(expr, IrAssumptionSet.Create(
            [new IrRelation(x, IrRelationKind.NotEqual, new IrExactScalar(ExactRational.One))]));
        var roundTrip = IrCanonicalJsonCodec.Decode(IrCanonicalJsonCodec.Encode(restricted));
        Assert.AreEqual(IrCanonicalHashes.SemanticStructuralSha256(restricted),
            IrCanonicalHashes.SemanticStructuralSha256(roundTrip));
        Assert.AreNotEqual(IrCanonicalHashes.SemanticStructuralSha256(expr),
            IrCanonicalHashes.SemanticStructuralSha256(restricted));
    }

    [TestMethod]
    public void MatrixDescriptorIsInspectableWithoutProviderPermission()
    {
        var matrix = IrMatrix.Create(2, 2,
            [new IrExactScalar(ExactRational.One), new IrExactScalar(ExactRational.Zero),
             new IrExactScalar(ExactRational.Zero), new IrExactScalar(ExactRational.One)]);
        Assert.IsTrue(MathOperationCapability.MatrixInspection.Accepts(matrix));
        Assert.IsFalse(MathOperationCapability.MatrixInspection.Accepts(
            new IrExactScalar(ExactRational.One)));
        Assert.AreEqual(MathValueKind.Matrix, MathValueDescriptor.Inspect(matrix).Kind);
    }

    [TestMethod]
    public void DirectAndFluentExactPipelineAgreeOnOneHundredFiftyKilometresPerHour()
    {
        var km = Unit("km", DimensionVector.LengthDimension, new ExactRational(1000, 1));
        var min = Unit("min", DimensionVector.TimeDimension, new ExactRational(60, 1));
        var kmh = Unit("km/h", DimensionVector.LengthDimension - DimensionVector.TimeDimension,
            new ExactRational(5, 18));
        var steps = new[] { ExactPipelineStep.Start(new ExactRational(5, 1), km),
            ExactPipelineStep.Divide(new ExactRational(2, 1), min),
            ExactPipelineStep.Convert(kmh) };
        var direct = ExactQuantityPipeline.Evaluate(steps);
        var fluent = ExactQuantityPipeline.From(new ExactRational(5, 1), km)
            .DivideBy(new ExactRational(2, 1), min).ConvertTo(kmh).Evaluate();
        Assert.AreEqual(new ExactRational(150, 1), direct.DisplayValue);
        Assert.AreEqual(direct.BaseQuantity, fluent.BaseQuantity);
        Assert.AreEqual(direct.DisplayValue, fluent.DisplayValue);
    }

    [TestMethod]
    public void IncompatibleDimensionIsRejectedWithoutAPartialExactResult()
    {
        var m = Unit("m", DimensionVector.LengthDimension, ExactRational.One);
        var kg = Unit("kg", DimensionVector.MassDimension, ExactRational.One);
        var steps = new[] { ExactPipelineStep.Start(ExactRational.One, m),
            ExactPipelineStep.Add(ExactRational.One, kg) };
        var context = ExactReplayContext.Create("assumptions/1", "policy/1", "catalog/1",
            "exact", "none/1");
        var result = ExactReplayRunner.TryRun(steps, context);
        Assert.AreEqual(ExactReplayStatus.Unsupported, result.Status);
        Assert.IsFalse(result.HasFinalValue);
        Assert.IsNull(result.Receipt);
    }

    [TestMethod]
    public void ExactReplayCacheVersionChangeCreatesAnIndependentIdentity()
    {
        var m = Unit("m", DimensionVector.LengthDimension, ExactRational.One);
        var steps = new[] { ExactPipelineStep.Start(new ExactRational(5, 1), m) };
        var a = ExactReplayContext.Create("a/1", "p/1", "c/1", "exact", "none/1");
        var b = ExactReplayContext.Create("a/1", "p/1", "c/2", "exact", "none/1");
        var cache = new ExactReplayCache(2);
        var one = ExactReplayRunner.TryRun(steps, a, cache);
        var two = ExactReplayRunner.TryRun(steps, a, cache);
        var three = ExactReplayRunner.TryRun(steps, b, cache);
        Assert.IsFalse(one.Receipt!.CacheHit);
        Assert.IsTrue(two.Receipt!.CacheHit);
        Assert.IsFalse(three.Receipt!.CacheHit);
        Assert.AreNotEqual(one.Receipt.Key.Digest, three.Receipt.Key.Digest);
        Assert.AreEqual(2, cache.Count);
    }

    [TestMethod]
    public void CancelledReplayCannotReturnFinalValueOrMutateCache()
    {
        var m = Unit("m", DimensionVector.LengthDimension, ExactRational.One);
        var steps = new[] { ExactPipelineStep.Start(ExactRational.One, m) };
        var context = ExactReplayContext.Create("a/1", "p/1", "c/1", "exact", "none/1");
        var cache = new ExactReplayCache(1);
        using var source = new CancellationTokenSource();
        source.Cancel();
        var result = ExactReplayRunner.TryRun(steps, context, cache, source.Token);
        Assert.AreEqual(ExactReplayStatus.Cancelled, result.Status);
        Assert.IsNull(result.Receipt);
        Assert.AreEqual(0, cache.Count);
    }
}
