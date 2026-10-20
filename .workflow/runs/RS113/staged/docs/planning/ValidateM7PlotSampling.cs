#:property PublishAot=false
#:property NuGetAudit=false
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

const string Backlog="docs/planning/backlog.json";
const string Native=".aura/workflow/plan/project.json";
const string Prior="docs/planning/evidence/M7-W01-C03-S01-T2-qualified.json";
const string Split="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string Evidence="docs/planning/evidence/M7-W01-C03-S02-T1-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c03-s02-t1.md";
const string Screen="docs/planning/decisions/m7-w01-c03-s02-ownership.md";
const string Phase="M7-W01-C03-S02";
const string Task="M7-W01-C03-S02-T1";
const string Parent="M7-W01-C03";
const string S01="M7-W01-C03-S01";
const string Marker="dw.tools.math/m7-w01-c03-s02-t1-reciprocal-segments/1.2 qualified; segments=2; finite=4; pole-break=true; max-points=8; renderer=false";
const string Status="m7-c03-s02-t1-finite-reciprocal-segments-and-point-budget-qualified";
const int MaxSamples=8,MaxPoints=8,MaxSegments=2;

static void Need(bool ok,string reason)
{
    if(!ok)throw new InvalidOperationException("Independent PlotModel study: "+reason);
}
static string Local(string relative)
{
    Need(relative.StartsWith("docs/",StringComparison.Ordinal)||relative==Native,
        "Only local read-only planning/evidence paths are admitted.");
    Need(!Path.IsPathRooted(relative)&&!relative.Contains("..",StringComparison.Ordinal)&&
         !relative.Contains('\\'),"Unsafe local path "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string path)=>File.ReadAllText(Local(path));
static JsonDocument Open(string path)=>JsonDocument.Parse(Read(path));
static string Hash(string path)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(path))))
        .ToLowerInvariant();
static JsonElement Find(JsonElement nodes,string id)=>
    nodes.EnumerateArray().Single(x=>x.GetProperty("id").GetString()==id);
static string Text(JsonElement item,string property)=>
    item.GetProperty(property).GetString() ??
        throw new InvalidOperationException("Expected text field: "+property);
static void State(JsonElement item,string expected,string field="status")=>
    Need(Text(item,field)==expected,
        "Unexpected "+field+" for "+Text(item,"id")+" expected "+expected);
static SampleResult Refuse(string status)=>
    new(false,status,Array.Empty<SamplePoint[]>(),0,0,0);
static SampleResult ReciprocalSegments(double[] abscissae)
{
    if(abscissae.Length<2||abscissae.Length>MaxSamples)
        return Refuse("sample-budget");
    for(var i=0;i<abscissae.Length;i++)
    {
        if(!double.IsFinite(abscissae[i]) ||
           (i>0&&!(abscissae[i]>abscissae[i-1])))
            return Refuse("invalid-sample-domain");
    }
    // This is a deliberately finite reciprocal-only reference, not a renderer.
    var segments=new List<List<SamplePoint>>();
    List<SamplePoint>? current=null;
    double previousNonzero=0;
    var hasPrevious=false;
    var gapCount=0;
    foreach(var x in abscissae)
    {
        if(x==0.0)
        {
            gapCount++;
            current=null;
            hasPrevious=false;
            continue;
        }
        if(hasPrevious && previousNonzero<0.0 && x>0.0)
            current=null; // a skipped sample at zero must not bridge its known pole
        var y=1.0/x;
        if(!double.IsFinite(y))return Refuse("nonfinite-reciprocal");
        if(current is null)
        {
            current=new List<SamplePoint>();
            segments.Add(current);
            if(segments.Count>MaxSegments)return Refuse("segment-budget");
        }
        current.Add(new SamplePoint(x,y));
        previousNonzero=x;
        hasPrevious=true;
    }
    var points=segments.Sum(segment=>segment.Count);
    if(points==0||points>MaxPoints)return Refuse("point-budget");
    return new(true,"qualified",segments.Select(segment=>segment.ToArray()).ToArray(),
        points,gapCount,Math.Max(0,segments.Count-1));
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only read-only --check is supported.");
var decision=Read(Decision);
var owner=Read(Screen);
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "[-2, -1, 0, 1, 2]","four finite points","one pole gap",
    "two disconnected segments","[-2, -1, 1, 2]",
    "8-sample","no product PlotModel renderer","S02-T2","AURA"})
    Need(decision.Contains(term,StringComparison.OrdinalIgnoreCase),
        "Research Q/O/D decision missing: "+term);
foreach(var term in new[]{"## Independent reciprocal segment oracle",
    "## Responsibility matrix","## T2 admission / deferral criteria",
    "eight samples","source-free","AURA/client owner","no partial result",
    "1/x","segment","Not admitted"})
    Need(owner.Contains(term,StringComparison.OrdinalIgnoreCase),
        "Owner/sampling screen missing: "+term);

using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var priorDoc=Open(Prior);
using var splitDoc=Open(Split);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var prior=priorDoc.RootElement;
var split=splitDoc.RootElement;
var target=File.Exists(Local(Evidence));
Need(Text(backlog,"plan_version")==(target?"0.1.112":"0.1.111"),
    "Unexpected exact M7 PlotModel baseline/target version.");
Need(Text(prior,"status")=="m7-c03-s01-t2-owner-gated-pure-dag-runtime-deferral-qualified" &&
     prior.GetProperty("s01_phase_done").GetBoolean() &&
     !prior.GetProperty("plot_phase_done").GetBoolean() &&
     !prior.GetProperty("dag_runtime_api_delivered").GetBoolean() &&
     !prior.GetProperty("plot_renderer_delivered").GetBoolean() &&
     !prior.GetProperty("aura_job_engine_copied").GetBoolean(),
     "Prior S01 DAG deferral cannot justify a graph runtime or renderer.");
foreach(var (field,path) in new[]
{
    ("boundary_validator_sha256","docs/planning/ValidateM7DagBoundary.cs"),
    ("decision_document_sha256","docs/planning/decisions/m7-w01-c03-s01-t2.md"),
    ("distribution_document_sha256","docs/distribution/m7-pure-dag-deferral.md")
})
    Need(Text(prior,field)==Hash(path),"Immutable RS112 evidence SHA drift: "+path);
Need(Text(split,"status")=="m7-c03-bounded-dag-plotmodel-split-qualified" &&
     split.GetProperty("native_nodes_after").GetInt32()==541 &&
     split.GetProperty("parent_chunk_still_planned").GetBoolean() &&
     !split.GetProperty("dag_runtime_api_delivered").GetBoolean() &&
     !split.GetProperty("plot_renderer_delivered").GetBoolean(),
     "Original accepted size-L split and ownership authority drifted.");

State(Find(backlog.GetProperty("releases"),"M6"),"done");
State(Find(native.GetProperty("milestones"),"M6"),"done","state");
State(Find(backlog.GetProperty("releases"),"M7"),"in_progress");
State(Find(backlog.GetProperty("work_packages"),"M7-W01"),"in_progress");
State(Find(backlog.GetProperty("chunks"),S01),"done");
var productParent=Find(backlog.GetProperty("chunks"),Parent);
var productPlot=Find(backlog.GetProperty("chunks"),Phase);
State(productParent,"planned");
State(productPlot,target?"in_progress":"planned");
Need(Text(productParent,"size")=="L"&&
     Text(productPlot,"size")=="M"&&Text(productPlot,"kind")=="spike"&&
     Text(productPlot,"parent_chunk")==Parent,
     "Plot phase is no longer the independently admitted size-M research split.");
var productT1=Find(productPlot.GetProperty("tasks"),Task);
var productT2=Find(productPlot.GetProperty("tasks"),"M7-W01-C03-S02-T2");
State(productT1,target?"done":"planned");
State(productT2,"planned");
var m7=Find(native.GetProperty("milestones"),"M7");
var wp=Find(m7.GetProperty("workPackages"),"M7-W01");
State(m7,"in-progress","state");
State(wp,"in-progress","state");
State(Find(wp.GetProperty("phases"),Parent),"not-ready","state");
State(Find(wp.GetProperty("phases"),S01),"done","state");
var nativePlot=Find(wp.GetProperty("phases"),Phase);
State(nativePlot,target?"in-progress":"not-ready","state");
var nativeT1=Find(nativePlot.GetProperty("tasks"),Task);
var nativeT2=Find(nativePlot.GetProperty("tasks"),"M7-W01-C03-S02-T2");
State(nativeT1,target?"done":"not-ready","state");
State(nativeT2,"not-ready","state");
for(var i=0;i<3;i++)
{
    var suffix=((char)('A'+i)).ToString();
    State(Find(productT1.GetProperty("subtasks"),Task+"-"+suffix),
        target?"done":"planned");
    State(Find(nativeT1.GetProperty("subtasks"),Task+"-"+suffix),
        target?"done":"not-ready","state");
    State(Find(productT2.GetProperty("subtasks"),"M7-W01-C03-S02-T2-"+suffix),
        "planned");
    State(Find(nativeT2.GetProperty("subtasks"),"M7-W01-C03-S02-T2-"+suffix),
        "not-ready","state");
}

var reference=ReciprocalSegments([-2,-1,0,1,2]);
Need(reference.Success&&reference.Status=="qualified" &&
     reference.PointCount==4&&reference.PoleGaps==1&&
     reference.BreakCount==1&&reference.Segments.Length==2&&
     reference.Segments[0].Length==2&&reference.Segments[1].Length==2&&
     reference.Segments[0][0]==new SamplePoint(-2,-0.5)&&
     reference.Segments[0][1]==new SamplePoint(-1,-1)&&
     reference.Segments[1][0]==new SamplePoint(1,1)&&
     reference.Segments[1][1]==new SamplePoint(2,0.5),
     "The finite 1/x grid did not produce exactly two unconnected segments.");
var omitted=ReciprocalSegments([-2,-1,1,2]);
Need(omitted.Success&&omitted.Segments.Length==2&&omitted.PointCount==4&&
     omitted.PoleGaps==0&&omitted.BreakCount==1,
     "A grid skipping the pole must still avoid an invalid line across zero.");
Console.WriteLine("M7-PLOT-ORACLE:RECIPROCAL:FINITE=4;SEGMENTS=2;GAPS=1;BREAKS=1;ZERO-OMITTED-BREAK=true");

var edge=ReciprocalSegments([-4,-3,-2,-1,1,2,3,4]);
Need(edge.Success&&edge.PointCount==MaxPoints&&
     edge.Segments.Length==MaxSegments&&edge.BreakCount==1,
     "Exact admitted eight-point grid budget was not preserved.");
void MustRefuse(string expected,double[] grid)
{
    var result=ReciprocalSegments(grid);
    Need(!result.Success&&result.Status==expected&&
         result.PointCount==0&&result.Segments.Length==0&&
         result.BreakCount==0&&result.PoleGaps==0,
         "Plot failure '"+expected+"' must not publish partial points/segments.");
}
MustRefuse("sample-budget",[1]);
MustRefuse("sample-budget",[1,2,3,4,5,6,7,8,9]);
MustRefuse("invalid-sample-domain",[-1,-1,1]);
MustRefuse("invalid-sample-domain",[2,1]);
MustRefuse("invalid-sample-domain",[-1,double.NaN,1]);
MustRefuse("invalid-sample-domain",[-1,double.PositiveInfinity]);
MustRefuse("nonfinite-reciprocal",[double.Epsilon,1]);
Console.WriteLine("M7-PLOT-ORACLE:REFUSALS:MAX-SAMPLES=8;MAX-POINTS=8;MAX-SEGMENTS=2;NONFINITE=true;ORDER=true;PARTIAL=false");

if(target)
{
    using var evidenceDoc=Open(Evidence);
    var e=evidenceDoc.RootElement;
    Need(Text(e,"status")==Status &&
         e.GetProperty("independent_oracle_groups").GetInt32()==2 &&
         e.GetProperty("reference_samples").GetInt32()==5 &&
         e.GetProperty("reference_finite_points").GetInt32()==4 &&
         e.GetProperty("reference_segments").GetInt32()==2 &&
         e.GetProperty("reference_pole_gaps").GetInt32()==1 &&
         e.GetProperty("reference_breaks").GetInt32()==1 &&
         e.GetProperty("pole_omitted_breaks").GetInt32()==1 &&
         e.GetProperty("research_max_samples").GetInt32()==MaxSamples &&
         e.GetProperty("research_max_points").GetInt32()==MaxPoints &&
         e.GetProperty("research_max_segments").GetInt32()==MaxSegments &&
         e.GetProperty("no_partial_result_on_refusal").GetBoolean() &&
         e.GetProperty("pole_not_bridged").GetBoolean() &&
         e.GetProperty("t1_done").GetBoolean() &&
         !e.GetProperty("t2_done").GetBoolean() &&
         !e.GetProperty("s02_phase_done").GetBoolean() &&
         !e.GetProperty("parent_c03_done").GetBoolean() &&
         !e.GetProperty("public_plot_model_api_delivered").GetBoolean() &&
         !e.GetProperty("plot_renderer_delivered").GetBoolean() &&
         !e.GetProperty("aura_job_engine_copied").GetBoolean() &&
         Text(e,"oracle_validator_sha256")==Hash(
             "docs/planning/ValidateM7PlotSampling.cs") &&
         Text(e,"decision_document_sha256")==Hash(Decision) &&
         Text(e,"owner_screen_sha256")==Hash(Screen),
         "Completed plot study target differs from exact finite oracle/non-delivery evidence.");
}
Console.WriteLine(Marker);
return 0;

sealed record SamplePoint(double X,double Y);
sealed record SampleResult(bool Success,string Status,SamplePoint[][] Segments,
    int PointCount,int PoleGaps,int BreakCount);
