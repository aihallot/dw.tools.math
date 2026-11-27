#:property NuGetAudit=false
#:property PublishAot=false
using System.Text.Json;

if (args.Length != 1 || args[0] != "--check")
    throw new ArgumentException("Expected --check.");
const string Request = "docs/coordination/requests/MATH-XR-004-assessa-bounded-integers.md";
const string Handoff = "docs/coordination/handovers/MATH-XR-004-assessa-bounded-statistics-published.md";
const string Version = "20260.20261120.1001";
const string Archive = "feb2d133760aa9468260ab103ec44ff28cd7db79160613fc1d8ac7cd76312ce1";
const string Dll = "684e8d5f35e53d4c38e9d02479539de3dfb8ac677535d0acfc8d9236509631ea";
const string DependencySha = "e8ab26619c72e0f683088ca5e3af1b5c02a01e5ad0793b0bde620ec6eafcf81d";
void Need(bool ok, string message)
{
    if (!ok) throw new InvalidDataException("RS275: "+message);
}
var request=File.ReadAllText(Request);
var handoff=File.ReadAllText(Handoff);
foreach (var value in new[] {Version,Archive,Dll,DependencySha})
    Need(request.Contains(value,StringComparison.Ordinal) &&
        handoff.Contains(value,StringComparison.Ordinal),
        "MATH-XR-004 intake and published handoff identity mismatch.");
Need(request.Contains("Assessa EG-A-01 / T002 adoption not yet qualified",StringComparison.Ordinal) &&
    request.Contains("Historical starting point",StringComparison.Ordinal) &&
    request.Contains("RS273",StringComparison.Ordinal) &&
    request.Contains("RS274",StringComparison.Ordinal) &&
    handoff.Contains("source-free",StringComparison.OrdinalIgnoreCase),
    "Missing historical/consumer qualification boundary.");
foreach(var item in new[]
{
    (Run:"RS273",Message:"test: qualify published bounded Statistics private feed readback",Prior:0,First:true),
    (Run:"RS274",Message:"docs: deliver published bounded Statistics handoff to Assessa",Prior:1,First:false)
})
{
    using var document=JsonDocument.Parse(File.ReadAllText(
        ".aura/workflow/reports/"+item.Run+"/attempt-001.json"));
    var root=document.RootElement;
    var prior=root.GetProperty("priorInvocationFailures");
    Need(root.GetProperty("runId").GetString()==item.Run &&
        root.GetProperty("outcome").GetString()=="succeeded" &&
        root.GetProperty("attempt").GetInt32()==1 &&
        root.GetProperty("git").GetProperty("commitMessage").GetString()==item.Message &&
        prior.GetProperty("count").GetInt32()==item.Prior &&
        prior.GetProperty("firstPassSucceeded").GetBoolean()==item.First,
        "Durable result differs: "+item.Run);
}
using var backlog=JsonDocument.Parse(File.ReadAllText("docs/planning/backlog.json"));
Need(backlog.RootElement.GetProperty("plan_version").GetString()=="0.1.188" &&
    backlog.RootElement.GetProperty("releases").EnumerateArray()
        .Single(x=>x.GetProperty("id").GetString()=="M9")
        .GetProperty("status").GetString()=="done",
    "Unexpected M9 product plan progress.");
using var native=JsonDocument.Parse(File.ReadAllText(".aura/workflow/plan/project.json"));
Need(native.RootElement.GetProperty("milestones").EnumerateArray()
        .Single(x=>x.GetProperty("id").GetString()=="M9")
        .GetProperty("state").GetString()=="done",
    "Unexpected M9 native progress.");
Console.WriteLine("MATH-RS275-REQUEST-RECONCILED|version="+Version+
    "|RS273=first-pass|RS274=delivered|Assessa=not-yet-accepted|M9=reconciled-done|PASS");
