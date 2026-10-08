using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Phase = "M1-W02-C01";
const string T1 = "M1-W02-C01-T1";
const string T2 = "M1-W02-C01-T2";
const string RedTests = "tests/projects/dw.quantities.tests/M1W02C01RedTests.cs";
const string GreenRedStage = "tests/projects/dw.quantities.tests/M1W02C01RedTests.green.cs";
const string GreenTests = "tests/projects/dw.quantities.tests/M1W02C01Tests.cs";
const string RedEvidence = "docs/planning/evidence/M1-W02-C01-red.json";
const string Foundation = "docs/planning/evidence/M1-W02-C01-initial-catalog.json";
const string Catalog = "src/projects/dw.quantities.standard/StandardUnitCatalog.cs";
const string Resolver = "src/projects/dw.quantities.standard/StandardExpressionUnitResolver.cs";
const string Converter = "src/projects/dw.quantities.standard/PureUnitConverter.cs";
const string StandardProject = "src/projects/dw.quantities.standard/dw.quantities.standard.csproj";
const string StandardLock = "src/projects/dw.quantities.standard/packages.lock.json";
const string TestProject = "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj";
const string TestLock = "tests/projects/dw.quantities.tests/packages.lock.json";
const string Solution = "dw.tools.math.slnx";
const string SourceCatalog = "lib/dw.quantities.standard/StandardUnitCatalog.cs";
const string SourceResolver = "lib/dw.quantities.standard/StandardExpressionUnitResolver.cs";
const string SourceCatalogSha = "f588545e89f51c78dd82a0d7c08ea92ab90fa48d0f5862d18666a00d0f13249a";
const string SourceResolverSha = "3e71ed4af6abc748665c3f382093d625974c002649a8353d82dc4b5ad02bb561";

var p = PayloadContext.Create();
var root = p.RepositoryRoot;
var plan = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/backlog.json")))!.AsObject();
var chunk = plan["chunks"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == Phase);
var work1 = plan["work_packages"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == "M1-W01");
var work2 = plan["work_packages"]!.AsArray().Select(n => n!.AsObject()).Single(n => (string?)n["id"] == "M1-W02");
var tasks = chunk["tasks"]!.AsArray().Select(n => n!.AsObject()).ToArray();
var first = tasks.Single(n => (string?)n["id"] == T1);
var second = tasks.Single(n => (string?)n["id"] == T2);
string Sub(JsonObject task, string suffix) =>
    (string?)task["subtasks"]!.AsArray().Select(n => n!.AsObject())
        .Single(n => (string?)n["id"] == (string?)task["id"] + "-" + suffix)["status"]
        ?? "<missing>";

var baseline = (string?)plan["plan_version"] == "0.1.16" &&
    (string?)work1["status"] == "in_progress" && (string?)work2["status"] == "planned" &&
    (string?)chunk["status"] == "planned" && tasks.All(t => (string?)t["status"] == "planned") &&
    new[] { "R", "G", "V" }.All(s => Sub(first,s) == "planned" && Sub(second,s) == "planned");
var target = (string?)plan["plan_version"] == "0.1.17" &&
    (string?)work1["status"] == "done" && (string?)work2["status"] == "in_progress" &&
    (string?)chunk["status"] == "in_progress" &&
    (string?)first["status"] == "done" && (string?)second["status"] == "in_progress" &&
    new[] { "R", "G", "V" }.All(s => Sub(first,s) == "done") &&
    Sub(second,"R") == "done" && Sub(second,"G") == "ready" && Sub(second,"V") == "planned";
if (!baseline && !target)
    throw new InvalidOperationException("RS018 product hierarchy is neither accepted baseline nor target.");

var sourceManifest = JsonNode.Parse(File.ReadAllText(Path.Combine(root, "docs/planning/source-baseline.json")))!.AsObject();
var aura = sourceManifest["repositories"]!.AsArray().Select(n=>n!.AsObject())
    .Single(n=>(string?)n["repository"]=="aura");
if ((string?)aura["head"] != "82a6b435a387a7e116b47a6b2c433ae9e067bf21")
    throw new InvalidOperationException("AURA source revision authority drifted.");
var sourceRoot = (string?)aura["observed_root"] ?? throw new InvalidOperationException("AURA root absent.");
var sourceEntries = aura["files"]!.AsArray().Select(n=>n!.AsObject()).ToArray();
JsonObject ObservePinnedSource(string source, string expected)
{
    var entry=sourceEntries.Single(n=>(string?)n["path"]==source);
    if((string?)entry["sha256"]!=expected)
        throw new InvalidOperationException("Pinned manifest hash differs for "+source);
    var filename=Path.Combine(sourceRoot,source.Replace('/',Path.DirectorySeparatorChar));
    var bytes=File.ReadAllBytes(filename);
    var actual=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    if(actual!=expected)
        throw new InvalidOperationException("Source has changed since the approved AURA snapshot: "+source);
    var lines=new UTF8Encoding(false,true).GetString(bytes).Replace("\r\n","\n",StringComparison.Ordinal).Split('\n');
    var declarations=lines.Select(x=>x.Trim())
        .Where(x=>x.StartsWith("namespace ",StringComparison.Ordinal) ||
            x.StartsWith("public ",StringComparison.Ordinal) ||
            x.StartsWith("using ",StringComparison.Ordinal))
        .Take(100).ToArray();
    return new JsonObject{
        ["source"]=source,
        ["source_sha256"]=actual,
        ["bytes"]=bytes.Length,
        ["captured_declarations"]=p.Json.StringArray(declarations),
        ["disposition"]="Observation and owner-authorized behavioral adaptation; source is not copied verbatim."
    };
}
var sources=p.Json.Array(
    ObservePinnedSource(SourceCatalog,SourceCatalogSha),
    ObservePinnedSource(SourceResolver,SourceResolverSha));
var decision=JsonNode.Parse(File.ReadAllText(Path.Combine(root,
    "docs/planning/evidence/M0-W02-C02-transfer-manifest.json")))!.AsObject();
if((string?)decision["localization_decision"]?["selected_option"]!="passive_bcl_metadata")
    throw new InvalidOperationException("Catalog localization decision no longer allows passive BCL metadata.");

// Pre-transfer RED: source and test assembly have no materialized standard package.
p.Files.ReplaceFromStaged("staged/"+RedTests,RedTests);
if(baseline)
{
    var red=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--filter","FullyQualifiedName~M1W02C01RedTests",
        "--logger","console;verbosity=normal");
    foreach(var marker in new[]{
        "M1-W02-C01-T1 RED: stand-alone dw.quantities.standard catalog has not been materialized.",
        "M1-W02-C01-T2 RED: explicit-profile strict unit resolver has not been materialized."})
        if(red.Code==0||!red.Output.Contains(marker,StringComparison.Ordinal))
            throw new InvalidOperationException("C01 pre-transfer RED mismatch: "+marker+" "+Significant(red.Output));
}
var redEvidence=new JsonObject{
    ["schema_version"]=1,["id"]="M1-W02-C01-red",
    ["status"]="both-initial-absences-observed",
    ["test_filter"]="FullyQualifiedName~M1W02C01RedTests",
    ["scope"]="Absence of stand-alone standard package and its strict profile resolver, before any Math package graph change.",
    ["source_candidates"]=sources.DeepClone()
};
p.Files.WriteComplete(RedEvidence,redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

foreach(var path in new[]{Catalog,Resolver,Converter,StandardProject,StandardLock,TestProject,TestLock,Solution,GreenTests})
    p.Files.ReplaceFromStaged("staged/"+path,path);
p.Files.ReplaceFromStaged("staged/"+GreenRedStage,RedTests);

RunRequired(root,"dotnet","restore",Solution,"--locked-mode");
RunRequired(root,"dotnet","build",Solution,"-c","Release","--no-restore");
foreach(var name in new[]{
    "CatalogHasStableExplicitVersionAndStandaloneNamespace",
    "InchAndIecInformationUnitsUseExactHandComputableConstants",
    "CupAndPintRequireExplicitProfileForAmbiguousTokens",
    "UsUkAustralianVolumeProfilesHaveExplicitAndDifferentExactConstants",
    "ProcessCultureCannotChangeResolutionOrConversion",
    "MismatchedDimensionsAreRejectedAndNoUnsafeUnitKindsAreAdmitted",
    "DecimalOrLocalizedAliasesNeverSelectAPintProfileImplicitly"})
    RunRequired(root,"dotnet","test",TestProject,"-c","Release","--no-restore","--no-build",
        "--filter","FullyQualifiedName~M1W02C01Tests."+name,"--logger","console;verbosity=normal");
RunRequired(root,"dotnet","test",TestProject,"-c","Release","--no-restore","--no-build",
    "--logger","console;verbosity=minimal");
RunRequired(root,"pwsh","-NoProfile","-NonInteractive","-File","scripts/verify.ps1");
RunRequired(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var evidence=new JsonObject{
    ["schema_version"]=1,
    ["id"]="M1-W02-C01-initial-catalog",
    ["status"]="initial-standalone-catalog-and-tests-qualified-full-source-parity-pending",
    ["aura_revision"]="82a6b435a387a7e116b47a6b2c433ae9e067bf21",
    ["original_sources"]=sources.DeepClone(),
    ["decision"]="docs/planning/decisions/m0-w02-c02.md",
    ["catalog_version"]="2026-10-exact-v1",
    ["package"]="dw.quantities.standard",
    ["dependency_policy"]="Only dw.quantities project; no AURA, dw.localization or host-specific localization dependency.",
    ["tests"]=p.Json.StringArray(
        "1 inch=127/5000 m; 1 KiB=1024 B",
        "Explicit US liquid, UK imperial and AU culinary cup and pint volume assumptions",
        "Australian pint uses the labelled 570 mL beer-serving profile, not a universal legal volumetric standard",
        "Ambiguous cup and pint require explicit profile; unresolved/unsupported currencies and logarithmic units rejected",
        "Host culture en-US, fr-BE, nl-BE cannot select aliases or conversion behavior",
        "Incompatible dimensions rejected; passive localized names only"),
    ["focused_filter"]="FullyQualifiedName~M1W02C01Tests",
    ["focused_passed"]=true,
    ["quantities_tests_passed"]=true,
    ["foundation_chain_passed"]=true,
    ["transfer_architecture_check_passed"]=true,
    ["limitations"]=p.Json.StringArray(
        "This is a bounded pure standard catalogue adaptation, not a byte-identical or exhaustive port of either AURA standard source.",
        "No expression-parser integration is claimed, and inherited aliases and coverage remain to be reconciled during T2.",
        "The Australian 570 mL pint denotes a chosen beer-serving profile rather than a statutory universal definition.",
        "No sibling repository is modified; package graph does not adopt dw.localization.")
};
p.Files.WriteComplete(Foundation,evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

p.Json.EditObject("docs/planning/backlog.json",product=>{
    var w1=product["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M1-W01");
    var w2=product["work_packages"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M1-W02");
    var c=product["chunks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
    var t1=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==T1);
    var t2=c["tasks"]!.AsArray().Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==T2);
    product["plan_version"]="0.1.17";
    w1["status"]="done";
    w2["status"]="in_progress";
    c["status"]="in_progress";
    c["refinement"]="RS018 verifies pinned AURA source authority, creates a BCL-only dw.quantities.standard adaptation rather than a byte-identical imported file, independently records T1/T2 missing-package RED, and qualifies seven exact focused catalog tests. Full source parity and integrated grammar coverage remain T2.";
    c["files"]=p.Json.StringArray(Catalog,Resolver,Converter,StandardProject,StandardLock,
        TestProject,TestLock,Solution,RedTests,GreenTests,RedEvidence,Foundation);
    c["commands"]=p.Json.StringArray(
        "dotnet restore dw.tools.math.slnx --locked-mode",
        "dotnet build dw.tools.math.slnx -c Release --no-restore",
        "dotnet test tests/projects/dw.quantities.tests/dw.quantities.tests.csproj -c Release --filter FullyQualifiedName~M1W02C01Tests",
        "pwsh -NoProfile -NonInteractive -File scripts/verify.ps1",
        "dotnet run --file docs/planning/ValidateTransferArchitecture.cs");
    c["evidence"]=p.Json.StringArray(RedEvidence,Foundation);
    t1["status"]="done";
    t1["evidence"]=p.Json.StringArray(RedEvidence,Foundation);
    foreach(var sub in t1["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
    {
        sub["status"]="done";
        sub["evidence"]=p.Json.StringArray((string?)sub["id"]==T1+"-R"?RedEvidence:Foundation);
    }
    t2["status"]="in_progress";
    var sub2=t2["subtasks"]!.AsArray().Select(x=>x!.AsObject()).ToArray();
    sub2.Single(x=>(string?)x["id"]==T2+"-R")["status"]="done";
    sub2.Single(x=>(string?)x["id"]==T2+"-R")["evidence"]=p.Json.StringArray(RedEvidence);
    sub2.Single(x=>(string?)x["id"]==T2+"-G")["status"]="ready";
});
if(baseline)
{
    p.ProjectPlan.ConvergeNodeToDone("M1-W01");
    p.ProjectPlan.TransitionNode("M1-W02","not-ready","ready");
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
}
else
{
    foreach(var id in new[]{"M1-W01",T1+"-R",T1+"-G",T1+"-V",T1,T2+"-R"})
        p.ProjectPlan.RequireNodeState(id,"done");
    foreach(var id in new[]{T2+"-G"})
        p.ProjectPlan.RequireNodeState(id,"ready");
    foreach(var id in new[]{"M1-W02",Phase,T2})
        p.ProjectPlan.RequireNodeState(id,"in-progress");
}
RunRequired(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
return p.Complete();

static string Significant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var result=index>=0?output[index..]:output;
    return result.Length>1800?result[..1800]:result;
}
static (int Code,string Output) Run(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo{
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true}};
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    var stdout=Task.FromResult(string.Empty);
    if(!process.Start())throw new InvalidOperationException("Cannot start "+executable);
    var readOutput=process.StandardOutput.ReadToEndAsync();
    var readError=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000)){
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out.");
    }
    var output=readOutput.GetAwaiter().GetResult()+"\n"+readError.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return (process.ExitCode,output);
}
static void RunRequired(string root,string exe,params string[] args)
{
    var r=Run(root,exe,args);
    if(r.Code!=0)throw new InvalidOperationException(
        exe+" "+string.Join(" ",args)+" exited "+r.Code+": "+Significant(r.Output));
}
