#:property NuGetAudit=false
#:property PublishAot=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Root = "docs/discovery/qualification/";
const string Receipt = Root + "RS133-neutral-candidate-qualified.json";
const string Boundary = Root + "RS133-neutral-candidate-boundary.md";
const string Tests = Root + "RS133-neutral-consumers/";
const string Status = "rs133-neutral-1001-profile-incompatible-old-consumer-preserved";
const string Package = "fff0cd1aa9cf81528030f52de012e1aac1fa7a1b98da11b764096bf926f433ec";
const string Dll = "70d38e80963378d8df5ea76fcb59a14e79d0c169c8f5926f4018628370b15064";

static void Need(bool value, string detail)
{
    if (!value) throw new InvalidOperationException("RS133 evidence check: " + detail);
}
static JsonDocument Open(string path) => JsonDocument.Parse(File.ReadAllText(path));
static string String(JsonElement root, string key) =>
    root.GetProperty(key).GetString() ??
        throw new InvalidOperationException("Missing string " + key);
static bool Bool(JsonElement root, string key) =>
    root.GetProperty(key).GetBoolean();
static int Count(JsonElement node)
{
    var count = 1;
    foreach (var name in new[] { "workPackages", "phases", "tasks", "subtasks" })
        if (node.TryGetProperty(name, out var children))
            count += children.EnumerateArray().Sum(Count);
    return count;
}
static string Sha(string path) => Convert.ToHexStringLower(
    SHA256.HashData(File.ReadAllBytes(path)));

Need(args.Length == 0 || (args.Length == 1 && args[0] == "--check"),
    "Only --check is supported");
using var backlog = Open("docs/planning/backlog.json");
using var plan = Open(".aura/workflow/plan/project.json");
using var oldReport = Open(".aura/workflow/reports/RS131/attempt-001.json");
using var lastReport = Open(".aura/workflow/reports/RS132/attempt-001.json");
using var oldReceipt = Open(Root + "RS131-portable-protocol-qualified.json");
using var lastReceipt = Open(Root + "RS132-clr-seed-qualified.json");
Need(String(backlog.RootElement, "plan_version") == "0.1.122" &&
     String(backlog.RootElement, "status") == "done", "backlog history");
Need(plan.RootElement.GetProperty("milestones").EnumerateArray().Sum(Count) == 541 &&
     plan.RootElement.GetProperty("milestones").EnumerateArray()
         .All(x => String(x, "state") == "done"), "native milestones");
Need(String(oldReport.RootElement, "runId") == "RS131" &&
     String(oldReport.RootElement, "outcome") == "succeeded" &&
     String(lastReport.RootElement, "runId") == "RS132" &&
     String(lastReport.RootElement, "outcome") == "succeeded" &&
     Bool(oldReport.RootElement.GetProperty("priorInvocationFailures"), "firstPassSucceeded") &&
     Bool(lastReport.RootElement.GetProperty("priorInvocationFailures"), "firstPassSucceeded"),
     "original owner reports");
Need(oldReceipt.RootElement.GetProperty("case_results").GetArrayLength() == 13 &&
     lastReceipt.RootElement.GetProperty("observed_bindings").GetArrayLength() == 4 &&
     !Bool(lastReceipt.RootElement, "generated_math_skill") &&
     !Bool(lastReceipt.RootElement, "broker_ready"), "prior bounded receipts");
foreach (var file in new[] {
    Boundary, Tests + "legacy.csproj", Tests + "neutral.csproj",
    Tests + "LegacyConsumer.cs", Tests + "NeutralConsumer.cs"
})
    Need(File.Exists(file), "missing staged source " + file);
if (File.Exists(Receipt))
{
    using var doc = Open(Receipt);
    var e = doc.RootElement;
    Need(String(e, "status") == Status &&
         String(e, "archive_sha256") == Package &&
         String(e, "packaged_dll_sha256") == Dll &&
         String(e, "loaded_dll_sha256") == Dll &&
         e.GetProperty("portable_cases").GetArrayLength() == 13 &&
         e.GetProperty("legacy_operations").GetArrayLength() == 4 &&
         e.GetProperty("neutral_refusals").GetArrayLength() == 4 &&
         e.GetProperty("reflected_bindings").GetArrayLength() == 4 &&
         e.GetProperty("command_receipts").GetArrayLength() == 10 &&
         e.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Bool(e, "direct_legacy_invocation_executed") &&
         Bool(e, "neutral_consumer_executed") &&
         Bool(e, "old_consumer_fallback_rerun_verified") &&
         !Bool(e, "neutral_math_profile_supported") &&
         !Bool(e, "neutral_migration_qualified") &&
         !Bool(e, "installed_package_rollback_qualified") &&
         !Bool(e, "broker_ready") &&
         !Bool(e, "skill_generated") &&
         !Bool(e, "new_package_published") &&
         !Bool(e, "external_requests_transmitted"),
         "bounded new current-candidate receipt");
    var files = e.GetProperty("source_files");
    foreach (var file in new[] {
        Boundary, Tests + "legacy.csproj", Tests + "neutral.csproj",
        Tests + "LegacyConsumer.cs", Tests + "NeutralConsumer.cs",
        Root + "ValidateRS133NeutralCandidate.cs"
    })
        Need(String(files, file) == Sha(file), "staged source hash drift: " + file);
    var commands = e.GetProperty("command_receipts").EnumerateArray().ToArray();
    Need(commands.All(c => c.GetProperty("exit_code").GetInt32() == 0),
         "a required compiled consumer or regression command did not succeed");
    var expectedIds = new[] {
        "math.quantity.exact-pipeline.evaluate", "math.replay.try-evaluate",
        "math.ir.canonical.encode", "math.ir.hash.semantic"
    };
    Need(e.GetProperty("legacy_operations").EnumerateArray()
        .Select(x => String(x, "id")).SequenceEqual(expectedIds),
        "legacy four-method identity or order changed");
    var negative = e.GetProperty("neutral_refusals").EnumerateArray().ToArray();
    Need(negative.Select(x => String(x, "id")).SequenceEqual(expectedIds) &&
         String(negative[0], "reason") == "sidecar-method-unresolved" &&
         negative.Skip(1).All(x => String(x, "reason") == "unsupported-signature"),
         "neutral unsupported-profile result changed");
    var oldHashes = lastReceipt.RootElement.GetProperty("observed_bindings")
        .EnumerateArray().Select(x => String(x, "signature_sha256")).ToArray();
    Need(e.GetProperty("reflected_bindings").EnumerateArray()
        .Select(x => String(x, "signature_sha256")).SequenceEqual(oldHashes),
        "original RS132 signatures changed");
    var expectedCases = oldReceipt.RootElement.GetProperty("case_results")
        .EnumerateArray().Select(x => String(x, "name")).ToArray();
    Need(e.GetProperty("portable_cases").EnumerateArray()
        .Select(x => String(x, "name")).SequenceEqual(expectedCases),
        "frozen portable case identities changed");
}
Console.WriteLine("MATH-RS133:PORTABLE=13;REFLECTED=4;LEGACY=4;NEUTRAL-UNSUPPORTED=4;NATIVE=541");
Console.WriteLine("MATH-RS133:NEUTRAL-MIGRATION=false;FALLBACK-DIRECT=true;SKILL=false;BROKER=false");
return 0;
