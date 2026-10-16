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

const string Milestone="M5",Work="M5-W01",Phase="M5-W01-C04",Task="M5-W01-C04-T1";
const string Red="M5-W01-C04-T1-R",Green="M5-W01-C04-T1-G",Verify="M5-W01-C04-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-W01-C03-T2-boundaries-qualified.json";
const string RedProof="docs/planning/evidence/M5-W01-C04-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M5-W01-C04-T1-contract-qualified.json";
const string QualifiedStatus="exact-markdown-unicode-and-original-ir-bound-office-omml-qualified";
const string MarkdownSource="src/projects/dw.tools.math.codecs/MarkdownUnicodeMathCodec.cs";
const string OmmlSource="src/projects/dw.tools.math.codecs/ExactOfficeOmmlCodec.cs";
const string Product="src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj";
const string Test="tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.codecs.tests/M5W01C04T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.codecs.tests/M5W01C04T1Tests.cs";
const string Decision="docs/planning/decisions/m5-w01-c04-t1-markdown-omml.md";
const string Document="docs/distribution/m5-markdown-omml.md";
const string ConsumerProject="tests/consumers/m5-w01-c04-t1/m5-markdown-omml-consumer.csproj";
const string ConsumerSource="tests/consumers/m5-w01-c04-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m5-w01-c04-t1/NuGet.Config";
const string PackageHelper=".workflow/runs/RS072/staged/checks/m5-w01-c04-t1-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.7";
const string RedMarker="M5-W01-C04-T1 RED: exact Office OMML public codec is absent.";
const string ConsumerMarker="dw.tools.math/m5-markdown-omml-consumer/1.0 qualified; probes=8";
const int IndependentGreen=24,AllTargeted=25;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS072",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS071",1,
    "feat: qualify exact MathML XML boundaries and close C03",
    "0.1.59",0,0,0,true);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("exact-annotated-mathml-xml-security-and-loss-boundaries-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(42));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(43));
p.Require.JsonValueEquals(Prior,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/compiled_red_observed",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("1.0.0-preview.6"));
p.Require.JsonValueEquals(Prior,"/m5_w01_c03_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_release_done",JsonValue.Create(false));

p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.70"),JsonValue.Create("0.1.71"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.71":"0.1.70"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",JsonValue.Create("planned"));
foreach(var index in new[]{33,34,35})
    p.Require.JsonValueEquals(Backlog,$"/chunks/{index}/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/36/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/36/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var slot in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/36/tasks/0/subtasks/{slot}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/36/tasks/1/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/37/status",JsonValue.Create("planned"));
p.Require.TextContains(Product,target?
    "<Version>1.0.0-preview.7</Version>":
    "<Version>1.0.0-preview.6</Version>");

p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
foreach(var node in new[]{"M5-W01-C01","M5-W01-C02","M5-W01-C03"})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W01-C04-T2","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");

if(!target)
{
    foreach(var file in new[]{MarkdownSource,OmmlSource,RedTest,GreenTests,
        Decision,Document,ConsumerProject,ConsumerSource,ConsumerConfig,
        RedProof,Qualified})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-office-omml-public-api-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",JsonValue.Create(QualifiedStatus));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/isolated_consumer_probes_passed",
        JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c04_t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c04_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_work_package_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_release_done",JsonValue.Create(false));
}

var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redReceipt=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redReceipt.AfterSha256 ??
    throw new InvalidOperationException("The compiled Markdown/OMML RED source hash is absent.");
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redOutput=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W01C04T1RedTests.StandaloneExactOfficeAndMarkdownUnicodePublicContractsMustExist",
        "--logger","console;verbosity=normal");
    if(redOutput.Code==0 ||
       !redOutput.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "A real compiled absent-public-API RED is required: "+
            Relevant(redOutput.Output));
    var redProof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-W01-C04-T1-contract-red",
        ["status"]="compiled-office-omml-public-api-absent-red-observed",
        ["prior_exact_mathml_boundary"]=Prior,
        ["compiled_predecessor"]="dw.tools.math.codecs/1.0.0-preview.6",
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["missing_api"]="ExactOfficeOmmlCodec and MarkdownUnicodeMathCodec public contracts",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true,
        ["nonclaim"]="A missing public API is not evidence of OMML semantic losslessness."
    };
    p.Files.WriteComplete(RedProof,
        redProof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

foreach(var file in new[]{Product,MarkdownSource,OmmlSource,GreenTests,
    Decision,Document,ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing exact Markdown/OMML source byte hash: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore",
    "--filter","FullyQualifiedName~M5W01C04T1",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 25",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 25",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "Recover compiled RED and pass all 24 independent Markdown/Office OMML GREEN tests.");
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
            "Exact IR/quantities regression did not pass: "+regressionProject);
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
var feed=Path.Combine(root,"build","artifacts","m5-w01-c04-t1-local-feed");
var archives=new List<string>();
foreach(var package in packages)
{
    Required(root,"dotnet","pack",package.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,package.Id+"."+package.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException(
            "Required freshly built local NuGet archive missing: "+package.Id);
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
        "The standalone Office/Markdown consumer must use one pinned NuGet codecs package.");
var configured=XDocument.Load(Path.Combine(root,ConsumerConfig))
    .Descendants().Single(x=>x.Name.LocalName=="packageSources");
if(!configured.Elements().Any(x=>x.Name.LocalName=="clear") ||
   configured.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException(
        "The independent NuGet consumer must clear external feeds.");
var isolated=Path.Combine(root,"build","artifacts","m5-w01-c04-t1-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException(
        "Eight independent pinned NuGet Office/Markdown probes not observed.");

if(!target)
{
    var proof=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W01-C04-T1-contract-qualified",
        ["status"]=QualifiedStatus,
        ["prior_qualified_mathml_profile"]=Prior,
        ["compiled_missing_public_api_red"]=RedProof,
        ["question"]="Can a small standalone Office OMML fraction and original equation round-trip without semantic loss while Unicode/Markdown distinguish inline and block math without source-free semantic inference?",
        ["observation"]="One compiled absent-API RED recovered; 24 independent GREEN oracles, exact source-bound OMML canonical replay, two Markdown placements and eight pinned NuGet-only consumer oracles verified.",
        ["decision"]="Admit only existing exact MathML-qualified IR through separately typed Unicode/Markdown display and an original-IR-bound, canonically verified standalone Office OMML subset; refuse altered/unqualified markup.",
        ["markdown_source"]=MarkdownSource,
        ["markdown_source_sha256"]=hashes[MarkdownSource],
        ["omml_source"]=OmmlSource,
        ["omml_source_sha256"]=hashes[OmmlSource],
        ["product_project"]=Product,
        ["product_project_sha256"]=hashes[Product],
        ["red_test"]=RedTest,
        ["red_test_sha256"]=hashes[RedTest],
        ["green_tests"]=GreenTests,
        ["green_tests_sha256"]=hashes[GreenTests],
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Document,
        ["distribution_document_sha256"]=hashes[Document],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["compiled_red_observed"]=true,
        ["isolated_consumer_probes_passed"]=8,
        ["markdown_rule"]="dw-math-markdown-unicode-exact-display/1",
        ["omml_rule"]="dw-math-office-omml-exact-original-ir/1",
        ["omml_namespace"]="http://schemas.openxmlformats.org/officeDocument/2006/math",
        ["omml_import_requires_original_typed_ir"]=true,
        ["exact_fraction_and_equation_roundtrip"]=true,
        ["semantic_structural_sha256_retained"]=true,
        ["unicode_display_inverse_qualified"]=false,
        ["markdown_inline_and_block_distinct"]=true,
        ["office_application_or_sdk_required"]=false,
        ["whole_office_documents_qualified"]=false,
        ["fonts_or_positioning_qualified"]=false,
        ["refused_result_has_no_ir_or_semantic_hash"]=true,
        ["locked_solution_build_passed"]=true,
        ["focused_codecs_ir_quantities_regressions_passed"]=true,
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
        ["m5_w01_c04_t1_done"]=true,
        ["m5_w01_c04_phase_done"]=false,
        ["m5_w01_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M5-W01-C04-T2: independent unsupported Office/Markdown input, positioning, documents, loss and ambiguity boundaries",
        ["nonclaims"]=p.Json.StringArray(
            "No general OMML, Word document conversion, Office installation, Open XML SDK, fonts or positioning support.",
            "No Unicode/Markdown inverse parsing, typography-derived identities, unqualified mathematical equivalence, arbitrary TeX/Office interpreter or general CAS.",
            "No public NuGet, stable 1.0, sibling agent or dw.discovery adoption.",
            "T1 alone completes; C04-T2, C04, M5-W01 and M5 remain open.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.71";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="planned" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException(
                "The accepted Markdown/Office C04 T1 baseline changed.");
        chunk["status"]="in_progress";
        chunk["refinement"]="RS072 T1 admits Unicode/Markdown math display with explicit inline/block delimiters and a separate, bounded canonical Office OMML subset retaining caller-supplied typed original IR for a rational fraction and a simple original equality. Independent compiled RED, 24 GREEN tests, eight NuGet-only consumer oracles and three inspected local packages qualify only this exact contract. OMML typography cannot authorize source-free semantic reconstruction. C04-T2 owns wider unsupported inputs, document conversion and font/positioning limitations; W01 and M5 remain in progress.";
        chunk["readiness"]="C04 T1 completed with exact original-IR-bound OMML and explicit Markdown math placement; T2 still pending independent loss and Office document/font boundaries";
        chunk["files"]=p.Json.StringArray(Product,MarkdownSource,OmmlSource,
            RedTest,GreenTests,Decision,Document,ConsumerProject,ConsumerSource,
            ConsumerConfig,RedProof,Qualified);
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M5W01C04T1",
            "dotnet test "+Test+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        chunk["evidence"]=p.Json.StringArray(RedProof,Qualified);
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedProof:Qualified);
        }
    });
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
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
}
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
foreach(var node in new[]{"M5-W01-C01","M5-W01-C02","M5-W01-C03"})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
foreach(var node in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,"done");
p.ProjectPlan.RequireNodeState("M5-W01-C04-T2","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS072" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?Phase:null) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException(
            "RS072 exact Markdown/OMML phase start cursor diverged from baseline/target.");
    state["activePhase"]=Phase;
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
    var prefix="M5-W01-C04-T1-PKG-SHA256:"+id+":"+version+":";
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
