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

const string Milestone="M5",Work="M5-W01",Phase="M5-W01-C02",Task="M5-W01-C02-T2";
const string Red="M5-W01-C02-T2-R",Green="M5-W01-C02-T2-G",Verify="M5-W01-C02-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-W01-C02-T1-contract-qualified.json";
const string RedProof="docs/planning/evidence/M5-W01-C02-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M5-W01-C02-T2-boundaries-qualified.json";
const string QualifiedStatus="bounded-latex-import-semantic-loss-and-adversarial-boundaries-qualified";
const string Source="src/projects/dw.tools.math.codecs/LatexImportBoundaryAudit.cs";
const string Product="src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj";
const string Test="tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.codecs.tests/M5W01C02T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.codecs.tests/M5W01C02T2Tests.cs";
const string Decision="docs/planning/decisions/m5-w01-c02-t2-latex-loss.md";
const string Document="docs/distribution/m5-latex-loss.md";
const string ConsumerProject="tests/consumers/m5-w01-c02-t2/m5-latex-loss-consumer.csproj";
const string ConsumerSource="tests/consumers/m5-w01-c02-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m5-w01-c02-t2/NuGet.Config";
const string PackageHelper=".workflow/runs/RS069/staged/checks/m5-w01-c02-t2-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.4";
const string RedMarker="M5-W01-C02-T2 RED: independent bounded LaTeX semantic-loss and provenance audit API is absent.";
const string ConsumerMarker="dw.tools.math/m5-latex-loss-consumer/1.0 qualified; probes=8";
const int IndependentGreen=46,AllTargeted=47;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS069",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS068",1,
    "feat: qualify bounded LaTeX import with explicit ambiguity policy",
    "0.1.59",2,2,0,false);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("bounded-latex-exact-subset-and-juxtaposition-ambiguity-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(50));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(51));
p.Require.JsonValueEquals(Prior,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/compiled_red_observed",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/default_juxtaposition_is_ambiguous",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/caller_convention_is_recorded",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("1.0.0-preview.3"));
p.Require.JsonValueEquals(Prior,"/m5_w01_c02_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w01_c02_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_release_done",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.67"),JsonValue.Create("0.1.68"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.68":"0.1.67"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/33/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/34/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/34/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/34/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var slot in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/34/tasks/1/subtasks/{slot}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/35/status",JsonValue.Create("planned"));
p.Require.TextContains(Product,target?
    "<Version>1.0.0-preview.4</Version>":
    "<Version>1.0.0-preview.3</Version>");

p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01-C02-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W01-C03","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");

if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTests,Decision,Document,
        ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-bounded-latex-loss-audit-api-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/isolated_consumer_probes_passed",
        JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c02_t2_done",
        JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c02_phase_done",
        JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_work_package_done",
        JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_release_done",
        JsonValue.Create(false));
}

var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var stagedRed=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=stagedRed.AfterSha256 ??
    throw new InvalidOperationException("Missing staged compiled RED byte hash.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redOutput=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W01C02T2RedTests.IndependentLatexLossAndProvenanceAuditApiMustExist",
        "--logger","console;verbosity=normal");
    if(redOutput.Code==0 ||
       !redOutput.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "A genuine compiled absent-boundary-API RED is required: "+
            Relevant(redOutput.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W01-C02-T2-contract-red",
        ["status"]="compiled-bounded-latex-loss-audit-api-absent-red-observed",
        ["prior_bounded_latex_import"]=Prior,
        ["compiled_predecessor"]="dw.tools.math.codecs/1.0.0-preview.3",
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["missing_api"]="LatexImportBoundaryAudit.Audit(string,LatexJuxtapositionConvention)",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true,
        ["nonclaim"]="An absent public audit API is not itself a losslessness or hostile-input safety proof."
    };
    p.Files.WriteComplete(RedProof,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Product,Source,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var stagedFile=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=stagedFile.AfterSha256 ??
        throw new InvalidOperationException("Missing Math LaTeX source identity: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore",
    "--filter","FullyQualifiedName~M5W01C02T2",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 47",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 47",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "Require one recovered compiled RED and 46 independent exact LaTeX loss GREEN tests.");
Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var regressionProject in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var regressionOutput=Required(root,"dotnet","test",regressionProject,
        "-c","Release","--no-build","--no-restore",
        "--logger","console;verbosity=minimal");
    if(!regressionOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(
            "Exact IR and quantities regression not observed: "+regressionProject);
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
var feed=Path.Combine(root,"build","artifacts","m5-w01-c02-t2-local-feed");
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
var packageInspectionOutput=Required(root,"dotnet",inspectorArguments.ToArray());
var products=new List<JsonNode>();
foreach(var package in packages)
{
    var receipt=ReadProductReceipt(packageInspectionOutput,package.Id,package.Version);
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
        "The LaTeX loss audit package-only consumer must clear external package feeds.");
var isolated=Path.Combine(root,"build","artifacts","m5-w01-c02-t2-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException(
        "Eight independent LaTeX loss audit NuGet-only consumer oracles not observed.");

if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W01-C02-T2-boundaries-qualified",
        ["status"]=QualifiedStatus,
        ["prior_bounded_latex_qualification"]=Prior,
        ["compiled_missing_api_red"]=RedProof,
        ["question"]="Can the exact bounded LaTeX importer preserve source-byte provenance, structural Math IR identity and an explicit ambiguity/loss classification while refusing hostile TeX and avoiding unqualified inverse-display claims?",
        ["observation"]="One genuine compiled missing-public-API RED recovered, 46 independent GREEN oracles and eight package-only consumer probes distinguish exact structural admission, source SHA-256, passive notation change, caller-dependent multiplication, ambiguous meaning, noncanonical fractions, invalid syntax, unsupported commands and resource refusal.",
        ["decision"]="Admit a separate bounded LaTeX loss/provenance audit wrapping the qualified T1 importer. Only exact admitted results expose IR, canonical text, structural semantic SHA-256 and an audit rule. Fingerprints of refused bounded input are never proofs.",
        ["source_file"]=Source,["source_sha256"]=hashes[Source],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Document,
        ["distribution_document_sha256"]=hashes[Document],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["compiled_red_observed"]=true,
        ["isolated_consumer_probes_passed"]=8,
        ["qualified_audit_rule"]="math-latex-import-boundary-loss/v1",
        ["qualified_import_rule"]="math-bounded-latex-exact-subset/v1",
        ["bounded_source_sha256_observed"]=true,
        ["oversized_source_does_not_allocate_hash"]=true,
        ["semantic_structural_sha256_retained"]=true,
        ["canonical_ir_json_roundtrip_observed"]=true,
        ["passive_notation_change_classified"]=true,
        ["caller_convention_requirement_explicit"]=true,
        ["unqualified_juxtaposition_refused"]=true,
        ["square_power_two_collision_refused"]=true,
        ["arbitrary_tex_macro_execution_admitted"]=false,
        ["noncanonical_fraction_normalization_admitted"]=false,
        ["refused_result_has_no_mathematical_ir_or_rule"]=true,
        ["latex_inverse_qualified"]=false,
        ["maximum_latex_characters"]=512,
        ["no_external_cas_or_agent_dependency"]=true,
        ["locked_solution_build_passed"]=true,
        ["focused_codec_and_ir_quantities_regressions_passed"]=true,
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
        ["m5_w01_c02_t2_done"]=true,
        ["m5_w01_c02_phase_done"]=true,
        ["m5_w01_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M5-W01-C03-T1: independently qualify semantic MathML/Markdown mathematical interchange, retaining typed original IR rather than inferring meaning from typography",
        ["nonclaims"]=p.Json.StringArray(
            "No arbitrary TeX parser/executor, macro expansion, universal input sanitizer, arbitrary functions or general symbolic CAS.",
            "No algebraic equivalence, binary64 inference, lossless TeX typography, arbitrary source LaTeX inverse or unchecked assumptions.",
            "No public NuGet or stable 1.0, AURA/Decision/MCDM consumer adoption or dw.discovery skill.",
            "Only C02 closes; M5-W01, M5 and C03 onward remain to be independently qualified.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.68";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="in_progress" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("The accepted M5-W01-C02 T2 baseline changed.");
        chunk["status"]="done";
        chunk["refinement"]="RS068 T1 qualifies the closed exact LaTeX importer, convention-bound juxtaposition, refusal statuses and source Math IR. RS069 T2 independently binds the bounded source SHA-256, distinguishes passive notation change from convention-dependent meaning, preserves typed canonical IR identity, and retains precise macro/noncanonical/invalid/ambiguous/budget refusals without proof on failure. One compiled absent-API RED, 46 independent GREEN tests, eight NuGet-only consumer probes and three locally inspected packages qualify the exact import boundary. M5-W01 and M5 remain in-progress; C03 MathML/Markdown mathematical interchange has not started.";
        chunk["readiness"]="C02 closed by RS068 exact subset and RS069 loss-provenance audit; M5-W01-C03 remains not-ready pending independent MathML/Markdown qualification";
        var previousFiles=chunk["files"]!.AsArray()
            .Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(previousFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M5W01C02T2",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        var oldEvidence=chunk["evidence"]!.AsArray()
            .Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(
            oldEvidence.Concat(new[]{RedProof,Qualified})
                .Distinct(StringComparer.Ordinal).ToArray());
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
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M5-W01-C02-T1","done");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M5-W01-C03","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS069" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException(
            "RS069 C02 phase-close cursor diverged from the exact baseline/target.");
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
    var prefix="M5-W01-C02-T2-PKG-SHA256:"+id+":"+version+":";
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
