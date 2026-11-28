#:property PublishAot=false
#:property NuGetAudit=false
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

const string Baseline = "a9547114a0ff0fc6ef6d0667bcc19f4079188ed8";
const string Audit = "docs/planning/math-2.0/audit-original.fr.md";
const string Proposal = "docs/planning/math-2.0/roadmap-admission.json";
static void Need(bool condition, string message)
{
    if (!condition) throw new InvalidDataException("MATH-2.0-ADMISSION: " + message);
}
static JsonObject Read(string path) => JsonNode.Parse(File.ReadAllText(path))!.AsObject();
static JsonObject AtBaseline(string commit, string path)
{
    using var process = new Process { StartInfo = new ProcessStartInfo
    {
        FileName = "git", UseShellExecute = false, RedirectStandardOutput = true,
        RedirectStandardError = true, StandardOutputEncoding = System.Text.Encoding.UTF8,
        WorkingDirectory = Directory.GetCurrentDirectory()
    }};
    process.StartInfo.ArgumentList.Add("show");
    process.StartInfo.ArgumentList.Add(commit + ":" + path);
    Need(process.Start(), "Could not read pinned historical Git object.");
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(30000)) { process.Kill(entireProcessTree: true); throw new TimeoutException("Historical Git object read timed out."); }
    Need(process.ExitCode == 0, "Historical Git object unavailable: " + stderr.GetAwaiter().GetResult());
    return JsonNode.Parse(stdout.GetAwaiter().GetResult())!.AsObject();
}
static string S(JsonNode node, string key) => (string?)node[key] ?? throw new InvalidDataException("Missing " + key);
static IEnumerable<JsonNode> Objects(JsonNode node, string key) => node[key]!.AsArray().Select(x => x!);
static void Exact(JsonNode? a, JsonNode? b, string message) => Need(JsonNode.DeepEquals(a,b),message);
static void CheckAppend(JsonArray original, JsonArray addition, JsonArray actual, string label)
{
    Need(actual.Count == original.Count + addition.Count, "Count " + label);
    for (int i=0;i<original.Count;i++) Exact(original[i],actual[i],"Historical drift "+label+"/"+i);
    for (int i=0;i<addition.Count;i++) Exact(addition[i],actual[original.Count+i],"Admission drift "+label+"/"+i);
}
static IEnumerable<JsonNode> Canonical(JsonNode b)
{
    foreach(var key in new[]{"releases","work_packages","chunks"})
        foreach(var x in Objects(b,key)) yield return x;
    foreach(var c in Objects(b,"chunks"))
        foreach(var t in Objects(c,"tasks"))
        {
            yield return t;
            foreach(var s in Objects(t,"subtasks")) yield return s;
        }
}
static IEnumerable<JsonNode> Walk(JsonNode n)
{
    yield return n;
    foreach(var key in new[]{"workPackages","phases","tasks","subtasks"})
        if(n[key] is JsonArray children)
            foreach(var x in children)
                foreach(var d in Walk(x!)) yield return d;
}
if (args.Length > 1 || (args.Length == 1 && args[0] != "--m12-ready"))
    throw new ArgumentException("Expected no arguments for historical admission or --m12-ready for RS431 readiness.");
var readiness = args.Length == 1;
var readyIds = new HashSet<string>(StringComparer.Ordinal) { "M12", "M12-W01", "M12-W01-C01", "M12-W01-C01-T1", "M12-W01-C01-T1-A" };
var old=AtBaseline(Baseline, "docs/planning/backlog.json");
var oldNative=AtBaseline(Baseline, ".aura/workflow/plan/project.json");
var current=Read("docs/planning/backlog.json");
var native=Read(".aura/workflow/plan/project.json");
var proposal=Read(Proposal);
Need(S(old,"plan_version")=="0.1.196" && S(old,"status")=="done","Historical version.");
Need(S(current,"plan_version")=="0.1.197" && S(current,"status")=="in_progress","Target version.");
Need(S(proposal["baseline"]!,"commit")==Baseline && S(proposal,"target_plan_version")=="0.1.197","Proposal identity.");
foreach(var key in new[]{"releases","work_packages","chunks","requirements","source_coverage"})
    CheckAppend(old[key]!.AsArray(),proposal[key]!.AsArray(),current[key]!.AsArray(),key);
foreach(var key in old.Select(x=>x.Key).Except(new[]{"releases","work_packages","chunks","requirements","source_coverage","plan_version","status"}))
    Exact(old[key],current[key],"Changed historical authority "+key);
Need(current.Select(x=>x.Key).ToHashSet(StringComparer.Ordinal).SetEquals(old.Select(x=>x.Key)),"Canonical root fields.");
Exact(oldNative["project"],native["project"],"Native identity.");
Exact(oldNative["schemaVersion"],native["schemaVersion"],"Native schema.");
Need(native.Select(x=>x.Key).ToHashSet(StringComparer.Ordinal).SetEquals(oldNative.Select(x=>x.Key)),"Native root fields.");
var expectedNewNative = proposal["native_milestones"]!.AsArray().DeepClone().AsArray();
if (readiness)
{
    var candidates = expectedNewNative.SelectMany(x=>Walk(x!)).Where(x=>readyIds.Contains(S(x,"id"))).ToArray();
    Need(candidates.Length==readyIds.Count && candidates.All(x=>S(x,"state")=="not-ready"),"Readiness baseline differs.");
    foreach (var candidate in candidates) candidate["state"]="ready";
}
CheckAppend(oldNative["milestones"]!.AsArray(),expectedNewNative,native["milestones"]!.AsArray(),"milestones");
var oldC=Canonical(old).ToArray();
var oldN=Objects(oldNative,"milestones").SelectMany(Walk).ToArray();
var c=Canonical(current).ToArray();
var n=Objects(native,"milestones").SelectMany(Walk).ToArray();
Need(oldC.Length==636 && oldN.Length==630 && oldC.All(x=>S(x,"status")=="done") && oldN.All(x=>S(x,"state")=="done"),"Historical completion boundary.");
Need(c.Length==955 && n.Length==949,"Target cardinality.");
var oldCIds=oldC.Select(x=>S(x,"id")).ToHashSet(StringComparer.Ordinal);
var oldNIds=oldN.Select(x=>S(x,"id")).ToHashSet(StringComparer.Ordinal);
var newC=c.Where(x=>!oldCIds.Contains(S(x,"id"))).ToArray();
var newN=n.Where(x=>!oldNIds.Contains(S(x,"id"))).ToArray();
Need(newC.Length==319 && newN.Length==319 && newC.All(x=>S(x,"status")=="planned" && x["evidence"]!.AsArray().Count==0) && newN.All(x=>S(x,"state")== (readiness && readyIds.Contains(S(x,"id")) ? "ready" : "not-ready")),"New scope must match the exact readiness frontier.");
Need(newC.Select(x=>S(x,"id")).ToHashSet(StringComparer.Ordinal).SetEquals(newN.Select(x=>S(x,"id"))),"New ID parity.");
Need(c.Select(x=>S(x,"id")).Distinct(StringComparer.OrdinalIgnoreCase).Count()==c.Length && n.Select(x=>S(x,"id")).Distinct(StringComparer.OrdinalIgnoreCase).Count()==n.Length,"Duplicate node.");
Need(oldCIds.Except(oldNIds).OrderBy(x=>x,StringComparer.Ordinal).SequenceEqual(c.Select(x=>S(x,"id")).Except(n.Select(x=>S(x,"id"))).OrderBy(x=>x,StringComparer.Ordinal)),"Historical omissions changed.");
Need(proposal["releases"]!.AsArray().Count==9 && proposal["work_packages"]!.AsArray().Count==15 && proposal["chunks"]!.AsArray().Count==59 && proposal["requirements"]!.AsArray().Count==59,"Admitted scope roster.");
Need(Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(Audit)).Replace("\r\n","\n",StringComparison.Ordinal)))).ToLowerInvariant()=="c0968488df8ecf89dc913f03b5c92c58c8bb6a81dfb11cd2ccdb271f07602272","Audit byte identity.");
Need(File.Exists("docs/planning/math-2.0/discovery-generation.md") && File.Exists("docs/planning/math-2.0/roadmap.md"),"Missing source documents.");
var rs429=Read(".aura/workflow/reports/RS429/attempt-001.json");
Need(S(rs429,"outcome")=="succeeded" && (int?)rs429["attempt"]==1 && (bool?)rs429["priorInvocationFailures"]?["firstPassSucceeded"]==true && (int?)rs429["priorInvocationFailures"]?["count"]==0 && S(rs429["executor"]!,"toolVersion")=="0.1.81" && rs429["boundaryViolations"]!.AsArray().Count==0,"RS429 evidence.");
if (readiness)
{
    const string admitted = "3805e8b69416081a9030371917f9586f167b09b6";
    Exact(AtBaseline(admitted,"docs/planning/backlog.json"),current,"Canonical readiness reconciliation must await durable native evidence.");
    var rs430=Read(".aura/workflow/reports/RS430/attempt-001.json");
    var proof=Read(".workflow/runs/RS430/preparation-proof.json");
    Need(S(rs430,"outcome")=="succeeded" && (int?)rs430["attempt"]==1 && (bool?)rs430["priorInvocationFailures"]?["firstPassSucceeded"]==true && (int?)rs430["priorInvocationFailures"]?["count"]==0 && S(rs430["executor"]!,"toolVersion")=="0.1.81" && rs430["boundaryViolations"]!.AsArray().Count==0 && rs430["progress"]!["transitions"]!.AsArray().Count==0 && (bool?)proof["payloadCompiled"]==true && (int?)proof["rehearsedValidationCount"]==10,"RS430 durable admission differs.");
    Need(File.Exists("docs/planning/decisions/m12-source-api-evidence-inventory-readiness-rs431.md"),"Missing inventory scope decision.");
    Console.WriteLine("MATH-RS431-M12-READINESS|ready=5|newNativeNotReady=314|historicalDone=630|canonicalPlan=0.1.197|publication=false|PASS");
}
else
    Console.WriteLine("MATH-RS430-ADMISSION|oldCanonicalDone=636|oldNativeDone=630|newPlanned=319|newNativeNotReady=319|phases=59|publication=false|PASS");
