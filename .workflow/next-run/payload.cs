using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string State=".aura/workflow/state.json";
const string Backlog="docs/planning/backlog.json";
const string M4Gate="docs/planning/evidence/M4-gate.json";
const string M2Gate="docs/planning/evidence/M2-gate.json";
const string M3Gate="docs/planning/evidence/M3-gate.json";
const string W01Candidate="docs/planning/evidence/M4-W01-C01-T1-qualified.json";
const string W01Host="docs/planning/evidence/M4-W01-C01-T2-boundaries-qualified.json";
const string W01Bridge="docs/planning/evidence/M4-W01-C02-T2-boundaries-qualified.json";
const string W01Transform="docs/planning/evidence/M4-W01-C03-T2-boundaries-qualified.json";
const string W02Derivative="docs/planning/evidence/M4-W02-C01-T2-boundaries-qualified.json";
const string W02Solver="docs/planning/evidence/M4-W02-C02-T2-boundaries-qualified.json";
const string W02Calculus="docs/planning/evidence/M4-W02-C03-T2-boundaries-qualified.json";
const string WorkAggregate="docs/planning/evidence/M4-W02-work-package-qualified.json";
const string GateValidator="docs/planning/ValidateM4ReleaseGate.cs";
const string Decision="docs/planning/decisions/m4-gate.md";
const string Document="docs/distribution/m4-release-gate.md";
const string ConsumerProject="tests/consumers/m4-gate/m4-gate-consumer.csproj";
const string ConsumerSource="tests/consumers/m4-gate/Program.cs";
const string ConsumerConfig="tests/consumers/m4-gate/NuGet.Config";
const string PackageHelper=".workflow/runs/RS065/staged/checks/m4-gate-package-sha.cs";
const string ProductProject="src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj";
const string Solution="dw.tools.math.slnx";
const string Version="0.5.0-preview.10";
const string GateMarker="dw.tools.math/m4-scientific-gate/0.5 qualified; phases=6; tasks=12; subtasks=36";
const string ConsumerMarker="dw.tools.math/m4-nuget-gate/0.5 qualified; probes=32";
const string GateStatus="m4-local-exact-symbolic-preview-gate-qualified";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,M4Gate));
p.Workflow.RequireCurrentRun("RS065",target?null:"M4",null,null,null,null);
p.Workflow.RequirePriorSuccess("RS064",1,
    "feat: qualify exact calculus boundaries and close M4 W02",
    "0.1.59",0,0,0,true);

p.Require.JsonValueEquals(M2Gate,"/status",
    JsonValue.Create("local-m2-typed-ir-composition-and-isolated-consumer-qualified"));
p.Require.JsonValueEquals(M3Gate,"/status",
    JsonValue.Create("m3-local-numerics-and-statistics-preview-gate-qualified"));
p.Require.JsonValueEquals(W01Candidate,"/status",
    JsonValue.Create("m4-two-managed-symbolic-provider-and-conditional-result-candidate-qualified"));
p.Require.JsonValueEquals(W01Host,"/status",
    JsonValue.Create("m4-isolated-host-budget-cancellation-and-assumption-refusal-qualified"));
p.Require.JsonValueEquals(W01Host,"/public_symbolic_adapter_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(W01Bridge,"/status",
    JsonValue.Create("strict-typed-symbolic-provider-output-and-condition-admission-qualified"));
p.Require.JsonValueEquals(W01Transform,"/status",
    JsonValue.Create("bounded-symbolic-provenance-cancellation-and-soft-deadline-qualified"));
p.Require.JsonValueEquals(W02Derivative,"/status",
    JsonValue.Create("real-exact-pointwise-derivative-and-domain-refusal-qualified"));
p.Require.JsonValueEquals(W02Solver,"/status",
    JsonValue.Create("original-real-equation-root-and-multiplicity-boundaries-qualified"));
p.Require.JsonValueEquals(W02Calculus,"/status",
    JsonValue.Create("real-calculus-original-domain-pole-hole-and-unknown-convergence-qualified"));
p.Require.JsonValueEquals(W02Calculus,"/independent_green_tests_passed",JsonValue.Create(51));
p.Require.JsonValueEquals(W02Calculus,"/independent_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(W02Calculus,"/local_product_version",JsonValue.Create(Version));
p.Require.JsonValueEquals(W02Calculus,"/m4_w02_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(W02Calculus,"/m4_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(WorkAggregate,"/status",
    JsonValue.Create("exact-symbolic-m4-w02-work-package-qualified"));
p.Require.JsonValueEquals(WorkAggregate,"/m4_w02_work_package_done",JsonValue.Create(true));
p.Require.JsonValueEquals(WorkAggregate,"/m4_release_done",JsonValue.Create(false));
p.Require.JsonValueEquals(WorkAggregate,"/last_symbolics_version",JsonValue.Create(Version));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.63"),JsonValue.Create("0.1.64"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.64":"0.1.63"));
p.Require.JsonValueEquals(Backlog,"/releases/2/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/work_packages/9/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/work_packages/10/status",JsonValue.Create("done"));
foreach(var index in new[]{27,28,29,30,31,32})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{index}/status",JsonValue.Create("done"));
p.Require.TextContains(ProductProject,"<Version>0.5.0-preview.10</Version>");
p.ProjectPlan.RequireNodeState("M2","done");
p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4",target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M4-W01","done");
p.ProjectPlan.RequireNodeState("M4-W02","done");
foreach(var phase in new[]{
    "M4-W01-C01","M4-W01-C02","M4-W01-C03",
    "M4-W02-C01","M4-W02-C02","M4-W02-C03"})
    p.ProjectPlan.RequireNodeState(phase,"done");
p.ProjectPlan.RequireNodeState("M5","not-ready");

if(!target)
{
    foreach(var file in new[]{GateValidator,Decision,Document,
        ConsumerProject,ConsumerSource,ConsumerConfig,M4Gate})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(M4Gate,"/status",JsonValue.Create(GateStatus));
    p.Require.JsonValueEquals(M4Gate,"/gate",JsonValue.Create("G-M4"));
    p.Require.JsonValueEquals(M4Gate,"/cross_vertical_nuget_consumer_oracles",
        JsonValue.Create(32));
    p.Require.JsonValueEquals(M4Gate,"/m4_milestone_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(M4Gate,"/packages_publicly_published",JsonValue.Create(false));
}

var stagedHashes=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{GateValidator,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var receipt=p.Files.ReplaceFromStaged("staged/"+file,file);
    stagedHashes[file]=receipt.AfterSha256 ??
        throw new InvalidOperationException("Missing staged gate source identity: "+file);
}
var scientific=Required(root,"dotnet","run","--file",GateValidator);
if(!HasExactLine(scientific,GateMarker))
    throw new InvalidOperationException("The six-phase scientific M4 gate was not observed.");

Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var testedProjects=new[]{
    "tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj",
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"};
foreach(var project in testedProjects)
{
    var result=Required(root,"dotnet","test",project,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!result.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Focused science pack lacks passing tests: "+project);
}
var complete=Required(root,"dotnet","test",Solution,
    "-c","Release","--no-build","--no-restore",
    "--logger","console;verbosity=minimal");
if(!complete.Contains("Passed",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Complete solution test success not observed.");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var packages=new (string Id,string Version,string Project,string[] DependsOn)[]
{
    ("dw.quantities","0.2.0-preview.1",
        "src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.symbolics",Version,ProductProject,["dw.tools.math.ir"])
};
var feed=Path.Combine(root,"build","artifacts","m4-gate-local-feed");
var archives=new List<string>();
foreach(var package in packages)
{
    Required(root,"dotnet","pack",package.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,package.Id+"."+package.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("Expected local .NET product archive absent: "+package.Id);
    archives.Add(archive);
}
var helperArguments=new List<string>{"run","--file",PackageHelper,"--"};
helperArguments.AddRange(archives);
var archiveOutput=Required(root,"dotnet",helperArguments.ToArray());
var products=new List<JsonNode>();
foreach(var package in packages)
{
    var receipt=ReadProductReceipt(archiveOutput,package.Id,package.Version);
    products.Add(new JsonObject
    {
        ["id"]=package.Id,["version"]=package.Version,
        ["bytes"]=receipt.Bytes,["sha256"]=receipt.Sha,
        ["dependencies"]=p.Json.StringArray(package.DependsOn),
        ["assembly"]="lib/net10.0/"+package.Id+".dll",
        ["publication"]="local-only-no-public-feed"
    });
}

var consumerXml=XDocument.Load(Path.Combine(root,ConsumerProject));
var refs=consumerXml.Descendants().Where(e=>e.Name.LocalName=="PackageReference").ToArray();
if(refs.Length!=1||
   (string?)refs[0].Attribute("Include")!="dw.tools.math.symbolics"||
   (string?)refs[0].Attribute("Version")!="["+Version+"]"||
   consumerXml.Descendants().Any(e=>e.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("Independent consumer must use one pinned symbolics NuGet dependency.");
var sources=XDocument.Load(Path.Combine(root,ConsumerConfig)).Descendants()
    .Single(e=>e.Name.LocalName=="packageSources");
if(!sources.Elements().Any(e=>e.Name.LocalName=="clear")||
   sources.Elements().Any(e=>e.Name.LocalName=="add"))
    throw new InvalidOperationException("The NuGet-only M4-gate consumer must clear external sources.");
var isolated=Path.Combine(root,"build","artifacts","m4-gate-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException("Thirty-two NuGet-only M4 release oracles not observed.");


if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-gate",
        ["status"]=GateStatus,
        ["milestone"]="M4",["gate"]="G-M4",
        ["release_version"]="0.5.0-preview",
        ["plan_version"]="0.1.64",
        ["previous_m2_gate"]=M2Gate,
        ["previous_m3_gate"]=M3Gate,
        ["checked_chunk_ids"]=p.Json.StringArray(
            "M4-W01-C01","M4-W01-C02","M4-W01-C03",
            "M4-W02-C01","M4-W02-C02","M4-W02-C03"),
        ["qualified_phase_evidence"]=p.Json.StringArray(
            W01Host,W01Bridge,W01Transform,W02Derivative,W02Solver,W02Calculus),
        ["qualified_work_packages"]=p.Json.StringArray("M4-W01","M4-W02"),
        ["qualified_tasks"]=12,["qualified_subtasks"]=36,
        ["gate_scientific_validator"]=GateValidator,
        ["gate_scientific_validator_sha256"]=stagedHashes[GateValidator],
        ["scientific_gate_marker"]=GateMarker,
        ["science_validator_rechecked_after_progress"]=true,
        ["locked_solution_restore_and_build_passed"]=true,
        ["focused_scientific_regressions"]=p.Json.StringArray(testedProjects),
        ["full_solution_test_suite_passed"]=true,
        ["full_solution_test_command"]="dotnet test "+Solution+" -c Release --no-build --no-restore",
        ["typed_ir_and_original_domain_retained"]=true,
        ["provider_candidate_is_experimental_only"]=true,
        ["unqualified_cas_and_unknown_results_refused"]=true,
        ["real_polynomial_degree_bound"]=7,
        ["real_derivative_maximum_power"]=8,
        ["original_domain_holes_remain_excluded"]=true,
        ["no_general_improper_convergence_claim"]=true,
        ["local_package_only"]=true,
        ["package_sha256_verified_by_independent_staged_helper"]=PackageHelper,
        ["observed_local_packages"]=p.Json.Array(products.ToArray()),
        ["cross_vertical_consumer_project"]=ConsumerProject,
        ["consumer_project_sha256"]=stagedHashes[ConsumerProject],
        ["cross_vertical_consumer_source"]=ConsumerSource,
        ["consumer_source_sha256"]=stagedHashes[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,
        ["consumer_config_sha256"]=stagedHashes[ConsumerConfig],
        ["cross_vertical_nuget_consumer_oracles"]=32,
        ["isolated_consumer_exact_output"]=ConsumerMarker,
        ["no_consumer_project_references"]=true,
        ["transfer_architecture_passed"]=true,
        ["gate_decision"]=Decision,
        ["gate_decision_sha256"]=stagedHashes[Decision],
        ["release_recipe"]=Document,
        ["release_recipe_sha256"]=stagedHashes[Document],
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["packages_publicly_published"]=false,
        ["external_aura_decision_mcdm_adoption_observed"]=false,
        ["m4_milestone_done"]=true,["m5_milestone_done"]=false,
        ["next_accepted_scope"]="M5-W01-C01-T1: independently qualify accepted notation/parsing scope; no M5 run started",
        ["nonclaims"]=p.Json.StringArray(
            "No general symbolic CAS, provider AST decoding, certified arbitrary integral, inequality, derivative, complex branch or irrational solver.",
            "No foreign Angouri/MathNET product runtime, Python interpreter, secure sandbox or hard preemptive in-process deadline.",
            "No public NuGet, stable 1.0, cross-RID/OS guarantee, AURA/Decision/MCDM adoption or dw.discovery skill.",
            "M5 public notation, interoperability and independent product gates remain to be qualified.")
    };
    p.Files.WriteComplete(M4Gate,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.64";
        var release=plan["releases"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]=="M4");
        if((string?)release["status"]!="in_progress"||
           (string?)release["gate"]!="G-M4")
            throw new InvalidOperationException("M4 release baseline/gate diverged.");
        release["status"]="done";
        release["evidence"]=p.Json.StringArray(W01Host,W01Bridge,W01Transform,
            W02Derivative,W02Solver,W02Calculus,WorkAggregate,M4Gate);
    });
    p.ProjectPlan.ConvergeNodeToDone("M4");
}
p.ProjectPlan.RequireNodeState("M2","done");
p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState("M4-W01","done");
p.ProjectPlan.RequireNodeState("M4-W02","done");
p.ProjectPlan.RequireNodeState("M5","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS065"||
       (string?)state["activeMilestone"]!=(target?null:"M4")||
       state["activeWorkPackage"] is not null||
       state["activePhase"] is not null||
       state["activeTask"] is not null||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS065 M4 gate active-cursor baseline/target diverged.");
    state["activeMilestone"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
var rechecked=Required(root,"dotnet","run","--file",GateValidator);
if(!HasExactLine(rechecked,GateMarker))
    throw new InvalidOperationException("Completed M4 scientific gate target was not revalidated.");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static bool HasExactLine(string output,string expected) =>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==expected);

static (long Bytes,string Sha) ReadProductReceipt(
    string output,string id,string version)
{
    var prefix="M4-GATE-PKG-SHA256:"+id+":"+version+":";
    var found=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(found.Length!=1)
        throw new InvalidOperationException("One local independent Math package receipt required for "+id);
    var parts=found[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes)||
       bytes<=0 || parts[1].Length!=64 ||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid local NuGet bytes/SHA-256 receipt for "+id);
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=index<0?output:output[index..];
    return part.Length>2500?part[..2500]:part;
}
static (int Code,string Output) Run(string root,string executable,params string[] args)
{
    using var process=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    process.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    process.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)process.StartInfo.ArgumentList.Add(arg);
    if(!process.Start())throw new InvalidOperationException("Cannot start "+executable);
    var stdout=process.StandardOutput.ReadToEndAsync();
    var stderr=process.StandardError.ReadToEndAsync();
    if(!process.WaitForExit(420000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" timed out during a bounded Math subprocess command.");
    }
    var output=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(output);
    return(process.ExitCode,output);
}
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
