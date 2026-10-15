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

const string Milestone="M5",Work="M5-W01",Phase="M5-W01-C01",Task="M5-W01-C01-T1";
const string Red="M5-W01-C01-T1-R",Green="M5-W01-C01-T1-G",Verify="M5-W01-C01-T1-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Gate="docs/planning/evidence/M4-gate.json";
const string RedProof="docs/planning/evidence/M5-W01-C01-T1-contract-red.json";
const string Qualified="docs/planning/evidence/M5-W01-C01-T1-contract-qualified.json";
const string Source="src/projects/dw.tools.math.codecs/CanonicalMathNotation.cs";
const string Product="src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj";
const string ProductLock="src/projects/dw.tools.math.codecs/packages.lock.json";
const string Test="tests/projects/dw.tools.math.codecs.tests/dw.tools.math.codecs.tests.csproj";
const string TestLock="tests/projects/dw.tools.math.codecs.tests/packages.lock.json";
const string RedTest="tests/projects/dw.tools.math.codecs.tests/M5W01C01T1RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.codecs.tests/M5W01C01T1Tests.cs";
const string Decision="docs/planning/decisions/m5-w01-c01-t1-canonical-text.md";
const string Document="docs/distribution/m5-canonical-text.md";
const string ConsumerProject="tests/consumers/m5-w01-c01-t1/m5-canonical-text-consumer.csproj";
const string ConsumerSource="tests/consumers/m5-w01-c01-t1/Program.cs";
const string ConsumerConfig="tests/consumers/m5-w01-c01-t1/NuGet.Config";
const string PackageHelper=".workflow/runs/RS066/staged/checks/m5-w01-c01-t1-package-sha.cs";
const string Solution="dw.tools.math.slnx",Version="1.0.0-preview.1";
const string RedMarker="M5-W01-C01-T1 RED: canonical math text parser and display-only LaTeX renderer APIs are absent.";
const string ConsumerMarker="dw.tools.math/m5-canonical-text-consumer/1.0 qualified; probes=8";
const int IndependentGreen=50,AllTargeted=51;
var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));

p.Workflow.RequireCurrentRun("RS066",
    target?Milestone:null,target?Work:null,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS065",1,
    "feat: qualify local M4 symbolic release gate",
    "0.1.59",1,1,0,false);
p.Require.JsonValueEquals(Gate,"/status",
    JsonValue.Create("m4-local-exact-symbolic-preview-gate-qualified"));
p.Require.JsonValueEquals(Gate,"/qualified_tasks",JsonValue.Create(12));
p.Require.JsonValueEquals(Gate,"/qualified_subtasks",JsonValue.Create(36));
p.Require.JsonValueEquals(Gate,"/cross_vertical_nuget_consumer_oracles",JsonValue.Create(32));
p.Require.JsonValueEquals(Gate,"/m4_milestone_done",JsonValue.Create(true));
p.Require.JsonValueEquals(Gate,"/m5_milestone_done",JsonValue.Create(false));
p.Require.JsonValueEquals(Gate,"/packages_publicly_published",JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.64"),JsonValue.Create("0.1.65"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.65":"0.1.64"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/5/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/work_packages/11/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/work_packages/12/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/33/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/33/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/33/tasks/0/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/33/tasks/1/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/34/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/35/status",JsonValue.Create("planned"));
p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Work,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M5-W01-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M5-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
foreach(var id in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,target?"done":"not-ready");
if(!target)
{
    foreach(var file in new[]{Product,ProductLock,Test,TestLock,Source,
        RedTest,GreenTests,Decision,Document,ConsumerProject,ConsumerSource,
        ConsumerConfig,RedProof,Qualified})
        p.Require.FileMissing(file);
    p.Require.TextContains(Solution,
        "<Project Path=\"src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj\" />");
}
else
{
    p.Require.TextContains(Solution,
        "<Project Path=\"src/projects/dw.tools.math.codecs/dw.tools.math.codecs.csproj\" />");
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("exact-canonical-text-ir-latex-subset-qualified"));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreen));
    p.Require.JsonValueEquals(Qualified,"/isolated_consumer_probes_passed",
        JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m5_w01_c01_t1_done",JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/m5_release_done",JsonValue.Create(false));
}
var hashes=new Dictionary<string,string>(StringComparer.Ordinal);
// Bootstrap the first real codec product project as a compilation-ready
// EMPTY assembly. One genuine compiled public-API RED must run before
// introducing the implementation source or GREEN scientific tests.
foreach(var file in new[]{Solution,Product,ProductLock,Test,TestLock,RedTest})
{
    var receipt=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=receipt.AfterSha256 ??
        throw new InvalidOperationException("Missing staged structural project hash: "+file);
}
if(!target)
{
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var red=Run(root,"dotnet","test",Test,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M5W01C01T1RedTests.PublicCanonicalMathTextAndLatexContractMustExist",
        "--logger","console;verbosity=normal");
    if(red.Code==0 || !red.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "A compiled missing public canonical math text / LaTeX API RED was not observed: "+
            Relevant(red.Output));
    var proof=new JsonObject
    {
        ["schema_version"]=1,["id"]="M5-W01-C01-T1-contract-red",
        ["status"]="compiled-canonical-text-apis-absent-red-observed",
        ["prior_m4_symbolic_gate"]=Gate,
        ["compiled_new_assembly_without_implementation"]="dw.tools.math.codecs/1.0.0-preview.1",
        ["red_test"]=RedTest,["red_test_sha256"]=hashes[RedTest],
        ["missing_api"]="CanonicalMathNotation.Parse(string);Render(IrNode)",
        ["expected_red_assertion"]=RedMarker,
        ["independent_red_test_count"]=1,
        ["compiler_failure_is_not_a_red_substitute"]=true,
        ["nonclaim"]="An absent API alone is not evidence of mathematical notation correctness."
    };
    p.Files.WriteComplete(RedProof,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
foreach(var file in new[]{Source,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var receipt=p.Files.ReplaceFromStaged("staged/"+file,file);
    hashes[file]=receipt.AfterSha256 ??
        throw new InvalidOperationException("Missing staged codec product/science hash: "+file);
}
Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
var targeted=Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore",
    "--filter","FullyQualifiedName~M5W01C01T1",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 51",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 51",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException(
        "Expected one compiled RED recovered plus fifty independent canonical text GREEN tests.");
Required(root,"dotnet","test",Test,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var existing in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var regression=Required(root,"dotnet","test",existing,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!regression.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(
            "Canonical Math text did not retain IR or exact quantities regression: "+existing);
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
var feed=Path.Combine(root,"build","artifacts","m5-w01-c01-t1-local-feed");
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
        "The canonical package-only consumer must clear external package feeds.");
var isolated=Path.Combine(root,"build","artifacts","m5-w01-c01-t1-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",isolated);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException(
        "Eight independent canonical text NuGet-only consumer oracles not observed.");

if(!target)
{
    var evidence=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M5-W01-C01-T1-contract-qualified",
        ["status"]="exact-canonical-text-ir-latex-subset-qualified",
        ["prior_m4_scientific_gate"]=Gate,
        ["compiled_missing_api_red"]=RedProof,
        ["question"]="Can bounded exact canonical text preserve rational, polynomial, function, quantity and original-domain exclusion IR meaning while emitting display-only LaTeX without running TeX or a CAS?",
        ["observation"]="A genuine compiled missing-API RED was recovered; fifty independent GREEN oracles and eight isolated NuGet-only probes cover typed text/IR/LaTeX, unit metadata, exclusions and canonical refusals.",
        ["decision"]="Admit a closed canonical ASCII prefix grammar with rational coefficients, x/y/z, arithmetic and sqrt/abs, three fixed units and typed eq/ne/ge restrictions; display LaTeX without interpreting it.",
        ["source_file"]=Source,
        ["source_sha256"]=hashes[Source],
        ["solution_file"]=Solution,
        ["solution_sha256"]=hashes[Solution],
        ["product_project"]=Product,
        ["product_project_sha256"]=hashes[Product],
        ["product_lock_file"]=ProductLock,
        ["product_lock_sha256"]=hashes[ProductLock],
        ["test_project"]=Test,
        ["test_project_sha256"]=hashes[Test],
        ["test_lock_file"]=TestLock,
        ["test_lock_sha256"]=hashes[TestLock],
        ["green_tests"]=GreenTests,
        ["green_tests_sha256"]=hashes[GreenTests],
        ["red_test"]=RedTest,
        ["red_test_sha256"]=hashes[RedTest],
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=hashes[Decision],
        ["distribution_document"]=Document,
        ["distribution_document_sha256"]=hashes[Document],
        ["independent_green_tests_passed"]=IndependentGreen,
        ["red_recovered_and_green_tests_passed"]=AllTargeted,
        ["compiled_red_observed"]=true,
        ["isolated_consumer_probes_passed"]=8,
        ["canonical_grammar_id"]="dw-math-canonical-text/1",
        ["canonical_ascii_only"]=true,
        ["maximum_input_characters"]=1024,
        ["maximum_latex_characters"]=4096,
        ["maximum_parser_depth"]=16,
        ["maximum_arguments"]=8,
        ["maximum_conditions"]=8,
        ["maximum_exact_rational_bits"]=128,
        ["number_of_whitelisted_units"]=3,
        ["text_ir_structural_roundtrip_qualified"]=true,
        ["latex_display_only_no_inverse_parser"]=true,
        ["original_domain_exclusions_retained"]=true,
        ["arbitrary_tex_command_execution_admitted"]=false,
        ["unqualified_unit_aliases_admitted"]=false,
        ["implicit_finite_binary64_exactness_admitted"]=false,
        ["no_external_cas_or_agent_dependency"]=true,
        ["locked_solution_build_passed"]=true,
        ["codec_tests_and_exact_ir_quantities_regressions_passed"]=true,
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
        ["m5_w01_c01_t1_done"]=true,
        ["m5_w01_c01_phase_done"]=false,
        ["m5_w01_work_package_done"]=false,
        ["m5_release_done"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M5-W01-C01-T2: escaping, depth, command allowlist and semantic/rendering ambiguity refusal, without parsing arbitrary TeX",
        ["nonclaims"]=p.Json.StringArray(
            "No general infix parser, implicit decimal aliases, TeX import/execution, MathML, Markdown, Office or generic symbol namespace.",
            "No inverse LaTeX/typographic fidelity claim, unbounded mathematical evaluation, arbitrary quantity conversion or CAS.",
            "No public NuGet, stable 1.0, AURA/Decision/MCDM adoption, broker permissions or dw.discovery conformance.",
            "No completion of C01 phase, M5-W01 work package or M5 release; boundaries T2 and future interoperability work remain open.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.65";
        var release=plan["releases"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Milestone);
        var wp=plan["work_packages"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Work);
        var chunk=plan["chunks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Phase);
        var task=chunk["tasks"]!.AsArray()
            .Select(x=>x!.AsObject()).Single(x=>(string?)x["id"]==Task);
        if((string?)release["status"]!="planned" ||
           (string?)wp["status"]!="planned" ||
           (string?)chunk["status"]!="planned" ||
           (string?)task["status"]!="planned")
            throw new InvalidOperationException("M5-W01-C01 baseline unexpectedly changed.");
        release["status"]="in_progress";
        wp["status"]="in_progress";
        chunk["status"]="in_progress";
        chunk["refinement"]="RS066 introduces a real .NET 10 dw.tools.math.codecs product with lock-file-managed IR/quantities dependencies, one compiled missing-API RED and 50 exact/boundary GREEN tests. A versioned closed canonical ASCII prefix grammar parses rationals, free real x/y/z, bounded arithmetic and function nodes, three typed unit identities and retained eq/ne/ge original-domain restrictions to Math IR. Rendering emits display-only LaTeX and source semantic hash without evaluation. Eight independent pinned NuGet-only oracles inspect the newly packed three-product graph and meaning-preserving IR roundtrip. T2 retains adversarial escaping, ambiguity and broader boundary responsibilities; M5/W01/C01 remain in-progress.";
        chunk["readiness"]="T1 exact canonical parser and safe LaTeX display admitted only after RS066 durable evidence; T2 and future import/formatting phases remain unfinished";
        chunk["files"]=p.Json.StringArray(Solution,Product,ProductLock,
            Test,TestLock,Source,RedTest,GreenTests,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified);
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+Test+" -c Release --filter FullyQualifiedName~M5W01C01T1",
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
    // M4 has just completed. This is a NEW active project hierarchy,
    // not a continuation of an existing active M4 path.
    foreach(var id in new[]{Milestone,Work,Phase,Task,Red})
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
    foreach(var id in new[]{Milestone,Work,Phase,Task,Red})
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Red);
    foreach(var id in new[]{Green,Verify})
    {
        p.ProjectPlan.TransitionNode(id,"not-ready","ready");
        p.ProjectPlan.TransitionNode(id,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(id);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
}
p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState(Task,"done");
p.ProjectPlan.RequireNodeState("M5-W01-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M5-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M5-W02","not-ready");
foreach(var id in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS066" ||
       (string?)state["activeMilestone"]!=(target?Milestone:null) ||
       (string?)state["activeWorkPackage"]!=(target?Work:null) ||
       (string?)state["activePhase"]!=(target?Phase:null) ||
       state["activeTask"] is not null || state["activeSubtask"] is not null)
        throw new InvalidOperationException(
            "RS066 exact new M5 hierarchy cursor baseline/target diverged.");
    state["activeMilestone"]=Milestone;
    state["activeWorkPackage"]=Work;
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
    var prefix="M5-W01-C01-T1-PKG-SHA256:"+id+":"+version+":";
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
