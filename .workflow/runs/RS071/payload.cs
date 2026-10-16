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

const string Milestone="M5",Work="M5-W01",Phase="M5-W01-C03",Task="M5-W01-C03-T2";
const string Red="M5-W01-C03-T2-R",Green="M5-W01-C03-T2-G",Verify="M5-W01-C03-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-W01-C03-T1-contract-qualified.json";
const string RedProof="docs/planning/evidence/M5-W01-C03-T2-contract-red.json";
const string Qualified="docs/planning/evidence/M5-W01-C03-T2-boundaries-qualified.json";
const string QualifiedStatus="exact-annotated-mathml-xml-security-and-loss-boundaries-qualified";
const string Source="src/projects/dw.tools.math.codecs/MathMlBoundaryAudit.cs";
const string Product="src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj";
const string Test="tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.codecs.tests/M5W01C03T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.codecs.tests/M5W01C03T2Tests.cs";
const string Decision="docs/planning/decisions/m5-w01-c03-t2-mathml-boundaries.md";
const string Document="docs/distribution/m5-mathml-boundaries.md";
const string ConsumerProject="tests/consumers/m5-w01-c03-t2/m5-mathml-boundary-consumer.csproj";
const string ConsumerSource="tests/consumers/m5-w01-c03-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m5-w01-c03-t2/NuGet.Config";
const string PackageHelper=".workflow/runs/RS071/staged/checks/m5-w01-c03-t2-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.6";
const string RedMarker="M5-W01-C03-T2 RED: independent bounded MathML XML provenance and semantic-loss audit API is absent.";
const string ConsumerMarker="dw.tools.math/m5-mathml-boundary-consumer/1.0 qualified; probes=8";
const int IndependentGreen=42,AllTargeted=43;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS071",Milestone,Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS070",1,
    "feat: qualify exact annotated MathML and IR interchange",
    "0.1.59",2,2,0,false);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("exact-annotated-content-presentation-mathml-ir-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(41));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(42));
p.Require.JsonValueEquals(Prior,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/compiled_red_observed",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/mathml_rule",
    JsonValue.Create("dw-math-mathml3-annotated-ir/1"));
p.Require.JsonValueEquals(Prior,"/mathml_namespace",
    JsonValue.Create("http://www.w3.org/1998/Math/MathML"));
p.Require.JsonValueEquals(Prior,"/local_product_version",
    JsonValue.Create("1.0.0-preview.5"));
p.Require.JsonValueEquals(Prior,"/m5_w01_c03_t1_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w01_c03_phase_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_release_done",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.69"),JsonValue.Create("0.1.70"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.70":"0.1.69"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/33/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/34/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/35/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/35/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/35/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var slot in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/35/tasks/1/subtasks/{slot}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/36/status",JsonValue.Create("planned"));
p.Require.TextContains(Product,target?
    "<Version>1.0.0-preview.6</Version>":
    "<Version>1.0.0-preview.5</Version>");

p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01-C01","done");
p.ProjectPlan.RequireNodeState("M5-W01-C02","done");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01-C03-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W01-C04","not-ready");
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
        JsonValue.Create("compiled-mathml-boundary-audit-api-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/isolated_consumer_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c03_t2_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c03_phase_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_release_done",JsonValue.Create(false));
}

var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redReceipt=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redReceipt.AfterSha256 ??
    throw new InvalidOperationException("The compiled MathML boundary RED has no source byte hash.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redOutput=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W01C03T2RedTests.IndependentMathMlXmlLossAndSourceProvenanceAuditMustExist",
        "--logger","console;verbosity=normal");
    if(redOutput.Code==0 ||
       !redOutput.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "One genuinely compiled missing-boundary-API RED is required: "+
            Relevant(redOutput.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-W01-C03-T2-contract-red",
        ["status"]="compiled-mathml-boundary-audit-api-absent-red-observed",
        ["prior_mathml_interop_qualification"]=Prior,
        ["compiled_predecessor"]="dw.tools.math.codecs/1.0.0-preview.5",
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["missing_api"]="MathMlBoundaryAudit.Audit(string)",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true,
        ["nonclaim"]="A missing MathML audit API does not prove semantic fidelity or XML security."
    };
    p.Files.WriteComplete(RedProof,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

foreach(var file in new[]{Product,Source,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing MathML boundary source byte hash: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore",
    "--filter","FullyQualifiedName~M5W01C03T2",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 43",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 43",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "Require one recovered compiled RED and all 42 independent MathML boundary GREEN oracles.");
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
            "Qualified MathML IR/quantities regression did not pass: "+regressionProject);
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
var feed=Path.Combine(root,"build","artifacts","m5-w01-c03-t2-local-feed");
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
        "The independent MathML boundary consumer requires only one pinned codec NuGet package.");
var configured=XDocument.Load(Path.Combine(root,ConsumerConfig))
    .Descendants().Single(x=>x.Name.LocalName=="packageSources");
if(!configured.Elements().Any(x=>x.Name.LocalName=="clear") ||
   configured.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException(
        "The MathML boundary package-only consumer must clear external package feeds.");
var isolated=Path.Combine(root,"build","artifacts","m5-w01-c03-t2-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException(
        "Eight independent MathML boundary NuGet-only consumer oracles not observed.");

if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W01-C03-T2-boundaries-qualified",
        ["status"]=QualifiedStatus,
        ["prior_exact_mathml_contract"]=Prior,
        ["compiled_missing_api_red"]=RedProof,
        ["question"]="Can independently audited exact annotated MathML 3 bind input provenance to source Math IR while refusing adversarial XML, missing annotations and changed presentation/content without falsely claiming a universal inverse?",
        ["observation"]="One compiled missing-public-API RED recovered, 42 independent GREEN oracles and eight isolated NuGet-only clients qualify structural Math IR replay, source-byte SHA-256, typed DTD/entity, namespace, presentation, tampering, canonicalization and resource-limit refusals.",
        ["decision"]="Admit only the existing qualified exact annotated MathML 3 subprofile through a separate loss/provenance audit. Keep bounded input source fingerprint separate from mathematical structural evidence. Refuse all uncertified markup without IR or a proof rule.",
        ["source_file"]=Source,["source_sha256"]=hashes[Source],
        ["product_project"]=Product,["product_project_sha256"]=hashes[Product],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=hashes[GreenTests],
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["decision_document"]=Decision,["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Document,["distribution_document_sha256"]=hashes[Document],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["compiled_red_observed"]=true,
        ["isolated_consumer_probes_passed"]=8,
        ["qualified_audit_rule"]="dw-math-mathml3-boundary-provenance/1",
        ["qualified_mathml_rule"]="dw-math-mathml3-annotated-ir/1",
        ["content_mathml_and_presentation_both_checked"]=true,
        ["typed_exact_fraction_symbol_and_original_equation_retained"]=true,
        ["source_sha256_binds_bounded_original_utf8"]=true,
        ["over_budget_xml_does_not_allocate_source_hash"]=true,
        ["source_fingerprint_is_not_a_mathematical_proof"]=true,
        ["math_ir_canonical_json_replay_qualified"]=true,
        ["semantic_structural_sha256_retained"]=true,
        ["presentation_inverse_qualified"]=false,
        ["missing_annotation_is_presentation_ambiguous"]=true,
        ["xml_dtd_or_external_entity_processing_admitted"]=false,
        ["unqualified_xml_namespace_admitted"]=false,
        ["tampered_presentation_content_annotation_refused"]=true,
        ["unreduced_annotation_admitted"]=false,
        ["maximum_xml_characters"]=4096,
        ["maximum_xml_nodes"]=32,
        ["maximum_xml_depth"]=12,
        ["refused_result_has_no_ir_or_semantic_proof"]=true,
        ["no_external_cas_or_agent_dependency"]=true,
        ["locked_solution_build_passed"]=true,
        ["focused_codecs_and_ir_quantities_regressions_passed"]=true,
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
        ["m5_w01_c03_t2_done"]=true,
        ["m5_w01_c03_phase_done"]=true,
        ["m5_w01_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M5-W01-C04-T1: independently qualify typed Markdown/Office mathematical interchange without inventing semantics from typography",
        ["nonclaims"]=p.Json.StringArray(
            "No general MathML/HTML/XML importer, arbitrary Content or Presentation interpretation, DTD processing, external entity loading or general XML sanitizer.",
            "No algebraic equivalence, presentation inverse, arbitrary TeX/Office/Markdown parsing, complex branches, general CAS or silent precision loss.",
            "No public NuGet, stable 1.0, AURA/Decision/MCDM external adoption or dw.discovery agent skill.",
            "Only MathML phase C03 closes; M5-W01, M5 and C04 onward remain pending independent qualification.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.70";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="in_progress" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException(
                "The accepted M5-W01-C03 T2 baseline changed.");
        chunk["status"]="done";
        chunk["refinement"]="RS070 T1 qualifies the MathML 3 canonical Content/Presentation exact annotated subset for rationals, real symbols, ordered sums and an original equality restriction. RS071 T2 adds an independent bounded input fingerprint and semantic-loss/refusal audit over the unchanged qualified importer. It independently replays exact Math IR JSON, canonical text and annotated Content/Presentation, retains precise DTD/entity/namespace/tampering/canonical/resource refusals, and never treats a refused input fingerprint as a semantic proof. One compiled absent-API RED, 42 GREEN tests, eight isolated NuGet oracles and three independently inspected local packages qualify the C03 boundary. W01 and M5 remain in-progress; C04 Markdown/Office has not begun.";
        chunk["readiness"]="C03 completed with T1 annotated MathML and T2 source-provenance, XML refusal and structural-loss evidence; C04 not-ready until independent Markdown/Office scope and tests";
        var existingFiles=chunk["files"]!.AsArray()
            .Select(x=>x!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(existingFiles.Concat(new[]{
            Product,Source,RedTest,GreenTests,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M5W01C03T2",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        var previousEvidence=chunk["evidence"]!.AsArray()
            .Select(x=>x!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(previousEvidence.Concat(new[]{
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
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01-C01","done");
p.ProjectPlan.RequireNodeState("M5-W01-C02","done");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M5-W01-C03-T1","done");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M5-W01-C04","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS071" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException(
            "RS071 MathML C03 close cursor diverged from its baseline/target.");
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
    var prefix="M5-W01-C03-T2-PKG-SHA256:"+id+":"+version+":";
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
