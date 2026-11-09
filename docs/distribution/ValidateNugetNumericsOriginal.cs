#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

const string Ready = "MATH-NUMERICS-F38:SOURCE=READY;PHYSICAL=POST-ATTEMPT;NATIVE=541";
const string Limits = "MATH-NUMERICS-F38:PUBLISHED=false;PUBLIC=false;ROLLBACK=false";
string[] files =
[
    "docs/distribution/nuget-federation-f38-numerics-original.md",
    "docs/distribution/ValidateNugetNumericsOriginal.cs",
    "tools/dw.tools.math.nuget-numerics-original-consumer/dw.tools.math.nuget-numerics-original-consumer.csproj",
    "tools/dw.tools.math.nuget-numerics-original-consumer/Program.cs"
];
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("RS171 original Numerics: " + message);
}
static string Text(JsonElement value, string key) =>
    value.GetProperty(key).GetString() ??
        throw new InvalidDataException("required exact JSON value missing: " + key);
Need(args.Length == 0 || args.Length == 1 && args[0] == "--check",
    "validator only accepts observational --check");
foreach (var file in files) Need(File.Exists(file), "missing immutable source " + file);
using var prior = JsonDocument.Parse(File.ReadAllText(
    ".aura/workflow/reports/RS170/attempt-001.json"));
using var f35 = JsonDocument.Parse(File.ReadAllText(
    "docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json"));
using var f37 = JsonDocument.Parse(File.ReadAllText(
    "docs/distribution/nuget-federation-f37-numerics-release-qualified.json"));
var report = prior.RootElement;
var original = f35.RootElement.GetProperty("candidate");
Need(Text(report, "runId") == "RS170" &&
     report.GetProperty("attempt").GetInt32() == 1 &&
     Text(report, "outcome") == "succeeded" &&
     !report.GetProperty("priorInvocationFailures")
         .GetProperty("firstPassSucceeded").GetBoolean() &&
     Text(report.GetProperty("executor"), "toolVersion") == "0.1.66" &&
     Text(f35.RootElement, "status") ==
         "rs168-numerics-dependent-calendar-candidate-qualified-unpublished" &&
     Text(f37.RootElement, "status") ==
         "rs170-numerics-immutable-release-controller-qualified-unpublished" &&
     !f37.RootElement.GetProperty("package_published").GetBoolean() &&
     f37.RootElement.GetProperty("controller")
         .GetProperty("originalCandidatePhysicallyVerified").GetBoolean() &&
     Text(original, "candidateArchiveSha256") ==
         "f8e50ed22ac89b2939aa5519f202e082a536cc7cd8c7843ba74205d8a134802d" &&
     Text(original, "candidateDllSha256") ==
         "77ff720ffbb959a7052d5b31d18b272e472cf81b07f37697dfe525c0b40bcde4",
     "RS170 or original F35 binary predecessor is not exact");
Need(File.ReadAllText(files[2]).Contains("<Compile Include=\"Program.cs\" />",
        StringComparison.Ordinal) &&
     !File.ReadAllText(files[2]).Contains("PackageReference", StringComparison.Ordinal) &&
     File.ReadAllText(files[3]).Contains("MATH-NUMERICS-F38-REFUSED",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[3]).Contains("archiveRebuilt = false",
        StringComparison.Ordinal) &&
     File.ReadAllText(files[3]).Contains("privateFeedPublished = false",
        StringComparison.Ordinal),
     "original consumer and no-publication code contract changed");
// Physical F35 archive bytes are intentionally absent from DWF's disposable clone.
// The independent focused post-attempt validation will run the compiled controller
// against the original operator checkout; its result belongs to the DWF attempt report.
Console.WriteLine(Ready);
Console.WriteLine(Limits);
