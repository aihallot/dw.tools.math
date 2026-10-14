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

const string Phase="M4-W01-C01", Work="M4-W01", Task="M4-W01-C01-T1";
const string Question="M4-W01-C01-T1-A", Observation="M4-W01-C01-T1-B",
    DecisionNode="M4-W01-C01-T1-C";
const string Backlog="docs/planning/backlog.json";
const string Qualified="docs/planning/evidence/M4-W01-C01-T1-qualified.json";
const string Prior="docs/planning/evidence/M3-gate.json";
const string Decision="docs/planning/decisions/m4-w01-c01.md";
const string Corpus="docs/distribution/symbolic-provider-qualification.md";
const string Inventory="docs/planning/decisions/m4-w01-c01-candidates.json";
const string AngouriProject="tests/providers/m4-w01-c01-angouri/m4-angouri-smoke.csproj";
const string AngouriSource="tests/providers/m4-w01-c01-angouri/Program.cs";
const string MathNetProject="tests/providers/m4-w01-c01-mathnet/m4-mathnet-smoke.csproj";
const string MathNetSource="tests/providers/m4-w01-c01-mathnet/Program.cs";
const string ProviderHashHelper=".workflow/runs/RS053/staged/checks/m4-provider-archive.cs";
const string AngouriMarker="m4-symbolics-angouri/2.5.0:qualified; derivative=2*x; roots=-1,1; domain=conditional";
const string MathNetMarker="m4-symbolics-mathnet/0.25.0:qualified; derivative=6@3; root-check=-1,1; exclusion=not-qualified";

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Qualified));

p.Workflow.RequireCurrentRun("RS053",target?"M4":null,
    target?Work:null,target?Phase:null,null,null);
p.Workflow.RequirePriorSuccess("RS052",1,
    "feat: qualify M3 numerical and statistics preview release gate",
    "0.1.59",2,2,0,false);
p.Require.JsonValueEquals(Prior,"/status",
    JsonValue.Create("m3-local-numerics-and-statistics-preview-gate-qualified"));
p.Require.JsonValueEquals(Prior,"/cross_vertical_nuget_consumer_oracles",
    JsonValue.Create(31));
p.Require.JsonValueEquals(Prior,"/external_adoption_observed",
    JsonValue.Create(false));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.51"),JsonValue.Create("0.1.52"));
p.Require.JsonValueEquals(Backlog,"/plan_version",
    JsonValue.Create(target?"0.1.52":"0.1.51"));
p.Require.JsonValueEquals(Backlog,"/releases/2/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/3/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/releases/4/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/work_packages/9/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/work_packages/10/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/27/status",
    JsonValue.Create(target?"in_progress":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/27/tasks/0/status",
    JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/27/tasks/1/status",JsonValue.Create("planned"));
foreach(var index in new[]{0,1,2})
    p.Require.JsonValueEquals(Backlog,$"/chunks/27/tasks/0/subtasks/{index}/status",
        JsonValue.Create(target?"done":"planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/28/status",JsonValue.Create("planned"));

p.ProjectPlan.RequireNodeState("M2","done");
p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4",target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Work,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Phase,target?"in-progress":"not-ready");
p.ProjectPlan.RequireNodeState(Task,target?"done":"not-ready");
p.ProjectPlan.RequireNodeState("M4-W01-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M4-W01-C02","not-ready");
p.ProjectPlan.RequireNodeState("M4-W02","not-ready");
foreach(var node in new[]{Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(node,target?"done":"not-ready");

if(!target)
{
    foreach(var file in new[]{Qualified,Decision,Corpus,Inventory,
        AngouriProject,AngouriSource,MathNetProject,MathNetSource})
        p.Require.FileMissing(file);
}
else
{
    p.Require.JsonValueEquals(Qualified,"/status",
        JsonValue.Create("m4-two-managed-symbolic-provider-and-conditional-result-candidate-qualified"));
    p.Require.JsonValueEquals(Qualified,"/selected_candidate",
        JsonValue.Create("AngouriMath/2.5.0-provisional"));
    p.Require.JsonValueEquals(Qualified,"/math_product_symbolic_adapter_delivered",
        JsonValue.Create(false));
}

var digests=new Dictionary<string,string>(StringComparer.Ordinal);
foreach(var file in new[]{Decision,Corpus,Inventory,
    AngouriProject,AngouriSource,MathNetProject,MathNetSource})
{
    var receipt=p.Files.ReplaceFromStaged("staged/"+file,file);
    digests[file]=receipt.AfterSha256 ??
        throw new InvalidOperationException("Missing Payload SDK staged-file digest for "+file);
}
if(!target)
{
    // The spike runs candidate projects in distinct NuGet package caches and
    // processes. Neither is added to the Math product solution.
    var providerProjects=new[]{
        (Project:AngouriProject,Id:"AngouriMath",Version:"2.5.0",
            Cache:"m4-w01-c01-angouri-packages",Marker:AngouriMarker),
        (Project:MathNetProject,Id:"MathNet.Symbolics",Version:"0.25.0",
            Cache:"m4-w01-c01-mathnet-packages",Marker:MathNetMarker)
    };
    var packageArchives=new List<string>();
    foreach(var provider in providerProjects)
    {
        var projectXml=XDocument.Load(Path.Combine(root,provider.Project));
        var packageReferences=projectXml.Descendants()
            .Where(e=>e.Name.LocalName=="PackageReference").ToArray();
        if(packageReferences.Length!=1 ||
            (string?)packageReferences[0].Attribute("Include")!=provider.Id ||
            (string?)packageReferences[0].Attribute("Version")!="["+provider.Version+"]" ||
            projectXml.Descendants().Any(e=>e.Name.LocalName=="ProjectReference"))
            throw new InvalidOperationException("Provider spike package identity or source-only boundary drifted: "+provider.Id);
        var cache=Path.Combine(root,"build","artifacts",provider.Cache);
        Required(root,"dotnet","restore",provider.Project,"--packages",cache);
        Required(root,"dotnet","build",provider.Project,"-c","Release","--no-restore");
        var programOutput=Required(root,"dotnet","run","--project",provider.Project,
            "-c","Release","--no-build","--no-restore");
        if(!HasExactLine(programOutput,provider.Marker))
            throw new InvalidOperationException("Provider scientific smoke marker absent: "+provider.Id);
        var packagePath=Path.Combine(cache,provider.Id.ToLowerInvariant(),provider.Version,
            provider.Id.ToLowerInvariant()+"."+provider.Version+".nupkg");
        if(!File.Exists(packagePath))
            throw new InvalidOperationException("Exact restored provider archive is not accessible: "+provider.Id);
        packageArchives.Add(packagePath);
    }

    var helperArgs=new List<string>{"run","--file",ProviderHashHelper,"--"};
    helperArgs.AddRange(packageArchives);
    var scanned=Required(root,"dotnet",helperArgs.ToArray());
    var observed=new List<JsonNode>();
    foreach(var provider in providerProjects)
    {
        var meta=ReadPackageReceipt(scanned,provider.Id,provider.Version);
        observed.Add(new JsonObject{
            ["id"]=provider.Id,["version"]=provider.Version,
            ["license"]=meta.License,["bytes"]=meta.Bytes,
            ["sha256"]=meta.Sha,["direct_dependencies"]=p.Json.StringArray(meta.Dependencies),
            ["runtime_marker"]=provider.Marker,
            ["project"]=provider.Project,["source_project"]=provider.Project,
            ["source_file"]=provider.Id=="AngouriMath"?AngouriSource:MathNetSource,
            ["source_hash_sha256"]=digests[provider.Id=="AngouriMath"?AngouriSource:MathNetSource],
            ["mode"]="isolated_test_provider_not_product_reference"
        });
    }

    // Only the already qualified exact/IR foundations regress at this spike.
    // M4 has no product symbolics library yet and no release gate is due.
    Required(root,"dotnet","restore","dw.tools.math.slnx","--locked-mode");
    Required(root,"dotnet","build","dw.tools.math.slnx","-c","Release","--no-restore");
    foreach(var test in new[]{
        "tests/projects/dw.quantities.tests/dw.quantities.tests.csproj",
        "tests/projects/dw.tools.math.ir.tests/dw.tools.math.ir.tests.csproj"})
    {
        var testResult=Required(root,"dotnet","test",test,
            "-c","Release","--no-build","--no-restore",
            "--logger","console;verbosity=minimal");
        if(!testResult.Contains("Passed",StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Math foundation regression has no passing test receipt: "+test);
    }
    Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

    var proof=new JsonObject{
        ["schema_version"]=1,
        ["id"]="M4-W01-C01-T1-qualified",
        ["status"]="m4-two-managed-symbolic-provider-and-conditional-result-candidate-qualified",
        ["prior_M3_gate"]=Prior,
        ["question"]="Which pinned managed .NET CAS candidates can preserve x!=1 under rational cancellation and meet elementary derivative/root checks without forcing a symbolic provider into public Math assemblies?",
        ["observation"]="Two isolated provider-only C# smokes ran with pinned MIT NuGet packages; Angouri returned a condition-bearing cancellation, MathNet.Symbolics checked derivative and point-roots without claiming domain protection.",
        ["decision"]="Keep AngouriMath 2.5.0 as the provisional conditional-result candidate; retain MathNet.Symbolics as comparison only and defer external CAS, T2 limits and the symbolic IR bridge.",
        ["selected_candidate"]="AngouriMath/2.5.0-provisional",
        ["compared_candidate"]="MathNet.Symbolics/0.25.0-no-domain-contract",
        ["out_of_process_candidate"]="deferred-no-mandatory-Python",
        ["candidate_inventory"]=Inventory,["inventory_sha256"]=digests[Inventory],
        ["decision_document"]=Decision,["decision_sha256"]=digests[Decision],
        ["experimental_contract"]=Corpus,["corpus_sha256"]=digests[Corpus],
        ["observed_provider_archives"]=p.Json.Array(observed.ToArray()),
        ["managed_provider_tests_executed"]=2,
        ["provider_cache_isolated"]=true,
        ["angouri_conditional_exclusion_observed"]=true,
        ["mathnet_conditional_exclusion_admitted"]=false,
        ["angouri_elementary_polynomial_roots_observed"]=true,
        ["mathnet_root_solver_admitted"]=false,
        ["derivative_corpus"]="x^2-1 -> 2*x; derivative(3)=6",
        ["conditional_exclusion"]="(x^2-1)/(x-1) may cancel only with x!=1 retained",
        ["exact_and_ir_regressions_passed"]=true,
        ["product_solution_build_locked"]=true,
        ["transfer_architecture_passed"]=true,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["math_product_symbolic_adapter_delivered"]=false,
        ["next_accepted_scope"]="M4-W01-C01-T2 cancellation, budgets and assumption interoperability",
        ["nonclaims"]=p.Json.StringArray(
            "No public Math symbolics engine, IR translation, provider-independent conditional result or general symbolic proof is delivered.",
            "No arbitrary solver family, certified unexecuted RID, native library, Python executor, out-of-process CAS or broker is qualified.",
            "No cooperative interruption, hard deadline, arbitrary complexity, thread safety, external Decision adoption, neutral dw.discovery or agent skill is claimed.",
            "The observed external NuGet SHA-256s identify only restored archives, not portable binary rebuilds.")
    };
    p.Files.WriteComplete(Qualified,
        proof.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");

    p.Json.EditObject(Backlog,roadmap=>
    {
        roadmap["plan_version"]="0.1.52";
        var m4=roadmap["releases"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]=="M4");
        var work=roadmap["work_packages"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Work);
        var phase=roadmap["chunks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Phase);
        var task=phase["tasks"]!.AsArray().Select(x=>x!.AsObject())
            .Single(x=>(string?)x["id"]==Task);
        m4["status"]="in_progress";
        work["status"]="in_progress";
        phase["status"]="in_progress";
        phase["refinement"]="RS053 T1 compares two pinned isolated .NET 10 symbolic provider candidates: AngouriMath 2.5.0 checks a condition-carrying x!=1 rational cancellation, exact polynomial-derivative residual and elementary ±1 roots; MathNet.Symbolics 0.25.0 checks derivative values/point-roots but does not qualify exclusion preservation. Actual NuGet MIT metadata, direct dependencies, archive SHA-256s, C# compilation, runtime, exact/IR regressions and prohibition of product provider references are evidenced. Angouri is a provisional candidate; C01-T2 owns cancellation/interruption, domain and resource budgets, C02 owns typed IR adapters.";
        phase["readiness"]="in progress: provider-selection spike T1 qualified only; bounds/assumption boundary T2 planned; no public Math symbolic implementation";
        phase["files"]=p.Json.StringArray(Decision,Corpus,Inventory,
            AngouriProject,AngouriSource,MathNetProject,MathNetSource,Qualified);
        phase["commands"]=p.Json.StringArray(
            "dotnet restore "+AngouriProject,
            "dotnet run --project "+AngouriProject+" -c Release",
            "dotnet restore "+MathNetProject,
            "dotnet run --project "+MathNetProject+" -c Release");
        phase["evidence"]=p.Json.StringArray(Qualified);
        task["status"]="done";
        task["evidence"]=p.Json.StringArray(Qualified);
        foreach(var child in task["subtasks"]!.AsArray().Select(x=>x!.AsObject()))
        {
            child["status"]="done";
            child["evidence"]=p.Json.StringArray(Qualified);
        }
    });

    // M4 is the first active hierarchy after M3 closure. Activate each
    // ancestor explicitly, as in the qualified RS039 milestone-entry path.
    // ActivateReadyContinuation requires an already-active ancestor prefix.
    p.ProjectPlan.TransitionNode("M4","not-ready","ready");
    p.ProjectPlan.TransitionNode(Work,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Phase,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Task,"not-ready","ready");
    p.ProjectPlan.TransitionNode(Question,"not-ready","ready");
    foreach(var node in new[]{"M4",Work,Phase,Task,Question})
        p.ProjectPlan.TransitionNode(node,"ready","in-progress");
    p.ProjectPlan.ConvergeNodeToDone(Question);
    foreach(var child in new[]{Observation,DecisionNode})
    {
        p.ProjectPlan.TransitionNode(child,"not-ready","ready");
        p.ProjectPlan.TransitionNode(child,"ready","in-progress");
        p.ProjectPlan.ConvergeNodeToDone(child);
    }
    p.ProjectPlan.ConvergeNodeToDone(Task);
    p.Json.EditObject(".aura/workflow/state.json",state=>
    {
        if((string?)state["pendingRun"]!="RS053" ||
            state["activeMilestone"] is not null ||
            state["activeWorkPackage"] is not null ||
            state["activePhase"] is not null ||
            state["activeTask"] is not null ||
            state["activeSubtask"] is not null)
            throw new InvalidOperationException("RS053 expected empty active hierarchy before first M4 entry.");
        state["activeMilestone"]="M4";
        state["activeWorkPackage"]=Work;
        state["activePhase"]=Phase;
    });
    Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
}

p.ProjectPlan.RequireNodeState("M3","done");
p.ProjectPlan.RequireNodeState("M4","in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Phase,"in-progress");
p.ProjectPlan.RequireNodeState(Task,"done");
p.ProjectPlan.RequireNodeState("M4-W01-C01-T2","not-ready");
p.ProjectPlan.RequireNodeState("M4-W01-C02","not-ready");
foreach(var node in new[]{Question,Observation,DecisionNode})
    p.ProjectPlan.RequireNodeState(node,"done");
Required(root,"dotnet","run","--file","docs/planning/ValidateNativeDwfAdoption.cs");
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
return p.Complete();

static bool HasExactLine(string output,string expected)=>
    output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Any(line=>line.Trim()==expected);

static (long Bytes,string Sha,string License,string[] Dependencies) ReadPackageReceipt(
    string output,string name,string version)
{
    var prefix="M4-PROVIDER-ARCHIVE:"+name+":"+version+":";
    var matches=output.Split(new[]{'\r','\n'},StringSplitOptions.RemoveEmptyEntries)
        .Where(line=>line.StartsWith(prefix,StringComparison.Ordinal)).ToArray();
    if(matches.Length!=1)
        throw new InvalidOperationException("Missing/duplicate independent provider archive receipt: "+name);
    var bits=matches[0][prefix.Length..].Split(':');
    if(bits.Length!=4 ||
        !long.TryParse(bits[0],out var bytes)||bytes<=0 ||
        bits[1].Length!=64 || bits[1].Any(ch=>!((ch>='0'&&ch<='9')||(ch>='a'&&ch<='f'))) ||
        bits[2]!="MIT")
        throw new InvalidOperationException("Invalid restored NuGet archive observation for "+name);
    var dependencies=bits[3].Split(';',StringSplitOptions.RemoveEmptyEntries);
    if(dependencies.Length<1)
        throw new InvalidOperationException("Symbolic package direct dependencies were not observed for "+name);
    return(bytes,bits[1],bits[2],dependencies);
}
static string Relevant(string output)
{
    var index=output.IndexOf("Error Message:",StringComparison.Ordinal);
    if(index<0)index=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    var part=index<0?output:output[index..];
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
    if(!process.WaitForExit(360000))
    {
        process.Kill(entireProcessTree:true);
        throw new TimeoutException(executable+" exceeded six-minute provider execution window.");
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
