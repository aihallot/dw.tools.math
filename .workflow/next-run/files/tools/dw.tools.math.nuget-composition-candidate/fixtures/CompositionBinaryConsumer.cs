using System.Security.Cryptography;
using Dw.Tools.Math.Composition;

if (args.Length != 1 || args[0].Length != 64)
    throw new InvalidOperationException("Exact packaged Composition DLL SHA-256 required.");
var passes = 0;
void Probe(string name, Action test)
{
    try
    {
        test();
        passes++;
        Console.WriteLine("MATH-COMPOSITION-CHECK|" + name + "|PASS");
    }
    catch (Exception problem)
    {
        throw new InvalidOperationException("Composition package-only consumer " + name, problem);
    }
}
static void Need(bool valid, string message)
{
    if (!valid) throw new InvalidOperationException(message);
}
static bool Refuses<TException>(Action action) where TException : Exception
{
    try { action(); return false; }
    catch (TException) { return true; }
}
var entries = MathCapabilityCatalog.Discover();
var cap = MathOperationCapability.MatrixInspection;
var provenance = MathResultProvenance.Create("matrix.inspect", "1");
Probe("loaded-composition-dll", () =>
{
    var asm = typeof(MathCapabilityCatalog).Assembly;
    Need(asm.GetName().Name == "dw.tools.math.composition",
        "Wrong Composition assembly loaded.");
    Need(Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(asm.Location))) ==
         args[0], "Loaded DLL differs from actual archive bytes.");
});
Probe("catalog-single", () =>
    Need(entries.Length == 1 && entries[0].OperationId == "matrix.inspect",
         "Unexpected catalog or inferred provider capability."));
Probe("catalog-find-known", () =>
    Need(MathCapabilityCatalog.Find("matrix.inspect") == cap,
         "Exact described capability missing."));
Probe("catalog-find-unknown", () =>
    Need(MathCapabilityCatalog.Find("matrix.execute") is null &&
         MathCapabilityCatalog.Find(null) is null,
         "Unknown or null capability must not fall back."));
Probe("capability-boundaries", () =>
    Need(cap.MaximumMatrixElements == 4096 && cap.SupportsExact &&
         cap.SupportsApproximate && cap.AcceptedInputKinds.Length == 1 &&
         cap.AcceptedInputKinds[0] == MathValueKind.Matrix,
         "Bounded exact/approximate matrix descriptor drifted."));
Probe("capability-null-refusal", () =>
    Need(!cap.Accepts(null), "Null IR node must not be admitted."));
Probe("provenance-identity", () =>
    Need(provenance.OperationId == "matrix.inspect" &&
         provenance.ContractVersion == "1",
         "Versioned, provider-neutral provenance identity drifted."));
Probe("provenance-empty-refusal", () =>
    Need(Refuses<ArgumentException>(() =>
         MathResultProvenance.Create("", "1")),
         "Empty provenance identity must refuse."));
Probe("provenance-limit-refusal", () =>
    Need(Refuses<ArgumentOutOfRangeException>(() =>
         MathResultProvenance.Create(new string('x', 129), "1")),
         "Oversized provenance must refuse."));
Probe("unsupported-no-final-value", () =>
{
    var result = MathOperationResult.Unsupported("not-supported", provenance);
    Need(result.Status == ComputationStatus.Unsupported &&
         !result.HasFinalValue && result.Value is null &&
         result.Reason == "not-supported" &&
         result.Provenance == provenance, "Unsupported result leaked final value.");
});
Probe("budget-no-final-value", () =>
{
    var result = MathOperationResult.BudgetExceeded("limit", provenance);
    Need(result.Status == ComputationStatus.BudgetExceeded &&
         !result.HasFinalValue && result.Reason == "limit",
         "Budget exceeded must not report a final value.");
});
Probe("result-empty-reason-refusal", () =>
    Need(Refuses<ArgumentException>(() =>
         MathOperationResult.Unsupported("", provenance)),
         "Empty failure reason must refuse."));
Probe("replay-context-identity", () =>
{
    var context = ExactReplayContext.Create("a1", "p1", "c1", "t1", "provider1");
    Need(context.Assumptions == "a1" && context.Policy == "p1" &&
         context.Catalog == "c1" && context.Tolerance == "t1" &&
         context.Provider == "provider1",
         "Replay context labels drifted.");
});
Probe("replay-context-refusal", () =>
    Need(Refuses<ArgumentException>(() =>
         ExactReplayContext.Create("a", " ", "c", "t", "p")) &&
         Refuses<ArgumentOutOfRangeException>(() =>
         ExactReplayContext.Create(new string('x', 129), "p", "c", "t", "q")),
         "Invalid replay label accepted."));
Probe("replay-cache-bounds", () =>
{
    var cache = new ExactReplayCache(1);
    Need(cache.Capacity == 1 && cache.Count == 0 &&
         ExactReplayCache.MaximumEntries == 32,
         "Cache capacity or empty state drifted.");
    cache.Clear();
    Need(cache.Count == 0 &&
         Refuses<ArgumentOutOfRangeException>(() => new ExactReplayCache(0)) &&
         Refuses<ArgumentOutOfRangeException>(() => new ExactReplayCache(33)),
         "Unbounded or invalid cache admitted.");
});
Need(passes == 15, "Exactly 15 compiled Composition consumer probes required.");
Console.WriteLine("MATH-COMPOSITION-BINARY|PASS|15");
