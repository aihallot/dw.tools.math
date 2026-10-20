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
const string Prior="docs/planning/evidence/M7-W01-C03-split-qualified.json";
const string Tensor="docs/planning/evidence/M7-W01-C02-T2-qualified.json";
const string Decision="docs/planning/decisions/m7-w01-c03-s01-t1.md";
const string Screen="docs/planning/decisions/m7-w01-c03-s01-owner-screen.md";
const string Evidence="docs/planning/evidence/M7-W01-C03-S01-T1-qualified.json";
const string Marker="dw.tools.math/m7-w01-c03-s01-t1-pure-dag-screen/1.2 qualified; diamond=13; shared=once; cycle=refused; limits=8/10/4; runtime=false";
const string Status="m7-c03-s01-t1-pure-diamond-dag-cycle-and-budget-research-qualified";
const int MaximumNodes=8,MaximumEdges=10,MaximumDepth=4;

static void Need(bool okay,string reason)
{
    if(!okay)throw new InvalidOperationException("Independent M7 DAG study: "+reason);
}
static string Local(string relative)
{
    Need(relative.StartsWith("docs/",StringComparison.Ordinal) ||
         relative==Native,"The local research oracle reads only Math planning authorities.");
    Need(!Path.IsPathRooted(relative)&&!relative.Contains("..",StringComparison.Ordinal)&&
         !relative.Contains('\\'),"Unsafe research path "+relative);
    return Path.Combine(Directory.GetCurrentDirectory(),
        relative.Replace('/',Path.DirectorySeparatorChar));
}
static string Read(string relative)=>File.ReadAllText(Local(relative));
static JsonDocument Open(string relative)=>JsonDocument.Parse(Read(relative));
static string Hash(string relative)=>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(Local(relative))))
        .ToLowerInvariant();
static JsonElement Find(JsonElement collection,string id)=>
    collection.EnumerateArray().Single(item=>item.GetProperty("id").GetString()==id);
static string Text(JsonElement obj,string property)=>obj.GetProperty(property).GetString() ??
    throw new InvalidOperationException("Missing string property: "+property);
static void RequireState(JsonElement obj,string expected,string property="status")=>
    Need(Text(obj,property)==expected,"Unexpected "+property+" for "+Text(obj,"id"));
static DagResult Refused(string status)=>
    new(false,0,0,0,new Dictionary<string,long>(StringComparer.Ordinal),status);
static DagResult Evaluate(DagNode[] nodes,string root)
{
    if(nodes.Length==0||nodes.Length>MaximumNodes)return Refused("node-budget");
    var map=new Dictionary<string,DagNode>(StringComparer.Ordinal);
    var edges=0;
    foreach(var node in nodes)
    {
        if(string.IsNullOrWhiteSpace(node.Id)||!map.TryAdd(node.Id,node))
            return Refused("duplicate-or-empty-id");
        edges+=node.Dependencies.Length;
        if(edges>MaximumEdges)return Refused("edge-budget");
        if(node.Dependencies.Distinct(StringComparer.Ordinal).Count()!=node.Dependencies.Length)
            return Refused("duplicate-dependency");
        if(node.Kind switch
        {
            "constant" => node.Dependencies.Length!=0,
            "add" or "multiply" => node.Dependencies.Length!=1,
            "sum" => node.Dependencies.Length!=2,
            _ => true
        })return Refused("unsupported-operation-shape");
    }
    if(!map.ContainsKey(root))return Refused("unknown-root");
    // Refuse any cycle or unknown dependency before arithmetic work; disconnected
    // invalid nodes cannot be hidden by choosing another requested root.
    var colors=new Dictionary<string,int>(StringComparer.Ordinal);
    var ordered=new List<DagNode>();
    bool Visit(string id)
    {
        if(!map.TryGetValue(id,out var node))return false;
        if(colors.TryGetValue(id,out var color))
            return color==2;
        colors[id]=1;
        foreach(var dependency in node.Dependencies)
            if(!Visit(dependency))return false;
        colors[id]=2;
        ordered.Add(node);
        return true;
    }
    foreach(var node in nodes)
        if(!Visit(node.Id))return Refused("dependency-or-cycle");
    var heights=new Dictionary<string,int>(StringComparer.Ordinal);
    foreach(var node in ordered)
    {
        var depth=1;
        foreach(var dependency in node.Dependencies)
            depth=Math.Max(depth,checked(heights[dependency]+1));
        if(depth>MaximumDepth)return Refused("depth-budget");
        heights.Add(node.Id,depth);
    }
    var values=new Dictionary<string,long>(StringComparer.Ordinal);
    try
    {
        foreach(var node in ordered)
        {
            long value=node.Kind switch
            {
                "constant" => node.Operand,
                "add" => checked(values[node.Dependencies[0]]+node.Operand),
                "multiply" => checked(values[node.Dependencies[0]]*node.Operand),
                "sum" => checked(values[node.Dependencies[0]]+values[node.Dependencies[1]]),
                _ => throw new InvalidOperationException("Operation preflight drift.")
            };
            values.Add(node.Id,value);
        }
    }
    catch(OverflowException)
    {
        // No partial numeric cache is returned to the caller.
        return Refused("checked-overflow");
    }
    return new(true,values[root],values.Count,heights[root],values,"qualified");
}

Need(args.Length==0||(args.Length==1&&args[0]=="--check"),
    "Only the read-only --check mode is supported.");
var decision=Read(Decision);
var ownerScreen=Read(Screen);
foreach(var term in new[]{"## Question","## Observation","## Decision",
    "shared exactly once","root=left+right=13","8 nodes","10 dependency edges",
    "4 nodes","directed cycle","no public DAG","T2","AURA"})
    Need(decision.Contains(term,StringComparison.OrdinalIgnoreCase),
        "Q/O/D DAG research decision missing: "+term);
foreach(var term in new[]{"## Independent diamond and refusal matrix",
    "## Ownership and provider decision","## Subsequent T2 admission criteria",
    "8 nodes","10 edges","4 depth","source-free consumer",
    "AURA ownership","no partial result"})
    Need(ownerScreen.Contains(term,StringComparison.OrdinalIgnoreCase),
        "DAG owner/budget screen missing: "+term);

using var backlogDoc=Open(Backlog);
using var nativeDoc=Open(Native);
using var splitDoc=Open(Prior);
using var tensorDoc=Open(Tensor);
var backlog=backlogDoc.RootElement;
var native=nativeDoc.RootElement;
var split=splitDoc.RootElement;
var tensor=tensorDoc.RootElement;
var target=File.Exists(Local(Evidence));
Need(Text(backlog,"plan_version")==(target?"0.1.110":"0.1.109"),
    "Unexpected accepted RS110 DAG research baseline or RS111 target.");
Need(Text(split,"status")=="m7-c03-bounded-dag-plotmodel-split-qualified" &&
     split.GetProperty("new_phase_count").GetInt32()==2 &&
     split.GetProperty("native_nodes_after").GetInt32()==541 &&
     split.GetProperty("dag_split_active").GetBoolean() &&
     !split.GetProperty("dag_oracles_executed").GetBoolean() &&
     !split.GetProperty("dag_runtime_api_delivered").GetBoolean() &&
     !split.GetProperty("plot_renderer_delivered").GetBoolean(),
    "The independently prepared split does not admit a DAG implementation.");
Need(Text(tensor,"status")=="m7-c02-t2-tensor-provider-deferral-and-shape-bounds-qualified" &&
     tensor.GetProperty("m7_w01_c02_parent_done").GetBoolean() &&
     !tensor.GetProperty("advanced_tensor_api_delivered").GetBoolean(),
    "Previous tensor owner boundary was not retained.");
RequireState(Find(backlog.GetProperty("releases"),"M7"),"in_progress");
RequireState(Find(backlog.GetProperty("work_packages"),"M7-W01"),"in_progress");
RequireState(Find(backlog.GetProperty("chunks"),"M7-W01-C02"),"done");
RequireState(Find(backlog.GetProperty("chunks"),"M7-W01-C03"),"planned");
var s01=Find(backlog.GetProperty("chunks"),"M7-W01-C03-S01");
var s02=Find(backlog.GetProperty("chunks"),"M7-W01-C03-S02");
RequireState(s01,"in_progress");
RequireState(s02,"planned");
Need(Text(s01,"parent_chunk")=="M7-W01-C03" &&
     Text(s01,"size")=="M"&&Text(s01,"kind")=="spike",
     "DAG split must remain a bounded size-M research phase.");
var t1=Find(s01.GetProperty("tasks"),"M7-W01-C03-S01-T1");
var t2=Find(s01.GetProperty("tasks"),"M7-W01-C03-S01-T2");
RequireState(t1,target?"done":"planned");
RequireState(t2,"planned");
var m7=Find(native.GetProperty("milestones"),"M7");
var wp=Find(m7.GetProperty("workPackages"),"M7-W01");
var parent=Find(wp.GetProperty("phases"),"M7-W01-C03");
var phase=Find(wp.GetProperty("phases"),"M7-W01-C03-S01");
var plot=Find(wp.GetProperty("phases"),"M7-W01-C03-S02");
RequireState(m7,"in-progress","state");
RequireState(wp,"in-progress","state");
RequireState(parent,"not-ready","state");
RequireState(phase,"in-progress","state");
RequireState(plot,"not-ready","state");
var nativeT1=Find(phase.GetProperty("tasks"),"M7-W01-C03-S01-T1");
var nativeT2=Find(phase.GetProperty("tasks"),"M7-W01-C03-S01-T2");
RequireState(nativeT1,target?"done":"not-ready","state");
RequireState(nativeT2,"not-ready","state");
for(var i=0;i<3;i++)
{
    var ending=((char)('A'+i)).ToString();
    var a="M7-W01-C03-S01-T1-"+ending;
    var b="M7-W01-C03-S01-T2-"+ending;
    RequireState(Find(t1.GetProperty("subtasks"),a),target?"done":"planned");
    RequireState(Find(nativeT1.GetProperty("subtasks"),a),target?"done":"not-ready","state");
    RequireState(Find(t2.GetProperty("subtasks"),b),"planned");
    RequireState(Find(nativeT2.GetProperty("subtasks"),b),"not-ready","state");
}

// Independently specified small pure diamond. The shared source is counted once.
DagNode[] diamond=[
    new("shared","constant",2,[]),
    new("left","add",3,["shared"]),
    new("right","multiply",4,["shared"]),
    new("root","sum",0,["left","right"])
];
var witness=Evaluate(diamond,"root");
Need(witness.Success&&witness.Status=="qualified"&&witness.Value==13&&
     witness.Evaluations==4&&witness.RootDepth==3&&
     witness.Values.Count==4&&
     witness.Values["shared"]==2&&witness.Values["left"]==5&&
     witness.Values["right"]==8,
     "Independent diamond DAG did not share one pure input and yield exact 13.");
Console.WriteLine("M7-DAG-ORACLE:DIAMOND:NODES=4;EDGES=4;VALUE=13;SHARED-EVALS=1;DEPTH=3");

void MustRefuse(string expected,DagNode[] graph,string root)
{
    var outcome=Evaluate(graph,root);
    Need(!outcome.Success&&outcome.Status==expected&&outcome.Evaluations==0&&
         outcome.Values.Count==0,
         "Expected exact zero-partial DAG refusal '"+expected+"', got "+outcome.Status);
}
MustRefuse("dependency-or-cycle",[
    new("a","add",1,["b"]),new("b","add",1,["a"])],"a");
MustRefuse("dependency-or-cycle",[new("a","add",1,["a"])],"a");
MustRefuse("dependency-or-cycle",[new("a","add",1,["missing"])],"a");
MustRefuse("duplicate-or-empty-id",[
    new("a","constant",1,[]),new("a","constant",2,[])],"a");
MustRefuse("node-budget",Enumerable.Range(0,9)
    .Select(i=>new DagNode("n"+i,"constant",i,[])).ToArray(),"n0");
MustRefuse("edge-budget",[
    new("one","constant",1,[]),
    new("two","constant",2,[]),
    new("s1","sum",0,["one","two"]),
    new("s2","sum",0,["one","two"]),
    new("s3","sum",0,["one","two"]),
    new("s4","sum",0,["one","two"]),
    new("s5","sum",0,["one","two"]),
    new("s6","sum",0,["one","two"])
],"s6");
MustRefuse("depth-budget",[
    new("a","constant",1,[]),new("b","add",0,["a"]),
    new("c","add",0,["b"]),new("d","add",0,["c"]),
    new("e","add",0,["d"])
],"e");
MustRefuse("unsupported-operation-shape",[new("bad","sum",0,["a"])],"bad");
MustRefuse("checked-overflow",[
    new("max","constant",long.MaxValue,[]),
    new("overflow","add",1,["max"])
],"overflow");
Console.WriteLine("M7-DAG-ORACLE:REFUSALS:CYCLE=true;UNKNOWN=true;DUPLICATE=true;NODES=8;EDGES=10;DEPTH=4;OVERFLOW=true;PARTIAL=false");

if(target)
{
    using var evidenceDoc=Open(Evidence);
    var e=evidenceDoc.RootElement;
    Need(Text(e,"status")==Status &&
         e.GetProperty("independent_oracle_groups").GetInt32()==2 &&
         e.GetProperty("reference_root_value").GetInt32()==13 &&
         e.GetProperty("reference_node_count").GetInt32()==4 &&
         e.GetProperty("reference_edge_count").GetInt32()==4 &&
         e.GetProperty("reference_shared_evaluations").GetInt32()==1 &&
         e.GetProperty("research_max_nodes").GetInt32()==MaximumNodes &&
         e.GetProperty("research_max_edges").GetInt32()==MaximumEdges &&
         e.GetProperty("research_max_depth").GetInt32()==MaximumDepth &&
         e.GetProperty("cycle_refused").GetBoolean() &&
         e.GetProperty("no_partial_result_on_refusal").GetBoolean() &&
         e.GetProperty("t1_done").GetBoolean()&&!e.GetProperty("t2_done").GetBoolean() &&
         !e.GetProperty("s01_phase_done").GetBoolean() &&
         !e.GetProperty("parent_c03_done").GetBoolean() &&
         !e.GetProperty("dag_runtime_api_delivered").GetBoolean() &&
         !e.GetProperty("aura_job_engine_copied").GetBoolean() &&
         !e.GetProperty("plot_renderer_delivered").GetBoolean() &&
         Text(e,"oracle_validator_sha256")==Hash(
             "docs/planning/ValidateM7DagAdmission.cs") &&
         Text(e,"decision_document_sha256")==Hash(Decision) &&
         Text(e,"owner_screen_sha256")==Hash(Screen),
         "Completed research target would claim unqualified DAG capability or drifted source.");
}
Console.WriteLine(Marker);
return 0;

sealed record DagNode(string Id,string Kind,long Operand,string[] Dependencies);
sealed record DagResult(bool Success,long Value,int Evaluations,int RootDepth,
    IReadOnlyDictionary<string,long> Values,string Status);
