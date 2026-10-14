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

const string Work="M4-W01",Phase="M4-W01-C03",Task="M4-W01-C03-T2";
const string Red="M4-W01-C03-T2-R",Green="M4-W01-C03-T2-G",
    Verify="M4-W01-C03-T2-V";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M4-W01-C03-T1-contract-qualified.json";
const string RedProof="docs/planning/evidence/M4-W01-C03-T2-boundary-red.json";
const string Qualified="docs/planning/evidence/M4-W01-C03-T2-boundaries-qualified.json";
const string Product="src/projects/dw.tools.math.symbolics/SymbolicBoundedExecution.cs";
const string Project="src/projects/dw.tools.math.symbolics/dw.tools.math.symbolics.csproj";
const string TestProject="tests/projects/dw.tools.math.symbolics.tests/dw.tools.math.symbolics.tests.csproj";
const string RedTest="tests/projects/dw.tools.math.symbolics.tests/M4W01C03T2RedTests.cs";
const string GreenTests="tests/projects/dw.tools.math.symbolics.tests/M4W01C03T2Tests.cs";
const string Decision="docs/planning/decisions/m4-w01-c03-t2-bounded-execution.md";
const string Document="docs/distribution/symbolic-bounded-execution.md";
const string ConsumerProject="tests/consumers/m4-w01-c03-t2/m4-symbolic-budget-consumer.csproj";
const string ConsumerSource="tests/consumers/m4-w01-c03-t2/Program.cs";
const string ConsumerConfig="tests/consumers/m4-w01-c03-t2/NuGet.Config";
const string PackageHelper=".workflow/runs/RS058/staged/checks/m4-c03-t2-product-sha.cs";
const string Solution="dw.tools.math.slnx",Version="0.5.0-preview.4";
const string RedMarker="M4-W01-C03-T2 RED: bounded exact symbolic execution contract is absent.";
const string ConsumerMarker="dw.tools.math/m4-symbolic-budget-consumer/0.5 qualified; probes=8";
const int IndependentGreenTests=26,TargetedTests=27;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));

p.Workflow.RequireCurrentRun("RS058","M4",target?null:Work,
    target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS057",1,
    "feat: qualify bounded exact symbolic transformations and closed substitutions",
    "0.1.59",3,3,0,false);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("exact-binomial-transformation-free-symbol-substitution-and-real-sqrt-qualified"));
p.Require.JsonValueEquals(Prior,"/independent_green_tests_passed",
    JsonValue.Create(28));
p.Require.JsonValueEquals(Prior,"/red_recovered_and_green_tests_passed",
    JsonValue.Create(29));
p.Require.JsonValueEquals(Prior,"/independent_consumer_probes_passed",
    JsonValue.Create(8));
p.Require.JsonValueEquals(Prior,"/coefficient_maximum_bits",
    JsonValue.Create(128));
p.Require.JsonValueEquals(Prior,"/general_binder_alpha_renaming_qualified",
    JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/external_CAS_runtime_required",
    JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/local_product_version",
    JsonValue.Create("0.5.0-preview.3"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.56"),JsonValue.Create("0.1.57"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.57":"0.1.56"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/9/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/10/status",
    JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/27/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/28/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/29/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/29/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/29/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var i in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/29/tasks/1/subtasks/{i}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/30/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/external_dependencies/3/status",
    JsonValue.Create("satisfied"));

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState(Work,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M4-W01-C01","done");
p.ProjectPlan.RequireNodeState("M4-W01-C02","done");
p.ProjectPlan.RequireNodeState("M4-W01-C03-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M4-W02","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02-C01","not-ready");
foreach(var node in new[]{Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");

if(!target)
{
    foreach(var file in new[]{Product,RedTest,GreenTests,Decision,Document,
        ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified})
        p.Require.FileMissing(file);
    p.Require.TextContains(Project,"<Version>0.5.0-preview.3</Version>");
}
else
{
    p.Require.JsonValueEquals(RedProof,"/status",
        JsonValue.Create("compiled-bounded-symbolic-execution-contract-absent-red-observed"));
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("bounded-symbolic-provenance-cancellation-and-soft-deadline-qualified"));
    p.Require.JsonValueEquals(Qualified,"/independent_green_tests_passed",
        JsonValue.Create(IndependentGreenTests));
    p.Require.JsonValueEquals(Qualified,"/independent_consumer_probes_passed",
        JsonValue.Create(8));
    p.Require.JsonValueEquals(Qualified,"/m4_work_package_done",
        JsonValue.Create(true));
    p.Require.JsonValueEquals(Qualified,"/deadline_is_preemptive",
        JsonValue.Create(false));
    p.Require.TextContains(Project,"<Version>0.5.0-preview.4</Version>");
}

var stageHashes=new Dictionary<string,string>(StringComparer.Ordinal);
var redInstalled=p.Files.ReplaceFromStaged("staged/"+RedTest,RedTest);
stageHashes[RedTest]=redInstalled.AfterSha256 ??
    throw new InvalidOperationException("Missing typed RED test source identity.");

if(!target)
{
    // Exact scientific RED: compiled preview.3 assembly with a missing
    // public bounded execution API, NOT a missing project or compiler error.
    Required(root,"dotnet","restore",Solution,"--locked-mode");
    Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");
    var redRun=Run(root,"dotnet","test",TestProject,"-c","Release",
        "--no-build","--no-restore",
        "--filter","FullyQualifiedName~M4W01C03T2RedTests.BoundedExecutionAndProofProvenanceMustHaveAPublicContract",
        "--logger","console;verbosity=normal");
    if(redRun.Code==0 ||
       !redRun.Output.Contains(RedMarker,StringComparison.Ordinal))
        throw new InvalidOperationException(
            "Controlled compiled RED for missing bounded execution API was not observed: "+
            Relevant(redRun.Output));

    var redEvidence=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W01-C03-T2-boundary-red",
        ["status"]="compiled-bounded-symbolic-execution-contract-absent-red-observed",
        ["prior_exact_transformations"]=Prior,
        ["red_test"]=RedTest,["red_test_sha256"]=stageHashes[RedTest],
        ["compiled_product_version"]="0.5.0-preview.3",
        ["missing_api"]="Dw.Tools.Math.Symbolics.SymbolicBoundedExecution.Execute",
        ["failed_red_assertion"]=RedMarker,
        ["single_independent_red_test"]=true,
        ["compiler_or_restore_failure_substituted_for_red"]=false,
        ["nonclaim"]="A public API's absence does not prove boundedness or mathematics."
    };
    p.Files.WriteComplete(RedProof,
        redEvidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}

foreach(var file in new[]{Project,Product,GreenTests,Decision,Document,
    ConsumerProject,ConsumerSource,ConsumerConfig})
{
    var receipt=p.Files.ReplaceFromStaged("staged/"+file,file);
    stageHashes[file]=receipt.AfterSha256 ??
        throw new InvalidOperationException("Missing staged target/source hash for "+file);
}

Required(root,"dotnet","restore",Solution,"--locked-mode");
Required(root,"dotnet","build",Solution,"-c","Release","--no-restore");

// Differential tests first, then the few relevant broader regressions.
var targeted=Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--filter","FullyQualifiedName~M4W01C03T2",
    "--logger","console;verbosity=normal");
if(!targeted.Contains("Total tests: 27",StringComparison.OrdinalIgnoreCase) ||
   !targeted.Contains("Passed: 27",StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Expected one recovered RED plus 26 GREEN boundary tests.");
Required(root,"dotnet","test",TestProject,"-c","Release",
    "--no-build","--no-restore","--logger","console;verbosity=minimal");
foreach(var project in new[]{
    "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj",
    "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj"})
{
    var result=Required(root,"dotnet","test",project,"-c","Release",
        "--no-build","--no-restore","--logger","console;verbosity=minimal");
    if(!result.Contains("Passed",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("A relevant Math IR/quantities test pack was not observed: "+project);
}
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

var packages=new (string Id,string Version,string Project,string[] DependsOn)[]
{
    ("dw.quantities","0.2.0-preview.1",
        "src/projects/dw.quantities/dw.quantities.csproj",[]),
    ("dw.tools.math.ir","0.3.0-preview.1",
        "src/projects/dw.tools.math.ir/dw.tools.math.ir.csproj",["dw.quantities"]),
    ("dw.tools.math.symbolics",Version,Project,["dw.tools.math.ir"])
};
var feed=Path.Combine(root,"build","artifacts","m4-w01-c03-t2-local-feed");
var archives=new List<string>();
foreach(var package in packages)
{
    Required(root,"dotnet","pack",package.Project,"-c","Release",
        "--no-build","--no-restore","-o",feed);
    var archive=Path.Combine(feed,package.Id+"."+package.Version+".nupkg");
    if(!File.Exists(archive))
        throw new InvalidOperationException("Expected local product package archive missing: "+package.Id);
    archives.Add(archive);
}
var helperArgs=new List<string>{"run","--file",PackageHelper,"--"};
helperArgs.AddRange(archives);
var packageOutput=Required(root,"dotnet",helperArgs.ToArray());
var productReceipts=new List<JsonNode>();
foreach(var package in packages)
{
    var receipt=ReadProductReceipt(packageOutput,package.Id,package.Version);
    productReceipts.Add(new JsonObject
    {
        ["id"]=package.Id,["version"]=package.Version,
        ["bytes"]=receipt.Bytes,["sha256"]=receipt.Sha,
        ["dependencies"]=p.Json.StringArray(package.DependsOn),
        ["assembly"]="lib/net10.0/"+package.Id+".dll",
        ["publication"]="local-only-no-public-feed"
    });
}

var clientXml=XDocument.Load(Path.Combine(root,ConsumerProject));
var direct=clientXml.Descendants().Where(x=>x.Name.LocalName=="PackageReference").ToArray();
if(direct.Length!=1 ||
   (string?)direct[0].Attribute("Include")!="dw.tools.math.symbolics" ||
   (string?)direct[0].Attribute("Version")!="["+Version+"]" ||
   clientXml.Descendants().Any(x=>x.Name.LocalName=="ProjectReference"))
    throw new InvalidOperationException("Independent M4 consumer requires one pinned NuGet-only product.");
var sources=XDocument.Load(Path.Combine(root,ConsumerConfig)).Descendants()
    .Single(x=>x.Name.LocalName=="packageSources");
if(!sources.Elements().Any(x=>x.Name.LocalName=="clear") ||
   sources.Elements().Any(x=>x.Name.LocalName=="add"))
    throw new InvalidOperationException("The package-only consumer must clear external sources.");
var packageCache=Path.Combine(root,"build","artifacts","m4-w01-c03-t2-isolated-packages");
Required(root,"dotnet","restore",ConsumerProject,"--source",feed,
    "--configfile",ConsumerConfig,"--packages",packageCache);
Required(root,"dotnet","build",ConsumerProject,"-c","Release","--no-restore");
var consumerOutput=Required(root,"dotnet","run","--project",ConsumerProject,
    "-c","Release","--no-build","--no-restore");
if(!HasExactLine(consumerOutput,ConsumerMarker))
    throw new InvalidOperationException("Eight independent NuGet-only budgeted symbolic oracles did not pass.");

if(!target)
{
    var qualified=new JsonObject
    {
        ["schema_version"]=1,["id"]="M4-W01-C03-T2-boundaries-qualified",
        ["status"]="bounded-symbolic-provenance-cancellation-and-soft-deadline-qualified",
        ["prior_exact_transformations"]=Prior,
        ["controlled_red_evidence"]=RedProof,
        ["question"]="Can four already-qualified exact rules expose finite IR/work/time budgets, stable local proof provenance and explicit interruption refusals without implying a general CAS or hard preemption?",
        ["observation"]="One compiled missing-API RED recovered; 26 independent GREEN tests and eight local NuGet-only client probes observed typed budgets, structural provenance and cancellation/deadline refusal without returned partial mathematical values.",
        ["decision"]="Admit one restricted synchronous local dispatcher, with finite input/output node/depth/work policies, soft elapsed/cancellation checks and four local T1 proof-rule IDs. General algebra, binder capture handling and hard preemption remain unsupported or separately owned.",
        ["new_product_source"]=Product,
        ["new_product_source_sha256"]=stageHashes[Product],
        ["product_project"]=Project,["product_project_sha256"]=stageHashes[Project],
        ["green_tests"]=GreenTests,["green_tests_sha256"]=stageHashes[GreenTests],
        ["red_test"]=RedTest,["red_test_sha256"]=stageHashes[RedTest],
        ["decision_document"]=Decision,
        ["decision_document_sha256"]=stageHashes[Decision],
        ["distribution_document"]=Document,
        ["distribution_document_sha256"]=stageHashes[Document],
        ["independent_green_tests_passed"]=IndependentGreenTests,
        ["red_recovered_and_green_tests_passed"]=TargetedTests,
        ["compiled_red_observed"]=true,
        ["all_relevant_symbolics_tests_passed"]=true,
        ["ir_and_quantities_regressions_passed"]=true,
        ["bounded_operations"]=p.Json.StringArray(
            "ExpandSquare","FactorSquare","SubstituteFree","NormalizeRealSqrtSquare"),
        ["known_rule_identifiers"]=p.Json.StringArray(
            "binomial-square/exact/v1","binomial-factor/exact/v1",
            "free-symbol-substitute/closed/v1","sqrt-real-square/domain/v1"),
        ["maximum_nodes_per_ir"]=1024,
        ["default_nodes_per_ir"]=128,
        ["maximum_ir_depth"]=32,
        ["default_ir_depth"]=16,
        ["maximum_combined_work_units"]=2048,
        ["default_combined_work_units"]=256,
        ["maximum_soft_elapsed_seconds"]=30,
        ["default_soft_elapsed_seconds"]=5,
        ["deadline_is_preemptive"]=false,
        ["cancellation_requires_polling"]=true,
        ["cancelled_or_expired_returns_mathematical_value"]=false,
        ["provenance"]="qualified-rule-identifier plus typed source math-ir/1 semantic structural sha256",
        ["source_hash_is_algebraic_equivalence_proof"]=false,
        ["combined_input_output_work_measured"]=true,
        ["unknown_rules_and_foreign_parameters_refused"]=true,
        ["bound_symbol_capture_refused"]=true,
        ["retained_domain_conditions_confirmed"]=true,
        ["cas_runtime_required"]=false,
        ["public_nuget_published"]=false,
        ["local_product_version"]=Version,
        ["local_product_package_graph"]=p.Json.Array(productReceipts.ToArray()),
        ["package_helper"]=PackageHelper,
        ["consumer_project"]=ConsumerProject,
        ["consumer_project_sha256"]=stageHashes[ConsumerProject],
        ["consumer_source"]=ConsumerSource,
        ["consumer_source_sha256"]=stageHashes[ConsumerSource],
        ["consumer_config"]=ConsumerConfig,
        ["consumer_config_sha256"]=stageHashes[ConsumerConfig],
        ["independent_consumer_probes_passed"]=8,
        ["consumer_marker"]=ConsumerMarker,
        ["provider_neutral_package_only_consumer"]=true,
        ["locked_solution_build_passed"]=true,
        ["transfer_architecture_passed"]=true,
        ["work_package"]="M4-W01",
        ["m4_work_package_done"]=true,
        ["m4_milestone_done"]=false,
        ["m4_w02_started"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["next_accepted_scope"]="M4-W02-C01-T1 functions and exact, bounded symbolic differentiation",
        ["nonclaims"]=p.Json.StringArray(
            "No general CAS, unconstrained algebra, broad binder-aware substitution, alpha-renaming, symbolic equivalence theorem, complex principal branches or unproved transformation.",
            "No hard real-time in-process interruption, unmanaged/foreign provider cancellation, secure OS sandbox or arbitrary memory/CPU cap.",
            "No externally admitted Angouri/MathNET parser, mandatory Python runtime, public NuGet release, agent permission, AURA/Decision/MCDM adaptation or neutral dw.discovery skill.",
            "No M4 milestone/G-M4 qualification or M4-W02 calculus/solver implementation.")
    };
    p.Files.WriteComplete(Qualified,
        qualified.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.57";
        var work=plan["work_packages"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Work);
        var chunk=plan["chunks"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Phase);
        var task=chunk["tasks"]!.AsArray().Select(v=>v!.AsObject())
            .Single(v=>(string?)v["id"]==Task);
        if((string?)work["status"]!="in_progress")
            throw new InvalidOperationException("M4-W01 was not the active work package.");
        foreach(var id in new[]{"M4-W01-C01","M4-W01-C02"})
        {
            if(plan["chunks"]!.AsArray().Select(v=>v!.AsObject())
                .Single(v=>(string?)v["id"]==id)["status"]?.GetValue<string>()!="done")
                throw new InvalidOperationException("Cannot close M4-W01 with unfinished dependency "+id);
        }
        work["status"]="done";
        chunk["status"]="done";
        chunk["refinement"]="RS057 T1 qualified four exact closed symbolic operations in preview.3: bounded binomial expand/factor, safe-by-refusal free-symbol substitution, and real sqrt(square(x)) -> abs(x) without an independent sign proof. RS058 T2 adds a provider-neutral, bounded synchronous dispatcher over those same four known rules: input/output node and depth caps, combined work units, explicit cancellation and soft deadline refusal, and math-ir/1 structural source/proof provenance. One compiled RED and 26 independent GREEN tests, eight NuGet-only consumer oracles, IR/quantities regressions and actual preview.4 package SHA-256 receipts qualify the boundary. This does not prove hard preemption, arbitrary CAS behavior or general capture-avoiding substitution.";
        chunk["readiness"]="closed: both exact symbolic transformation T1 and bounded/provenance/cancellation T2 qualified with explicit limitation receipts; M4-W02 remains planned";
        var oldFiles=chunk["files"]!.AsArray().Select(v=>v!.GetValue<string>());
        chunk["files"]=p.Json.StringArray(oldFiles.Concat(new[]{
            Project,Product,RedTest,GreenTests,Decision,Document,
            ConsumerProject,ConsumerSource,ConsumerConfig,RedProof,Qualified
        }).Distinct(StringComparer.Ordinal).ToArray());
        chunk["commands"]=p.Json.StringArray(
            "dotnet restore "+Solution+" --locked-mode",
            "dotnet build "+Solution+" -c Release --no-restore",
            "dotnet test "+TestProject+" -c Release --filter FullyQualifiedName~M4W01C03T2",
            "dotnet test "+TestProject+" -c Release",
            "dotnet run --project "+ConsumerProject+" -c Release");
        var oldEvidence=chunk["evidence"]!.AsArray().Select(v=>v!.GetValue<string>());
        chunk["evidence"]=p.Json.StringArray(oldEvidence.Concat(new[]{RedProof,Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(RedProof,Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(v=>v!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(
                (string?)child["id"]==Red?RedProof:Qualified);
        }
    });

    // Phase and work package must close in the same native DWF mutation,
    // with one truthful contribution achievement for each completed node.
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
    p.ProjectPlan.ConvergeNodeToDone(Work);
}

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState(Work,"done");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M4-W01-C01","done");
p.ProjectPlan.RequireNodeState("M4-W01-C02","done");
p.ProjectPlan.RequireNodeState("M4-W02","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02-C01","not-ready");
foreach(var id in new[]{Task,Red,Green,Verify})
    p.ProjectPlan.RequireNodeState(id,"done");
p.Json.EditObject(State,state=>
{
    if((string?)state["pendingRun"]!="RS058" ||
       (string?)state["activeMilestone"]!="M4" ||
       (string?)state["activeWorkPackage"]!=(target?null:Work) ||
       (string?)state["activePhase"]!=(target?null:Phase) ||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS058 requires the exact before/after active M4-W01 hierarchy.");
    state["activeWorkPackage"]=null;
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
    var prefix="M4-C03-T2-PKG-SHA256:"+id+":"+version+":";
    var found=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(found.Length!=1)
        throw new InvalidOperationException("One independent NuGet receipt required for "+id);
    var parts=found[0][prefix.Length..].Split(':');
    if(parts.Length!=2 ||
       !long.TryParse(parts[0],NumberStyles.None,CultureInfo.InvariantCulture,out var bytes) ||
       bytes<=0 || parts[1].Length!=64 ||
       parts[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))))
        throw new InvalidOperationException("Invalid product package SHA-256 or byte receipt: "+id);
    return(bytes,parts[1]);
}
static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=i<0?output:output[i..];
    return part.Length>2600?part[..2600]:part;
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
        throw new TimeoutException(executable+" exceeded a bounded external command timeout.");
    }
    var result=stdout.GetAwaiter().GetResult()+"\n"+stderr.GetAwaiter().GetResult();
    Console.WriteLine(result);
    return(process.ExitCode,result);
}
static string Required(string root,string executable,params string[] args)
{
    var result=Run(root,executable,args);
    if(result.Code!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " exited "+result.Code+": "+Relevant(result.Output));
    return result.Output;
}
