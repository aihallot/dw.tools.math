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

const string Milestone="M5",Work="M5-W01",Phase="M5-W01-C02",Task="M5-W01-C02-T1";
const string Red="M5-W01-C02-T1-R",Green="M5-W01-C02-T1-G",Verify="M5-W01-C02-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M5-W01-C01-T2-boundaries-qualified.json";
const string RedProof="docs/planning/evidence/M5-W01-C02-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M5-W01-C02-T1-contract-qualified.json";
const string Source="src/projects/dw.tools.math.codecs/BoundedLatexImporter.cs";
const string Product="src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj";
const string Test="tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.codecs.tests/M5W01C02T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.codecs.tests/M5W01C02T1Tests.cs";
const string Decision="docs/planning/decisions/m5-w01-c02-t1-bounded-latex-import.md";
const string Document="docs/distribution/m5-bounded-latex-import.md";
const string ConsumerProject="tests/consumers/m5-w01-c02-t1/m5-latex-import-consumer.csproj";
const string ConsumerSource="tests/consumers/m5-w01-c02-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m5-w01-c02-t1/NuGet.Config";
const string PackageHelper=".workflow/runs/RS068/staged/checks/m5-w01-c02-t1-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.3";
const string RedMarker="M5-W01-C02-T1 RED: bounded LaTeX importer and explicit ambiguity convention public API are absent.";
const string ConsumerMarker="dw.tools.math/m5-latex-import-consumer/1.0 qualified; probes=8";
const int IndependentGreen=50,AllTargeted=51;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS068",Milestone,Work,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS067",1,
    "feat: qualify canonical Math display boundaries and close C01",
    "0.1.59",1,1,0,false);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("exact-safe-canonical-display-original-restrictions-and-ambiguity-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",JsonValue.Create(51));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",JsonValue.Create(52));
p.Require.JsonValueEquals(Prior,"/isolated_consumer_probes_passed",JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/compiled_red_observed",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/canonical_text_grammar",
    JsonValue.Create("dw-math-canonical-text/1"));
p.Require.JsonValueEquals(Prior,"/local_product_version",JsonValue.Create("1.0.0-preview.2"));
p.Require.JsonValueEquals(Prior,"/m5_w01_c01_phase_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/m5_w01_work_package_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/m5_release_done",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.66"),JsonValue.Create("0.1.67"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.67":"0.1.66"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/33/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/34/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/34/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var slot in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/34/tasks/0/subtasks/{slot}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/34/tasks/1/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/35/status",JsonValue.Create("planned"));

p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M5-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W01-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M5-W01-C03","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
foreach(var id in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");

if(!target)
{
    foreach(var file in new[]{Source,RedTest,GreenTests,Decision,Document,
        ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified})
        p.Require.FileMissing(file);
    p.Require.TextContains(Product,"<Version>1.0.0-preview.2</Version>");
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-bounded-latex-import-api-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("bounded-exact-latex-import-and-ambiguity-convention-qualified"));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/isolated_consumer_probes_passed",JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c02_t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c02_phase_done",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/m5_release_done",JsonValue.Create(false));
    p.Require.TextContains(Product,"<Version>1.0.0-preview.3</Version>");
}

var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redReceipt=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
hashes[RedTest]=redReceipt.AfterSha256 ??
    throw new InvalidOperationException("Missing staged reflection RED test SHA-256.");

if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var failedRed=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W01C02T1RedTests.CompiledBoundedLatexImportApiMustBePresent",
        "--logger","console;verbosity=normal");
    if(failedRed.Code==0 ||
       !failedRed.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "One compiled missing-public-API bounded LaTeX RED is required: "+
            Relevant(failedRed.Output));
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-W01-C02-T1-contract-red",
        ["status"]="compiled-bounded-latex-import-api-absent-red-observed",
        ["prior_display_boundary_contract"]=Prior,
        ["compiled_predecessor"]="dw.tools.math.codecs/1.0.0-preview.2",
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["missing_api"]="BoundedLatexImporter.Import(string,LatexJuxtapositionConvention)",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_not_a_red_substitute"]=true,
        ["nonclaim"]="An absent public importer is not evidence of mathematical LaTeX semantics."
    };
    p.Files.WriteComplete(RedProof,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

foreach(var file in new[]{Product,Source,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var installed=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=installed.AfterSha256 ??
        throw new InvalidOperationException("Missing staged Math LaTeX import source SHA-256: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M5W01C02T1",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 51",StringComparison.OrdinalIgnoreCase)||
   !targeted.Contains("Passed: 51",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "The recovered compiled RED and 50 independent LaTeX GREENs were not all observed.");
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
            "Exact IR/quantities regression not qualified: "+regressionProject);
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
var feed=Path.Combine(root,"build","artifacts","m5-w01-c02-t1-local-feed");
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
        "The bounded LaTeX import package-only consumer must clear external package feeds.");
var isolated=Path.Combine(root,"build","artifacts","m5-w01-c02-t1-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException(
        "Eight independent bounded LaTeX import NuGet-only consumer oracles not observed.");

if(!target)
{
    var evidence=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W01-C02-T1-contract-qualified",
        ["status"]="bounded-latex-exact-subset-and-juxtaposition-ambiguity-qualified",
        ["prior_display_boundary_qualification"]=Prior,
        ["compiled_missing_api_red"]=RedProof,
        ["question"]="Can a closed non-executing LaTeX subset parse exact 1/2 and selected mathematical structures while refusing ambiguous function-versus-multiplication notation without a declared caller convention?",
        ["observation"]="One compiled missing-API RED recovered, 50 independent GREEN oracles and eight isolated NuGet-only consumer probes distinguish exact fraction/operation import, convention-controlled x(y), square/power LaTeX collision, invalid syntax, unsupported macros and resource budgets.",
        ["decision"]="Admit a bounded passive LaTeX importer with exact rational and limited real-expression IR output; default to Ambiguous for juxtaposition, allow explicit Multiplication convention, never infer the source tree from colliding T1 square/power displays.",
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
        ["latex_import_rule"]="math-bounded-latex-exact-subset/v1",
        ["default_juxtaposition_is_ambiguous"]=true,
        ["caller_convention_is_recorded"]=true,
        ["latex_half_is_exact"]="1/2",
        ["latex_t1_square_power_collision_is_ambiguous"]=true,
        ["input_tex_commands_are_never_executed"]=true,
        ["arbitrary_macros_unsupported"]=true,
        ["noncanonical_fraction_is_refused"]=true,
        ["maximum_latex_characters"]=512,
        ["maximum_parser_depth"]=12,
        ["maximum_exact_bits"]=128,
        ["source_semantic_structural_sha256_qualified"]=true,
        ["ambiguity_or_refusal_has_no_ir_or_proof"]=true,
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
        ["m5_w01_c02_t1_done"]=true,
        ["m5_w01_c02_phase_done"]=false,
        ["m5_w01_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M5-W01-C02-T2: richer semantic loss metadata, hostile macro and ambiguity boundaries without broad all-LaTeX claims",
        ["nonclaims"]=p.Json.StringArray(
            "No unrestricted TeX interpreter, macro expansion, arbitrary Unicode/infix grammar, broad function application or unknown symbol admission.",
            "No lossless inverse to any display-only LaTeX, general exponent algebra, implicit algebraic simplification or generic symbolic CAS.",
            "No public NuGet, stable 1.0, other RID platform evidence, AURA/Decision/MCDM adoption or dw.discovery skill.",
            "Only M5-W01-C02-T1 completes; C02 T2, work package W01 and M5 release remain unqualified.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.67";
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)chunk["status"]!="planned" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("The accepted M5-W01-C02 T1 baseline changed.");
        chunk["status"]="in_progress";
        chunk["refinement"]="RS068 T1 establishes a bounded non-executing exact LaTeX import grammar over the already-qualified canonical Math IR/text codec. Fractions, three free real identifiers, elementary functions, explicit multiplication and bounded sums/differences are imported exactly. Juxtaposition x(y) is ambiguous by default and multiplication needs a caller-declared convention; T1 square/power LaTeX collision also remains ambiguous. Refusal states distinguish invalid syntax, unsupported TeX macros, noncanonical rationals and budget exhaustion. One compiled missing-API RED, 50 independent GREEN tests, eight NuGet-only consumer probes and three locally inspected archives qualify T1. C02 remains in-progress; T2 retains adversarial loss/ambiguity policy, and M5/W01 remain in-progress.";
        chunk["readiness"]="T1 bounded LaTeX exact import and explicit ambiguity convention qualified by RS068; T2 hostile inputs/loss metadata and the full C02 phase remain not-ready";
        chunk["files"]=p.Json.StringArray(Product,Source,RedTest,GreenTests,
            Decision,Document,ConsumerProject,ConsumerSource,ConsumerConfig,
            RedProof,Qualified);
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M5W01C02T1",
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
p.ProjectPlan.RequireNodeState("M5-W01-C01","done");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState(Task,"done");
p.ProjectPlan.RequireNodeState("M5-W01-C02-T2","not-ready");
p.ProjectPlan.RequireNodeState("M5-W01-C03","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
foreach(var child in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(child,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS068" ||
       (string?)state["activeMilestone"]!=Milestone ||
       (string?)state["activeWorkPackage"]!=Work ||
       (string?)state["activePhase"]!=(target?Phase:null) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException(
            "RS068 exact M5-W01-C02 activation cursor baseline/target diverged.");
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
    var prefix="M5-W01-C02-T1-PKG-SHA256:"+id+":"+version+":";
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
