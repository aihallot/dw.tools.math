using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Dw.Tools.Workflow.Payloads;

const string Milestone="M6",Work="M6-W02",Parent="M6-W02-C03";
const string State=".aura/workflow/state.json",Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Validator="docs/planning/ValidateNativeDwfAdoption.cs";
const string Decision="docs/planning/decisions/m6-w02-c03-size-l-split.md";
const string Proof="docs/planning/evidence/M6-W02-C03-size-l-split-qualified.json";
const string Status="m6-w02-c03-size-l-split-native-topology-qualified-no-product";
const int TargetNodes=523;

var p=PayloadContext.Create();
var root=p.RepositoryRoot;
var target=File.Exists(Path.Combine(root,Proof));
p.Workflow.RequireCurrentRun("RS092",Milestone,Work,null,null,null);
p.Workflow.RequirePriorSuccess("RS091",1,
    "feat: qualify special-function angle Gamma and complex branch boundaries and close M6 W02 C02",
    "0.1.59",0,0,0,true);
p.Require.JsonValueEquals(
    "docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json",
    "/status",JsonValue.Create("bounded-managed-special-angle-gamma-branch-risk-audit-qualified"));
p.Require.JsonValueEquals(
    "docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json",
    "/m6_w02_c02_phase_done",JsonValue.Create(true));
p.Require.JsonValueBaselineOrTarget(Backlog,"/plan_version",
    JsonValue.Create("0.1.90"),JsonValue.Create("0.1.91"));
p.Require.JsonValueEquals(Backlog,"/plan_version",JsonValue.Create(target?"0.1.91":"0.1.90"));
p.Require.JsonValueEquals(Backlog,"/releases/6/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/work_packages/14/status",JsonValue.Create("in_progress"));
p.Require.JsonValueEquals(Backlog,"/chunks/43/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/44/status",JsonValue.Create("done"));
p.Require.JsonValueEquals(Backlog,"/chunks/45/id",JsonValue.Create(Parent));
p.Require.JsonValueEquals(Backlog,"/chunks/45/size",JsonValue.Create("L"));
p.Require.JsonValueEquals(Backlog,"/chunks/45/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/45/tasks/0/status",JsonValue.Create("planned"));
p.Require.JsonValueEquals(Backlog,"/chunks/45/tasks/1/status",JsonValue.Create("planned"));
p.Require.TextContains(Validator,target?"Require(expected.Count == 523,":
    "Require(expected.Count == 496,");
p.ProjectPlan.RequireNodeState("M5","done");
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W01","done");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState("M6-W02-C01","done");
p.ProjectPlan.RequireNodeState("M6-W02-C02","done");
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C04","not-ready");
p.ProjectPlan.RequireNodeState("M6-W02-C05","not-ready");

var specs=new[]
{
    new {
        Id="M6-W02-C03-S01", Title="Finite Euclidean geometry and projection",
        Size="M", DependsOn=new[]{"M6-W02-C02","M6-W01-C03"},
        Deliverables="Bounded finite 2D/3D Euclidean norms, distances and projections with explicit domain and finite budgets.",
        Acceptance="Norm of (3,4) is 5; distance (0,0) to (3,4) is 5; projection of (3,4) on (1,0) is (3,0).",
        Boundaries="Refuse zero projection axes, nonfinite values, dimension mismatch and non-Euclidean/geodesic interpretations; no implicit units.",
        TestPrefix="M6W02C03S01", FileStem="FiniteGeometry64"
    },
    new {
        Id="M6-W02-C03-S02", Title="Finite direct DFT and linear convolution",
        Size="M", DependsOn=new[]{"M6-W02-C03-S01"},
        Deliverables="Finite direct DFT and linearly indexed convolution with explicit sign, inverse normalization, and bounded allocations.",
        Acceptance="DFT impulse [1,0,0,0] is [1,1,1,1]; convolution [1,2] with [1,1] is [1,3,2].",
        Boundaries="Forward sign negative, inverse 1/N; distinguish direct DFT from FFT, linear versus circular convolution, aliasing and size budgets.",
        TestPrefix="M6W02C03S02", FileStem="FiniteSignal64"
    },
    new {
        Id="M6-W02-C03-S03", Title="Finite uniformly sampled time series",
        Size="M", DependsOn=new[]{"M6-W02-C03-S02"},
        Deliverables="Bounded uniformly sampled first differences and sliding arithmetic means with explicit sample interval and finite windows.",
        Acceptance="Differences of [1,3,6] are [2,3]; two-sample moving averages are [2,4.5].",
        Boundaries="No irregular-time resampling or forecasting; reject missing values, nonpositive sample intervals and oversized windows.",
        TestPrefix="M6W02C03S03", FileStem="FiniteUniformSeries64"
    }
};
var ids=specs.Select(s=>s.Id).ToArray();
if(ids.Length!=3||ids.Distinct(StringComparer.Ordinal).Count()!=3)
    throw new InvalidOperationException("Exactly three distinct split children required.");
if(!target)
{
    p.Require.FileMissing(Decision);
    p.Require.FileMissing(Proof);
}
else
{
    p.Require.JsonValueEquals(Proof,"/status",JsonValue.Create(Status));
    p.Require.JsonValueEquals(Proof,"/new_phase_count",JsonValue.Create(3));
    p.Require.JsonValueEquals(Proof,"/native_node_count",JsonValue.Create(TargetNodes));
    p.Require.JsonValueEquals(Proof,"/product_qualified",JsonValue.Create(false));
    p.Require.JsonValueEquals(Proof,"/parent_chunk_remains_planned",JsonValue.Create(true));
}

p.Files.ReplaceFromStaged("staged/"+Validator,Validator);
p.Files.ReplaceFromStaged("staged/"+Decision,Decision);

if(!target)
{
    p.Json.EditObject(Backlog,doc=>
    {
        var chunks=doc["chunks"]!.AsArray();
        var index=FindIndex(chunks,Parent);
        if(index!=45)throw new InvalidOperationException("C03 product parent moved from accepted index.");
        if(specs.Any(s=>ContainsId(chunks,s.Id)))
            throw new InvalidOperationException("A C03 split child already exists in the baseline.");
        var parent=chunks[index]!.AsObject();
        if((string?)parent["status"]!="planned"||(string?)parent["size"]!="L")
            throw new InvalidOperationException("Large C03 parent must remain planned.");
        parent["refinement"]="RS092 accepted three bounded planned child phases: S01 finite geometry, S02 direct DFT/linear convolution, S03 uniform time series. Original C03 identity, size-L aggregate T1/T2, requirements and dependent release gates are retained; no scientific product completion.";
        parent["readiness"]="planned; size-L split topology admitted with three size-M child phases; each child requires its own independent compiled RED/GREEN/verify run. Parent C03 becomes eligible for its aggregate gate only after all three children and original T1/T2 criteria pass.";
        for(var i=0;i<specs.Length;i++)
        {
            var s=specs[i];
            var child=parent.DeepClone().AsObject();
            child["id"]=s.Id;
            child["title"]=s.Title;
            child["size"]=s.Size;
            child["parent_chunk"]=Parent;
            child["depends_on"]=p.Json.StringArray(s.DependsOn);
            child["deliverables"]=s.Deliverables;
            child["acceptance"]=s.Acceptance;
            child["boundary_acceptance"]=s.Boundaries;
            child["status"]="planned";
            child["refinement"]="RS092 structural split candidate only; no code or product qualification.";
            child["readiness"]="planned; refine bounded exact source files, compiled red, independent tests, active runner and NuGet oracles before ready.";
            var source="src/projects/dw.tools.math.scientific/"+s.FileStem+".cs";
            var test="tests/projects/dw.tools.math.scientific.tests/"+s.TestPrefix+"Tests.cs";
            child["proposed_files"]=p.Json.StringArray(source,test);
            child["proposed_test_file"]=test;
            child["proposed_commands"]=p.Json.StringArray(
                "dotnet test tests/projects/dw.tools.math.scientific.tests/dw.tools.math.scientific.tests.csproj -c Release --filter FullyQualifiedName~"+s.TestPrefix);
            child["files"]=p.Json.StringArray();
            child["commands"]=p.Json.StringArray();
            child["evidence"]=p.Json.StringArray();
            var tasks=child["tasks"]!.AsArray();
            if(tasks.Count!=2)throw new InvalidOperationException("Each C03 split requires two accepted tasks.");
            for(var taskIndex=0;taskIndex<2;taskIndex++)
            {
                var task=tasks[taskIndex]!.AsObject();
                var taskId=s.Id+"-T"+(taskIndex+1);
                task["id"]=taskId;
                task["behavior"]=taskIndex==0?s.Deliverables:s.Boundaries;
                task["test_case"]=taskIndex==0?s.Acceptance:s.Boundaries;
                task["status"]="planned";
                task["evidence"]=p.Json.StringArray();
                var subs=task["subtasks"]!.AsArray();
                if(subs.Count!=3)throw new InvalidOperationException("Each split task requires three RED/GREEN/V children.");
                var suffixes=new[]{"R","G","V"};
                for(var subIndex=0;subIndex<3;subIndex++)
                {
                    var sub=subs[subIndex]!.AsObject();
                    sub["id"]=taskId+"-"+suffixes[subIndex];
                    sub["status"]="planned";
                    sub["evidence"]=p.Json.StringArray();
                    sub["description"]=suffixes[subIndex] switch
                    {
                        "R"=>"Observe a compiled independent RED for "+s.Acceptance,
                        "G"=>"Implement only the finite accepted scope: "+s.Deliverables,
                        _=>"Verify the independent finite oracles, explicit boundaries and retained Math regressions for "+taskId
                    };
                }
            }
            chunks.Insert(index+1+i,child);
        }
        var release=doc["releases"]!.AsArray().Select(n=>n!.AsObject())
            .Single(n=>(string?)n["id"]==Milestone);
        var required=release["required_chunks"]!.AsArray();
        var parentPosition=FindStringIndex(required,Parent);
        for(var i=0;i<ids.Length;i++)
            required.Insert(parentPosition+1+i,JsonValue.Create(ids[i]));
        doc["plan_version"]="0.1.91";
    });

    p.Json.EditObject(Native,native=>
    {
        var milestone=native["milestones"]!.AsArray().Select(n=>n!.AsObject())
            .Single(n=>(string?)n["id"]==Milestone);
        var work=milestone["workPackages"]!.AsArray().Select(n=>n!.AsObject())
            .Single(n=>(string?)n["id"]==Work);
        var phases=work["phases"]!.AsArray();
        var parentIndex=FindIndex(phases,Parent);
        if(parentIndex!=2)throw new InvalidOperationException("Native C03 parent moved.");
        if(specs.Any(s=>ContainsId(phases,s.Id)))
            throw new InvalidOperationException("A native split node already exists.");
        var original=phases[parentIndex]!.AsObject();
        if((string?)original["state"]!="not-ready")
            throw new InvalidOperationException("Native C03 L parent must remain not-ready.");
        for(var i=0;i<specs.Length;i++)
        {
            var s=specs[i];
            var phase=original.DeepClone().AsObject();
            phase["id"]=s.Id;
            phase["title"]=s.Title;
            phase["state"]="not-ready";
            phase["dependsOn"]=p.Json.StringArray(s.DependsOn);
            phase["completionCondition"]="Complete accepted split "+s.Id+
                " finite oracles and boundaries after independent RED/GREEN/V and NuGet evidence.";
            var tasks=phase["tasks"]!.AsArray();
            if(tasks.Count!=2)throw new InvalidOperationException("Native split task count drift.");
            for(var ti=0;ti<2;ti++)
            {
                var task=tasks[ti]!.AsObject();
                var taskId=s.Id+"-T"+(ti+1);
                task["id"]=taskId;
                task["state"]="not-ready";
                task["title"]=ti==0?"Contract and outcome":"Boundaries and integration";
                task["completionCondition"]="Satisfy accepted "+taskId+" oracle and evidence from docs/planning/backlog.json.";
                var subs=task["subtasks"]!.AsArray();
                if(subs.Count!=3)throw new InvalidOperationException("Native split subtask count drift.");
                var suffixes=new[]{"R","G","V"};
                for(var si=0;si<3;si++)
                {
                    var sub=subs[si]!.AsObject();
                    sub["id"]=taskId+"-"+suffixes[si];
                    sub["state"]="not-ready";
                    sub["title"]=suffixes[si] switch
                    {
                        "R"=>"RED","G"=>"GREEN",_=>"Verify and document"
                    };
                    sub["completionCondition"]="Satisfy accepted "+taskId+"-"+suffixes[si]+
                        " subtask in docs/planning/backlog.json.";
                }
            }
            phases.Insert(parentIndex+1+i,phase);
        }
    });
}
p.ProjectPlan.RequireNodeState(Milestone,"in-progress");
p.ProjectPlan.RequireNodeState(Work,"in-progress");
p.ProjectPlan.RequireNodeState(Parent,"not-ready");
foreach(var id in ids)
{
    p.ProjectPlan.RequireDirectChild(Work,id);
    p.ProjectPlan.RequireNodeState(id,"not-ready");
    foreach(var suffix in new[]{"T1","T2"})
    {
        var taskId=id+"-"+suffix;
        p.ProjectPlan.RequireNodeState(taskId,"not-ready");
        foreach(var sub in new[]{"R","G","V"})
            p.ProjectPlan.RequireNodeState(taskId+"-"+sub,"not-ready");
    }
}

Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--write");
Required(root,"dotnet","run","--file",Validator);
Required(root,"dotnet","run","--file","docs/planning/ValidatePlan.cs","--","--check");
Required(root,"dotnet","run","--file","docs/planning/ValidateTransferArchitecture.cs");

if(!target)
{
    var manifest=new JsonObject
    {
        ["schema_version"]=1,["id"]="M6-W02-C03-size-l-split-qualified",
        ["status"]=Status,
        ["prior_c02_qualified"]="docs/planning/evidence/M6-W02-C02-T2-boundaries-qualified.json",
        ["parent_chunk"]=Parent,
        ["parent_chunk_size"]="L",
        ["parent_chunk_remains_planned"]=true,
        ["new_child_phases"]=p.Json.StringArray(ids),
        ["new_phase_count"]=3,
        ["new_task_count"]=6,
        ["new_subtask_count"]=18,
        ["native_node_count"]=TargetNodes,
        ["native_phase_count"]=56,
        ["native_task_count"]=112,
        ["native_subtask_count"]=330,
        ["all_new_native_nodes_not_ready"]=true,
        ["requirements_and_dependencies_preserved"]=true,
        ["release_gate_includes_split_children"]=true,
        ["decision_document"]=Decision,
        ["independent_oracles"]=p.Json.StringArray(
            "S01: Euclidean norm(3,4)=5 and projection((3,4),(1,0))=(3,0)",
            "S02: DFT([1,0,0,0])=[1,1,1,1] and convolution([1,2],[1,1])=[1,3,2]",
            "S03: difference([1,3,6])=[2,3] and moving-average2([1,3,6])=[2,4.5]"),
        ["next_scope"]="RS093: independently qualify finite geometry S01 T1; retain separate S01 T2 and S02/S03 tasks.",
        ["product_qualified"]=false,
        ["scientific_tests_executed_for_split"]=false,
        ["publicly_published"]=false,
        ["dotnet_sdk"]=Required(root,"dotnet","--version").Trim(),
        ["observed_runtime_identifier"]=RuntimeInformation.RuntimeIdentifier,
        ["nonclaims"]=p.Json.StringArray(
            "This structural split delivered no geometry, DFT, convolution or series code.",
            "The parent C03 and all three child phases remain unfinished with no fabricated scientific acceptance.",
            "The prior C01/C02 packages and mathematical guarantees are unchanged.",
            "No public package, untested OS or downstream AURA/Decision/MCDM adoption is certified.")
    };
    p.Files.WriteComplete(Proof,
        manifest.ToJsonString(new JsonSerializerOptions{WriteIndented=true})+"\n");
}
p.Require.JsonValueEquals(Proof,"/status",JsonValue.Create(Status));
p.Require.JsonValueEquals(Proof,"/native_node_count",JsonValue.Create(TargetNodes));
p.Require.JsonValueEquals(Proof,"/product_qualified",JsonValue.Create(false));
p.Json.EditObject(State,doc=>
{
    if((string?)doc["pendingRun"]!="RS092"||
       (string?)doc["activeMilestone"]!=Milestone||
       (string?)doc["activeWorkPackage"]!=Work||
       doc["activePhase"] is not null||
       doc["activeTask"] is not null||
       doc["activeSubtask"] is not null)
        throw new InvalidOperationException("RS092 structural split has an unexpected DWF cursor.");
});
return p.Complete();

static bool ContainsId(JsonArray items,string id)=>
    items.Any(node=>(string?)node?.AsObject()["id"]==id);
static int FindIndex(JsonArray items,string id)
{
    for(var i=0;i<items.Count;i++)
        if((string?)items[i]?.AsObject()["id"]==id)return i;
    throw new InvalidOperationException("Required accepted structural id not present: "+id);
}
static int FindStringIndex(JsonArray items,string id)
{
    for(var i=0;i<items.Count;i++)
        if((string?)items[i]==id)return i;
    throw new InvalidOperationException("Required release gate identity not present: "+id);
}
static string Relevant(string output)
{
    var i=output.IndexOf("error ",StringComparison.OrdinalIgnoreCase);
    if(i<0)i=output.IndexOf("invalid:",StringComparison.OrdinalIgnoreCase);
    var text=i<0?output:output[i..];
    return text.Length>2200?text[..2200]:text;
}
static string Run(string root,string executable,params string[] args)
{
    using var p=new Process{StartInfo=new ProcessStartInfo
    {
        FileName=executable,WorkingDirectory=root,UseShellExecute=false,
        RedirectStandardOutput=true,RedirectStandardError=true
    }};
    p.StartInfo.Environment["DOTNET_NOLOGO"]="1";
    p.StartInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"]="1";
    foreach(var arg in args)p.StartInfo.ArgumentList.Add(arg);
    if(!p.Start())throw new InvalidOperationException("Cannot start "+executable);
    var outTask=p.StandardOutput.ReadToEndAsync();
    var errTask=p.StandardError.ReadToEndAsync();
    if(!p.WaitForExit(300000))
    {
        p.Kill(entireProcessTree:true);
        throw new TimeoutException("Bounded split validator timed out.");
    }
    var text=outTask.GetAwaiter().GetResult()+"\n"+errTask.GetAwaiter().GetResult();
    Console.WriteLine(text);
    if(p.ExitCode!=0)
        throw new InvalidOperationException(executable+" "+string.Join(" ",args)+
            " failed: "+Relevant(text));
    return text;
}
static string Required(string root,string executable,params string[] args) =>
    Run(root,executable,args);
