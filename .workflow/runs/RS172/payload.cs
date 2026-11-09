using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Dist = "docs/distribution/";
const string Document = Dist + "nuget-federation-f38-numerics-reconciled.md";
const string Validator = Dist + "ValidateNugetNumericsF38Receipt.cs";
const string Evidence = Dist + "nuget-federation-f38-numerics-reconciled.json";
const string Report = ".aura/workflow/reports/RS171/attempt-001.json";
const string Status = "rs172-f38-original-numerics-dwf-physical-evidence-reconciled-unpublished";
const string Qualified = "MATH-NUMERICS-F38:DWf=VERIFIED;RECEIPT=RECONCILED;BINARY=15;NATIVE=541";
const string Limits = "MATH-NUMERICS-F38:NEW-PHYSICAL=false;SHARED-PUBLISHED=false;PUBLIC=false";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var complete = File.Exists(Path.Combine(root, Evidence.Replace('/', Path.DirectorySeparatorChar)));
p.Workflow.RequireCurrentRun("RS172", null, null, null, null, null);
p.Workflow.RequirePriorSuccess("RS171", 1,
    "test: qualify original Numerics immutable binary consumer with IR and Quantities",
    "0.1.66", 2, 2, 0, false);
p.ProjectPlan.RequireNodeState("M3", "done");
p.ProjectPlan.RequireNodeState("M7", "done");
p.Require.JsonValueEquals("docs/planning/backlog.json", "/plan_version",
    JsonValue.Create("0.1.122"));
p.Require.JsonValueEquals("docs/planning/backlog.json", "/status",
    JsonValue.Create("done"));
p.Require.JsonValueEquals("docs/workflow/manifest.json", "/guidanceVersion",
    JsonValue.Create("0.26"));
p.Require.JsonValueEquals(Report, "/outcome", JsonValue.Create("succeeded"));
p.Require.JsonValueEquals(Report, "/attempt", JsonValue.Create(1));
p.Require.JsonValueEquals(Report, "/validations/0/Id",
    JsonValue.Create("original-f35-binary-physical"));
p.Require.JsonValueEquals(Report, "/validations/0/ExitCode", JsonValue.Create(0));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f35-numerics-candidate-qualified.json",
    "/package_published", JsonValue.Create(false));
p.Require.JsonValueEquals("docs/distribution/nuget-federation-f37-numerics-release-qualified.json",
    "/package_published", JsonValue.Create(false));
if (!complete) p.Require.FileMissing(Evidence);
else
{
    p.Require.JsonValueEquals(Evidence, "/status", JsonValue.Create(Status));
    p.Require.JsonValueEquals(Evidence, "/package_published", JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence, "/new_physical_execution", JsonValue.Create(false));
    p.Require.JsonValueEquals(Evidence, "/native_nodes_retained", JsonValue.Create(541));
}

var sources = new[] { Document, Validator, Evidence };
if (!complete)
{
    foreach (var path in sources) p.Require.FileMissing(path);
}
foreach (var path in sources)
    p.Files.ReplaceFromStaged("staged/" + path, path);
p.Require.TextContains(Document, "## Reconciliation operation and bounds");
p.Require.TextContains(Validator, "original-f35-binary-physical");
p.Require.TextContains(Validator, "new_physical_execution");
// The evidence is a canonical staged snapshot of the already-committed RS171
// observation, not a locally recomputed workflow identity. Its exact source
// relationship is independently checked by the product validator.
var before = Run(root, "read-only-source-validation", Validator);
Need(before.Contains(Qualified, StringComparison.Ordinal) &&
     before.Contains(Limits, StringComparison.Ordinal),
    "RS172 committed physical evidence snapshot refused");

p.ProjectPlan.RequireNodeState("M3", "done");
var final = Run(root, "reconciled-receipt-validation", Validator);
Need(final.Contains(Qualified, StringComparison.Ordinal) &&
     final.Contains(Limits, StringComparison.Ordinal),
     "RS172 durable F38 reconciliation receipt rejected");
return p.Complete();

static void Need(bool yes, string reason)
{
    if (!yes) throw new InvalidDataException("RS172: " + reason);
}
static string Run(string root, string name, string file)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = root,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        }
    };
    process.StartInfo.ArgumentList.Add("run");
    process.StartInfo.ArgumentList.Add("--file");
    process.StartInfo.ArgumentList.Add(file);
    process.StartInfo.ArgumentList.Add("--");
    process.StartInfo.ArgumentList.Add("--check");
    var stdout = process.Start() ? process.StandardOutput.ReadToEndAsync() :
        throw new InvalidDataException("RS172 validator process start failed");
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(240000))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException("RS172 " + name + " timed out");
    }
    var output = stdout.GetAwaiter().GetResult();
    var error = stderr.GetAwaiter().GetResult();
    Need(process.ExitCode == 0,
        name + " failed: " + (output + "\n" + error)[..System.Math.Min((output + "\n" + error).Length, 2200)]);
    return output;
}
