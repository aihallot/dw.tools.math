using System.Security.Cryptography;
using Dw.Discovery;
using Dw.Tools.Math.Composition;
using Dw.Tools.Math.Ir;

static void Need(bool ok, string detail)
{
    if (!ok) throw new InvalidOperationException("RS133 neutral Math consumer: " + detail);
}

const string ExpectedDllSha256 =
    "70d38e80963378d8df5ea76fcb59a14e79d0c169c8f5926f4018628370b15064";
var assembly = typeof(DiscoveryCandidateInspector).Assembly;
Need(assembly.GetName().Name == "dw.discovery" &&
     assembly.GetName().Version == new Version(1, 0, 0, 0),
     "neutral loaded assembly identity is not the published CLR 1.0.0.0");
var dll = Convert.ToHexStringLower(SHA256.HashData(
    File.ReadAllBytes(assembly.Location)));
Need(dll == ExpectedDllSha256, "loaded neutral DLL hash differs from the published nupkg DLL");
Console.WriteLine("NEUTRAL-IDENTITY|dw.discovery|20260.20261027.1001|PASS");

var positive = DiscoveryCandidateInspector.Inspect(typeof(Rs133Probe), "math");
Need(positive.IsValid && positive.Methods.Count == 1 &&
     positive.Methods[0].CapabilityId == "math.probe.echo" &&
     !positive.Methods[0].IsAgentCallable &&
     !positive.Methods[0].IsBrokerReady &&
     !positive.Methods[0].IsRuntimeAuthorized &&
     Rs133Probe.Calls == 0,
     "supported primitive-only probe failed or invoked a provider");
Console.WriteLine("NEUTRAL-PRIMITIVE-CONTROL|PASS");

(Type Type, string Id, string Family, string Member, string ExpectedCode)[] cases =
[
    (typeof(ExactQuantityPipeline),
        "math.quantity.exact-pipeline.evaluate", "math.quantity",
        "Evaluate", "sidecar-method-unresolved"),
    (typeof(ExactReplayRunner),
        "math.replay.try-evaluate", "math.replay",
        "TryRun", "unsupported-signature"),
    (typeof(IrCanonicalJsonCodec),
        "math.ir.canonical.encode", "math.ir",
        "Encode", "unsupported-signature"),
    (typeof(IrCanonicalHashes),
        "math.ir.hash.semantic", "math.ir",
        "SemanticStructuralSha256", "unsupported-signature")
];
foreach (var entry in cases)
{
    var observation = DiscoveryCandidateInspector.Inspect(
        entry.Type, "math",
        [new DiscoverySidecarSelection(entry.Id, entry.Family, entry.Member)]);
    Need(!observation.IsValid && observation.Methods.Count == 0 &&
         observation.Diagnostics.Any(d => d.Code == entry.ExpectedCode),
         "unexpected neutral profile for " + entry.Id);
    Console.WriteLine("NEUTRAL-UNSUPPORTED|" + entry.Id + "|" + entry.ExpectedCode);
}
Need(assembly.GetType("Dw.Discovery.DiscoveryCandidateBundleBuilder") is null,
     "source-only post-release bundle builder unexpectedly became part of the frozen package");
Console.WriteLine("NEUTRAL-MATH-CONSUMER|EXPECTED-UNSUPPORTED|4");

public static class Rs133Probe
{
    public static int Calls;

    [DiscoveryCandidate("math.probe.echo", "math.probe")]
    public static int Echo(int value)
    {
        Calls++;
        return value;
    }
}
