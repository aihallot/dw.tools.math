using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W02-C02";
const string Task = "M1-W02-C02-T1";
const string Red = "M1-W02-C02-T1-R";
const string Green = "M1-W02-C02-T1-G";
const string TestPath = "tests/projects/dw.quantities.tests/M1W02C02RedTests.cs";
const string Evidence = "docs/planning/evidence/M1-W02-C02-source-red.json";
const string MarkerA = "M1-W02-C02-T1 RED: standalone dw.quantities.expression assembly is absent from Math.";
const string MarkerB = "M1-W02-C02-T1 RED: no exported parser or expression contract is materialized.";

var sources = new (string Original, string ExpectedSha, string Snapshot)[]
{
    ("lib/dw.quantities.expression/dw.quantities.expression.csproj",
        "7c7c1d8bc7df4b69fc642af0b79c88721a38e9f8e706ff530973a8664f25f833",
        "docs/planning/evidence/M1-W02-C02-AURA-expression-project.txt"),
    ("lib/dw.quantities.expression/ExpressionParser.cs",
        "6f553e58c5e4e788932b3d02cfadae2f8abecf564e8595296c8a91d7c4bfd765",
        "docs/planning/evidence/M1-W02-C02-AURA-ExpressionParser.txt"),
    ("tests/dw.quantities.tests/ExpressionEngineTests.cs",
        "b19d74ca76930513b0dd8e3d324b1591a6662cfa8408a44fee8c676883219087",
        "docs/planning/evidence/M1-W02-C02-AURA-ExpressionEngineTests.txt"),
    ("src/aura.domains/core/aura.domains.core.math/ExpressionParser.cs",
        "9e53bc3bea0b48eb153e88f03b89607b14ded4e4aa838ad5b4602b72c93e4e08",
        "docs/planning/evidence/M1-W02-C02-AURA-host-parser-facade.txt")
};

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var backlog = JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/backlog.json")))!.AsObject();
var chunk = backlog["chunks"]!.AsArray().Select(n=>n!.AsObject()).Single(n=>(string?)n["id"]==Phase);
var task = chunk["tasks"]!.AsArray().Select(n=>n!.AsObject()).Single(n=>(string?)n["id"]==Task);
var subs = task["subtasks"]!.AsArray().Select(n=>n!.AsObject()).ToArray();
var red = subs.Single(n=>(string?)n["id"]==Red);
var green = subs.Single(n=>(string?)n["id"]==Green);
var verify = subs.Single(n=>(string?)n["id"]=="M1-W02-C02-T1-V");
var next = chunk["tasks"]!.AsArray().Select(n=>n!.AsObject()).Single(n=>(string?)n["id"]=="M1-W02-C02-T2");

var baseline = (string?)backlog["plan_version"]=="0.1.19" &&
    (string?)chunk["status"]=="planned" && (string?)task["status"]=="planned" &&
    (string?)red["status"]=="planned" && (string?)green["status"]=="planned" &&
    (string?)verify["status"]=="planned" && (string?)next["status"]=="planned";
var target = (string?)backlog["plan_version"]=="0.1.20" &&
    (string?)chunk["status"]=="in_progress" && (string?)task["status"]=="in_progress" &&
    (string?)red["status"]=="done" && (string?)green["status"]=="ready" &&
    (string?)verify["status"]=="planned" && (string?)next["status"]=="planned";
if(!baseline && !target)
    throw new InvalidOperationException("RS021 product lifecycle is neither approved baseline nor exact RED target.");

var manifest = JsonNode.Parse(File.ReadAllText(Path.Combine(root,"docs/planning/source-baseline.json")))!.AsObject();
var aura = manifest["repositories"]!.AsArray().Select(n=>n!.AsObject())
    .Single(n=>(string?)n["repository"]=="aura");
if((string?)aura["head"]!="82a6b435a387a7e116b47a6b2c433ae9e067bf21")
    throw new InvalidOperationException("AURA authority revision differs from the approved source baseline.");
var auraRoot = (string?)aura["observed_root"]
    ?? throw new InvalidOperationException("Missing authorized AURA source root.");
var pinned = aura["files"]!.AsArray().Select(n=>n!.AsObject()).ToArray();

p.Files.ReplaceFromStaged("staged/"+TestPath,TestPath);
if(baseline)
{
    var redResult=Run(root,"dotnet","test",
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "-c","Release","--filter","FullyQualifiedName~M1W02C02RedTests",
        "--logger","console;verbosity=normal");
    if(redResult.Code==0 ||
        !redResult.Output.Contains(MarkerA,StringComparison.Ordinal) ||
        !redResult.Output.Contains(MarkerB,StringComparison.Ordinal))
        throw new InvalidOperationException("Independent missing-parser RED did not fail for both expected markers: "+
            Significant(redResult.Output));
}

JsonObject Capture((string Original,string ExpectedSha,string Snapshot) file)
{
    var entry=pinned.Single(n=>(string?)n["path"]==file.Original);
    if((string?)entry["sha256"]!=file.ExpectedSha)
        throw new InvalidOperationException("Manifest source SHA drifted: "+file.Original);
    var external=Path.Combine(auraRoot,file.Original.Replace('/',Path.DirectorySeparatorChar));
    var bytes=File.ReadAllBytes(external);
    if(Hash(bytes)!=file.ExpectedSha)
        throw new InvalidOperationException("AURA source has changed since authorization: "+file.Original);
    var decoded=new UTF8Encoding(false,true).GetString(bytes);
    p.Files.WriteComplete(file.Snapshot,decoded);
    var snapshotPath=Path.Combine(root,file.Snapshot.Replace('/',Path.DirectorySeparatorChar));
    var savedHash=Hash(File.ReadAllBytes(snapshotPath));
    var lines=decoded.Replace("\r\n","\n",StringComparison.Ordinal).Split('\n');
    var declarations=lines.Select(x=>x.Trim())
        .Where(x=>x.StartsWith("using ",StringComparison.Ordinal) ||
                  x.StartsWith("namespace ",StringComparison.Ordinal) ||
                  x.StartsWith("public ",StringComparison.Ordinal) ||
                  x.StartsWith("internal ",StringComparison.Ordinal) ||
                  x.StartsWith("private const ",StringComparison.Ordinal))
        .Take(180).ToArray();
    var hits=new[]{"4096","256","32","16","IExpressionUnitResolver",
        "ExpressionUnitResolution","ExpressionParser","ExactRational","dw.localization"};
    return new JsonObject
    {
        ["source"]=file.Original,
        ["source_sha256"]=file.ExpectedSha,
        ["byte_count"]=bytes.Length,
        ["line_count"]=lines.Length,
        ["review_snapshot"]=file.Snapshot,
        ["snapshot_sha256"]=savedHash,
        ["snapshot_matches_source_bytes"]=savedHash==file.ExpectedSha,
        ["declarations_captured"]=p.Json.StringArray(declarations),
        ["textual_feature_mentions"]=p.Json.StringArray(
            hits.Where(x=>decoded.Contains(x,StringComparison.Ordinal)).ToArray()),
        ["disposition"]=file.Original.Contains("aura.domains",StringComparison.Ordinal)
            ? "AURA command/domain facade: review only; not a Math transfer."
            : file.Original.StartsWith("tests/",StringComparison.Ordinal)
                ? "Independent legacy tests: characterization reservoir, not a Math product source."
                : "Owner-authorized Math extraction candidate: review its exact API and dependencies before transfer."
    };
}

var snapshots=p.Json.Array(sources.Select(Capture).Cast<JsonNode>().ToArray());
var evidence=new JsonObject
{
    ["schema_version"]=1,
    ["id"]="M1-W02-C02-source-red",
    ["status"]="source-contract-characterized-with-observed-parser-absence",
    ["aura_revision"]="82a6b435a387a7e116b47a6b2c433ae9e067bf21",
    ["missing_parser_test"]="FullyQualifiedName~M1W02C02RedTests",
    ["red_failure_markers"]=p.Json.StringArray(MarkerA,MarkerB),
    ["source_observations"]=snapshots,
    ["expected_independent_oracles"]=p.Json.StringArray(
        "1/3 + 1/6 = 1/2 without binary64 conversion",
        "Admitted exact square roots produce exact rationals; irrational roots reject exact-only mode",
        "Length up to 4096 characters, 256 tokens, depth 32, 256 digit magnitude and power/root 16 must be characterized at public API boundaries",
        "Expression input never executes arbitrary C# and never accepts free variables"),
    ["limits"]=p.Json.StringArray(
        "This RED proves the independent expression package is absent; no parser behavior or resource bounds have passed yet.",
        "Source captures are pinned and complete; declaration excerpts and numeric token occurrences are only discovery, not behavioral qualification.",
        "The AURA domain facade is excluded from extraction; the pure parser package alone is the candidate.")
};
p.Files.WriteComplete(Evidence,evidence.ToJsonString(new JsonSerializerOptions { WriteIndented = true })+"\n");

p.Json.EditObject("docs/planning/backlog.json",product=>{
    var c=product["chunks"]!.AsArray().Select(n=>n!.AsObject()).Single(n=>(string?)n["id"]==Phase);
    var t=c["tasks"]!.AsArray().Select(n=>n!.AsObject()).Single(n=>(string?)n["id"]==Task);
    var r=t["subtasks"]!.AsArray().Select(n=>n!.AsObject()).Single(n=>(string?)n["id"]==Red);
    var g=t["subtasks"]!.AsArray().Select(n=>n!.AsObject()).Single(n=>(string?)n["id"]==Green);
    product["plan_version"]="0.1.20";
    c["status"]="in_progress";
    c["refinement"]="RS021 captures and SHA-verifies the approved pure expression parser, its project and legacy tests together with the separately excluded AURA host facade, and establishes an independent missing-assembly RED. RS022 must derive the compiled public API and mathematical test oracles from these complete snapshots before claiming transfer or RED/GREEN verification.";
    c["files"]=p.Json.StringArray(
        "src/projects/dw.quantities.expression/ExpressionParser.cs",
        "src/projects/dw.quantities.expression/dw.quantities.expression.csproj",
        TestPath,Evidence,
        sources[0].Snapshot,sources[1].Snapshot,sources[2].Snapshot,sources[3].Snapshot);
    c["commands"]=p.Json.StringArray(
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --filter FullyQualifiedName~M1W02C02RedTests",
        "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
    c["readiness"]="T1 RED qualified; approved pure parser/project and inherited test sources captured and hash-verified; T1 GREEN requires compiled dependency and public API qualification.";
    c["evidence"]=p.Json.StringArray(Evidence);
    t["status"]="in_progress";
    r["status"]="done";
    r["evidence"]=p.Json.StringArray(Evidence);
    g["status"]="ready";
});
if(baseline)
{
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Red,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Red);
    p.ProjectPlan.ConvergeNodeToDone(Red);
    p.ProjectPlan.TransitionNode(Green,"not-ready","ready");
}
else
{
    p.ProjectPlan.RequireNodeState(Phase,"in-progress");
    p.ProjectPlan.RequireNodeState(Task,"in-progress");
    p.ProjectPlan.RequireNodeState(Red,"done");
    p.ProjectPlan.RequireNodeState(Green,"ready");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
return p.Complete();

static string Hash(byte[] bytes)=>Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
static string Significant(string output)
{
    var marker=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(marker<0)marker=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var interesting=marker>=0?output[marker..]:output;
    return interesting.Length>2500?interesting[..2500]:interesting;
}
static (int Code,string Output) Run(string root,string exe,params string[] args)
{
    using var proc=new Process{StartInfo=new ProcessStartInfo{
        FileName=exe,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    proc.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    proc.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)proc.StartInfo.ArgumentList.Add(arg);
    if(!proc.Start())throw new InvalidOperationException("Cannot start "+exe);
    var stdout=proc.StandardOutput.ReadToEndAsync();
    var stderr=proc.StandardError.ReadToEndAsync();
    if(!proc.WaitForExit(420000))
    {
        proc.Kill(entireProcessTree:true);
        throw new TimeoutException(exe+" timed out.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(proc.ExitCode,output);
}
