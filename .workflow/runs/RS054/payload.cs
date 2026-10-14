using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Dw.Tools.Workflow.Payloads;

const string Work="M4-W01", Phase="M4-W01-C01", Task="M4-W01-C01-T2";
const string Question="M4-W01-C01-T2-A", Observation="M4-W01-C01-T2-B",
    DecisionNode="M4-W01-C01-T2-C";
const string Backlog="docs/planning/backlog.json";
const string Prior="docs/planning/evidence/M4-W01-C01-T1-qualified.json";
const string Qualified="docs/planning/evidence/M4-W01-C01-T2-boundaries-qualified.json";
const string Project="tests/providers/m4-w01-c01-boundaries/m4-provider-boundaries.csproj";
const string Program="tests/providers/m4-w01-c01-boundaries/Program.cs";
const string Decision="docs/planning/decisions/m4-w01-c01-t2-boundaries.md";
const string Distribution="docs/distribution/symbolic-provider-bounded-host.md";
const string HashHelper=".workflow/runs/RS054/staged/checks/m4-t2-provider-archive.cs";
const string ProviderVersion="2.5.0";
const string ProviderSha="8f194bf9ee09cc0514bac5894d413bb8c7e6c6f01071aa8274e24718bf855419";
const string ExpectedHost="dw.tools.math/m4-symbolic-boundaries/0.5 qualified; probes=14; timeout=host; cancellation=host; ir=unsupported";
const string ExpectedArchive="M4-T2-ANGOURI-ARCHIVE:2.5.0:"+ProviderSha+":MIT:deps=5";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));
p.Workflow.RequireCurrentRun("RS054","M4",Work,target?null:Phase,null,null);
p.Workflow.RequirePriorSuccess("RS053",1,
    "docs: qualify M4 symbolic provider candidates and conditional domain behavior",
    "0.1.59",1,1,0,false);

p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m4-two-managed-symbolic-provider-and-conditional-result-candidate-qualified"));
p.Require.JsonValueEquals(Prior,"/selected_candidate",
    JsonValue.Create("AngouriMath/2.5.0-provisional"));
p.Require.JsonValueEquals(Prior,"/angouri_conditional_exclusion_observed",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/provider_cache_isolated",JsonValue.Create(true));
p.Require.JsonValueEquals(Prior,"/math_product_symbolic_adapter_delivered",JsonValue.Create(false));
p.Require.JsonValueEquals(Prior,"/observed_provider_archives/0/id",JsonValue.Create("AngouriMath"));
p.Require.JsonValueEquals(Prior,"/observed_provider_archives/0/version",
    JsonValue.Create(ProviderVersion));
p.Require.JsonValueEquals(Prior,"/observed_provider_archives/0/sha256",
    JsonValue.Create(ProviderSha));
p.Require.JsonValueEquals(Prior,"/observed_provider_archives/0/license",JsonValue.Create("MIT"));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.52"),JsonValue.Create("0.1.53"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.53":"0.1.52"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/9/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/27/status",
    JsonValue.Create(target?"done":"in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/27/tasks/0/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/27/tasks/1/status",
    JsonValue.Create(target?"done":"planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/27/tasks/1/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/28/status",JsonValue.Create("planned"));
p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,target?"done":"in-progress");
p.ProjectPlan.RequireNodeState("M4-W01-C01-T1","done");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
foreach(var child in new[]{Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(child,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M4-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02","not-ready");

if(!target)
{
    foreach(var name in new[]{Qualified,Project,Program,Decision,Distribution})
        p.Require.FileMissing(name);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("m4-isolated-host-budget-cancellation-and-assumption-refusal-qualified"));
    p.Require.JsonValueEquals(Qualified,"/host_probes_passed",JsonValue.Create(14));
    p.Require.JsonValueEquals(Qualified,"/public_symbolic_adapter_delivered",JsonValue.Create(false));
    p.Require.JsonValueEquals(Qualified,"/provider_cooperative_cancellation_qualified",
        JsonValue.Create(false));
}

var staged=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var path in new[]{Project,Program,Decision,Distribution})
{
    var receipt=p.Files.ReplaceFromStaged("staged/"+path,path);
    staged[path]=receipt.AfterSha256 ??
        throw new InvalidOperationException("Missing typed SDK staged-file SHA-256: "+path);
}

if(!target)
{
    // The only CAS package is test-only and remains outside the canonical
    // Math solution. No implicit provider product dependency is authorized.
    var projectXml=XDocument.Load(Path.Combine(root,Project));
    var references=projectXml.Descendants().Where(n=>n.Name.LocalName=="PackageReference").ToArray();
    if(references.Length!=1 ||
       (string?)references[0].Attribute("Include")!="AngouriMath" ||
       (string?)references[0].Attribute("Version")!="[2.5.0]" ||
       projectXml.Descendants().Any(n=>n.Name.LocalName=="ProjectReference"))
        throw new InvalidOperationException("T2 host no longer has exactly one pinned test-only Angouri package.");
    var solution=File.ReadAllText(Path.Combine(root,"dw.tools.math.slnx"));
    if(solution.Contains("m4-provider-boundaries",StringComparison.OrdinalIgnoreCase)||
       solution.Contains("m4-w01-c01-angouri",StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException("An unqualified CAS host was inserted into the Math product solution.");

    var cache=Path.Combine(root,"build","artifacts","m4-w01-c01-t2-packages");
    Required(root,"dotnet","restore",Project,"--packages",cache);
    var archive=Path.Combine(cache,"angourimath",ProviderVersion,
        "angourimath."+ProviderVersion+".nupkg");
    if(!File.Exists(archive))throw new InvalidOperationException("Pinned Angouri NuGet input missing.");
    var archiveProof=Required(root,"dotnet","run","--file",HashHelper,"--",archive);
    if(!HasExactLine(archiveProof,ExpectedArchive))
        throw new InvalidOperationException("Pinned restored Angouri MIT package SHA-256 observation missing.");
    Required(root,"dotnet","build",Project,"-c","Release","--no-restore");
    var output=Required(root,"dotnet","run","--project",Project,
        "-c","Release","--no-build","--no-restore");
    if(!HasExactLine(output,ExpectedHost))
        throw new InvalidOperationException("Bounded symbolic host's fourteen independent oracles did not all pass.");

    // The candidate test host does not change product assemblies; retain
    // the exact and IR baselines without routinely recertifying all M3.
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    foreach(var test in new[]{
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj"})
    {
        var testOutput=Required(root,"dotnet","test",test,"-c","Release",
            "--no-build","--no-restore","--logger","console;verbosity=minimal");
        if(!testOutput.Contains("Passed",StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Relevant IR/quantity regression has no passing test output: "+test);
    }
    Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

    var evidence=new JsonObject
    {
        ["schema_version"]=1,
        ["id"]="M4-W01-C01-T2-boundaries-qualified",
        ["status"]="m4-isolated-host-budget-cancellation-and-assumption-refusal-qualified",
        ["prior_provider_qualification"]=Prior,
        ["question"]="Can an experimental host refuse unsupported symbolic assumptions and forcibly bound a noncooperative Angouri child process without exposing a public CAS contract?",
        ["observation"]="The isolated .NET 10 AngouriMath worker/host passes 14 admission, domain, derivative, conditional text, nonzero exit, timeout and cancellation probes; exact restored MIT archive identity matches RS053.",
        ["decision"]="Close provider qualification only for fixed corpus, host-enforced process boundary and explicit unknown-assumption refusal; keep Angouri provisional and typed IR predicates/cancellation cooperation deferred.",
        ["provider_id"]="AngouriMath",
        ["provider_version"]=ProviderVersion,
        ["observed_package_sha256"]=ProviderSha,
        ["observed_package_license"]="MIT",
        ["observed_package_dependencies"]=p.Json.StringArray(
            "Antlr4.Runtime.Standard","GenericTensor","HonkSharp","PeterO.Numbers","System.Memory"),
        ["archive_helper"]=HashHelper,
        ["actual_archive_verified_against_RS053"]=true,
        ["host_project"]=Project,
        ["host_project_sha256"]=staged[Project],
        ["host_source"]=Program,
        ["host_source_sha256"]=staged[Program],
        ["decision_document"]=Decision,
        ["decision_sha256"]=staged[Decision],
        ["distribution_document"]=Distribution,
        ["distribution_sha256"]=staged[Distribution],
        ["host_probes_passed"]=14,
        ["host_exact_marker"]=ExpectedHost,
        ["input_length_limit_characters"]=128,
        ["input_nesting_limit"]=8,
        ["input_token_character_limit"]=64,
        ["host_input_whitelist_is_closed"]=true,
        ["conditional_exclusion_at_x_equal_1_refused"]=true,
        ["conditional_provider_text_is_typed_IR"]=false,
        ["sqrt_x_squared_without_real_assumptions_admitted"]=false,
        ["unknown_symbolic_expressions_admitted"]=false,
        ["worker_error_never_success"]=true,
        ["host_timeout_milliseconds"]=200,
        ["host_cancellation_after_launch_milliseconds"]=150,
        ["host_kills_child_process_tree"]=true,
        ["provider_cooperative_cancellation_qualified"]=false,
        ["operating_system_security_sandbox_qualified"]=false,
        ["memory_cpu_quota_qualified"]=false,
        ["other_runtime_platforms_qualified"]=false,
        ["product_solution_compiles_locked"]=true,
        ["relevant_IR_and_quantities_tests_passed"]=true,
        ["transfer_architecture_passed"]=true,
        ["public_symbolic_adapter_delivered"]=false,
        ["math_work_package_done"]=false,
        ["next_accepted_scope"]="M4-W01-C02 typed IR condition and exclusion-preserving symbolic bridge",
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["nonclaims"]=p.Json.StringArray(
            "No arbitrary input CAS parser, general symbolic engine, approved provider type bridge, unconditional simplification, proven equivalence or external broker action.",
            "No cooperative Angouri cancellation, general-purpose resource accounting, secure sandbox, non-Windows process-tree guarantee, or unrestricted computation.",
            "No Math product package dependency on AngouriMath, mandatory Python runtime, AURA or Decision/MCDM integration.",
            "No M4 release, completed M4-W01 work package, public Symbolics package, dw.discovery generated skill or BrokerReady authority.")
    };
    p.Files.WriteComplete(Qualified,
        evidence.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
    p.Json.EditObject(Backlog,plan=>
    {
        plan["plan_version"]="0.1.53";
        var phase=plan["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        phase["status"]="done";
        phase["refinement"]="RS053 T1 pinned isolated managed provider executables: AngouriMath 2.5.0 preserved observable provided text for a rational domain exclusion and qualified elementary roots/derivative, while MathNet.Symbolics 0.25.0 checked derivative and two candidate root values with no exclusion guarantee. RS054 T2 independently verified identical Angouri package archive SHA-256, a closed fixed mathematical corpus, 14 host/child process probes including typed refusals, finite host timeout and real parent cancellation signal, and relevant exact/IR regressions. Only an experimental host-level process boundary is qualified; no cooperative CAS cancellation, security sandbox, arbitrary symbolic input or typed Math IR condition bridge is admitted. Both tasks complete C01; work package and milestone remain in progress.";
        phase["readiness"]="closed: T1 managed provider comparison and T2 bounded-host/assumption-refusal experiments admitted; next M4-W01-C02 owns actual public symbolic IR bridge";
        var files=phase["files"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["files"]=p.Json.StringArray(files.Concat(new[]{
            Project,Program,Decision,Distribution,Qualified}).Distinct(StringComparer.Ordinal).ToArray());
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+Project+" --packages build/artifacts/m4-w01-c01-t2-packages",
            "dotnet build "+Project+" -c Release --no-restore",
            "dotnet run --project "+Project+" -c Release --no-build --no-restore",
            "dotnet run --file docs/planning/ValidatePlan.cs -- --check");
        var old=phase["evidence"]!.AsArray().Select(x=>x!.GetValue<string>());
        phase["evidence"]=p.Json.StringArray(old.Concat(new[]{Qualified})
            .Distinct(StringComparer.Ordinal).ToArray());
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(Qualified);
        }
    });

    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Question,"not-ready","ready");
    p.ProjectPlan.ActivateReadyContinuation(Question);
    p.ProjectPlan.ConvergeNodeToDone(Question);
    foreach(var child in new[]{Observation,DecisionNode})
    {
        p.ProjectPlan.TransitionNode(child,"not-ready","ready");
        p.ProjectPlan.TransitionNode(child,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(child);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.ProjectPlan.ConvergeNodeToDone(Phase);
}

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"done");
p.ProjectPlan.RequireNodeState("M4-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02","not-ready");
foreach(var id in new[]{Task,Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(id,"done");
p.Json.EditObject(".aura/workflow/state.json",state=>
{
    if((string?)state["pendingRun"]!="RS054"||
       (string?)state["activeMilestone"]!="M4"||
       (string?)state["activeWorkPackage"]!=Work||
       (string?)state["activePhase"]!=(target?null:Phase)||
       state["activeTask"] is not null ||
       state["activeSubtask"] is not null)
        throw new InvalidOperationException("RS054 current/target project cursor diverged.");
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
static string Relevant(string output)
{
    var i=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(i<0)i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=i<0?output:output[i..];
    return part.Length>2400?part[..2400]:part;
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
        throw new TimeoutException(executable+" timed out.");
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
