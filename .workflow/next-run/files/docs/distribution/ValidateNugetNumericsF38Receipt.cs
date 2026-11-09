#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

const string PriorReport = ".aura/workflow/reports/RS171/attempt-001.json";
const string F35 = "docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json";
const string F37 = "docs/distribution/nuget-federation-f37-numerics-release-qualified.json";
const string Receipt = "docs/distribution/nuget-federation-f38-numerics-reconciled.json";
const string Document = "docs/distribution/nuget-federation-f38-numerics-reconciled.md";
const string Validator = "docs/distribution/ValidateNugetNumericsF38Receipt.cs";
const string Status = "rs172-f38-original-numerics-dwf-physical-evidence-reconciled-unpublished";
const string OriginalSha = "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d";
const string DllSha = "77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4";
const string IrSha = "d2f9d5bc185b5260cdf2e01e295ebe3bf39e2c73c8ca9296fddd07b72586e480";
const string QuantitiesSha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
const string Ready = "MATH-NUMERICS-F38:DWf=VERIFIED;RECEIPT=PENDING;NATIVE=541";
const string Qualified = "MATH-NUMERICS-F38:DWf=VERIFIED;RECEIPT=RECONCILED;BINARY=15;NATIVE=541";
const string Limits = "MATH-NUMERICS-F38:NEW-PHYSICAL=false;SHARED-PUBLISHED=false;PUBLIC=false";

static void Need(bool condition, string reason)
{
    if (!condition) throw new InvalidDataException("RS172 F38 reconciliation: " + reason);
}
static string Text(JsonElement value, string key) =>
    value.GetProperty(key).GetString() ??
    throw new InvalidDataException("missing exact field " + key);
static string Sha(string file) =>
    Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(file)));
static string HashText(string data) =>
    Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(data)));

Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
     "only non-mutating --check is permitted");
foreach (var file in new[] { PriorReport, F35, F37, Document, Validator })
    Need(File.Exists(file), "required Git evidence/source is missing: " + file);
using var report = JsonDocument.Parse(File.ReadAllText(PriorReport));
using var candidate = JsonDocument.Parse(File.ReadAllText(F35));
using var controller = JsonDocument.Parse(File.ReadAllText(F37));
var previous = report.RootElement;
var f35 = candidate.RootElement.GetProperty("candidate");
var f37 = controller.RootElement.GetProperty("controller");
var failures = previous.GetProperty("priorInvocationFailures");
Need(Text(previous, "runId") == "RS171" &&
     previous.GetProperty("attempt").GetInt32() == 1 &&
     Text(previous, "outcome") == "succeeded" &&
     !previous.GetProperty("correctionMode").GetBoolean() &&
     Text(previous.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     Text(previous.GetProperty("git"), "commitMessage") ==
         "test: qualify original Numerics immutable binary consumer with IR and Quantities" &&
     failures.GetProperty("count").GetInt32() == 2 &&
     failures.GetProperty("preparationFailureCount").GetInt32() == 2 &&
     failures.GetProperty("allocatedAttemptFailureCount").GetInt32() == 0 &&
     !failures.GetProperty("firstPassSucceeded").GetBoolean(),
     "RS171 prior physical validation must have exact successful lineage");
Need(Text(candidate.RootElement, "status") ==
         "rs168-numerics-dependent-calendar-candidate-qualified-unpublished" &&
     !candidate.RootElement.GetProperty("package_published").GetBoolean() &&
     Text(f35, "candidateArchiveSha256") == OriginalSha &&
     Text(f35, "candidateDllSha256") == DllSha &&
     f35.GetProperty("candidateArchiveBytes").GetInt32() == 18313 &&
     Text(controller.RootElement, "status") ==
         "rs170-numerics-immutable-release-controller-qualified-unpublished" &&
     !controller.RootElement.GetProperty("package_published").GetBoolean() &&
     !controller.RootElement.GetProperty("release_authorized").GetBoolean() &&
     f37.GetProperty("originalCandidatePhysicallyVerified").GetBoolean(),
     "original F35/F37 package identities and publication limits changed");

var validations = previous.GetProperty("validations").EnumerateArray().ToArray();
Need(validations.Length == 1, "exactly one focused physical RS171 validation expected");
var v = validations[0];
var argsObserved = v.GetProperty("Arguments").EnumerateArray()
    .Select(x => x.GetString()).ToArray();
Need(Text(v, "Id") == "original-f35-binary-physical" &&
     Text(v, "classification") == "focused" &&
     Text(v, "FileName") == "dotnet" &&
     v.GetProperty("packs").EnumerateArray()
         .Select(x => x.GetString()).SequenceEqual(new[] { "numeric" }) &&
     argsObserved.Contains("--verify-original", StringComparer.Ordinal) &&
     v.GetProperty("ExitCode").GetInt32() == 0 &&
     !v.GetProperty("TimedOut").GetBoolean() &&
     !v.GetProperty("Cancelled").GetBoolean() &&
     Text(v, "StandardError") == "",
     "RS171 actual focused package-only validation did not pass");
var stdout = Text(v, "StandardOutput");
using var physical = JsonDocument.Parse(stdout);
var observed = physical.RootElement;
Need(Text(observed, "status") ==
         "qualified-original-f35-numerics-consumer-unpublished" &&
     Text(observed, "packageId") == "dw.tools.math.numerics" &&
     Text(observed, "packageVersion") == "20260.20261109.1001" &&
     observed.GetProperty("originalArchiveBytes").GetInt32() == 18313 &&
     Text(observed, "originalArchiveSha256") == OriginalSha &&
     Text(observed, "originalDllSha256") == DllSha &&
     Text(observed, "disposableArchiveSha256") == OriginalSha &&
     observed.GetProperty("originalArchivePhysicallyVerified").GetBoolean() &&
     observed.GetProperty("freshNuGetPackagesCache").GetBoolean() &&
     !observed.GetProperty("archiveRebuilt").GetBoolean() &&
     !observed.GetProperty("rehearsalArchiveSubstituted").GetBoolean() &&
     !observed.GetProperty("privateFeedPublished").GetBoolean() &&
     !observed.GetProperty("releaseAuthorized").GetBoolean() &&
     observed.GetProperty("compiledBinaryChecks").GetInt32() == 15 &&
     Text(observed, "directDependencyId") == "dw.tools.math.ir" &&
     Text(observed, "directDependencyVersion") == "20260.20261104.1001" &&
     Text(observed, "directDependencyArchiveSha256") == IrSha &&
     Text(observed, "transitiveDependencyId") == "dw.quantities" &&
     Text(observed, "transitiveDependencyVersion") == "20260.20261102.1001" &&
     Text(observed, "transitiveDependencyArchiveSha256") == QuantitiesSha,
     "observed physical output differs from exact original package/dependency proof");
var inner = observed.GetProperty("commandReceipts").EnumerateArray().ToArray();
Need(inner.Length == 4 &&
     inner.Select(x => Text(x, "id")).SequenceEqual(
       new[] { "consumer-restore", "consumer-build", "consumer-binary", "sdk" }) &&
     inner.All(x => x.GetProperty("exit_code").GetInt32() == 0) &&
     Text(inner[2], "output_excerpt").Contains(
         "MATH-NUMERICS-BINARY|PASS|15", StringComparison.Ordinal),
     "actual four inner consumer commands or fifteen probes missing");

if (File.Exists(Receipt))
{
    using var current = JsonDocument.Parse(File.ReadAllText(Receipt));
    var result = current.RootElement;
    Need(Text(result, "status") == Status &&
         Text(result, "source_run") == "RS171" &&
         result.GetProperty("source_attempt").GetInt32() == 1 &&
         Text(result, "source_validation_id") == "original-f35-binary-physical" &&
         Text(result, "stdout_sha256") == HashText(stdout) &&
         Text(result, "source_report") == PriorReport &&
         Text(result, "source_result_commit") ==
             "01d9fffe1f7a1aa0f770cec215aa116b9f140104" &&
         result.GetProperty("validation_exit_code").GetInt32() == 0 &&
         JsonNode.DeepEquals(
             JsonNode.Parse(result.GetProperty("observation").GetRawText()),
             JsonNode.Parse(stdout)) &&
         !result.GetProperty("new_physical_execution").GetBoolean() &&
         !result.GetProperty("package_published").GetBoolean() &&
         !result.GetProperty("release_authorized").GetBoolean() &&
         result.GetProperty("native_nodes_retained").GetInt32() == 541 &&
         Text(result.GetProperty("observation"), "originalArchiveSha256") == OriginalSha &&
         result.GetProperty("source_files").EnumerateObject().Count() == 2 &&
         result.GetProperty("source_files").EnumerateObject()
             .All(x => File.Exists(x.Name) && x.Value.GetString() == Sha(x.Name)),
         "reconciled receipt drifted from durable RS171 focused validation");
}
Console.WriteLine(File.Exists(Receipt) ? Qualified : Ready);
Console.WriteLine(Limits);
