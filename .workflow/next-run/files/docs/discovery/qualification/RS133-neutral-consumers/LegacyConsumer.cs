using dw.quantities;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Ir;

static void Need(bool ok, string detail)
{
    if (!ok) throw new InvalidOperationException("RS133 legacy Math consumer: " + detail);
}

static UnitDefinition Linear(string id, DimensionVector dimension, ExactRational scale) =>
    new(id, id, id, UnitSystem.Si, dimension, scale,
        ExactRational.Zero, UnitTransformKind.Linear);

var km = Linear("km", DimensionVector.LengthDimension, new ExactRational(1000, 1));
var metre = Linear("m", DimensionVector.LengthDimension, ExactRational.One);
var minute = Linear("min", DimensionVector.TimeDimension, new ExactRational(60, 1));
var kmh = Linear("km/h",
    DimensionVector.LengthDimension - DimensionVector.TimeDimension,
    new ExactRational(5, 18));
var steps = new[]
{
    ExactPipelineStep.Start(new ExactRational(5, 1), km),
    ExactPipelineStep.Convert(metre),
    ExactPipelineStep.Divide(new ExactRational(2, 1), minute),
    ExactPipelineStep.Convert(kmh)
};
var expected = new ExactRational(150, 1);
var direct = ExactQuantityPipeline.Evaluate(steps);
Need(direct.DisplayValue.Equals(expected), "exact pipeline expected 150 km/h");
Console.WriteLine("LEGACY-CALL|math.quantity.exact-pipeline.evaluate|PASS");

var context = ExactReplayContext.Create(
    "assumptions/a", "policy/1", "catalog/1", "exact", "none/1");
var replay = ExactReplayRunner.TryRun(
    steps, context, new ExactReplayCache(2), CancellationToken.None);
Need(replay.HasFinalValue && replay.Receipt is not null &&
     replay.Receipt.Result.DisplayValue.Equals(expected),
     "typed exact replay result changed");
Console.WriteLine("LEGACY-CALL|math.replay.try-evaluate|PASS");

var ir = new IrExactScalar(new ExactRational(1, 3));
var encoded = IrCanonicalJsonCodec.Encode(ir);
var decoded = IrCanonicalJsonCodec.Decode(encoded);
Need(decoded is IrExactScalar scalar &&
     scalar.Value.Equals(new ExactRational(1, 3)),
     "canonical IR round trip changed");
Console.WriteLine("LEGACY-CALL|math.ir.canonical.encode|PASS");

var digest = IrCanonicalHashes.SemanticStructuralSha256(ir);
Need(digest.Length == 64 &&
     digest.All(c => (c >= '0' && c <= '9') ||
                     (c >= 'a' && c <= 'f')) &&
     digest == IrCanonicalHashes.SemanticStructuralSha256(decoded),
     "semantic structural hash changed");
Console.WriteLine("LEGACY-CALL|math.ir.hash.semantic|PASS");
Console.WriteLine("LEGACY-MATH-CONSUMER|PASS|4");
