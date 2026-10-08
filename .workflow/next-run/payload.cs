using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W02-C01";
const string Task = "M1-W02-C01-T2";
const string Green = "M1-W02-C01-T2-G";
const string Verify = "M1-W02-C01-T2-V";
const string Catalog = "src/projects/dw.quantities.standard/StandardUnitCatalog.cs";
const string Resolver = "src/projects/dw.quantities.standard/StandardExpressionUnitResolver.cs";
const string Tests = "tests/projects/dw.quantities.tests/M1W02C01BoundaryTests.cs";
const string SnapshotCatalog = "docs/planning/evidence/M1-W02-C01-AURA-StandardUnitCatalog.txt";
const string SnapshotResolver = "docs/planning/evidence/M1-W02-C01-AURA-StandardExpressionUnitResolver.txt";
const string Evidence = "docs/planning/evidence/M1-W02-C01-boundary-green.json";
const string TestProject = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";
const string OriginalCatalog = "lib/dw.quantities.standard/StandardUnitCatalog.cs";
const string OriginalResolver = "lib/dw.quantities.standard/StandardExpressionUnitResolver.cs";
const string PinnedCatalog = "f588545e89f51c78dd82a0d7c08ea92ab90fa48d0f5862d18666a00d0f13249a";
const string PinnedResolver = "3e71ed4af6abc748665c3f382093d625974c002649a8353d82dc4b5ad02bb561";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var backlog = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/backlog.json")))!.AsObject();
var chunk = backlog["chunks"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == Phase);
var task = chunk["tasks"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == Task);
var subtask = task["subtasks"]!.AsArray().Select(n => n!.AsObject()).ToArray();
var greenState = (string?)subtask.Single(n => (string?)n["id"] == Green)["status"];
var verifyState = (string?)subtask.Single(n => (string?)n["id"] == Verify)["status"];
var redState = (string?)subtask.Single(n => (string?)n["id"] == Task + "-R")["status"];
var baseline = (string?)backlog["plan_version"] == "0.1.17" &&
    (string?)chunk["status"] == "in_progress" && (string?)task["status"] == "in_progress" &&
    redState == "done" && greenState == "ready" && verifyState == "planned";
var target = (string?)backlog["plan_version"] == "0.1.18" &&
    (string?)chunk["status"] == "in_progress" && (string?)task["status"] == "in_progress" &&
    redState == "done" && greenState == "done" && verifyState == "ready";
if (!baseline && !target)
    throw new InvalidOperationException("RS019 product status is neither exact baseline nor completed GREEN target.");

var manifest = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/source-baseline.json")))!.AsObject();
var aura = manifest["repositories"]!.AsArray().Select(n => n!.AsObject())
    .Single(n => (string?)n["repository"] == "aura");
if ((string?)aura["head"] != "82a6b435a387a7e116b47a6b2c433ae9e067bf21")
    throw new InvalidOperationException("AURA source revision is not authorized.");
var sourceRoot = (string?)aura["observed_root"]
    ?? throw new InvalidOperationException("AURA source observed root is absent.");
var entries = aura["files"]!.AsArray().Select(n => n!.AsObject()).ToArray();

JsonObject Snapshot(string original, string expectedHash, string targetPath)
{
    var recorded = entries.Single(n => (string?)n["path"] == original);
    if ((string?)recorded["sha256"] != expectedHash)
        throw new InvalidOperationException("AURA recorded SHA drifted: " + original);
    var bytes = File.ReadAllBytes(Path.Combine(sourceRoot, original.Replace('/', Path.DirectorySeparatorChar)));
    var actual = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    if (actual != expectedHash)
        throw new InvalidOperationException("AURA observed source SHA drifted: " + original);
    var text = new UTF8Encoding(false, true).GetString(bytes);
    p.Files.WriteComplete(targetPath, text);
    return new JsonObject
    {
        ["source_path"] = original,
        ["sha256"] = expectedHash,
        ["observed_bytes"] = bytes.Length,
        ["review_copy_path"] = targetPath,
        ["review_copy_sha256"] = Convert.ToHexString(SHA256.HashData(
            File.ReadAllBytes(Path.Combine(root, targetPath.Replace('/', Path.DirectorySeparatorChar)))))
            .ToLowerInvariant(),
        ["disposition"] = "Full reference text captured within Math for parity review; not compiled into Math and not claimed byte-identical when text encoding differs."
    };
}

// Capture full approved mathematical sources rather than inferring all aliases
// from the brief declaration excerpts in RS018.
var catalogSource = Snapshot(OriginalCatalog, PinnedCatalog, SnapshotCatalog);
var resolverSource = Snapshot(OriginalResolver, PinnedResolver, SnapshotResolver);

p.Files.ReplaceFromStaged("staged/" + Catalog, Catalog);
p.Files.ReplaceFromStaged("staged/" + Resolver, Resolver);
p.Files.ReplaceFromStaged("staged/" + Tests, Tests);

RunRequired(root, "dotnet", "restore", "dw.tools.math.slnx", "--locked-mode");
RunRequired(root, "dotnet", "build", "dw.tools.math.slnx", "-c", "Release", "--no-restore");
foreach (var test in new[]
{
    "BoundedCandidateDiscoveryShowsAllAmbiguousProfiles",
    "CatalogIdsAreUniqueAndLookupsAreCanonical",
    "SymbolsAreCaseSensitiveWhereCaseChangesTheMeaning",
    "BoundedTokensRejectExcessivelyLongOrEmptyInput",
    "ExplicitAliasProfilesRejectUnresolvedAmbiguity",
    "EveryAdmittedUnitConvertsExactlyToAndFromItsOwnBaseScale",
    "CultureChangesCannotReorderOrChooseAmbiguousCandidates",
    "ExplicitProfileDoesNotOverrideUniqueCanonicalIdWithDifferentProfile"
})
    RunRequired(root, "dotnet", "test", TestProject, "-c", "Release",
        "--no-build", "--no-restore",
        "--filter", "FullyQualifiedName~M1W02C01BoundaryTests." + test,
        "--logger", "console;verbosity=normal");
RunRequired(root, "dotnet", "test", TestProject, "-c", "Release",
    "--no-build", "--no-restore", "--logger", "console;verbosity=minimal");
RunRequired(root, "pwsh", "-NoProfile", "-NonInteractive", "-File", "scripts/verify.ps1");
RunRequired(root, "dotnet", "run", "--file", "docs/planning/ValidateTransferArchitecture.cs");

var evidence = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W02-C01-boundary-green",
    ["status"] = "bounded-profile-discovery-qualified-inherited-source-parity-pending",
    ["source_review"] = p.Json.Array(catalogSource, resolverSource),
    ["product_files"] = p.Json.StringArray(Catalog, Resolver, Tests),
    ["focused_filter"] = "FullyQualifiedName~M1W02C01BoundaryTests",
    ["focused_passed"] = true,
    ["full_quantities_tests_passed"] = true,
    ["foundation_regression_passed"] = true,
    ["locked_solution_build_passed"] = true,
    ["transfer_architecture_passed"] = true,
    ["tested_contracts"] = p.Json.StringArray(
        "All explicit cup/pint profiles are discoverable without culture-dependent selection.",
        "Ambiguous aliases are not guessed; exact profiles resolve to one explicitly admitted unit.",
        "Symbols are case-sensitive while canonical IDs and named aliases match invariantly.",
        "Unit IDs are unique and lookups are limited to 128 characters.",
        "Token discovery is limited to 128 characters and rejects empty input.",
        "All admitted units roundtrip exact large BigInteger rational values.",
        "Ambient en-US, fr-BE, nl-BE and tr-TR cultures cannot alter candidate order.",
        "Currencies and logarithmic units remain unsupported, and mismatched profiles cannot coerce a different unit."),
    ["remaining_verification"] = p.Json.StringArray(
        "T2-V must compare exact source text against the adapted catalogue to document inherited IDs, aliases and numerical constants that are retained, changed or deferred.",
        "The original standard resolver uses dw.quantities.expression and an explicit culture argument; compatibility with the future expression layer is not qualified here.",
        "The admitted BCL-only subset is not an exhaustive or byte-for-byte port of AURA.",
        "No changes are made to AURA, Decision, MCDM or other sibling repositories.")
};
p.Files.WriteComplete(Evidence,
    evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");

p.Json.EditObject("docs/planning/backlog.json", product =>
{
    var c = product["chunks"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == Phase);
    var t = c["tasks"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == Task);
    var subs = t["subtasks"]!.AsArray().Select(n => n!.AsObject()).ToArray();
    product["plan_version"] = "0.1.18";
    c["status"] = "in_progress";
    c["refinement"] = "RS018 delivered an independent BCL-only catalog adaptation with 14 admitted units. RS019 strengthens deterministic candidate discovery, token bounds, catalog uniqueness and exact roundtrips, and captures both full SHA-pinned AURA sources for substantive inherited-source parity review. T2-V remains ready; full inherited alias/constant coverage and expression integration are not yet claimed.";
    c["files"] = p.Json.StringArray(
        "src/projects/dw.quantities.standard/StandardUnitCatalog.cs",
        "src/projects/dw.quantities.standard/StandardExpressionUnitResolver.cs",
        "src/projects/dw.quantities.standard/PureUnitConverter.cs",
        "src/projects/dw.quantities.standard/dw.quantities.standard.csproj",
        "src/projects/dw.quantities.standard/packages.lock.json",
        "tests/projects/dw.quantities.tests/M1W02C01RedTests.cs",
        "tests/projects/dw.quantities.tests/M1W02C01Tests.cs",
        Tests, "docs/planning/evidence/M1-W02-C01-red.json",
        "docs/planning/evidence/M1-W02-C01-initial-catalog.json",
        SnapshotCatalog, SnapshotResolver, Evidence);
    c["commands"] = p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --filter FullyQualifiedName~M1W02C01BoundaryTests",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1");
    var ce = c["evidence"]!.AsArray();
    if (!ce.Any(n => (string?)n == Evidence))
        ce.Add((JsonNode?)JsonValue.Create(Evidence));
    t["status"] = "in_progress";
    t["evidence"] = p.Json.StringArray("docs/planning/evidence/M1-W02-C01-red.json", Evidence);
    var g = subs.Single(n => (string?)n["id"] == Green);
    var v = subs.Single(n => (string?)n["id"] == Verify);
    g["status"] = "done";
    g["evidence"] = p.Json.StringArray(Evidence);
    v["status"] = "ready";
});
if (baseline)
{
    p.ProjectPlan.ActivateReadyContinuation(Green);
    p.ProjectPlan.ConvergeNodeToDone(Green);
    p.ProjectPlan.TransitionNode(Verify, "not-ready", "ready");
}
else
{
    p.ProjectPlan.RequireNodeState(Green, "done");
    p.ProjectPlan.RequireNodeState(Verify, "ready");
    p.ProjectPlan.RequireNodeState(Task, "in-progress");
    p.ProjectPlan.RequireNodeState(Phase, "in-progress");
}
RunRequired(root, "dotnet", "run", "--file", "docs/planning/ValidatePlan.cs", "--", "--write");
return p.Complete();

static string ImportantFailure(string output)
{
    var n = output.IndexOf("Error Message:", StringComparison.Ordinal);
    if (n < 0) n = output.IndexOf("error ", StringComparison.OrdinalIgnoreCase);
    var useful = n < 0 ? output : output[n..];
    return useful.Length > 2900 ? useful[..2900] : useful;
}

static (int Code, string Output) Run(string root, string exe, params string[] args)
{
    using var proc = new Process { StartInfo = new ProcessStartInfo
    {
        FileName = exe, WorkingDirectory = root, UseShellExecute = false,
        RedirectStandardOutput = true, RedirectStandardError = true
    }};
    proc.StartInfo.Environment["DOTNET_NOLOGO"] = "1";
    proc.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
    foreach (var arg in args) proc.StartInfo.ArgumentList.Add(arg);
    if (!proc.Start()) throw new InvalidOperationException("Cannot start " + exe);
    var stdout = proc.StandardOutput.ReadToEndAsync();
    var stderr = proc.StandardError.ReadToEndAsync();
    if (!proc.WaitForExit(420000))
    {
        proc.Kill(entireProcessTree: true);
        throw new TimeoutException(exe + " timed out");
    }
    var output = stdout.GetAwaiter().GetResult() + "\n" + stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return (proc.ExitCode, output);
}

static void RunRequired(string root, string exe, params string[] args)
{
    var result = Run(root, exe, args);
    if (result.Code != 0)
        throw new InvalidOperationException(exe + " " + string.Join(" ", args) +
            " exited " + result.Code + ": " + ImportantFailure(result.Output));
}
