using dw.quantities;
using Dw.Tools.Math.Ir;
using Dw.Tools.Math.Composition;

var fraction = new ExactRational(1, 3);
var json = IrCanonicalJsonCodec.Encode(new IrExactScalar(fraction));
if (((IrExactScalar)IrCanonicalJsonCodec.Decode(json)).Value != fraction)
    throw new InvalidOperationException("NuGet consumer IR/quantity roundtrip failed.");

var metre = new UnitDefinition("m", "m", "metre", UnitSystem.Si,
    DimensionVector.LengthDimension, ExactRational.One, ExactRational.Zero,
    UnitTransformKind.Linear);
var steps = new[] { ExactPipelineStep.Start(new ExactRational(5, 1), metre) };
var context = ExactReplayContext.Create("a/1", "p/1", "c/1", "exact", "none/1");
var cache = new ExactReplayCache(2);
var first = ExactReplayRunner.TryRun(steps, context, cache);
var second = ExactReplayRunner.TryRun(steps, context, cache);
if (!first.HasFinalValue || first.Receipt is null ||
    first.Receipt.Result.DisplayValue != new ExactRational(5, 1) ||
    second.Receipt is null || !second.Receipt.CacheHit)
    throw new InvalidOperationException("NuGet consumer composition/replay qualification failed.");

Console.WriteLine("dw.tools.math/m2-gate-consumer/0.3 qualified");
