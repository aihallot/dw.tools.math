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
if (args is ["--m12-question-complete"])
{
    const string pin = "155d0d9aa2c3b2237c5a2b30e44a504a710cc366";
    const string decisionPath = "docs/planning/decisions/m12-source-api-evidence-inventory-protocol-rs436.md";
    const string protocolPath = "docs/engineering/m12-source-api-inventory-protocol-rs436.json";
    var expectedCanonical = AtBaseline(pin, "docs/planning/backlog.json");
    Need(S(expectedCanonical, "plan_version") == "0.1.198", "RS435 canonical pin.");
    string[] activeIds = ["M12", "M12-W01", "M12-W01-C01", "M12-W01-C01-T1", "M12-W01-C01-T1-A"];
    var ids = activeIds.ToHashSet(StringComparer.Ordinal);
    var active = Canonical(expectedCanonical).Where(x => ids.Contains(S(x,"id"))).ToArray();
    Need(active.Length == ids.Count && active.All(x => S(x,"status") == "ready" && x["evidence"]!.AsArray().Count == 0), "RS435 canonical readiness frontier.");
    foreach (var node in active) node["status"] = "in_progress";
    var inventory = active.Single(x => S(x,"id") == "M12-W01-C01");
    inventory["files"]!.AsArray().Add(JsonValue.Create(decisionPath));
    inventory["files"]!.AsArray().Add(JsonValue.Create(protocolPath));
    inventory["refinement"] = "RS435 durable activation reconciled. RS436 specifies pinned source, project, metadata and evidence observation with an exact roster, independent negative controls and resource limits. Observation must qualify or reuse the missing metadata reader and gather real evidence before modernization.";
    inventory["readiness"] = "RS435 delivered five native Question activation transitions; RS436 reconciles those statuses as in_progress and targets native Question closure and Observation readiness. Canonical closure and Observation readiness await durable RS436 evidence.";
    expectedCanonical["plan_version"] = "0.1.199";
    Exact(expectedCanonical, Read("docs/planning/backlog.json"), "RS436 canonical delta differs from pinned RS435.");
    var expectedNative = AtBaseline(pin, ".aura/workflow/plan/project.json");
    var nativeNodes = Objects(expectedNative, "milestones").SelectMany(Walk).ToArray();
    Need(nativeNodes.Length == 949, "Native roster.");
    var question = nativeNodes.Single(x => S(x,"id") == "M12-W01-C01-T1-A");
    var observation = nativeNodes.Single(x => S(x,"id") == "M12-W01-C01-T1-B");
    Need(S(question,"state") == "in-progress" && S(observation,"state") == "not-ready", "RS435 native baseline.");
    question["state"] = "done";
    observation["state"] = "ready";
    Exact(expectedNative, Read(".aura/workflow/plan/project.json"), "Only Question closure and Observation readiness are admitted.");
    var prior = Read(".aura/workflow/reports/RS435/attempt-001.json");
    var priorProof = Read(".workflow/runs/RS435/preparation-proof.json");
    var transitions = prior["progress"]!["transitions"]!.AsArray();
    Need(S(prior,"runId") == "RS435" && S(prior,"outcome") == "succeeded" && (int?)prior["attempt"] == 1 &&
        S(prior["executor"]!,"toolVersion") == "0.1.81" && (bool?)prior["priorInvocationFailures"]!["firstPassSucceeded"] == true &&
        (int?)prior["priorInvocationFailures"]!["count"] == 0 && prior["boundaryViolations"]!.AsArray().Count == 0 &&
        transitions.Count == ids.Count && transitions.Select(x => S(x!,"id")).ToHashSet(StringComparer.Ordinal).SetEquals(ids) &&
        transitions.All(x => S(x!,"beforeState") == "ready" && S(x!,"afterState") == "in-progress" && (bool?)x!["completed"] == false) &&
        (bool?)priorProof["payloadCompiled"] == true && (int?)priorProof["rehearsedValidationCount"] == 10, "RS435 durable activation evidence differs.");
    var recipe = Read(protocolPath);
    Need((int?)recipe["schemaVersion"] == 1 && S(recipe,"status") == "specified-not-observed" &&
        S(recipe,"sourceCommit") == pin && S(recipe,"sdk") == "11.0.100" &&
        S(recipe,"targetFramework") == "net11.0" && S(recipe,"languageVersion") == "15.0", "Protocol pin.");
    var projectRoster = System.Xml.Linq.XDocument.Load("dw.tools.math.slnx").Descendants("Project").Select(x => (string)x.Attribute("Path")!).ToArray();
    var recordedRoster = recipe["maintainedProjects"]!.AsArray().Select(x => (string)x!).ToArray();
    Need(projectRoster.Length == 24 && recordedRoster.Length == 24 && projectRoster.Distinct(StringComparer.Ordinal).Count() == 24 &&
        projectRoster.ToHashSet(StringComparer.Ordinal).SetEquals(recordedRoster) && projectRoster.Count(x => x.StartsWith("src/projects/", StringComparison.Ordinal)) == 13 &&
        projectRoster.Count(x => x.StartsWith("tests/projects/", StringComparison.Ordinal)) == 11 &&
        projectRoster.All(x => File.Exists(x) && File.Exists(Path.Combine(Path.GetDirectoryName(x)!, "packages.lock.json"))), "Maintained project roster or locks.");
    var controlIds = recipe["negativeControls"]!.AsArray().Select(x => (string)x!).ToArray();
    Need(controlIds.Length == 6 && controlIds.ToHashSet(StringComparer.Ordinal).SetEquals(new[] { "missing-maintained-project", "duplicate-project-or-member-key", "source-or-assembly-digest-drift", "unsupported-or-malformed-metadata", "nonpublic-member-excluded", "overloads-distinct" }), "Independent control specification.");
    var observationSpec = recipe["observation"]!;
    var reportFields = recipe["reportContract"]!["requiredRootFields"]!.AsArray().Select(x => (string)x!).ToArray();
    Need(S(observationSpec,"implementationStatus") == "not-yet-implemented-or-qualified" &&
        S(observationSpec,"implementation") == "docs/engineering/InspectM12SourceApiInventory.cs" && S(observationSpec,"reportPath") == "docs/engineering/m12-source-api-inventory-observation.json" &&
        observationSpec["commands"]!.AsArray().Count == 5 &&
        (int?)recipe["limits"]!["maxObservationSeconds"] == 1200 &&
        reportFields.Length == 14 && reportFields.ToHashSet(StringComparer.Ordinal).SetEquals(new[] { "schemaVersion", "sourceCommit", "sdk", "targetFramework", "configuration", "trackedRoster", "projects", "assemblies", "apiMembers", "historicalEvidence", "toolDispositions", "negativeControls", "limits", "status" }) &&
        File.Exists(decisionPath), "Observation recipe or bounds missing.");
    Console.WriteLine("MATH-RS436-M12-QUESTION|canonicalActive=5|nativeQuestion=done|nativeObservation=ready|maintainedProjects=24|controlsSpecified=6|observed=false|PASS");
    return;
}
if (args.Length > 1 || (args.Length == 1 && args[0] is not ("--m12-ready" or "--m12-question-active")))
    throw new ArgumentException("Expected historical admission, --m12-ready or --m12-question-active.");
var readiness = args.Length == 1;
var questionActive = args.Length == 1 && args[0] == "--m12-question-active";
var readyIds = new HashSet<string>(StringComparer.Ordinal) { "M12", "M12-W01", "M12-W01-C01", "M12-W01-C01-T1", "M12-W01-C01-T1-A" };
var old=AtBaseline(Baseline, "docs/planning/backlog.json");
var oldNative=AtBaseline(Baseline, ".aura/workflow/plan/project.json");
var current=Read("docs/planning/backlog.json");
var native=Read(".aura/workflow/plan/project.json");
var proposal=Read(Proposal);
Need(S(old,"plan_version")=="0.1.196" && S(old,"status")=="done","Historical version.");
Need(S(current,"plan_version")==(questionActive ? "0.1.198" : "0.1.197") && S(current,"status")=="in_progress","Target version.");
Need(S(proposal["baseline"]!,"commit")==Baseline && S(proposal,"target_plan_version")=="0.1.197","Proposal identity.");
var expectedNewCanonical=proposal.DeepClone().AsObject();
if (questionActive)
{
    var projected=Canonical(expectedNewCanonical).Where(x=>readyIds.Contains(S(x,"id"))).ToArray();
    Need(projected.Length==readyIds.Count && projected.All(x=>S(x,"status")=="planned" && x["evidence"]!.AsArray().Count==0),"Canonical readiness baseline differs.");
    foreach(var item in projected) item["status"]="ready";
    var inventory=projected.Single(x=>S(x,"id")=="M12-W01-C01");
    inventory["files"]=new JsonArray(JsonValue.Create("dw.tools.math.slnx"), JsonValue.Create("global.json"), JsonValue.Create("Directory.Build.props"), JsonValue.Create("Directory.Packages.props"), JsonValue.Create("NuGet.config"), JsonValue.Create("docs/engineering/m10-cutover-readiness-verification-rs409.json"), JsonValue.Create("docs/engineering/VerifyM10ProductionCutover.cs"), JsonValue.Create("docs/engineering/InspectM10ProjectDispositions.cs"), JsonValue.Create("docs/engineering/InspectM8Net11PackageGraph.cs"), JsonValue.Create("docs/engineering/InspectM8W02TestCoverage.cs"), JsonValue.Create("docs/engineering/InspectM8Net11NumericApiSignals.cs"), JsonValue.Create("tools/dw.tools.math.discovery-bindings/Program.cs"), JsonValue.Create("tools/dw.tools.math.nuget-preflight/Program.cs"), JsonValue.Create("docs/discovery/qualification/GenerateM8MathSkill.cs"), JsonValue.Create("docs/planning/decisions/m12-source-api-evidence-inventory-question-rs435.md"));
    inventory["commands"]=new JsonArray(JsonValue.Create("dotnet run --file docs/engineering/VerifyM10ProductionCutover.cs -- --target"), JsonValue.Create("dotnet run --file docs/engineering/InspectM8Net11PackageGraph.cs -- --check"), JsonValue.Create("dotnet run --file docs/engineering/InspectM8W02TestCoverage.cs -- --check"), JsonValue.Create("dotnet run --file docs/engineering/InspectM8Net11NumericApiSignals.cs -- --check"), JsonValue.Create("dotnet run --file docs/planning/ValidatePlan.cs -- --check"));
    inventory["refinement"]="Reconcile the exact RS431 readiness evidence; activate only the Question chain. Reuse historical M10 cutover/roster receipts, current source graph and project-reference inspectors; distinguish lexical signals, declared test references, actual code coverage, selected CLR fingerprints and a complete metadata-only inventory. Specify bounded observation outputs and negative oracles before any new helper or locked build.";
    inventory["readiness"]="RS431 delivered five native readiness transitions; RS435 projects those five statuses as ready and requests native Question activation. Observation and Decision remain unstarted. No new inventory execution or benchmark claim.";
}
foreach(var key in new[]{"releases","work_packages","chunks","requirements","source_coverage"})
    CheckAppend(old[key]!.AsArray(),expectedNewCanonical[key]!.AsArray(),current[key]!.AsArray(),key);
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
    foreach (var candidate in candidates) candidate["state"]=questionActive ? "in-progress" : "ready";
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
Need(newC.Length==319 && newN.Length==319 && newC.All(x=>S(x,"status")== (questionActive && readyIds.Contains(S(x,"id")) ? "ready" : "planned") && x["evidence"]!.AsArray().Count==0) && newN.All(x=>S(x,"state")== (readiness && readyIds.Contains(S(x,"id")) ? (questionActive ? "in-progress" : "ready") : "not-ready")),"New scope must match the exact readiness frontier.");
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
    if (!questionActive)
        Exact(AtBaseline(admitted,"docs/planning/backlog.json"),current,"Canonical readiness reconciliation must await durable native evidence.");
    var rs430=Read(".aura/workflow/reports/RS430/attempt-001.json");
    var proof=Read(".workflow/runs/RS430/preparation-proof.json");
    Need(S(rs430,"outcome")=="succeeded" && (int?)rs430["attempt"]==1 && (bool?)rs430["priorInvocationFailures"]?["firstPassSucceeded"]==true && (int?)rs430["priorInvocationFailures"]?["count"]==0 && S(rs430["executor"]!,"toolVersion")=="0.1.81" && rs430["boundaryViolations"]!.AsArray().Count==0 && rs430["progress"]!["transitions"]!.AsArray().Count==0 && (bool?)proof["payloadCompiled"]==true && (int?)proof["rehearsedValidationCount"]==10,"RS430 durable admission differs.");
    Need(File.Exists("docs/planning/decisions/m12-source-api-evidence-inventory-readiness-rs431.md"),"Missing inventory scope decision.");
    if (questionActive)
    {
        var rs431=Read(".aura/workflow/reports/RS431/attempt-001.json");
        var moves=rs431["progress"]!["transitions"]!.AsArray();
        Need(S(rs431,"runId")=="RS431" && S(rs431,"outcome")=="succeeded" &&
            (int?)rs431["attempt"]==1 && (bool?)rs431["priorInvocationFailures"]?["firstPassSucceeded"]==true &&
            (int?)rs431["priorInvocationFailures"]?["count"]==0 &&
            S(rs431["executor"]!,"toolVersion")=="0.1.81" &&
            rs431["boundaryViolations"]!.AsArray().Count==0 && moves.Count==readyIds.Count &&
            moves.Select(x=>S(x!,"id")).ToHashSet(StringComparer.Ordinal).SetEquals(readyIds) &&
            moves.All(x=>S(x!,"beforeState")=="not-ready" && S(x!,"afterState")=="ready" && (bool?)x!["completed"]==false),
            "RS431 durable readiness transition differs.");
        Need(File.Exists("docs/planning/decisions/m12-source-api-evidence-inventory-question-rs435.md"),"Question scope record missing.");
        Console.WriteLine("MATH-RS435-M12-QUESTION|canonicalReady=5|nativeActive=5|newNativeNotReady=314|historicalDone=630|canonicalPlan=0.1.198|publication=false|PASS");
    }
    else
        Console.WriteLine("MATH-RS431-M12-READINESS|ready=5|newNativeNotReady=314|historicalDone=630|canonicalPlan=0.1.197|publication=false|PASS");
}
else
    Console.WriteLine("MATH-RS430-ADMISSION|oldCanonicalDone=636|oldNativeDone=630|newPlanned=319|newNativeNotReady=319|phases=59|publication=false|PASS");
