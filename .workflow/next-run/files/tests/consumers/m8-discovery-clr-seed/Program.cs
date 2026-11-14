using System.Reflection;
using dw.quantities;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Ir;

var checks = new (string Capability, Type Owner, string Method, Type[] Parameters, Type Result)[]
{
    ("math.quantity.exact-pipeline.evaluate", typeof(ExactQuantityPipeline), "Evaluate",
        [typeof(IEnumerable<ExactPipelineStep>)], typeof(ExactPipelineResult)),
    ("math.replay.try-evaluate", typeof(ExactReplayRunner), "TryRun",
        [typeof(IEnumerable<ExactPipelineStep>), typeof(ExactReplayContext),
         typeof(ExactReplayCache), typeof(CancellationToken)], typeof(ExactReplayAttempt)),
    ("math.ir.canonical.encode", typeof(IrCanonicalJsonCodec), "Encode",
        [typeof(IrNode)], typeof(string)),
    ("math.ir.hash.semantic", typeof(IrCanonicalHashes), "SemanticStructuralSha256",
        [typeof(IrNode)], typeof(string))
};

foreach (var item in checks)
{
    var method = item.Owner.GetMethod(item.Method,
        BindingFlags.Public | BindingFlags.Static, item.Parameters);
    Require(method is not null && method.ReturnType == item.Result,
        "CLR signature mismatch: " + item.Capability);
}

// Actual typed invocations: check exact number transport and structural identity.
IrNode exact = new IrExactScalar(new ExactRational(1, 2));
var encoded = IrCanonicalJsonCodec.Encode(exact);
Require(encoded.Contains("\"numerator\":\"1\"", StringComparison.Ordinal) &&
        encoded.Contains("\"denominator\":\"2\"", StringComparison.Ordinal),
    "Exact rational transport changed.");
var restored = IrCanonicalJsonCodec.Decode(encoded);
Require(IrCanonicalHashes.SemanticStructuralSha256(exact) ==
        IrCanonicalHashes.SemanticStructuralSha256(restored),
    "Canonical structural hash changed after round-trip.");
var relabeled = IrSymbol.Free("a", "first");
var anotherLabel = IrSymbol.Free("a", "second");
Require(IrCanonicalHashes.SemanticStructuralSha256(relabeled) ==
        IrCanonicalHashes.SemanticStructuralSha256(anotherLabel),
    "Passive labels changed the semantic structural hash.");
Require(IrCanonicalHashes.PresentationSha256(relabeled) !=
        IrCanonicalHashes.PresentationSha256(anotherLabel),
    "Presentation digest did not preserve labels.");

// Positive selected Composition call: exact, bounded, typed input.
static UnitDefinition Linear(string id, DimensionVector dimension, ExactRational scale) =>
    new(id, id, id, UnitSystem.Si, dimension, scale,
        ExactRational.Zero, UnitTransformKind.Linear);
var km = Linear("km", DimensionVector.LengthDimension, new ExactRational(1000, 1));
var metre = Linear("m", DimensionVector.LengthDimension, ExactRational.One);
var minute = Linear("min", DimensionVector.TimeDimension, new ExactRational(60, 1));
var kmPerHour = Linear("km/h",
    DimensionVector.LengthDimension - DimensionVector.TimeDimension,
    new ExactRational(5, 18));
var boundedSteps = new[]
{
    ExactPipelineStep.Start(new ExactRational(5, 1), km),
    ExactPipelineStep.Convert(metre),
    ExactPipelineStep.Divide(new ExactRational(2, 1), minute),
    ExactPipelineStep.Convert(kmPerHour)
};
var directQuantity = ExactQuantityPipeline.Evaluate(boundedSteps);
Require(directQuantity.DisplayValue == new ExactRational(150, 1) &&
        directQuantity.BaseQuantity.Value == new ExactRational(125, 3) &&
        directQuantity.BaseQuantity.Dimension ==
            DimensionVector.LengthDimension - DimensionVector.TimeDimension &&
        directQuantity.PresentationUnit?.Id == "km/h" &&
        directQuantity.DetailedSteps.Length == 4,
    "Selected positive exact-pipeline invocation changed.");

// Positive selected Replay TryRun call; cache and context are local, not runtime permissions.
var replayContext = ExactReplayContext.Create(
    "assumptions/1", "policy/1", "catalog/1", "exact", "none/1");
var replayCache = new ExactReplayCache(2);
var firstReplay = ExactReplayRunner.TryRun(boundedSteps, replayContext, replayCache);
var cachedReplay = ExactReplayRunner.TryRun(boundedSteps, replayContext, replayCache);
Require(firstReplay.Status == ExactReplayStatus.Exact &&
        firstReplay.HasFinalValue && firstReplay.Receipt is { CacheHit: false } &&
        firstReplay.Receipt.Result.DisplayValue == new ExactRational(150, 1) &&
        cachedReplay.Status == ExactReplayStatus.Exact &&
        cachedReplay.HasFinalValue && cachedReplay.Receipt is { CacheHit: true } &&
        cachedReplay.Receipt.Result.DisplayValue == new ExactRational(150, 1) &&
        cachedReplay.Receipt.Key.Digest == firstReplay.Receipt.Key.Digest &&
        replayCache.Count == 1,
    "Selected positive replay/cached result changed.");
var changedContext = ExactReplayContext.Create(
    "assumptions/2", "policy/1", "catalog/1", "exact", "none/1");
var isolatedReplay = ExactReplayRunner.TryRun(boundedSteps, changedContext, replayCache);
Require(isolatedReplay.Status == ExactReplayStatus.Exact &&
        isolatedReplay.Receipt is { CacheHit: false } &&
        isolatedReplay.Receipt.Key.Digest != firstReplay.Receipt.Key.Digest &&
        replayCache.Count == 2,
    "Replay cache admitted cross-context reuse.");

Console.WriteLine("M8-DISCOVERY-CLR-SEED|bindings=4|examples=5|positiveComposition=2|brokerReady=0|scope=source-project-consumer");

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
