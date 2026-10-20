#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Evidence="docs/planning/evidence/M7-W01-work-package-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-research-closure.md";
const string Distribution="docs/distribution/m7-w01-advanced-research-matrix.md";
const string C01T1="docs/planning/evidence/M7-W01-C01-T1-qualified.json";
const string C01T2="docs/planning/evidence/M7-W01-C01-T2-qualified.json";
const string C02T1="docs/planning/evidence/M7-W01-C02-T1-qualified.json";
const string C02T2="docs/planning/evidence/M7-W01-C02-T2-qualified.json";
const string S01T1="docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json";
const string S01T2="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string S02T1="docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json";
const string S02T2="docs/planning/evidence/M7-W01-C03-S02-T2-qualified.json";
const string C03T1="docs/planning/evidence/M7-W01-C03-T1-qualified.json";
const string C03T2="docs/planning/evidence/M7-W01-C03-T2-qualified.json";
const string Status="m7-w01-advanced-research-portfolio-qualified-products-deferred-unpublished";
const string Marker="dw.tools.math/m7-w01-advanced-research-closure/1.2 qualified; phases=5; tasks=10; qod=30; families=4; product=false; m7=open";

static void Need(bool okay,string reason)
{
    if(!okay)throw new InvalidOperationException("M7 advanced work-package research gate: "+reason);
}
static string Local(string path)
{
    Need(path.StartsWith("docs/",StringComparison.Ordinal)||path==Native,
        "Only read-only local Math planning and immutable evidence paths are accepted.");
    Need(!Path.IsPathRooted(path)&&!path.Contains("..",StringComparison.Ordinal)&&
        !path.Contains('\\'),"Unsafe source path "+path);
    return Path.Combine(Directory.GetCurrentDirectory(),
        path.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Sha(string path)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(path))))
        .ToLowerInvariant();
static JsonElement ById(JsonElement nodes,string id)=>
    nodes.EnumerateArray().Single(n=>n.GetProperty("id").GetString()==id);
static string Get(JsonElement item,string property)=>
    item.GetProperty(property).GetString()??
        throw new InvalidOperationException("Missing string field "+property);
static bool Flag(JsonElement item,string property)=>
    item.GetProperty(property).GetBoolean();
static void State(JsonElement item,string expected,string field="status")=>
    Need(Get(item,field)==expected,
        "Expected "+expected+" "+field+" for "+Get(item,"id"));
static void Pins(JsonElement receipt,params (string Field,string Path)[] files)
{
    foreach(var (field,path) in files)
        Need(Get(receipt,field)==Sha(path),"Previously qualified source SHA drift: "+path);
}
static void Contains(string source,string token,string label)=>
    Need(source.Contains(token,StringComparison.OrdinalIgnoreCase),
        label+" missing "+token);
static int Count(JsonElement node)
{
    var count=1;
    foreach(var property in new[]{"workPackages","phases","tasks","subtasks"})
        if(node.TryGetProperty(property,out var children))
            count+=children.EnumerateArray().Sum(Count);
    return count;
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check is supported.");
using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var c01aDoc=Open(C01T1);
using var c01bDoc=Open(C01T2);
using var c02aDoc=Open(C02T1);
using var c02bDoc=Open(C02T2);
using var s01aDoc=Open(S01T1);
using var s01bDoc=Open(S01T2);
using var s02aDoc=Open(S02T1);
using var s02bDoc=Open(S02T2);
using var c03aDoc=Open(C03T1);
using var c03bDoc=Open(C03T2);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var c01a=c01aDoc.RootElement;
var c01b=c01bDoc.RootElement;
var c02a=c02aDoc.RootElement;
var c02b=c02bDoc.RootElement;
var s01a=s01aDoc.RootElement;
var s01b=s01bDoc.RootElement;
var s02a=s02aDoc.RootElement;
var s02b=s02bDoc.RootElement;
var c03a=c03aDoc.RootElement;
var c03b=c03bDoc.RootElement;
var target=File.Exists(Local(Evidence));

Need(Get(backlog,"plan_version")==(target?"0.1.116":"0.1.115"),
    "Exact RS116 research baseline or RS117 target version not found.");
State(ById(backlog.GetProperty("releases"),"M6"),"done");
State(ById(native.GetProperty("milestones"),"M6"),"done","state");
State(ById(backlog.GetProperty("releases"),"M7"),"in_progress");
State(ById(backlog.GetProperty("work_packages"),"M7-W02"),"planned");
var wp=ById(backlog.GetProperty("work_packages"),"M7-W01");
State(wp,target?"done":"in_progress");
var milestone=ById(native.GetProperty("milestones"),"M7");
var nativeWork=ById(milestone.GetProperty("workPackages"),"M7-W01");
State(milestone,"in-progress","state");
State(nativeWork,target?"done":"in-progress","state");
State(ById(milestone.GetProperty("workPackages"),"M7-W02"),"not-ready","state");
var ids=new[]{"M7-W01-C01","M7-W01-C02","M7-W01-C03",
    "M7-W01-C03-S01","M7-W01-C03-S02"};
var researchedTasks=0;
var researchedQod=0;
foreach(var id in ids)
{
    var chunk=ById(backlog.GetProperty("chunks"),id);
    var phase=ById(nativeWork.GetProperty("phases"),id);
    State(chunk,"done");
    State(phase,"done","state");
    Need(Get(chunk,"kind")=="spike" &&
        chunk.GetProperty("tasks").GetArrayLength()==2 &&
        phase.GetProperty("tasks").GetArrayLength()==2,
        "Each W01 phase must preserve two accepted research tasks: "+id);
    for(var j=1;j<=2;j++)
    {
        var taskId=id+"-T"+j;
        var task=ById(chunk.GetProperty("tasks"),taskId);
        var nativeTask=ById(phase.GetProperty("tasks"),taskId);
        State(task,"done");
        State(nativeTask,"done","state");
        researchedTasks++;
        Need(task.GetProperty("subtasks").GetArrayLength()==3 &&
            nativeTask.GetProperty("subtasks").GetArrayLength()==3,
            "Each research task must preserve Q/O/D evidence: "+taskId);
        foreach(var suffix in new[]{"A","B","C"})
        {
            State(ById(task.GetProperty("subtasks"),taskId+"-"+suffix),"done");
            State(ById(nativeTask.GetProperty("subtasks"),taskId+"-"+suffix),
                "done","state");
            researchedQod++;
        }
    }
}
Need(researchedTasks==10&&researchedQod==30,
    "Work-package closure requires exactly 10 done tasks and 30 Q/O/D subtasks.");
Need(native.GetProperty("milestones").EnumerateArray().Sum(Count)==541,
    "Original 541-node DWF roadmap must not be silently reduced.");

Need(Get(c01a,"status")=="m7-c01-t1-bcl-exact-precision-interval-oracles-provider-screen-qualified" &&
    c01a.GetProperty("exact_integer_result").GetString()=="1" &&
    c01a.GetProperty("observed_binary64_cancellation").GetString()=="0" &&
    Flag(c01a,"reference_tenth_enclosed") &&
    !Flag(c01a,"public_precision_api_delivered"),
    "C01 precision BCL oracle baseline drifted.");
Need(Get(c01b,"status")=="m7-c01-t2-precision-provider-deferral-and-interval-boundaries-qualified" &&
    Get(c01b,"disposition")=="defer-new-precision-interval-provider" &&
    Flag(c01b,"m7_w01_c01_parent_done") &&
    !Flag(c01b,"external_provider_adopted") &&
    !Flag(c01b,"public_precision_api_delivered"),
    "C01 precision provider must remain explicitly deferred.");
Need(Get(c02a,"status")=="m7-c02-t1-dense-shape-oracles-and-provider-screen-qualified" &&
    c02a.GetProperty("reference_sum").GetInt32()==36 &&
    c02a.GetProperty("reference_index_value").GetInt32()==6 &&
    c02a.GetProperty("research_element_budget").GetInt32()==64 &&
    c02a.GetProperty("research_byte_budget").GetInt32()==512 &&
    Flag(c02a,"same_element_count_different_shape_refused") &&
    !Flag(c02a,"advanced_tensor_api_delivered"),
    "C02 tensor shape reference or refusal drifted.");
Need(Get(c02b,"status")=="m7-c02-t2-tensor-provider-deferral-and-shape-bounds-qualified" &&
    Get(c02b,"disposition")=="defer-unqualified-tensor-api-and-providers" &&
    Flag(c02b,"m7_w01_c02_parent_done") &&
    !Flag(c02b,"external_tensor_provider_adopted") &&
    !Flag(c02b,"advanced_tensor_api_delivered"),
    "C02 tensor runtime/provider must remain deferred.");
Need(Get(s01a,"status")=="m7-c03-s01-t1-pure-diamond-dag-cycle-and-budget-research-qualified" &&
    s01a.GetProperty("reference_root_value").GetInt32()==13 &&
    s01a.GetProperty("reference_shared_evaluations").GetInt32()==1 &&
    Flag(s01a,"cycle_refused") && Flag(s01a,"no_partial_result_on_refusal") &&
    !Flag(s01a,"dag_runtime_api_delivered"),
    "Pure graph reference or refusal drifted.");
Need(Get(s01b,"status")=="m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified" &&
    Get(s01b,"disposition")=="defer-unqualified-pure-dag-runtime-and-providers" &&
    Flag(s01b,"s01_phase_done")&&!Flag(s01b,"dag_runtime_api_delivered") &&
    !Flag(s01b,"aura_job_engine_copied"),
    "Graph runtime and AURA scheduler must remain deferred.");
Need(Get(s02a,"status")=="m7-c03-s02-t1-finite-reciprocal-segments-and-point-budget-qualified" &&
    s02a.GetProperty("reference_finite_points").GetInt32()==4 &&
    s02a.GetProperty("reference_segments").GetInt32()==2 &&
    Flag(s02a,"pole_not_bridged")&&Flag(s02a,"no_partial_result_on_refusal") &&
    !Flag(s02a,"public_plot_model_api_delivered"),
    "Finite reciprocal segment research evidence drifted.");
Need(Get(s02b,"status")=="m7-c03-s02-t2-owner-gated-plotmodel-api-deferral-qualified" &&
    Get(s02b,"disposition")=="defer-unqualified-plotmodel-and-rendering-provider" &&
    Flag(s02b,"s02_phase_done")&&!Flag(s02b,"public_plot_model_api_delivered") &&
    !Flag(s02b,"plot_renderer_delivered"),
    "PlotModel and renderer must remain deferred.");
Need(Get(c03a,"status")=="m7-c03-t1-aggregate-pure-dag-and-reciprocal-plot-research-qualified" &&
    c03a.GetProperty("prior_receipts_verified").GetInt32()==4 &&
    Flag(c03a,"children_both_done")&&!Flag(c03a,"parent_c03_done"),
    "Original C03 aggregate scientific T1 evidence changed.");
Need(Get(c03b,"status")=="m7-c03-t2-original-aggregate-owner-gated-research-closure-qualified" &&
    Get(c03b,"disposition")=="research-complete-products-deferred" &&
    Flag(c03b,"parent_c03_done") &&
    !Flag(c03b,"public_dag_api_delivered") &&
    !Flag(c03b,"public_plot_model_api_delivered") &&
    !Flag(c03b,"renderer_delivered") &&
    !Flag(c03b,"aura_job_engine_copied"),
    "Original C03 research closure must not claim runtime delivery.");

Pins(c01a,
    ("oracle_validator_sha256","docs/planning/ValidateM7PrecisionAdmission.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c01.md"),
    ("provider_screen_sha256","docs/planning/decisions/m7-w01-c01-provider-screen.md"));
Pins(c01b,
    ("boundary_validator_sha256","docs/planning/ValidateM7PrecisionBoundary.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c01-t2.md"),
    ("distribution_document_sha256","docs/distribution/m7-precision-interval-deferral.md"));
Pins(c02a,
    ("oracle_validator_sha256","docs/planning/ValidateM7TensorAdmission.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c02.md"),
    ("provider_screen_sha256","docs/planning/decisions/m7-w01-c02-provider-screen.md"));
Pins(c02b,
    ("boundary_validator_sha256","docs/planning/ValidateM7TensorBoundary.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c02-t2.md"),
    ("distribution_document_sha256","docs/distribution/m7-tensor-shape-deferral.md"));
Pins(s01a,
    ("oracle_validator_sha256","docs/planning/ValidateM7DagAdmission.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c03-s01-t1.md"),
    ("owner_screen_sha256","docs/planning/decisions/m7-w01-c03-s01-owner-screen.md"));
Pins(s01b,
    ("boundary_validator_sha256","docs/planning/ValidateM7DagBoundary.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c03-s01-t2.md"),
    ("distribution_document_sha256","docs/distribution/m7-pure-dag-deferral.md"));
Pins(s02a,
    ("oracle_validator_sha256","docs/planning/ValidateM7PlotSampling.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c03-s02-t1.md"),
    ("owner_screen_sha256","docs/planning/decisions/m7-w01-c03-s02-ownership.md"));
Pins(s02b,
    ("boundary_validator_sha256","docs/planning/ValidateM7PlotBoundary.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c03-s02-t2.md"),
    ("distribution_document_sha256","docs/distribution/m7-plotmodel-deferral.md"));
Pins(c03a,
    ("aggregate_validator_sha256","docs/planning/ValidateM7DagPlotAggregate.cs"),
    ("decision_sha256","docs/planning/decisions/m7-w01-c03-t1.md"),
    ("distribution_sha256","docs/distribution/m7-dag-plot-aggregate-screen.md"));
Pins(c03b,
    ("closure_validator_sha256","docs/planning/ValidateM7DagPlotClosure.cs"),
    ("decision_sha256","docs/planning/decisions/m7-w01-c03-t2.md"),
    ("distribution_sha256","docs/distribution/m7-dag-plot-closure.md"));

var decision=Read(Decision);
var matrix=Read(Distribution);
foreach(var term in new[]{"## Question","## Verified observations",
    "## Decision","Four mandatory future","five", "10 tasks","30 Q/O/D",
    "No public advanced Math product", "M7-W02","541 nodes"})
    Contains(decision,term,"M7-W01 research decision");
foreach(var term in new[]{"## Family-level evidence",
    "## Future review triggers","## Completed versus pending",
    "541", "source-free", "No provider is automatically admitted",
    "precision","tensor","DAG","PlotModel"})
    Contains(matrix,term,"M7-W01 distribution matrix");
Need(matrix.Split('\n').Count(x=>x.Contains("| **Defer",StringComparison.Ordinal))==4,
    "Four advanced API/provider families must be deferred, not silently adopted.");

// Independent hand-checkable BCL references, not production implementations.
BigInteger exactBase=BigInteger.Parse("10000000000000000");
var exact=(exactBase+BigInteger.One)-exactBase;
double roundedBase=1e16;
var rounded=(roundedBase+1d)-roundedBase;
var tensor=new[]{1,2,3,4,5,6,7,8};
var tensorIndex=((1*2)+0)*2+1;
var dag=(2+3)+(2*4);
var negative=new[]{-2d,-1d}.Select(x=>1d/x).ToArray();
var positive=new[]{1d,2d}.Select(x=>1d/x).ToArray();
Need(exact==BigInteger.One&&rounded==0d &&
     tensor.Sum()==36&&tensor[tensorIndex]==6 &&
     dag==13&&negative.SequenceEqual(new[]{-0.5d,-1d}) &&
     positive.SequenceEqual(new[]{1d,0.5d}),
     "Independent advanced research scalar/shape/graph/plot references drifted.");
Console.WriteLine("M7-ADVANCED-PORTFOLIO-ORACLE:PRECISION=1/0;TENSOR=36/6;DAG=13;PLOT=4/2");

if(target)
{
    using var targetDoc=Open(Evidence);
    var e=targetDoc.RootElement;
    Need(Get(e,"status")==Status &&
        e.GetProperty("research_phases_done").GetInt32()==5 &&
        e.GetProperty("research_tasks_done").GetInt32()==10 &&
        e.GetProperty("research_subtasks_done").GetInt32()==30 &&
        e.GetProperty("source_pinned_receipts").GetInt32()==10 &&
        e.GetProperty("native_nodes_retained").GetInt32()==541 &&
        e.GetProperty("review_family_count").GetInt32()==4 &&
        Flag(e,"precision_provider_deferred")&&Flag(e,"tensor_provider_deferred") &&
        Flag(e,"dag_runtime_deferred")&&Flag(e,"plot_model_deferred") &&
        Flag(e,"m7_w01_work_package_done") &&
        !Flag(e,"m7_w02_work_package_done")&&!Flag(e,"m7_release_done") &&
        !Flag(e,"new_advanced_product_delivered") &&
        !Flag(e,"publicly_published")&&!Flag(e,"aura_job_engine_copied") &&
        Get(e,"decision_sha256")==Sha(Decision) &&
        Get(e,"distribution_sha256")==Sha(Distribution) &&
        Get(e,"validator_sha256")==Sha("docs/planning/ValidateM7AdvancedWorkPackage.cs"),
        "Completed W01 target evidence would overclaim product or lack exact source pins.");
    var evidencePaths=wp.GetProperty("evidence").EnumerateArray()
        .Select(x=>x.GetString() ?? "").ToArray();
    Need(evidencePaths.Length==11 &&
        evidencePaths.Contains(Evidence,StringComparer.Ordinal) &&
        evidencePaths.Distinct(StringComparer.Ordinal).Count()==11,
        "Target work package must retain ten task receipts plus independent portfolio evidence.");
}
Console.WriteLine(Marker);
return 0;
