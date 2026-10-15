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

const string Work="M5-W01",Phase="M5-W01-C01",Task="M5-W01-C01-T2";
const string Red="M5-W01-C01-T2-R",Green="M5-W01-C01-T2-G",Verify="M5-W01-C01-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-W01-C01-T1-contract-qualified.json";
const string RedProof="docs/planning/evidence/M5-W01-C01-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M5-W01-C01-T2-boundaries-qualified.json";
const string Source="src/projects/dw.tools.math.codecs/CanonicalDisplayBoundaryAudit.cs";
const string Product="src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj";
const string Test="tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.codecs.tests/M5W01C01T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.codecs.tests/M5W01C01T2Tests.cs";
const string Decision="docs/planning/decisions/m5-w01-c01-t2-display-boundaries.md";
const string Document="docs/distribution/m5-display-boundaries.md";
const string ConsumerProject="tests/consumers/m5-w01-c01-t2/m5-display-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m5-w01-c01-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m5-w01-c01-t2/NuGet.Config";
const string PackageHelper=".workflow/runs/RS067/staged/checks/m5-w01-c01-t2-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.2";
const string RedMarker="M5-W01-C01-T2 RED: canonical display-boundary text and IR audit APIs are absent.";
const string ConsumerMarker="dw.tools.math/m5-display-boundary-consumer/1.0 qualified; probes=8";
const int IndependentGreen=51,AllTargeted=52;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS067","M5",Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS066",1,
    "feat: qualify canonical Math text and LaTeX IR codecs",
    "0.1.59",2,2,0,false);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("exact-canonical-text-ir-latex-subset-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(50));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(51));
p.Require.JsonValueEquals(Prior,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/compiled_red_observed",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/canonical_grammar_id",
    JsonValue.Create("dw-math-canonical-text/1"));
p.Require.JsonValueEquals(Prior,"/local_product_version",
    JsonValue.Create("1.0.0-preview.1"));
p.Require.JsonValueEquals(Prior,"/m5_w01_c01_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w01_c01_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_release_done",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.65"),JsonValue.Create("0.1.66"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.66":"0.1.65"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/32/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/33/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/33/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/33/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/33/tasks/1/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/34/status",JsonValue.Create("planned"));
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState("M5","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01-C01-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTests,Decision,Document,
        ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified})
        p.Require.FileMissing(file);
    p.Require.TextContains(Product,"<Version>1.0.0-preview.1</Version>");
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-canonical-display-boundary-apis-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("exact-safe-canonical-display-original-restrictions-and-ambiguity-qualified"));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/isolated_consumer_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c01_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_release_done",JsonValue.Create(false));
    p.Require.TextContains(Product,"<Version>1.0.0-preview.2</Version>");
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redInstalled.AfterSha256 ??
    throw new InvalidOperationException("The staged codec display-boundary RED test has no byte receipt.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W01C01T2RedTests.IndependentCanonicalDisplayBoundaryAuditMustBePublic",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "One compiled missing-public-API display boundary RED is mandatory: "+
            Relevant(red.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-W01-C01-T2-contract-red",
        ["status"]="compiled-canonical-display-boundary-apis-absent-red-observed",
        ["prior_canonical_text_contract"]=Prior,
        ["compiled_predecessor"]="dw.tools.math.codecs/1.0.0-preview.1",
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["missing_api"]="CanonicalDisplayBoundaryAudit.AuditText(string);AuditIr(IrNode)",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_not_a_red_substitute"]=true,
        ["nonclaim"]="An absent API is not itself an admission of TeX safety or structural fidelity."
    };
    p.Files.WriteComplete(RedProof,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Product,Source,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var receipt=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=receipt.AfterSha256 ??
        throw new InvalidOperationException("Missing staged Math codec proof hash for "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore",
    "--filter","FullyQualifiedName~M5W01C01T2",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 52",StringComparison.OrdinalIgnoreCase)||
   !targeted.Contains("Passed: 52",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "Require the compiled RED recovered plus 51 independent Math display-boundary GREEN tests.");
Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var project in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var observed=Required(root,"dotnet","test",project,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!observed.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("Required T2 IR/quantities regression not observed: "+project);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var packages=new (string Id,string Version,string Project,string[] DependsOn)[]
{
    ("dw.quantities","0.2.0-preview.1",
        "src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.codecs",Version,Product,["dw.tools.math.ir"])
};
var feed=Path.Combine(root,"build","artifacts","m5-w01-c01-t2-local-feed");
var archives=new List<string>();
foreach(var package in packages)
{
    Required(root,"dotnet","pack",package.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,package.Id+"."+package.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException(
            "Required generated local codec product archive missing: "+package.Id);
    archives.Add(archive);
}
var inspectorArguments=new List<string>{"run","--file",PackageHelper,"--"};
inspectorArguments.AddRange(archives);
var observed=Required(root,"dotnet",inspectorArguments.ToArray());
var products=new List<JsonNode>();
foreach(var package in packages)
{
    var receipt=ReadProductReceipt(observed,package.Id,package.Version);
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
var references=consumerXml.Descendants()
    .Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(references.Length!=1 ||
   (string?)references[0].Attribute("Include")!="dw.tools.math.codecs" ||
   (string?)references[0].Attribute("Version")!="["+Version+"]" ||
   consumerXml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException(
        "The independent canonical consumer requires only one pinned codec NuGet package.");
var configured=XDocument.Load(Path.Combine(root,ConsumerConfig))
    .Descendants().Single(x=>x.Name.LocalName=="packageSources");
if(!configured.Elements().Any(x=>x.Name.LocalName=="clear") ||
   configured.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException(
        "The display-boundary package-only consumer must clear external package feeds.");
var isolated=Path.Combine(root,"build","artifacts","m5-w01-c01-t2-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException(
        "Eight independent display-boundary NuGet-only consumer oracles not observed.");

if(!target)
{
    var evidence=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W01-C01-T2-boundaries-qualified",
        ["status"]="exact-safe-canonical-display-original-restrictions-and-ambiguity-qualified",
        ["prior_canonical_text_contract"]=Prior,
        ["compiled_missing_api_red"]=RedProof,
        ["question"]="Can the accepted Math T1 canonical text/IR/LaTeX subset be independently audited for bounded renderer commands, original-domain assumption binding, exact contradictions and noninjective display without admitting arbitrary TeX import or execution?",
        ["observation"]="One genuine compiled missing-API RED recovered, 51 independent GREEN tests and eight isolated NuGet-only oracles qualify passive display command boundaries, exact structural IR roundtrips, unbound/contradictory assumption refusals and a known square-versus-power display collision.",
        ["decision"]="Admit an independent Math display-boundary audit over the T1 grammar and renderer; allow only named passive TeX commands, retain original IR and structural SHA-256, classify a known collision without ever claiming inverse LaTeX parsing.",
        ["source_file"]=Source,["source_sha256"]=hashes[Source],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Document,
        ["distribution_document_sha256"]=hashes[Document],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["compiled_red_observed"]=true,["isolated_consumer_probes_passed"]=8,
        ["canonical_text_grammar"]="dw-math-canonical-text/1",
        ["safe_display_audit_rule"]="canonical-safe-math-display-domain-audit/v1",
        ["only_renderer_generated_latex_is_scanned"]=true,
        ["latex_ascii_and_braces_bounded"]=true,
        ["allowed_tex_commands"]=p.Json.StringArray(
            "frac","left","right","cdot","sqrt","mathrm","ne","ge"),
        ["latex_inverse_qualified"]=false,
        ["square_and_power_two_have_identical_latex"]=true,
        ["known_display_collision_reported"]=true,
        ["semantic_structural_hash_retained"]=true,
        ["canonical_math_ir_replay_verified"]=true,
        ["unbound_original_assumptions_rejected"]=true,
        ["contradictory_equalities_rejected"]=true,
        ["refused_boundary_has_no_latex_or_mathematical_proof"]=true,
        ["arbitrary_tex_import_or_execution_admitted"]=false,
        ["no_external_cas_or_agent_dependency"]=true,
        ["locked_solution_build_passed"]=true,
        ["codec_ir_quantities_regressions_passed"]=true,
        ["local_product_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(products.ToArray()),
        ["product_package_manifest_sha_helper"]=PackageHelper,
        ["consumer_project"]=ConsumerProject,
        ["consumer_project_sha256"]=hashes[ConsumerProject],
        ["consumer_source"]=ConsumerSource,
        ["consumer_source_sha256"]=hashes[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,
        ["consumer_config_sha256"]=hashes[ConsumerConfig],
        ["consumer_marker"]=ConsumerMarker,
        ["packages_publicly_published"]=false,
        ["m5_w01_c01_t2_done"]=true,
        ["m5_w01_c01_phase_done"]=true,
        ["m5_w01_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M5-W01-C02-T1: separately qualify bounded LaTeX import and explicit ambiguity controls without claiming T1 passive display is reversible",
        ["nonclaims"]=p.Json.StringArray(
            "No arbitrary LaTeX input, macro execution, general TeX sanitizer, safe rendering of arbitrary caller strings or reversible TeX grammar.",
            "No general algebraic equivalence, symbolic CAS, complex principal branches, arbitrary text/Unicode aliases, MathML or Office interop.",
            "No public NuGet or stable 1.0, AURA/Decision/MCDM external consumption or neutral dw.discovery skill.",
            "Only C01 closes; M5-W01, M5 and C02 onward remain to be independently qualified.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.66";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="in_progress" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("M5-W01-C01 T2 baseline diverged.");
        chunk["status"]="done";
        chunk["refinement"]="RS066 T1 qualifies the closed canonical exact ASCII text-to-IR and passive LaTeX contract. RS067 T2 independently audits renderer-generated command braces/allowlist, exact canonical structural IR roundtrips, original-domain restriction binding and contradictions. Distinct IR square and power-two shapes produce the same LaTeX; the safe display is explicitly flagged noninjective, never marked reversible. A genuine compiled absent-API RED, 51 GREEN tests, eight isolated NuGet-only consumer oracles, three locally inspected packages and relevant codec/IR/quantities tests qualify C01. W01 and M5 remain in-progress; C02 bounded LaTeX import is not started.";
        chunk["readiness"]="C01 exact canonical text and display audit qualified by RS066 and RS067; C02 remains not-ready pending separate LaTeX import and ambiguity grammar acceptance";
        var priorFiles=chunk["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(priorFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M5W01C01T2",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        var priorEvidence=chunk["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(priorEvidence.Concat(new[]{
            RedProof,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedProof:Qualified);
        }
    });
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Red,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Red);
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach(var child in new[]{Green,Verify})
    {
        p.ProjectPlan.TransitionNode(child,"not-ready","ready");
        p.ProjectPlan.TransitionNode(child,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(child);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState("M5","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M5-W01-C01-T1","done");
foreach(var child in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,"done");
p.ProjectPlan.RequireNodeState("M5-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS067" ||
       (string?)state["activeMilestone"]!="M5" ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS067 C01 phase-close cursor baseline/target diverged.");
    state["activePhase"]=null;
});
if(!target)
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static bool HasExactLine(string output,string expected) =>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==expected);

static (long Bytes,string Sha) ReadProductReceipt(
    string output,string id,string version)
{
    var prefix="M5-W01-C01-T2-PKG-SHA256:"+id+":"+version+":";
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
