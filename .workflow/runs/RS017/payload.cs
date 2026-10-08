using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W01-C04";
const string T1 = "M1-W01-C04-T1";
const string T2 = "M1-W01-C04-T2";
const string Imported = "src/projects/dw.quantities/ApproximateEquivalence.cs";
const string Policy = "src/projects/dw.quantities/ExactComparisonPolicy.cs";
const string RedTests = "tests/projects/dw.quantities.tests/M1W01C04RedTests.cs";
const string GreenTests = "tests/projects/dw.quantities.tests/M1W01C04Tests.cs";
const string RedEvidence = "docs/planning/evidence/M1-W01-C04-red.json";
const string QualifiedEvidence = "docs/planning/evidence/M1-W01-C04-qualified.json";
const string Original = "lib/dw.quantities/ApproximateEquivalence.cs";
const string SourceSha = "34c1726fe637ea4ed3da41b793a5c9e9ef3f0088baf21d8e96c42aabd33949dc";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var plan = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/backlog.json")))!.AsObject();
var chunk = plan["chunks"]!.AsArray().Select(x => x!.AsObject())
    .Single(x => (string?)x["id"] == Phase);
var tasks = chunk["tasks"]!.AsArray().Select(x => x!.AsObject()).ToArray();
var a = tasks.Single(x => (string?)x["id"] == T1);
var b = tasks.Single(x => (string?)x["id"] == T2);
string State(JsonObject t, string suffix) =>
    (string?)t["subtasks"]!.AsArray().Select(x => x!.AsObject())
        .Single(x => (string?)x["id"] == (string?)t["id"] + "-" + suffix)["status"]
    ?? throw new InvalidOperationException("Missing subtask state.");

var baseline = (string?)plan["plan_version"] == "0.1.15" &&
    (string?)chunk["status"] == "planned" &&
    tasks.All(t => (string?)t["status"] == "planned") &&
    new[] { "R", "G", "V" }.All(s => State(a,s) == "planned" && State(b,s) == "planned");
var target = (string?)plan["plan_version"] == "0.1.16" &&
    (string?)chunk["status"] == "done" &&
    tasks.All(t => (string?)t["status"] == "done") &&
    new[] { "R", "G", "V" }.All(s => State(a,s) == "done" && State(b,s) == "done");
if (!baseline && !target)
    throw new InvalidOperationException("RS017 product lifecycle is neither accepted baseline nor full target.");

var sourceManifest = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/source-baseline.json")))!.AsObject();
var aura = sourceManifest["repositories"]!.AsArray().Select(x => x!.AsObject())
    .Single(x => (string?)x["repository"] == "aura");
if ((string?)aura["head"] != "82a6b435a387a7e116b47a6b2c433ae9e067bf21")
    throw new InvalidOperationException("The owner-authorized AURA revision is not pinned.");
var sourceEntry = aura["files"]!.AsArray().Select(x => x!.AsObject())
    .Single(x => (string?)x["path"] == Original);
if ((string?)sourceEntry["sha256"] != SourceSha)
    throw new InvalidOperationException("Authorized source SHA in manifest drifted.");
var auraRoot = (string?)aura["observed_root"]
    ?? throw new InvalidOperationException("Missing authorized AURA observed root.");
var originalPath = Path.Combine(auraRoot, Original.Replace('/', Path.DirectorySeparatorChar));
var sourceBytes = File.ReadAllBytes(originalPath);
if (Sha(sourceBytes) != SourceSha)
    throw new InvalidOperationException("Observed AURA source bytes differ from the authorized SHA.");
var sourceText = new UTF8Encoding(false,true).GetString(sourceBytes);
var declarations = sourceText.Replace("\r\n","\n",StringComparison.Ordinal)
    .Split('\n').Select(x=>x.Trim())
    .Where(x=>x.StartsWith("namespace ",StringComparison.Ordinal) ||
              x.StartsWith("public ",StringComparison.Ordinal) ||
              x.StartsWith("internal ",StringComparison.Ordinal) ||
              x.StartsWith("using ",StringComparison.Ordinal))
    .Take(100).ToArray();

// Both independent absence REDs execute against the genuine pre-transfer Math assembly.
p.Files.ReplaceFromStaged("staged/" + RedTests, RedTests);
if (baseline)
{
    var red = Run(root,"dotnet","test",
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "-c","Release", "--filter","FullyQualifiedName~M1W01C04RedTests",
        "--logger","console;verbosity=normal");
    foreach (var marker in new[] {
        "M1-W01-C04-T1 RED: imported exact comparison and quantity comparison contracts are absent.",
        "M1-W01-C04-T2 RED: tagged temperature comparison contract is absent." })
        if (red.Code == 0 || !red.Output.Contains(marker,StringComparison.Ordinal))
            throw new InvalidOperationException("Missing independent pre-transfer RED: "+marker+" | "+ImportantFailure(red.Output));
}

var redEvidence = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W01-C04-red",
    ["status"] = "independent-T1-and-T2-type-absence-observed",
    ["source_path"] = Original,
    ["source_sha256"] = SourceSha,
    ["source_api_declarations"] = p.Json.StringArray(declarations),
    ["test_filter"] = "FullyQualifiedName~M1W01C04RedTests",
    ["T1_red"] = "Imported comparison and dimensional tolerance types are absent before transfer.",
    ["T2_red"] = "Tagged temperature comparison policy type is absent before implementation.",
    ["limitations"] = "Pre-transfer absence does not establish correctness of the later exact or tolerance behaviors."
};
p.Files.WriteComplete(RedEvidence, redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

// Preserve exact transferred source. A mismatched existing destination is never overwritten.
var dest = Path.Combine(root,Imported.Replace('/',Path.DirectorySeparatorChar));
if (File.Exists(dest) && Sha(File.ReadAllBytes(dest)) != SourceSha)
    throw new InvalidOperationException("Imported Math source contains unapproved third-state bytes.");
p.Files.WriteComplete(Imported,sourceText);
if (Sha(File.ReadAllBytes(dest)) != SourceSha)
    throw new InvalidOperationException("Imported AURA source was not preserved byte-for-byte.");
p.Files.ReplaceFromStaged("staged/" + Policy,Policy);
p.Files.ReplaceFromStaged("staged/" + GreenTests,GreenTests);

RunRequired(root,"dotnet","build",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--verbosity","minimal");

foreach (var name in new[]{
    "ExactRationalEqualityIsRepresentationIndependent",
    "OneMetreAndOneHundredOneCentimetresAreEquivalentAtInclusiveOneCentimetreBoundary",
    "RelativeComparisonIsSymmetricAndMaxMagnitudeBased",
    "MismatchedDimensionsAndInvalidTolerancesAreRejected",
    "BothZeroQuantitiesHaveZeroRelativeDifference",
    "TaggedAbsoluteTemperaturesCompareOnCanonicalKelvin",
    "TemperatureToleranceMustBeAnIntervalAndNonNegative",
    "VeryLargeExactValuesDoNotConvertToFloatingPoint"
})
{
    RunRequired(root,"dotnet","test",
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "-c","Release","--no-build","--no-restore",
        "--filter","FullyQualifiedName~M1W01C04Tests."+name,
        "--logger","console;verbosity=normal");
}
RunRequired(root,"dotnet","test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-build","--no-restore",
    "--filter","FullyQualifiedName~M1W01C04",
    "--logger","console;verbosity=minimal");
RunRequired(root,"dotnet","test",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
    "-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
RunRequired(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");

var qualified = new JsonObject
{
    ["schema_version"] = 1,
    ["id"] = "M1-W01-C04-qualified",
    ["status"] = "qualified-with-declared-exact-comparison-scope",
    ["aura_revision"] = "82a6b435a387a7e116b47a6b2c433ae9e067bf21",
    ["imported_source_path"] = Imported,
    ["imported_source_sha256"] = SourceSha,
    ["comparison_policy"] = Policy,
    ["pretransfer_RED"] = RedEvidence,
    ["tested_contracts"] = p.Json.StringArray(
        "Canonical rational equality is independent of numerator/denominator representation.",
        "1 m and 101 cm are equivalent on inclusive 1 cm absolute-tolerance boundary; atol zero refuses.",
        "Symmetric relative bound uses max of absolute operands; zero and large BigInteger values stay exact.",
        "Mismatched quantity dimensions, negative tolerances and dimensional relative tolerances are rejected.",
        "Dimensionless zero is admitted as absolute tolerance with explicit nonnegative relative tolerance.",
        "Temperature comparisons use canonical base-scale tagged measurements; absolute tolerance is an interval, never an absolute temperature.",
        "Mixed absolute/interval temperatures, negative tolerance and unknown temperature kinds are refused."),
    ["focused_filter"] = "FullyQualifiedName~M1W01C04",
    ["focused_tests_passed"] = true,
    ["dw_quantities_tests_passed"] = true,
    ["foundation_chain_passed"] = true,
    ["limitations"] = p.Json.StringArray(
        "The imported ApproximateEquivalence public API is transferred with exact source provenance, but these new behavioral tests qualify the independent ExactComparisonPolicy entry points rather than every inherited overload.",
        "TemperatureMeasurement inputs must be normalized to a shared base such as Kelvin by callers using the typed UnitDefinition overload.",
        "Bare Quantity still does not encode affine temperature point/interval kind.",
        "No double fallback, global resource-budget guarantee, AURA adoption or other sibling repository mutation is claimed.")
};
p.Files.WriteComplete(QualifiedEvidence,
    qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",product=>
{
    var c=product["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var taskList=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
    product["plan_version"]="0.1.16";
    c["status"]="done";
    c["refinement"]="RS017 pin-verifies and transfers ApproximateEquivalence; records independent missing-API T1/T2 RED; implements explicit dimension-aware quantity and tagged temperature exact comparison; qualifies inclusive, zero, negative and incompatible tolerances with 8 independent oracles, full project and foundation regression.";
    c["files"]=p.Json.StringArray(Imported,Policy,RedTests,GreenTests,RedEvidence,QualifiedEvidence);
    c["commands"]=p.Json.StringArray(
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --filter FullyQualifiedName~M1W01C04",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
    c["evidence"]=p.Json.StringArray(RedEvidence,QualifiedEvidence);
    foreach(var t in taskList)
    {
        t["status"]="done";
        t["evidence"]=p.Json.StringArray(RedEvidence,QualifiedEvidence);
        foreach(var sub in t["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            sub["status"]="done";
            var id=(string?)sub["id"];
            sub["evidence"]=p.Json.StringArray(id?.EndsWith("-R",StringComparison.Ordinal)==true
                ? RedEvidence : QualifiedEvidence);
        }
    }
});

if (baseline)
{
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    p.ProjectPlan.TransitionNode(T1,"not-ready","ready");
    p.ProjectPlan.TransitionNode(T1+"-R","not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(T1+"-R");
    p.ProjectPlan.ConvergeNodeToDone(T1+"-R");
    p.ProjectPlan.TransitionNode(T1+"-G","not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(T1+"-G");
    p.ProjectPlan.ConvergeNodeToDone(T1+"-G");
    p.ProjectPlan.TransitionNode(T1+"-V","not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(T1+"-V");
    p.ProjectPlan.ConvergeNodeToDone(T1+"-V");
    p.ProjectPlan.ConvergeNodeToDone(T1);
    p.ProjectPlan.TransitionNode(T2,"not-ready","ready");
    p.ProjectPlan.TransitionNode(T2+"-R","not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(T2+"-R");
    p.ProjectPlan.ConvergeNodeToDone(T2+"-R");
    p.ProjectPlan.TransitionNode(T2+"-G","not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(T2+"-G");
    p.ProjectPlan.ConvergeNodeToDone(T2+"-G");
    p.ProjectPlan.TransitionNode(T2+"-V","not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(T2+"-V");
    p.ProjectPlan.ConvergeNodeToDone(T2+"-V");
    p.ProjectPlan.ConvergeNodeToDone(T2);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
else
{
    foreach(var id in new[]{
        T1+"-R",T1+"-G",T1+"-V",T1,
        T2+"-R",T2+"-G",T2+"-V",T2,Phase})
        p.ProjectPlan.RequireNodeState(id,"done");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
return p.Complete();

static string Sha(byte[] bytes) =>
    Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

static string ImportantFailure(string output)
{
    var marker=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(marker<0)marker=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var text=marker>=0?output[marker..]:output;
    return text.Length>3000?text[..3000]:text;
}
static (int Code,string Output) Run(string root,string fileName,params string[] args)
{
    using var proc=new Process{StartInfo=new ProcessStartInfo{
        FileName=fileName,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    proc.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    proc.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)proc.StartInfo.ArgumentList.Add(arg);
    if(!proc.Start())throw new InvalidOperationException("Failed to start "+fileName);
    var stdout=proc.StandardOutput.ReadToEndAsync();
    var stderr=proc.StandardError.ReadToEndAsync();
    if(!proc.WaitForExit(420000)){
        proc.Kill(entireProcessTree:true);
        throw new TimeoutException(fileName+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(proc.ExitCode,output);
}
static void RunRequired(string root,string exe,params string[] args)
{
    var r=Run(root,exe,args);
    if(r.Code!=0)
        throw new InvalidOperationException(exe+" "+string.Join(" ",args)+
            " exited "+r.Code+": "+ImportantFailure(r.Output));
}
